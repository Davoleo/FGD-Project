using Gameplay;
using Player;
using UnityEngine;

namespace Triggers
{
    public class DoorTriggerHandler : MonoBehaviour
    {
        private CollectiblesManager manager;
        private PlayerInputHandler inputHandler = null;

        private bool isClose;

        private void Start()
        {
            manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CollectiblesManager>();
        }

        private void Update()
        {
            if (isClose)
            {
                Debug.Log(inputHandler.interactAction.action.triggered);
                if (inputHandler.interactAction.action.triggered && manager.UseKey())
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (inputHandler == null)
                    inputHandler = other.gameObject.GetComponent<PlayerInputHandler>();

                Debug.Log("Exit");
                isClose = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (inputHandler == null)
                    inputHandler = other.gameObject.GetComponent<PlayerInputHandler>();

                Debug.Log("Enter");
                isClose = true;
            }
        }
    }
}