using Controllers;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private PlayerCharacterController characterController;
        [SerializeField] private Animator animator;

        private static readonly int SpeedHash          = Animator.StringToHash("Speed");
        private static readonly int GroundedHash       = Animator.StringToHash("Grounded");
        private static readonly int AirborneHash       = Animator.StringToHash("Airborne");
        private static readonly int DashHash           = Animator.StringToHash("Dash");
        private static readonly int JumpHash           = Animator.StringToHash("Jump");
        private static readonly int VerticalSpeedHash  = Animator.StringToHash("VerticalSpeed");

        private void OnEnable()
        {
            characterController.OnJumped += TriggerJump;
        }

        private void OnDisable()
        {
            characterController.OnJumped -= TriggerJump;
        }

        private void TriggerJump()
        {
            animator.SetTrigger(JumpHash);
        }

        private void Update()
        {
            bool isGrounded = characterController.IsGrounded;
            animator.SetFloat(SpeedHash,         characterController.ForwardSpeed);
            animator.SetFloat(VerticalSpeedHash, characterController.VerticalSpeed);
            animator.SetBool(GroundedHash,       isGrounded);
            animator.SetBool(AirborneHash,       !isGrounded);
            animator.SetBool(DashHash,           characterController.CurrentState == CharacterState.Dashing);
        }
    }
}
