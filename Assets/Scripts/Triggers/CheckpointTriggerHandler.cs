using Gameplay;
using Player;
using UnityEngine;

namespace Triggers
{
    public class CheckpointTriggerHandler : MonoBehaviour
    {
        private HealthManager healthManager;
        private CheckpointManager cpManager;

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            healthManager = player.GetComponent<HealthManager>();
            cpManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckpointManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            healthManager.Heal(HealthManager.MaxHealth);
            var offsetPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            cpManager.lastCheckPoint = offsetPos;
        }
    }
}