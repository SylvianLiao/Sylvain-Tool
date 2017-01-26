using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

public class Slot_ChatCover : MonoBehaviour {

	public UILabel 		lbName 		= null;
	public UILabel		lbCancle	= null;
	public UIButton		btnCancel	= null;

	public string 		Targetname	= null;
	public int			iTargetID	= 0;
	//-------------------------------------------------------------
	void Start ()
	{}
	//-------------------------------------------------------------
	void Update ()
	{}
	//-------------------------------------------------------------
	public void Clear()
	{
		Targetname = null;
		iTargetID = 0;

	}

	public void SetSlot(int ID,string Tname)
	{
		iTargetID = ID;
		Targetname = Tname;

		SetDisPlay();
	}

	void SetDisPlay()
	{
		lbName.text = Targetname;
		lbCancle.text = GameDataDB.GetString(224);
	}
}
