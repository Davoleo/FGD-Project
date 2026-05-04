using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{

    public enum CollectibleType
    {
        Coin,
        Key,
    }

    public struct Collectibles
    {
        public List<GameObject> coins;
        public List<GameObject> keys;
        //Dash Power
        //Bow
    }

    public class CollectiblesManager : MonoBehaviour
    {
        public Collectibles collectibles;

        public int coins;
        public int keys;

        private void Start()
        {
            collectibles.coins = new List<GameObject>();
            collectibles.coins.AddRange(GameObject.FindGameObjectsWithTag("Coins"));
            collectibles.keys.AddRange(GameObject.FindGameObjectsWithTag("Keys"));
        }

        public void Collect(CollectibleType type, GameObject collectible) {
            switch (type)
            {
                case CollectibleType.Coin:
                    coins++;
                    break;
                case CollectibleType.Key:
                    keys++;
                    break;
            }
        }
    }
}
