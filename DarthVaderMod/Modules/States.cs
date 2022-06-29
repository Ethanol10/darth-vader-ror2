using DarthVaderMod.SkillStates;
using DarthVaderMod.SkillStates.BaseStates;

namespace DarthVaderMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(SpawnState));


            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));

            Modules.Content.AddEntityState(typeof(SlashCombo));
            Modules.Content.AddEntityState(typeof(Force));
            Modules.Content.AddEntityState(typeof(RageMode));

        }
    }
}