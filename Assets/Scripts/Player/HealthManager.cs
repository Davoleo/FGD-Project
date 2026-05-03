using System;
using UnityEngine;

namespace Player
{
    public class HealthManager : MonoBehaviour
    {
        private int _health;
        public const int MaxHealth = 6;

        public int Health
        {
            get => _health;
            private set => _health = value;
        }
    
        private void Start()
        {
            Health = MaxHealth;
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(Health - damage, 0);
        }
    
        public void Heal(int heal)
        {
            Health = Math.Min(Health + heal, MaxHealth);
        }
    }
}
