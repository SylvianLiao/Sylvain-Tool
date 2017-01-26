using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class LBSkill
{
	public UISprite 		SkillFrame;	//技能框
	public UISprite			SkillIcon;	//技能圖
	[HideInInspector]
	public int				SkillGUID;	//技能GUID
}

public class UI_PetLimitBreak : NGUIChildGUI  
{
	public UILabel			lbTitle					= null; //主標題
	public UILabel			lbTypeName				= null; //種族名
	public UISprite			spClass					= null; //職業圖
	[Header("屬性數值")]
	public UILabel			lbNowName_LimitLV		= null; //現在名稱+突破等級
	public UISprite			spPoint					= null; //名稱標題指向
	public UILabel			lbNextName_LimitLV		= null; //下一級名稱+突破等級
	public UILabel[]		lbSkillTiltes			= new UILabel[6]; //屬性標題 生命 物攻 物防 仙攻 仙防 爆擊
	public UILabel[]		lbCurrentUnit			= new UILabel[6]; //現有屬性值
	public UIWidget			NextPointSet			= null; //下一級指標圖
	public UILabel[]		lbUpUnit				= new UILabel[6]; //下一級屬性值
	public UIWidget			UpUnitSet				= null;
	public UILabel[]		lbUpValue				= new UILabel[6]; //提升數值
	public UIWidget			UpValueSet				= null;
	[Header("突破技能")]
	public UILabel			lbLBSkillTitle			= null; //突破技能標題
	public UISprite			spriteNextSkillBorder	= null; //下一個技能的指向
	public UIGrid			gdLBSkillList			= null; //突破技能排序
	public LBSkill[]		LBSkillList				= new LBSkill[4];
	public UILabel			lbVIPLimitTip			= null;
	public UILabel			lbPetPieceTitle			= null; //寵物碎片量標題
	public UILabel			lbAmout					= null; //寵物碎片總數
	public UIButton			btnLimitBreak			= null; //突破
	public UILabel			lbLimitBreak			= null; //突破按鈕字樣
	[Header("購買突破碎片")]
	public UIWidget		BuyLimitBreakRequest		= null; //購買突破碎片頁面
	public UILabel		lbBuyPetPieceRequest		= null; //購買突破碎片文字
	public UIButton		btnBuyPiece					= null; //買單個碎片
	public UIButton		btnBuyLimitBreak			= null; //買突破所需的碎片
	public UILabel		lbBuyPiece					= null; //買單個碎片文字
	public UILabel		lbBuyLimitBreak				= null; //買突破所需的碎片文字
	public UIButton		btnCloseBuyPage				= null; //關閉購買頁按鈕
	[Header("突破技能說明")]
	public UIWidget		LimitBreakSkillIntroduce	= null; //突破技能說明頁
	public UILabel		lbSkillName					= null; //技能名稱
	public UILabel		lbSkillIntroduce			= null; //技能介紹
	[Header("加載Tween的物件")]
	public List<GameObject>	TweenGameObjects	= new List<GameObject>();

	[HideInInspector]
	public S_PetData	m_OldPetData;
	//
	private int			m_PetGUID 					= 0;
	[HideInInspector]
	public int 		m_VIPLimitLV				= 99;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_PetLimitBreak";
	//-------------------------------------------------------------------------------------------------
	private UI_PetLimitBreak() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		//取得VIP開放等級並設定提示
		GetVIPLimitLV();
		//
		lbSkillTiltes[0].text		= GameDataDB.GetString(582);
		lbSkillTiltes[1].text 		= GameDataDB.GetString(583); 	//物攻成長
		lbSkillTiltes[2].text 		= GameDataDB.GetString(584); 	//物防成長
		lbSkillTiltes[3].text 		= GameDataDB.GetString(585); 	//仙攻成長
		lbSkillTiltes[4].text 		= GameDataDB.GetString(586); 	//仙防成長
		lbSkillTiltes[5].text 		= GameDataDB.GetString(587); 	//爆擊成長
		lbLBSkillTitle.text			= GameDataDB.GetString(595); 	//突破技能
		//
		lbPetPieceTitle.text		= GameDataDB.GetString(594); 	//碎片數量
		//lbLimitBreak.text			= GameDataDB.GetString(2602);	//突破
		LimitBreakSkillIntroduce.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		//載入寵物屬性資訊
		LoadLimitBreakPetData();
		ARPGApplication.instance.ResetTweenObjects(TweenGameObjects);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetPetGUIDbyUpGrade(int petGUID)
	{
		m_PetGUID = petGUID;
	}
	//-------------------------------------------------------------------------------------------------
	#region 屬性數值計算
	public void LoadLimitBreakPetData()
	{
		if(m_PetGUID<=0) 
		{
			Hide();
			return;
		}
		S_PetData pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(m_PetGUID);
		if(pd == null)
		{
			Hide();
			return;
		}
		if(pd.iPetLevel == 0)
		{
			Hide();
			return;
		}
		//
		LimitBreakSkillIntroduce.gameObject.SetActive(false);
		//
		m_OldPetData = pd;
		S_PetData_Tmp 		pdTmp 		= GameDataDB.PetDB.GetData(m_PetGUID);
		lbTypeName.text = GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(pdTmp.emCharType));
		Utility.ChangeAtlasSprite(spClass,ARPGApplication.instance.GetPetCalssIconID(pdTmp.emCharClass));
		//
		string pName = SetUILabelRareColor(GameDataDB.GetString(pdTmp.iName),pdTmp.iRank,pd.iPetLimitLevel);
		lbNowName_LimitLV.text = pName+(pd.iPetLimitLevel<=0?"":"[FFFFFF]+"+pd.iPetLimitLevel.ToString()+"[-]");
		//設定計算數值
		int[] PetCurrentValues = CalculatePetValue(pd);
		//載入現有數值
		for(int i=0;i<lbCurrentUnit.Length;++i)
			lbCurrentUnit[i].text = PetCurrentValues[i].ToString();
		//已達突破最大數
		if(pd.iPetLimitLevel>=GameDefine.MAX_PET_LIMITLEVEL)
		{
			spPoint.gameObject.SetActive(false);
			lbNextName_LimitLV.gameObject.SetActive(false);
			NextPointSet.gameObject.SetActive(false);
			UpUnitSet.gameObject.SetActive(false);
			UpValueSet.gameObject.SetActive(false);
			lbAmout.text = "--/--";
			btnLimitBreak.isEnabled = false;
			return;
		}
		//未達突破上限
		pName = SetUILabelRareColor(GameDataDB.GetString(pdTmp.iName),pdTmp.iRank,pd.iPetLimitLevel+1);
		lbNextName_LimitLV.text = pName+"[FFFFFF]+"+(pd.iPetLimitLevel+1).ToString()+"[-]";
		//clone一隻突破為下一級的petdata
		S_PetData UpPetData = pd.GetCloneDiffLimitLV(pd.iPetLimitLevel+1);
		//設定計算數值
		int[] PetNextValues = CalculatePetValue(UpPetData);
		//載入提升數值
		for(int i=0;i<lbUpUnit.Length;++i)
			lbUpUnit[i].text = PetNextValues[i].ToString();
		//增加係數(綠色值顯示)
		for(int i=0;i<lbUpValue.Length;++i)
			lbUpValue[i].text = GameDataDB.GetString(588+i)+"+"+(PetNextValues[i]-PetCurrentValues[i]).ToString();
		//碎片數
		S_PetChip_Tmp pChip = GameDataDB.PetChipDB.GetData(pd.GetPetRank());
		int iNeedPCount = pChip.iLimitBreakCostPetChip[pd.iPetLimitLevel];
		
		lbAmout.text = pd.iMaterialCount.ToString() +"/"+ iNeedPCount.ToString();
		//設定寵物突破技能資訊
		SetPetLimitBreakSkillInfo();
	}
	//-------------------------------------------------------------------------------------------------
	//依突破設定文字品階色
	public string SetUILabelRareColor(string str,int iRank,int iLimitLV)
	{
		if(iLimitLV > GameDefine.MAX_PET_LIMITLEVEL)
			return str;
		
		switch(iLimitLV)
		{
		case 2:
		case 3:
			iRank += GameDefine.PET_UPGRADE_LEVEL1;
			break;
		case 4:
			iRank += GameDefine.PET_UPGRADE_LEVEL2;
			break;
		}
		iRank = iRank>=GameDefine.MAX_PET_UPGRADERANK?GameDefine.MAX_PET_UPGRADERANK:iRank;
		
		switch(iRank)
		{
		case 0:
		case 1:
			//白(150、150、150)
			str = "[FFFFFF]"+str+"[-]";
			break;
		case 2:
			//綠(0、130、30)
			str = "[32FF46]"+str+"[-]";
			break;
		case 3:
			//藍(0、150、255)
			str = "[00AAFF]"+str+"[-]";
			break;
		case 4:
			//紫 (150、55、200)
			str = "[E150FF]"+str+"[-]";
			break;
		case 5:
			//橘(255、140、40)
			str = "[FF8C28]"+str+"[-]";
			break;
		case 6:
			//黃(255、60、100)
			str = "[FFFF00]"+str+"[-]";
			break;
		case 7:
			//金(255、230、125)
			str = "[FFA800]"+str+"[-]";
			break;
		}
		return str;
	}
	//-------------------------------------------------------------------------------------------------
	//設定計算數值
	private int[] CalculatePetValue(S_PetData pd)
	{
		S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,pd.iPetDBFID);
		S_ItemData[] AllEQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems();
		//設定是否載入戰陣
		S_FormationData formationData = null;
		bool bCalculateFormation = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.DetectPetforBattle(m_PetGUID);
		if(bCalculateFormation)
			formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
		List<S_PetData> sPet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData;
		S_RoleAttr PetAttrValue = ARPGCharacter.CreatePetRoleAttr(pd,EQList,sPet,pd.GetTalentSkill(),null,formationData,AllEQList);
		int[] datas = new int[6];
		datas[0] = (int)(Math.Round(PetAttrValue.sBattleFinial.fMaxHP,		0,MidpointRounding.AwayFromZero));
		datas[1] = (int)(Math.Round(PetAttrValue.sBattleFinial.fAttack,		0,MidpointRounding.AwayFromZero));
		datas[2] = (int)(Math.Round(PetAttrValue.sBattleFinial.fDefense,		0,MidpointRounding.AwayFromZero));
		datas[3] = (int)(Math.Round(PetAttrValue.sBattleFinial.fAbilityPower,0,MidpointRounding.AwayFromZero));
		datas[4] = (int)(Math.Round(PetAttrValue.sBattleFinial.fMagicResist,	0,MidpointRounding.AwayFromZero));
		datas[5] = (int)(Math.Round(PetAttrValue.sBattleFinial.fCritsRate,	0,MidpointRounding.AwayFromZero));
		return datas;
	}
	//-------------------------------------------------------------------------------------------------
	#endregion 屬性數值計算
	//-------------------------------------------------------------------------------------------------
	//先找出開放的VIP等級
	private void GetVIPLimitLV()
	{
		GameDataDB.VIPLVDB.ResetByOrder();
		for(int i=1;i<=GameDataDB.VIPLVDB.GetDataSize();++i)
		{
			S_VIPLV_Tmp vipTmp = GameDataDB.VIPLVDB.GetData(i);
			if(vipTmp.PetChipSwitch == GameDefine.VIP_FUNCTION_ON)
			{
				m_VIPLimitLV = vipTmp.GUID-1;
				break;
			}
		}

		//設定TIP內容
		if(m_VIPLimitLV >= 99)
			lbVIPLimitTip.gameObject.SetActive(false);
		else
			lbVIPLimitTip.text	= string.Format(GameDataDB.GetString(477),m_VIPLimitLV); 	//VIP等級提示
	}
	//-------------------------------------------------------------------------------------------------
	//設定寵物突破技能資訊
	public void SetPetLimitBreakSkillInfo()
	{
		Color grayColor 	= new Color(0.0f,1.0f,1.0f);
		Color NormalColor	= new Color(1.0f,1.0f,1.0f);
		S_PetData petData 		= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(m_PetGUID);
		S_PetData_Tmp petTmp	= GameDataDB.PetDB.GetData(petData.iPetDBFID);
		//
		for(int i=0;i<GameDefine.MAX_PET_LIMITLEVEL;++i)
		{
			if(petTmp.iLimitBreakPSkillID[i] <=0)
			{
				LBSkillList[i].SkillFrame.gameObject.SetActive(false);
				LBSkillList[i].SkillIcon.gameObject.SetActive(false);
				continue;
			}
			
			//設定技能ID
			LBSkillList[i].SkillGUID = petTmp.iLimitBreakPSkillID[i];
			//如果突破等級為0的話即都灰階
			if(petData.iPetLimitLevel==0)
			{
				LBSkillList[i].SkillFrame.color = grayColor;
				LBSkillList[i].SkillIcon.color = grayColor;
				continue;
			}
			if(petData.iPetLimitLevel>=(i+1))
			{
				LBSkillList[i].SkillFrame.color = NormalColor;
				LBSkillList[i].SkillIcon.color = NormalColor;
			}
			else 
			{
				LBSkillList[i].SkillFrame.color = grayColor;
				LBSkillList[i].SkillIcon.color = grayColor;
			}
		}
		//指向框
		if(petData.iPetLimitLevel>=0 && petData.iPetLimitLevel<GameDefine.MAX_PET_LIMITLEVEL && LBSkillList[petData.iPetLimitLevel].SkillIcon.gameObject.activeSelf)
		{
			spriteNextSkillBorder.transform.parent = LBSkillList[petData.iPetLimitLevel].SkillIcon.transform;
			spriteNextSkillBorder.transform.localPosition = Vector3.zero;
			spriteNextSkillBorder.gameObject.SetActive(true);
		}
		else
			spriteNextSkillBorder.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
}
