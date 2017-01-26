using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_Mission : NGUIChildGUI 
{
	public UISprite				spriteMissionIcon		= null; //任務icon
	public UILabel				lbMissionName			= null; //任務名稱
	public UILabel				lbMissionTarget			= null; //任務目標
	public UILabel				lbMissionNote			= null; //任務說明
	public UILabel				lbRewardTitle			= null; //獎勵標頭
	public UIGrid				GridRewards				= null; //獎勵排序
	public UISprite[]			RewardIcons				= new UISprite[4];
	public UILabel[] 			RewardCounts			= new UILabel[4];
	public UIButton				btnActive				= null; //啟動任務
	public UILabel				lbActive				= null; //啟動任務內容
	public UISprite				spriteTip				= null; //提醒tip
	[System.NonSerialized]
	public bool					bComplete				= false;
	[System.NonSerialized]
	public bool					bFinish					= false;
	[System.NonSerialized]
	public S_NewQuestData_Tmp	NewQDataTmp				= null;
	[System.NonSerialized]
	public int					index 					= -1;

	private const int			QuestIconIDplus			= 1151;
	//
	private S_Item_Tmp[]		ItemTmps				= new S_Item_Tmp[4];
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_Mission";
	//-------------------------------------------------------------------------------------------------
	private Slot_Mission() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize ()
	{
		base.Initialize();
	}
	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		for(int i=1;i<RewardIcons.Length;++i)
		{
			UIEventListener.Get(RewardIcons[i].gameObject).onClick		+= ShowItemInfo;
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void ShowItemInfo(GameObject gb)
	{
		for(int i=1;i<RewardIcons.Length;++i)
		{
			if(gb == RewardIcons[i].gameObject)
				ShowChestItemDetail(ItemTmps[i]);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//設定任務內容
	public void SetMissionDataToSlot(S_NewQuestData_Tmp newQuestTmp)
	{
		if(newQuestTmp == null)
			return;
		bool bTriggerQuest = true;
		//取得是否已完成
		bFinish = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckNewQuestUnlockOrNot(newQuestTmp.GUID);
		//先判斷開放時間
		/*if(newQuestTmp.emTime == ENUM_NewQuestTime_Type.ENUM_NewQuestTime_Type_CreateRole)
		{
			//計算出間隔時間(現在時間-創角時間)
			TimeSpan interval = DateTime.Now - ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.tCreateTime;
			int TotalInvervalSec = (int)interval.TotalSeconds;
			int DuringDay = TotalInvervalSec / GameDefine.NEWQUEST_DAY_SECONDS;
			bTriggerQuest = (DuringDay >= (newQuestTmp.iDayNum-1));
		}
		else*/ if(newQuestTmp.emTime == ENUM_NewQuestTime_Type.ENUM_NewQuestTime_Type_Fixed)
		{
			int iStartCompare 	= DateTime.Compare(newQuestTmp.StartTime,DateTime.Now);
			int iEndCompare		= DateTime.Compare(DateTime.Now,newQuestTmp.EndTime);
			if(iStartCompare>0 || iEndCompare>0)
				bTriggerQuest = false;
		}

		if(bTriggerQuest)
		{
			//任務觸發條件1
			if(newQuestTmp.iPreFlag>=0)
			{
				bTriggerQuest = (ARPGApplication.instance.m_RoleSystem.sBaseNewQuestFlag.Get(newQuestTmp.iPreFlag));
			}
			//任務觸發條件2
			if(newQuestTmp.iRoleLv>0 && bTriggerQuest)
			{
				bTriggerQuest = (ARPGApplication.instance.m_RoleSystem.iBaseLevel >= newQuestTmp.iRoleLv);
			}
		}

		if(bFinish == true && newQuestTmp.emType != ENUM_NewQuest_Type.ENUM_NewQuest_Type_CreateRole)
		{
			bTriggerQuest = false;
		}

		if(bTriggerQuest == false)
		{
			this.gameObject.SetActive(false);
			return;
		}

		spriteTip.gameObject.SetActive(false);
		NewQDataTmp = newQuestTmp;
		//設定任務icon
		int QuestIconID = GetQuestIconID(newQuestTmp.iIconID);
		Utility.ChangeAtlasSprite(spriteMissionIcon,QuestIconID);
		//設定任務名稱
		lbMissionName.text = GameDataDB.GetString(newQuestTmp.iName);
		//設定任務說明
		lbMissionNote.text = GameDataDB.GetString(newQuestTmp.iNote);
		//設定任務目標與儲存目標進度
		int iCurTarget = ARPGApplication.instance.GetCurrentStatebyTargetType(newQuestTmp.emTarget,newQuestTmp.iCondition);
		lbMissionTarget.gameObject.SetActive(iCurTarget != -1);
		lbMissionTarget.text = iCurTarget.ToString() + "/" + newQuestTmp.iConditionValue.ToString();
		bComplete = ARPGApplication.instance.GetMissionStatus(newQuestTmp.emTarget,iCurTarget,newQuestTmp.iConditionValue);
		//巔峰pvp排名另做處理
		if(NewQDataTmp.emTarget == ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_PeakPvpRank)
		{
			int result = iCurTarget<=newQuestTmp.iConditionValue?1:0;
			lbMissionTarget.text = result.ToString()+"/1";
		}
		//設定獎勵
		lbRewardTitle.text = GameDataDB.GetString(962);
		SetEachReward(0,10005,newQuestTmp.iExp);
		S_Item_Tmp itemTmp;
		for(int i=0;i<GameDefine.QUEST_REWARD_MAX;++i)
		{
			itemTmp = GameDataDB.ItemDB.GetData(newQuestTmp.sQuestReward[i].iRewardID);
			if(itemTmp == null)
			{
				RewardIcons[i+1].gameObject.SetActive(false);
				RewardCounts[i+1].gameObject.SetActive(false);
				continue;
			}
			SetEachReward(i+1,itemTmp.ItemIcon,newQuestTmp.sQuestReward[i].iRewardCount);
			ItemTmps[i+1] = itemTmp;
		}
		//重排獎勵
		GridRewards.Reposition();
		//設定按鍵狀態
		if(bComplete)
		{
			lbActive.text = GameDataDB.GetString(960); //領取獎勵
			lbMissionTarget.text = GameDataDB.GetString(1173); //任務完成
			spriteTip.gameObject.SetActive(true);
		}
		else
		{
			lbActive.text = GameDataDB.GetString(959); //前往
		}
		//創角類型的判別
		if(bFinish == true)
		{
			lbActive.text = GameDataDB.GetString(961);	//已領取
			spriteTip.gameObject.SetActive(false);
			btnActive.isEnabled = false;
		}
		else
		{
			btnActive.isEnabled = true;
		}

		this.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	private int GetQuestIconID(int iconID)
	{
		int QuestIconID = QuestIconIDplus+iconID;
		return QuestIconID;
	}
	//-------------------------------------------------------------------------------------------------
	//聯結相關事件
	public void LinkToMission()
	{
		ENUM_QuestTarget_Type qtType = NewQDataTmp.emTarget;
		switch(qtType)
		{
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_Dungeon:
			int DungeonID = NewQDataTmp.iCondition;
			S_Dungeon_Tmp DungeonTmp = GameDataDB.DungeonDB.GetData(DungeonID);
			switch(DungeonTmp.StageType)
			{
			case ENUM_StageType.ENUM_StageType_Normal:
			case ENUM_StageType.ENUM_StageType_hard:
				//檢查關卡解鎖條件, 任務未解= 鎖住+防點擊
				bool isUnlock = false;
				S_QuestData_Tmp questDBF = null;
				if(DungeonTmp.iUnlockQuestID >= 0)
				{
					questDBF = GameDataDB.QuestDB.GetData(DungeonTmp.iUnlockQuestID);
					//判斷任務是否已達解鎖關卡
					if(questDBF!=null && questDBF.iPreFlag>=0)
					{
						if (ARPGApplication.instance.m_RoleSystem.sBaseQuestFlag.Get(questDBF.iPreFlag))
						{
							if(DungeonTmp.iGroupRank == ENUM_GroupRank_Type.ENUM_Rank_Hard)
							{
								if(CheckHardRankUnlockStatus(DungeonTmp.iGroup))
									isUnlock = true;
								else
								{
									isUnlock = false;
								}
							}
							else
								isUnlock = true;
						}
						else
						{
							isUnlock = false;
						}
					}
				}
				//
				if(isUnlock)
				{
					ChooseDungeonState cds = ARPGApplication.instance.GetGameStateByName(GameDefine.CHOOSEDUNGEON_STATE) as ChooseDungeonState;
					if(cds != null)
						cds.bEnterSecondUI = false;
					ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.m_ExitGRankType 	= DungeonTmp.iGroupRank;
					ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.m_ExitChapter 		= DungeonTmp.iGroup;
					ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.m_ExitStage		= DungeonTmp;
					ARPGApplication.instance.PushState(GameDefine.CHOOSEDUNGEON_STATE);
				}
				else
				{
					ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(473));	//關卡尚未解鎖
				}
				break;
			case ENUM_StageType.ENUM_StageType_Activity:
			case ENUM_StageType.ENUM_StageType_Money:
			case ENUM_StageType.ENUM_StageType_EXP:
			case ENUM_StageType.ENUM_StageType_PVP:
				ARPGApplication.instance.PushState(GameDefine.ACTIVITYMENU_STATE);
				break;
			case ENUM_StageType.ENUM_StageType_PEAK_PVP:
				ARPGApplication.instance.PushState(GameDefine.PEAKARENA_STATE);
				break;
			case ENUM_StageType.ENUM_StageType_Tower:
				ARPGApplication.instance.PushState(GameDefine.TOWERTRAIL_STATE);
				break;
			}
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_RoleLv:
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_Skill:
			ARPGApplication.instance.PushState(GameDefine.TALENT_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_Pet:
			ARPGApplication.instance.PushState(GameDefine.SETPETPICTURE_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_PetLv:
			ARPGApplication.instance.PushState(GameDefine.SETBATTLEPET_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_PetLimitLv:
			ARPGApplication.instance.PushState(GameDefine.SETPETPICTURE_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_FormationLV:
			ARPGApplication.instance.PushState(GameDefine.FORMATION_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_EqStrengthen:
			ARPGApplication.instance.PushState(GameDefine.ITEMBAG_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_EqMelting:
			ARPGApplication.instance.PushState(GameDefine.ITEMBAG_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_EqUpStar:
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_RoleEqRank:
			ARPGApplication.instance.PushState(GameDefine.ITEMBAG_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_PetEqRank:
			ARPGApplication.instance.PushState(GameDefine.ITEMBAG_STATE);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_PeakPvpRank:
			JsonSlot_Dungeon.Send_CtoM_GetActivitData();
			ARPGApplication.instance.m_ActivityMgrSystem.SetCheckPeakPVPDataFlag(true);
			break;
		case ENUM_QuestTarget_Type.ENUM_QuestTarget_Type_GuildJoin:
			ARPGApplication.instance.PushState(GameDefine.GUILDV2_STATE);
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------
	private bool CheckHardRankUnlockStatus(int iHardChapter)
	{
		if(iHardChapter == 1)
			return true;
		
		GameDataDB.DungeonDB.ResetByOrder();
		for(int i=0; i<GameDataDB.DungeonDB.GetDataSize(); ++i)
		{
			S_Dungeon_Tmp dbf = GameDataDB.DungeonDB.GetDataByOrder();
			if(dbf.iGroupRank == ENUM_GroupRank_Type.ENUM_Rank_Normal &&
			   dbf.iGroup == iHardChapter &&
			   dbf.DungeonIconSize == ENUM_DungeonIconSize.ENUM_DungeonIconSize_big)
			{
				RoleStageData rsData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetStageData(dbf.GUID);
				if(rsData != null && rsData.iStar>0)
					return true;
			}
		}
		return false;
	}
	//-------------------------------------------------------------------------------------------------
	private void SetEachReward(int index,int spriteGUID,int icount)
	{
		Utility.ChangeAtlasSprite(RewardIcons[index],spriteGUID);
		RewardCounts[index].text = "*"+ icount.ToString();
		RewardIcons[index].gameObject.SetActive(icount>0);
		RewardCounts[index].gameObject.SetActive(icount>0);
	}
	//-------------------------------------------------------------------------------------------------
	public void ShowChestItemDetail(S_Item_Tmp dbf)
	{
		if(dbf == null)
			return;
		
		ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(dbf.GUID , 1);
		EventDelegate.Add(ARPGApplication.instance.m_uiItemTip.ButtonFullScreen.onClick , CloseItemInfo);
	}
	//-----------------------------------------------------------------------------------------------------
	//關閉物品資訊,由UI_ItemTip呼叫
	public void CloseItemInfo()
	{
		EventDelegate.Remove(ARPGApplication.instance.m_uiItemTip.ButtonFullScreen.onClick , CloseItemInfo);
	}
	//-----------------------------------------------------------------------------------------------------
}
