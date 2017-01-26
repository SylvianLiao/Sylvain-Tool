using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_PetInfo : NGUIChildGUI
{
	public UILabel			lbInfoTitle			= null;
	public UILabel			lbTypeName			= null; //種族名
	public UISprite			spClass				= null; //職業圖
	[Header("屬性數值")]
	public UILabel			lbPetLV				= null; //等級
	public UISprite[]		spStars				= new UISprite[4]; //突破顯示
	public UILabel			lbExpTitle			= null; //經驗值標題
	public UILabel			lbExpValue			= null; //經驗值數值
	public UISlider			spExpBar			= null; //經驗值bar
	public UILabel[]		lbAttributeTitles	= new UILabel[12]; //生命 法攻 法防 爆傷 攻速 跑速 物攻 物防 爆擊 韌性 物穿 法穿
	public UILabel[]		lbAttributeValues	= new UILabel[12];
	[Header("加載Tween的物件")]
	public List<GameObject>	TweenGameObjects	= new List<GameObject>();

	private int				m_PetGUID			= 0;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_PetInfo";
	//-------------------------------------------------------------------------------------------------
	private UI_PetInfo() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		lbInfoTitle.text 			= GameDataDB.GetString(1031);//狀態
		lbExpTitle.text				= GameDataDB.GetString(1026);//經驗值
		lbAttributeTitles[0].text 	= GameDataDB.GetString(1027);//生命
		lbAttributeTitles[1].text	= GameDataDB.GetString(1063);//技能攻擊
		lbAttributeTitles[2].text	= GameDataDB.GetString(1064);//技能防禦
		lbAttributeTitles[3].text	= GameDataDB.GetString(1068);//爆傷
		lbAttributeTitles[4].text	= GameDataDB.GetString(1029);//攻速
		lbAttributeTitles[5].text	= GameDataDB.GetString(1019);//跑速
		lbAttributeTitles[6].text	= GameDataDB.GetString(1028);//物理攻擊
		lbAttributeTitles[7].text	= GameDataDB.GetString(1030);//物理防禦
		lbAttributeTitles[8].text	= GameDataDB.GetString(1025);//爆擊
		lbAttributeTitles[9].text	= GameDataDB.GetString(1058);//韌性
		lbAttributeTitles[10].text	= GameDataDB.GetString(1065);//物理穿透
		lbAttributeTitles[11].text	= GameDataDB.GetString(1066);//技能穿透
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		LoadPetInfoData();
		ARPGApplication.instance.ResetTweenObjects(TweenGameObjects);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetPetGUIDbyInfo(int PetGUID)
	{
		m_PetGUID = PetGUID;
	}
	//-------------------------------------------------------------------------------------------------
	public void LoadPetInfoData()
	{
		if(m_PetGUID<=0) 
		{
			Hide();
			return;
		}
		//載入S_PetData資料如果無則自動Clone一個
		S_PetData pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(m_PetGUID);
		if(pd==null)
		{
			pd = GetInitPetData();
		}
		else if(pd.iPetLevel == 0)
		{
			pd = GetInitPetData();
		}
		S_PetData_Tmp 		pdTmp 		= GameDataDB.PetDB.GetData(m_PetGUID);
		S_PetLevelUp_Tmp 	pLevelUp 	= GameDataDB.PetLevelUpDB.GetData(pd.iPetLevel);
		if(pdTmp == null)
		{
			Hide();
			return;
		}
		lbTypeName.text = GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(pdTmp.emCharType));
		Utility.ChangeAtlasSprite(spClass,ARPGApplication.instance.GetPetCalssIconID(pdTmp.emCharClass));
		//
		lbPetLV.text = GameDataDB.GetString(1056)+"[FFFF00]"+pd.iPetLevel.ToString()+"[-]";
		for(int i=0;i<spStars.Length;++i)
		{
			spStars[i].gameObject.SetActive(false);
			if(pd.iPetLimitLevel>i)
				spStars[i].gameObject.SetActive(true);
		}
		//經驗值顯示
		lbExpValue.text 	= pd.iPetExp.ToString()+"/"+pLevelUp.iExpRank[pdTmp.iRank-1].ToString();			//寵物經驗值
		float expRatio 		= (float)pd.iPetExp / pLevelUp.iExpRank[pdTmp.iRank-1];
		if (expRatio > 1) expRatio = 1;
			spExpBar.value 		= expRatio;

		PetAttributeValueLoad(pd);
	}
	//-------------------------------------------------------------------------------------------------
	private S_PetData GetInitPetData()
	{
		S_PetData pd = new S_PetData();
		pd = pd.GetCloneDiffLV(1,m_PetGUID);
		return pd;
	}
	//-------------------------------------------------------------------------------------------------
	private void PetAttributeValueLoad(S_PetData pd)
	{
		//取得寵物角色資料
		S_RoleAttr PetRoleAttr;
		S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,m_PetGUID);
		S_ItemData[] AllItemList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems();
		//設定是否載入戰陣
		S_FormationData formationData = null;
		bool bCalculateFormation = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.DetectPetforBattle(m_PetGUID);
		if(bCalculateFormation)
			formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
		List<S_PetData> sPet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData;

		PetRoleAttr = ARPGCharacter.CreatePetRoleAttr(pd,EQList,sPet,pd.GetTalentSkill(),null,formationData,AllItemList);
		//顯示整數
		lbAttributeValues[0].text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fMaxHP,			0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//生命
		lbAttributeValues[1].text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fAbilityPower,	0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能攻擊力
		lbAttributeValues[2].text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fMagicResist,	0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能防禦力
		lbAttributeValues[3].text			= "[FFFF00]"+(100.0f * Math.Round(PetRoleAttr.sBattleFinial.fCriticalDamage,4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";		//爆傷
		lbAttributeValues[4].text			= "[FFFF00]"+(100.0f * Math.Round(PetRoleAttr.sBattleFinial.fAtkSpeed,		4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";		//攻速
		lbAttributeValues[5].text			= "[FFFF00]"+(100.0f * Math.Round(PetRoleAttr.sBattleFinial.fMoveSpeed_Per,	4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";		//跑速
		lbAttributeValues[6].text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fAttack,			0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理攻擊力
		lbAttributeValues[7].text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fDefense,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理防禦力
		lbAttributeValues[8].text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fCritsRate,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//爆擊
		lbAttributeValues[9].text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fTenacity,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//韌性
		double petMRP = PetRoleAttr.sBattleFinial.fMagicResistPen;
		petMRP = petMRP * Mathf.Max(0.05f,(float)(1+PetRoleAttr.sBattleFinial.fMagicResistPen_Per));
		lbAttributeValues[10].text			= "[FFFF00]"+(Math.Round(petMRP,									0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能穿透
		double petARP = PetRoleAttr.sBattleFinial.fArmorPen;
		petARP = petARP * Mathf.Max(0.05f,(float)(1+PetRoleAttr.sBattleFinial.fArmorPen_Per));
		lbAttributeValues[11].text			= "[FFFF00]"+(Math.Round(petARP,									0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理穿透
	}
	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------
}
