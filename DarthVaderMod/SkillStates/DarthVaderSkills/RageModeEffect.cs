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
    public class RageEffectController : MonoBehaviour
    {
        public CharacterBody charbody;
        private GameObject effectObj;
        public float timer;

        public void Start()
        {
            charbody = this.gameObject.GetComponent<CharacterBody>();
            effectObj = Object.Instantiate<GameObject>(Modules.Assets.rageAuraEffect, charbody.footPosition, Quaternion.LookRotation(Vector3.up));
            effectObj.transform.parent = charbody.gameObject.transform;
        }
        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer > 2f)
            {
                Destroy(effectObj);
                Destroy(this);
            }

            if (!charbody)
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