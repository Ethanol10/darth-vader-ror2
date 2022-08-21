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

        //energy
        internal static float baseForceEnergy = 100f;
        internal static float levelForceEnergy = 10f;
        internal static float regenForceEnergyFraction = 0.025f;
        internal static float drainForceEnergyFraction = 0.8f;
        internal static float basemeleeForceEnergyGain = 1f;
        internal static float meleeOnHitForceEnergyGain = 10f;
        internal static float forcePushPullCost = 30f;
        internal static float deflectPerHitCost = 20f;
    }
}