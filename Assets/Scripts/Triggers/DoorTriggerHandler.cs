using Gameplay;
using Player;
using UnityEngine;

namespace Triggers
{
    public class DoorTriggerHandler : MonoBehaviour
    {
        private CollectiblesManager manager;

        private PlayerInputHandler inputHandler = null;

        private void Start()
        {
            manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CollectiblesManager>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (inputHandler == null)
                    inputHandler = other.gameObject.GetComponent<PlayerInputHandler>();


                Debug.Log(inputHandler.interactAction.action.triggered + " " + manager.keys);
                if (inputHandler.interactAction.action.triggered && manager.UseKey())
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}