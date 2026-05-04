using Controllers;
using UnityEngine;

namespace Triggers
{
    public class LadderCollisionHandler : MonoBehaviour
    {

        private PlayerCharacterController controller;

        private void Start()
        {
            controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (controller.CurrentState != CharacterState.Climbing)
                {
                    Debug.Log(controller.CurrentState);

                    // TODO: update character rotation to match ladder climbing
                    controller.TransitionToState(CharacterState.Climbing);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (controller.CurrentState == CharacterState.Climbing)
                {
                    controller.TransitionToState(controller.motor.GroundingStatus.IsStableOnGround ?
                        CharacterState.Grounded : CharacterState.Airborne);
                }
            }
        }
    }
}
