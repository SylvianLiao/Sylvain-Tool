using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;
using System.Collections;

public class UI_EnhancePets : NGUIChildGUI 
{
// 	public UIPanel					panelBase				= null;		//夥伴強化介面
// 	//SelectedPetSet
// 	public UIPanel 	panelSelectedPetSet	 	= null; //已選定強化角色的相關集合
// 	public UIButton	btnSelectedPet			= null; //選擇角色按鈕
// 	public UILabel	lbPleaseSelectPet		= null; //選擇角色字樣
// 	//
// 	public UIPanel	panelPetInfo			= null; //資訊框相關
// 	public UISprite	spriteEnhancePet		= null; //角色圖
// 	public UILabel 	lbPetName				= null;	//角色名稱
// 	public UILabel 	lbLevelTilte			= null; //等級標題
// 	public UILabel 	lbPetLevel				= null; //等級數值
// 	public UILabel 	lbBloodTitle			= null; //生命標題
// 	public UILabel 	lbPetBlood				= null; //生命數值
// 	public UILabel 	lbAttackTitle			= null; //攻擊力標題
// 	public UILabel 	lbPetAttack				= null; //攻擊力數值
// 	public UISprite spritePetInfoBG			= null; //資訊框架
// 	public UILabel	lbPetInfo				= null; //資訊框文字(Info按鈕)
// 	public UIButton	btnInfo					= null; //資訊按鈕
// 	public UISprite	spriteRoleLevelUpTip	= null; //角色本身升級特效提示
// 	//
// 	public UIPanel	panelPreviewPart		= null; //預覽群
// 	public UILabel	lbPreviewUPLV			= null; //預覽等級
// 	public UILabel	lbPreviewUPHP			= null; //預覽血量
// 	public UILabel	lbPreviewUPATK			= null; //預覽攻擊力
// 	//
// 	public UIPanel	panelPreviewLimitLV		= null; //預覽突破
// 	public UILabel	lbRemainTimes			= null; //剩餘突破數
// 	//
// 	public UISprite[] spritePreLimitLV	= new UISprite[4];
// 	public UISprite[] spriteLimitLVRank = new UISprite[4];
// 	public UISprite[] spriteStarRankSet = new UISprite[7];
// 	[HideInInspector]public S_PetData EnhancePetData		= null; 
// 	//-----------------------------------------------------------------------
// 	//SelectedMaterialSet
// 	public UIPanel 	panelSelectedMaterialSet	= null; //已選擇強化素材相關集合
// 	public UILabel	lbSelected					= null; //已選材料字樣
// 	//
// 	public UILabel	lbExp						= null; //經驗值文字顯示
// 	public UISprite	spriteExpValue				= null; //經驗值進度條
// 	public UILabel	lbMoneyValue				= null; //強化所需費用
// 	public UISprite	spriteLevelUpTip			= null; //升級特效提示
// 	[HideInInspector]public int		iEnhanceCost 	=0;		//紀錄強化所需費用
// 	[HideInInspector]public int		iExp			=0;		//紀錄現有經驗值
// 	[HideInInspector]public int 	iPlusExp		=0;		//素材所加上的經驗值
// 	[HideInInspector]public int 	iMaxExp						=0;		//紀錄下一次升級經驗值
// 	[HideInInspector]public bool 	isUpdateExp					= false;
// 	//
// 	public UIButton[]	btnSelectedMaterialSet	= new UIButton[5];
// 	public UISprite[] 	spriteMatPetIconSet		= new UISprite[5];
// 	public UILabel[]	lbStartRankNumSet		= new UILabel[5];
// 	public UISprite[]	spriteSelectEffect		= new UISprite[5];
// 	//-----------------------------------------------------------------------
// 	//CanUseMaterialsBG
// 	public UISprite CanUseMaterialSet			= null; //備選材料集合
// 	public UIPanel	panelMaterialList			= null;	//備選材料列表
// 	public UILabel	lbPrepareList				= null; //備選材料字樣
// 	public ColListPopulator scrollCanUsePetList = null; //scroll列中可用素材
// 	//
// 	//-----------------------------------------------------------------------
// 	//SelectPetForStrength
// 	public UIPanel	panelSelectPetForStrength	= null; //選擇要強化的寵物相關集合
// 	//
// 	public UILabel lbMyPets						= null; //我的隊伍字樣
// 	public UILabel lbReadyPets					= null; //待命夥伴字樣
// 
// 	public UISprite	spritePet1Blank				= null; //空戰寵1
// 	public UIPanel panelBP1PetInfo				= null; //戰寵1的顯示
// 	public UISprite	spriteBattlePet1			= null; //戰寵1角色圖
// 	public UILabel 	lbBP1Name					= null;	//角色名稱
// 	public UILabel 	lbBP1LevelTilte				= null; //等級標題
// 	public UILabel 	lbBP1PetLevel				= null; //等級數值
// 	public UILabel 	lbBP1BloodTitle				= null; //生命標題
// 	public UILabel 	lbBP1PetBlood				= null; //生命數值
// 	public UILabel 	lbBP1AttackTitle			= null; //攻擊力標題
// 	public UILabel 	lbBP1PetAttack				= null; //攻擊力數值
// 	public UISprite spriteBP1PetInfoBG			= null; //選擇框架
// 	public UILabel	lbBP1Info					= null; //選擇框文字
// 	public UIButton	btnBP1Info					= null; //戰寵1資訊
// 	public UIButton	btnBP1Select				= null; //選擇戰寵1
// 	//
// 	public UISprite	spritePet2Blank				= null; //空戰寵2
// 	public UIPanel panelBP2PetInfo				= null; //戰寵2的顯示
// 	public UISprite	spriteBattlePet2			= null; //戰寵1角色圖
// 	public UILabel 	lbBP2Name					= null;	//角色名稱
// 	public UILabel 	lbBP2LevelTilte				= null; //等級標題
// 	public UILabel 	lbBP2PetLevel				= null; //等級數值
// 	public UILabel 	lbBP2BloodTitle				= null; //生命標題
// 	public UILabel 	lbBP2PetBlood				= null; //生命數值
// 	public UILabel 	lbBP2AttackTitle			= null; //攻擊力標題
// 	public UILabel 	lbBP2PetAttack				= null; //攻擊力數值
// 	public UISprite spriteBP2PetInfoBG			= null; //選擇框架
// 	public UILabel	lbBP2Info					= null; //選擇框文字
// 	public UIButton	btnBP2Info					= null; //戰寵2資訊
// 	public UIButton	btnBP2Select				= null; //選擇戰寵2
// 	//
// 	public InfinitePetListPopulator1	scrollPetList		= null;		//待命夥伴列
// 	public UIPanel	panelReadyPetList			= null; //待命夥伴panel
// 	public UILabel	lbReadyPetSelect			= null; //待命夥伴選擇字樣
// 	public UILabel 	lbRPLevelTilte				= null; //待命夥伴等級標題
// 	public UILabel 	lbRPBloodTitle				= null; //待命夥伴生命標題
// 	public UILabel 	lbRPAttackTitle				= null; //待命夥伴攻擊力標題
// 	//
// 	public UIButton btnCancel					= null;	//關閉選擇頁
// 	//
// 	[HideInInspector]public S_PetData BP1Petdata					= null; 
// 	[HideInInspector]public S_PetData BP2Petdata					= null;
// 	public UISprite[] spriteBP1LimitLVRank = new UISprite[4];
// 	public UISprite[] spriteBP2LimitLVRank = new UISprite[4];
// 	public UISprite[] spriteBP1StarRankSet = new UISprite[7];
// 	public UISprite[] spriteBP2StarRankSet = new UISprite[7];
// 	[HideInInspector]public string	commonSelectStr ="";		//選擇頁共用的選擇字樣
// 	//-----------------------------------------------------------------------
// 	[HideInInspector]
// 	public UI_TopStateView		uiTopStateView				= null;		//資訊列
// 	[HideInInspector]
// 	public UI_RolesDetailInfo	uiRolesDetailInfo			= null;		//寵物詳細資訊框
// 	//-----------------------------------------------------------------------
// 	public GameObject	gBtnContainer				= null; //按鈕集合
// 	public UIButton	btnEnhancePet				= null; //強化按鈕
// 	public UIButton btnAutoPutMaterial			= null; //自動裝滿強化素材
// 	public UIButton btnAutoUpGrade				= null; //自動升級
// 	public UILabel	lbAutoPutMaterial			= null; //自動裝滿強化素材字樣
// 	public UILabel	lbAutoUpGrade				= null; //自動升級字樣
// 	public UILabel	lbEnhancePet				= null; //強化字樣
// 
// 	//-----------------------------------------------------------------------
// 	//暫代
// 
// 	public UISprite	spriteBigEnhance				= null;//大成功效果
// 	public UISprite spriteUpLimitLV					= null;//突破效果
// 	public UIPanel	panelAutoGrading				= null;//自動升級中畫面
// 
// 	//-----------------------------------------------------------------------
// 	//教學導引相關元件
// 	public UIPanel 	panelGuide						= null;	//導引相關集合
// 	public UISprite spriteBlackBackGround			= null; //黑幕
// 	public UIButton	btnFullScreen					= null; //任意鍵繼續說明
// 	public UILabel	lbGuideSelectRole				= null; //進入選擇強化角色導引
// 	public UISprite	spriteGuideRoleSelected_R		= null; //靠右選擇強化角色導引
// 	public UILabel	lbExplanation_R				= null;	//選擇強化角色導引文字
// 	public UISprite	spriteGuideSelectMaterial	= null; //靠左選擇強化角色導引
// 	public UILabel	lbGuideSelectMaterial			= null;	//選擇強化角色導引文字
// 	//public UILabel	lbPListExplanation				= null; //備選材料說明導引
// 	//public UILabel	lbPreviewResult				= null; //預覽煉化結果導引
// 	//public UISprite	spriteGuideAutoSelect				= null; //自動選擇導引
// 	//public UILabel	lbGuideAutoSelect				= null; 
// 	public UISprite	spriteGuideEnhancePet			= null; //強化導引導引
// 	public UILabel	lbGuideEnhancePet				= null; 
// 	public UISprite	spriteGuidePowerUp					= null;	//戰力提升導引
// 	public UILabel	lbGuidePowerUp					= null;	
// 	public UISprite	spriteGuideQuitEnhance			= null; //結束煉化指引
// 	public UILabel	lbExplanationQuitEnhance		= null; //結束煉化指引文字
	//-----------------------------------------------------------------------
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_EnhancePets";
	
	//-------------------------------------------------------------------------------------------------------------
	private UI_EnhancePets() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	public override void Show()
// 	{
// 		base.Show();
// 		
// 		//重新設定Depath
// 		uiTopStateView.panelBase.depth = panelBase.depth + 800;
// 		uiTopStateView.Show();
// 
// 		if(uiRolesDetailInfo != null)
// 		{
// 			//重新設定Depath
// 			uiRolesDetailInfo.panelBase.depth = panelBase.depth + 900;
// 			uiRolesDetailInfo.Show();
// 		}
// 		uiRolesDetailInfo.panelBase.gameObject.SetActive(false);
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	public override void Hide()
// 	{
// 		uiTopStateView.Hide();
// 		//
// 		if(uiRolesDetailInfo!=null)
// 			uiRolesDetailInfo.Hide();
// 
// 		base.Hide();
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	private void Start()
// 	{
// 		panelPreviewPart.gameObject.SetActive(false);			//隱藏預覽群
// 		panelPreviewLimitLV.gameObject.SetActive(false);		//隱藏突破
// 		panelSelectPetForStrength.gameObject.SetActive(false); 	//隱藏要強化的寵物介面"選擇頁"
// 		panelPetInfo.gameObject.SetActive(false);				//隱藏顯示已選寵物資訊
// 		spriteBigEnhance.gameObject.SetActive(false);
// 		spriteUpLimitLV.gameObject.SetActive(false);
// 		panelAutoGrading.gameObject.SetActive(false);
// 		InitSelectedMaterialList();
// 		//
// 		//設定選擇戰寵1btn事件
// 		//UIEventListener.Get(btnBP1Select.gameObject).onClick 	+= SetBattlePetToEnhance;
// 		//設定選擇戰寵2btn事件
// 		//UIEventListener.Get(btnBP2Select.gameObject).onClick 	+= SetBattlePetToEnhance;
// 		spriteExpValue.fillAmount = 0;
// 		//字串指派-------------------------------------------------------------------------------------------------------------
// 		//強化頁
// 		lbPleaseSelectPet.text 	= GameDataDB.GetString(1406);			//"請選擇強化夥伴"
// 		lbLevelTilte.text 		= GameDataDB.GetString(1056);			//"等級"
// 		lbBloodTitle.text 		= GameDataDB.GetString(1057);			//"血量"
// 		lbAttackTitle.text		= GameDataDB.GetString(1013);			//"攻擊力"
// 		lbPetInfo.text			= GameDataDB.GetString(1405);			//"資訊"
// 		lbSelected.text 		= GameDataDB.GetString(1403);			//"已選材料"
// 		lbExp.text 				= "0/0";
// 		lbMoneyValue.text		= "0";
// 		lbPrepareList.text		= GameDataDB.GetString(1404);			//"備選材料"
// 		lbEnhancePet.text		= GameDataDB.GetString(1401);			//"強化"
// 		lbAutoUpGrade.text		= GameDataDB.GetString(1402);			//"自動升級"
// 		lbAutoPutMaterial.text 	= GameDataDB.GetString(1407);			//"自動選擇"
// 		//選擇頁
// 		lbMyPets.text			= GameDataDB.GetString(1051);			//"夥伴隊伍"
// 		lbReadyPets.text		= GameDataDB.GetString(1055);			//"待命夥伴"
// 		lbBP1LevelTilte.text 	= GameDataDB.GetString(1056);			//"等級"
// 		lbBP1BloodTitle.text 	= GameDataDB.GetString(1057);			//"血量"
// 		lbBP1AttackTitle.text	= GameDataDB.GetString(1013);			//"攻擊力"
// 		lbBP2LevelTilte.text 	= GameDataDB.GetString(1056);			//"等級"
// 		lbBP2BloodTitle.text 	= GameDataDB.GetString(1057);			//"血量"
// 		lbBP2AttackTitle.text	= GameDataDB.GetString(1013);			//"攻擊力"
// 		commonSelectStr 		= GameDataDB.GetString(1405);			//"資訊"
// 		lbBP1Info.text			= commonSelectStr;
// 		lbBP2Info.text			= commonSelectStr;
// 		lbReadyPetSelect.text	= commonSelectStr;
// 		lbRPLevelTilte.text 	= GameDataDB.GetString(1056);			//"等級"
// 		lbRPBloodTitle.text 	= GameDataDB.GetString(1057);			//"血量"
// 		lbRPAttackTitle.text	= GameDataDB.GetString(1013);			//"攻擊力"
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	private void Update()
// 	{
// 		if(isUpdateExp && iMaxExp!=0)
// 		{	
// 			isUpdateExp = false;
// 			int iFinalHP = iExp+iPlusExp;
// 			float fProcess;
// 			if(iFinalHP>iMaxExp)
// 				fProcess=1;
// 			else
// 				fProcess = (float)iFinalHP / (float)iMaxExp;
// 
// 				spriteExpValue.fillAmount = fProcess;
// 				lbExp.text = iFinalHP.ToString()+"/"+iMaxExp.ToString();
// 				lbMoneyValue.text = iEnhanceCost.ToString();
// 			//啟用預覽
// 			PreviewAfterUpGrade();
// 			PreviewLimitLevel();
// 		}
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	public void BigEnhanceEffect()
// 	{
// 		spriteBigEnhance.gameObject.SetActive(true);
// 		MusicControlSystem.PlaySound("Sound_System_014",1);
// 		FadeOut FadeoutEffect =  spriteBigEnhance.gameObject.AddComponent<FadeOut>();
// 		FadeoutEffect.duration = 2.0f;
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	public void UpLimitLVEffect()
// 	{
// 		spriteUpLimitLV.gameObject.SetActive(true);
// 		MusicControlSystem.PlaySound("Sound_System_014",1);
// 		FadeOut FadeoutEffect =  spriteUpLimitLV.gameObject.AddComponent<FadeOut>();
// 		FadeoutEffect.duration = 2.0f;
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	public void InitSelectedMaterialList()
// 	{
// 		for(int i=0;i<spriteMatPetIconSet.Length;++i)
// 			spriteMatPetIconSet[i].gameObject.SetActive(false);
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//(已選擇的材料按鈕UI事件)到state再指派
// 	public void SetbtnSelectedMaterial(GameObject gb)
// 	{
// 		for(int i=0;i<spriteMatPetIconSet.Length;++i)
// 		{
// 			if(gb==btnSelectedMaterialSet[i].gameObject)
// 			{
// 				if(spriteMatPetIconSet[i].gameObject.activeSelf == true)
// 					MusicControlSystem.PlaySound("Sound_System_005",1);
// 				spriteMatPetIconSet[i].gameObject.SetActive(false);
// 			}
// 		}
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//顯示解鎖寵物選擇列表
// 	public void ShowUnlockPetList()
// 	{
// 		TweenPosition tw = spritePet1Blank.gameObject.GetComponent<TweenPosition>();
// 		if(tw!=null && tw.enabled == false)
// 		{
// 			tw.ResetToBeginning();
// 			tw.PlayForward();
// 		}
// 		tw = spritePet2Blank.gameObject.GetComponent<TweenPosition> ();
// 		if(tw!=null && tw.enabled == false)
// 		{
// 			tw.ResetToBeginning();
// 			tw.PlayForward();
// 		}
// 		//
// 		uiTopStateView.gameObject.SetActive (false);
// 		if(panelBP1PetInfo.gameObject.activeSelf==false)
// 			SetBattlePet1();
// 		if(panelBP2PetInfo.gameObject.activeSelf==false)
// 			SetBattlePet2();
// 		panelSelectPetForStrength.gameObject.SetActive(true);
// 
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//自動升級時所有按鈕類失效功能
// 	public void SwitchBtnFunDuringAuto(bool bswitch)
// 	{
// 		btnEnhancePet.isEnabled = bswitch;
// 		btnAutoPutMaterial.isEnabled = bswitch;
// 		btnAutoUpGrade.isEnabled = bswitch;
// 		btnSelectedPet.isEnabled = bswitch;
// 		btnInfo.isEnabled = bswitch;
// 		for(int i=0;i<btnSelectedMaterialSet.Length;++i)
// 			btnSelectedMaterialSet[i].isEnabled=bswitch;
// 		
// 		uiTopStateView.SwitchBtnWork (bswitch);
// 		panelMaterialList.gameObject.GetComponent<UIScrollView>().enabled = bswitch;
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//單一顯示解鎖寵物資訊蒐集(2D圖，星等，名稱，等級，血量，攻擊力，經驗值，突破等級)
// 	private int[] CollectEachUnlockPetInfo(S_PetData pd)
// 	{
// 		if (pd == null)
// 			return null;
// 		int[] InfoSet = new int[8]; 
// 		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(pd.iPetDBFID);
// 		InfoSet[0]= pdTmp.Texture;		//2D圖
// 		InfoSet[1]= pdTmp.iRank;		//星等
// 		InfoSet[2]= pdTmp.iName;		//名稱
// 		InfoSet[3]= pd.iPetLevel;		//等級
// 		int PlusHP = (int)pdTmp.fMaxHP_UP*(pd.iPetLevel-1);
// 		InfoSet[4]= pdTmp.sAttrTable.iMaxHP + PlusHP;			//血量
// 		int PlusATK = (int)pdTmp.fAttack_UP*(pd.iPetLevel-1);
// 		InfoSet[5]= pdTmp.sAttrTable.iAttack + PlusATK;			//攻擊力
// 		//經驗值
// 		InfoSet[6]= pd.iPetExp;
// 		//突破等級
// 		InfoSet[7]= pd.iPetLimitLevel;
// 		return InfoSet;
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//設定選擇頁戰寵1
// 	public void SetBattlePet1()
// 	{
// 		if (BP1Petdata==null)
// 			return;
// 
// 		int[] BP1InfoSet = CollectEachUnlockPetInfo(BP1Petdata);
// 		//設定角色圖
// 		Utility.ChangeAtlasSprite(spriteBattlePet1,BP1InfoSet[0]);	
// 		//設定星等
// 		for(int i=0;i<spriteBP1StarRankSet.Length;++i)
// 		{
// 			if(i<BP1InfoSet[1])
// 				spriteBP1StarRankSet[i].gameObject.SetActive(true);
// 			else
// 				spriteBP1StarRankSet[i].gameObject.SetActive(false);
// 		}
// 		//選擇使用哪個資訊框
// 		if(BP1InfoSet[1]>0 && BP1InfoSet[1]<4)
// 			Utility.ChangeAtlasSprite(spriteBP1PetInfoBG,1006);	//夥伴普卡1~3星用
// 		if(BP1InfoSet[1]>3 && BP1InfoSet[1]<6)
// 			Utility.ChangeAtlasSprite(spriteBP1PetInfoBG,1005);	//夥伴銀卡4~5星用
// 		if(BP1InfoSet[1]>5 && BP1InfoSet[1]<=7)
// 			Utility.ChangeAtlasSprite(spriteBP1PetInfoBG,1004);	//夥伴金卡6~7星用
// 		if(BP1InfoSet[1]<0 || BP1InfoSet[1]>7)
// 			return;												//非正常狀況時
// 		//設定名稱
// 		lbBP1Name.text = GameDataDB.GetString(BP1InfoSet[2]);
// 		/*
// 		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(BP1Petdata.iPetDBFID);
// 		if (pdTmp != null)
// 			pdTmp.SetRareColor(lbBP1Name);*/
// 		//設定等級
// 		lbBP1PetLevel.text = BP1InfoSet[3].ToString();
// 		//設定血量
// 		lbBP1PetBlood.text = BP1InfoSet[4].ToString();
// 		//設定攻擊力
// 		lbBP1PetAttack.text = BP1InfoSet[5].ToString();
// 		//設定突破等級
// 		for(int i=0;i<spriteBP1LimitLVRank.Length;++i)
// 		{
// 			if(i<BP1InfoSet[7])
// 				spriteBP1LimitLVRank[i].gameObject.SetActive(true);
// 			else
// 				spriteBP1LimitLVRank[i].gameObject.SetActive(false);
// 		}
// 
// 		panelBP1PetInfo.gameObject.SetActive(true);
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	public void SetBattlePetToEnhance(GameObject gb)
// 	{
// 		if(gb==btnBP1Select.gameObject)
// 			EnhancePetData = BP1Petdata;
// 		else if(gb==btnBP2Select.gameObject)
// 			EnhancePetData = BP2Petdata;
// 
// 		SetEnhancePet();
// 		uiTopStateView.gameObject.SetActive(true);
// 		panelSelectPetForStrength.gameObject.SetActive(false);
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//設定選擇頁戰寵2
// 	public void SetBattlePet2()
// 	{
// 		if (BP2Petdata==null)
// 			return;
// 
// 		int[] BP2InfoSet = CollectEachUnlockPetInfo(BP2Petdata);
// 		//設定角色圖
// 		Utility.ChangeAtlasSprite(spriteBattlePet2,BP2InfoSet[0]);	
// 		//設定星等
// 		for(int i=0;i<spriteBP2StarRankSet.Length;++i)
// 		{
// 			if(i<BP2InfoSet[1])
// 				spriteBP2StarRankSet[i].gameObject.SetActive(true);
// 			else
// 				spriteBP2StarRankSet[i].gameObject.SetActive(false);
// 		}
// 		//選擇使用哪個資訊框
// 		if(BP2InfoSet[1]>0 && BP2InfoSet[1]<4)
// 			Utility.ChangeAtlasSprite(spriteBP2PetInfoBG,1006);	//夥伴普卡1~3星用
// 		if(BP2InfoSet[1]>3 && BP2InfoSet[1]<6)
// 			Utility.ChangeAtlasSprite(spriteBP2PetInfoBG,1005);	//夥伴銀卡4~5星用
// 		if(BP2InfoSet[1]>5 && BP2InfoSet[1]<=7)
// 			Utility.ChangeAtlasSprite(spriteBP2PetInfoBG,1004);	//夥伴金卡6~7星用
// 		if(BP2InfoSet[1]<0 || BP2InfoSet[1]>7)
// 			return;												//非正常狀況時
// 		//設定名稱
// 		lbBP2Name.text = GameDataDB.GetString(BP2InfoSet[2]);
// 		/*
// 		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(BP2Petdata.iPetDBFID);
// 		if (pdTmp != null)
// 			pdTmp.SetRareColor(lbBP2Name);*/
// 		//設定等級
// 		lbBP2PetLevel.text = BP2InfoSet[3].ToString();
// 		//設定血量
// 		lbBP2PetBlood.text = BP2InfoSet[4].ToString();
// 		//設定攻擊力
// 		lbBP2PetAttack.text = BP2InfoSet[5].ToString();
// 		//設定突破等級
// 		for(int i=0;i<spriteBP2LimitLVRank.Length;++i)
// 		{
// 			if(i<BP2InfoSet[7])
// 				spriteBP2LimitLVRank[i].gameObject.SetActive(true);
// 			else
// 				spriteBP2LimitLVRank[i].gameObject.SetActive(false);
// 		}
// 
// 		panelBP2PetInfo.gameObject.SetActive(true);
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//設定主頁強化寵物
// 	public void SetEnhancePet()
// 	{
// 		if (EnhancePetData==null)
// 			return;
// 		
// 		int[] mainInfoSet = CollectEachUnlockPetInfo(EnhancePetData);
// 		//設定角色圖
// 		Utility.ChangeAtlasSprite(spriteEnhancePet,mainInfoSet[0]);	
// 		//設定星等
// 		for(int i=0;i<spriteStarRankSet.Length;++i)
// 		{
// 			if(i<mainInfoSet[1])
// 				spriteStarRankSet[i].gameObject.SetActive(true);
// 			else
// 				spriteStarRankSet[i].gameObject.SetActive(false);
// 		}
// 		//選擇使用哪個資訊框
// 		if(mainInfoSet[1]>0 && mainInfoSet[1]<4)
// 			Utility.ChangeAtlasSprite(spritePetInfoBG,1006);	//夥伴普卡1~3星用
// 		if(mainInfoSet[1]>3 && mainInfoSet[1]<6)
// 			Utility.ChangeAtlasSprite(spritePetInfoBG,1005);	//夥伴銀卡4~5星用
// 		if(mainInfoSet[1]>5 && mainInfoSet[1]<=7)
// 			Utility.ChangeAtlasSprite(spritePetInfoBG,1004);	//夥伴金卡6~7星用
// 		if(mainInfoSet[1]<0 || mainInfoSet[1]>7)
// 			return;												//非正常狀況時
// 		//設定名稱
// 		lbPetName.text = GameDataDB.GetString(mainInfoSet[2]);
// 		/*
// 		S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(EnhancePetData.iPetDBFID);
// 		if (pdTmp != null)
// 			pdTmp.SetRareColor(lbPetName);*/
// 		//設定等級
// 		lbPetLevel.text = mainInfoSet[3].ToString();
// 		//設定血量
// 		lbPetBlood.text = mainInfoSet[4].ToString();
// 		//設定攻擊力
// 		lbPetAttack.text = mainInfoSet[5].ToString();
// 		//設定經驗值
// 		iExp = mainInfoSet [6];
// 		iMaxExp = GameDataDB.PetLevelUpDB.GetData(mainInfoSet[3]).iExpRank[mainInfoSet[1]];
// 		isUpdateExp = true;
// 		//設定突破等級
// 		for(int i=0;i<spriteLimitLVRank.Length;++i)
// 		{
// 			if(i<mainInfoSet[7])
// 				spriteLimitLVRank[i].gameObject.SetActive(true);
// 			else
// 				spriteLimitLVRank[i].gameObject.SetActive(false);
// 		}
// 		panelPetInfo.gameObject.SetActive(true);
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//預覽強化後如有升級的變化
// 	private void PreviewAfterUpGrade()
// 	{
// 		int PetLV = EnhancePetData.iPetLevel;
// 		int iPetRank = EnhancePetData.GetPetRank();
// 		int curPetNeedExp = GameDataDB.PetLevelUpDB.GetData(PetLV).iExpRank[iPetRank];
// 		int SumEXP = iExp + iPlusExp;
// 		if(SumEXP<iMaxExp)
// 			panelPreviewPart.gameObject.SetActive(false);
// 		else
// 		{
// 			while(!(SumEXP<curPetNeedExp))	//當被減的經驗值小於等級相對應所需的經驗值迴圈
// 			{
// 				SumEXP-=curPetNeedExp;
// 				++PetLV;
// 				curPetNeedExp = GameDataDB.PetLevelUpDB.GetData(PetLV).iExpRank[iPetRank];
// 			}
// 			S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(EnhancePetData.iPetDBFID);
// 			int PlusHP  	= (int)pdTmp.fMaxHP_UP*(PetLV-1);
// 			int TotalHP	 	= pdTmp.sAttrTable.iMaxHP + PlusHP;			//提升後的血量
// 			int PlusATK 	= (int)pdTmp.fAttack_UP*(PetLV-1);
// 			int TotalATK 	= pdTmp.sAttrTable.iAttack + PlusATK;		//提升後的攻擊力
// 
// 			lbPreviewUPLV.text 	= PetLV.ToString();
// 			lbPreviewUPHP.text 	= TotalHP.ToString();
// 			lbPreviewUPATK.text = TotalATK.ToString();
// 			panelPreviewPart.gameObject.SetActive(true);
// 		}
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//預覽突破的變化
// 	private void PreviewLimitLevel()
// 	{
// 		if(EnhancePetData.iPetLimitLevel > 3)
// 			return;
// 		//Init
// 		for(int i=0;i<spritePreLimitLV.Length;++i)
// 			spritePreLimitLV[i].gameObject.SetActive(false);
// 		lbRemainTimes.gameObject.SetActive(false);
// 
// 		int iTheSamePet = 0;
// 		EnhancePetsState enhancepetsstate = (EnhancePetsState)ARPGApplication.instance.GetGameStateByName(GameDefine.ENHANCEPETS_STATE);
// 		for(int i=0;i<enhancepetsstate.m_NutrientPetDBID.Length;++i)
// 		{
// 			if(enhancepetsstate.m_NutrientPetDBID[i] == EnhancePetData.iPetDBFID)
// 				++iTheSamePet;
// 		}
// 		if(iTheSamePet<=0)
// 			panelPreviewLimitLV.gameObject.SetActive(false);
// 		else
// 		{
// 			int m_SamePetNum = iTheSamePet;
// 			for(int j=EnhancePetData.iPetLimitLevel;j<spritePreLimitLV.Length;++j)
// 			{
// 				if(m_SamePetNum>0)
// 				{
// 					spritePreLimitLV[j].gameObject.SetActive(true);
// 					--m_SamePetNum;
// 				}
// 			}// end for
// 			if((EnhancePetData.iPetLimitLevel+iTheSamePet)<spritePreLimitLV.Length)
// 			{
// 				lbRemainTimes.text = "剩餘突破數"+(spritePreLimitLV.Length-(EnhancePetData.iPetLimitLevel+iTheSamePet)).ToString();
// 				lbRemainTimes.gameObject.SetActive(true);
// 			}
// 			panelPreviewLimitLV.gameObject.SetActive(true);
// 		}//end else
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	IEnumerator AutoUpGradeProcess() 
// 	{
// 		//取出現在的UI狀態
// 		EnhancePetsState enhancepetsstate = (EnhancePetsState)ARPGApplication.instance.GetGameStateByName(GameDefine.ENHANCEPETS_STATE);
// 		if(enhancepetsstate.m_AutoGrade==true)
// 		{
// 			yield return new WaitForSeconds(1.0f);
// 			enhancepetsstate.AutoPutMaterialEvent();
// 			yield return new WaitForSeconds(1.0f);
// 			if(enhancepetsstate.m_AutoGrade==true)
// 				enhancepetsstate.EnhancePetEvent();
// 		}
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//執行自動升級動作
// 	public void ExecuteAutoGrade()
// 	{
// 		StartCoroutine(AutoUpGradeProcess());
// 	}
// 	//-------------------------------------------------------------------------------------------------------------
// 	//-------------------------------------------------------------------------------------------------------------
}