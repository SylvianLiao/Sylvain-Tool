using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

class UI_VIPNote : NGUIChildGUI 
{
	public UIPanel		panelBase				= null;		//自身Panel
	public UIPanel		panelVIPNote			= null;	
	public UIScrollView	svVIPNote				= null;		
	public UISprite		spVIPNoteTitle			= null;		
	public UILabel		lbVIPNote				= null;		

	public UILabel 		lbVIPNoteContent		= null;	
	public UIGrid 		gdVIPRewards			= null;	
	public UIButton		btnRightChoose			= null;		
	public UIButton		btnLeftChoose			= null;		
	public UIButton		btnClose				= null;	
	//-----------------------------------------------------------------------------------------------------
	[HideInInspector]
	public List<Slot_Item> 		m_RewardSlotList	= new List<Slot_Item>();	//VIP獎勵物品
    [System.NonSerialized]
	public List<S_Reward_Tmp> 	m_VipRewardList		= new List<S_Reward_Tmp>(); //VIP獎勵資料
	//-----------------------------執行用變數-------------------------------------------------------
	[HideInInspector]
	public int 			m_NowVipShowing			= -1;	//目前顯示是多少等級的VIP說明
	private const string 	m_SlotName			= "Slot_Item";
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel				panelGuide					= null; //教學集合
	public UIButton				btnTopFullScreen			= null; //最上層的全螢幕按鈕
	public UIButton				btnFullScreen				= null; //全螢幕按鈕
	public UISprite				spGuideShowVipNote			= null; //導引介紹VIP特權
	public UILabel				lbGuideShowVipNote			= null;
	//-----------------------------------------------------------------------

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME 	= "UI_VIPNote";
	//-----------------------------------------------------------------------------------------------------
	private UI_VIPNote() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitVipNote();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	public void InitVipNote()
	{
		AssignNoteContent(m_NowVipShowing);
		ShowVipReward(m_NowVipShowing);
		ShowSwitchBtn();
	}
	//-----------------------------------------------------------------------------------------------------
	public void NextVipNote(GameObject go)
	{
		AssignNoteContent(++m_NowVipShowing);
		ShowVipReward(m_NowVipShowing);
		ShowSwitchBtn();
		svVIPNote.ResetPosition();
	}
	//-----------------------------------------------------------------------------------------------------
	public void PreVipNote(GameObject go)
	{
		AssignNoteContent(--m_NowVipShowing);
		ShowVipReward(m_NowVipShowing);
		ShowSwitchBtn();
		svVIPNote.ResetPosition();
	}
	//-----------------------------------------------------------------------------------------------------
	private void AssignNoteContent(int vip)
	{
		if (vip < GameDefine.VIP_LEVEL_MIN || vip > GameDefine.VIP_LEVEL_MAX)
			return;
	
		S_VIPLV_Tmp vipTmp = GameDataDB.VIPLVDB.GetData(vip+1);	//Vip0 = Vip Guid 1 
		//m_NowVipShowing = vip;
		//更新Vip說明內容
		string NoteContent="";

		if (vip > 0)
		{
			int subVipExp = vipTmp.VIPEXP-ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBuyDiamond();
			if (subVipExp>0)
				NoteContent = AddNoteContent(NoteContent,259,vipTmp.VIPEXP,subVipExp); 		//"[FFDD33]*累計儲值[00FF00]{0}[-]寶石可享受該級特權，目前尚需[00FF00]{1}[-]寶石[-]"
			else
				NoteContent = AddNoteContent(NoteContent,296,vipTmp.VIPEXP); 				//"[FFDD33]*累計儲值[00FF00]{0}[-]寶石可享受該級特權[-]"
		}
		if (vipTmp.RaidsSwitch == 0)
			NoteContent = AddNoteContent(NoteContent,504); 								//"多次掃蕩功能(VIP3開啟)"
		else if (vipTmp.RaidsSwitch == 1)
			NoteContent = AddNoteContent(NoteContent,505); 								//"多次掃蕩功能已開啟"
		NoteContent = AddNoteContent(NoteContent,506,vipTmp.BackpackLimitField); 			//"背包欄位上限{0}格。"
		NoteContent = AddNoteContent(NoteContent,507,vipTmp.DailytLimitResetPlayCount); 	//"每日可以免費重置關卡{0}次。"
		NoteContent = AddNoteContent(NoteContent,508,vipTmp.DailytLimitResetPVPList);		//"每日可以刷新競技場對手{0}次。"
		NoteContent = AddNoteContent(NoteContent,509,vipTmp.DailytLimitBuyMoneyTree);		//"每日可以使用搖錢樹{0}次。"
		NoteContent = AddNoteContent(NoteContent,512,vipTmp.DailytLimitBuyAP);				//"每日可以購買AP{0}次。"
		NoteContent = AddNoteContent(NoteContent,994,vipTmp.AddEqStrenghMax);				//" 額外裝備強化上限{0}次。"
		if (vipTmp.CheckStoreUnlock(ENUM_ItemMallType.ENUM_ItemMallType_VIP8))
			NoteContent = AddNoteContent(NoteContent,518);								//"開啟雜貨商店[00FF00]『逸品行』[-]"
		if (vipTmp.CheckStoreUnlock(ENUM_ItemMallType.ENUM_ItemMallType_VIP10))
			NoteContent = AddNoteContent(NoteContent,519);								//"開啟雜貨商店[00FF00]『黑市集』[-]"
		if (vipTmp.CheckPurTypeUnlock(ENUM_PurifyType.ENUM_PurifyType_Master))
			NoteContent = AddNoteContent(NoteContent,520);								//"開啟[00FF00]大師洗煉[-]功能"
		if (vipTmp.CheckPurTypeUnlock(ENUM_PurifyType.ENUM_PurifyType_GreatMaster))
			NoteContent = AddNoteContent(NoteContent,521);								//"開啟[00FF00]宗師洗煉[-]功能"
		if (vipTmp.PetSummonSwitch >= 1)
			NoteContent = AddNoteContent(NoteContent,522);								//"開啟[00FF00]主題召喚[-]功能"
		if (vipTmp.PetChipSwitch >= 1)
			NoteContent = AddNoteContent(NoteContent,476);								//"開放購買夥伴碎片功能"
		if (vipTmp.TowerChallengeCount > 0)
			NoteContent = AddNoteContent(NoteContent,514,vipTmp.TowerChallengeCount);		//"每日可以重置通天塔{0}次"

		lbVIPNoteContent.text 	= NoteContent;
		//標題字串
		spVIPNoteTitle.spriteName	= "VIP" + (vip).ToString(); 
		lbVIPNote.text 				= string.Format("[ffdd33]"+GameDataDB.GetString (510)+"[-]" , vip);  //"VIP權益功能說明"

		lbVIPNoteContent.autoResizeBoxCollider = false;
		lbVIPNoteContent.autoResizeBoxCollider = true;
	}
	//-----------------------------------------------------------------------------------------------------
	private string AddNoteContent(string str,int strNum, int value1,int value2 = 0)
	{
		if(str=="")
		{
			if (value2 > 0)
				str = string.Format(GameDataDB.GetString(strNum), value1, value2);
			else
				str = string.Format(GameDataDB.GetString(strNum), value1);
		}
		else if (value1 > 0)
			str = str + "\n" + string.Format(GameDataDB.GetString(strNum) , "[00FF00]"+value1+"[-]");

		return str;
	}
	//-----------------------------------------------------------------------------------------------------
	private string AddNoteContent(string str,int strNum)
	{
		str = str + "\n" + GameDataDB.GetString(strNum);
		return str;
	}
	//-------------------------------------------------------------------------------------------------
	//控制左右切頁按鈕的顯示
	private void ShowSwitchBtn()
	{
		if (m_NowVipShowing == GameDefine.VIP_LEVEL_MIN)
			btnLeftChoose.gameObject.SetActive(false);
		else if (m_NowVipShowing == GameDefine.VIP_LEVEL_MAX)
			btnRightChoose.gameObject.SetActive(false);
		else
		{
			btnLeftChoose.gameObject.SetActive(true);
			btnRightChoose.gameObject.SetActive(true);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//顯示VIP獎勵
	private void ShowVipReward(int vip)
	{
		GetVipReward(vip);
		//生成獎勵物品
		CreateRewardSlot();
		//指定獎勵資料給Slot
		AssignRewardData();
	}
	//-------------------------------------------------------------------------------------------------
	//取得VIP獎勵
	private void GetVipReward(int vip)
	{
		CleanRewardData();
		S_VIPLV_Tmp vipTmp = GameDataDB.VIPLVDB.GetData(vip+1); 
		if (vipTmp == null)
			return;
		//取得VIP獎勵資料
		S_Reward_Tmp rewardTmp = new S_Reward_Tmp();
		rewardTmp = GameDataDB.RewardDB.GetData(vipTmp.VIPRewardListID_1);
		m_VipRewardList.Add(rewardTmp);
		rewardTmp = GameDataDB.RewardDB.GetData(vipTmp.VIPRewardListID_2);
		m_VipRewardList.Add(rewardTmp);
		rewardTmp = GameDataDB.RewardDB.GetData(vipTmp.VIPRewardListID_3);
		m_VipRewardList.Add(rewardTmp);
	}
	//-------------------------------------------------------------------------------------------------
	//生成獎勵物品並顯示
	private void CreateRewardSlot()
	{
		if (m_RewardSlotList.Count > 0)
			return;

		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_VIPLvUp load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		if (m_VipRewardList.Count < 1)
		{
			UnityDebugger.Debugger.Log("無獎勵資料");
			return;
		}

		//Slot
		for(int i=0; i < m_VipRewardList.Count; ++i)
		{
			Slot_Item newgo= GameObject.Instantiate(go) as Slot_Item;
			newgo.transform.parent			= gdVIPRewards.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition 	= Vector3.zero;
			newgo.gameObject.SetActive(true);
			m_RewardSlotList.Add(newgo);
			newgo.name = string.Format("slotItem{0:00}",m_RewardSlotList.Count-1);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//指派獎勵資料至實體物品並整理
	private void AssignRewardData()
	{
		if (m_VipRewardList.Count != m_RewardSlotList.Count)
			return;
		for(int i=0; i < m_VipRewardList.Count; ++i)
		{
			//根據VIP獎勵資料開關Slot
			m_RewardSlotList[i].gameObject.SetActive(m_VipRewardList[i] != null);
			if (m_VipRewardList[i] == null)
				continue;

			m_RewardSlotList[i].SetSlotWithCount(m_VipRewardList[i].ItemGUID , m_VipRewardList[i].Count , true);	
		}
		//重新排序
		gdVIPRewards.enabled = true;
		gdVIPRewards.Reposition();
	}
	//-------------------------------------------------------------------------------------------------
	//清除獎勵物品及資料
	private void CleanRewardData()
	{
		if (m_VipRewardList.Count > 0)
			m_VipRewardList.Clear();
	}
}
