using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using DarthVaderMod.Modules.Survivors;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace DarthVaderMod.SkillStates
{
    public class AnalyzeEffectController : MonoBehaviour
    {
        public CharacterBody charbody;
        private GameObject effectObj;
        public float timer;

        public void Start()
        {
            charbody = this.gameObject.GetComponent<CharacterBody>();
            effectObj = Object.Instantiate<GameObject>(Modules.Assets.rageAuraEffect, charbody.corePosition, Quaternion.LookRotation(charbody.characterDirection.forward));
            effectObj.transform.parent = charbody.gameObject.transform;
        }
        public void FixedUpdate()
        {

            if (timer > 1f)
            {
                Destroy(effectObj);
            }
        }

        public void OnDestroy()
        {
            Destroy(effectObj);
        }
    }
}