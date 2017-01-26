using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_GuideStep : NGUIChildGUI 
{
	public UIPanel			panelBase			= null;
	public GameObject		gAvatar				= null;
	public GameObject []	gAvatarPosition		= null;
	//
	public GameObject		gNoAvatar			= null;
	public GameObject []	gNoAvatarPosition	= null;
	//
	public UISprite			spFullBlackBG		= null;
	public GameObject		gFourBlackBG		= null;
	public UISprite []		spFourBlackBG		= null;
	//
	public UIButton			btnFullScreen		= null;
	public UISprite			spGuideTarget		= null;
	//-------------------------------------------執行用變數------------------------------------------------------
	[NonSerialized]private S_NewGuide_Tmp 	m_NewGuideTmp		= null;
	private bool 			m_IsNextStep		= false;
	//-------------------------------------------------------------------------------------------------
	private const string 	m_GuideFrameName	= "Sprite(GuideFrame)";
	private const string 	m_GuideNoteName		= "Sprite(GuideFrame)/Label(Explanation)";
	private const string 	m_GuideCenterNoteName	= "Label(Explanation)";
	//-------------------------------------------------------------------------------------------------
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_GuideStep";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_GuideStep() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		AddCallBack();
	}
	//-------------------------------------------------------------------------------------------------
	public void AddCallBack()
	{
		UICamera.onClick += NextStep;
	}
	//-------------------------------------------------------------------------------------------------
	public void RemoveCallBack()
	{
		UICamera.onClick -= NextStep;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetData(S_NewGuide_Tmp guideTmp)
	{
		m_NewGuideTmp = guideTmp;
		Show();
		StartCoroutine(SetUI());
	}
	//-------------------------------------------------------------------------------------------------
	private IEnumerator SetUI()
	{
		SwitchFullBlackBG(m_NewGuideTmp.iWaitSeconds > 0);
		if (m_NewGuideTmp.iWaitSeconds > 0)
		{
			SwitchFourBlackBG(false);
			gNoAvatar.SetActive(false);
			gAvatar.SetActive(false);
			SwitchBtnFullScreen(false);
		}

		yield return new WaitForSeconds(m_NewGuideTmp.iWaitSeconds); 

		SwitchFullBlackBG(false);

		if (m_NewGuideTmp.FramePositionType != ENUM_GuideFramePosition.ENUM_GuideFramePosition_Center)
		{
			//設定並檢查教學目標
			bool targetExsit = SetGuideTarget();
			if (!targetExsit)
			{
				CloseGuide();
				yield break;
			}
			//設定全螢幕按鈕
			SwitchBtnFullScreen(m_NewGuideTmp.ClickType == ENUM_GuideClickType.ENUM_GuideClickType_Any);
			//設定黑色底圖
			SwitchFourBlackBG(m_NewGuideTmp.ClickType == ENUM_GuideClickType.ENUM_GuideClickType_Target);
		}
		else
		{
			SwitchBtnFullScreen(true);
			SwitchFourBlackBG(false);
		}
		
		//設定說明板
		GameObject[] gArray = null;
		switch(m_NewGuideTmp.FrameType)
		{
		case ENUM_GuideFrameType.ENUM_GuideFrameType_Avatar:
			gArray = gAvatarPosition;
			gNoAvatar.SetActive(false);
			gAvatar.SetActive(true);
			break;
		case ENUM_GuideFrameType.ENUM_GuideFrameType_NoAvatar:
			gArray = gNoAvatarPosition;
			gAvatar.SetActive(false);
			gNoAvatar.SetActive(true);
			break;
		}
		for(int i=0; i<gArray.Length; ++i)
		{
			gArray[i].SetActive(false);
			ENUM_GuideFramePosition guidePos = (ENUM_GuideFramePosition)i;
			if (guidePos == m_NewGuideTmp.FramePositionType)
			{
				Transform trans = null;
				//說明板位置
				if (guidePos != ENUM_GuideFramePosition.ENUM_GuideFramePosition_Center)
				{
					//取得說明文物件
					trans = gArray[i].transform.FindChild(m_GuideNoteName);
					gArray[i].transform.localPosition = spGuideTarget.transform.localPosition;
				}
				else
					trans = gArray[i].transform.FindChild(m_GuideCenterNoteName);
				if (trans == null)
					continue;
				UILabel lbNote = trans.GetComponent<UILabel>();
				if (lbNote == null)
					continue;
				
				//說明板內容
				lbNote.text = GameDataDB.GetString(m_NewGuideTmp.iNote);
				ARPGApplication.instance.SetAndPlayOSSpeech(m_NewGuideTmp.iOSID);

				gArray[i].SetActive(true);
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	//設定教學目標
	private bool SetGuideTarget()
	{
		NGUIChildGUI ui = ARPGApplication.instance.guiManager.GetGUI(m_NewGuideTmp.strUIName) as NGUIChildGUI;
		if (ui == null)
			return false;
		//尋找教學目標
		GuideEvent[] guideEvent = ui.transform.GetComponentsInChildren<GuideEvent>();
		if (guideEvent == null)
			return false;
		//找出UI中GuideID相同的教學目標
		for(int i=0; i<guideEvent.Length; ++i)
		{
			if (guideEvent[i].GuideGUID == m_NewGuideTmp.GUID)
			{
				UISprite tempTarget = guideEvent[i].GetGuideTarget();
				if (tempTarget == null)
					break;
				tempTarget.transform.parent = this.transform;
				spGuideTarget.transform.localPosition = tempTarget.transform.localPosition;
				spGuideTarget.height = tempTarget.height;
				spGuideTarget.width = tempTarget.width;
				tempTarget.transform.parent = ui.transform;
				return true;
			}
		}
		return false;
	}
	//-------------------------------------------------------------------------------------------------
	//根據教學目標設定黑底圖Anchor
	private void SetBackGroundAnchor(Transform trans)
	{
		if (spFourBlackBG == null)
			return;
		for(int i=0; i<spFourBlackBG.Length; ++i)
		{
			spFourBlackBG[i].SetAnchor(trans);
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void SwitchBtnFullScreen(bool bSwitch)
	{
		btnFullScreen.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	private void SwitchFourBlackBG(bool bSwitch)
	{
		gFourBlackBG.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	private void SwitchFullBlackBG(bool bSwitch)
	{
		spFullBlackBG.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void NextStep(GameObject go)
	{
		//若點擊目標為四邊黑底則停止
		if (m_NewGuideTmp.ClickType == ENUM_GuideClickType.ENUM_GuideClickType_Target && CheckClickFourBlackBG(go))
			return;
		//若點擊目標不是透明的全螢幕按鈕則停止
		else if (m_NewGuideTmp.ClickType == ENUM_GuideClickType.ENUM_GuideClickType_Any && !CheckClickBtnFullScreen(go))
			return;
		//若點擊目標為全螢幕黑底BG則停止
		else if (CheckClickFullBlackBG(go))
			return;
		m_NewGuideTmp = GameDataDB.NewGuideDB.GetData(m_NewGuideTmp.iNextGUID);
		if (m_NewGuideTmp == null)
		{
			CloseGuide();
			return;
		}
		UnityDebugger.Debugger.Log("NewGuide Next Step, GuideGUID = "+m_NewGuideTmp.GUID);
		RemoveCallBack();
		//m_IsCheckClick = false;
		m_IsNextStep = true;
	}
	//-------------------------------------------------------------------------------------------------
	public void CheckNextStep()
	{
		if (m_IsNextStep && CheckUIReady())
		{
			SetData(m_NewGuideTmp);
			m_IsNextStep = false;
			AddCallBack();
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void CloseGuide()
	{
		RemoveCallBack();
		ARPGApplication.instance.m_TeachingSystem.ClearCurrentGuideType();
		ARPGApplication.instance.guiManager.DeleteGUI(typeof(UI_GuideStep).Name);
	}
	//-------------------------------------------------------------------------------------------------
	private bool CheckUIReady()
	{
		NGUIChildGUI ui = ARPGApplication.instance.guiManager.GetGUI(m_NewGuideTmp.strUIName) as NGUIChildGUI;
		if (ui == null)
			return false;
		if (!ui.IsVisible())
			return false;
		return true;
	}
	//-------------------------------------------------------------------------------------------------
	private bool CheckClickFourBlackBG(GameObject go)
	{
		for(int i=0; i<spFourBlackBG.Length; ++i)
		{
			if (go == spFourBlackBG[i].gameObject)
				return true;
		}
		return false;
	}
	//-------------------------------------------------------------------------------------------------
	private bool CheckClickBtnFullScreen(GameObject go)
	{
		if (go == btnFullScreen.gameObject)
			return true;
		return false;
	}
	//-------------------------------------------------------------------------------------------------
	private bool CheckClickFullBlackBG(GameObject go)
	{
		if (go == spFullBlackBG.gameObject)
			return true;
		return false;
	}
}
