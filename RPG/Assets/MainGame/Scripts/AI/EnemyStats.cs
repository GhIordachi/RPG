using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GI {
    public class EnemyStats : CharacterStats
    {        
        EnemyAnimatorManager enemyAnimatorManager;

        public UIEnemyHealthBar healthBar;

        public int souldAwardedOnDeath = 50;

        private void Awake()
        {
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamageNoAnimation(int damage)
        {
            currentHealth = currentHealth - damage;

            healthBar.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }
        }

        public override void TakeDamage(int damage, string damageAnimation = "Damage_01")
        {
            if (isDead)
                return;

            currentHealth = currentHealth - damage;
            healthBar.SetHealth(currentHealth);

            enemyAnimatorManager.PlayTargetAnimation(damageAnimation, true);

            if(currentHealth <= 0)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            currentHealth = 0;
            enemyAnimatorManager.PlayTargetAnimation("Death_01", true);
            isDead = true;
        }
    }
}
