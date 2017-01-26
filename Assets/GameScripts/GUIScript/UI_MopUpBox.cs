using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;


public class MopUpRewardData
{
	public S_Item_Tmp itemData = null;
	public int itemCount = 0;
}

public class UI_MopUpBox : NGUIChildGUI 
{
	private const string GUI_MOPUPBOX_NAME  = "UI_MopupBox";
	
	//---------------------------------UI Prefab上的變數----------------------------------------
	public UIPanel			panelBase;			//此UI的Panel
	//-----------------------------掃蕩選擇視窗-------------------------
	public UIButton			btnFullScreen		= null;     //用來擋住其他點擊功能的全螢幕透明按鈕
	public GameObject   	gMopUpChoose		= null; 	//選擇掃蕩視窗 , 預設開啟
	public UILabel 			lbChooseTitle		= null;		//選擇掃蕩標題
	public UILabel 			lbOneMopUp			= null;
	public UIButton 		btnOneMopUp			= null;		//單次掃蕩
	public UIButton 		btnManyMopUp		= null;		//多次掃蕩
	public UILabel 			lbManyMopUp			= null;
	public UILabel  		lbVipRemind			= null;		//提醒VIP多少會開啟多次掃蕩功能
	public UIButton			btnBuyChests		= null;		//購買額外寶箱點選處
	public UISprite			spCheckBuyChests	= null;		//購買額外寶箱示意圖
	public UILabel 			lbBuyChests			= null;		//購買寶箱文字
	public UILabel 			lbBuyChestsCost		= null; 	//購買寶箱消耗文字
	public UIButton 		btnCloseChoose		= null;		//關閉選擇掃蕩視窗
	//-----------------------------掃蕩結果視窗-------------------------
	public GameObject   	gMopUpResult		= null; 	//掃蕩結果視窗 , 預設關閉
	public UILabel 			lbResultTitle		= null;		//掃蕩結果標題
	public UIWidget			wgBottomBtn			= null;		//下方按鈕的集合
	public UIGrid			gdBottomBtn			= null;		
	public UIButton 		btnMopUpAgain		= null; 	//再次掃蕩
	public UILabel 			lbMopUpAgain		= null;
	public UIButton 		btnResultConfirm	= null; 	//獎勵確認(等同關閉結果視窗)
	public UILabel 			lbResultConfirm		= null; 	
	public UIButton 		btnCloseResult		= null;		//關閉掃蕩結果視窗
	//-----------------------------ScrollView樣板-------------------------
	public UIPanel 			panelResultList		= null;		//結果清單集合
	public GameObject 		gResultUnit			= null;		//每次掃蕩結果複製用樣板 , 預設Set false
	public Slot_MopUpReward slotResultUnit 		= null;		//樣板中的各個獎勵物品連結
	public UILabel 			lbMopUpProgress		= null;		//掃蕩進度標題
	public UIGrid  			gdGrid				= null;		//排序掃蕩結果用
	//-----------------------------接收Server資料清單---------------------------------
	public List<MopUpRewardData>  m_RewardDataList   = new List<MopUpRewardData>();	//單次掃蕩獎勵物品清單
	//--------------------------------------------------------------------------------------------
	public List<GameObject>	m_ResultUnitList	= new List<GameObject>();	//複製出來的掃蕩結果樣版清單
	//----------------------------執行用變數-----------------------------------
	private ChooseDungeonState chooseDgState	= null;
	private Vector3	 		m_HaveTweenedPos 	= Vector3.zero;  			//目前TweenPosition到的位置
	private Vector3			m_ScrollViewPos 	= Vector3.zero;				//用於儲存Panel scrollview的初始位置
	public  float 			m_TweenHeight 		= 392.0f; 					//一次TweenPosition的高度
	private const float 	m_ResultShowTime	= 2.0f;   					//每次掃蕩結果顯示時間
	private const float     m_WaitForServerTime = 1.5f;						//掃蕩完成後等待server修改client資料的時間
	private const float 	m_EachTweenTime		= 0.5f;   					//每次掃蕩結果往上移的時間
	private int 			m_BuyChestsCount 	= 0;						//實際購買寶箱次數
	[HideInInspector]
	public	int 			m_MopUpCount 		= 0; 						//掃蕩次數
	private int 			m_MopUpNumber 		= 0;						//目前進行第幾次掃蕩
	[HideInInspector]
	public  int 			m_MoneyTextureID	= 10002;
	[HideInInspector]
	public  int 			m_ExpTextureID		= 10005;

	//--------------------------------------------------------------------------------------------
	private UI_MopUpBox() : base(GUI_MOPUPBOX_NAME)
	{
	}
	void Start()
	{/*
		//測試用
		for (int i = 0 ; i < m_RewardArray.Length ; i++)
		{
			m_RewardArray[i] = gRewardList.transform.GetChild(i);
		}
		gResultUnit.SetActive (false);
		InitMopUpResult (false, 1);
		for(int i =0 ; i< m_MopUpCount ; i++)
			SpawnResultUnit ();*/
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization , call by GuiManager
	public override void Initialize()
	{
		InitMopUpInfo ();
		slotResultUnit.Initialize();
		base.Initialize();
	}
	//-----------------------------------------------------------------------------------------------------
	private void InitMopUpInfo()
	{
		chooseDgState = (ChooseDungeonState)ARPGApplication.instance.GetGameStateByName(GameDefine.CHOOSEDUNGEON_STATE);
		slotResultUnit = gResultUnit.GetComponent<Slot_MopUpReward>();
		//設定選擇掃蕩視窗字串
		lbChooseTitle.text 		= GameDataDB.GetString (2109);	//關卡掃蕩
		lbVipRemind.text 		= GameDataDB.GetString (2111);	//多次掃蕩功能於Vip3開啟
		lbBuyChests.text 		= GameDataDB.GetString (2112);	//購買寶箱(可抽四箱)
		int buyChestsCost = 0;
		buyChestsCost =	GameDataDB.ShopPrizeDB.GetData(GameDefine.ITEMMALL_SPEEDCLEAR_CHEST_MONEY_ID).GetPrize(0);
		lbBuyChestsCost.text = buyChestsCost.ToString();

		//設定掃蕩結果視窗字串
		lbResultConfirm.text 	= GameDataDB.GetString (2116);	//確定
		//一些會受到prefab影響的初始設定, 以防萬一
		gResultUnit.GetComponent<BoxCollider>().enabled = false;
		m_ScrollViewPos = panelResultList.gameObject.transform.localPosition;
		gMopUpChoose.SetActive (false);
		gMopUpResult.SetActive (false);

		gdGrid.hideInactive = false;
		gdGrid.enabled = false;
		gdBottomBtn.enabled = false;
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Show()
	{
		SwitchMopUpBox (true);
		base.Show();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		SwitchMopUpBox (false);
		base.Hide ();
	}
	//-----------------------------------------------------------------------------------------
	//開關掃蕩UI
	public void SwitchMopUpBox(bool bSwitch)
	{
		//跟著開關選擇掃蕩視窗
		SwitchMopUpChoose(bSwitch);
		//跟著開關TopStateView的功能
		chooseDgState.uiTopStateView.SwitchBtnWork (!bSwitch);
		//跟著開關章節左右滑移的功能
		chooseDgState.uiChooseDungeon.CDChapterBase.m_wgChooseChapter.gameObject.SetActive(!bSwitch);
		//關閉掃蕩UI後將變數Reset
		if (!bSwitch)
		{
			chooseDgState.bBuyFourChests = false;
			RaiseScreenColliderDepth(false);
		}
		else
		{
			TweenPosition twPos = gMopUpChoose.GetComponent<TweenPosition>();
			TweenAlpha twAlpha = gMopUpChoose.GetComponent<TweenAlpha>();
			twPos.enabled = true;
			twPos.ResetToBeginning();
			twAlpha.enabled = true;
			twAlpha.ResetToBeginning();
		}
		this.gameObject.SetActive (bSwitch);
	}
	//-----------------------------------------------------------------------------------------
	//開關掃蕩選擇視窗
	public void SwitchMopUpChoose(bool bSwitch)
	{
		if (bSwitch)
		{
			//開啟時須做的相關檢查
			chooseDgState.UpdateShowingUI(chooseDgState.CalculateChallengeRemnant());
			int buyChestsCost = GameDataDB.ShopPrizeDB.GetData(GameDefine.ITEMMALL_SPEEDCLEAR_CHEST_MONEY_ID).GetPrize(0);
			if (buyChestsCost < 0)
				return;
			if (chooseDgState.bBuyFourChests && ARPGApplication.instance.m_RoleSystem.iBaseItemMallMoney < buyChestsCost)
				chooseDgState.bBuyFourChests = false;
			//判斷Vip是否足夠開啟多次掃蕩功能
			if (ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetVIPRank() >= GameDefine.DUNGEON_MOPUP_VIPLIMIT) //暫訂
				btnManyMopUp.isEnabled = true;
			else
				btnManyMopUp.isEnabled = false;
		}
		gMopUpChoose.gameObject.SetActive (bSwitch);
	}
	//-----------------------------------------------------------------------------------------
	//開關掃蕩結果視窗 , call by JsonSlot_Dungeon
	public void SwitchMopUpResult(bool bSwitch)
	{
		if (!bSwitch)
			ResetSpawnData();
		gMopUpResult.SetActive (bSwitch);
		SwitchMopUpChoose(!bSwitch);
	}
	//-----------------------------------------------------------------------------------------------------
	//是否提高全螢幕collider的depth
	public void RaiseScreenColliderDepth(bool bSwitch)
	{
		if (bSwitch)
			btnFullScreen.transform.FindChild("Background").GetComponent<UISprite>().depth = gMopUpResult.GetComponent<UIWidget>().depth +10 ;
		else
			btnFullScreen.transform.FindChild("Background").GetComponent<UISprite>().depth = gMopUpResult.GetComponent<UIWidget>().depth -10 ;
	}
	//-----------------------------------------------------------------------------------------------------
	#region  樣板初始化、生成複製、還原
	//初始化掃蕩結果視窗
	/// <summary>
	/// Inits the mop up result.
	/// </summary>
	public void InitMopUpResult(bool isMopingUp , int mopUpCount)
	{
		//暫存掃蕩次數
		m_MopUpCount = mopUpCount;
		//掃蕩中或是掃蕩完成
		if (isMopingUp)
		{
			lbResultTitle.text = GameDataDB.GetString (2114);	//關卡掃蕩中...
			btnResultConfirm.gameObject.SetActive(false);
			btnMopUpAgain.gameObject.SetActive(false);
			btnCloseResult.gameObject.SetActive(false);
			//掃蕩中擋住除了scrollview之外的點擊功能
			RaiseScreenColliderDepth(true);
			//計算實際購買了幾次額外寶箱
			if (chooseDgState.bBuyFourChests)
			{
				int buyChestsCost = GameDataDB.ShopPrizeDB.GetData(GameDefine.ITEMMALL_SPEEDCLEAR_CHEST_MONEY_ID).GetPrize(0);
				if (buyChestsCost < 0)
					return;
				m_BuyChestsCount = ARPGApplication.instance.m_RoleSystem.iBaseItemMallMoney	/ buyChestsCost;
				if (m_BuyChestsCount > m_MopUpCount)
					m_BuyChestsCount = m_MopUpCount;
			}
		}
		else
		{
			lbResultTitle.text = GameDataDB.GetString (2115);	//掃蕩完成
			//等待server送到client的資料更新
			StartCoroutine(WaitForServerSendingData());
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//複製每次掃蕩結果樣版, call by JsonSlot_Dungeon
	public void SpawnResultUnit()
	{
		m_MopUpNumber++;

		//每次掃蕩結果UI初始設定
		if (m_MopUpCount == 1)
			lbMopUpProgress.text = GameDataDB.GetString(2122);	//開始掃蕩
		else
		{
			string mopUpProgress = string.Format (GameDataDB.GetString (2118), m_MopUpNumber.ToString());//第N次掃蕩
			lbMopUpProgress.text = mopUpProgress; 
		}
		//實際購買幾次就顯示幾次額外的寶箱 
		if (m_MopUpNumber > m_BuyChestsCount)
		{
			for (int i = 3 ; i < slotResultUnit.m_RewardArray.Length ; i++)	//第三個開始是額外寶箱
				slotResultUnit.m_RewardArray[i].gameObject.SetActive(false);
			//將購買寶箱取消
			chooseDgState.bBuyFourChests = false;
		}

		//換獎勵物品圖示
		for(int i =0 ; i< slotResultUnit.m_RewardArray.Length ; i++)
		{
			if (slotResultUnit.m_RewardArray[i] == null || m_RewardDataList[i].itemData == null)
				continue;

			slotResultUnit.m_RewardArray[i].SetSlotWithCount(m_RewardDataList[i].itemData.GUID ,  m_RewardDataList[i].itemCount , true);
		}
		//複製樣板
		GameObject newgo = GameObject.Instantiate(gResultUnit) as GameObject;
		Slot_MopUpReward slotCloned = newgo.GetComponent<Slot_MopUpReward>();
		m_ResultUnitList.Add(newgo);
		newgo.transform.parent = gdGrid.transform;
		newgo.transform.localScale = Vector3.one;
		newgo.transform.position  = gResultUnit.transform.position;
		//將複製出來的掃蕩結果加入點擊監聽，並暫時移除點擊功能
		for(int i = 0 ; i < slotCloned.m_RewardArray.Length ; ++i)
		{
			slotCloned.m_RewardArray[i].ButtonSlot.userData = m_RewardDataList[i];	//將獎勵資料分別儲存於各個Button內
			slotCloned.m_RewardArray[i].ButtonSlot.GetComponent<BoxCollider>().enabled = false;
			UIEventListener.Get(slotCloned.m_RewardArray[i].ButtonSlot.gameObject).onClick += AddRewardOnClick;
		}
		//最後一次掃蕩結果生成完
		if (m_MopUpNumber == m_MopUpCount)
		{
			//預設將第一次掃蕩打開
			m_ResultUnitList[0].SetActive(true);

			//排序並顯示掃蕩結果
			gdGrid.enabled = true;	
			gdGrid.Reposition();
			if (m_MopUpNumber == 1)
				InitMopUpResult(false , m_MopUpCount);			//單次掃蕩顯示掃蕩完成
			else if (m_MopUpNumber != 0)
				StartCoroutine(ActiveTweenPosition());			//多次掃蕩開始自動捲動ScrollView
/*
			//檢查首次獲得寵物
			if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckFirstTimeGetPet()>0)
			{

			}
			else
			{
				//檢查角色升級
				if(ARPGApplication.instance.m_RoleSystem.bLvUpFlag)
				{
					ARPGApplication.instance.PushState(GameDefine.LEVELUPINFOBASE_STATE);
				}
			}
*/
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void AddRewardOnClick(GameObject go)
	{
		MopUpRewardData reward = (MopUpRewardData)go.GetComponent<UIButton>().userData;
		ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(reward.itemData.GUID , reward.itemCount);
	}
	//-----------------------------------------------------------------------------------------------------
	//掃蕩結果自動往上捲
	private IEnumerator ActiveTweenPosition()
	{
		yield return new WaitForSeconds(m_ResultShowTime);
		//設定Tween Positon參數
		TweenPosition twPos = gdGrid.GetComponent<TweenPosition>();
		twPos.enabled = false;
		Vector3 vec = m_HaveTweenedPos;
		twPos.from = m_HaveTweenedPos;
		vec.y += m_TweenHeight;
		m_HaveTweenedPos = vec;
		twPos.to = m_HaveTweenedPos;
		twPos.duration = m_EachTweenTime;

		twPos.enabled = true;
		twPos.ResetToBeginning ();

		EventDelegate.Add(twPos.onFinished , SettingWhenTweenOver);
	}
	//-----------------------------------------------------------------------------------------------------
	//每次tween完要做的事
	private void SettingWhenTweenOver()
	{
		TweenPosition twPos = gdGrid.GetComponent<TweenPosition>();
		EventDelegate.Remove(twPos.onFinished , SettingWhenTweenOver);
		//播放取得獎勵音效
		//MusicControlSystem.PlaySound("Sound_System_022" , 1);
		//依Tween的順序打開相對應的掃蕩次數
		int number = (int)(m_HaveTweenedPos.y / m_TweenHeight);
		if (number  < 0)
		{
			UnityDebugger.Debugger.Log("掃蕩結果捲動顯示該次掃蕩錯誤, 讀取位置= "+(number -1));
			return;
		}
		m_ResultUnitList [number].SetActive (true);
		//若目前Tween的高度幾乎等於預設要Tween的高度 , 便停止TweenPosition
		if ((m_TweenHeight * (m_MopUpCount-1))	- m_HaveTweenedPos.y < 1.0f)
			InitMopUpResult(false , m_MopUpCount);	//顯示掃蕩完成
		else 
			StartCoroutine(ActiveTweenPosition());	//繼續Tween
	}
	//-----------------------------------------------------------------------------------------------------
	//等待server更新client資料後才做動作
	private IEnumerator WaitForServerSendingData()
	{
		//掃蕩完成UI的設定
		wgBottomBtn.alpha = 0.0f;
		if (m_MopUpCount  == 1)
		{
			btnMopUpAgain.transform.parent = gdBottomBtn.transform;
			btnMopUpAgain.gameObject.SetActive(true);
		}
		else
		{
			btnMopUpAgain.transform.parent = gMopUpResult.transform;
			btnMopUpAgain.gameObject.SetActive(false);
		}
		gdBottomBtn.Reposition();
		btnResultConfirm.gameObject.SetActive(true);
		btnCloseResult.gameObject.SetActive(true);
		yield return new WaitForSeconds (m_WaitForServerTime);
		//顯示底部按鈕
		wgBottomBtn.alpha = 1.0f;
		//還原除了scrollview之外的點擊功能
		RaiseScreenColliderDepth(false);
		//將scrollview點擊功能還原
		for (int i = 0; i < m_ResultUnitList.Count ; i++)
		{
			m_ResultUnitList[i].GetComponent<BoxCollider>().enabled = true;
			Slot_MopUpReward slotCloned = m_ResultUnitList[i].GetComponent<Slot_MopUpReward>();
			for (int m = 0; m < slotCloned.m_RewardArray.Length ; m++)
				slotCloned.m_RewardArray[m].ButtonSlot.GetComponent<BoxCollider>().enabled = true;
		}
		//更新掃蕩相關UI顯示
		chooseDgState.UpdateShowingUI(chooseDgState.CalculateChallengeRemnant());
	}
	//-----------------------------------------------------------------------------------------------------
	//還原樣板資料
	public void ResetSpawnData()
	{
		if (m_ResultUnitList.Count <=0 || slotResultUnit.m_RewardArray.Length <= 0)
			return;
		//清掉臨時儲存複製後的樣版
		for (int i = 0 ; i < m_ResultUnitList.Count ; i++)
			DestroyImmediate(m_ResultUnitList[i].gameObject);
		m_ResultUnitList.Clear();
		//顯示全部的獎勵圖示 , 預設是打開的
		for (int i = 0 ; i < slotResultUnit.m_RewardArray.Length ; i++)
			slotResultUnit.m_RewardArray[i].gameObject.SetActive(true);
		//樣板變數還原
		m_MopUpCount 		= 0; 			
		m_MopUpNumber 		= 0; 		
		m_BuyChestsCount 	= 0;		
		gdGrid.transform.localPosition = Vector3.zero;
		m_HaveTweenedPos  = gdGrid.transform.localPosition;
		//scrollview位置還原
		panelResultList.transform.localPosition = m_ScrollViewPos;
		panelResultList.clipOffset = Vector2.zero;
		SpringPanel spPanel = panelResultList.GetComponent<SpringPanel>();
		if (spPanel != null)
			DestroyImmediate(spPanel);
	}
	#endregion
}
