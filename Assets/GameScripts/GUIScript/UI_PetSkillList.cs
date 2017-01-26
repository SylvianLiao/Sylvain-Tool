using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_PetSkillList : NGUIChildGUI 
{
	[Header("技能列")]
	public UILabel						lbSkillListTitle		= null; //夥伴技能列標題
	public Slot_PetSkillUpInfo 			SkillSlotPrefab			= null; //夥伴技能個別Prefab
	public UIScrollView					svSkillList				= null; //技能列表
	public UIGrid						gdSkillList				= null; //技能排列
	public UIButton						btnAllUp				= null; //全部升級按鈕
	public UILabel						lbAllUp					= null; //全部升級字樣
	[Header("技能說明框")]
	public UIWidget						SkillInfoSet			= null; //說明項目集合
	public UILabel						lbInfoTitle				= null; //說明框標題
	public UILabel						lbSkillLV				= null; //技能等級
	public UILabel						lbCoolDown				= null; //技能冷卻時間
	public UILabel						lbCurrentSkillNote		= null; //現在技能說明
	public UIWidget						NextSkillNoteSet		= null; //下一個技能集合
	public UILabel						lbNextSkillTile			= null; //升級效果
	public UILabel						lbNextSkillNote			= null; //下一個技能說明
	[Header("加載Tween的物件")]
	public List<GameObject>	TweenGameObjects	= new List<GameObject>();
	//
	private int							m_PetGUID 			= 0;
	private S_PetData					m_PetData			= null;
	//儲存按下的技能info
	private ENUM_PetSkill				m_SelectSkillType	= ENUM_PetSkill.ENUM_PetSkill_Max;
	//
	[HideInInspector]
	public List<Slot_PetSkillUpInfo>	SkillUpInfoList			= new List<Slot_PetSkillUpInfo>();	
	[HideInInspector]
	public Dictionary<ENUM_PetSkill,int>		PetSkillList		= new Dictionary<ENUM_PetSkill, int>();
	//儲存按下哪個技能的列舉
	[HideInInspector]
	public ENUM_PetSkill				petSkillType = ENUM_PetSkill.ENUM_PetSkill_ASkillID;
	//
	private bool						bRemoveBtnTweenTarget = false;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_PetSkillList";
	
	//-------------------------------------------------------------------------------------------------
	private UI_PetSkillList() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		SkillInfoSet.gameObject.SetActive(false);
		CreateSkillSlots();
		AssignText();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		//載入夥伴技能相關資料
		LoadPetSkillListData();
		ARPGApplication.instance.ResetTweenObjects(TweenGameObjects);
	}
	//-------------------------------------------------------------------------------------------------
	void Update()
	{
		if(bRemoveBtnTweenTarget)
		{
			bRemoveBtnTweenTarget = false;
			for(int i=0;i<SkillUpInfoList.Count;++i)
				SkillUpInfoList[i].btnSkillFrame.tweenTarget = null;
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void CreateSkillSlots()
	{
		if(SkillSlotPrefab == null)
			return;

		SkillUpInfoList.Clear();
		for(int i=0;i<(int)ENUM_PetSkill.ENUM_PetSkill_Max;++i)
		{
			//createPrefab
			Slot_PetSkillUpInfo newgo= Instantiate(SkillSlotPrefab) as Slot_PetSkillUpInfo;
			newgo.transform.parent			= gdSkillList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.name 			= "PetSkill"+i.ToString();
			newgo.gameObject.SetActive(false);
			newgo.onClickSkill				+= SkillInfoEvent;
			SkillUpInfoList.Add(newgo);
		}
		SkillSlotPrefab.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	private void AssignText()
	{
		lbSkillListTitle.text 	= GameDataDB.GetString(2701); //技能
		lbNextSkillTile.text	= GameDataDB.GetString(9717);//"升級效果"
	}
	//-------------------------------------------------------------------------------------------------
	public void SetPetGUIDbySkllList(int petGUID)
	{
		m_PetGUID = petGUID;
	}
	//-------------------------------------------------------------------------------------------------
	public void LoadPetSkillListData()
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
		SkillInfoSet.gameObject.SetActive(false);
		m_SelectSkillType = ENUM_PetSkill.ENUM_PetSkill_Max;
		//
		m_PetData = pd;
		S_PetData_Tmp 		pdTmp 		= GameDataDB.PetDB.GetData(m_PetGUID);
		int iActiveSkillNum =7;
		//S_SkillData_Tmp skillData;
		PetSkillList.Clear();
		PetSkillList.Add(ENUM_PetSkill.ENUM_PetSkill_ASkillID,pdTmp.iASkillID);
		PetSkillList.Add(ENUM_PetSkill.ENUM_PetSkill_PSkill1,pdTmp.PassiveSkill[0].iPSkillID);
		PetSkillList.Add(ENUM_PetSkill.ENUM_PetSkill_PSkill2,pdTmp.PassiveSkill[1].iPSkillID);
		PetSkillList.Add(ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID1,pdTmp.iLimitBreakPSkillID[0]);
		PetSkillList.Add(ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID2,pdTmp.iLimitBreakPSkillID[1]);
		PetSkillList.Add(ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID3,pdTmp.iLimitBreakPSkillID[2]);
		PetSkillList.Add(ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID4,pdTmp.iLimitBreakPSkillID[3]);
		foreach(ENUM_PetSkill pSkillType in PetSkillList.Keys)
		{
			if(PetSkillList[pSkillType] <=0)
			{
				SkillUpInfoList[(int)pSkillType].gameObject.SetActive(false);
				--iActiveSkillNum;
				continue;
			}
			SkillUpInfoList[(int)pSkillType].btnSkillFrame.userData = pSkillType;
			SkillUpInfoList[(int)pSkillType].gameObject.SetActive(true);
			SkillUpInfoList[(int)pSkillType].SetSkillData(pSkillType,PetSkillList[pSkillType],pd);
		}
		gdSkillList.repositionNow 	= true;
		bRemoveBtnTweenTarget 		= true;
		if(iActiveSkillNum<=4)
			svSkillList.enabled=false;
	}
	//-------------------------------------------------------------------------------------------------
	#region 技能資訊事件宣告
	private void SkillInfoEvent(ENUM_PetSkill pskill,S_PetData pd)
	{
		if(m_SelectSkillType == pskill)
		{
			m_SelectSkillType = ENUM_PetSkill.ENUM_PetSkill_Max;
			SkillInfoSet.gameObject.SetActive(false);
			return;
		}
		m_SelectSkillType = pskill;
		S_PetData_Tmp	pdtmp		= GameDataDB.PetDB.GetData(pd.iPetDBFID);
		S_SkillData_Tmp skillData 	= GameDataDB.SkillDB.GetData(PetSkillList[pskill]);
		//
		lbInfoTitle.text			= GameDataDB.GetString(skillData.SkillName);
		lbSkillLV.text				= pd.iSkillLv[(int)pskill]<=0?"":string.Format("{0}{1}{2}",GameDataDB.GetString(1056),":",pd.iSkillLv[(int)pskill]);
		lbCoolDown.text				= skillData.fCoolDown<=0?"":string.Format("{0}{1}{2}",GameDataDB.GetString(9716),":",skillData.fCoolDown.ToString());
		lbCurrentSkillNote.text		= ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skillData.iNote),pd.GetTalentSkill(),SkillLevelType.Now);
		if(pd.iSkillLv[(int)pskill] == skillData.iUpgradeLimitSkillLv)
			NextSkillNoteSet.gameObject.SetActive(false);
		else
		{
			NextSkillNoteSet.gameObject.SetActive(true);
			lbNextSkillNote.text = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skillData.iNote),pd.GetTalentSkill(),SkillLevelType.Next);

			switch(pskill)
			{
			case ENUM_PetSkill.ENUM_PetSkill_PSkill1:
				if(pd.bPSkill[0] == 0)
					NextSkillNoteSet.gameObject.SetActive(false);
				break;
			case ENUM_PetSkill.ENUM_PetSkill_PSkill2:
				if(pd.bPSkill[1] == 0)
					NextSkillNoteSet.gameObject.SetActive(false);
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID1:
				if(pd.iPetLimitLevel<1)
					NextSkillNoteSet.gameObject.SetActive(false);
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID2:
				if(pd.iPetLimitLevel<2)
					NextSkillNoteSet.gameObject.SetActive(false);
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID3:
				if(pd.iPetLimitLevel<3)
					NextSkillNoteSet.gameObject.SetActive(false);
				break;
			case ENUM_PetSkill.ENUM_PetSkill_LimitBreakPSkillID4:
				if(pd.iPetLimitLevel<4)
					NextSkillNoteSet.gameObject.SetActive(false);
				break;
			}
		}
		SkillInfoSet.gameObject.SetActive(true);
		ARPGApplication.instance.ResetTweenEffect(SkillInfoSet.gameObject);
	}
	#endregion 技能資訊事件宣告
	//-------------------------------------------------------------------------------------------------
}
