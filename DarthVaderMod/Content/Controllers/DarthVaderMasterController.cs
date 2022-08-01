//using EntityStates;
//using RoR2;
//using RoR2.Skills;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Networking;
//using R2API.Networking;
//using DarthVaderMod.SkillStates;

//namespace DarthVaderMod.Modules.Survivors
//{
//	[RequireComponent(typeof(CharacterBody))]
//	[RequireComponent(typeof(TeamComponent))]
//	[RequireComponent(typeof(InputBankTest))]
//	public class DarthVaderMasterController : MonoBehaviour
//	{
//		string prefix = DarthVader.DARTHVADER_PREFIX;

//		public DarthVaderMasterController DarthVadermastercon;
//		public DarthVaderController DarthVadercon;
//		private CharacterMaster characterMaster;
//		private CharacterBody characterBody;




//		public void Awake()
//		{

//			//On.RoR2.CharacterBody.Start += CharacterBody_Start;
			

//		}

//		//public void OnDestroy()
//		//{
//		//	On.RoR2.CharacterBody.Start -= CharacterBody_Start;
//		//}

//		public void Start()
//		{
//			characterMaster = gameObject.GetComponent<CharacterMaster>();
//			characterBody = characterMaster.GetBody();

//			DarthVadermastercon = characterMaster.gameObject.GetComponent<DarthVaderMasterController>();
//			DarthVadercon = characterBody.gameObject.GetComponent<DarthVaderController>();

//		}



//		public void FixedUpdate()
//		{

//		}

		

//	}
//}
