using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_ActivityPointBonus : NGUIChildGUI  
{
	public UILabel			LabelPoint		= null;	// 分數

	//slot
	public string			slotName 		= "Slot_Item";
	public List<Transform>  itemLocal		= new List<Transform>();	// Slot定位點
	public List<Slot_Item>  itemSlotList 	= new List<Slot_Item>();	// 道具格slot

	public Color			notyetTop		= new Color();
	public Color			notyetBottom	= new Color();
	public Color			notyeteffect	= new Color();

	public Color			getTop			= new Color();
	public Color			getBottom		= new Color();
	public Color			geteffect		= new Color();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_ActivityPointBonus";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_ActivityPointBonus() : base(GUI_SMARTOBJECT_NAME)
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
		CreatSlot();
	}

	//-------------------------------------------------------------------------------------------------
	void CreatSlot()
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

		// GuildList
		for(int i=0; i<GameDefine.ACTIVITY_RANK_REWARD_ARRAY; ++i) 
		{
			Slot_Item newgo= Instantiate(go) as Slot_Item;
			
			newgo.transform.parent			= itemLocal[i].transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;//itemSlotLocal[i].localPosition;
			
			newgo.name = string.Format("slotItem{0:00}",i);
			
			newgo.InitialSlot();
			newgo.gameObject.SetActive(false);
			itemSlotList.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetBonusSlot(S_PointReward data, int myPoint)
	{
		//已領取
		if(myPoint >= data.iPoint)
		{
			LabelPoint.text = GameDataDB.GetString(2828);	//已達成
			LabelPoint.gradientTop = getTop;
			LabelPoint.gradientBottom = getBottom;
			LabelPoint.effectColor = geteffect;
		}
		else
		{
			LabelPoint.text = string.Format("{0}{1}", data.iPoint, GameDataDB.GetString(2829));	//分領取

			LabelPoint.gradientTop = notyetTop;
			LabelPoint.gradientBottom = notyetBottom;
			LabelPoint.effectColor = notyeteffect;
		}

		//獎勵格
		for(int i=0; i<GameDefine.ACTIVITY_RANK_REWARD_ARRAY; ++i)
		{
			S_Reward_Tmp dbf = GameDataDB.RewardDB.GetData(data.iRewardID[i]);
			if(dbf == null)
			{
				itemSlotList[i].gameObject.SetActive(false);
//				UnityDebugger.Debugger.Log(string.Format("獎勵樣板表空的 獎勵編號{0}", data.iRewardID[i]));
				continue;
			}
			else
			{
				itemSlotList[i].gameObject.SetActive(true);
				itemSlotList[i].SetSlotWithCount(dbf.ItemGUID, dbf.Count, false);
			}
		}
	}
}
