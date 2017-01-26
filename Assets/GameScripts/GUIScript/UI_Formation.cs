using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UI_Formation : NGUIChildGUI 
{
	public UISprite		spTitleName		= null;

	public UIPanel 		basePanel		= null;
	public UIButton		btnUpgrade		= null;
	public UIButton		btnStartup		= null;
	public UIButton		btnExplanation	= null;
	public UIButton		btnExplanationQuit	= null;

	public UIButton		btnEarth		= null;
	public UIButton		btnFire			= null;
	public UIButton		btnWater		= null;
	public UIButton		btnWind			= null;

	public UILabel		lbLevelInfo		= null;
	public UILabel		lbUpgrade		= null;
	public UILabel		lbStartup		= null;
	public UILabel		lbLvUpMoney		= null;
	public UILabel		lbExplanation	= null;

	public UISprite		spLvUpMoney		= null;
	public UISprite		spExplanation	= null;
	public UISprite		spP_BG			= null;

	public UIScrollView	scrForManager	= null;			//右側戰陣選單ScrollView
	public UIWrapContentEX wrapFormation = null;		//右側戰陣選單物鍵

	public UIWidget		Formation_1		= null;
	//public UIWidget		Formation_2		= null;
	//public UIWidget		Formation_3		= null;
	//public UIWidget		Formation_4		= null;

	public UIWidget		Line_1			= null;
	public UIWidget		Line_2			= null;
	public UIWidget		Line_3			= null;
	public UIWidget		Line_4			= null;

	public Slot_FormationNode[] FormationNodes_1;		//第一戰陣群
	//public Vector3[] 			FormationNodesPos;		//陣眼座標
	//public Slot_FormationNode[] FormationNodes_2;		//第二戰陣群
	//public Slot_FormationNode[] FormationNodes_3;		//第三戰陣群
	//public Slot_FormationNode[] FormationNodes_4;		//第四戰陣群

	public Slot_FormationManager[] FormationMans;		//右側戰陣列表
	[HideInInspector]
	public int[] arrayTempPetID;						//暫存陣眼寵物ID
	[HideInInspector]
	public Slot_FormationNode[] nowForNodes;			//現在選用的陣眼

	public ENUM_FormationType emNowForType;				//現在選用的戰陣
	[HideInInspector]
	public ENUM_FormationType nowStartUpFormaitonID;	//啟動的戰陣ID

	public Slot_EquipmentUPStar_Item UpgradeItem;		//升級物品
	//-------------
	[Header("ReadyPet")]
	//readypet閒置寵物列表
	public UIPanel			panelReadyPet	= null;
	public UIScrollView 	scReadtPet		= null;
	public UIWrapContentEX	wpReadyPet		= null;
	public UIButton			btnReadyPetQuit	= null;
	[HideInInspector]
	public List<S_PetData> 	m_unlockPetList = new List<S_PetData>(); 
	[HideInInspector]
	public int				Pet1DBFID				= 0;		//紀錄出戰夥伴1DBFID
	[HideInInspector]
	public int 				Pet2DBFID				= 0;		//紀錄出戰夥伴2DBFID
	//-------------------------------------新手教學用-------------------------------------
	[Header("panelGuide")]
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UILabel		lbGuideIntroduce				= null;
	public UISprite		spGuideChooseNode				= null; //導引點擊第一個陣眼
	public UILabel		lbGuideChooseNode				= null;
	public UISprite		spGuideChoosePet				= null; //導引選擇陣眼寵物
	public UILabel		lbGuideChoosePet				= null;
	public UISprite		spGuideUpgradeFormation			= null; //導引升級戰陣
	public UILabel		lbGuideUpgradeFormation			= null;
	public UILabel		lbGuideFinish					= null;	//導引教學結束
	//-------------------------------------------------------------------------------------------------
	private StringBuilder strBuilder = new StringBuilder();

	private const string 	GUI_SMARTOBJECT_NAME = "UI_Formation";
	//-------------------------------------------------------------------------------------------------
	private UI_Formation() : base(GUI_SMARTOBJECT_NAME)
	{}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		arrayTempPetID = new int[GameDefine.DEF_FormationNode_MAX];
		InitLabel();
		//initialize();
	}
	//-------------------------------------------------------------------------------------------------
	public void InitLabel()
	{
		lbLevelInfo.text = string.Format(GameDataDB.GetString(260),"0","0","\n","0");
		lbLvUpMoney.text = "0/0";
		lbStartup.text = GameDataDB.GetString(262);
		lbUpgrade.text = GameDataDB.GetString(261);
		lbExplanation.text = GameDataDB.GetString(263);
	}
	//-------------------------------------------------------------------------------------------------
	public void initialize()
	{
		panelReadyPet.gameObject.SetActive(false);
		spExplanation.gameObject.SetActive(false);
		emNowForType = nowStartUpFormaitonID;

		FormationNodes_1 = Formation_1.gameObject.GetComponentsInChildren<Slot_FormationNode>();
		FormationMans = wrapFormation.gameObject.GetComponentsInChildren<Slot_FormationManager>();

//		FormationNodesPos = new Vector3[FormationNodes_1.Length];
//		for(int i=0; i<FormationNodes_1.Length;++i)
//		{
//			if (FormationNodesPos.Length > i)
//				FormationNodesPos[i] = FormationNodes_1[i].transform.localPosition;
//		}
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 選擇開啟哪個戰陣 </summary>
	public void SwitchTurnOnFormation(ENUM_FormationType em)
	{
		Line_2.gameObject.SetActive(false);
		Line_3.gameObject.SetActive(false);
		Line_4.gameObject.SetActive(false);

		switch(em)
		{
		case ENUM_FormationType.Earth:
			Utility.ChangeAtlasSprite(spP_BG,55);
			Utility.ChangeAtlasSprite(spTitleName,37);
			break;
		case ENUM_FormationType.Fire:
			Utility.ChangeAtlasSprite(spP_BG,56);
			Utility.ChangeAtlasSprite(spTitleName,38);
			break;
		case ENUM_FormationType.Water:
			Utility.ChangeAtlasSprite(spP_BG,57);
			Utility.ChangeAtlasSprite(spTitleName,39);
			break;
		case ENUM_FormationType.Wind:
			Utility.ChangeAtlasSprite(spP_BG,58);
			Utility.ChangeAtlasSprite(spTitleName,40);
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 設定戰陣所有陣眼資料 </summary>
	public void SetAllNodeData(ENUM_FormationType em,S_FormationData forData)
	{
		GameObject go = null;
		go = Formation_1.gameObject;
		int BGGuid = 0;
		switch(em)
		{
		case ENUM_FormationType.Earth:
			//go = Formation_1.gameObject;
			BGGuid = 41;
			break;
		case ENUM_FormationType.Fire:
			//go = Formation_2.gameObject;
			BGGuid = 42;
			break;
		case ENUM_FormationType.Water:
			//go = Formation_3.gameObject;
			BGGuid = 43;
			break;
		case ENUM_FormationType.Wind:
			//go = Formation_4.gameObject;
			BGGuid = 44;
			break;
		}

		//Slot_FormationNode[] Nodes = go.GetComponentsInChildren<Slot_FormationNode>();
		//取戰陣表
		S_Formation_Tmp forTmp = GameDataDB.FormationDB.GetData((int)forData.iFormationID);

		if(forTmp == null)
		{
			UnityDebugger.Debugger.LogError("戰陣表出問題啦!!!");
			return;
		}

		string str = "";
		S_FormationNode_Tmp nodeTmp = null;

		arrayTempPetID.Initialize();

		for(int i = 0;i<forData.iNode.Length;++i)
		{
			FormationNodes_1[i].Init();
			//取陣眼表
			nodeTmp = GameDataDB.FormationNodeDB.GetData(forTmp.INodeID[i]);
			if(nodeTmp == null)
				continue;

			S_PetData sPet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(forData.iNode[i]);

			if(i*10 <= forData.iLV && sPet != null)
			{
				S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,sPet.iPetDBFID);
				
				str = "";
				str = ARPGApplication.instance.CountFormationNodeValue(
					forData.iLV,
					sPet,
					nodeTmp,
					ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData,
					EQList,
					ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems()).ToString();
				str = MakeUpNodeEffect(nodeTmp,str);
			}
			else
			{
				strBuilder.Remove(0,strBuilder.Length);
				str = "";
				str = strBuilder.AppendFormat("{0}\n{1}",GameDataDB.GetString(288),SwitchEffectString(nodeTmp)).ToString();
				//str = string.Format("{0}\n{1}",GameDataDB.GetString(288),SwitchEffectString(nodeTmp));
			}
			arrayTempPetID[i] = forData.iNode[i];
			FormationNodes_1[i].SetSlot(i,str,arrayTempPetID[i],forData.iLV,BGGuid,nodeTmp.iAbility);
		}
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 選擇效果文字 </summary>
	public string SwitchEffectString(S_FormationNode_Tmp node_tmp)
	{
		string str = "";

		str = GameDataDB.GetString(node_tmp.iName);

		return str;
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 計算戰陣升級素質) </summary>
	public int CountFormationLvUp(int ForLv,int Abvalue,int AbValueGrow,int AbValueRange)
	{
		int i = (int)(AbValueGrow * (0.5f * Mathf.RoundToInt(ForLv/(float)AbValueRange)));
		return i;
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 設定陣眼效果文字 </summary>
	public string MakeUpNodeEffect(S_FormationNode_Tmp nodeTmp,string EffectNum)
	{
		strBuilder.Remove(0,strBuilder.Length);
		return strBuilder.AppendFormat("{0}\n{1}\n{2}{3}{4}{5}",GameDataDB.GetString(288),SwitchEffectString(nodeTmp),GameDataDB.GetString(1326),EffectNum,GameDataDB.GetString(1329),GameDataDB.GetString(289)).ToString();
		//return string.Format("{0}\n{1}\n{2}{3}{4}{5}",GameDataDB.GetString(288),SwitchEffectString(nodeTmp),GameDataDB.GetString(1326),EffectNum,GameDataDB.GetString(1329),GameDataDB.GetString(289));
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 設定Title文字 </summary>
	public void SetTitleLabel(int RoleLV,int FormationLV,ENUM_FormationType emFormation)
	{
		S_Formation_Tmp sformation_Tmp =GameDataDB.FormationDB.GetData((int)emFormation); 

		S_FormationNode_Tmp node = GameDataDB.FormationNodeDB.GetData(sformation_Tmp.INodeID[0]);

		float rate = node.iAbilityValue + node.iAbilityValueGrow * FormationLV;

		rate *= 100;

		string strRate = rate.ToString("F1") + '%';

		lbLevelInfo.text = string.Format(GameDataDB.GetString(260),FormationLV,GameDefine.DEF_FORMATION_LEVELMAX,"\n",strRate);
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 設定升級花費文字 </summary>
	public void SetUpLvMoney(int ForLV)
	{
		S_FormationCost_Tmp ForCost_Tmp = GameDataDB.FormationCostDB.GetData(ForLV);

		if(ForCost_Tmp == null)
		{
			UnityDebugger.Debugger.LogError("戰陣升級花費表出問題啦!!!");
		}

		lbLvUpMoney.text = ForCost_Tmp.iFormationCostMoney.ToString();
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 未出戰解鎖夥伴收集列表 </summary>
	public void PrepareUnlockPetList(ENUM_FormationType em,int PetID)
	{
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetListSize()==0)
			return;
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.FormationData.ContainsKey(em) == false)
			return;

		S_FormationData forData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.FormationData[em];

		int[] iRolePetData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BattlePetIDs;
		//過濾裝備中的寵物
		for (int i = 0; i < iRolePetData.Length; i++)
		{
			switch(i)
			{
			case 0:
				Pet1DBFID = iRolePetData[i];
				break;
			case 1:
				Pet2DBFID = iRolePetData[i];
				break;
			}
		}
		m_unlockPetList.Clear();
		foreach(S_PetData pd in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData)
		{
			if(Pet1DBFID == pd.iPetDBFID)
				continue;
			else if(Pet2DBFID == pd.iPetDBFID)
				continue;
			else if(pd.iPetLevel == GameDefine.PET_PIECE_LEVEL)
				continue;
			/*else if(CheckInNodesPet(pd.iPetDBFID,forData.iNode))
				continue;*/
			else if(pd.iPetDBFID ==  PetID)
				continue;
			else
				m_unlockPetList.Add(pd);
		}
		m_unlockPetList.Sort((x, y) => { return -x.GetPetRank().CompareTo(y.GetPetRank()); });
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 未出戰解鎖夥伴收集列表 </summary>
	public bool CheckInNodesPet(int ID,int[] nodes)
	{
		for (int i = 0; i < nodes.Length; i++) 
		{
			if(ID == nodes[i])
			{
				return true;
			}
		}
		return false;
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 設定升級道具 </summary>
	public void SetUpGradeItem(int forLv)
	{
		S_FormationCost_Tmp costTmp = GameDataDB.FormationCostDB.GetData(forLv);
		if(costTmp == null)
		{
			UnityDebugger.Debugger.LogError("戰陣升級花費表出問題啦!!!");
		}
		S_FormationCost_Tmp NextCostTmp = GameDataDB.FormationCostDB.GetData(forLv + 1);
		if(NextCostTmp == null)
		{
			UpgradeItem.gameObject.SetActive(false);
			spLvUpMoney.gameObject.SetActive(false);
			btnUpgrade.isEnabled = false;
		}
		else if(costTmp.iFormationCostItemCount == 0)
		{
			spLvUpMoney.gameObject.SetActive(true);
			//UpgradeItem.ClearSlot();
			//UpgradeItem.collider.enabled = false;
			//btnUpgrade.isEnabled = true;
			UpgradeItem.gameObject.SetActive(false);
		}
		else
		{
			UpgradeItem.gameObject.SetActive(true);
			spLvUpMoney.gameObject.SetActive(true);
			btnUpgrade.isEnabled = true;
			UpgradeItem.ClearSlot();
			UpgradeItem.GetComponent<Collider>().enabled = true;
			UpgradeItem.SetSlotForFormation(costTmp);
		}
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 升級HUD文字 </summary>
	public void LvUpHUDString()
	{
		S_Formation_Tmp nowForTmp = GameDataDB.FormationDB.GetData((int)emNowForType);
		S_FormationData forData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.FormationData[emNowForType];

		string str = "";
		int now = 0;
		int last = 0;
		S_FormationNode_Tmp nodeTmp = null;

		for(int i = 0;i<forData.iNode.Length;++i)
		{
			if(i*10 > forData.iLV)
			{
				continue;
			}
			//取陣眼表
			nodeTmp = GameDataDB.FormationNodeDB.GetData(nowForTmp.INodeID[i]);
			if(nodeTmp == null)
				continue;
			str = "";
			str = SwitchEffectString(nodeTmp) +" + ";
			//now = CountFormationLvUp(forData.iLV,nodeTmp.iAbility,nodeTmp.iAbilityValue,nodeTmp.iAbilityValueGrow,nodeTmp.iAbilityValueRange);
			//last = CountFormationLvUp(forData.iLV - 1,nodeTmp.iAbility,nodeTmp.iAbilityValue,nodeTmp.iAbilityValueGrow,nodeTmp.iAbilityValueRange);
			str += (now - last).ToString();
			strBuilder.Remove(0,strBuilder.Length);
			str = strBuilder.AppendFormat("{0}{1}{2}", GameDataDB.GetString(1326),str, GameDataDB.GetString(1329)).ToString();
			ARPGApplication.instance.m_UIHUDmsg.SetMsg(str);
		}
	}
	//-------------------------------------------------------------------------------------------------
	/// <summary> 升級HUD文字 </summary>
	public void LvUpFormationString(string name,int Lv)
	{
		strBuilder.Remove(0,strBuilder.Length);
		strBuilder.AppendFormat(GameDataDB.GetString(290),name,Lv);
		ARPGApplication.instance.m_uiMessageBox.SetMsgBox(strBuilder.ToString());
	}
}
