using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace DarthVaderMod.Modules
{
    public static class Buffs
    {
        // spatialmovement armor buff 
        internal static BuffDef RageBuff;

        internal static void RegisterBuffs()
        {
            RageBuff = AddNewBuff("RageBuff", Assets.lightningBuffIcon, Color.red, false, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}