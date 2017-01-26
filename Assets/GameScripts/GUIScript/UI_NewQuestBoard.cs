using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_NewQuestBoard : NGUIChildGUI 
{
	public UILabel			lbBoardTitle		= null; //任務系統標頭
	public UIButton			btnExit				= null; //結束任務系統
	//左側
	public UIScrollView		svTitleList			= null;	//任務類型scroll view
	public UIGrid			gdTitleList			= null;	//任務類型排序
	public QuestTitle		Prefab				= null;	//任務類型prefab
	//右側
	public UIScrollView		svContentList		= null;	//任務項目scroll view
	public UIGrid			gdContentList		= null; //任務項目排序
	[System.NonSerialized]
	public List<QuestTitle>	TitleList			= new List<QuestTitle>();
	[System.NonSerialized]
	public ENUM_NewQuest_Type	ThisQuestType;
	[System.NonSerialized]
	public List<UIButton>	btnGroups			= new List<UIButton>();
	[System.NonSerialized]
	public int				iCurGroup			= -1;
	[System.NonSerialized]
	public List<Slot_Mission> MissionList		= new List<Slot_Mission>();

	//
	private const int		m_groupNameIDplus	= 1121;
	private string			SlotName			= "Slot_Mission";
	private const string GUI_SMARTOBJECT_NAME = "UI_NewQuestBoard";
	
	//-------------------------------------------------------------------------------------------
	private UI_NewQuestBoard(): base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-------------------------------------------------------------------------------------------
	public override void Initialize ()
	{
		base.Initialize();
		//建立任務列表(空)
		CreateMissionSlots();
	}
	//-------------------------------------------------------------------------------------------
	//建立並設定群組列表(未加入按鈕功能)
	public void CreateGroupList(List<int> gList,List<S_NewQuestData_Tmp> NQDList)
	{
		if(gList.Count == 0)
		{
			Prefab.gameObject.SetActive(false);
			return;
		}
		btnGroups.Clear();
		TitleList.Clear();
		QuestTitle 	QTclone = null;
		int 		iEnableBtnNum = 0;
		for(int i=0;i<gList.Count;++i)
		{
			QTclone = Instantiate(Prefab) as QuestTitle; 
			QTclone.transform.parent			= gdTitleList.transform;
			QTclone.transform.localScale		= Vector3.one;
			QTclone.transform.localRotation		= Quaternion.identity;
			QTclone.transform.localPosition		= Vector3.zero;
			QTclone.transform.name				= i<10?"Group0"+i.ToString():"Group"+i.ToString();
			//設定名稱
			int GroupNameID = GetGroupStringID(gList[i]);
			QTclone.lbTitle.text 	= GameDataDB.GetString(GroupNameID);
			//設定toggle群組
			QTclone.tgTitle.group = 0;
			QTclone.tgTitle.value = i==0;
			QTclone.tgTitle.group = 1;
			//儲存按鍵事件所需資料
			QTclone.btnTitle.userData = GroupNameID;
			if(ThisQuestType == ENUM_NewQuest_Type.ENUM_NewQuest_Type_CreateRole)
			{
				bool bTriggerQuest = false;
				for(int j=0;j<NQDList.Count;++j)
				{
					if(NQDList[j].iGroup == gList[i])
					{
						//計算出間隔時間(現在時間-創角時間)
						DateTime NowDay 	= new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
						DateTime CreateDate = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.tCreateTime;
						DateTime CreateDay	= new DateTime(CreateDate.Year,CreateDate.Month,CreateDate.Day);
						//
						TimeSpan interval = NowDay - CreateDay;
						int TotalInvervalSec = (int)interval.TotalSeconds;
						int DuringDay = TotalInvervalSec / GameDefine.NEWQUEST_DAY_SECONDS;
						++DuringDay;
						bTriggerQuest = (DuringDay >= (NQDList[j].iDayNum));
						if(bTriggerQuest)
							++iEnableBtnNum;
						break;
					}
				}
				//
				QTclone.btnTitle.isEnabled = bTriggerQuest;
			}
			btnGroups.Add(QTclone.btnTitle);
			TitleList.Add(QTclone);
			//提示標記
			QTclone.Mark.gameObject.SetActive(false);
			if(QTclone.btnTitle.isEnabled == true)
			{
				QTclone.Mark.gameObject.SetActive(CheckTipShow(gList[i]));
			}
		}
		//隱藏prefab
		Prefab.gameObject.SetActive(false);
		//重排
		gdTitleList.Reposition();
		//創角列表用
		if(iEnableBtnNum<=4)
			svTitleList.enabled = false;
		else
			svTitleList.enabled = true;
	}
	//-----------------------------------------------------------------------------------------------------
	//檢查標題標記是否顯示
	public bool CheckTipShow(int iGroup)
	{
		//先檢查指定類別的任務系統
		GameDataDB.NewQuestDB.ResetByOrder();
		for(int i=0;i<GameDataDB.NewQuestDB.GetDataSize();++i)
		{
			S_NewQuestData_Tmp NewQuestTmp = GameDataDB.NewQuestDB.GetDataByOrder();

			if(NewQuestTmp.emType != ThisQuestType)
				continue;

			if(NewQuestTmp.iGroup != iGroup)
				continue;

			if(ARPGApplication.instance.GetQuestCompleteStatus(NewQuestTmp))
				return true;
		}
		return false;
	}
	//-----------------------------------------------------------------------------------------------------
	public int GetGroupStringID(int iGroup)
	{
		int stringID = m_groupNameIDplus+iGroup;
		return stringID;
	}
	//-------------------------------------------------------------------------------------------
	//建立任務列表(空)
	private void CreateMissionSlots()
	{
		Slot_Mission go = ResourceManager.Instance.GetGUI(SlotName).GetComponent<Slot_Mission>();
		if(go == null)
		{
			UnityDebugger.Debugger.Log( string.Format("Slot_Mission load prefeb error,path:{0}", "GUI/"+SlotName) );
			return;
		}
		MissionList.Clear();

		for(int i=0;i<GameDefine.NEWQUEST_MISSONCOUNT_MAX;++i)
		{
			Slot_Mission missonSlot = Instantiate(go) as Slot_Mission;
			missonSlot.transform.parent				= gdContentList.transform;
			missonSlot.transform.localScale			= Vector3.one;
			missonSlot.transform.localRotation		= Quaternion.identity;
			missonSlot.transform.localPosition		= Vector3.zero;
			missonSlot.transform.gameObject.SetActive(false);
			missonSlot.btnActive.userData = i;
			missonSlot.index = i;
			//存到List中待用
			MissionList.Add(missonSlot);
		}
	}
	//-------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------
}
