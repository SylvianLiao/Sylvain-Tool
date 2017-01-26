using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum Enum_msgType
{
	Enum_msgType_Original,
	Enum_msgType_Enhance,
}

public class UI_HUDmsg : NGUIChildGUI 
{

	public	UISprite 	spritetest 		= null;
	public 	UIGrid		gridMsg 		= null;
	public 	UILabel		labMsg			= null;

	private	int			objcount		= 0;

	public 	UIGrid		GridMsgForStrengThen = null;
	private	int			objcountTemp	= 0;

	//
	private Enum_msgType msgType;
	private Vector3		gridMsgLoc		= new Vector3();
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_HUDmsg";

	string[] msg = {"AAAAAA","bbbbb","ccccc","ddddd","eeeee"};
	//-------------------------------------------------------------------------------------------------
	private UI_HUDmsg() : base(GUI_SMARTOBJECT_NAME)
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
	void Start()
	{
	}
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		//先紀錄grid位置
		gridMsgLoc = gridMsg.transform.localPosition;
		msgType = Enum_msgType.Enum_msgType_Original;
		spritetest.gameObject.SetActive(false);
		labMsg.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetMsg(string str)
	{
		if(!this.gameObject.activeSelf)
		{
			this.gameObject.SetActive(true);
		}

		if(gridMsg.GetChildList().Count == 0)
		{
			objcount = 0;
		}
		else
		{
			if(msgType != Enum_msgType.Enum_msgType_Original)
			{
				foreach(Transform t in gridMsg.GetChildList())
				{
					GameObject.DestroyImmediate(t.gameObject);
				}
				msgType = Enum_msgType.Enum_msgType_Original;
				objcount = 0;
				gridMsg.transform.localPosition = gridMsgLoc;
			}
		}

		UILabel lab = Instantiate(labMsg) as UILabel;

		lab.transform.parent 		= gridMsg.transform;
		lab.transform.localScale 	= Vector3.one;
		lab.transform.localPosition = Vector3.zero;

		lab.name = "lab"+objcount;
		lab.text = str;

		lab.gameObject.SetActive(true);

		++objcount;

		gridMsg.Reposition();
		msgType = Enum_msgType.Enum_msgType_Original;
		gridMsg.transform.localPosition = gridMsgLoc;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetMsg(string str, Vector3 loc,Enum_msgType type)
	{
		if(!this.gameObject.activeSelf)
		{
			this.gameObject.SetActive(true);
		}
		
		if(gridMsg.GetChildList().Count == 0)
		{
			objcount = 0;
		}
		else
		{
			if(msgType != type)
			{
				foreach(Transform t in gridMsg.GetChildList())
				{
					GameObject.DestroyImmediate(t.gameObject);
				}
				msgType = type;
				objcount = 0;
				gridMsg.gameObject.transform.position = loc;
			}
		}
		
		UILabel lab = Instantiate(labMsg) as UILabel;
		
		lab.transform.parent 		= gridMsg.transform;
		lab.transform.localScale 	= Vector3.one;
		lab.transform.localPosition = Vector3.zero;
		
		lab.name = "lab"+objcount;
		lab.text = str;
		
		lab.gameObject.SetActive(true);
		
		++objcount;

		gridMsg.Reposition();

		msgType = type;
		gridMsg.gameObject.transform.position = loc;
	}
	//-------------------------------------------------------------------------------------------------
	public void SetMsgForStrengThen(string str)
	{
		if(!this.gameObject.activeSelf)
		{
			this.gameObject.SetActive(true);
		}
		
		if(gridMsg.GetChildList().Count == 0)
		{
			objcountTemp = 0;
		}
		
		UILabel lab = Instantiate(labMsg) as UILabel;
		
		lab.transform.parent 		= GridMsgForStrengThen.transform;
		lab.transform.localScale 	= Vector3.one;
//		lab.transform.localPosition = new Vector3 (-400,125,0);
		lab.transform.localPosition = Vector3.zero;
		
		lab.name = "lab"+objcount;
		lab.text = str;
		
		lab.gameObject.SetActive(true);
		
		++objcountTemp;
		
		GridMsgForStrengThen.Reposition();
	}
	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------------
}
