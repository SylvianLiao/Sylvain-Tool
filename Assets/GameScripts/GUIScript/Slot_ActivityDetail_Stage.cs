using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_ActivityDetail_Stage : NGUIChildGUI  
{
	public UISprite			SpriteStageBG			= null;				// 底板
	public UILabel			LabelStageName			= null;				// 關卡名稱
	public UIButton			ButtonBattle			= null;				// 挑戰按鈕
	public UILabel			LabelBattle				= null;				// 挑戰文字
	public UISprite			SpriteCostAPIcon		= null;				// AP圖樣
	public UILabel			LabelCostAP				= null;				// 消耗AP
	public Transform[]		itemSlotLocal			= new Transform[3]; // 道具格定位點

	//slot
	public string	slotName 		= "Slot_Item";
	public List<Slot_Item> itemSlotList = new List<Slot_Item>();	// 道具格slot	
//	int dungeonID = -1;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_ActivityDetail_Stage";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_ActivityDetail_Stage() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialSlot();
	}

	//-------------------------------------------------------------------------------------------------
	public void InitialSlot()
	{
		CreateItemSlot();
	}
	//-------------------------------------------------------------------------------------------------
	void CreateItemSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_Item"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_Item go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}

		//取資料
//		ActivityDetailState state = ARPGApplication.instance.GetGameStateByName(GameDefine.ACTIVITYDETAIL_STATE) as ActivityDetailState;
//		S_Activity data = state.GetActivityData();
		S_Activity data = ARPGApplication.instance.m_ActivityMgrSystem.GetSelectActivityData();

		List<int> dIDList = ARPGApplication.instance.m_ActivityMgrSystem.GetDungeonListByActivityID(data.iActivityDBID);
		S_Dungeon_Tmp dungeonDBF = GameDataDB.DungeonDB.GetData(dIDList[0]);
		if(dungeonDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動副本編號錯誤 副本編號 {0}", dIDList[0]));
			return ;
		}

		//Slot
		for(int i=0; i<dungeonDBF.ShowChestItem.Length; ++i)
		{
			Slot_Item newgo= Instantiate(go) as Slot_Item;

			newgo.transform.parent			= itemSlotLocal[i].transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;//itemSlotLocal[i].localPosition;
			newgo.gameObject.SetActive(true);
			
			newgo.name = string.Format("slotItem{0:00}",i);
			
			newgo.InitialSlot();
			itemSlotList.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetSlot(int dungeonID)
	{
		S_Dungeon_Tmp dungeonDBF = GameDataDB.DungeonDB.GetData(dungeonID);
		if(dungeonDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動副本編號錯誤 副本編號 {0}", dungeonID));
			return ;
		}
		// 關卡名稱
		LabelStageName.text = GameDataDB.GetString(dungeonDBF.iName);				
		// 消耗AP
		LabelCostAP.text = dungeonDBF.iAP.ToString();
		int lv;
		//等級不夠關閉按鈕
		if(!ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckLevelLimit(dungeonID, out lv))
		{
			ButtonBattle.gameObject.SetActive(false);
			LabelBattle.text = string.Format("{1}{0}", lv, GameDataDB.GetString(2802));	//進入等級
		}
		else if(!ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckUnlockQuest(dungeonID))
		{
			ButtonBattle.gameObject.SetActive(false);
			LabelBattle.text = string.Format("{0}", GameDataDB.GetString(2830));	//任務未完成
		}
		else if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetChallengeTimes(dungeonID) <=0)
		{
			ButtonBattle.gameObject.SetActive(false);
			LabelBattle.text = string.Format("{0}", GameDataDB.GetString(2831));	//已挑戰過
		}
/*		else if(ARPGApplication.instance.m_ActivityMgrSystem.CheckActivityValue() <=0)
		{
			ButtonBattle.gameObject.SetActive(false);
			LabelBattle.text = string.Format("{0}", GameDataDB.GetString(2831));	//已挑戰過
		}*/
		else
		{
			ButtonBattle.gameObject.SetActive(true);
			LabelBattle.text = GameDataDB.GetString(2801);	//挑戰
		}

		// slot(道具 獎勵 掉落物)
		for(int i=0;i<itemSlotList.Count; ++i)
		{
			//有設定
			if(dungeonDBF.ShowChestItem[i]>0)
			{
				itemSlotList[i].SetActive(true);
				itemSlotList[i].SetSlotWithCount(dungeonDBF.ShowChestItem[i], 0, false);
			}
			else
			{
				itemSlotList[i].SetActive(false);
			}
		}
	}

	//-------------------------------------------------------------------------------------------------

}