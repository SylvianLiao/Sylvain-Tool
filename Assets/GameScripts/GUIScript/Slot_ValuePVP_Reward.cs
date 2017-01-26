using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_ValuePVP_Reward : NGUIChildGUI {

	public UILabel		LabelTitle 		= null;
	public Transform[]	btnTransform	= new Transform[3];
	//slot
	public string	slotName 		= "Slot_Item";
	public List<Slot_Item> itemSlotList = new List<Slot_Item>();	// 道具格slot	

	private const string GUI_SMARTOBJECT_NAME = "Slot_ValuePVPRankReward";
	//-------------------------------------------------------------------------------------------------
	private Slot_ValuePVP_Reward():base(GUI_SMARTOBJECT_NAME)
	{}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize ()
	{
		base.Initialize ();

	}
	//-------------------------------------------------------------------------------------------------
	public void Clear()
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
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityRank_Reward load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		// 排行獎勵格
		for(int i=0; i<btnTransform.Length; ++i)
		{
			Slot_Item newgo= Instantiate(go) as Slot_Item;
			
			newgo.transform.parent			= btnTransform[i].transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;//itemSlotLocal[i].localPosition;
			newgo.GetComponent<UIWidget>().depth = 23;
			
			newgo.name = string.Format("slotItem{0:00}",i);
			
			newgo.InitialSlot();
			newgo.gameObject.SetActive(false);
			itemSlotList.Add(newgo);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlotValue(ValuePVPRewardValue value,ValuePVP_RewardIndex page)
	{
		//開關Slot
		if(value.rRankFrom == -1||value.rRankTo == -1||value.rPoint == -1)
		{
			this.gameObject.SetActive(false);
			return;
		}

		this.gameObject.SetActive(true);


		//設定條件顯示
		switch(page)
		{
		case ValuePVP_RewardIndex.RewardRank:
			if(value.rRankFrom == value.rRankTo)
			{
				LabelTitle.text = string.Format(GameDataDB.GetString(1578),GameDataDB.GetString(1326),value.rRankFrom,GameDataDB.GetString(1329));							//"第{0}{1}{2}名"
			}
			else
			{
				LabelTitle.text = string.Format(GameDataDB.GetString(1579),GameDataDB.GetString(1326),value.rRankFrom,value.rRankTo,GameDataDB.GetString(1329));			//"{0}{1}~{2}{3}名"
			}
			break;
		case ValuePVP_RewardIndex.RewardPoint:
			ValuePVPState state = (ValuePVPState)ARPGApplication.instance.GetGameStateByName(GameDefine.VALUEPVP_STATE);
			int p = state.uiValuePVP.GetNowRankPoint();
			if(p >= value.rPoint)
			{
				LabelTitle.text = string.Format("{0}{1}{2}",GameDataDB.GetString(1326),GameDataDB.GetString(1581),GameDataDB.GetString(1329));							    //{1}="已達成"	
			}
			else
			{
				LabelTitle.text = string.Format("{0}{1}{2}{3}",GameDataDB.GetString(1326),value.rPoint,GameDataDB.GetString(1329),GameDataDB.GetString(1580));				//{3}="積分"
			}
			break;
		}


		for(int i=0;i<itemSlotList.Count; ++i)
		{
			if(value.ItemID[i] != -1)
			{
				S_Reward_Tmp dbf = GameDataDB.RewardDB.GetData(value.ItemID[i]);
				if(dbf == null)
				{
					itemSlotList[i].gameObject.SetActive(false);
					UnityDebugger.Debugger.Log(string.Format("獎勵樣板表空的 獎勵編號{0}", value.ItemID[i]));
					continue;
				}
				else
				{
					itemSlotList[i].gameObject.SetActive(true);
					itemSlotList[i].SetSlotWithCount(dbf.ItemGUID, dbf.Count, false);
					itemSlotList[i].GetComponent<UIButton>().userData = dbf;
				}	
			}
		}

		AddCallBack();

	}

	public void AddCallBack()
	{
		UIEventListener.Get(itemSlotList[0].gameObject).onClick += CallItemTip;
		UIEventListener.Get(itemSlotList[1].gameObject).onClick += CallItemTip;
		UIEventListener.Get(itemSlotList[2].gameObject).onClick += CallItemTip;
	}

	public void CallItemTip(GameObject go)
	{
		UnityDebugger.Debugger.Log("***SlotRankReward OnItemSlotClick***");
		
		S_Reward_Tmp dbf = (S_Reward_Tmp)go.GetComponent<UIButton>().userData;
		
		if(dbf.ItemGUID > 0)
		{
			//顯示道具tip
			ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(dbf.ItemGUID,dbf.Count);
		}
		else
		{
			UnityDebugger.Debugger.LogError(string.Format("道具格顯示TIP錯誤 道具編號{0}", dbf.ItemGUID ));
		}


	}
}
