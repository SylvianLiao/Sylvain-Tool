using System;
using UnityEngine;
using GameFramework;

public enum EnumGetAPState
{
    EnumGetAPState_CanGet,
    EnumGetAPState_BeGet,
    EnumGetAPState_CantGet,
}

public class UI_FreeAP : NGUIChildGUI
{
    public UIButton btnFullScreen;
    public UIButton btnQuit;
    public UIButton btnGetAP;
    public UISprite spriteRole;
    public UILabel labelTip;
    public UILabel labelTitle;
    public UILabel labelNote;
    public UILabel labelGetAP;

    public EnumGetAPState emGetAPState = EnumGetAPState.EnumGetAPState_CanGet;
    public ENUM_GetAP_Time_Type timeType = ENUM_GetAP_Time_Type.ENUM_GetAP_Time_None;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_FreeAP";

	//-----------------------------------------------------------------------------------------------------
	private UI_FreeAP() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
    public override void Initialize()
    {
        base.Initialize();
        labelTitle.text = GameDataDB.GetString(2562); //體力領取
        labelTip.text = GameDataDB.GetString(310); //請於每日[00FF00]12-14[-]點、[00FF00]18-20[-]點、[00FF00]21-23[-]點領取。
        labelGetAP.text = GameDataDB.GetString(15051); //請點擊開始更新
        UIEventListener.Get(btnQuit.gameObject			).onClick	+= OnClose;
    }
	//-----------------------------------------------------------------------------------------------------
	void Update()
	{
		//Android手機回上頁功能
		ARPGApplication.instance.AndroidPhonePreviousEvent(OnClose,ARPGApplication.instance.GetCurrentGameState().name);
	}
	//-----------------------------------------------------------------------------------------------------
    public override void Show()
    {
        CheckTimesAndState();
        SetInfo();
        base.Show();

    }
    public void CheckTimesAndState()
    {
        //檢查時間
        DateTime GetFreeAPBeginTime;
        TimeSpan ts;
        DateTime nowTime = DateTime.UtcNow;
        GetFreeAPBeginTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, GameDefine.SEND_AP_TIME1, 0, 0);
        ts = nowTime - GetFreeAPBeginTime;
        if (ts.TotalSeconds >= 0 && ts.TotalSeconds < GameDefine.GETTIME)
        {
            timeType = ENUM_GetAP_Time_Type.ENUM_GetAP_Time_1;
            if (ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.sRoleProgressFlag.GetFlag(ENUM_RoleProgressFlag.ENUM_RoleProgressFlag_FreeAP_1))
            {
                emGetAPState = EnumGetAPState.EnumGetAPState_BeGet;
                return;
            }
            else
            {
                emGetAPState = EnumGetAPState.EnumGetAPState_CanGet;
                return;
            }
        }
        GetFreeAPBeginTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, GameDefine.SEND_AP_TIME2, 0, 0);
        ts = nowTime - GetFreeAPBeginTime;
        if (ts.TotalSeconds >= 0 && ts.TotalSeconds < GameDefine.GETTIME)
        {
            timeType = ENUM_GetAP_Time_Type.ENUM_GetAP_Time_2;
            if (ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.sRoleProgressFlag.GetFlag(ENUM_RoleProgressFlag.ENUM_RoleProgressFlag_FreeAP_2))
            {
                emGetAPState = EnumGetAPState.EnumGetAPState_BeGet;
                return;
            }
            else
            {
                emGetAPState = EnumGetAPState.EnumGetAPState_CanGet;
                return;
            }

        }
        GetFreeAPBeginTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, GameDefine.SEND_AP_TIME3, 0, 0);
        ts = nowTime - GetFreeAPBeginTime;
        if (ts.TotalSeconds >= 0 && ts.TotalSeconds < GameDefine.GETTIME)
        {
            timeType = ENUM_GetAP_Time_Type.ENUM_GetAP_Time_3;
            if (ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.sRoleProgressFlag.GetFlag(ENUM_RoleProgressFlag.ENUM_RoleProgressFlag_FreeAP_3))
            {
                emGetAPState = EnumGetAPState.EnumGetAPState_BeGet;
                return;
            }
            else
            {
                emGetAPState = EnumGetAPState.EnumGetAPState_CanGet;
                return;
            }

            //JsonSlot_RoleSync.Send_CtoM_GetAP(ENUM_GetAP_Time_Type.ENUM_GetAP_Time_3);
        }
        timeType = ENUM_GetAP_Time_Type.ENUM_GetAP_Time_None;
        emGetAPState = EnumGetAPState.EnumGetAPState_CantGet;

    }
    public void SetInfo()
    {
        switch(emGetAPState)
        {
            case EnumGetAPState.EnumGetAPState_CanGet:
            labelNote.text = GameDataDB.GetString(2565);
            labelGetAP.text = GameDataDB.GetString(2563);
            break;
            case EnumGetAPState.EnumGetAPState_CantGet:
            labelNote.text = GameDataDB.GetString(2571);
            labelGetAP.text = GameDataDB.GetString(2563);
            break;
            case EnumGetAPState.EnumGetAPState_BeGet:
            labelNote.text = GameDataDB.GetString(2569);
            labelGetAP.text = GameDataDB.GetString(2567);
            break;
        }
    }
    void OnClose(GameObject go)
    {
        LobbyState lobby = (LobbyState)ARPGApplication.instance.GetGameStateByName(GameDefine.LOBBY_STATE);
        lobby.SetAutoMission(ENUM_AutoMission.AM_Resume);
        this.gameObject.SetActive(false);
    }
}

