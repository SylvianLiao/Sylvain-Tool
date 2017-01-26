using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_ActivityMenu : NGUIChildGUI  
{
	public UIWidget			slot				= null;
	public UIButton			btnActivity			= null;
	public UITexture		texActivityIcon		= null;
	public UILabel			lbActivityName		= null;

	public UILabel			LabelActivityAD		= null;
	public UILabel			LabelLastTime		= null;
	public UISprite			SpriteLastTime		= null;

	public UISprite			SpriteCanPlayTip	= null;

	public int				index				= -1;
	public S_Activity		ActivityData		= null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_EquipmentUPStar";

	//-------------------------------------------------------------------------------------------------
	private Slot_ActivityMenu() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	public void InitialSlot()
	{
		lbActivityName.text 	= "";
		LabelActivityAD.text 	= "";
		LabelLastTime.text 		= "";
	}

	//-------------------------------------------------------------------------------------------------
//	public void SetSlot(int activityID)
	public void SetSlot(S_Activity data)
	{
		int activityID = data.iActivityDBID;

		ActivityData = data;

		S_Activity_Tmp 	activityDBF = GameDataDB.ActivityDB.GetData(activityID);
		if(activityDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動表錯誤 活動編號 {0}", activityID));
			return ;
		}

		//檢查時間
		DateTime time = DateTime.UtcNow;
		TimeSpan ts = time - activityDBF.StartTime.ToUniversalTime();
		if( ts.TotalSeconds < 0)
		{
			slot.gameObject.SetActive(false);
			UnityDebugger.Debugger.LogError(string.Format("現在時間 {0} 開始時間 {1}", time, activityDBF.StartTime));
			return ;
		}

		ts = time - activityDBF.EndTime.ToUniversalTime();
		if(ts.TotalSeconds > 0)
		{
			slot.gameObject.SetActive(false);
			UnityDebugger.Debugger.LogError(string.Format("現在時間 {0} 結束時間 {1}", time, activityDBF.EndTime));
			return ;
		}

		S_ActivityInfo_Tmp infoDBF = GameDataDB.ActivityInfoDB.GetData(activityDBF.iGroup);
		if(infoDBF == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("讀取活動資料表錯誤 活動群組 {0}", activityDBF.iGroup));
			return ;
		}

		slot.gameObject.SetActive(true);

		slot.gameObject.GetComponentInChildren<UIButton>().userData = data;
		
		//設定背景圖
		Utility.ChangeTexture(texActivityIcon, infoDBF.iBackGroundType);

		if(activityDBF.iUnlockLevel > ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel())
		{
			//灰階變化
			texActivityIcon.color = new Color(0.0f, texActivityIcon.color.g, texActivityIcon.color.b);
		}


		//設定活動名稱
		lbActivityName.text = GameDataDB.GetString( infoDBF.CName);

		// 活動廣告
		LabelActivityAD.text = GameDataDB.GetString( activityDBF.ActivityAD);


		if(infoDBF.emActivityType == EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_WeekCycle)
		{
			string week = null;
			//週循環活動顯示星期幾開
			if(activityDBF.sOpenDay.GetFlag(ENUM_ActivityDayFlag.ENUM_ActivityDayFlag_Monday))
			{
				week += GameDataDB.GetString(2842);	//一
			}
			if(activityDBF.sOpenDay.GetFlag(ENUM_ActivityDayFlag.ENUM_ActivityDayFlag_Tuesday))
			{
				week += GameDataDB.GetString(2843);	//二
			}
			if(activityDBF.sOpenDay.GetFlag(ENUM_ActivityDayFlag.ENUM_ActivityDayFlag_Wednesday))
			{
				week += GameDataDB.GetString(2844);	//三
			}
			if(activityDBF.sOpenDay.GetFlag(ENUM_ActivityDayFlag.ENUM_ActivityDayFlag_Thursday))
			{
				week += GameDataDB.GetString(2845);	//四
			}
			if(activityDBF.sOpenDay.GetFlag(ENUM_ActivityDayFlag.ENUM_ActivityDayFlag_Friday))
			{
				week += GameDataDB.GetString(2846);	//五
			}
			if(activityDBF.sOpenDay.GetFlag(ENUM_ActivityDayFlag.ENUM_ActivityDayFlag_Saturday))
			{
				week += GameDataDB.GetString(2847);	//六
			}
			if(activityDBF.sOpenDay.GetFlag(ENUM_ActivityDayFlag.ENUM_ActivityDayFlag_Sunday))
			{
				week += GameDataDB.GetString(2848);	//日
			}

			if(!string.IsNullOrEmpty(week))
			{
				LabelLastTime.text = string.Format(GameDataDB.GetString(2849), week);	//星期{0}開放
				SpriteLastTime.gameObject.SetActive(true);
			}
			else
			{
				LabelLastTime.text = "";
				SpriteLastTime.gameObject.SetActive(false);
			}
		}
		else
		{
			//設定活動剩下時間
			if(activityDBF.ShowLastTime())
			{
				LabelLastTime.text = ARPGApplication.instance.m_ActivityMgrSystem.GetActivtyRemainTime(activityDBF.EndTime);
				SpriteLastTime.gameObject.SetActive(true);
			}
			else
			{
				LabelLastTime.text = "";
				SpriteLastTime.gameObject.SetActive(false);
			}
		}

		//設定活動還有次數能打提示
		if(ARPGApplication.instance.m_ActivityMgrSystem.CheckActivityValue(data))
		{
			SpriteCanPlayTip.gameObject.SetActive(true);
		}
		else
		{
			SpriteCanPlayTip.gameObject.SetActive(false);
		}
	}


}
