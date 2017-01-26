using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameFramework;

public class UI_LoginReward : NGUIChildGUI
{
    public UIPanel panelMonthInfo;
    public UIGrid monthInfoGrid;
    List<Slot_Item> m_SlotDayList = new List<Slot_Item>();

	public GameObject getRewardIcon;
    public GameObject maskIcon;
    public UILabel dayLabel;

    public UILabel TitleLabel;
    public UILabel TipLabel;
    public UILabel monthLabel;
    public UILabel monthInfoLabel;

    //隔日獎勵
	[Header("Right")]
	public UISprite	nextDayRewardItemIcon;	//明日獎勵圖示
	public UISprite	nextDayRewardItemMask;	//明日獎勵邊框
//	public UISprite	nextDayRewardItemBG 	= null;	//明日獎勵底圖
    public UILabel NextDayTitleLabel;
    public UILabel NextDayItemNameLabel;
    public UILabel NextDayItemInfoLabel;
    public UISprite[] spriteNextDayStars       = new UISprite[7]; //寵物星數
    public UIButton closeButton;
    public UILabel lbClose;
	public UIButton RetroactiveButton;		//補登按鈕
	public UILabel lbRetroactive;			//補登
    //今日獎勵介面
	[Header("Left")]
    public UIPanel TodayPanel;
    public UISprite TodayRewardItemIcon;		//今日獎勵圖示
	public UISprite TodayRewardItemMask;		//今日獎勵邊框
    public UILabel LabelTodayItemName;
	public UILabel LabelTodayCount;
	public UILabel LabelTodayGet;
    public UIButton ButtonToday;
    public List<UIButton> ButtonRewardList = new List<UIButton>();
	private List<GameObject> goRewardCheckList = new List<GameObject>();
    public UISprite HighFrameSprite;
    public UILabel LabelInfoCount;

	//補登介面
	[Header("Retroactive")]
	public UIWidget wgRetroactive;		//補登資訊介面
	public UIButton btnRetInfoClose;	//補登資訊關閉按鈕
	public UIButton btnRetBuyOnce;		//補登買一次
	public UILabel	lbRetBuyOnce;		
	public UIButton btnRetBuyAll;		//補登買全部
	public UILabel	lbRetBuyAll;
	public UILabel	lbRetInfo;			//補增資訊
	public UILabel	lbRetOncePrice;		//補增買一次金額
	public UILabel	lbRetAllPrice;		//補增買全部金額

    // 物品詳細資訊
	[Header("ItemInfo")]
    public GameObject ItemInfo          = null;  //物品資訊相關
    public UIButton btnCloseItemInfo    = null; //關閉物品資訊按鈕
    public UISprite spriteIcon          = null; //物品圖樣
    public UILabel  lbItemName          = null; //物品名稱
    public UISprite[] spriteStars       = new UISprite[7]; //寵物星數
    public UILabel  lbItemNote          = null; //物品介紹

	private int finishLoginDays;
	private float xStart = -405;
	private float yStart = 155;
	private float xMove = 82.4f;
	private float yMove = 82.5f;
	private float xEnd = 110;
	private float yEnd = -194;
	private int   m_PetPieceID = 1140;  //寵物碎片圖示編號

	private bool activeReward = false;
	private GameObject loginRewardToday;
	private Vector3 oriScale;

	private const string rewardItemMaskName = "Sprite(Mask)";
    private const string 	m_SlotName			="Slot_Item";

	//************
	//指引教學相關元件
	//************
	[Header("Guide")]
	public UIPanel			panelGuide				= null; //指引集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen 			= null; //全螢幕按鍵
	public UISprite			spGuideTodayReward		= null; //導引今日獎勵
	public UILabel			lbGuideTodayReward		= null; 
	public UISprite			spGuideAllReward		= null; //導引整月獎勵內容
	public UILabel			lbGuideAllReward		= null; 
	public UISprite			spGuideTomorrowReward	= null; //導引明日獎勵
	public UILabel			lbGuideTomorrowReward	= null; 
	public UISprite			spGuideEnterGame		= null; //導引進入遊戲
	public UILabel			lbGuideEnterGame		= null; 

    //-----------------------------------------------------------------------
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_LoginReward";
	//-------------------------------------------------------------------------------------------------------------
	private UI_LoginReward() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	// Use this for initialization
    //-------------------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
        TitleLabel.text = GameDataDB.GetString(2205);
        NextDayTitleLabel.text = GameDataDB.GetString(2202); 
        TipLabel.text = GameDataDB.GetString(2207);
        lbClose.text = GameDataDB.GetString(15053);  //進入遊戲
		lbRetroactive.text = GameDataDB.GetString(970); //補簽
		lbRetBuyOnce.text = GameDataDB.GetString(971); //補簽一次
		lbRetBuyAll.text = GameDataDB.GetString(972); //全部補簽

		wgRetroactive.gameObject.SetActive(false);
    }
	//------------------------------------------------------------------------
	public void SetData ()
	{
		finishLoginDays = ARPGApplication.instance.m_RoleSystem.iLoginDays;

		SetupRewardThisMonth();
		SetupRewardCalendar();
	}
	//------------------------------------------------------------------------
	// Update is called once per frame
	void Update ()
	{

	}
	//------------------------------------------------------------------------
	//設定目前登入領取狀況
	void SetupRewardCalendar()
	{
        DateTime moment = DateTime.Now;
		for(int i = 0;i < finishLoginDays;i++)
		{
			if(getRewardIcon != null)
			{
             /*   GameObject rewardIcon = null;
                if(i < finishLoginDays)
				    rewardIcon = Instantiate(getRewardIcon) as GameObject;
                else
				    rewardIcon = Instantiate(maskIcon) as GameObject;

				rewardIcon.transform.parent = m_SlotDayList[i].transform;
                rewardIcon.transform.localPosition = Vector3.zero;
                rewardIcon.transform.localScale = Vector3.one;
				rewardIcon.name = "RewardDay"+(i+1);
				rewardIcon.SetActive(true);*/

			}
		}
/*		RewardShow(moment.Day);*/
	}
	//------------------------------------------------------------------------
	//設定本月登入領取的內容
	void SetupRewardThisMonth()
	{
        DateTime moment = DateTime.Now;
        int days = DateTime.DaysInMonth(moment.Year, moment.Month);
		int month = DateTime.Now.Month;
		monthLabel.text = month.ToString();
        ButtonRewardList.Clear();

		for(int i=1;i<=days;i++)
		{
            //從DBF取得獎勵資料設定
            int LoginRewardGUID = moment.Month * 100 + i;
			S_LoginReward_Tmp loginRewardTmp = GameDataDB.LoginRewardDB.GetData(LoginRewardGUID);
			if(loginRewardTmp == null) continue;

            GameObject go = ResourceManager.Instance.GetGUI(m_SlotName);
            GameObject newgo= Instantiate(go) as GameObject;
			newgo.transform.parent = monthInfoGrid.transform;
			newgo.transform.localPosition = Vector3.zero;
			newgo.name = "RewardDay"+(i);
			newgo.transform.localScale = Vector3.one;
			newgo.SetActive(true);
    
            Slot_Item item = newgo.GetComponent<Slot_Item>();

            item.ButtonSlot.userData = LoginRewardGUID;
            ButtonRewardList.Add(item.ButtonSlot);

            S_Reward_Tmp rewarddbf = GameDataDB.RewardDB.GetData(loginRewardTmp.iRewardID);          
            if (rewarddbf == null)
            {
                UnityDebugger.Debugger.LogError("No this RewardDBF data with RewardDBID" + loginRewardTmp.iRewardID.ToString());
                return;
            }

            item.SetSlotWithCount(rewarddbf.ItemGUID,rewarddbf.Count,false);

            //設定日期
            GameObject daytemp = null;
            daytemp = Instantiate(dayLabel.gameObject) as GameObject;
            UILabel day = daytemp.GetComponent<UILabel>();
            day.text = i.ToString();
            daytemp.transform.parent = newgo.transform;
            daytemp.transform.localPosition = new Vector3(-35,28,0);
            daytemp.transform.localScale = Vector3.one;
            daytemp.name = "Day" + i;
            daytemp.SetActive(true);


			if(getRewardIcon != null)
			{
				GameObject rewardIcon = null;
				rewardIcon = Instantiate(getRewardIcon) as GameObject;
				
				rewardIcon.transform.parent = newgo.transform;
				rewardIcon.transform.localPosition = Vector3.zero;
				rewardIcon.transform.localScale = Vector3.one;
				rewardIcon.name = "RewardDay"+(i+1);
				rewardIcon.SetActive(i <= finishLoginDays);
				goRewardCheckList.Add(rewardIcon);
			}

            UIDragScrollView drag = newgo.gameObject.GetComponent<UIDragScrollView>();
            drag.scrollView = panelMonthInfo.GetComponent<UIScrollView>();
            m_SlotDayList.Add(item);
		}
        monthInfoGrid.enabled = true;

        TodayRewardShow(finishLoginDays);

        //設定隔天獎勵
        int InfoRewardGUID = 0;
        if (DateTime.DaysInMonth(moment.Year, moment.Month) < finishLoginDays + 1)
        {
            InfoRewardGUID = (moment.Month + 1) * 100 + 1;
        }
        else
            InfoRewardGUID = moment.Month * 100 + (finishLoginDays + 1);
        NextDayRewardInfoShow(InfoRewardGUID);
	}
	//------------------------------------------------------------------------
	public void UpdateFinishDay()
	{
		finishLoginDays = ARPGApplication.instance.m_RoleSystem.iLoginDays;
	}
	//------------------------------------------------------------------------
	//刷新圈圈
	public void RefreshRewardCheck()
	{
		for(int i = 0;i<goRewardCheckList.Count;++i)
		{
			goRewardCheckList[i].SetActive(i < finishLoginDays);
		}
	}
	//------------------------------------------------------------------------
	public void RefreshNextReward()
	{
		DateTime moment = DateTime.Now;
		//設定隔天獎勵
		int InfoRewardGUID = 0;
		if (DateTime.DaysInMonth(moment.Year, moment.Month) < finishLoginDays + 1)
		{
			InfoRewardGUID = (moment.Month + 1) * 100 + 1;
		}
		else
			InfoRewardGUID = moment.Month * 100 + (finishLoginDays + 1);
		NextDayRewardInfoShow(InfoRewardGUID);


		RefreshMonthLabelInfo(moment.Month,finishLoginDays);
	}
	//------------------------------------------------------------------------
	public void RefreshMonthLabelInfo(int month,int day)
	{
		monthLabel.text = month.ToString();
		monthInfoLabel.text = string.Format( GameDataDB.GetString(304), day);
	}
	//------------------------------------------------------------------------
// 	void RewardShow(int days)
// 	{
// 		loginRewardToday = Instantiate(getRewardIcon) as GameObject;
// 		loginRewardToday.transform.parent = getRewardIcon.transform.parent;
// 		loginRewardToday.transform.localPosition = new Vector3(rewardPosList[days-1].x,rewardPosList[days-1].y,getRewardIcon.transform.localPosition.z);
// 		loginRewardToday.name = "RewardDay"+(days);
// 		loginRewardToday.transform.localScale = new Vector3(getRewardIcon.transform.localScale.x*1.5f,getRewardIcon.transform.localScale.y*1.5f,getRewardIcon.transform.localScale.z);
// 		loginRewardToday.SetActive(true);
// 		activeReward = true;
// 	}
	//------------------------------------------------------------------------
    void TodayRewardShow(int day)
    {
        DateTime moment = DateTime.Now;
        int LoginRewardGUID = moment.Month * 100 + (day);
        S_LoginReward_Tmp loginRewardTmp = GameDataDB.LoginRewardDB.GetData(LoginRewardGUID);
        if (loginRewardTmp == null)
            return;
        S_Reward_Tmp rewarddbf = GameDataDB.RewardDB.GetData(loginRewardTmp.iRewardID);
        if (rewarddbf == null)
        {
            UnityDebugger.Debugger.LogError("No this RewardDBF data with RewardDBID" + loginRewardTmp.iRewardID.ToString());
            return;
        }
        S_Item_Tmp itemdbf = GameDataDB.ItemDB.GetData(rewarddbf.ItemGUID);
        if (itemdbf == null)
        {
            UnityDebugger.Debugger.LogError("No this ItemDBF data with ItemDBID" + rewarddbf.ItemGUID.ToString());
            return;
        }
        if (itemdbf.ItemType == ENUM_ItemType.ENUM_ItemType_Pet)
        {
            S_PetData_Tmp petdbf = GameDataDB.PetDB.GetData(itemdbf.iPetID);
            if (petdbf == null)
            {
                UnityDebugger.Debugger.LogError("No this PetDBF data with PetDBID" + itemdbf.iPetID.ToString());
                return;
            }
            Utility.ChangeAtlasSprite(TodayRewardItemIcon, petdbf.AvatarIcon);		//設定圖
            LabelTodayItemName.text = GameDataDB.GetString(petdbf.iName);
			petdbf.SetRareColorString(LabelTodayItemName);

			itemdbf.SetItemRarity(TodayRewardItemMask , null);
        }
        else
        {
			Utility.ChangeAtlasSprite(TodayRewardItemIcon, itemdbf.ItemIcon);		//設定圖
			LabelTodayItemName.text = GameDataDB.GetString(itemdbf.iName);
			itemdbf.SetRareColorString(LabelTodayItemName);
			if (itemdbf.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece)
			{
//				Utility.ChangeAtlasSprite(TodayRewardItemMask, m_PetPieceID);
				itemdbf.SetPetPieceRarity(TodayRewardItemMask , null);
			}
			else
			{
				itemdbf.SetItemRarity(TodayRewardItemMask , null);
			}
        }
		//層級換色
//		itemdbf.SetRareColor(TodayRewardItemMask , null);


        LabelTodayCount.text = rewarddbf.Count.ToString();
        if (ARPGApplication.instance.m_RoleSystem.m_temp.isAutoOpenTodayLoginReward)
            TodayPanel.gameObject.SetActive(true);
        else
			TodayPanel.gameObject.SetActive(false);

        LabelTodayGet.text = GameDataDB.GetString(2257);
        
        monthInfoLabel.text = string.Format( GameDataDB.GetString(304), day);
    }
	//------------------------------------------------------------------------
    public void NextDayRewardInfoShow(int LoginRewardGUID)
    {  
        S_LoginReward_Tmp loginRewardTmp = GameDataDB.LoginRewardDB.GetData(LoginRewardGUID);
        if (loginRewardTmp == null)
            return;

        S_Reward_Tmp rewarddbf = GameDataDB.RewardDB.GetData(loginRewardTmp.iRewardID);
        if (rewarddbf == null)
        {
            UnityDebugger.Debugger.LogError("No this RewardDBF data with RewardDBID" + loginRewardTmp.iRewardID.ToString());
            return;
        }

        S_Item_Tmp itemdbf = GameDataDB.ItemDB.GetData(rewarddbf.ItemGUID);
        if (itemdbf == null)
        {
            UnityDebugger.Debugger.LogError("No this ItemDBF data with ItemDBID" + rewarddbf.ItemGUID.ToString());
            return;
        }

        if (itemdbf.ItemType == ENUM_ItemType.ENUM_ItemType_Pet)
        {
            S_PetData_Tmp petdbf = GameDataDB.PetDB.GetData(itemdbf.iPetID);
            if (petdbf == null)
            {
                UnityDebugger.Debugger.LogError("No this PetDBF data with PetDBID" + itemdbf.iPetID.ToString());
                return;
            }
			Utility.ChangeAtlasSprite(nextDayRewardItemIcon, petdbf.AvatarIcon);		//設定圖
            NextDayItemNameLabel.text = GameDataDB.GetString(petdbf.iName);
			petdbf.SetRareColorString(NextDayItemNameLabel);

			string str = GameDataDB.GetString(itemdbf.iNote);
			str = ARPGApplication.instance.m_StringParsing.Parsing(str,null,SkillLevelType.Now);
			NextDayItemInfoLabel.text = str;

            //NextDayItemInfoLabel.text = GameDataDB.GetString(itemdbf.iNote);
			//層級換色
			//一般道具
			itemdbf.SetItemRarity(nextDayRewardItemMask , null);
        }
        else
        {
			Utility.ChangeAtlasSprite(nextDayRewardItemIcon, itemdbf.ItemIcon);		//設定圖
            NextDayItemNameLabel.text = GameDataDB.GetString(itemdbf.iName);
			itemdbf.SetRareColorString(NextDayItemNameLabel);

			string str = GameDataDB.GetString(itemdbf.iNote);
			str = ARPGApplication.instance.m_StringParsing.Parsing(str,null,SkillLevelType.Now);
			NextDayItemInfoLabel.text = str;
            //NextDayItemInfoLabel.text = GameDataDB.GetString(itemdbf.iNote);

			if (itemdbf.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece)
			{
				//寵物碎片
				itemdbf.SetPetPieceRarity(nextDayRewardItemMask , null);
			}
			else
			{
				//一般道具
				itemdbf.SetItemRarity(nextDayRewardItemMask , null);
			}
        }
		//層級換色
//		itemdbf.SetRareColor(nextDayRewardItemMask , null);

        if (rewarddbf.Count > 1)
        {
            LabelInfoCount.text = rewarddbf.Count.ToString();
            LabelInfoCount.gameObject.SetActive(true);
        }
        else
        {
            LabelInfoCount.gameObject.SetActive(false);
        }
		/*
        //物品星等顯示
        for (int i = 0; i < spriteNextDayStars.Length; ++i)
        {
            spriteNextDayStars[i].gameObject.SetActive(false);
            if (i < (int)itemdbf.RareLevel)
                spriteNextDayStars[i].gameObject.SetActive(true);
        }*/

    }
    //-----------------------------------------------------------------------------------------------------
    public void ShowItemInfo(int LoginRewardGUID)
    {
        S_LoginReward_Tmp loginRewardTmp = GameDataDB.LoginRewardDB.GetData(LoginRewardGUID);
        if (loginRewardTmp == null)
            return;

        S_Reward_Tmp rewarddbf = GameDataDB.RewardDB.GetData(loginRewardTmp.iRewardID);
        if (rewarddbf == null)
        {
			UnityDebugger.Debugger.LogError("No this RewardDBF data with RewardDBID" + loginRewardTmp.iRewardID.ToString());
            return;
        }

        S_Item_Tmp itemdbf = GameDataDB.ItemDB.GetData(rewarddbf.ItemGUID);
        if (itemdbf == null)
        {
			UnityDebugger.Debugger.LogError("No this ItemDBF data with ItemDBID" + rewarddbf.ItemGUID.ToString());
            return;
        }
		ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(itemdbf.GUID , rewarddbf.Count);
		/*
        Utility.ChangeAtlasSprite(spriteIcon, itemdbf.ItemIcon);
        lbItemName.text = GameDataDB.GetString(itemdbf.iName);
        lbItemNote.text = GameDataDB.GetString(itemdbf.iNote);
        //物品星等顯示
        for (int i = 0; i < spriteStars.Length; ++i)
        {
            spriteStars[i].gameObject.SetActive(false);
			if (i < (int)itemdbf.RareLevel)
                spriteStars[i].gameObject.SetActive(true);
        }
        //顯示
        ItemInfo.gameObject.SetActive(true);*/
    }

	//-----------------------------------------------------------------------------------------------------
	public void ShowRetInfo()
	{	
		DateTime moment = DateTime.Now;
		int iLoginDays = ARPGApplication.instance.m_RoleSystem.iLoginDays;
		int iBuyCounts = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.sRoleItemMallData.iBuyLoginRewardCount;
		int iCanRet = Mathf.Min (moment.Day - iLoginDays,GameDefine.ITEMMALL_BUY_LOGINREWARD_MAX - iBuyCounts);

		lbRetInfo.text = string.Format(GameDataDB.GetString(969)
		                               ,iCanRet
		                               ,iBuyCounts
		                               ,GameDefine.ITEMMALL_BUY_LOGINREWARD_MAX);

		S_ShopPrize_Tmp sShopTmp = GameDataDB.ShopPrizeDB.GetData(GameDefine.ITEMMALL_BUY_LOGINREWARD_ID);
		if(sShopTmp != null)
		{
			lbRetOncePrice.text = sShopTmp.GetPrize(iBuyCounts).ToString();
			lbRetAllPrice.text = (sShopTmp.GetPrize(iBuyCounts) * iCanRet).ToString();
		}
		else
		{
			UnityDebugger.Debugger.Log("ShopPrizeDB : 18 is null");
			return;
		}

		wgRetroactive.gameObject.SetActive(true);
	}
}
