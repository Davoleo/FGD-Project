using Controllers;
using UnityEngine;

namespace Gameplay
{
    public class CheckpointManager : MonoBehaviour
    {

        private GameObject player;
        public Vector3 lastCheckPoint = new Vector3(0, 2, 0);
        private PlayerCharacterController playerController;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerCharacterController>();
        }

        public void Respawn()
        {
            playerController.motor.SetPosition(lastCheckPoint);
        }

    }
}