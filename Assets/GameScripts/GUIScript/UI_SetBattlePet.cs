using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

//(2D圖，星等，名稱，血量，攻擊力)
public enum Enum_PetInfoArray
{
	Sprite2D = 0,
	StarRank,
	Name,
	Blood,
	Attack,
}

class UI_SetBattlePet : NGUIChildGUI 
{
	public UIPanel					panelBase				= null;		//夥伴介面
	public UIPanel 					panelContent 			= null; 	//介面內容
	//
	public UILabel					lbMyBattlePet1			= null;		//夥伴隊伍文字
    public UILabel					lbMyBattlePet2			= null;		//夥伴隊伍文字
// 	public UIButton					btnPetClass				= null;		//分類
// 	public UIButton					btnPetEnhance			= null;		//強化
// 	public UIButton					btnPetGet				= null;		//夥伴召喚
	public UILabel					lbBattlePetReadyList	= null;		//待命夥伴文字
	public InfinitePetListPopulator	PetList					= null;		//待命夥伴列
	//
	public UIButton					btnPet1Set				= null;		//出戰夥伴1
	public UIButton					btnPet2Set				= null;		//出戰夥伴2
	public UISprite					spritePet1Border		= null;		//出戰夥伴1選擇框
	public UISprite					spritePet2Border		= null;		//出戰夥伴2選擇框
	public UISprite					spritePetPrefab			= null;		//初始夥伴預製物件
	public UILabel					lbLevelTitle			= null; 	//等級字串
	public UILabel					lbBloodTitle			= null;		//血量字串
	public UILabel					lbAttackTitle			= null;		//攻擊力字串
	public UILabel					lbPetInfo				= null;		//資訊字串
	[HideInInspector]
	public S_PetData				TempPet1Data			= new S_PetData();		//暫存出戰夥伴1PetData
	[HideInInspector]
	public S_PetData				TempPet2Data			= new S_PetData();		//暫存出戰夥伴2PetData
	//
	[HideInInspector]
	public int						Pet1DBFID				= 0;		//紀錄出戰夥伴1DBFID(傳Server)
	[HideInInspector]
	public int 						Pet2DBFID				= 0;		//紀錄出戰夥伴2DBFID(傳Server)

	public UIPanel					panelCheckResetFor		= null;		//確認寵物與戰陣內重複
	public UIButton					btnResetForYes			= null;		
	public UIButton					btnResetForNo			= null;
	public UILabel					lbResetFor				= null;		//確認寵物與戰陣內重複內容
	public UILabel					lbResetForYes			= null;
	public UILabel					lbResetForNo			= null;
	
	//
	[System.NonSerialized]
	public int						InitPet1DBFID			= 0;		//紀錄一開始出戰夥伴1DBFID
	[System.NonSerialized]
	public int						InitPet2DBFID			= 0;		//紀錄一開始出戰夥伴2DBFID
	//private bool					bisScroll				= false;	//可否捲動
	//private const int				iScrollMaxNum			= 4;		//捲動的條件值(要4個以上才能執行)
	[HideInInspector]
	public int						RecordUpgradePetDBID	= 0;

	[Header("FilterClass")]
	public UIButton					btnPetTypeAll						= null; //全部顯示
	public UIButton					btnPetTypeAttack					= null; //攻擊篩選
	public UIButton					btnPetTypeDefend					= null; //防禦篩選
	public UIButton					btnPetTypeAssist					= null; //輔助篩選
	public UIButton					btnPetType1							= null; //職業1
	public UIButton					btnPetType2							= null; //職業2
	public UIButton					btnPetType3							= null; //職業3
	public UIButton					btnPetType4							= null; //職業4
	public UIButton					btnPetType5							= null; //職業5
	//
	public UILabel					lbPetTypeAll						= null;
	public UILabel					lbPetTypeATK						= null;
	public UILabel					lbPetTypeDEF						= null;
	public UILabel					lbPetTypeASIS						= null;
	public UILabel					lbPetType1							= null;
	public UILabel					lbPetType2							= null;
	public UILabel					lbPetType3							= null;
	public UILabel					lbPetType4							= null;
	public UILabel					lbPetType5							= null;
	//
	public UILabel					lbCurrentCalss						= null;
	//************
	//指引教學相關元件
	//************
	//第一次組隊教學用
	public UIPanel 					PanelGuide				= null; //指引集合
	public UIButton					btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton 				btnFullScreen 			= null;
	public UISprite 				spriteGuideSelectPet 	= null;
	public UILabel 					lbGuideSelectPet 		= null;
	public UISprite 				spriteGuideSetFirstPet 	= null;
	public UILabel 					lbGuideSetFirstPet 		= null;
	public UISprite 				spriteGuideQuit 		= null;
	public UILabel 					lbGuideQuit 			= null;
	public UIButton					btnFakeQuit				= null;	//新手教學專用離開按鈕
	//培養教學用
	public UISprite 				spriteGuideClickEnhance = null;
	public UILabel 					lbGuideClickEnhance 	= null;

	//第二次組隊教學用
	public UISprite 				spriteGuideSetSecPet 	= null;
	public UILabel 					lbGuideSetSecPet 		= null;
//	public UISprite 				spriteGuideBattlePower 	= null;
//	public UILabel 					lbGuideBattlePowerExp 	= null;
//	public UISprite 				spriteGuideCanSummon 	= null;
//	public UILabel 					lbGuideCanSummonExp 	= null;

	[System.NonSerialized]
	public ENUM_CHARACTER_CALSS		m_SelectClass			= ENUM_CHARACTER_CALSS.CALSS_0;



	[HideInInspector]
	public List<S_PetData> 	m_unlockPetList = new List<S_PetData>(); 
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_SetBattlePet";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_SetBattlePet() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
        base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{

		//PetList.GetComponent<UIScrollView>().enabled = isScrollList();				//判斷是否可以捲動待命夥伴列
		//
		lbMyBattlePet1.text 									= GameDataDB.GetString(1051);			//"夥伴隊伍"
        lbMyBattlePet2.text                                     = GameDataDB.GetString(1051);			//"夥伴隊伍"
        //btnPetClass.GetComponentInChildren<UILabel>().text	= GameDataDB.GetString(1052);			//"分類"
		//btnPetEnhance.GetComponentInChildren<UILabel>().text 	= GameDataDB.GetString(1053);			//"強化"
		//btnPetGet.GetComponentInChildren<UILabel>().text		= GameDataDB.GetString(1054);			//"夥伴召喚"
		lbBattlePetReadyList.text								= GameDataDB.GetString(1055);			//"待命夥伴"
		lbPetInfo.text											= GameDataDB.GetString(1401);			//"培養"
		lbLevelTitle.text 										= GameDataDB.GetString (1056); 			//"等級"
		lbBloodTitle.text 										= GameDataDB.GetString (1057); 			//"血量"
		lbAttackTitle.text 										= GameDataDB.GetString (1013); 			//"攻擊力"
		lbResetFor.text											= GameDataDB.GetString (264); 			//
		lbResetForYes.text										= GameDataDB.GetString (277); 			//
		lbResetForNo.text										= GameDataDB.GetString (278); 			//	
		//
		panelCheckResetFor.gameObject.SetActive(false);
		InitPetSet();
		FilterPetClass(ENUM_CHARACTER_CALSS.CALSS_0);
		//
		lbPetTypeAll.text	= GameDataDB.GetString(1181);		//"全部"
		lbPetTypeATK.text	= GameDataDB.GetString(1182);		//"司戰"
		lbPetTypeDEF.text	= GameDataDB.GetString(1184);		//"持援"
		lbPetTypeASIS.text  = GameDataDB.GetString(1183);		//"利兵"
		lbPetType1.text		= GameDataDB.GetString(1185);		//"堅盾"
		lbPetType2.text		= GameDataDB.GetString(1186);		//"勅令"
		lbPetType3.text		= GameDataDB.GetString(1187);		//"懸壺"
		lbPetType4.text		= GameDataDB.GetString(1188);		//"伐謀"
		lbPetType5.text		= GameDataDB.GetString(1189);		//"蕩決"
		//
		lbCurrentCalss.text	= GameDataDB.GetString(1181);
	}
	//-----------------------------------------------------------------------------------------------------
	//判斷之前有無夥伴設定
	public void InitPetSet()
	{
		//先判斷是否有設定過出戰夥伴
		int[] iRolePetData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BattlePetIDs;
		for(int i=0; i<iRolePetData.Length; ++i)
		{
			switch(i)
			{
			case (int)ENUM_PET_USE.PET_1:
				Pet1DBFID 		= iRolePetData[i];
				InitPet1DBFID 	= iRolePetData[i];
				break;
			case (int)ENUM_PET_USE.PET_2:
				Pet2DBFID 		= iRolePetData[i];
				InitPet2DBFID	= iRolePetData[i];
				break;
			}
		}
		//有設出戰夥伴的狀態時
		if(Pet1DBFID!=-1 || Pet2DBFID!=-1)
		{
			foreach(S_PetData pd in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData)
			{
				if(Pet1DBFID!=0 && pd.iPetDBFID == Pet1DBFID)
				{
					SetInitBattlePet(btnPet1Set,pd);
					TempPet1Data = pd;
				}
				if(Pet2DBFID!=0 && pd.iPetDBFID == Pet2DBFID)
				{
					SetInitBattlePet(btnPet2Set,pd);	
					TempPet2Data = pd;
				}
			}// End foreach
		}// End Pet1DBFID!=-1 || Pet2DBFID!=-1
		spritePet1Border.gameObject.SetActive (false);	//初始取消選擇框
		spritePet2Border.gameObject.SetActive (false);	//初始取消選擇框
	}//End InitPetSet
	//-----------------------------------------------------------------------------------------------------
	//未出戰解鎖夥伴收集列表
	public void FilterPetClass(ENUM_CHARACTER_CALSS chClass)
	{
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetListSize()==0)
			return;
		
		m_unlockPetList.Clear();
		foreach(S_PetData pd in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData)
		{
			S_PetData_Tmp spd = GameDataDB.PetDB.GetData(pd.iPetDBFID);
			
			//如果有篩選
			if(chClass != ENUM_CHARACTER_CALSS.CALSS_0)
			{
				if(spd.emCharClass != chClass)
					continue;
			}
			
			if(Pet1DBFID == pd.iPetDBFID)
				continue;
			else if(Pet2DBFID == pd.iPetDBFID)
				continue;
			else if(pd.iPetLevel == GameDefine.PET_PIECE_LEVEL)
				continue;
			else
				m_unlockPetList.Add(pd);
		}
		m_unlockPetList.Sort((x, y) => { return -x.GetPetRank().CompareTo(y.GetPetRank()); });
		PetList.UnLockPets = m_unlockPetList;
		PetList.StartDemo();
		//取消選擇
		if(PetList.TempPetSet != null)
		{
			DestroyImmediate(PetList.TempPetSet.gameObject);
			PetList.TempPetSet 		= null;
			PetList.TempDataIndex	= -1;
		}
		//
		m_SelectClass = chClass;
	}
	//-----------------------------------------------------------------------------------------------------


	private void SetInitBattlePet(UIButton btn,S_PetData pd)
	{
		//把空背景圖關掉
		Transform t = btn.transform.FindChild("Background");
		//t.localScale = new Vector3(0,0,0);
		Transform p = null;
		if(btn.name == "Button(Pet1Set)")
			p = btn.transform.FindChild("Pet1Selected");
		else if(btn.name == "Button(Pet2Set)")
			p = btn.transform.FindChild("Pet2Selected");
		else
			UnityDebugger.Debugger.LogError("AssignWrongButton");

		if(p!=null)
			Destroy(p.gameObject);
		//產生夥伴物件
		GameObject g 				= Instantiate(spritePetPrefab.gameObject) as GameObject;
		g.transform.parent 			= btn.transform;
		g.transform.localPosition 	= t.transform.localPosition;
		if(btn.name == "Button(Pet1Set)")
			g.name = "Pet1Selected";
		else if(btn.name == "Button(Pet2Set)")
			g.name = "Pet2Selected";
		g.GetComponent<BoxCollider>().enabled = false;	
		//載入相關資訊
		PreLoadSetPetData(g,pd);
		g.SetActive(true);
		//
		spritePetPrefab.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	//初始進入介面時設定已選擇的夥伴資訊
	void PreLoadSetPetData(GameObject g,S_PetData pd)
	{
		InfinitePetItemBehavior PetInfo = g.GetComponent<InfinitePetItemBehavior>();
		PetInfo.isSetBattle = true;
		//蒐集應顯示資訊
		int[] PetInfoSet = new int[5];
		PetInfoSet = CollectEachUnlockPetInfo(pd);
		//寵物圖像更換
		UISprite		PetIcon = g.GetComponent<UISprite>();
		Utility.ChangeAtlasSprite(PetIcon,PetInfoSet[(int)Enum_PetInfoArray.Sprite2D]);
		//資訊更換
		PetInfo.isSetBattle			= true;
		PetInfo.lbPetName.text 		= GameDataDB.GetString((int)PetInfoSet[(int)Enum_PetInfoArray.Name]);
		S_PetData_Tmp petTmp = GameDataDB.PetDB.GetData(pd.iPetDBFID); 
		if (petTmp != null)
			petTmp.SetRareColorString(PetInfo.lbPetName,true);
		PetInfo.lbPetLevel.text 		= pd.iPetLevel.ToString();
		int PetCalssIconID = ARPGApplication.instance.GetPetCalssIconID(petTmp.emCharClass);
		Utility.ChangeAtlasSprite(PetInfo.spPetCalss,PetCalssIconID);
		PetInfo.lbTypeName.text = GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(petTmp.emCharType));
		//計算數值
		S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,pd.iPetDBFID);
		S_ItemData[] AllEQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems();
		S_FormationData formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
		List<S_PetData> sPet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData;
		S_RoleAttr PetAttrValue = ARPGCharacter.CreatePetRoleAttr(pd,EQList,sPet,pd.GetTalentSkill(),null,formationData,AllEQList);
		int iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fMaxHP,0,MidpointRounding.AwayFromZero));
		PetInfo.lbPetBlood.text		= iAtrValue.ToString();
		iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fAttack,0,MidpointRounding.AwayFromZero));
		PetInfo.lbPetAttack.text		= iAtrValue.ToString();
		//
		PetInfo.iDBFID				= pd.iPetDBFID;
		//設定星等產生數,突破等級與資訊框選擇
		UISprite[] spriteRankStars 	= PetInfo.spriteRankStars;
		UISprite spritePetInfoBG 	= PetInfo.spritePetInfoBG;
		UISprite spritePetBorder	= PetInfo.spritePetBorder;
		//設定星等
		for(int i=0;i<spriteRankStars.Length;++i)
		{
			if(i<pd.iPetLimitLevel)
				spriteRankStars[i].gameObject.SetActive(true);
			else
				spriteRankStars[i].gameObject.SetActive(false);
		}
		int RealRank = ARPGApplication.instance.GetPetRealRankWithLimitBreak(pd.iPetDBFID);

		//選擇使用哪個資訊框
		if(RealRank>0 && RealRank<4)
		{
			Utility.ChangeAtlasSprite(spritePetInfoBG,1010);	//夥伴普卡1~3星用資訊框
			Utility.ChangeAtlasSprite(spritePetBorder,1006);	//夥伴普卡1~3星用純框
		}
		if(RealRank>3 && RealRank<6)
		{
			Utility.ChangeAtlasSprite(spritePetInfoBG,1009);	//夥伴銀卡4~5星用資訊框
			Utility.ChangeAtlasSprite(spritePetBorder,1005);	//夥伴普卡1~3星用純框
		}
		if(RealRank>5 && RealRank<=7)
		{
			Utility.ChangeAtlasSprite(spritePetInfoBG,1008);	//夥伴金卡6~7星用資訊框
			Utility.ChangeAtlasSprite(spritePetBorder,1004);	//夥伴普卡1~3星用純框
		}
		if(RealRank<0 || RealRank>7)
			return;												//非正常狀況時
		//隱藏選擇框
		PetInfo.spriteBorder.gameObject.SetActive(false);
		//載入是否需要顯示tip
		bool bTipSwith = ARPGApplication.instance.m_SLPetNotifySystem.GetThePetUpTipStatus(pd.iPetDBFID); 
		PetInfo.spriteTip.gameObject.SetActive(bTipSwith);
	}
	//-------------------------------------------------------------------------------------------------------------
	//寵物資訊蒐集(2D圖，星等，名稱，血量，攻擊力)
	public int[] CollectEachUnlockPetInfo(S_PetData pd)
	{
		if (pd == null)
			return null;
		int[] InfoSet = new int[5]; 
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(pd.iPetDBFID);
		InfoSet[0]= pdTmp.Texture;		//2D圖
		InfoSet[1]= pdTmp.iRank;		//星等
		InfoSet[2]= pdTmp.iName;			//名稱
		int PlusHP = (int)pdTmp.fMaxHP_UP*(pd.iPetLevel-1);
		InfoSet[3]= pdTmp.sAttrTable.iMaxHP + PlusHP;			//血量
		int PlusATK = (int)pdTmp.fAttack_UP*(pd.iPetLevel-1);
		InfoSet[4]= pdTmp.sAttrTable.iAttack + PlusATK;			//攻擊力
		return InfoSet;
	}
	//-----------------------------------------------------------------------------------------------------
	//選擇夥伴1的事件
	public void IsSelectedMyPet1()
	{
		//Group Pet1 Pet2 to the same Set
		spritePet2Border.gameObject.SetActive (false);
		spritePet1Border.gameObject.SetActive(!spritePet1Border.gameObject.activeSelf);
		//相互選擇出戰指定事件
		if(spritePet1Border.gameObject.activeSelf == true)
		{
			CancelInvoke("Pet2SetBehvaior");						//取消另一個Invoke事件監聽
			InvokeRepeating("Pet1SetBehavior",0.0f,0.1f);			//啟動監聽事件
		}
		else if (spritePet1Border.gameObject.activeSelf == false)
		{
			CancelInvoke("Pet1SetBehavior");						//取消Invoke事件監聽
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//選擇夥伴2的事件
	public void IsSelectedMyPet2()
	{
		//Group Pet1 Pet2 to the same Set
		spritePet1Border.gameObject.SetActive (false);
		spritePet2Border.gameObject.SetActive(!spritePet2Border.gameObject.activeSelf);
		//相互選擇出戰指定事件
		if(spritePet2Border.gameObject.activeSelf == true)
		{
			CancelInvoke("Pet1SetBehavior");						//取消另一個Invoke事件監聽
			InvokeRepeating("Pet2SetBehvaior",0.0f,0.1f);			//啟動監聽事件
		}
		else if (spritePet2Border.gameObject.activeSelf == false)
		{
			CancelInvoke("Pet2SetBehvaior");						//取消Invoke事件監聽
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void OpenPetConfirmBoxEvent(ENUM_PET_USE pUse)
	{
		//開啟確認UI
		int unSelectPetID = 0;
		S_PetData pd = PetList.dataList[PetList.TempDataIndex] as S_PetData;
		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(pd.iPetDBFID);
		string selectPetName = pdTmp.GetNameWithColor();
		string strContent = string.Format(GameDataDB.GetString(983),selectPetName); //是否替換為?
		switch(pUse)
		{
		case ENUM_PET_USE.PET_1:
			if(TempPet1Data != null)
				unSelectPetID = TempPet1Data.iPetDBFID;
			break;
		case ENUM_PET_USE.PET_2:
			if(TempPet2Data != null)
				unSelectPetID = TempPet2Data.iPetDBFID;
			break;
		}

		ARPGApplication.instance.PushPetsConfirmBox(strContent,unSelectPetID,pd.iPetDBFID,ConfirmEvent,CancelEvent,true,GameDataDB.GetString(5028),true);
	}
	//-----------------------------------------------------------------------------------------------------
	void Pet1SetBehavior()
	{
		if(PetList.TempPetSet!=null)
		{
			if(TempPet1Data.iPetDBFID<=0)
			{
				Pet1SetProcess();
			}
			else
			{
				OpenPetConfirmBoxEvent(ENUM_PET_USE.PET_1);
			}
			//取消Invoke事件監聽
			CancelInvoke("Pet1SetBehavior");
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void Pet1SetProcess()
	{
		//把空背景圖關掉
		//Transform t = btnPet1Set.transform.FindChild("Background");
		//t.localScale = new Vector3(0.0f,0.0f,0.0f);
		
		Transform t=btnPet1Set.transform.FindChild("Pet1Selected");
		if(t!=null)
		{
			m_unlockPetList.Add(TempPet1Data);
			Destroy(t.gameObject);
		}
		t = Instantiate(PetList.TempPetSet) as Transform;
		t.parent = btnPet1Set.transform;
		t.localPosition = btnPet1Set.GetComponentInChildren<UISprite>().gameObject.transform.localPosition;
		t.localRotation = btnPet1Set.GetComponentInChildren<UISprite>().gameObject.transform.localRotation;
		t.localScale = btnPet1Set.GetComponentInChildren<UISprite>().gameObject.transform.localScale;
		t.name = "Pet1Selected";
		t.GetComponent<InfinitePetItemBehavior>().isSetBattle = true;
		t.GetComponent<BoxCollider>().enabled = false;
		
		//暫存現有指定夥伴狀態
		TempPet1Data = PetList.dataList[PetList.TempDataIndex] as S_PetData;
		Pet1DBFID = TempPet1Data.iPetDBFID;
		//關掉選擇框並清除選擇跟PetList中的Temp
		t.gameObject.SetActive(true);
		UpdateBattlePetData(t,TempPet1Data);														//更新出戰寵物資料
		spritePet1Border.gameObject.SetActive(false);												//取消出戰指定的選擇框
		m_unlockPetList.Remove(TempPet1Data);														//刪除UnlockList中的夥伴資料
		DestroyImmediate(PetList.TempPetSet.gameObject);											//先清掉暫存物件
		PetList.TempPetSet = null;																	//再清掉待命夥伴中的暫存
		//PetList.GetComponent<UIScrollView>().enabled = isScrollList();								//判斷是否可以捲動
		//重新載入待命夥伴
		FilterPetClass(m_SelectClass);
	}
	//-----------------------------------------------------------------------------------------------------
	void Pet2SetBehvaior()
	{
		if(PetList.TempPetSet!=null)
		{
			if(TempPet2Data.iPetDBFID <=0)
			{
				Pet2SetProcess();
			}
			else
			{
				OpenPetConfirmBoxEvent(ENUM_PET_USE.PET_2);
			}
			//取消Invoke事件監聽
			CancelInvoke("Pet2SetBehvaior");
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void Pet2SetProcess()
	{
		//把空背景圖關掉
		Transform t = btnPet2Set.transform.FindChild("Background");
		//t.localScale = new Vector3(0.0f,0.0f,0.0f);
		
		t=btnPet2Set.transform.FindChild("Pet2Selected");
		if(t!=null)			
		{
			//有圖的情況
			m_unlockPetList.Add(TempPet2Data);
			Destroy(t.gameObject);
		}
		t = Instantiate(PetList.TempPetSet) as Transform;
		t.parent = btnPet2Set.transform;
		t.localPosition = btnPet2Set.GetComponentInChildren<UISprite>().gameObject.transform.localPosition;
		t.localRotation = btnPet2Set.GetComponentInChildren<UISprite>().gameObject.transform.localRotation;
		t.localScale = btnPet2Set.GetComponentInChildren<UISprite>().gameObject.transform.localScale;
		t.name = "Pet2Selected";
		t.GetComponent<InfinitePetItemBehavior>().isSetBattle = true;
		t.GetComponent<BoxCollider>().enabled = false;
		
		//暫存現有指定夥伴狀態
		TempPet2Data = PetList.dataList[PetList.TempDataIndex] as S_PetData;
		Pet2DBFID = TempPet2Data.iPetDBFID;
		//關掉選擇框並清除選擇跟PetList中的Temp
		t.gameObject.SetActive(true);
		UpdateBattlePetData(t,TempPet2Data);														//更新出戰寵物資料
		spritePet2Border.gameObject.SetActive(false);												//取消出戰指定的選擇框
		m_unlockPetList.Remove(TempPet2Data);														//刪除UnlockList中的夥伴資料
		DestroyImmediate(PetList.TempPetSet.gameObject);											//先清掉暫存物件
		PetList.TempPetSet = null;																	//再清掉待命夥伴中的暫存
		//PetList.GetComponent<UIScrollView>().enabled = isScrollList();								//判斷是否可以捲動
		//重新載入待命夥伴
		FilterPetClass(m_SelectClass);
	}
	//-----------------------------------------------------------------------------------------------------
	private void ConfirmEvent(int SourcePetGUID, int TargetPetGUID, bool ToggleValue)
	{
		if(spritePet1Border.gameObject.activeSelf == true)
		{
			if(ToggleValue)
				JsonSlot_Item.Send_CtoM_ExchangePetEq(SourcePetGUID,TargetPetGUID);
			else
				Pet1SetProcess();
		}
		else if(spritePet2Border.gameObject.activeSelf == true)
		{
			if(ToggleValue)
				JsonSlot_Item.Send_CtoM_ExchangePetEq(SourcePetGUID,TargetPetGUID);
			else
				Pet2SetProcess();
		}

	}
	//-----------------------------------------------------------------------------------------------------
	public void UpdatePetSwitch()
	{
		if(spritePet1Border.gameObject.activeSelf == true)
		{
			Pet1SetProcess();
		}
		else if(spritePet2Border.gameObject.activeSelf == true)
		{
			Pet2SetProcess();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void CancelEvent(int SourcePetGUID, int TargetPetGUID, bool ToggleValue)
	{
		InfinitePetItemBehavior SelectPet = PetList.TempPetSet.GetComponent<InfinitePetItemBehavior>();
		Transform t = PetList.itemsPool[SelectPet.itemNumber];
		InfinitePetItemBehavior petBehavior = t.GetComponent<InfinitePetItemBehavior>();
		petBehavior.spriteBorder.gameObject.SetActive(false);
		spritePet1Border.gameObject.SetActive(false);
		spritePet2Border.gameObject.SetActive(false);
		DestroyImmediate(PetList.TempPetSet.gameObject);
		PetList.TempPetSet = null;
		PetList.TempDataIndex = -1;
	}
	//-----------------------------------------------------------------------------------------------------
	private void UpdateBattlePetData(Transform t,S_PetData pd)
	{
		InfinitePetItemBehavior petBehavior = t.GetComponent<InfinitePetItemBehavior>();
		if(petBehavior != null)
		{
			petBehavior.spriteBorder.enabled = false;
			//計算數值
			S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,pd.iPetDBFID);
			S_ItemData[] AllEQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems();
			S_FormationData formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
			List<S_PetData> sPet = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData;
			S_RoleAttr PetAttrValue = ARPGCharacter.CreatePetRoleAttr(pd,EQList,sPet,pd.GetTalentSkill(),null,formationData,AllEQList);
			int iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fMaxHP,0,MidpointRounding.AwayFromZero));
			petBehavior.lbPetBlood.text		= iAtrValue.ToString();
			iAtrValue = (int)(Math.Round(PetAttrValue.sBattleFinial.fAttack,0,MidpointRounding.AwayFromZero));
			petBehavior.lbPetAttack.text		= iAtrValue.ToString();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	/*public bool isScrollList()
	{
		//當PetData裡的數量小於等於條件值
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetListSize()<= iScrollMaxNum)
			return false;
		else
			return true;
	}*/
	//-----------------------------------------------------------------------------------------------------
	//重新更新寵物資料
	public void ReNewPetsList()
	{
		//取消出戰寵物選擇框
		spritePet1Border.gameObject.SetActive(false);
		spritePet2Border.gameObject.SetActive(false);
		//取消Invoke事件監聽
		CancelInvoke("Pet1SetBehavior");
		CancelInvoke("Pet2SetBehvaior");
		//初始化
		PetList.TempPetSet = null;
		PetList.TempDataIndex = -1;
		//InitPetSet();
		S_PetData pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(RecordUpgradePetDBID);
		if(RecordUpgradePetDBID == TempPet1Data.iPetDBFID)
			SetInitBattlePet(btnPet1Set,pd);
		else if(RecordUpgradePetDBID == TempPet2Data.iPetDBFID)
			SetInitBattlePet(btnPet2Set,pd);
		else
		{
			int i=0;
			foreach(S_PetData petdata in m_unlockPetList)
			{
				if(petdata.iPetDBFID == RecordUpgradePetDBID)
				{
					m_unlockPetList[i] = pd;
					break;
				}
				++i;
			}
			//m_unlockPetList.Sort((x, y) => { return x.iPetDBFID.CompareTo(y.iPetDBFID); });
			m_unlockPetList.Sort((x, y) => { return -x.GetPetRank().CompareTo(y.GetPetRank()); });
			PetList.UnLockPets = m_unlockPetList;
		}
		//重新載入待命夥伴
		PetList.StartDemo();
	}
	//-----------------------------------------------------------------------------------------------------	
}

