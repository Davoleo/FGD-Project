using UnityEngine;

namespace Player
{
    /// <summary>
    /// Snapshot of player input, collected each Update by PlayerInputHandler
    /// and consumed each FixedUpdate by PlayerCharacterController.
    /// </summary>
    public struct PlayerInputs
    {
        public Vector2 MoveInput;
        public Vector3 CameraForward;   // pre-flattened to the horizontal plane
        public Vector3 CameraRight;     // pre-flattened to the horizontal plane
        public bool    JumpPressed;
        public float   RotationInput;   // -1 = rotate left, 0 = none, +1 = rotate right
        public bool    DashPressed;     // placeholder — wire to a Dash action when ready
        public bool    ShootPressed;
        public Vector2 ClimbInput;
        public bool InteractPressed;
    }
}


