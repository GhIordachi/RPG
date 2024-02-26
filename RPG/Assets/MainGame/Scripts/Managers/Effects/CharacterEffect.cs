using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GI
{
    public class CharacterEffect : ScriptableObject
    {
        public int effectID;

        public virtual void ProcessEffect(CharacterManager character)
        {

        }
    }
}
