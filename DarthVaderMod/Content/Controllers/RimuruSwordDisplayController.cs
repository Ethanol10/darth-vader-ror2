using UnityEngine;

namespace DarthVaderMod.Modules.Survivors
{
    internal class DarthVaderSwordDisplayController : MonoBehaviour
    {
        public Transform swordTargetTransform;
        public Transform swordTransform;


        public void Update()
        {
            if (swordTargetTransform && swordTargetTransform)
            {
                swordTransform.position = swordTargetTransform.position;
                swordTransform.rotation = swordTargetTransform.rotation;
            }
        }

    }
}