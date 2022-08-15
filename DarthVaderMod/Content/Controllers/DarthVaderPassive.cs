using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace DarthVaderMod.Content.Controllers
{
    public class DarthVaderPassive : MonoBehaviour
    {
		public SkillDef normalCooldownPassive;
		public SkillDef energyPassive;
		public GenericSkill passiveSkillSlot;

		public bool isEnergyPassive()
		{
			if (energyPassive && this.passiveSkillSlot) 
			{
				return this.passiveSkillSlot.skillDef == energyPassive;
			}

			return false;
		}
	}
}
