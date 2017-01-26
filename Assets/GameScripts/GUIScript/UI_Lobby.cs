using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_Lobby : NGUIChildGUI
{
	//Info
	public UIButton			btnAddGold			= null;	//儲值
	public UILabel			lbGold				= null;	//商城幣
	public UILabel			lbMoney				= null;	//遊戲幣
	public UILabel			lbRolePower			= null;	//戰力
	public UILabel			lbFP				= null;	//好友點數
	public UILabel			lbAP				= null;	//體力值
	public UILabel			lbLV				= null;	//等級
	public UIButton			btnPVE				= null;	//自動任務
	public UILabel			lbPVE				= null; //任務名稱
	public UILabel			lbRoleName			= null; //角色名稱
	public Slot_RoleIcon	roleIcon		    = null; //角色圖示
    public UISprite			spriteVIP		    = null; //VIP圖示
	public UIProgressBar	barRoleExp			= null;	//角色經驗值條
	//Right
	public UIWidget			widgetRight			= null;	//RightSet
	public UIButton			btnActivity			= null;	//活動
	public UISprite			spriteActivityTip	= null; //活動系統的提示
	public UISprite			spritePVPTip		= null; //PVP系統的提示
	public UIButton			btnValuePVP			= null;	//文字競技
	public UIButton			btnDungeon			= null;	//關卡
	public UIButton			btnGuild			= null;	//公會
	public UISprite			SpriteGuildTip		= null;	//公會系統的提示
	public UIButton			btnTowerTrail		= null; //通天塔
	public UISprite			spTowerTip			= null; //通天塔提示
    public UIButton         btnNewQuest         = null; //新任務系統
    public UIButton         btnNewQuest2        = null; //新任務系統2
    public UISprite         spNotifyNewQuest    = null; //新任務系統提示
    public UISprite         spNotifyNewQuest2   = null; //新任務系統提示2
	//Top
	public UIWidget			widgetTop			= null;	//TopRightSet
	public UIButton         btnDayActive        = null;	//活躍度介面
	public UISprite			spNotifyDayActive   = null; //活躍度獎勵提示
	public UIButton			btnCashTree			= null;	//搖錢樹
	public UIButton			btnStoreMenu		= null;	//VIP商城
	public UIButton			btnEveryDayDiamond	= null;	//領取每日寶石
	public UIButton			btnLoginReward		= null;	//每日登入獎勵
    public UIButton			btnFreeAP	        = null;	//免費體力
    public UISprite			spNotifyFreeAP      = null; //免費體力提示
	public UIButton			btnLottery			= null; //幸運轉輪
	public UILabel			lbLottery			= null; //幸運轉輪字樣
	//Bottom
	public UIWidget			widgetBottom		= null;	//BottomSet
	public UIButton			btnBag				= null;	//背包
	public UIButton			btnSummon			= null;	//召喚
	public UIButton			btnSetBattlePet		= null;	//隊伍
	public UIButton			btnPicture			= null;	//圖鑑
	public UISprite			spriteMarkPet		= null; //碎片系統的提示 
    public UIButton			btnFormation		= null;	//戰陣
	public UIButton			btnTalent			= null; //天賦
	//TipMark
	public UIWidget			wgBagTip			= null; //背包裝備欄有物品可升階或強化提示
	public UIWidget			wgSummonTip			= null; //召喚提示
	public UIWidget			wgTalentTip			= null; //天賦技能升級提示
	public UIWidget			wgSetBattlePetTip	= null; //寵物技能升級提示
	public UIWidget			wgFormationTip		= null;	//戰陣升級提示
	//Left
	public UIButton			btnMail				= null;	//信箱
	public UIWidget			wgNewMailTip		= null; //新信提示集合
	public UISprite			spNewMailTip		= null; //新信提示
	public UISprite			spStillHaveMailTip	= null; //還有信件未領提示
	public UILabel			lbNotifyNewMail		= null; 
	public UILabel			lbMailNum			= null; //信件數量
	public UIButton			btnChat				= null;	//聊天室按鈕
	public UILabel			lbbubuChat			= null;	//聊天泡泡內容
	public UISprite			spriteChatRM		= null;	//接收密語提醒
	public int				ChatMessageNum		= 8;	//留天泡泡字數上限

	public UIButton			btnFriend			= null;	//好友
	public UISprite			spFriendTip			= null;	//好友提示
	//商店清單
	public UIPanel			panelStoreMenu		= null;	//商店清單集合
	public Animation		animStoreMenu		= null; //商店清單的位移動畫
	public UIGrid			gdStoreMenu			= null; //排列商店清單用
	public UILabel			lbStoreMenuTitle	= null; //商店清單標題
	public UIButton			btnRefreshStore		= null; //刷新商店按鈕
	public UILabel			lbRefreshStore		= null; 
	public UIButton			btnPvPStore			= null;	//PVP商店按鈕
	public UILabel			lbPvPStore			= null;
    public UIButton         btnVipStore         = null;	//VIP商店按鈕
    public UILabel          lbVipStore          = null;
    public UIButton         btnOperational      = null;	//營運獎勵按鈕
    public UILabel          lbOperational       = null;	
    public UISprite			spOperationalTip	= null; //獎勵提示
	public UIButton			btnInvestment		= null; //投資理財按鈕
	public UILabel			lbInvestment		= null; 
	public UIWidget			wgInvestmentTip		= null; //投資理財提示
	public UIButton			btnCloseStoreMenu	= null;	

	//解鎖按鈕會依附於這些清單紀錄按鈕順序於按鈕上
	public List<UIButton> 	d_BottomButtons 	= new List<UIButton>();//底部按鈕集合
	public List<UIButton> 	d_RightButtons 		= new List<UIButton>();//右側按鈕集合
	public List<UIButton> 	d_TopButtons 		= new List<UIButton>();//上方按鈕集合
	//已解鎖按鈕清單
	public List<UIButton>	m_BotBtnArdyUnlock 	= new List<UIButton>();
	public List<UIButton>	m_RightBtnArdyUnlock = new List<UIButton>();
	public List<UIButton>	m_TopBtnArdyUnlock 	= new List<UIButton>();
	public UISprite 		spriteLock 			= null;
	//[HideInInspector]
	//public Dictionary<ENUM_LOCKBTN,GameObject>	gbLockList 			= new Dictionary<ENUM_LOCKBTN,GameObject>();//鎖的串列
	//
	public UISprite			spriteBlackBG		= null; //壓黑冪
	//
	public UIButton			btnGM				= null;	//開關GM介面

	//
	public UIButton			btnResize			= null;	//縮放
	public float			timer				= 0.1f;	//縮放時間
	public UISprite			spriteResizePoint	= null; //縮放圖樣
	//************
	//按鈕名稱
	//************
    public UILabel lbAddGold            = null;	//儲值
    public UILabel lbEveryDayDiamond    = null;	//每日寶石
    public UILabel lbFreeAP             = null;	//免費領取寶石
    public UILabel lbStoreMenu          = null;	//商店
    public UILabel lbLoginReward        = null;	//每日登入
    public UILabel lbGetAP              = null;	//免費寶石
    public UILabel lbCastTree           = null;	//搖錢樹
    public UILabel lbActivity           = null;	//活動
    public UILabel lbDayActive          = null; //每日任務 
    public UILabel lbMail               = null;	//信箱
    public UILabel lbValuePVP           = null;	//競技
    public UILabel lbDungeon            = null;	//關卡
    public UILabel lbTalent             = null;	//天賦
    public UILabel lbBag                = null;	//背包
    public UILabel lbSetBattlePet       = null;	//隊伍
    public UILabel lbPicture            = null;	//圖鑑
    public UILabel lbSummon             = null;	//招換
    public UILabel lbFormation          = null;	//戰陣
    public UILabel lbChatRoom           = null;	//聊天室
	public UILabel lbFriend         	= null;	//好友
	public UILabel lbTowerTrail			= null; //通天塔
    public UILabel lbNewQuest           = null; //新任務系統
    public UILabel lbNewQuest2          = null; //新任務系統
	public UILabel lbGuild				= null;	//公會

	//************
	//指引教學相關元件
	//************
	public UIPanel			panelGuide				= null; //指引集合
	public UISprite			spriteGuideBlackBG		= null; //教學背景壓黑用的底圖
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen 			= null; //全螢幕按鍵
	public UISprite			spriteGuideBottom		= null; //指引框
	public UILabel			lbGuideBottom			= null; //指引說明
	public UISprite			spriteGuideAutoMission	= null; //自動導引
	public UILabel			lbGuideAutoMission		= null; 
	public GameObject 		gobjectGuideOnlyEx 		= null; //無箭頭及框的指引框
	public UILabel 			lbExplanationOnlyEx 	= null; //無箭頭及框的說明
	public UISprite			spriteGuideRight		= null; //活動指引框
	public UILabel			lbGuideRight			= null; 
	public UISprite			spriteGuideLeft			= null; //活動指引框
	public UILabel			lbGuideLeft				= null; 
	public UISprite			spriteGuideTop			= null; //指引框
	public UILabel			lbGuideTop				= null; //指引說明
	public UISprite			spGuideExpBar			= null; //導引玩家經驗值
	public UILabel			lbGuideExpBar			= null; 
	public UISprite			spGuideShowAP			= null; //導引體力值
	public UILabel			lbGuideShowAP			= null; 
	public UISprite			spGuideRoleInfo			= null; //導引玩家頭像資訊
	public UILabel			lbGuideRoleInfo			= null; 
	public UISprite			spGuideDepositBtn		= null; //導引儲值按鈕
	public UILabel			lbGuideDepositBtn		= null; 
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Lobby";

	//-----------------------------------------------------------------------------------------------------
	private UI_Lobby() : base(GUI_SMARTOBJECT_NAME)
	{
	}
    //-------------------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
        spriteVIP.spriteName = "VIP" + ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetVIPRank().ToString();
    }
	//-------------------------------------------------------------------------------------------------------------
	//
	public void Update()
	{
		S_PlayerData_Tmp dbf = GameDataDB.PlayerDB.GetData(ARPGApplication.instance.m_RoleSystem.iBaseLevel);

		lbGold.text		= ARPGApplication.instance.m_RoleSystem.iBaseItemMallMoney.ToString();
		lbMoney.text	= ARPGApplication.instance.m_RoleSystem.iBaseBodyMoney.ToString();
		lbFP.text		= ARPGApplication.instance.m_RoleSystem.iBaseFP.ToString();
		lbAP.text		= ARPGApplication.instance.m_RoleSystem.iBaseAP.ToString() + "/" + dbf.iMaxAP.ToString();
		lbLV.text		= ARPGApplication.instance.m_RoleSystem.iBaseLevel.ToString();
		//
		lbRolePower.text = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetMainRolePower().ToString();
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
        lbAddGold.text = GameDataDB.GetString(15054);		//儲值    
        lbEveryDayDiamond.text = GameDataDB.GetString(15055);//每日寶石
		lbStoreMenu.text = GameDataDB.GetString(312);		//商店清單  
        lbLoginReward.text = GameDataDB.GetString(15057);	//每日登入    
        lbGetAP.text = GameDataDB.GetString(15058);			//系統設置    
        lbCastTree.text = GameDataDB.GetString(15059);		//搖錢樹   
        lbActivity.text = GameDataDB.GetString(15060);		//活動   
        lbDayActive.text = GameDataDB.GetString(15061);		//每日任務   
        lbMail.text = GameDataDB.GetString(15062);		    //信箱   
        lbValuePVP.text = GameDataDB.GetString(15063);		//競技   
        lbDungeon.text = GameDataDB.GetString(15064);		//關卡    
        lbTalent.text = GameDataDB.GetString(15065);		//天賦 
        lbBag.text = GameDataDB.GetString(15066);		    //背包    
        lbSetBattlePet.text = GameDataDB.GetString(15067);	//隊伍   
        lbPicture.text = GameDataDB.GetString(15068);		//圖鑑    
        lbSummon.text = GameDataDB.GetString(15069);		//招換 
        lbFormation.text = GameDataDB.GetString(15070);		//戰陣  
        lbChatRoom.text = GameDataDB.GetString(15071);		//聊天室
		lbFriend.text = GameDataDB.GetString(5200);			//好友
		lbInvestment.text = GameDataDB.GetString(573);		//投資理財
		lbLottery.text = GameDataDB.GetString(940);			//幸運轉輪
		lbTowerTrail.text = GameDataDB.GetString(543);		//通天塔
        lbNewQuest.text = GameDataDB.GetString(963);		//任務
        lbNewQuest2.text = GameDataDB.GetString(964);		//七日任務
		lbbubuChat.text = "";

		lbGuild.text = GameDataDB.GetString(15083);			//公會

		//商店清單
		lbStoreMenuTitle.text = GameDataDB.GetString(312);	//商店清單  
		lbRefreshStore.text = GameDataDB.GetString(15056);	//雜貨商店   
		lbPvPStore.text = GameDataDB.GetString(2833);		//PVP商店	
		lbVipStore.text = GameDataDB.GetString(315);		//商城

        lbOperational.text = GameDataDB.GetString(574);		//獎勵  
		base.Initialize();

		//將底部按鈕設集合
		AddBottomBtnsInList();
		//將右側按鈕設集合
		AddRightBtnsInDict();
		//將上方按鈕設集合
		AddTopBtnsInDict();

		//指定鎖頭圖示
		Utility.ChangeAtlasSprite (spriteLock, 129000);
		//隱藏鎖頭樣版
		spriteLock.gameObject.SetActive(false);

		if (ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.sRoleItemMallData.iMonthly == 1 &&
		    ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.sRoleItemMallData.iMonthlyGet == 0 )
			btnEveryDayDiamond.gameObject.SetActive(true);
		else
			btnEveryDayDiamond.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	 IEnumerator RemoveTweenPos()
	{
		yield return new WaitForSeconds(timer);
		for(int i=0;i<d_BottomButtons.Count;++i)
			GameObject.Destroy(d_BottomButtons[i].gameObject.GetComponent<TweenPosition>());
	}
	//-----------------------------------------------------------------------------------------------------
	//移掉tweenpos效果
	public void RemovePosEffect()
	{
		StartCoroutine("RemoveTweenPos");

	}
	//-----------------------------------------------------------------------------------------------------
	//將右側按鈕加入Dictionary容器中
	private void AddRightBtnsInDict()
	{
		for(int i =0 ; i < d_RightButtons.Count; i++)
		{
			LockBtnData posData = new LockBtnData();
			posData.ButtonPos = ENUM_LOCKBTN_POSITION.RIGHT;
			posData.PositionIndex = i;
			d_RightButtons[i].userData = posData;
			d_RightButtons[i].gameObject.SetActive(true);
		}
			
	}
	//-----------------------------------------------------------------------------------------------------
	//將底部按鈕加入清單管理並記錄各按鈕的順序
	private void AddBottomBtnsInList()
	{
		for(int i =0 ; i < d_BottomButtons.Count; i++)
		{
			LockBtnData posData = new LockBtnData();
			posData.ButtonPos = ENUM_LOCKBTN_POSITION.BOTTOM;
			posData.PositionIndex = i;
			d_BottomButtons[i].userData = posData;
			d_BottomButtons[i].gameObject.SetActive(true);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//將右側按鈕加入Dictionary容器中
	private void AddTopBtnsInDict()
	{
		for(int i =0 ; i < d_TopButtons.Count; i++)
		{
			LockBtnData posData = new LockBtnData();
			posData.ButtonPos = ENUM_LOCKBTN_POSITION.TOP;
			posData.PositionIndex = i;
			d_TopButtons[i].userData = posData;
			d_TopButtons[i].gameObject.SetActive(true);
		}
	}
	//-----------------------------------------------------------------------------------------------------
}
