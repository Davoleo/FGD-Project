using UnityEngine;

namespace Projectiles
{
    public class Arrow : MonoBehaviour
    {
        [Header("Flight")]
        [SerializeField] float speed = 25f;
        [SerializeField] float maxRange = 40f;
        [SerializeField] float tipOffset = 0.45f;
        [SerializeField] LayerMask collisionMask;

        [Header("Stuck")]
        [SerializeField] float stuckLifetime = 15f;
        [SerializeField] Collider standingCollider;

        Vector3 _direction;
        float   _distanceTraveled;
        bool    _launched;

        void Update()
        {
            if (!_launched) return;

            float step = speed * Time.deltaTime;

            if (Physics.Raycast(transform.position, _direction, out RaycastHit hit, step + tipOffset, collisionMask))
            {
                Stick(hit.point);
                return;
            }

            transform.position += _direction * step;
            _distanceTraveled  += step;

            if (_distanceTraveled >= maxRange)
                Destroy(gameObject);
        }

        void Stick(Vector3 hitPoint)
        {
            transform.position = hitPoint - _direction * tipOffset;
            transform.forward  = _direction;

            if (standingCollider != null) standingCollider.enabled = true;
            enabled = false; // stops calling the Update() function
            Destroy(gameObject, stuckLifetime);
        }

        public void Launch(Vector3 direction)
        {
            _direction = direction.normalized;
            _launched  = true;
            if (standingCollider != null) standingCollider.enabled = false;
        }
    }
}
