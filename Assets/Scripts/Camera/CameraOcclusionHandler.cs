using System;
using System.Collections.Generic;
using UnityEngine;

namespace Camera
{
    /// <summary>
    /// Fades world geometry that occludes the player from the camera.
    /// Attach to the PlayerCamera. Assign the Player transform in the Inspector.
    ///
    /// Works by casting multiple rays from the camera toward the player and
    /// several surrounding offset points, then smoothly fading any hit renderers
    /// to a very low alpha using runtime material instances (URP-compatible).
    /// Materials are restored and instances destroyed when occlusion clears.
    /// </summary>
    public class CameraOcclusionHandler : MonoBehaviour
    {
        
        private Transform player;

        [Header("Occlusion")]
        [Tooltip("Alpha applied to occluding objects (0 = invisible, 1 = opaque).")]
        [SerializeField] [Range(0f, 1f)] private float occludedAlpha = 0.12f;

        [Tooltip("Speed at which alpha fades in/out.")]
        [SerializeField] private float fadeSpeed = 10f;

        [Tooltip("Layers considered for occlusion")]
        [SerializeField] private LayerMask occlusionLayers = ~0;

        [Tooltip("Sphere radius for the broad sweep toward the player center. Larger = wider detection.")]
        [SerializeField] private float sweepRadius = 0.35f;

        // Precise ray targets in player-local space (feet → head + sides).
        // These catch objects that the sphere sweep might miss at the edges.
        private static readonly Vector3[] PlayerOffsets =
        {
            new(0f,    0f,   0f),   // feet / pivot
            new(0f,    0.5f, 0f),   // waist
            new(0f,    1.0f, 0f),   // chest
            new(0f,    1.6f, 0f),   // head
            new(0.5f,  0.9f, 0f),   // right shoulder
            new(-0.5f, 0.9f, 0f),   // left shoulder
            new(0f,    0.9f, 0.4f), // front
            new(0f,    0.9f,-0.4f), // back
        };

        private class FadeState
        {
            public Material[] origShared;
            public Material[] instances;
            public float alpha;         // current animated alpha of instances
            public bool occluding;
        }

        private readonly Dictionary<Renderer, FadeState> _states   = new();
        private readonly HashSet<Renderer>               _thisFrame = new();
        private readonly List<Renderer>                  _toRemove  = new();
        
        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        private void LateUpdate()
        {
            GatherOccluders();
            UpdateFades();
        }

        // ── Raycasting ────────────────────────────────────────────────────────

        private void GatherOccluders()
        {
            _thisFrame.Clear();
            Vector3 camPos = transform.position;

            // Broad sphere sweep toward player center — catches all walls/objects
            // along the line of sight regardless of how many layers deep they are.
            Vector3 toCenter = player.position - camPos;
            float   mainDist = toCenter.magnitude;
            if (mainDist > 0.01f)
                CastAndCollect(Physics.SphereCastAll(
                    camPos, sweepRadius, toCenter / mainDist, mainDist,
                    occlusionLayers, QueryTriggerInteraction.Ignore));

            // Precise rays to body points for coverage at the player's edges.
            foreach (Vector3 localOffset in PlayerOffsets)
            {
                Vector3 worldTarget = player.position + player.TransformDirection(localOffset);
                Vector3 dir         = worldTarget - camPos;
                float   dist        = dir.magnitude;
                if (dist < 0.01f) continue;

                CastAndCollect(Physics.RaycastAll(
                    camPos, dir / dist, dist,
                    occlusionLayers, QueryTriggerInteraction.Ignore));
            }
        }

        private void CastAndCollect(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits)
            {
                Transform t = hit.transform;
                if (t == player || t.IsChildOf(player)) continue;

                // GetComponentsInChildren includes the object itself and all descendants
                // — handles compound prefabs where collider and renderer are on different nodes.
                foreach (Renderer r in hit.collider.GetComponentsInChildren<Renderer>())
                    _thisFrame.Add(r);

                // Also walk up: the collider might live on a child while the mesh is on a parent.
                Renderer parentRend = hit.collider.GetComponentInParent<Renderer>();
                if (parentRend != null)
                    _thisFrame.Add(parentRend);
            }
        }

        // ── Fade management ───────────────────────────────────────────────────

        private void UpdateFades()
        {
            // Begin fading any newly occluded renderer
            foreach (Renderer r in _thisFrame)
            {
                if (r == null) continue;
                if (!_states.ContainsKey(r))
                    _states[r] = BeginFade(r);
            }

            // Advance all active fades
            _toRemove.Clear();
            float dt = Time.deltaTime;

            foreach (var (r, state) in _states)
            {
                if (r == null) { _toRemove.Add(r); continue; }

                state.occluding = _thisFrame.Contains(r);
                float target    = state.occluding ? occludedAlpha : 1f;
                state.alpha     = Mathf.MoveTowards(state.alpha, target, fadeSpeed * dt);

                ApplyAlpha(state);

                if (!state.occluding && Mathf.Approximately(state.alpha, 1f))
                {
                    RestoreAndCleanup(r, state);
                    _toRemove.Add(r);
                }
            }

            foreach (Renderer r in _toRemove)
                _states.Remove(r);
        }

        private static FadeState BeginFade(Renderer r)
        {
            Material[] shared    = r.sharedMaterials;
            Material[] instances = new Material[shared.Length];

            for (int i = 0; i < shared.Length; i++)
            {
                instances[i] = new Material(shared[i]);
                MakeUrpTransparent(instances[i]);
            }

            FadeState state = new()
            {
                origShared = shared,
                instances  = instances,
                alpha      = 1f,   // starts fully visible, then lerps down
                occluding  = true,
            };

            r.sharedMaterials = instances;
            return state;
        }

        private static void ApplyAlpha(FadeState state)
        {
            float a = state.alpha;
            foreach (Material m in state.instances)
            {
                if (m == null) continue;
                if (m.HasProperty("_BaseColor"))
                {
                    Color c = m.GetColor("_BaseColor");
                    c.a = a;
                    m.SetColor("_BaseColor", c);
                }
                // Legacy shader fallback
                if (m.HasProperty("_Color"))
                {
                    Color c = m.GetColor("_Color");
                    c.a = a;
                    m.SetColor("_Color", c);
                }
            }
        }

        private static void RestoreAndCleanup(Renderer r, FadeState state)
        {
            r.sharedMaterials = state.origShared;
            foreach (Material m in state.instances)
                if (m != null) Destroy(m);
        }

        // ── URP transparency helper ───────────────────────────────────────────

        private static void MakeUrpTransparent(Material mat)
        {
            if (mat.HasProperty("_Surface"))    mat.SetFloat("_Surface",   1f); // Transparent
            if (mat.HasProperty("_Blend"))      mat.SetFloat("_Blend",     0f); // Alpha blend
            if (mat.HasProperty("_AlphaClip"))  mat.SetFloat("_AlphaClip", 0f);
            if (mat.HasProperty("_ZWrite"))     mat.SetFloat("_ZWrite",    0f);

            if (mat.HasProperty("_SrcBlend"))
                mat.SetFloat("_SrcBlend",
                    (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            if (mat.HasProperty("_DstBlend"))
                mat.SetFloat("_DstBlend",
                    (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.DisableKeyword("_ALPHATEST_ON");
        }

        private void OnDestroy()
        {
            // Clean up any leftover instances if the camera is destroyed mid-play
            foreach (var (r, state) in _states)
            {
                if (r != null)
                    r.sharedMaterials = state.origShared;
                foreach (Material m in state.instances)
                    if (m != null) Destroy(m);
            }
            _states.Clear();
        }
    }
}
