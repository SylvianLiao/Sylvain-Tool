using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_MessageBox : NGUIChildGUI 
{
	public UILabel		labMsg			= null;
	public UIWidget		MsgGenerateSpace= null;
	public UIGrid		GridMsg			= null;
	
	// smartObjectName
	private int			iCount=0;
	private float		fResetTime		= 0;
	private const string 	GUI_SMARTOBJECT_NAME = "UI_MessageBox";
	//-------------------------------------------------------------------------------------------------
	private UI_MessageBox() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		Hide();	
		InitialUI();
	}
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		labMsg.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	void Update()
	{
		if(fResetTime>0)
		{
			fResetTime = fResetTime-Time.deltaTime;
			if(fResetTime <= 0)
				iCount=0;
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetMsgBox(string str)
	{
		//打點紀錄
		for(int i=0;i<ARPGApplication.instance.LackStringIDs.Count;++i)
		{
			if(str == GameDataDB.GetString(ARPGApplication.instance.LackStringIDs[i]))
			{
				ARPGApplication.instance.DadianSystemRecordPoint(28);  //首次提示鑽石/金幣不足
				break;
			}
		}

		Show();
		/*UILabel Msg = MsgGenerateSpace.gameObject.GetComponentInChildren<UILabel>();
		if(Msg != null)
		{


			//DestroyImmediate(Msg.gameObject);
			Msg = null;
		}*/
		++iCount;
		fResetTime =3.0f;
		UILabel Msg = Instantiate(labMsg) as UILabel;
		//
		Msg.transform.parent 		= MsgGenerateSpace.transform;
		Msg.transform.localScale 	= Vector3.one;
		Msg.transform.localPosition = Vector3.zero;
		Msg.name = "Msg"+(iCount<10?"0"+iCount.ToString():iCount.ToString());
		Msg.text = str;
		Msg.gameObject.SetActive(true);
		GridMsg.repositionNow = true;
		//Destroy(Msg.gameObject,2.0f);
	}
	//-------------------------------------------------------------------------------------------------	
	public  override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------
	public  override void Hide()
	{
		base.Hide();
	}
	//-------------------------------------------------------------------------------------------------
}
