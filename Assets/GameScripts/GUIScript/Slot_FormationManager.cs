using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

public class Slot_FormationManager : MonoBehaviour {

	public UIButton btnFormation;
	public UILabel	lbCondition;
	public UILabel	lbName;
	public UISprite	spStartup;
	
	public int RealIndex;
	//-------------------------------------------------------------
	void Start ()
	{}
	//-------------------------------------------------------------
	void Update () 
	{}
	//-------------------------------------------------------------
	public void Init()
	{
		lbCondition.text = "";
		lbName.text = "";
		spStartup.gameObject.SetActive(true);
		spStartup.color = Color.gray;
	}
	public void SetStarUp()
	{
		//spStartup.gameObject.SetActive(true);
		spStartup.color = Color.white;
	}

	//-------------------------------------------------------------
	public void SetSlot()
	{
		S_Formation_Tmp forTmp = GameDataDB.FormationDB.GetData(RealIndex+1);;
		if(forTmp == null)
		{
			UnityDebugger.Debugger.LogError("戰陣表出問題啦!!!");
		}

		if(ARPGApplication.instance.m_RoleSystem.StartUpFormationID == (ENUM_FormationType)(RealIndex + 1))
		{
			SetStarUp();
			GetComponent<UIToggle>().value = true;
		}

		//spStartup.gameObject.SetActive(ARPGApplication.instance.m_RoleSystem.StartUpFormationID == (ENUM_FormationType)(RealIndex + 1));
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckQuestUnlockOrNot(forTmp.iPreQuestID))
		//if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLastQuestID() > forTmp.iPreQuestID)
		{
			btnFormation.isEnabled = true;
			switch(RealIndex)
			{
			case 0:
				lbName.text = GameDataDB.GetString(266);
				break;
			case 1:
				lbName.text = GameDataDB.GetString(267);
				break;
			case 2:
				lbName.text = GameDataDB.GetString(268);
				break;
			case 3:
				lbName.text = GameDataDB.GetString(269);
				break;
			}
			return;
		}
		else
		{
			btnFormation.isEnabled = false;
			switch(RealIndex)
			{
			case 0:
				lbCondition.text = GameDataDB.GetString(273);
				lbName.text = GameDataDB.GetString(266);
				break;
			case 1:
				lbCondition.text = GameDataDB.GetString(274);
				lbName.text = GameDataDB.GetString(267);
				break;
			case 2:
				lbCondition.text = GameDataDB.GetString(275);
				lbName.text = GameDataDB.GetString(268);
				break;
			case 3:
				lbCondition.text = GameDataDB.GetString(276);
				lbName.text = GameDataDB.GetString(269);
				break;
			}
		}

	}
	//-------------------------------------------------------------
}
