using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_PetUpgrade : NGUIChildGUI 
{
	public UILabel			lbUpgradeTilte		= null; //主標
	public UILabel			lbPetTypeName		= null; //種族名
	public UISprite			spPetClass			= null; //職業圖
	public UIPanel			panelMaskUpgrading	= null;
	[Header("經驗值")]
	public UILabel			lbExpTitle			= null; //經驗值標題
	public UISprite			spExpValue			= null; //經驗值條
	public UILabel			lbExpValue			= null; //經驗值數值
	public UILabel			lbWarningMark		= null; //經驗值超過提示
	[Header("屬性數值")]
	public UILabel[]		lbAttributeTitle	= new UILabel[8]; // 等級 生命 法攻 法防 物攻 物防 爆擊 突破數
	public UILabel[]		lbAttributeValue	= new UILabel[8];
	public UISprite[]		spStars				= new UISprite[4];
	[Header("升級預覽")]
	public UIWidget			PreviewSet			= null; // 預覽集合
	public UILabel[]		lbPreviewValues		= new UILabel[7];
	[Header("已選擇道具列")]
	public UILabel			lbSelectedTitle		= null; //已選 標題
	public UIWidget			SelectedItemPrefab  = null; //已選擇道具prefab
	[Header("培養道具列")]
	public UILabel			lbExpItemTitle		= null; //可選擇經驗值道具標題
	public ExpItemData		ExpItemPrefab		= null; //經驗值道具
	public UIScrollView		svExpItemList		= null; //經驗值道具列表
	public UIGrid			gdExpItemList		= null; //經驗值道具排序
	[Header("底部按鈕")]
	public UIButton			btnAutoSelect		= null; //自動選擇
	public UILabel			lbAutoSelect		= null; //自動選擇字樣
	public UIButton			btnUpgrade			= null; //夥伴強化
	public UILabel			lbUpgrade			= null; //夥伴強化字樣
	[Header("多數使用功能")]
	public UIPanel	panelSetItemCounts		= null; //設定吃道具的數量介面
	public UIButton	btnPlus					= null; //加道具數
	public UIButton btnMinus				= null; //減道具數
	public UIButton	btnQuit					= null; //離開
	public UILabel	lbQuit					= null;
	public UIButton	btnSetItemValue			= null; //設定道具數
	public UILabel	lbSetItemValue			= null;
	public UILabel	lbItemCounter			= null; //道具數值
	[Header("加載Tween的物件")]
	public List<GameObject>	TweenGameObjects	= new List<GameObject>();

	//
	private int				m_PetGUID 			= 0;
	private S_PetData		m_PetData			= null;
	//
	[HideInInspector]public int		iExp			= 0;		//紀錄現有經驗值
	[HideInInspector]public int 	iPlusExp		= 0;		//素材所加上的經驗值
	[HideInInspector]public int 	iMaxExp			= 0;		//紀錄下一次升級經驗值
	[HideInInspector]public bool 	isUpdateExp		= false;
	[HideInInspector]public bool 	bOverLV			= false;
	//
	[HideInInspector]public	int 			CheckItemindex;		//點擊暫存的index
	[HideInInspector]public	int 			CheckItemIcon;		//點擊暫存的Icon
	[HideInInspector]public	int 			CheckItemGUID;		//點擊暫存的GUID
	[HideInInspector]public int			iItemCount			= -1;		// 紀錄點擊的物品堆疊數
	[HideInInspector]public float		ForSaleNum			= 1;		// 販賣數量(起始是1)
	private bool						bPlusPress			= false;	// 是否按住Plus鍵;
	private bool						bMinusPress			= false;	// 是否按住Minus鍵;
	private float						Speedtime			= 0;		// 記算快速增加/減少時間
	private const float					MinSaleValue 		= 2;		// 最小出售數量
	//
	private List<GameObject>		SItems		= new List<GameObject>();
	[HideInInspector]public List<UISprite>			ItemIcons	= new List<UISprite>();
	[HideInInspector]public List<UISprite>			sItemBGs	= new List<UISprite>();
	[HideInInspector]public List<UISprite>			sItemBorders= new List<UISprite>();
	[HideInInspector]public List<UIButton>			CancelBtn	= new List<UIButton>();
	[HideInInspector]public List<UILabel>			ItemCounts	= new List<UILabel>();
	//
	[HideInInspector]public List<GameObject>		ExpItems	= new List<GameObject>();
	//
	[HideInInspector]public List<uItemData>		m_ExpDatas 		= new List<uItemData>();		//經驗值道具列
	[HideInInspector]public List<selectData>	m_SelectDatas	= new List<selectData>();		//暫存已選擇道具

	private bool				bRepositionItemList = false;
	//
	[HideInInspector]public int[] 			m_NutrientItemDBID 	= new int[GameDefine.MAX_PET_NUTRIENT];
	[HideInInspector]public int[] 			m_ItemCount 		= new int[GameDefine.MAX_PET_NUTRIENT];
	[HideInInspector]public int 			m_iCurrentMaxExp 	= -1; 			//紀錄最高可以接受的經驗值
	[HideInInspector]public bool			bExpItemPutting		= false;		//擋放上道具流程重覆進入
	[HideInInspector]public bool 			bAutoPut			= false;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_PetUpgrade";
	
	//-------------------------------------------------------------------------------------------------
	private UI_PetUpgrade() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		//先clone出5個被選擇的按鈕
		CloneMultiSelectedPrefab();
		//
		lbUpgradeTilte.text			= GameDataDB.GetString(261); 		//升級
		lbExpTitle.text				= GameDataDB.GetString(1026);		//經驗值
		lbAttributeTitle[0].text	= GameDataDB.GetString(1056);		//"等級"
		lbAttributeTitle[1].text	= GameDataDB.GetString(1027);		//"生命值"
		lbAttributeTitle[2].text	= GameDataDB.GetString(1063);		//"仙術攻擊力"
		lbAttributeTitle[3].text	= GameDataDB.GetString(1064);		//"仙術防禦力"
		lbAttributeTitle[4].text	= GameDataDB.GetString(1028);		//"攻擊力"
		lbAttributeTitle[5].text	= GameDataDB.GetString(1030);		//"防禦力"
		lbAttributeTitle[6].text	= GameDataDB.GetString(1025);		//"爆擊率"
		lbAttributeTitle[7].text 	= GameDataDB.GetString(2602)+":";	//"突破"
		//出售介面字串指定
		lbQuit.text 				= GameDataDB.GetString(1034); 		//取消
		lbSetItemValue.text 		= GameDataDB.GetString(1033); 		//確定
		//
		lbSelectedTitle.text		= GameDataDB.GetString(989);		//已選
		lbExpItemTitle.text			= GameDataDB.GetString(990);		//可選
		//
		lbAutoSelect.text			= GameDataDB.GetString(151);		//自動添加
		lbUpgrade.text				= GameDataDB.GetString(261); 		//升級
		//蒐集背包中經驗值道具
		CollectExpItems();
		//建立出存放經驗值道具物件數
		CloneExpItemGameobject(m_ExpDatas.Count);

	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		//載入資料
		LoadUpgradePetData();
		//重載tween
		ARPGApplication.instance.ResetTweenObjects(TweenGameObjects);
		//關閉遮罩
		panelMaskUpgrading.gameObject.SetActive(false);
		//將資料餵給已經生成出來的物件
		AssignExpDataToItems();
		//初始化已選擇資料(預設皆為-1,數量為0)
		ClearSelectDatas();
		//設定所需最高的經驗值數(己扣除擁有的經驗值)
		m_iCurrentMaxExp = CalNeededMaxExp();
	}
	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		if(bRepositionItemList)
		{
			svExpItemList.ResetPosition();
			bRepositionItemList = false;
		}

		if(isUpdateExp && iMaxExp!=0)
		{	
			isUpdateExp = false;
			int iFinalHP = iExp+iPlusExp;
			float fProcess;
			if(iFinalHP>iMaxExp)
				fProcess=1;
			else
				fProcess = (float)iFinalHP / (float)iMaxExp;
			
			spExpValue.fillAmount = fProcess;
			if(iPlusExp<=0)
				lbExpValue.text = iExp.ToString()+"/"+iMaxExp.ToString();
			else
				lbExpValue.text = iExp.ToString()+"+([00FF00]"+iPlusExp.ToString()+"[-])/"+iMaxExp.ToString();
			//啟用預覽
			PreviewAfterUpGrade();
		}
		
		
		//增加出售數量事件
		if (bPlusPress)
		{
			if(ForSaleNum<((float)iItemCount))
			{
				Speedtime+=Time.deltaTime;
				if(Speedtime>0.5f)
					ForSaleNum+=Time.deltaTime*10;
				//防止超過
				if((int)ForSaleNum > iItemCount)
					ForSaleNum = (float)iItemCount;
				
				lbItemCounter.text = ((int)ForSaleNum).ToString()+"/"+iItemCount.ToString();
			}
		}
		
		//減少出售數量事件
		if (bMinusPress)
		{
			if(ForSaleNum>MinSaleValue)		//要出售數量大於最低限度的出售值才能動作
			{
				Speedtime+=Time.deltaTime;
				if(Speedtime>0.5f)
					ForSaleNum-=Time.deltaTime*10;
				
				lbItemCounter.text = ((int)ForSaleNum).ToString()+"/"+iItemCount.ToString();
			}
		}
		//
		if (!bPlusPress && !bMinusPress)
			Speedtime = 0;
	}
	//-------------------------------------------------------------------------------------------------
	private void CloneMultiSelectedPrefab()
	{
		Transform t;
		for(int i=0;i<GameDefine.MAX_PET_NUTRIENT;++i)
		{
			GameObject gb = NGUITools.AddChild(SelectedItemPrefab.transform.parent.gameObject,SelectedItemPrefab.gameObject);
			gb.transform.localPosition = SelectedItemPrefab.transform.localPosition+ Vector3.right*95*i;
			t= gb.transform.FindChild("Sprite(ItemIcon)");
			ItemIcons.Add(t.GetComponent<UISprite>());
			t= gb.transform.FindChild("Frame(BackGround)");
			sItemBGs.Add(t.GetComponent<UISprite>());
			t= gb.transform.FindChild("Frame(Border)");
			sItemBorders.Add(t.GetComponent<UISprite>());
			t= gb.transform.FindChild("Button(CancelItem)");
			t.gameObject.SetActive(false);
			CancelBtn.Add(t.GetComponent<UIButton>());
			t.GetComponent<UIButton>().userData = i;
			t.FindChild("Label").GetComponent<UILabel>().text = GameDataDB.GetString(1034);		//"取消"
			t= gb.transform.FindChild("Label(ItemCount)");
			ItemCounts.Add(t.GetComponent<UILabel>());
			t.GetComponent<UILabel>().gameObject.SetActive(false);
			SItems.Add(gb);
		}
		//原prefab隱藏
		SelectedItemPrefab.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void SetPetGUIDbyUpGrade(int petGUID)
	{
		m_PetGUID = petGUID;
	}
	//-------------------------------------------------------------------------------------------------
	public void LoadUpgradePetData()
	{
		if(m_PetGUID<=0) 
		{
			Hide();
			return;
		}
		S_PetData pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(m_PetGUID);
		if(pd == null)
		{
			Hide();
			return;
		}
		if(pd.iPetLevel == 0)
		{
			Hide();
			return;
		}
		m_PetData = pd;
		S_PetData_Tmp 		pdTmp 		= GameDataDB.PetDB.GetData(m_PetGUID);
		lbPetTypeName.text = GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(pdTmp.emCharType));
		Utility.ChangeAtlasSprite(spPetClass,ARPGApplication.instance.GetPetCalssIconID(pdTmp.emCharClass));
		//設定經驗值
		iExp = pd.iPetExp;
		iMaxExp = GameDataDB.PetLevelUpDB.GetData(pd.iPetLevel).iExpRank[pd.GetPetRank()-1];
		isUpdateExp = true;
		//
		for(int i=0;i<spStars.Length;++i)
		{
			spStars[i].gameObject.SetActive(false);
			if(pd.iPetLimitLevel>i)
				spStars[i].gameObject.SetActive(true);
		}
		LoadPetAttributeValue(pd);
	}
	//-------------------------------------------------------------------------------------------------
	private void LoadPetAttributeValue(S_PetData pd)
	{
		//設定計算數值
		S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,pd.iPetDBFID);
		S_ItemData[] AllEQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems();
		//設定是否載入戰陣
		S_FormationData formationData = null;
		bool bCalculateFormation = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.DetectPetforBattle(m_PetGUID);
		if(bCalculateFormation)
			formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
		List<S_PetData> sPet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData;
		S_RoleAttr PetAttrValue = ARPGCharacter.CreatePetRoleAttr(pd,EQList,sPet,pd.GetTalentSkill(),null,formationData,AllEQList);
		int cal_HP 	= (int)(Math.Round(PetAttrValue.sBattleFinial.fMaxHP,		0,MidpointRounding.AwayFromZero));
		int cal_ATK = (int)(Math.Round(PetAttrValue.sBattleFinial.fAttack,		0,MidpointRounding.AwayFromZero));
		int cal_DEF = (int)(Math.Round(PetAttrValue.sBattleFinial.fDefense,		0,MidpointRounding.AwayFromZero));
		int cal_CRIT = (int)(Math.Round(PetAttrValue.sBattleFinial.fCritsRate,	0,MidpointRounding.AwayFromZero));
		int cal_AP 	= (int)(Math.Round(PetAttrValue.sBattleFinial.fAbilityPower,0,MidpointRounding.AwayFromZero));
		int cal_MR 	= (int)(Math.Round(PetAttrValue.sBattleFinial.fMagicResist,	0,MidpointRounding.AwayFromZero));

		lbAttributeValue[0].text			= pd.iPetLevel.ToString();
		lbAttributeValue[1].text			= cal_HP.ToString();
		lbAttributeValue[2].text			= cal_AP.ToString();	
		lbAttributeValue[3].text			= cal_MR.ToString();
		lbAttributeValue[4].text			= cal_ATK.ToString();
		lbAttributeValue[5].text			= cal_DEF.ToString();
		lbAttributeValue[6].text			= cal_CRIT.ToString();
		lbAttributeValue[7].text			= pd.iPetLimitLevel.ToString();
	}
	//-------------------------------------------------------------------------------------------------
	//預覽強化後如有升級的變化
	private void PreviewAfterUpGrade()
	{
		int PetLV = m_PetData.iPetLevel;
		int iPetRank = m_PetData.GetPetRank();
		int curPetNeedExp = GameDataDB.PetLevelUpDB.GetData(PetLV).iExpRank[iPetRank-1];
		int SumEXP = iExp + iPlusExp;
		int iOverExpValue  = 0;
		//
		if(PetLV>=ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel())
		{
			bOverLV = true;
			//警語
			lbWarningMark.gameObject.SetActive(true);
			if( SumEXP>iMaxExp )
				iOverExpValue = SumEXP - iMaxExp;
			else
				lbWarningMark.gameObject.SetActive(false);
			//
			lbWarningMark.text = string.Format(GameDataDB.GetString(2703),iOverExpValue.ToString());
			return;
		}
		else 
		{
			bOverLV = false;
			lbWarningMark.gameObject.SetActive(false);
		}
		
		if(SumEXP<iMaxExp)
		{
			bOverLV = false;
			PreviewSet.gameObject.SetActive(false);
			lbWarningMark.gameObject.SetActive(false);
		}
		else
		{
			while(!(SumEXP<curPetNeedExp))	//當被減的經驗值小於等級相對應所需的經驗值迴圈
			{
				SumEXP-=curPetNeedExp;
				++PetLV;
				curPetNeedExp = GameDataDB.PetLevelUpDB.GetData(PetLV).iExpRank[iPetRank-1];
			}
			//如果超過主角等級就以主角等級為最大限
			if(PetLV>ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel())
			{
				bOverLV = true;
				PetLV = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel();
				//警語
				lbWarningMark.gameObject.SetActive(true);
				iOverExpValue = iExp + iPlusExp - CalNeededMaxExp();
				lbWarningMark.text = string.Format(GameDataDB.GetString(2703),iOverExpValue.ToString());
			}
			else 
			{
				bOverLV = false;
				lbWarningMark.gameObject.SetActive(false);
			}

			//設定計算數值
			S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,m_PetData.iPetDBFID);
			S_ItemData[] AllEQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems();
			S_FormationData formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
			
			S_PetData PrePetData = m_PetData.GetCloneDiffLV(PetLV);
			S_RoleAttr PetAttrValue = ARPGCharacter.CreatePetRoleAttr(PrePetData,EQList,null,m_PetData.GetTalentSkill(),null,formationData,AllEQList);
			S_RoleAttr curValue = ARPGCharacter.CreatePetRoleAttr(m_PetData,EQList,null,m_PetData.GetTalentSkill(),null,formationData,AllEQList);
			//等級差
			lbPreviewValues[0].text = "+("+(PetLV-m_PetData.iPetLevel).ToString()+")";
			//血量差
			int iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fMaxHP,0,MidpointRounding.AwayFromZero));
			int iCurValue = (int)(Math.Round(curValue.sBattleFinial.fMaxHP,0,MidpointRounding.AwayFromZero));
			lbPreviewValues[1].text = "+("+(iAtrValue-iCurValue).ToString()+")";
			//仙術攻擊差
			iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fAbilityPower,0,MidpointRounding.AwayFromZero));
			iCurValue = (int)(Math.Round(curValue.sBattleFinial.fAbilityPower,0,MidpointRounding.AwayFromZero));
			lbPreviewValues[2].text = "+("+(iAtrValue-iCurValue).ToString()+")";
			//仙術防禦差
			iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fMagicResist,0,MidpointRounding.AwayFromZero));
			iCurValue = (int)(Math.Round(curValue.sBattleFinial.fMagicResist,0,MidpointRounding.AwayFromZero));
			lbPreviewValues[3].text = "+("+(iAtrValue-iCurValue).ToString()+")";
			//攻擊差
			iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fAttack,0,MidpointRounding.AwayFromZero));
			iCurValue = (int)(Math.Round(curValue.sBattleFinial.fAttack,0,MidpointRounding.AwayFromZero));
			lbPreviewValues[4].text = "+("+(iAtrValue-iCurValue).ToString()+")";
			//防禦差
			iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fDefense,0,MidpointRounding.AwayFromZero));
			iCurValue = (int)(Math.Round(curValue.sBattleFinial.fDefense,0,MidpointRounding.AwayFromZero));
			lbPreviewValues[5].text = "+("+(iAtrValue-iCurValue).ToString()+")";
			//爆擊率差
			iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fCritsRate,0,MidpointRounding.AwayFromZero));
			iCurValue = (int)(Math.Round(curValue.sBattleFinial.fCritsRate,0,MidpointRounding.AwayFromZero));
			lbPreviewValues[6].text = "+("+(iAtrValue-iCurValue).ToString()+")";
		
			PreviewSet.gameObject.SetActive(true);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//檢查目前擁有的經驗值是否大於等於限制等級所需經驗值
	//計算可接受的經驗值
	public int CalNeededMaxExp()
	{
		S_PetLevelUp_Tmp sPetLV;
		int iCurrentNeededExpValue = 0;
		int RoleLevel = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel();
		if(m_PetData.iPetLevel>RoleLevel)
			return 0;
		
		int i = m_PetData.iPetLevel;
		//先計算出限制等級所需經驗值
		for(;i<=RoleLevel;++i)
		{
			sPetLV = GameDataDB.PetLevelUpDB.GetData(i);
			iCurrentNeededExpValue += sPetLV.iExpRank[m_PetData.GetPetRank()-1];
		}
		if(m_PetData.iPetExp >= iCurrentNeededExpValue)
			return 0;
		else
			return (iCurrentNeededExpValue-m_PetData.iPetExp);
	}
	//-------------------------------------------------------------------------------------------------
	#region 多數使用功能
	//販賣按鈕(增加數量)
	public void PlusForSaleCounter(GameObject gb,bool isdown)
	{
		bPlusPress = isdown;
	}
	//-----------------------------------------------------------------------------------------------------
	//販賣按鈕(減少數量)
	public void MinusForSaleCounter(GameObject gb,bool isdown)
	{
		bMinusPress = isdown;
	}
	//-----------------------------------------------------------------------------------------------------
	public void PlusOnClick(GameObject gb)
	{
		if(ForSaleNum<((float)iItemCount))
		{
			ForSaleNum += 1;
			lbItemCounter.text = ((int)ForSaleNum).ToString()+"/"+iItemCount.ToString();
			MusicControlSystem.StopOnceSound("Sound_System_003");
			MusicControlSystem.PlaySound("Sound_System_003",1);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void MinusOnClick(GameObject gb)
	{
		if(((int)ForSaleNum)>((int)MinSaleValue-1))		//要出售數量大於最低限度的出售值才能動作
		{
			ForSaleNum -= 1;
			lbItemCounter.text = ((int)ForSaleNum).ToString()+"/"+iItemCount.ToString();
			MusicControlSystem.StopOnceSound("Sound_System_003");
			MusicControlSystem.PlaySound("Sound_System_003",1);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void QuitCounter()
	{
		//音效
		MusicControlSystem.StopOnceSound("Sound_System_003");
		MusicControlSystem.PlaySound("Sound_System_003",1);
		//
		ForSaleNum = 1;																// 數量回初始值
		iItemCount = 0;
		panelSetItemCounts.gameObject.SetActive(false);								// 關閉出售頁面
	}
	#endregion 多數使用功能
	//-------------------------------------------------------------------------------------------------
	#region 生成可選擇經驗值物件與相關事件宣告
	public void CloneExpItemGameobject(int amount)
	{
		if(amount == 0)
		{
			ExpItemPrefab.gameObject.SetActive(false);
			return;
		}
		ExpItems.Clear();
		//生成放置道具資訊物件
		for(int i=0;i<amount;++i)
		{
			GameObject gb = NGUITools.AddChild(gdExpItemList.gameObject,ExpItemPrefab.gameObject);
			UIEventListener.Get(gb).onClick		+= ClickExpItem;
			ExpItems.Add(gb);
		}
		ExpItemPrefab.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	//點擊經驗值道具事件
	private void ClickExpItem(GameObject gb)
	{
		//檢查材料列是否有空位
		if(CheckFullList())
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(2715));	//"材料列已滿"
			return;
		}
		//檢查目前夥伴等級
		if(CheckPetLVMoreThanSelfLV())
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(2714));	//"夥伴經驗值已滿"
		}
		//檢查經驗值狀態
		if(m_iCurrentMaxExp == 0)
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(2714));	//"夥伴經驗值已滿"
		}
		//檢查預估經驗值的狀態
		if(iPlusExp>= m_iCurrentMaxExp)
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(2714));	//"夥伴經驗值已滿"
			return;
		}
		//前一步的操作還沒完成時
		if(bExpItemPutting)
			return;
		
		//紀錄
		ExpItemData edata = gb.GetComponent<ExpItemData>();
		CheckItemIcon 	= edata.itemIcon;
		CheckItemindex 	= edata.itemindex;
		CheckItemGUID	= edata.itemGUID;
		iItemCount = edata.itemCount;
		
		//
		if(CheckItemGUID != 0 && iItemCount != 0)
		{
			if(iItemCount==1)	//當物品只有一項時 直接放上
			{
				PushExpItemEvent(CheckItemindex,CheckItemIcon,CheckItemGUID,1);
				
				return;
			}
			else
			{
				MusicControlSystem.StopOnceSound("Sound_System_003");
				MusicControlSystem.PlaySound("Sound_System_003",1);
				//初始畫面與事件指定
				panelSetItemCounts.gameObject.SetActive(true);
				//
				lbItemCounter.text = "1/"+iItemCount.ToString();							// 出售畫面預設顯示
				
				//if (ARPGApplication.instance.m_TeachingSystem.m_CurrentGuideType == ENUM_GUIDETYPE.EnhancePet)
					//up.GuideCheckItemNumber();
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	//檢查夥伴等級是否大於自身等級
	public bool CheckPetLVMoreThanSelfLV()
	{
		S_PetData pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(m_PetGUID);
		if(pd.iPetLevel > ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel())
			return true;
		
		return false;
	}
	//-------------------------------------------------------------------------------------------------
	//檢查已選材料列是否己滿
	public bool CheckFullList()
	{
		for(int i=0;i<GameDefine.MAX_PET_NUTRIENT;++i)
		{
			if(m_NutrientItemDBID[i] == -1)
				return false;
		}
		return true;
	}
	//-------------------------------------------------------------------------------------------------
	//蒐集經驗值道具
	public void CollectExpItems()
	{
		m_ExpDatas.Clear();
		foreach(S_ItemData sItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(sItem.GetItemType() != ENUM_ItemType.ENUM_ItemType_EXPItem)
				continue;
			
			uItemData ud = new uItemData();
			ud.ItemGUID = sItem.ItemGUID;
			ud.ItemIcon	= sItem.GetItemIcon();
			ud.count	= sItem.iCount;
			ud.ItemExp	= GameDataDB.ItemDB.GetData(sItem.ItemGUID).iAddExp;
			m_ExpDatas.Add(ud);
		}
		//排序
		m_ExpDatas.Sort((x,y)=>
		                {if(x.ItemExp == y.ItemExp) 
			return x.ItemGUID.CompareTo(y.ItemGUID);
			return x.ItemExp.CompareTo(y.ItemExp);
		});
	}
	//-------------------------------------------------------------------------------------------------
	//計算可升到最大限等級的經驗值
	public int CalUpMaxLevelExp()
	{
		S_PetLevelUp_Tmp sPetLV;
		int iCurrentNeededExpValue = 0;
		int RoleLevel = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel();
		if(m_PetData.iPetLevel>RoleLevel)
			return 0;
		
		int i = m_PetData.iPetLevel;
		//先計算出限制等級所需經驗值
		for(;i<RoleLevel;++i)
		{
			sPetLV = GameDataDB.PetLevelUpDB.GetData(i);
			iCurrentNeededExpValue += sPetLV.iExpRank[m_PetData.GetPetRank()-1];
		}
		if(m_PetData.iPetExp >= iCurrentNeededExpValue)
			return 0;
		else
			return (iCurrentNeededExpValue-m_PetData.iPetExp);
	}
	//-------------------------------------------------------------------------------------------------
	//塞資料到物件上
	public void AssignExpDataToItems()
	{
		if(m_ExpDatas.Count>ExpItems.Count)
			UnityDebugger.Debugger.LogError("data length"+m_ExpDatas.Count.ToString()+"long than object length"+ExpItems.Count.ToString());
		
		S_Item_Tmp sItemTmp;
		int i =0;
		foreach(uItemData id in m_ExpDatas)
		{
			sItemTmp = GameDataDB.ItemDB.GetData(id.ItemGUID);
			
			ExpItemData exdata = ExpItems[i].GetComponent<ExpItemData>();
			exdata.itemCount = id.count;
			exdata.itemGUID = id.ItemGUID;
			exdata.itemindex = i;
			exdata.itemIcon = id.ItemIcon;
			//設定經驗值量
			exdata.lbExpValue.text = sItemTmp.iAddExp.ToString()+GameDataDB.GetString(207); //?點
			//設定品階框
			sItemTmp.SetItemRarity(exdata.spriteBorder,exdata.spriteBG);
			//設定圖
			Utility.ChangeAtlasSprite(exdata.spriteItemIcon,id.ItemIcon);
			//設定數量
			exdata.lbItemCount.text = id.count.ToString();
			if(id.count ==0)
				exdata.spriteItemIcon.color = new Color(0.27f,0.27f,0.27f);
			else 
				exdata.spriteItemIcon.color = new Color(1f,1f,1f);
			++i;
		}
		if(m_ExpDatas.Count<ExpItems.Count)
		{
			for(int a=i;a<ExpItems.Count;++a)
				ExpItems[a].SetActive(false);
		}
		bRepositionItemList = true;
	}
	//-------------------------------------------------------------------------------------------------
	public void ClearSelectDatas()
	{
		for(int i=0;i<GameDefine.MAX_PET_NUTRIENT;++i)
		{
			m_NutrientItemDBID[i] = -1;
			m_ItemCount[i] = 0;
			ItemCounts[i].gameObject.SetActive(false);
			CancelBtn[i].gameObject.SetActive(false);
			Utility.ChangeAtlasSprite(ItemIcons[i],-1);
			//還原品階框
			InitSelectedBorder(sItemBGs[i],sItemBorders[i]);
		}
		m_SelectDatas.Clear();
	}
	//-------------------------------------------------------------------------------------------------
	//還原品階框
	public void InitSelectedBorder(UISprite spBG,UISprite spBorder)
	{
		Utility.ChangeAtlasSprite(spBG, 17067);
		Utility.ChangeAtlasSprite(spBorder, 17060);
	}
	//-------------------------------------------------------------------------------------------------
	//確認放上經驗值道具事件
	public void PushExpItemEvent(int index,int icon,int GUID,int count)
	{
		//擋重覆進入
		if(bExpItemPutting)
			return;
		
		bExpItemPutting = true;
		//品階框使用
		S_Item_Tmp sItemTmp;
		
		//經驗值的累加並更新
		iPlusExp 	+= GameDataDB.ItemDB.GetData(GUID).iAddExp*count;
		isUpdateExp = true;
		
		int icount=0;
		//已經有在上面的
		for(int i=0;i<GameDefine.MAX_PET_NUTRIENT;++i)
		{
			if(m_NutrientItemDBID[i] == GUID)
			{
				//紀錄
				foreach(selectData sd in m_SelectDatas)
				{
					if(sd.index == index)
					{
						sd.icount +=count;
						icount = sd.icount;
					}
				}
				if(icount == 0)
				{
					selectData newData = new selectData();
					newData.icount=count;
					newData.ListNum = i;
					newData.index = index;
					m_SelectDatas.Add(newData);
					foreach(selectData sd in m_SelectDatas)
					{
						if(sd.ListNum == i)
							icount +=sd.icount;
					}
				}
				//設定數量
				ItemCounts[i].text = icount.ToString();
				//更新底下道具列表
				UpdateExpDatas(index,(m_ExpDatas[index].count-count));
				QuitCounter();
				//Sound
				if(bAutoPut == false)
				{
					MusicControlSystem.StopOnceSound("Sound_System_005");
					MusicControlSystem.PlaySound("Sound_System_005",1);
				}
				else 
					bAutoPut = false;
				//恢復可以放狀態
				bExpItemPutting = false;
				return;
			}
		}
		//還沒有放上去過的
		for(int i=0;i<GameDefine.MAX_PET_NUTRIENT;++i)
		{
			if(m_NutrientItemDBID[i] == -1)
			{
				m_NutrientItemDBID[i]=GUID;
				//紀錄
				selectData newData = new selectData();
				newData.icount=count;
				newData.ListNum = i;
				newData.index = index;
				m_SelectDatas.Add(newData);
				//設定品階框
				sItemTmp = GameDataDB.ItemDB.GetData(GUID);
				//				sItemTmp.SetRareColor(m_uiUpgradePets.sItemBorders[i],m_uiUpgradePets.sItemBGs[i]);
				sItemTmp.SetItemRarity(sItemBorders[i],sItemBGs[i]);
				//設定圖
				Utility.ChangeAtlasSprite(ItemIcons[i],icon);
				//設定數量
				ItemCounts[i].text = count.ToString();
				ItemCounts[i].gameObject.SetActive(true);
				//更新底下道具列表
				UpdateExpDatas(index,m_ExpDatas[index].count-count);
				//顯示取消鈕
				CancelBtn[i].gameObject.SetActive(true);
				//顯示
				QuitCounter();
				//
				if(bAutoPut == false)
				{
					MusicControlSystem.StopOnceSound("Sound_System_005");
					MusicControlSystem.PlaySound("Sound_System_005",1);
				}
				else 
					bAutoPut = false;
				//恢復可以放狀態
				bExpItemPutting = false;
				return;
			}
		}
		//恢復可以放狀態
		bExpItemPutting = false;
	}
	//---------------------------------------------------------------------------------------------------------------------
	private void UpdateExpDatas(int index,int count)
	{
		m_ExpDatas[index].count = count;
		
		AssignExpDataToItems();
	}
	//-------------------------------------------------------------------------------------------------
	#endregion 生成可選擇經驗值物件與相關事件宣告
	//-------------------------------------------------------------------------------------------------
}