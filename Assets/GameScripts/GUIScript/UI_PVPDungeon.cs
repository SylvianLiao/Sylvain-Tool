using System;
using UnityEngine;
using GameFramework;

class UI_PVPDungeon : NGUIChildGUI
{
	public UIButton		btnSkill = null;
	public UIButton		btnSkill01 = null;
	public UIButton		btnSkill02 = null;
	public UIButton		btnSkill03 = null;
	public UIButton		btnReturnToLobby = null;
	//
	private const string GUI_SMARTOBJECT_NAME = "UI_PVPDungeon";

	//-----------------------------------------------------------------------------------------------------
	private UI_PVPDungeon() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
}
