using System;
using Gameplay;
using UnityEngine;

namespace Player
{
    public class HealthManager : MonoBehaviour
    {
        private CheckpointManager cpManager;
        private int _health;
        public const int MaxHealth = 6;

        public int Health
        {
            get => _health;
            private set
            {
                _health = value;
                if (_health <= 0) HandleDeath();
            }
        }

        private void Start()
        {
            Health = MaxHealth;
            cpManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckpointManager>();
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(Health - damage, 0);
        }

        public void Heal(int heal)
        {
            Health = Math.Min(Health + heal, MaxHealth);
        }

        private void HandleDeath() => cpManager.Respawn();
    }
}
