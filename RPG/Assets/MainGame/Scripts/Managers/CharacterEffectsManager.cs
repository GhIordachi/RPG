using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GI
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Static Effects")]
        [SerializeField] List<StaticCharacterEffect> staticCharacterEffects;

        [Header("Current FX")]
        public GameObject instantiatedFXModel;

        [Header("Damage FX")]
        public GameObject bloodSplatterFX;

        [Header("Weapon FX")]
        public WeaponFX rightWeaponFX;
        public WeaponFX leftWeaponFX;

        [Header("Poison")]
        public GameObject defaultPoisonParticleFX;
        public GameObject currentPoisonParticleFX;
        public Transform buildUpTransform; //The location of build up particle FX that will spawn
        public bool isPoisoned;
        public float poisonBuildup = 0; //The build up over time that poisons the player after reaching 100
        public float poisonAmount = 100; //The amount of poison the player has to process before becoming unpoisoned
        public float defaultPoisonAmount = 100; //The default amount of poison a player has to process once they become poisoned
        public float poisonTimer = 2; //The amount of time between each poison damage Tick
        public int poisonDamage = 1;
        float timer;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        private void Start()
        {
            foreach(var effect in staticCharacterEffects)
            {
                effect.AddStaticEffect(character);
            }
        }

        public void AddStaticEffect(StaticCharacterEffect effect)
        {
            StaticCharacterEffect staticEffect;
            for(int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if (staticCharacterEffects[i] != null)
                {
                    if (staticCharacterEffects[i].effectID == effect.effectID)
                    {
                        staticEffect = staticCharacterEffects[i];
                        //We remove the actual effect from our character
                        staticEffect.RemoveStaticEffect(character);
                        //We remove the effect from the list of active effects
                        staticCharacterEffects.Remove(staticEffect);
                    }
                }
            }

            //We add the effect to our list of active effects
            staticCharacterEffects.Add(effect);
            //We add the actual effect to our character
            effect.AddStaticEffect(character);

            //Check the list for null items and remove them
            for( int i = staticCharacterEffects.Count -1; i > -1; i--)
            {
                if (staticCharacterEffects[i] == null)
                {
                    staticCharacterEffects.RemoveAt(i);
                }
            }
        }

        public void RemoveStaticEffect(int effectID)
        {
            StaticCharacterEffect staticEffect;

            for(int i = staticCharacterEffects.Count - 1;i > -1; i--)
            {
                if (staticCharacterEffects[i] != null)
                {
                    if (staticCharacterEffects[i].effectID == effectID)
                    {
                        staticEffect = staticCharacterEffects[i];
                        //We remove the actual effect from our character
                        staticEffect.RemoveStaticEffect(character);
                        //We remove the effect from the list of active effects
                        staticCharacterEffects.Remove(staticEffect);
                    }
                }
            }

            //Check the list for null items and remove them
            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if (staticCharacterEffects[i] == null)
                {
                    staticCharacterEffects.RemoveAt(i);
                }
            }
        }

        public virtual void PlayWeaponFX(bool isLeft)
        {
            if(!isLeft)
            {
                if(rightWeaponFX != null)
                {
                    rightWeaponFX.PlayWeaponFX();
                }
            }
            else
            {
                if(leftWeaponFX != null)
                {
                    leftWeaponFX.PlayWeaponFX();
                }
            }
        }

        public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterLocation)
        {
            GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterLocation, Quaternion.identity);
        }

        public virtual void HandleAllBuildUpEffects()
        {
            if(character.isDead) 
                return;

            HandlePoisonBuildUp();
            HandleIsPoisonedEffect();
        }

        protected virtual void HandlePoisonBuildUp()
        {
            if (isPoisoned)
                return;

            if(poisonBuildup > 0 && poisonBuildup < 100)
            {
                poisonBuildup = poisonBuildup -1 * Time.deltaTime;
            }
            else if (poisonBuildup >= 100)
            {
                isPoisoned = true;
                poisonBuildup = 0;

                if(buildUpTransform != null)
                {
                    currentPoisonParticleFX = Instantiate(defaultPoisonParticleFX, buildUpTransform.transform);
                }
                else
                {
                    currentPoisonParticleFX = Instantiate(defaultPoisonParticleFX, character.transform);
                }
            }
        }

        protected virtual void HandleIsPoisonedEffect()
        {
            if (isPoisoned)
            {
                if(poisonAmount > 0)
                {
                    timer += Time.deltaTime;

                    if(timer >= poisonTimer)
                    {
                        character.characterStatsManager.TakePoisonDamage(poisonDamage);
                        timer = 0;
                    }
                    poisonAmount = poisonAmount - 1 * Time.deltaTime;
                }
                else
                {
                    isPoisoned = false;
                    poisonAmount = defaultPoisonAmount;
                    Destroy(currentPoisonParticleFX);
                }
            }
        }

        public virtual void InteruptEffect()
        {
            //Can be used to destroy effects models (Drinking potions, arrows etc.)
            if(instantiatedFXModel != null)
            {
                Destroy(instantiatedFXModel);
            }

            //Firesthe character's bow and removes the arrow
            if(character.isHoldingArrow)
            {
                character.animator.SetBool("isHoldingArrow", false);
                Animator rangedWeaponAnimator = character.characterWeaponSlotManager.rightHandSlot.curentWeaponModel.GetComponentInChildren<Animator>();

                if(rangedWeaponAnimator != null)
                {
                    rangedWeaponAnimator.SetBool("isDrawn", false);
                    rangedWeaponAnimator.Play("Bow_TH_Fire_01");
                }
            }

            //Removes player from aiming state
            if (character.isAiming)
            {
                character.animator.SetBool("isAiming", false);
            }
        }
    }
}
