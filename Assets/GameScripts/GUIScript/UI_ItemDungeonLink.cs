using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_ItemDungeonLink : NGUIChildGUI 
{
	public UIPanel			panelItemDungeonLink		= null;
	public UISprite			SpriteItemInfoBG			= null;
	public UILabel			lbTitle						= null;
	public UIButton			ButtonClose					= null;
	public UISprite			SpriteIcon					= null;
	public UILabel			LabelItemName				= null;
	public UILabel			lbType						= null;
	public UIButton			ButtonFullScreen			= null;
	public UIGrid			Grid						= null;
	public List<Slot_DungeonLinkInfo>		DLinks		= new List<Slot_DungeonLinkInfo>();
	public Transform		TSIconLocal					= null;
	private string			slotName 					= "Slot_Item";
	[HideInInspector]
	public Slot_Item		itemSlot					= null;
	//
	private bool			bFromChooseDungeon			= false;
	private S_Dungeon_Tmp	RecordDBF					= null;
	//temp
	private List<int>		templList			= new List<int>(); //暫用特例排除不在道具格顯示數量清單
	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ItemDungeonLink";
	//-------------------------------------------------------------------------------------------------
	private UI_ItemDungeonLink() : base(GUI_SMARTOBJECT_NAME)
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
		CreatSlot();
		UIEventListener.Get(ButtonClose.gameObject).onClick	+= HideItemDungeonLink;
		//
		for(int i=0;i<GameDefine.ITEM_DROPDUNGEON_MAX;++i)
		{
			UIEventListener.Get(DLinks[i].btnInfo.gameObject).onClick	+= TransferToChooseDungeon;
		}
		lbTitle.text = GameDataDB.GetString(471);	//"取得方式"
	}
	//-------------------------------------------------------------------------------------------------
	private void HideItemDungeonLink(GameObject gb)
	{
		Hide();	
	}
	//-------------------------------------------------------------------------------------------------
	private void TransferToChooseDungeon(GameObject gb)
	{
		int DungeonDBFID = (int)gb.GetComponent<UIButton>().userData;
		if(DungeonDBFID <=0)
			return;

		S_Dungeon_Tmp DungeonDBF = GameDataDB.DungeonDB.GetData(DungeonDBFID);
		if(DungeonDBF == null)
			return;

		//設定進入關卡地圖時該進的模式與關卡
		ENUM_GroupRank_Type GRType = DungeonDBF.iGroupRank;
		ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.m_ExitGRankType = GRType;
		ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.iChapter = DungeonDBF.iGroup;
		ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.stageDBF = DungeonDBF;

		if(ARPGApplication.instance.CheckCurrentGameStates(GameDefine.CHOOSEDUNGEON_STATE))
		{
			RecordDBF = DungeonDBF;
			bFromChooseDungeon = true;
		}

		//隱藏自己
		Hide();
		//隱藏上一層
		if(ARPGApplication.instance.m_uiItemTip.IsVisible())
			ARPGApplication.instance.m_uiItemTip.HideItemTip();

		//關閉自動導引狀態
		LobbyState ls = ARPGApplication.instance.GetGameStateByName(GameDefine.LOBBY_STATE) as LobbyState;
		if(ls != null)
			ls.SetAutoMission(ENUM_AutoMission.AM_Close);

		//開啟第二層介面
		ChooseDungeonState cds = ARPGApplication.instance.GetGameStateByName(GameDefine.CHOOSEDUNGEON_STATE) as ChooseDungeonState;
		if(cds != null)
			cds.bEnterSecondUI = true;

		//進入關卡地圖到指定關卡
		if(ARPGApplication.instance.CheckCurrentGameStates(GameDefine.LOBBY_STATE))
		{
			ARPGApplication.instance.PushStateAndPopStates(GameDefine.CHOOSEDUNGEON_STATE);
		}
		else
		{
			//非在大廳時(副本)使用
			ARPGApplication.instance.PushStateAndPopLastState(GameDefine.CHOOSEDUNGEON_STATE);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//關卡地圖限定使用
	public void CheckOverwriteRecordStage()
	{
		if(bFromChooseDungeon)
		{
			//設定進入關卡地圖時該進的模式與關卡
			ENUM_GroupRank_Type GRType = RecordDBF.iGroupRank;
			ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.m_ExitGRankType	= GRType;
			ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.iChapter			= RecordDBF.iGroup;
			ARPGApplication.instance.m_ActivityMgrSystem.m_RecordOpenDungeon.stageDBF			= RecordDBF;
		}
		bFromChooseDungeon = false;
		RecordDBF = null;
	}
	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		//UIEventListener.Get(ButtonFullScreen.gameObject).onClick += HideItemDungeonLink;
		//
	}
	//-------------------------------------------------------------------------------------------------
	//產生道具
	void CreatSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_Item";
		}
		
		Slot_Item go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Item load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		//Slot
		itemSlot = Instantiate(go) as Slot_Item;
		
		itemSlot.transform.parent			= TSIconLocal;
		itemSlot.transform.localScale		= Vector3.one;
		itemSlot.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		itemSlot.transform.localPosition	= Vector3.zero;
		itemSlot.gameObject.SetActive(true);
		
		itemSlot.name = string.Format("slotItem00");
		
		itemSlot.InitialSlot();
		//暫用
		templList = itemSlot.specialList;
	}
	//-------------------------------------------------------------------------------------------------
	/*void ShowItemTmp(S_Item_Tmp dbf)
	{
		if(dbf == null)
			return;
		
		if(!IsVisible())
		{
			//檢查特殊顯示名稱
			if(templList.Contains(dbf.GUID) && itemSlot.iCount >0)
			{
				//還原Label顏色
				LabelItemName.color = Color.white;
				LabelItemName.text = string.Format("{0} {1}", itemSlot.iCount, dbf.GetNameWithColor());
			}
			else
			{
				LabelItemName.text = GameDataDB.GetString(dbf.iName);
				dbf.SetRareColor(LabelItemName);
			}
			//顯示
			Show();
		}
	}*/
	//-------------------------------------------------------------------------------------------------
	private void CloseItemDungeonLink(GameObject gb)
	{
		if(IsVisible())
		{
			Hide ();
		}
	}
	//-------------------------------------------------------------------------------------------------
	void ShowItemTmp(int guid)
	{
		S_Item_Tmp dbf = GameDataDB.ItemDB.GetData(guid);
		if(dbf == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("道具格顯示TIP錯誤 道具編號{0}", guid));
			return;
		}
		
		if(!IsVisible())
		{
			//init
			lbType.gameObject.SetActive(false);
			//檢查特殊顯示名稱
			if(templList.Contains(guid) && itemSlot.iCount >1)
			{
				//還原Label顏色
				LabelItemName.color = Color.white;
				LabelItemName.text = string.Format("{0} {1}", itemSlot.iCount, dbf.GetNameWithColor());
			}
			else
			{
				LabelItemName.text = GameDataDB.GetString(dbf.iName);
				dbf.SetRareColorString(LabelItemName);
			}
			//物品類型
			if(dbf.iItemNote>0)
			{
				lbType.gameObject.SetActive(true);
				lbType.text = GameDataDB.GetString(dbf.iItemNote);
			}


			//物品掉落關卡按鈕設定
			S_Dungeon_Tmp DungeonDBF;
			for(int i=0;i<GameDefine.ITEM_DROPDUNGEON_MAX;++i)
			{
				if(dbf.iDropDungeonID[i]<=0)
				{
					DLinks[i].gameObject.SetActive(false);
					continue;
				}
				//
				DungeonDBF = GameDataDB.DungeonDB.GetData(dbf.iDropDungeonID[i]);
				if(DungeonDBF == null)
					return;

				DLinks[i].gameObject.SetActive(true);
				DLinks[i].btnInfo.userData = dbf.iDropDungeonID[i];
				DLinks[i].SetLinkData(DungeonDBF);
			}
			Grid.Reposition();
			/*
			//物品星等顯示
			if((int)dbf.RareLevel == 0)
			{
				for(int i=0; i<emptyStars.Count; ++i)
				{
					emptyStars[i].gameObject.SetActive(false);
					
					stars[i].gameObject.SetActive(false);
				}
			}
			else
			{
				for(int i=0; i<stars.Count; ++i)
				{
					stars[i].gameObject.SetActive(false);
					if(i<(int)dbf.RareLevel)
					{
						stars[i].gameObject.SetActive(true);
					}
				}
			}*/
			//顯示
			Show();
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void ShowItemDungeonLink(int guid)
	{
		itemSlot.itemGUID  	  = guid;
		itemSlot.iCount  	  = 1;
		//		itemSlot.specialList  = slot.specialList;
		
		itemSlot.SetSlotWithCount(itemSlot.itemGUID, itemSlot.iCount , false);
		
		ShowItemTmp(itemSlot.itemGUID);
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
}
