using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_GuildBossBattlefield : NGUIChildGUI  
{
	public bool				showBattle				= false;

	public UILabel			LabelGuildBattlefieldTitle	= null;

	//左邊
	public UITexture		TextureRoleLeft			= null;

	public Transform		TransformLeftInfo		= null;

	public UISprite			SpriteRoleLeftInfo		= null;
	public UILabel			LabelRoleLeftName		= null;
	public UISprite			SpriteRoleLeftHP		= null;
	public UILabel			LabelRoleLeftHP			= null;
	public UISprite			SpriteRoleLeftPower		= null;
	public UILabel			LabelRoleLeftPower		= null;
	public UISprite			SpriteRoleLeftAtk		= null;
	public UILabel			LabelRoleLeftAtk		= null;
	public UISprite			SpriteRoleLeftDef		= null;
	public UILabel			LabelRoleLeftDef		= null;

	public UILabel			LabelLefttHurtValue		= null;
	public UIProgressBar	ProgressBarLeft			= null;

	//右邊
	public UITexture		TextureRoleRight		= null;

	public Transform		TransformRightInfo		= null;

	public UISprite			SpriteRoleRightInfo		= null;
	public UILabel			LabelRoleRightName		= null;
	public UISprite			SpriteRoleRightHP		= null;
	public UILabel			LabelRoleRightHP		= null;
	public UISprite			SpriteRoleRightPower	= null;
	public UILabel			LabelRoleRightPower		= null;
	public UISprite			SpriteRoleRightAtk		= null;
	public UILabel			LabelRoleRightAtk		= null;
	public UISprite			SpriteRoleRightDef		= null;
	public UILabel			LabelRoleRightDef		= null;

	public UILabel			LabelRightHurtValue		= null;
	public UIProgressBar	ProgressBarRight		= null;

	//寵物列
	public UISprite			SpritePetListBG			= null;
	public UISprite			SpritePetListFrame		= null;
	public UIGrid			GridPetList				= null;

	public UISprite			SpriteFight				= null;

	//動態
	public	Animation		AnimationEnter			= null;
//	public	Animation		AnimationLeftAtk		= null;
//	public	Animation		AnimationRightAtk		= null;

	public UILabel			LabelTotalDamage		= null;
	public TweenScale		tweenTotalDamage		= null;

	//
	public UIButton			ButtonSkip				= null;

	//slot
	string	slotName	= "";
	public List<Slot_GuildBattlefield> slotList		= new List<Slot_GuildBattlefield>();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_GuildBossBattlefield";

	//temp
	int i = 0;
	S_PetData_Tmp petDBF 	= null;
//	GuildBaseData guildData = null;
	string animationName	= null;

	//-------------------------------------------------------------------------------------------------
	private UI_GuildBossBattlefield() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{	
		//左邊
		LabelGuildBattlefieldTitle.text	= "";
		LabelRoleLeftName.text		= "";
		LabelRoleLeftHP.text		= "";
		LabelRoleLeftPower.text		= "";
		LabelRoleLeftAtk.text		= "";
		LabelRoleLeftDef.text		= "";
		LabelLefttHurtValue.text	= "";

		//右邊
		LabelRoleRightName.text		= "";
		LabelRoleRightHP.text		= "";
		LabelRoleRightPower.text	= "";
		LabelRoleRightAtk.text		= "";
		LabelRoleRightDef.text		= "";
		LabelRightHurtValue.text	= "";

		LabelTotalDamage.text		= "";

		CreatSlot();
	}

	//-------------------------------------------------------------------------------------------------
	void CreatSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_GuildBattlefield"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_GuildBattlefield go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_GuildBattlefield>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_GuildBossBattlefield load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}

		// GuildList
		for(int i=0; i<GameDefine.GUILDWAR_PET_PARTY_MAX; ++i) 
		{
			Slot_GuildBattlefield newgo	= Instantiate(go) as Slot_GuildBattlefield;
			
			newgo.transform.parent			= GridPetList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("slot{0:00}",i);
//			newgo.btnActivity.userData = i;
			
			newgo.InitialSlot();
			newgo.gameObject.SetActive(false);
			slotList.Add(newgo);
		}
	}
	
	//-------------------------------------------------------------------------------------------------
	//設定戰鬥寵物圖像
	public void SetBattlePetIcon()
	{
		//寵物圖像
		int[] petID = ARPGApplication.instance.m_GuildSystem.GetBossBattlePet();

		for(int i=0; i<slotList.Count; ++i)
		{	
			petDBF = null;

			if(petID.Length > i)
			{
				petDBF = GameDataDB.PetDB.GetData(petID[i]);
				if(petDBF != null)
				{
					slotList[i].gameObject.SetActive(true);
					Utility.ChangeAtlasSprite(slotList[i].SpritePetIcon, petDBF.AvatarIcon);
				}
				else
				{
					slotList[i].gameObject.SetActive(false);
					Utility.ChangeAtlasSprite(slotList[i].SpritePetIcon, -1);
				}
			}
			else
			{
				slotList[i].gameObject.SetActive(false);
				Utility.ChangeAtlasSprite(slotList[i].SpritePetIcon, -1);
			}
		}
	}

	//-------------------------------------------------------------------------------------------------
	//設定初始戰鬥資訊
	public void SetBattleBeginInfo()
	{
		GuildBaseData guildData = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		GuildBattleResult resultData = ARPGApplication.instance.m_GuildSystem.GetGuildBattleResult();
//		S_GuildBossData bossData = ARPGApplication.instance.m_GuildSystem.GetGuildBossDataByBossID(resultData.iGuildBossID);
		S_GuildWars_Tmp guildWarTmp = GameDataDB.GuildWarsDB.GetData(GameDefine.GUILDWAR_DBF_GUID);
		if (guildWarTmp != null)
			return;
		//玩家方BUFF
		if(guildData == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_GuildBossBattlefield GetGuildBaseData error") );

			LabelRoleLeftAtk.text 	= "0";
			LabelRoleLeftDef.text	= "0";
		}
		else
		{
			int buffAtk = (int)(guildData.iATKBuffLv*guildWarTmp.fInspirePerValue*100);
			LabelRoleLeftAtk.text 	= buffAtk.ToString();
			LabelRoleLeftDef.text	= guildData.iDEFBuffLv.ToString();
		}

		//boss方BUFF
		if(resultData == null)
		{
			LabelRoleRightAtk.text 	= "0";
			LabelRoleRightDef.text	= "0";
		}
		else
		{
			LabelRoleRightAtk.text 	= resultData.iBossAtkLv.ToString();
			LabelRoleRightDef.text	= resultData.iBossDefLv.ToString();
		}

		//BOSS圖和名子
		S_GuildBossValue_Tmp bossTmp = GameDataDB.GuildBossValueDB.GetData(resultData.iGuildBossID);
		if(bossTmp == null)
		{
			LabelRoleRightName.text	= "";
			Utility.ChangeTexture(TextureRoleRight, -1);
		}
		else
		{
			LabelRoleRightName.text	= GameDataDB.GetString(bossTmp.iGuildBossName);
			Utility.ChangeTexture(TextureRoleRight, bossTmp.iGuildBossFullAvatar);
		}
		//boss原本的血量
		LabelRoleRightHP.text 		= resultData.iBossOriginalHP.ToString();


		
	}

	//-------------------------------------------------------------------------------------------------
/*	//設定初始戰鬥資訊
	public void SetBattleBeginInfo()
	{

	}
*/
	//-------------------------------------------------------------------------------------------------
	//設定戰力
	public void SetPetPower(int power)
	{
		LabelRoleLeftPower.text = power.ToString();
	}

	//-------------------------------------------------------------------------------------------------
	public void SetPetNameHP(int dbID, int hp)
	{
		petDBF = null;

		petDBF = GameDataDB.PetDB.GetData(dbID);
		if(petDBF != null)
		{
			LabelRoleLeftName.text = GameDataDB.GetString(petDBF.iName);

			LabelRoleLeftHP.text = hp.ToString();

			Utility.ChangeTexture(TextureRoleLeft, petDBF.FullAvatar);
		}
	}
/*
	//-------------------------------------------------------------------------------------------------
	public float PlayAnimation(int type)
	{
		AnimationEnter.Stop();
		AnimationLeftAtk.Stop();
		AnimationRightAtk.Stop();

		switch(type)
		{
		case GameDefine.GUILDWAR_BATTLE_ANITYPE_ENTER:
			AnimationEnter.Play();
			return AnimationEnter[AnimationEnter.name].length;
			break;
		case GameDefine.GUILDWAR_BATTLE_ANITYPE_LATK:
			AnimationLeftAtk.Play();
			return AnimationLeftAtk[AnimationLeftAtk.name].length;
			break;
		case GameDefine.GUILDWAR_BATTLE_ANITYPE_RATK:
			AnimationRightAtk.Play();
			return AnimationRightAtk[AnimationRightAtk.name].length;
			break;
		default:
			break;
		}

		return 0;
	}
*/	
	//-------------------------------------------------------------------------------------------------
	public float PlayAnimation(string type)
	{
		AnimationEnter.Stop();

		if(AnimationEnter[type] != null)
		{
			AnimationEnter.Play(type);

			UnityDebugger.Debugger.Log(type + ":  "+AnimationEnter[type].length);

			return AnimationEnter[type].length;
		}
		else
		{
			UnityDebugger.Debugger.LogError( string.Format("{0} == null", type));
			return 0;
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetAtkInfo()
	{

	}

	//-------------------------------------------------------------------------------------------------
	public bool CheckShowBattle()
	{
		return showBattle;
	}
}
