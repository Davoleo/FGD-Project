using System;
using Gameplay;
using UnityEngine;

namespace Triggers
{
    public class CollectibleTriggerHandler : MonoBehaviour
    {
        private CollectiblesManager manager;

        private void Start()
        {
            manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CollectiblesManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                gameObject.SetActive(false);

                CollectibleType type = gameObject.tag switch
                {
                    "Coins" => CollectibleType.Coin,
                    "Keys" => CollectibleType.Key,
                    _ => throw new ArgumentOutOfRangeException()
                };
                manager.Collect(type, gameObject);
            }
        }
    }
}