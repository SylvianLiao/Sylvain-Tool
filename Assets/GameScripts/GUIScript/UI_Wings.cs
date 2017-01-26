using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_Wings : NGUIChildGUI 
{
	public UIPanel			panelBase					= null;
	public UIButton			btnExplain					= null;
	public UIButton			btnClose					= null;	
	public GameObject		gBackGround					= null;	

	[Header("Wings Info")]					  	
	public UILabel			lbWingsName					= null;
	public UILabel			lbWingsLevel				= null;
	public GameObject		gUnlockCondition			= null;
	public UILabel			lbUnlockConditionTitle		= null;
	public UILabel			lbUnlockCondition			= null;
	public UIGrid			gdWingsAttribute			= null;
	public UILabel			lbWingsAttribute			= null;	//複製用樣版
	public UIButton			btnUpgrade					= null;	
	public UILabel			lbUpgrade					= null;
	public UISprite			spUpgradeTip				= null;
	public UILabel			lbWingsNote					= null;

	[Header("Wings Box")]	
	public UIScrollView		scWingsBox					= null;
	public UIWrapContentEX	wcWingsBox					= null;
	public UISprite			spChoosenFrame				= null;

	[Header("Wings Page")]
	public UIButton			btnWings					= null;
	public UILabel			lbWings						= null;
	public UIButton			btnFeathers					= null;	
	public UILabel			lbFeathers					= null;

	[Header("Role Power")]
	public UILabel			lbRolePower					= null;
	public AddNumber		jumpNumber					= null;

	[Header("3D Model")]
	public GameObject		gModelPosition				= null;
	public SpinWithMouse	spinRotateModel				= null;

	[Header("Upgrade Effect")]
	public GameObject		gUpgradeEffect				= null;
	//-------------------------------------------------------------------------------------------------
	public List<UIGrid>									m_GridGroupWings		= new List<UIGrid>();
	[NonSerialized] public Dictionary<int,Slot_Wings[]> m_WrapContentObjList	= new Dictionary<int, Slot_Wings[]>();
	[NonSerialized] public List<S_WingUpgrade_Tmp>		m_WingsListForShow		= new List<S_WingUpgrade_Tmp>();
	[NonSerialized] public List<UILabel>				m_LabelAttributeList	= new List<UILabel>();
	[NonSerialized] public UI_SpeedUpBoard2				m_uiSpeedUpBoard2		= null;
	[NonSerialized] private ARPGModuleDisplay 			m_PlayerModel			= null;
	[HideInInspector]public GameObject 					m_CloneBackGround		= null;
	//-------------------------------------------------------------------------------------------------
	public int 				m_WingsNumOneRow			= 4;
	private const int 		m_WingsNumOnePage			= 8;
	private const string 	m_SlotWingsName				= "Slot_Wings";
	private const string 	m_UISpeedUpBoard2Name		= "UI_SpeedUpBoard2";

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME 		= "UI_Wings";
	
	//-------------------------------------------------------------------------------------------------
	private UI_Wings() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();

		InitialBackGround();
		m_PlayerModel = new ARPGModuleDisplay();
		CreatePlayerModel(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.iCosBack);
		jumpNumber.InitialUI();
		InitialWingsData();
		InitialLabel();
		CreateUpgradeUI();
		CreateSlotWings();

		gUpgradeEffect.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		ShowModelActAnimation(ARPGAnimation.AnimIndex.Stay);
	}
	//-------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	#region 設定資料或UI相關
	//-------------------------------------------------------------------------------------------------
	//生成背景於另一支UI Camera
	private void InitialBackGround()
	{
		GameObject gb = GameObject.Find("UIBGCamera");
		if(gb != null)
		{
			m_CloneBackGround = NGUITools.AddChild(gb,gBackGround);
			m_CloneBackGround.layer = gb.layer;
			m_CloneBackGround.SetActive(true);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//生成翅膀顯像用的資料
	public void InitialWingsData()
	{
		if (m_WingsListForShow.Count > 0)
			m_WingsListForShow.Clear();

		C_RoleDataEx roleDataEx = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		foreach (KeyValuePair<int, List<S_WingUpgrade_Tmp>> tmpData in GameDataDB.WingUpgradeList)
		{
			bool isOwn = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckIsOwnWings(tmpData.Key);
			//若未擁有該翅膀則加入該翅膀一級的資料
			if (!isOwn)
				m_WingsListForShow.Add(tmpData.Value[0]);
			else
			{
				if (roleDataEx.WingDataList[tmpData.Key] != null)
					m_WingsListForShow.Add(roleDataEx.WingDataList[tmpData.Key]);
			}
		}
		scWingsBox.enabled = m_WingsListForShow.Count>m_WingsNumOnePage;
	}
	//-------------------------------------------------------------------------------------------------
	private void InitialLabel()
	{
		lbUnlockConditionTitle.text = GameDataDB.GetString(8331);		//"獲得條件："
		lbWingsNote.text 			= GameDataDB.GetString(8310)+"\n"+GameDataDB.GetString(8311);		//"獲得翅膀即獲得對應屬性 \n 無論裝備，效果皆會累加"
		lbWings.text				= GameDataDB.GetString(8301);		//"翅膀"
		lbFeathers.text				= GameDataDB.GetString(8351);		//"神羽"

		lbRolePower.text = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetMainRolePower().ToString();
	}
	//-------------------------------------------------------------------------------------------------
	private void CreateSlotWings()
	{
		Slot_Wings go = ResourceManager.Instance.GetGUI(m_SlotWingsName).GetComponent<Slot_Wings>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("UI_Wings CreateSlotWings() error,path:{0}", "GUI/"+m_SlotWingsName) );
			return;
		}

		for(int i=0; i<m_GridGroupWings.Count; ++i)
		{
			for(int m=0; m<m_WingsNumOneRow; ++m)
			{
				Slot_Wings newgo = NGUITools.AddChild(m_GridGroupWings[i].gameObject,go.gameObject).GetComponent<Slot_Wings>();
				newgo.name = m_SlotWingsName+m.ToString();
				newgo.Initialize();
			}
			m_GridGroupWings[i].enabled = true;
			m_GridGroupWings[i].Reposition();
		}
	}
	//-------------------------------------------------------------------------------------------------
	//生成翅膀升級UI
	private void CreateUpgradeUI()
	{
		UI_SpeedUpBoard2 go = ResourceManager.Instance.GetGUI(m_UISpeedUpBoard2Name).GetComponent<UI_SpeedUpBoard2>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_Wings CreateUpgradeUI() error,path:{0}", "GUI/"+m_UISpeedUpBoard2Name) );
			return;
		}
		
		UI_SpeedUpBoard2 newgo = NGUITools.AddChild(this.gameObject,go.gameObject).GetComponent<UI_SpeedUpBoard2>();;
		newgo.InitialUI();
		newgo.InitialLabel(8303,8302,8323,8304);
		m_uiSpeedUpBoard2 = newgo;
	}
	//-------------------------------------------------------------------------------------------------
	//將UI設定好的翅膀Slot物件加入WrapContent管理，並刷新資料
	public void InitialWrapContent()
	{
		for(int i=0; i<m_GridGroupWings.Count; ++i)
		{
			Slot_Wings[] slotWingsGroup= m_GridGroupWings[i].GetComponentsInChildren<Slot_Wings>();
			
			if (slotWingsGroup == null)
				continue;
			m_WrapContentObjList.Add(i, slotWingsGroup);
		}
		
		//根據翅膀資料數量設定WrapContent參數
		wcWingsBox.maxIndex = (-1)*Mathf.CeilToInt((float)m_WingsListForShow.Count / m_WingsNumOneRow);
		wcWingsBox.minIndex = 0;
		wcWingsBox.onInitializeItem = AssignWingsData;
		
		//關閉用不到的WrapContent元件
		List<Slot_Wings> slotWingsList = new List<Slot_Wings>();
		foreach(Slot_Wings[] value in m_WrapContentObjList.Values)
		{
			List<Slot_Wings> tempList = new List<Slot_Wings>(value);
			slotWingsList.AddRange(tempList);
		}
		if (slotWingsList.Count > m_WingsListForShow.Count)
		{
			int closeCount = slotWingsList.Count - m_WingsListForShow.Count;
			for(int i=0; i<closeCount; ++i)
			{
				slotWingsList[slotWingsList.Count-1-i].gameObject.SetActive(false);
			}
		}
		wcWingsBox.SortBasedOnScrollMovement();
		wcWingsBox.UpdateAllItem();
	}
	//-------------------------------------------------------------------------------------------------
	//設定翅膀資訊
	public void SetWingsInfo(S_WingUpgrade_Tmp wingsTmp)
	{
		if (wingsTmp == null)
			return;
		S_Item_Tmp	itemTmp = GameDataDB.ItemDB.GetData(wingsTmp.iItemID);
		if (itemTmp == null)
			return;
		C_RoleDataEx roleDataEX =  ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		
		lbWingsName.text = GameDataDB.GetString(itemTmp.iName);
		bool isOwn = roleDataEX.CheckIsOwnWings(itemTmp.GUID);
		lbWingsLevel.gameObject.SetActive(isOwn);
		gUnlockCondition.SetActive(!isOwn);
		if (isOwn)
		{
			lbWingsLevel.text = GameDataDB.GetString(8302)+" "+wingsTmp.iLevel.ToString();
		}
		else
		{
			lbUnlockCondition.text = GameDataDB.GetString(wingsTmp.iNoteID);
		}
		CreateAndSetLabelAttribute(wingsTmp);
	}
	//-------------------------------------------------------------------------------------------------
	//建立並設定翅膀屬性字串
	private void CreateAndSetLabelAttribute(S_WingUpgrade_Tmp wingsTmp)
	{
		C_RoleDataEx roleDataEX =  ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		List<string> strAttrList = wingsTmp.GetWingsAllAttributeString();
		if (strAttrList == null)
			return;
		
		if (strAttrList.Count > m_LabelAttributeList.Count)
		{
			int createCount = strAttrList.Count - m_LabelAttributeList.Count;
			for(int i=0; i<createCount; ++i)
			{
				UILabel newgo = NGUITools.AddChild(gdWingsAttribute.gameObject,lbWingsAttribute.gameObject).GetComponent<UILabel>();
				if (newgo == null)
				{
					UnityDebugger.Debugger.Log(this.name+" CreateLabelAttribute() Error!! lbWingsAttribute = "+newgo);
					return;
				}
				m_LabelAttributeList.Add(newgo);
				newgo.gameObject.SetActive(false);
			}
		}
		
		for(int i=0; i<m_LabelAttributeList.Count; ++i)
		{
			if (i >= strAttrList.Count)
				m_LabelAttributeList[i].gameObject.SetActive(false);
			else
			{
				m_LabelAttributeList[i].text = strAttrList[i];
				m_LabelAttributeList[i].gameObject.SetActive(true);
			}
		}
		gdWingsAttribute.enabled = true;
		gdWingsAttribute.Reposition();
	}
	//-------------------------------------------------------------------------------------------------
	//WrapContent更新資料用的CallBack
	private void AssignWingsData(GameObject wingsObj, int wrapIndex, int realIndex)
	{
		//		UnityDebugger.Debugger.Log("wrapIndex = "+wrapIndex+" ,realIndex = "+realIndex);
		
		if (!m_WrapContentObjList.ContainsKey(wrapIndex))
		{
			UnityDebugger.Debugger.Log("WrapContent Index Error!! WrapContent Object Group Count = "+m_WrapContentObjList.Count+" ,wrapIndex = "+wrapIndex);
			return;
		}
		realIndex = Mathf.Abs(realIndex);
		Slot_Wings[] slotWingsGroup = m_WrapContentObjList[wrapIndex];
		
		//關閉沒資料的WrapContent元件
		int remanent = slotWingsGroup.Length - ((realIndex+1)*slotWingsGroup.Length-m_WingsListForShow.Count);	//該列需關閉的翅膀數量 = 一列的翅膀數量 - (目前列數 * 一列的翅膀數量 - 翅膀需顯示的數量)
		CloseRemanentSlot(wrapIndex, remanent);
		if (remanent >= 0)
		{
			//更新slot圖示
			for(int i=0; i<slotWingsGroup.Length; ++i)
			{
				if (i >= remanent)
					continue;
				slotWingsGroup[i].SetSlotWings(m_WingsListForShow[i+(slotWingsGroup.Length*realIndex)]);
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	//設定翅膀選擇框
	public void SetChoosenFrame(GameObject go)
	{
		if (go == null)
			return;
		spChoosenFrame.transform.parent = go.transform;
		spChoosenFrame.transform.localPosition = Vector3.zero;
	}
	//-------------------------------------------------------------------------------------------------
	//設定玩家戰力
	public void SetRolePower(int power = 0)
	{
		if (power <= 0)
			power = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetMainRolePower();
		jumpNumber.SetEndNumber(power);
	}
	#endregion
	#region 玩家3D模型相關
	//-----------------------------------------------------------------------------------------------------
	//建立玩家角色模型
	public void CreatePlayerModel(int wingsItemID)
	{
		if (m_PlayerModel.PlayerPrefab != null)
			GameObject.DestroyImmediate(m_PlayerModel.PlayerPrefab);

		//根據傳入的翅膀顯示3D模型
		C_RoleDataEx roleDataEx = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		S_PlayerAvatar fakeAvatar = new S_PlayerAvatar(roleDataEx.m_roleSexType, 
		                                               roleDataEx.BaseRoleData.iBodyDBID,
		                                               roleDataEx.BaseRoleData.iHeadDBID,
		                                               roleDataEx.BaseRoleData.iWeaponDBID,
		                                               roleDataEx.BaseRoleData.iCosClothes,
		                                               wingsItemID);
		m_PlayerModel.PlayerShow(fakeAvatar,true);
		m_PlayerModel.PlugInAnimation(ARPGModuleDisplay.RoleKinds.Player);
		AssignModelPosition(m_PlayerModel.PlayerPrefab);
		//設定旋轉角色
		spinRotateModel.target = m_PlayerModel.PlayerPrefab.transform;
	}
	//-----------------------------------------------------------------------------------------------------
	//指定玩家模型位置
	private void AssignModelPosition(GameObject gb)
	{
		gb.transform.parent = gModelPosition.transform;
		gb.transform.localPosition = Vector3.zero;
		gb.transform.localRotation = new Quaternion(0,180,0,0);
		gb.transform.localScale = new Vector3(200,200,200);
		//改變layer
		m_PlayerModel.RecursiveChangeLayers(gb.transform,"NGUI");
	}
	//-----------------------------------------------------------------------------------------------------
	//玩家模型播放角色動作
	public void ShowModelActAnimation(ARPGAnimation.AnimIndex action)
	{
		//停止先前所有動作並播放目前角色的動作
		ARPGAnimation 	comARPGAni;
		Animation		comAni;
		string			PreShowBehavior = "";
		comARPGAni 	= m_PlayerModel.PlayerPrefab.GetComponent<ARPGAnimation>();
		comAni		= m_PlayerModel.PlayerPrefab.GetComponent<Animation>();
		if(comARPGAni == null)
			return;
		
		if(comAni == null)
			return;
	
		PreShowBehavior = comARPGAni.animNames[(int)action];		//指定哪一個動作
		
		if(comAni.isPlaying)
			comAni.Stop();
		
		comAni.CrossFade(PreShowBehavior);
		comAni.Play();
		
		comAni.CrossFade(comARPGAni.animNames[(int)ARPGAnimation.AnimIndex.Stay]);
	}
	#endregion
	//-------------------------------------------------------------------------------------------------
	//關閉該列(grid)用不到的翅膀
	private void CloseRemanentSlot(int groupIndex, int remanent)
	{
		if (!m_WrapContentObjList.ContainsKey(groupIndex))
			return;

		Slot_Wings[] slotWingsGroup = m_WrapContentObjList[groupIndex];
		if (slotWingsGroup == null)
			return;
		if (remanent >= slotWingsGroup.Length)
			return;
		for(int i=0; i<slotWingsGroup.Length; ++i)
		{
			if (i < remanent)
				continue;
			slotWingsGroup[i].gameObject.SetActive(false);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchBtnUpgrade(bool bSwitch)
	{
		btnUpgrade.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchUpgradeTip(bool bSwitch)
	{
		spUpgradeTip.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void PlayUpgradeEffect()
	{
		gUpgradeEffect.gameObject.SetActive(false);
		gUpgradeEffect.gameObject.SetActive(true);
	}
}
