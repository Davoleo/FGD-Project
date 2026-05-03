using System;
using UnityEngine;

namespace Collisions
{
    public class LadderCollisionHandler : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("collided with ladder");
        }
    }
}
