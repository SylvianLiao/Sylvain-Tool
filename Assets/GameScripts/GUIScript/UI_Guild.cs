using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_Guild : NGUIChildGUI
{
	// BeginPage
	public UIWidget			BeginPage				= null;

	public UIButton			ButtonSign 				= null;
	public UIButton			ButtonBuild 			= null;
	public UIButton			ButtonMember 			= null;
	public UIButton			ButtonRank 				= null;

	public UILabel			LabelGuildName			= null;
	public UILabel			LabelGuildIDTitle		= null;
	public UILabel			LabelGuildID			= null;
	public UILabel			LabelGuildLVTitle		= null;
	public UILabel			LabelGuildLV			= null;
	public UILabel			LabelGuildEXPTitle		= null;
	public UILabel			LabelGuildEXP			= null;
	public UILabel			LabelGuildLeaderTitle	= null;
	public UILabel			LabelGuildLeader		= null;
	public UILabel			LabelGuildMemberTitle	= null;
	public UILabel			LabelGuildMember		= null;
	public UILabel			LabelGuildNoteTitle		= null;
	public UILabel			LabelGuildNote			= null;

	public UIButton			ButtonRaidBG			= null;
	public UIButton			ButtonGuildWarBG		= null;

	// MemberListPage
	public UIWidget			MemberListPage			= null;
	public UIButton			ButtonLeaveGuild 		= null;
	public UIButton			ButtonLeaveMemberListPage 	= null;
	public UILabel			LabelLeaveMemberListPage	= null;


	//
	public List<UIWidget>	guildPagesBG			= new List<UIWidget>();

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_Guild";
	
	//-------------------------------------------------------------------------------------------------
	private UI_Guild() : base(GUI_SMARTOBJECT_NAME)
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
//		object[] gos = UI_Guild .FindObjectsOfType(typeof( UIWidget));
//		for(int i=0; i<gos.Length; ++i)
//		{
//			if(gos[i] != null)
//			{
//				UIWidget widget = gos[i] as	UIWidget;
//				guildPagesBG.Add(widget);
//			}
//		}

//		UIWidget[] widgets = this.GetComponentsInChildren<UIWidget>();
/*		UIWidget[] widgets = this.GetComponents<UIWidget>();
		for(int i=0; i<widgets.Length; ++i)
		{
			if(widgets[i] != null)
			{
//				UIWidget widget = gos[i] as	UIWidget;
				guildPagesBG.Add(widgets[i]);
			}
		}
*/
	}

	//-------------------------------------------------------------------------------------------------
	public void SetGuildInfo(JSONPG_MtoC_GuildBaseData pg, string name, int count)
	{
//		public int 		GuildMoney;			//公會金
//		public int 		GuildSettings;		//公會設定

		//公會名稱
		LabelGuildName.text 	= pg.GuildName;		
		//公會編號
		LabelGuildID.text 		= pg.iGuildID.ToString();
		//公會等級
		LabelGuildLV.text 		= pg.BuildingLevel[(int)ENUM_GUILD_BUILD.EMUM_GUILD_BUILDE_Guild].ToString();	
		//公會積分(相當於經驗值)
		LabelGuildEXP.text 		= pg.GuildExp.ToString();		
		//會長名稱
		LabelGuildLeader.text 	= name;		
		//人數
		LabelGuildMember.text 	= count.ToString();
		//公告(暫無)
//		LabelGuildNote.text		= ;		//note
	}

	//-------------------------------------------------------------------------------------------------
	public void ShowPage(UIWidget page)
	{
		string str = page.gameObject.name;
		for(int i=0; i<guildPagesBG.Count; ++i)
		{
			if(guildPagesBG[i].gameObject.name == str)
			{
				guildPagesBG[i].gameObject.SetActive(true);
			}
			else
			{
				guildPagesBG[i].gameObject.SetActive(false);
			}
		}
	}

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------
}
