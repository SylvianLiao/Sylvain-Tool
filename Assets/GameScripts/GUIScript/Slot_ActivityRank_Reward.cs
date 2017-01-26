using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_ActivityRank_Reward : NGUIChildGUI   
{
	public UILabel			LabelTitlle				= null;		//名次標題
	public UISprite			SpriteTitlleBG			= null;		//名次底板
	
	public Transform[]		itemSlotLocal			= new Transform[3]; // 道具格定位點

	//slot
	public string	slotName 		= "Slot_Item";
	public List<Slot_Item> itemSlotList = new List<Slot_Item>();	// 道具格slot	
	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_ActivityRank_Reward";

	//-------------------------------------------------------------------------------------------------
	private Slot_ActivityRank_Reward() : base(GUI_SMARTOBJECT_NAME)
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
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityRank_Reward load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}

		// 排行獎勵格
		for(int i=0; i<GameDefine.ACTIVITY_RANK_REWARD_ARRAY; ++i)
		{
			Slot_Item newgo= Instantiate(go) as Slot_Item;
			
			newgo.transform.parent			= itemSlotLocal[i].transform;
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
	public void SetSlot(S_RankReward data, bool isGuildRank)
	{
		bool noRange = false;
		if(data.iPointRankFrom == data.iPointRankTo)
		{
			LabelTitlle.text = string.Format("{0}{1}{2}", GameDataDB.GetString(2823), 
			                                 data.iPointRankFrom, GameDataDB.GetString(2825));//第 名
			noRange = true;
		}
		else
		{
			LabelTitlle.text = string.Format("{0}{1}{2}{3}{4}", GameDataDB.GetString(2823), 
			                                 data.iPointRankFrom, GameDataDB.GetString(2824), 
			                                 data.iPointRankTo, GameDataDB.GetString(2825));//第 名 - 第 名
		}

		for(int i=0;i<itemSlotList.Count; ++i)
		{
//			if(data.iRewardID[i] != -1)
//			{
				S_Reward_Tmp dbf = GameDataDB.RewardDB.GetData(data.iRewardID[i]);
				if(dbf == null)
				{
					itemSlotList[i].gameObject.SetActive(false);
//					UnityDebugger.Debugger.Log(string.Format("獎勵樣板表空的 獎勵編號{0}", data.iRewardID[i]));
					continue;
				}
				else
				{
					itemSlotList[i].gameObject.SetActive(true);
					int rewardCount = 0;
					if (isGuildRank)
					{
						S_GuildWars_Tmp gdTmp = GameDataDB.GuildWarsDB.GetData(GameDefine.GUILDWAR_DBF_GUID);
						if (dbf.GUID == GameDefine.GUILDWAR_TREASURE_REWARD_ID)
						{
							for(int m=0; m<gdTmp.GuildTreasures.Length; ++m)
							{
								GuildTreasure guildTreasure = gdTmp.GuildTreasures[m];
								if (guildTreasure.iPointRankFrom == data.iPointRankFrom)
								{
									//沒有名次區間的獎勵根據公式計算公會財庫
									if (noRange)
									{
										S_ActivityRankData rankData = ARPGApplication.instance.m_GuildSystem.GetGuildBossRankByIndex(data.iPointRankFrom-1);
										if (rankData != null)
										{
											rewardCount = Mathf.CeilToInt(rankData.iPoint * guildTreasure.fPointValue);
											if (rewardCount > guildTreasure.iPointLimit)
												rewardCount = guildTreasure.iPointLimit;
											else if (rewardCount < guildTreasure.iPointFloor)
												rewardCount = guildTreasure.iPointFloor;
										}
										else
											rewardCount = guildTreasure.iPointLimit;
									}
									//有名次區間的獎勵直接把上限當作公會財庫
									else
									{
										rewardCount = guildTreasure.iPointLimit;
									}
									break;
								}
							}
						}
					}
					else
						rewardCount = dbf.Count;
					itemSlotList[i].SetSlotWithCount(dbf.ItemGUID,rewardCount , false);
				}	
//			}
		}
	}
}
