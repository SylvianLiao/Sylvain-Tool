using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_Team : NGUIChildGUI  
{
	//上
	public	UILabel		LabelTitle				= null;
	public	UIButton	ButtonClose				= null;

	//中
	public	UIPanel		PanelScrollView			= null;
	public	UIGrid		GridMemberList			= null;

	//下
	public	UISprite	SpriteBottomBG			= null;
	public	UILabel		LabelMemberCount		= null;
	public	UILabel		LabelNote				= null;
	public	UILabel		LabelTotalCostTitle		= null;
	public	UILabel		LabelTotalCost			= null;
	public	UIButton	ButtonBattle			= null;
	public	UILabel		LabelBattle				= null;
	public	UILabel		LabelNote2				= null;
	public	UILabel		LabelNote3				= null;

	public	UIWidget	MaskContainer			= null;

	string slotName								= "Slot_Team";
	public List<Slot_Team>		slotList		= new List<Slot_Team>();

	S_Dungeon_Tmp	tempDBF	= null;

	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UISprite		spGuideJoinBattle				= null; //導引參戰按鈕
	public UILabel		lbGuideJoinBattle				= null;
	public UISprite		spGuideNeedMoney				= null; //導引需求雇用金
	public UILabel		lbGuideNeedMoney				= null;
	public UILabel		lbGuideFinish					= null;	//導引教學結束

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_Team";
	
	//-------------------------------------------------------------------------------------------------
	private UI_Team() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		CreatSlot();

		LabelTitle.text 	  	 = GameDataDB.GetString(5401);	//組隊清單
		LabelNote.text			 = GameDataDB.GetString(5403);	//可重選出戰成員
		LabelTotalCostTitle.text = GameDataDB.GetString(5404);	//總費用
		LabelBattle.text 		 = GameDataDB.GetString(5405);	//戰鬥
		LabelTotalCost.text 	 = "0";
		LabelNote2.text 		 = GameDataDB.GetString(5410);	//※與好友組隊將額外贈與好友部份傭金
		LabelNote3.text 		 = GameDataDB.GetString(5420);	//※好友傭金費會儲入該好友的傭金財庫

		S_Dungeon_Tmp tempDBF= ARPGApplication.instance.m_FriendSystem.GetSelectDungeon();
		if(tempDBF == null)
		{
			LabelMemberCount.text 	 = string.Format(GameDataDB.GetString(5402),0,1);	//組隊人數限制 {0}/{1}
		}
		else
		{
			LabelMemberCount.text 	 = string.Format(GameDataDB.GetString(5402),0,tempDBF.UserMax-1);	//組隊人數限制 {0}/{1}
		}
	}

	//-------------------------------------------------------------------------------------------------
	// 建立SLOT
	void CreatSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_Team"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_Team go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_Team>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Team load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}
		
		// GuildList
		for(int i=0; i< 10 ; ++i) 
		{
			Slot_Team newgo	= Instantiate(go) as Slot_Team;
			
			newgo.transform.parent			= GridMemberList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.SetActive(true);

			newgo.ButtonSelect.userData = i;
			newgo.name = string.Format("slot{0:00}",i);
			
			newgo.InitialSlot();
			slotList.Add(newgo);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetTotalCost(int index)
	{
		//1327 [FF0000]  紅色
		int cost = ARPGApplication.instance.m_FriendSystem.GetTeammateCost(index);
		if(cost > ARPGApplication.instance.m_RoleSystem.iBaseBodyMoney)
		{
			LabelTotalCost.text = string.Format("[FF0000]{0}[-]", cost);
		}
		else
		{
			LabelTotalCost.text = cost.ToString();
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void ClearTotalCost()
	{
		LabelTotalCost.text = "0";
	}

	//-------------------------------------------------------------------------------------------------
	public void SetMemberCount()
	{
		int count = ARPGApplication.instance.m_FriendSystem.GetSelectTeammateCount();

		S_Dungeon_Tmp tempDBF= ARPGApplication.instance.m_FriendSystem.GetSelectDungeon();
		if(tempDBF == null)
		{
			LabelMemberCount.text 	 = string.Format(GameDataDB.GetString(5402), count, 1);	//組隊人數限制 {0}/{1}
		}
		else
		{

			string str = "";
			if(count >= tempDBF.UserMax-1)
			{
				str = GameDataDB.GetString(5414);	//※組隊人數限制 [00EC00]{0}[-]/[00EC00]{1}[-]  (綠色)
			}
			else
			{
				str = GameDataDB.GetString(5402);	//※組隊人數限制 {0}/{1}
			}
			LabelMemberCount.text 	 = string.Format(str, count, tempDBF.UserMax-1);	
		}
	}
}
