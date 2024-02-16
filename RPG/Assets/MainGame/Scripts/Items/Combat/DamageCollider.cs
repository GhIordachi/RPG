using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GI
{
    public class DamageCollider : MonoBehaviour
    {
        public CharacterManager characterManager;
        protected Collider damageCollider;
        public bool enabledDamageColliderOnStartUp = false;

        [Header("Team I.D")]
        public int teamIDNumber = 0;

        [Header("Poise")]
        public float poiseBreak;
        public float offensivePoiseBonus;

        [Header("Damage")]
        public int physicalDamage;
        public int fireDamage;
        public int magicDamage;
        public int lightningDamage;
        public int darkDamage;

        protected bool shieldHasBeenHit;
        protected bool hasBeenParried;
        protected string currentDamageAnimation;

        protected virtual void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = enabledDamageColliderOnStartUp;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        protected virtual void OnTriggerEnter(Collider collision)
        {
            if(collision.tag == "Character")
            {
                shieldHasBeenHit = false;
                hasBeenParried = false;
                CharacterStatsManager enemyStats = collision.GetComponent<CharacterStatsManager>();
                CharacterManager enemyManager = collision.GetComponent<CharacterManager>();
                CharacterEffectsManager enemyEffects = collision.GetComponent<CharacterEffectsManager>();
                BlockingCollider shield = collision.transform.GetComponentInChildren<BlockingCollider>();

                if(enemyManager != null)
                {
                    if (enemyStats.teamIDNumber == teamIDNumber)
                        return;

                    CheckForParry(enemyManager);
                    CheckForBlock(enemyManager,shield,enemyStats);

                }


                if(enemyStats != null )
                {
                    if (enemyStats.teamIDNumber == teamIDNumber)
                        return;

                    if (hasBeenParried)
                        return;

                    if (shieldHasBeenHit) 
                        return;

                    enemyStats.poiseResetTimer = enemyStats.totalPoiseResetTime;
                    enemyStats.totalPoiseDefence = enemyStats.totalPoiseDefence - poiseBreak;

                    //Detects where the collider is hit by the weapon
                    Vector3 contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    float directionHitFrom = (Vector3.SignedAngle(characterManager.transform.forward, enemyManager.transform.forward, Vector3.up));
                    ChooseWhichDirectionDamageCameFrom(directionHitFrom);
                    enemyEffects.PlayBloodSplatterFX(contactPoint);

                    //Deals damage
                    DealDamage(enemyStats);
                }
            }

            

            if(collision.tag == "Illusionary Wall")
            {
                IllusionaryWall illusionaryWall = collision.GetComponent<IllusionaryWall>();

                illusionaryWall.wallHasBeenHit = true;
            }
        }

        protected virtual void CheckForParry(CharacterManager enemyManager)
        {
            if (enemyManager.isParrying)
            {
                //Check here if you are parryable
                characterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Parried", true);
                hasBeenParried = true;
            }
        }

        protected virtual void CheckForBlock(CharacterManager enemyManager, BlockingCollider shield, CharacterStatsManager enemyStats)
        {
            if (shield != null && enemyManager.isBlocking)
            {
                float physicalDamageAfterBlock = physicalDamage - (physicalDamage * shield.blockingPhysicalDamageAbsortion) / 100;
                float fireDamageAfterBlock = fireDamage - (fireDamage * shield.blockingFireDamageAbsortion) / 100;

                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), Mathf.RoundToInt(fireDamageAfterBlock), "Block Guard", characterManager);
                    shieldHasBeenHit = true;
                }
            }
        }

        protected virtual void DealDamage(CharacterStatsManager enemyStats)
        {
            float finalPhysicalDamage = physicalDamage;
            //If we are using the right weapon, we compare the right weapon modifiers
            if(characterManager.isUsingRightHand)
            {
                if (characterManager.characterCombatManager.currentAttackType == AttackType.light)
                {
                    finalPhysicalDamage = physicalDamage * characterManager.characterInventoryManager.rightWeapon.lightAttackDamageModifier;
                }
                else if (characterManager.characterCombatManager.currentAttackType == AttackType.heavy)
                {
                    finalPhysicalDamage = physicalDamage * characterManager.characterInventoryManager.rightWeapon.heavyAttackDamageModifier;
                }
            }
            //otherwise we compare the left weapon modifiers
            else if(characterManager.isUsingLeftHand)
            {
                if (characterManager.characterCombatManager.currentAttackType == AttackType.light)
                {
                    finalPhysicalDamage = physicalDamage * characterManager.characterInventoryManager.leftWeapon.lightAttackDamageModifier;
                }
                else if (characterManager.characterCombatManager.currentAttackType == AttackType.heavy)
                {
                    finalPhysicalDamage = physicalDamage * characterManager.characterInventoryManager.leftWeapon.heavyAttackDamageModifier;
                }
            }

            //Deal modified damage
            if (enemyStats.totalPoiseDefence > poiseBreak)
            {
                enemyStats.TakeDamageNoAnimation(Mathf.RoundToInt(finalPhysicalDamage), fireDamage);
            }
            else
            {
                enemyStats.TakeDamage(Mathf.RoundToInt(finalPhysicalDamage), fireDamage, currentDamageAnimation, characterManager);
            }
        }

        protected virtual void ChooseWhichDirectionDamageCameFrom(float direction)
        {
            Debug.Log(direction);
            if(direction >=145 && direction <= 180)
            {
                currentDamageAnimation = "Damage_Forward_01";
            }
            else if (direction <= -145 && direction >= -180)
            {
                currentDamageAnimation = "Damage_Forward_01";
            }
            else if(direction >= -45 && direction <= 45)
            {
                currentDamageAnimation = "Damage_Back_01";
            }
            else if(direction >= -144 && direction <= -45)
            {
                currentDamageAnimation = "Damage_Right_01";
            }
            else if(direction >= 45 && direction <= 144)
            {
                currentDamageAnimation = "Damage_Left_01";
            }
        }
    }
}
