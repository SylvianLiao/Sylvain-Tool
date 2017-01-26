using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class DayActiveRewardData
{
	public S_Item_Tmp itemData = null;
	public int itemCount = 0;
}

public class UI_DayActive : NGUIChildGUI
{
	//Common component

	public List<Slot_DayActiveCondition>	ConditionList	= new List<Slot_DayActiveCondition>(); //條件框內容顯示
	//SingleBtnSet
    public GameObject HighFrame         = null;
	public UIButton	btnReward1		    = null; //20%獎勵按鈕
    public UIButton	btnReward2		    = null; //40%獎勵按鈕
    public UIButton	btnReward3		    = null; //60%獎勵按鈕
    public UIButton	btnReward4		    = null; //80%獎勵按鈕
    public UIButton	btnReward5		    = null; //100%獎勵按鈕
    public UIButton	btnGetReward		= null; //領取按鈕
    public UILabel	lbGetReward		    = null; //
    public UILabel	lbPointTitle	    = null; //
    public UILabel	lbAllPoint		    = null; //
    public UILabel	lbTip		        = null; //
    public UILabel	lbEXP		        = null; //
	public UIButton btnCancel			= null; //取消按鈕
	public UIWidget wgRewardItems		= null; //物品集合
	public UIWidget wgLeftReward		= null; //左邊獎勵集合
	public int 		m_AdjustSlotDepth	= 25;	//調整獎勵物品depth用
    public List<UISprite>	CompletePoint = null; //完成度Bar
    public List<UISprite> 	spriteRewardPosList	= new List<UISprite>(); //獎勵物品位置
    public List<UISprite> 	spriteRewardHintList= new List<UISprite>(); //未領獎勵提醒
    public UIScrollView SVConditionList 	= null;
	public UIPanel 		PanelConditionList 	= null;
    public UIGrid 		GridConditionList 	= null;

    public UILabel lbRewardItemTitle		= null; //
	[HideInInspector]
	public List<Slot_Item> 	slotRewardList		= new List<Slot_Item>(); //獎勵物品
    //public List<UILabel> 	lbItemCountList	    = new List<UILabel>(); //獎勵物品數量
	private const string 	m_SlotName			="Slot_Item";
    private const string 	m_SlotConditionName			="Slot_DayActiveCondition";

	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UISprite		spGuideDayActivity				= null; //導引介紹每日任務
	public UILabel		lbGuideDayActivity				= null;
	public UISprite		spGuideTodayProgress			= null; //導引介紹今日任務進度
	public UILabel		lbGuideTodayProgress			= null;
	public UISprite		spGuideClick100Button			= null; //導引點擊任務完成度100%按鈕
	public UILabel		lbGuideClick100Button			= null;
	public UISprite		spGuide100Reward				= null; //導引顯示任務進度100%獎勵
	public UILabel		lbGuide100Reward				= null;

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_DayActive";
	//-----------------------------------------------------------------------------------------------------
	private UI_DayActive() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitDayActive();
	}
    //-----------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
        lbPointTitle.text = GameDataDB.GetString(2254);
        lbTip.text = GameDataDB.GetString(2258);
    }
	//-----------------------------------------------------------------------------------------------------
	private void InitDayActive()
	{
        lbRewardItemTitle.text = GameDataDB.GetString(2166);
		for(int i = 0; i < spriteRewardPosList.Count; ++i)
			spriteRewardPosList[i].gameObject.SetActive(false);

		CreateItemSlot();
	}
    //-----------------------------------------------------------------------------------------------------
    public void SetConditionData()
    {
        ConditionList.Clear();
        for (int i = 0; i < GameDataDB.DayActiveDB.GetDataSize(); ++i)
        {
            GameObject go = ResourceManager.Instance.GetGUI(m_SlotConditionName);
            GameObject newgo = Instantiate(go) as GameObject;
            newgo.transform.parent = GridConditionList.transform;
            newgo.transform.localPosition = Vector3.zero;
            newgo.transform.localScale = Vector3.one;
            newgo.SetActive(true);

            Slot_DayActiveCondition dayActiveCondition = newgo.GetComponent<Slot_DayActiveCondition>();

            S_DayActive_Tmp dayActiveDB = GameDataDB.DayActiveDB.GetData(i + 1);
            if(dayActiveDB == null)
                continue;
            int nowCompleteCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetDayActiveCompleteCount((ENUM_DayActiveType)i + 1);
            dayActiveCondition.LabelCount.text = nowCompleteCount + "/" + dayActiveDB.iCount.ToString();
            dayActiveCondition.LabelConditionContent.text = GameDataDB.GetString(dayActiveDB.iDayActiveinfo);
            dayActiveCondition.LabelPoint.text = "+" + dayActiveDB.iFraction.ToString();
            dayActiveCondition.SpriteProgressBar2.fillAmount = nowCompleteCount * 1.0f / dayActiveDB.iCount;
            if (nowCompleteCount == dayActiveDB.iCount)
            {
                dayActiveCondition.LabelGoto.text = GameDataDB.GetString(2256); //2256 已達成
                dayActiveCondition.ButtonGoto.gameObject.SetActive(false);
                dayActiveCondition.SpriteBG.spriteName = "pic_everyday_c"; // 完成擺暗圖
                dayActiveCondition.LabelConditionContent.color = Color.cyan;
                dayActiveCondition.LabelPoint.color = Color.cyan;
            }
            else
            {
                dayActiveCondition.LabelGoto.text = GameDataDB.GetString(2255);
                dayActiveCondition.ButtonGoto.userData = i + 1;
                dayActiveCondition.ButtonGoto.gameObject.SetActive(true);
                dayActiveCondition.SpriteBG.spriteName = "pic_everyday_n"; // 未完成擺亮圖
                dayActiveCondition.LabelConditionContent.color = Color.white;
                dayActiveCondition.LabelPoint.color = Color.white;
            }

            UIDragScrollView drag = dayActiveCondition.ButtonGoto.GetComponent<UIDragScrollView>();
            drag.scrollView = SVConditionList;

            ConditionList.Add(dayActiveCondition);
        }
        GridConditionList.enabled = true;
    }
    //-----------------------------------------------------------------------------------------------------
    public void SetRewardData(int index)
    {

        S_DayActiveReward_Tmp dayActiveDB = GameDataDB.DayActiveRewardDB.GetData(index);
        if (dayActiveDB != null)
        {
            //設定經驗值
            S_PlayerData_Tmp playerData = GameDataDB.PlayerDB.GetData(ARPGApplication.instance.m_RoleSystem.iBaseLevel);
            if (playerData != null)
            {
				lbEXP.text = GameDataDB.GetString(2260) + (playerData.iDayActiveExp * dayActiveDB.iExp / 100);
            }

			for(int i = 0; i < slotRewardList.Count; ++i)
            {
                if (dayActiveDB.questReward[i].iRewardID == -1)
                {
					slotRewardList[i].gameObject.SetActive(false);
                    continue;
                }
                S_Item_Tmp itemDB = GameDataDB.ItemDB.GetData(dayActiveDB.questReward[i].iRewardID);
                if (itemDB != null)
                {
					slotRewardList[i].SetSlotWithCount(itemDB.GUID , dayActiveDB.questReward[i].iRewardCount , false);
					slotRewardList[i].SetDepth(m_AdjustSlotDepth);
					DayActiveRewardData rewardData = new DayActiveRewardData();
					rewardData.itemData = itemDB;
					rewardData.itemCount = dayActiveDB.questReward[i].iRewardCount;
					slotRewardList[i].ButtonSlot.userData = rewardData;
                    slotRewardList[i].gameObject.SetActive(true);
					UIEventListener.Get(slotRewardList[i].ButtonSlot.gameObject).onClick += AddItemOnClick;
					/*
					Utility.ChangeAtlasSprite(spriteItemPosList[i],itemDB.ItemIcon);
					spriteItemPosList[i].gameObject.SetActive(true);
                    lbItemCountList[i].text = dayActiveDB.questReward[i].iRewardCount.ToString();*/
                }
            }
        }     
    }
    //-----------------------------------------------------------------------------------------------------
    //設定目前選取領取的分數框
    public void SetHighFrame(GameObject go)
    {
        HighFrame.transform.position = go.transform.position;
    }
    //-----------------------------------------------------------------------------------------------------
    //設定分數的Bar條
    public void ShowCompletePoint(int nowPoint)
    {
        int point = 0;
        point = nowPoint;
        for (int i = 0; i < CompletePoint.Count; ++i)
        {
            CompletePoint[i].fillAmount = 0.0f;
        }
        for (int i = 0; i < CompletePoint.Count; ++i)
        {
            if (point > 20)
            {
                CompletePoint[i].fillAmount = 1.0f;
                point -= 20;
            }
            else
            {
                CompletePoint[i].fillAmount = point * 1.0f / 20;
                return;
            }
        }
    }
    //-----------------------------------------------------------------------------------------------------
    public void SetFinishIcon(int nowPoint)
    {
        string closeIcon = "pic_everyday3";
        string OpenIcon = "pic_everyday";
        switch(nowPoint / 20)
        {
            case 5:
                btnReward5.normalSprite = OpenIcon;
                btnReward4.normalSprite = OpenIcon;
                btnReward3.normalSprite = OpenIcon;
                btnReward2.normalSprite = OpenIcon;
                btnReward1.normalSprite = OpenIcon;
            break;
            case 4:
                btnReward5.normalSprite = closeIcon;
                btnReward4.normalSprite = OpenIcon;
                btnReward3.normalSprite = OpenIcon;
                btnReward2.normalSprite = OpenIcon;
                btnReward1.normalSprite = OpenIcon;
            break;
            case 3:
                btnReward5.normalSprite = closeIcon;
                btnReward4.normalSprite = closeIcon;
                btnReward3.normalSprite = OpenIcon;
                btnReward2.normalSprite = OpenIcon;
                btnReward1.normalSprite = OpenIcon;
            break;
            case 2:
                btnReward5.normalSprite = closeIcon;
                btnReward4.normalSprite = closeIcon;
                btnReward3.normalSprite = closeIcon;
                btnReward2.normalSprite = OpenIcon;
                btnReward1.normalSprite = OpenIcon;
            break;
            case 1:
                btnReward5.normalSprite = closeIcon;
                btnReward4.normalSprite = closeIcon;
                btnReward3.normalSprite = closeIcon;
                btnReward2.normalSprite = closeIcon;
                btnReward1.normalSprite = OpenIcon;
            break;
            case 0:
                btnReward5.normalSprite = closeIcon;
                btnReward4.normalSprite = closeIcon;
                btnReward3.normalSprite = closeIcon;
                btnReward2.normalSprite = closeIcon;
                btnReward1.normalSprite = closeIcon;
            break;
        }
    }
    public void SetRewardHint(int nowPoint)
    {
        int RewardCount = 0;
        int count = 0;
        RewardCount = nowPoint / 20;
        for (int i = (int)ENUM_DayActiveType.ENUM_DayActiveType_State1; i <= (int)ENUM_DayActiveType.ENUM_DayActiveType_State5; ++i, count++)
        {
            int beGet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetDayActiveCompleteCount((ENUM_DayActiveType)i);
            if (RewardCount > count)
            {
                if (!Convert.ToBoolean(beGet))
                {
                    spriteRewardHintList[count].gameObject.SetActive(true);
                    continue;
                }
            }
            spriteRewardHintList[count].gameObject.SetActive(false);
        }
    }
    //-----------------------------------------------------------------------------------------------------
    public void SetGetRewardBtn(int nowPoint, int nowChooseRewardPoint)
    {
        // 設定獎勵按鈕
        int index = (int)ENUM_DayActiveType.ENUM_DayActiveType_State1 + (nowChooseRewardPoint / 20) - 1;
        int beGet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetDayActiveCompleteCount((ENUM_DayActiveType)index);

        if  (Convert.ToBoolean(beGet))
        {
            btnGetReward.enabled = false;
            btnGetReward.state = UIButtonColor.State.Disabled;
            lbGetReward.text = GameDataDB.GetString(2251);
        }
        else if (nowPoint < nowChooseRewardPoint)
        {
            btnGetReward.enabled = false;
            btnGetReward.state = UIButtonColor.State.Disabled;
            lbGetReward.text = GameDataDB.GetString(2252);
        }
        else
        {
            btnGetReward.enabled = true;
            lbGetReward.text = GameDataDB.GetString(2253);
        }
    }
	//-----------------------------------------------------------------------------------------------------
	private void CreateItemSlot()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		//Slot
		for(int i=0; i < spriteRewardPosList.Count; ++i)
		{
			Slot_Item newgo= Instantiate(go) as Slot_Item;
			newgo.transform.parent			= wgRewardItems.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition 	= spriteRewardPosList[i].transform.localPosition;

			newgo.name = string.Format("slotItem{0:00}",i);
			newgo.gameObject.SetActive(true);
			slotRewardList.Add(newgo);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void AddItemOnClick(GameObject go)
	{
		DayActiveRewardData rewardData = (DayActiveRewardData)go.GetComponent<UIButton>().userData;
		ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(rewardData.itemData.GUID , rewardData.itemCount);
	}
}

