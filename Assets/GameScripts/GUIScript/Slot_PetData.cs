using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

public class Slot_PetData : MonoBehaviour {
	
	//指定變動項目
	public UISprite[]				spriteRankStars		= new UISprite[7];		//寵物星級

	public UILabel					lbLevelTitle		= null;
	public UILabel					lbBloodTitle		= null;
	public UILabel					lbAttTitle			= null;

	public UILabel					lbPetName			= null;		//寵物名稱
	public UILabel					lbTypeName			= null;  	//職別名稱
	public UISprite					spCalss				= null;		//屬性圖
	public UILabel					lbPetLevel			= null;		//寵物等級
	public UILabel					lbPetBlood			= null;		//寵物血量
	public UILabel					lbPetAttack			= null;		//寵物攻擊力
	public UISprite					spritePetBorder		= null;		//寵物純框
	public UISprite					spritePetInfoBG		= null;		//寵物資訊框(金，銀，灰)
	public UIButton					btnPetInfo			= null;		//寵物資訊按鈕

	public UISprite					spInfoMask			= null;
	public UISprite					spInfoBG			= null;
	public UILabel					lbInfo				= null;

	//公會BOSS專用
	//public UILabel					lbBattleNumber		= null;
	//onClick事件發生時會變動的項目
	public UISprite					spriteBorder 		= null;		//被選擇的邊框
	public UISprite 				backgroundSprite;
	//控制變數
	public bool						isSelected			= false;	//是否被選擇
	[HideInInspector]
	public bool						isSetBattle			= false;	//是否被指定為出戰
	[HideInInspector]
	public int						iDBFID				= 0;		//紀錄此寵物的DBFID
	//
	private BoxCollider thisCollider;

	//-------------------------------------------------------------------------------------------------
	public void SetSlot(S_PetData petdata,ENUM_FormationNodeAbility AttrType = ENUM_FormationNodeAbility.Null)
	{
		//蒐集應顯示資訊
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(petdata.iPetDBFID);
		if(pdTmp == null)
			return;
		int[] PetInfoSet = new int[5];
		PetInfoSet = CollectEachUnlockPetInfo(petdata);

		//初始文字
		lbLevelTitle.text	= GameDataDB.GetString(270);
		lbBloodTitle.text 	= GameDataDB.GetString(271);
		//第三項依需求變換呈現素質
		lbAttTitle.text 	= GetArrtName(AttrType);	
				//寵物圖像更換
		Utility.ChangeAtlasSprite(backgroundSprite,PetInfoSet[(int)Enum_PetInfoArray.Sprite2D]);
		//資訊更換
		lbPetName.text 		= GameDataDB.GetString(PetInfoSet[(int)Enum_PetInfoArray.Name]);
		pdTmp.SetRareColorString(lbPetName,true);
		lbPetLevel.text 	= petdata.iPetLevel.ToString ();
		lbTypeName.text		= GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(pdTmp.emCharType));
		Utility.ChangeAtlasSprite(spCalss,ARPGApplication.instance.GetPetCalssIconID(pdTmp.emCharClass));
		//
		//設定計算數值
		S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,petdata.iPetDBFID);
		//S_FormationData formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
		//List<S_PetData> sPet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData;
		S_RoleAttr PetAttrValue = ARPGCharacter.CreatePetRoleAttr(petdata,EQList,null,petdata.GetTalentSkill(),null,null,null);
		int iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fMaxHP,0,MidpointRounding.AwayFromZero));
		lbPetBlood.text		= iAtrValue.ToString();
		//第三項依需求變換呈現素質
		iAtrValue = GetAttrValue(PetAttrValue,AttrType);
		lbPetAttack.text		= iAtrValue.ToString();

		iDBFID					= petdata.iPetDBFID;
		//設定星等產生數與資訊框選擇
		//設定星等
		for(int i=0;i<spriteRankStars.Length;++i)
		{
			if(i<petdata.iPetLimitLevel)
				spriteRankStars[i].gameObject.SetActive(true);
			else
				spriteRankStars[i].gameObject.SetActive(false);
		}
		
		//選擇使用哪個資訊框
		int RealRank = ARPGApplication.instance.GetPetRealRankWithLimitBreak(petdata.iPetDBFID);
		if(RealRank>0 && RealRank<4)
		{
			Utility.ChangeAtlasSprite(spritePetInfoBG,1010);	//夥伴普卡1~3星用資訊框
			Utility.ChangeAtlasSprite(spritePetBorder,1006);	//夥伴普卡1~3星用純框
		}
		if(RealRank>3 && RealRank<6)
		{
			Utility.ChangeAtlasSprite(spritePetInfoBG,1009);	//夥伴銀卡4~5星用資訊框
			Utility.ChangeAtlasSprite(spritePetBorder,1005);	//夥伴普卡1~3星用純框
		}
		if(RealRank>5 && RealRank<=7)
		{
			Utility.ChangeAtlasSprite(spritePetInfoBG,1008);	//夥伴金卡6~7星用資訊框
			Utility.ChangeAtlasSprite(spritePetBorder,1004);	//夥伴普卡1~3星用純框
		}
		if(RealRank<0 || RealRank>7)
			return;												//非正常狀況時
		//先隱藏選擇框
		spriteBorder.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 寵物資訊蒐集(2D圖，星等，名稱，血量，攻擊力) </summary>
	public int[] CollectEachUnlockPetInfo(S_PetData pd)
	{
		if (pd == null)
			return null;
		int[] InfoSet = new int[5]; 
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(pd.iPetDBFID);
		InfoSet[0]= pdTmp.Texture;		//2D圖
		InfoSet[1]= pdTmp.iRank;		//星等
		InfoSet[2]= pdTmp.iName;			//名稱
		int PlusHP = (int)pdTmp.fMaxHP_UP*(pd.iPetLevel-1);
		InfoSet[3]= pdTmp.sAttrTable.iMaxHP + PlusHP;			//血量
		int PlusATK = (int)pdTmp.fAttack_UP*(pd.iPetLevel-1);
		InfoSet[4]= pdTmp.sAttrTable.iAttack + PlusATK;			//攻擊力
		return InfoSet;
	}
	//-------------------------------------------------------------------------------------------------
	//取得特定屬性名稱
	private string GetArrtName(ENUM_FormationNodeAbility AttrType)
	{
		switch (AttrType) 
		{		
		case ENUM_FormationNodeAbility.Armor:
			return GameDataDB.GetString(7101);	//物防
		case ENUM_FormationNodeAbility.MaxHP:
		case ENUM_FormationNodeAbility.AttackDamage:
			return GameDataDB.GetString(7113);	//物攻
		case ENUM_FormationNodeAbility.AbilityPower:
			return GameDataDB.GetString(7107);	//術攻
		case ENUM_FormationNodeAbility.MagicResist:
			return GameDataDB.GetString(7102);	//術防
		case ENUM_FormationNodeAbility.CritsRate:
			return GameDataDB.GetString(7106);	//爆擊	
		case ENUM_FormationNodeAbility.ArmorPen:
			return GameDataDB.GetString(7105);	//物穿
		case ENUM_FormationNodeAbility.MagicResistPen:
			return GameDataDB.GetString(7104);	//術穿
		case ENUM_FormationNodeAbility.Null:
			return GameDataDB.GetString(272);	//物攻
		case ENUM_FormationNodeAbility.CDReduction:
		case ENUM_FormationNodeAbility.Tenacity:
		case ENUM_FormationNodeAbility.Healing_Per:
		case ENUM_FormationNodeAbility.MoveSpeed:
		default:
			return "NoString";	
		}
	}
	//-------------------------------------------------------------------------------------------------
	//取得特定屬性攻擊值
	private int GetAttrValue(S_RoleAttr sAttr,ENUM_FormationNodeAbility AttrType)
	{
		switch (AttrType) 
		{
		case ENUM_FormationNodeAbility.Armor:
			return (int)(Math.Round(sAttr.sBattleFinial.fDefense,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.MaxHP:
		case ENUM_FormationNodeAbility.AttackDamage:
			return (int)(Math.Round(sAttr.sBattleFinial.fAttack,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.AbilityPower:
			return (int)(Math.Round(sAttr.sBattleFinial.fAbilityPower,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.MagicResist:
			return (int)(Math.Round(sAttr.sBattleFinial.fMagicResist,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.CDReduction:
			return (int)(Math.Round(sAttr.sBattleFinial.fCDReduction,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.CritsRate:
			return (int)(Math.Round(sAttr.sBattleFinial.fCritsRate,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.Tenacity:
			return (int)(Math.Round(sAttr.sBattleFinial.fTenacity,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.Healing_Per:
			return (int)(Math.Round(sAttr.sBattleFinial.fHealing_Per,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.ArmorPen:
			return (int)(Math.Round(sAttr.sBattleFinial.fArmorPen,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.MagicResistPen:
			return (int)(Math.Round(sAttr.sBattleFinial.fMagicResistPen,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.MoveSpeed:
			return (int)(Math.Round(sAttr.sBattleFinial.fMoveSpeed,0,MidpointRounding.AwayFromZero));
		case ENUM_FormationNodeAbility.Null:
			return (int)(Math.Round(sAttr.sBattleFinial.fAttack,0,MidpointRounding.AwayFromZero));
		default:
			return 0;
		}
	}
}
