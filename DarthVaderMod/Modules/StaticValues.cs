using System;
using UnityEngine;

namespace DarthVaderMod.Modules
{
    internal static class StaticValues
    {
        //melee attacks
        internal const float swordDamageCoefficient = 2f;

        //Force
        internal const float forcepushDamageCoefficient = 3.5f;
        internal const float forcepullDamageCoefficient = 4f;
        internal static float forceMaxRange = 100f;
        internal static float forceMaxTrackingAngle = 30f;

        //ragemode
        internal const int ragebuffDuration = 15;

        //deflect
        internal const int deflectbuffDuration = 6;
    }
}