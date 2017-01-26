using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildBossRank_Rank : NGUIChildGUI  
{
	public UILabel			lbName				= null; //
	
	public UILabel			lbLevelTitle		= null; //
	public UILabel			lbLevel				= null; //

	public UILabel			lbIDTitle			= null; //
	public UILabel			lbID				= null; //
	
	public UILabel			lbRankTitle			= null; //
	public UILabel			lbRank				= null; //
	public UISprite			spRank				= null; //

	public UILabel			lbScore				= null; //
	
	public UISprite			spSelfMark			= null; //標記自己用邊框
	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_GuildBossRank_Rank";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_GuildBossRank_Rank() : base(GUI_SMARTOBJECT_NAME)
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
		lbLevelTitle.text = GameDataDB.GetString(270);	//"等級"
		lbRankTitle.text = GameDataDB.GetString(1683);	//"名"
		lbIDTitle.text = GameDataDB.GetString(5223);	//"ID"
		spSelfMark.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetSlot(S_ActivityRankData rankData, int index)
	{
		GuildBaseData guildData = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		if (guildData == null)
			return;
		if (rankData == null)
			return;
		//
		lbID.text = rankData.iGuildID.ToString();
		lbName.text = rankData.strRoleName;
		lbLevel.text = rankData.iLv.ToString();
		spSelfMark.gameObject.SetActive(guildData.iGuildID == rankData.iGuildID);
		//排名
		lbRank.text = (index+1).ToString();
		
		if(index == 0)
		{
			Utility.ChangeAtlasSprite(spRank, 300);
		}
		else if(index == 1)
		{
			Utility.ChangeAtlasSprite(spRank, 301);
		}
		else if(index == 2)
		{
			Utility.ChangeAtlasSprite(spRank, 302);
		}
		else if(index <= 9 && index >= 3)
		{
			Utility.ChangeAtlasSprite(spRank, 303);
		}
		else if(index <=99 && index >= 10)
		{
			Utility.ChangeAtlasSprite(spRank, 304);
		}
		
		//積分設定
		if(ARPGApplication.instance.m_ActivityMgrSystem.GetSelectActivityType() == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_GuildWar)
		{
			lbScore.text = rankData.iPoint.ToString();
			lbScore.gameObject.SetActive(true);
		}
		else
		{
			lbScore.gameObject.SetActive(false);
		}
	}
}
