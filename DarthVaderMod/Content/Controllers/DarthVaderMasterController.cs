using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Networking;
using DarthVaderMod.SkillStates;

namespace DarthVaderMod.Modules.Survivors
{
	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(TeamComponent))]
	[RequireComponent(typeof(InputBankTest))]
	public class DarthVaderMasterController : MonoBehaviour
	{
		string prefix = DarthVader.DARTHVADER_PREFIX;

		public DarthVaderMasterController DarthVadermastercon;
		public DarthVaderController DarthVadercon;
		private CharacterMaster characterMaster;
		private CharacterBody characterBody;


		public bool alloyvulture;
		public bool alphacontruct;
		public bool beetle;
		public bool beetleguard;
		public bool blindpest;
		public bool blindvermin;
		public bool bison;
		public bool bronzong;
		public bool clayapothecary;
		public bool claytemplar;
		public bool greaterwisp;
		public bool gup;
		public bool hermitcrab;
		public bool imp;
		public bool jellyfish;
		public bool larva;
		public bool lemurian;
		public bool lesserwisp;
		public bool lunarexploder;
		public bool lunargolem;
		public bool lunarwisp;
		public bool minimushrum;
		public bool parent;
		public bool roboballminib;
		public bool stonegolem;
		public bool voidbarnacle;
		public bool voidjailer;
		public bool voidreaver;

		public bool beetlequeen;
		public bool claydunestrider;
		public bool grandparent;
		public bool grovetender;
		public bool impboss;
		public bool magmaworm;
		public bool overloadingworm;
		public bool scavenger;
		public bool soluscontrolunit;
		public bool stonetitan;
		public bool voiddevastator;
		public bool xiconstruct;



		public void Awake()
		{

			//On.RoR2.CharacterBody.Start += CharacterBody_Start;
			alloyvulture = false;
			 alphacontruct = false;
			 beetle = false;
			 beetleguard = false;
			 blindpest = false;
			 blindvermin = false;
			 bison = false;
			 bronzong = false;
			 clayapothecary = false;
			 claytemplar = false;
			 greaterwisp = false;
			 gup = false;
			 hermitcrab = false;
			 imp = false;
			 jellyfish = false;
			 larva = false;
			 lemurian = false;
			 lesserwisp = false;
			 lunarexploder = false;
			 lunargolem = false;
			 lunarwisp = false;
			 minimushrum = false;
			 parent = false;
			 roboballminib = false;
			 stonegolem = false;
			 voidbarnacle = false;
			 voidjailer = false;
			 voidreaver = false;

			 beetlequeen = false;
			 claydunestrider = false;
			 grandparent = false;
			 grovetender = false;
			 impboss = false;
			 magmaworm = false;
			 overloadingworm = false;
			 scavenger = false;
			 soluscontrolunit = false;
			 stonetitan = false;
			 voiddevastator = false;
			 xiconstruct = false;

		}

		//public void OnDestroy()
		//{
		//	On.RoR2.CharacterBody.Start -= CharacterBody_Start;
		//}

		public void Start()
		{
			characterMaster = gameObject.GetComponent<CharacterMaster>();
			characterBody = characterMaster.GetBody();

			DarthVadermastercon = characterMaster.gameObject.GetComponent<DarthVaderMasterController>();
			DarthVadercon = characterBody.gameObject.GetComponent<DarthVaderController>();

			alloyvulture = false;
			alphacontruct = false;
			beetle = false;
			beetleguard = false;
			blindpest = false;
			blindvermin = false;
			bison = false;
			bronzong = false;
			clayapothecary = false;
			claytemplar = false;
			greaterwisp = false;
			gup = false;
			hermitcrab = false;
			imp = false;
			jellyfish = false;
			larva = false;
			lemurian = false;
			lesserwisp = false;
			lunarexploder = false;
			lunargolem = false;
			lunarwisp = false;
			minimushrum = false;
			parent = false;
			roboballminib = false;
			stonegolem = false;
			voidbarnacle = false;
			voidjailer = false;
			voidreaver = false;

			beetlequeen = false;
			claydunestrider = false;
			grandparent = false;
			grovetender = false;
			impboss = false;
			magmaworm = false;
			overloadingworm = false;
			scavenger = false;
			soluscontrolunit = false;
			stonetitan = false;
			voiddevastator = false;
			xiconstruct = false;
		}



		public void FixedUpdate()
		{

		}

		

	}
}
