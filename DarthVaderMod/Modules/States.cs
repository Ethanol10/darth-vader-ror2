using DarthVaderMod.SkillStates;
using DarthVaderMod.SkillStates.BaseStates;

namespace DarthVaderMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(SpawnState));

            Modules.Content.AddEntityState(typeof(Devour));

            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(DashAttack));
            Modules.Content.AddEntityState(typeof(DashAttackExit));
            Modules.Content.AddEntityState(typeof(DarthVaderPrimary));
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(BlackLightning));

            Modules.Content.AddEntityState(typeof(SpatialMovement));

            Modules.Content.AddEntityState(typeof(Transform));
            Modules.Content.AddEntityState(typeof(TransformSlime));

            Modules.Content.AddEntityState(typeof(Waterblade));
        }
    }
}