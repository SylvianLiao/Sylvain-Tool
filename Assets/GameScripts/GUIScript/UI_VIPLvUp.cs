using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_VIPLvUp : NGUIChildGUI  
{

	public UIPanel		panelBase					= null;		//自身Panel
	public UISprite		wgFullSreen					= null;		//全螢幕Collider
	public UILabel		lbVipLvUp		 			= null;		//VIP升級字串
	public UISprite		spNowVipLV					= null;		//升級前VIP圖示
	public UISprite		spFinalVipLV				= null;		//升級後VIP圖示
	public UILabel		lbVipReward		 			= null;		//VIP禮包字串
	public UIGrid		gdVipRewardList				= null;		//VIP禮包清單
	public UIButton		btnVipNote					= null;		//VIP說明按鈕
	public UILabel		lbVipNote					= null;		
	public UIButton		btnConfirm					= null;		//確定按鈕
	public UILabel		lbConfirm					= null;
	//-------------------------------------------------------------------------------------------------
	public List<GameObject>		m_VipRewardPos		= new List<GameObject>();	//VIP獎勵位置
	private List<S_Reward_Tmp>	m_VipRewardList		= new List<S_Reward_Tmp>();	//VIP獎勵資料
	private List<Slot_Item>		m_RewardSlotList	= new List<Slot_Item>();	//VIP獎勵物品
	//-------------------------------------------------------------------------------------------------
	private const string 	m_SlotName				= "Slot_Item";
	//-------------------------------------------------------------------------------------------------
	private DepositDiamondState m_DepositState		= null;
	[HideInInspector]
	public int					m_NowVipLV			= 0;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_VIPLvUp";
	
	//-------------------------------------------------------------------------------------------------
	private UI_VIPLvUp() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		m_DepositState = ARPGApplication.instance.GetGameStateByName(GameDefine.DEPOSITDIAMOND_STATE) as DepositDiamondState;
		base.Initialize();
		InitVipLvUp();
	}
	//-------------------------------------------------------------------------------------------------
	private void InitVipLvUp()
	{
		gdVipRewardList.enabled = false;
		lbVipLvUp.text = GameDataDB.GetString(1913);			//"VIP升級"
		lbVipNote.text = GameDataDB.GetString(1915);			//"檢視特權"
		lbConfirm.text = GameDataDB.GetString(1033);			//"確定"
	}
	//-------------------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();

		CreateRewardData();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		if (m_DepositState.m_VipLvUpCount > 0)
		{
			--m_DepositState.m_VipLvUpCount;
			CreateRewardData();
			return;
		}
		base.Hide();
	}
	//-------------------------------------------------------------------------------------------------
	//生成獎勵資料
	public void CreateRewardData()
	{
		//保護機制
		if (m_NowVipLV == GameDefine.VIP_LEVEL_MAX)
		{
			m_DepositState.m_VipLvUpCount = 0;
			Hide();
			return;
		}

		spNowVipLV.spriteName = "VIP" + (m_NowVipLV).ToString();
		//spFinalVipLV.spriteName = "VIP" + (m_DepositState.m_FinalVipLV).ToString();
		spFinalVipLV.spriteName = "VIP" + (++m_NowVipLV).ToString();
		lbVipReward.text = string.Format("[ffdd33]"+GameDataDB.GetString(1914)+"[-]" , m_NowVipLV.ToString());	//"VIP{0}禮包"
		GetVipReward(m_NowVipLV);
		CreateRewardSlot();
		AssignRewardData();
	}
	//-------------------------------------------------------------------------------------------------
	//取得VIP獎勵
	private void GetVipReward(int vip)
	{
		S_VIPLV_Tmp vipTmp = GameDataDB.VIPLVDB.GetData(vip+1); 
		if (vipTmp == null)
			return;
		CleanRewardData();

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
			newgo.transform.parent			= gdVipRewardList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition 	= m_VipRewardPos[i].transform.localPosition;
			newgo.gameObject.SetActive(true);
			m_RewardSlotList.Add(newgo);
			newgo.name = string.Format("slotItem{0:00}",m_RewardSlotList.Count-1);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//指派獎勵資料至實體物品
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
		gdVipRewardList.enabled = true;
		gdVipRewardList.Reposition();
	}
	//-------------------------------------------------------------------------------------------------
	//清除獎勵物品及資料
	private void CleanRewardData()
	{
		if (m_VipRewardList.Count > 0)
			m_VipRewardList.Clear();
	}
}
