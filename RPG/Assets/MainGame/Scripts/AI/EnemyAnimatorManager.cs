using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GI
{
    public class EnemyAnimatorManager : CharacterAnimatorManager
    {
        EnemyManager enemyManager;
        EnemyEffectsManager enemyEffectsManager;
        EnemyBossManager enemyBossManager;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            enemyManager = GetComponent<EnemyManager>();
            enemyEffectsManager = GetComponent<EnemyEffectsManager>();
            enemyBossManager = GetComponent<EnemyBossManager>();
        }

        public void AwardSoulsOnDeath()
        {
            //Scan for the player in the scene and award him
            PlayerStatsManager playerStats = FindObjectOfType<PlayerStatsManager>();
            SoulCountBar soulCountBar = FindObjectOfType<SoulCountBar>();

            if (playerStats != null)
            {
                playerStats.AddSouls(characterStatsManager.souldAwardedOnDeath);

                if (soulCountBar != null)
                {
                    soulCountBar.SetSoulCountText(playerStats.soulCount);
                }
            }
        }

        public void InstantiateBossParticleFX()
        {
            BossFXTransform bossFXTransform = GetComponentInChildren<BossFXTransform>();

            GameObject phaseFX = Instantiate(enemyBossManager.particleFX, bossFXTransform.transform);
        }

        public void PlayWeaponTrailFX()
        {
            enemyEffectsManager.PlayWeaponFX(false);
        }

        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidBody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidBody.velocity = velocity;

            if(enemyManager.isRotatingWithRootMotion)
            {
                enemyManager.transform.rotation *= animator.deltaRotation;
            }
        }
    }
}
