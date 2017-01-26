using System;
using System.Collections;
using UnityEngine;
using GameFramework;

public enum ENUM_Disconnect_State
{
	ENUM_Disconnect_Begin = 0,	//等待連線事件觸發
	ENUM_Disconnect_Waiting,	//連線中
}

class UI_Disconnect : NGUIChildGUI 
{
	public UIPanel					panelBase			= null;
	//
	public UIButton					m_btn				= null;
	public UILabel					m_btnContent		= null;
	//
	public UILabel					m_lbIsConnecting	= null;

	[Header("RoleLock")]
	public UIWidget					m_wgRoleLock		= null;
	public UILabel					m_lbLockTitle		= null;
	public UILabel					m_lbLockContents	= null;

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Disconnect";
	
	//-------------------------------------------------------------------------------------------------------------
	private UI_Disconnect() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-------------------------------------------------------------------------------------------------------------
	private void Start()
	{
		m_btnContent.text = GameDataDB.GetString(500);		//"重新連線"
		m_lbIsConnecting.text = GameDataDB.GetString(501);	//"連線中..."

		m_wgRoleLock.gameObject.SetActive(false);
		m_lbLockTitle.text = GameDataDB.GetString(525);		//此角色已被凍結
		m_lbLockContents.text = GameDataDB.GetString(526);	//請聯繫遊戲客服

	}

}
