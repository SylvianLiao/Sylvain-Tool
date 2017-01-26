using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

//分頁列舉
public enum Enum_ItemClass
{
	All = 0,
	Equips,
	CosAll,
	NonEquip,
}
public enum Enum_Target
{
	BattlePet1 = 0,
	BattlePet2,
	MainRole,
	Max
}

public class UI_ItemBag : NGUIChildGUI 
{
	public UITexture					texBackGround		= null;		// UI背景圖
	public UIPanel						panelBase			= null;		// UI本體
	public UIPanel						panelDraggablePanel	= null;		// ItemListView
	//上方分頁相關
	public UISprite						spriteItemSwitchSet	= null;		// 各分頁鈕的集合
	public UIWidget						wgRolesPower		= null;		// 戰力值集合
	public UILabel						lbRolesPower		= null;		// 戰力值

	public AddNumber  					jumpNumber			= null;		// 戰力更新跳數值表演用 掛在lbRolesPower底下

	public UIToggle						tgAll				= null;		// 全部分頁
	public UIToggle						tgEquips			= null;		// 裝備分頁
	public UIToggle						tgCosAll			= null;		// 時裝分頁
	public UIToggle						tgOther				= null;		// 其他分頁
	public UILabel						lbAllPage			= null;
	public UILabel						lbEquipsPage		= null;
	public UILabel						lbCosAllPage		= null;
	public UILabel						lbOtherPage			= null;
	//
	public UIButton						btnAll				= null;		// 全部分頁事件按鈕
	public UIButton						btnEquips			= null;		// 裝備分頁事件按鈕
	public UIButton						btnCosAll			= null;		// 時裝分頁事件按鈕
	public UIButton						btnOther			= null;		// 其他分頁事件按鈕

	//Roles Page Control Set
	public UIButton						btnSwitchState		= null;		// 背包/狀態切換鈕
	public UILabel						lbSwitchState		= null;		// 背包/狀態切換字樣
	public UILabel						lbRolesName			= null;		// 角色名稱
	public UIButton						btnAllEnhance		= null;		// 一鍵強化
	public UILabel						lbAllEnhance		= null;		// 一鍵強化字樣
	public UIWidget						wgEnhanceTip		= null;		// 強化提示
	public UIButton						btnSpeedEquip		= null;		// 一鍵換裝
	public UIToggle						tgRolesWeapon		= null;		// 武器欄
	public UIToggle						tgRolesArmor		= null;  	// 防具欄
	public UIToggle						tgRolesNecklace		= null;  	// 項鍊欄
	public UIToggle						tgRolesRing			= null;  	// 戒指欄
	public UIToggle						tgRolesMagicWeapon	= null;  	// 法寶欄
	public UIToggle 					tgRolesAmulet		= null;  	// 護符欄
	public UIToggle						tgRolesCosClothes	= null;		// 時裝欄
	public UIToggle						tgRolesCosBack		= null;		// 背飾欄
	//
	public GameObject					ModuleShowLoc		= null;		//3D人物生成位置
	//
	public UIButton						btnRolesWeapon		= null;		// 武器欄事件按鈕
	public UIButton						btnRolesArmor		= null;  	// 防具欄事件按鈕
	public UIButton						btnRolesNecklace	= null;  	// 項鍊欄事件按鈕
	public UIButton						btnRolesRing		= null;  	// 戒指欄事件按鈕
	public UIButton						btnRolesMagicWeapon	= null;  	// 法寶欄事件按鈕
	public UIButton 					btnRolesAmulet		= null;  	// 護符欄事件按鈕
	public UIButton						btnRolesCosClothes	= null;		// 時裝欄事件按鈕
	public UIButton						btnRolesCosBack		= null;		// 背飾欄事件按鈕
	public UIButton						btnShowHideCosClothes =null;	// 是否顯示時裝
	public UISprite						spriteSwitchIcon	= null;		// 顯示時裝icon		
	public UIButton						btnWings			= null;		// 是否顯示背飾
	public UISprite						spWingsTip			= null;		// 翅膀可升級提示
	//新增切換相關
	[Header("Switch")]
	public UIToggle						tgShowMainRole		= null;		// 切換主角
	public UIToggle						tgShowBattlePet1	= null;		// 切換寵物1
	public UIToggle						tgShowBattlePet2	= null;		// 切換寵物2	
	public Slot_RoleIcon				swShowMainRole		= null;		// 切換主角圖
	public UISprite						swShowBattlePet1	= null;		// 切換寵物1圖
	public UISprite						swShowBattlePet2	= null;		// 切換寵物2圖
	public UIButton						btnPetList			= null;		// 切換到未出戰寵物
	public UILabel						lbPetList			= null;		// 切換到未出戰寵物字串
	//
	public Dictionary<ENUM_WearPosition,int>RoleEquipList	= new Dictionary<ENUM_WearPosition, int>();		// 玩家已裝備物品列表
	public Dictionary<ENUM_WearPosition,int>Pet1EquipList	= new Dictionary<ENUM_WearPosition, int>();		// 寵物1已裝備物品列表
	public Dictionary<ENUM_WearPosition,int>Pet2EquipList	= new Dictionary<ENUM_WearPosition, int>();		// 寵物2已裝備物品列表

	public Dictionary<ENUM_WearPosition, S_ItemData>RoleEquipList2	= new Dictionary<ENUM_WearPosition, S_ItemData>();		// 玩家已裝備物品列表
	public Dictionary<ENUM_WearPosition, S_ItemData>Pet1EquipList2	= new Dictionary<ENUM_WearPosition, S_ItemData>();		// 寵物1已裝備物品列表
	public Dictionary<ENUM_WearPosition, S_ItemData>Pet2EquipList2	= new Dictionary<ENUM_WearPosition, S_ItemData>();		// 寵物2已裝備物品列表
	public List<UILabel>				lbEQInheritNum		= null;		// 裝備中裝備強化值(+999)
	public List<UISprite>				spriteEQbackground	= null;		// 裝備中裝備強化值的背景
	public List<UIButton>				btnUpRankTip		= null;		// 裝備中升階提示
	public List<UISprite>				spriteEmptyEqTip	= null; 	// 未裝備欄位中顯示有裝備可用的提示
	//
	public Enum_Target					curTarget			= Enum_Target.MainRole;		// 目前頁面的角色目標(預設玩家)
	public Enum_Target					tempTarget			= Enum_Target.MainRole;		// 暫存角色目標(預設玩家)
	public ENUM_WearPosition			tempWearPos			= ENUM_WearPosition.ENUM_WearPosition_None;	// 暫存目前裝備位置
	public UISprite[]					spriteUpEffect 		= new UISprite[5];
	//	
	private	UISprite[]					Equiped				= null;		// 已裝備Temp
	private UISprite[] 					EqIcon				= null;		// 裝備欄換裝換圖Temp
	private int							TempEqID			= 0;		// 暫存裝備欄換圖TempID
	private bool[]						bEnableKinds		= new bool[8]; //用來開啟關閉篩選功能
	//
	[HideInInspector]
	public bool							ExchangeEquip		= false;	//控制是否為交換裝備
	//
	[HideInInspector]
	public bool							bNUPetEquipTip		= false; 	//控制非使用寵物的裝備Tip顯示
	[HideInInspector]
	public int							iTargetPetID		= -1;		//紀錄未出戰寵物ID
	
	public UILabel						lbSellPrice			= null;		// 販賣價格
	private const string				m_SlotName			= "Slot_Item";
	private const int					m_AdjustSlotDepth	= 100;		//SlotItem的Depth調整值
	//Item Page Control Set
	public UISprite						spriteBagBackGround = null;		// 物品列表與各個功能鈕的集合
	public UIToggle						tgItemBG			= null;		// 道具格樣版
	public UILabel						lbBlankNums			= null;		// 格子數
	public UIButton						btnAddBlank			= null;		// 購買格子數btn
	public UIButton						btnForSale			= null;		// 販賣btn
	public UILabel						lbForsale			= null;		// 販賣btn字樣
	public UIPanel						panelForSaleCounts	= null;		// 販賣版面
	public UIButton						btnPlus				= null;		// 增加販賣數量btn
	public UIButton						btnMinus			= null;		// 減少販賣數量btn
	public UIButton						btnQuitForSale		= null;		// 離開販賣
	public UIButton						btnSendForSale		= null;		// 送出販賣需求
	public UILabel						lbForSaleCounter	= null;		// 販賣數量
	[HideInInspector]
	public int							iItemCount			= -1;		// 紀錄點擊的物品堆疊數
	[HideInInspector]
	public ulong						Itemiserial			= 0;		// 紀錄點擊的物品序號
//	{
//		get{return selectItmeSerial;}
//		set
//		{
//			S_ItemData selectItemData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(selectItmeSerial);
//			if (selectItemData != null)
//			{
//				List<S_Fusion_Tmp> fusionList = GameDataDB.GetFusionResult(selectItemData.ItemGUID);
//				if (fusionList != null)
//				{
//					for(int i=0 ; i < fusionList.Count ; ++i)
//					{
//
//					}
//				}
//
//			}
//			selectItmeSerial = value;
//		}
//	}	
//	private long						selectItmeSerial	= -1;
	//
	private float						ForSaleNum			= 1;		// 販賣數量(起始是1)
	private bool						bPlusPress			= false;	// 是否按住Plus鍵;
	private bool						bMinusPress			= false;	// 是否按住Minus鍵;
	private float						Speedtime			= 0;		// 記算快速增加/減少時間
	private const float					MinSaleValue 		= 2;		// 最小出售數量
	private bool						bCanSell			= false;	// 是否能販售
	//ItemListView
	const string 						listItemTag 		= "listItem";
	public Transform 					itemPrefab;
	public UITable 						table;							// 顯示清單
	public UIScrollView 				draggablePanel;					// 滾動軸
	public bool 						enableLog 			= true;		// Log開關
	public float 						cellHeight 			= 80f;		// 每行高度
	// item pool
	public  int							ColSize				= 4;	
	public	int 						PoolSize 			= 100;		// ItemSize(格子數)	(ColSize倍數)
	public  int							ShowIndex			= 8;		// 可以看到格子的量	(ColSize倍數)
	public UIToggle						BkToggle			= null;		// 隱藏用toggle(讓畫面上不顯示click)
	private int 						startIndex 			= 0; 		// 開始點	
	private int 						RowSize 			= 0;		// 行數
	private int		 					iMaxBagNums 		= 0;		// 資料最大筆數
	private bool 						isUpdatingList 		= false;	// 更新旗標
	public List<Transform> 				itemsPool 			= new List<Transform>();	// ItemPool-格子控管器
	private Hashtable 					dataTracker 		= new Hashtable();			// 目前有顯示編號控管器[格數，編號]	
	private int							ClickIndex			= -1;		// 紀錄點擊的格子位置
	[HideInInspector]
	public int							ItemDBFID			= -1;		// 紀錄點擊的物品DBFID
	// Tip Rank Star相關
	private UISprite[]					spriteRanks			= null;		// 星星圖片
	private const int					iMAXRank			= 7;		// 星星數
	//
	[HideInInspector]
	public List<ulong> 					SortItemList		= new List<ulong>();			// 排序用背包清單(塞物品序號)
	private List<int>					AllNonEquipList		= new List<int>();			// 用來記算背包裡非裝備的物品數量
	public Enum_ItemClass				emSortPage			= Enum_ItemClass.All;		// 顯示種類
	public UIPanel						panelUpEffect		= null;						//掛載特效用的prefab
	//
	[HideInInspector]public UI_TopStateView		uiTopStateView		= null;
	[HideInInspector]public UI_BagTip			uiBagTip			= null;
	//-------------------------------角色數值-------------------------------------------------------
	public UIButton						btnCloseRoleInfo	= null;     //關閉狀態
	public UIWidget						InfoBoard			= null;		//角色狀態表
	public UISprite						spriteRoleInfo		= null;		//角色狀態數值表
	public UIPanel						panelInfoTable		= null;		//右側狀態列表
	//上方屬性欄
	public UILabel						lbStateRoleName		= null;		//角色名稱
	public UIWidget						containerType		= null; 	//職別型能集合
	public UILabel						lbTypeName			= null; 	//職別名稱
	public UISprite						spCalss				= null; 	//型態圖
	public UILabel						lbRoleType			= null;		//角色類型(近戰或遠程)
	public UIProgressBar				barRoleExp			= null;		//角色經驗值條
	public UILabel						lbExpTitle			= null;		//經驗值標題
	public UILabel						lbExpValue			= null;		//經驗值
	//public UILabel					lbLevelTitle		= null;		//等級標題
	public UILabel						lbLevelValue			= null;		//等級數值
	//下方屬性欄
	public UILabel						lbHPTitle			= null;		//生命標題
	public UILabel						lbADTitle			= null;		//攻擊力標題
	public UILabel						lbArmorTitle		= null;		//防禦力標題
	public UILabel						lbAPTitle			= null;		//技能攻擊力標題
	public UILabel						lbMRTitle			= null;		//技能防禦力標題
	public UILabel						lbCriricalTitle		= null;		//爆擊標題
	public UILabel						lbCriticalDmgTitle	= null;		//爆擊傷害標題
	public UILabel						lbTenacityTitle		= null;		//韌性標題
	public UILabel						lbAttackSpeedTitle	= null;		//攻速標題
	public UILabel						lbARPTitle			= null;		//物理穿透標題
	public UILabel						lbMoveSpeedTitle	= null;		//跑速標題
	public UILabel						lbMRPTitle			= null;		//技能穿透標題

	public UILabel						lbHPValue			= null;		//生命數值
	public UILabel						lbADValue			= null;		//物理攻擊力數值
	public UILabel						lbArmorValue		= null;		//物理防禦力數值
	public UILabel						lbAPValue			= null;		//技能攻擊力數值
	public UILabel						lbMRValue			= null;		//技能防禦力數值
	public UILabel						lbCriricalValue		= null;		//爆擊數值
	public UILabel						lbCriticalDmgValue	= null;		//爆擊傷害數值
	public UILabel						lbTenacityValue		= null;		//韌性數值
	public UILabel						lbAttackSpeedValue	= null;		//攻速數值
	public UILabel						lbARPValue			= null;		//物理穿透數值
	public UILabel						lbMoveSpeedValue	= null;		//跑速數值
	public UILabel						lbMRPValue			= null;		//技能穿透數值
//	public UIButton						btnTenacityValue	= null;		//韌性數值按鈕
//	public UIButton						btnMRPValue			= null;		//技能穿透數值按鈕
//	public UIButton						btnARPValue			= null;		//物理穿透數值按鈕
	
	//寵物突破數
	public GameObject					gPetBreakLimit		= null;		//突破圖示集合
	public UISprite	[]					spBreakArray		= new UISprite[4];		//突破圖示陣列
	public UISprite						spriteStateStar1	= null;		//星等1
	public UISprite						spriteStateStar2	= null;		//星等2
	public UISprite						spriteStateStar3	= null;		//星等3
	public UISprite						spriteStateStar4	= null;		//星等4
	public UISprite						spriteStateStar5	= null;		//星等5
	public UISprite						spriteStateStar6	= null;		//星等6
	public UISprite						spriteStateStar7	= null;		//星等7

	private int 						Pet1DBFID			= -1;		//出戰寵物1的DBFID
	private int 						Pet2DBFID			= -1;		//出戰寵物2的DBFID
	private S_PetData					Pet1Data			= new S_PetData();	//出戰寵物1的PetData
	private S_PetData					Pet2Data			= new S_PetData();	//出戰寵物2的PetData
	private List<UISprite>				StateStarSet		= new List<UISprite>();		//狀態表中星等的集合
	//

	//暫時代替使用
	public UIPanel						panelTempPause		= null;		//暫時阻止畫面(未設定寵物)
	public UIButton						btnquitcur			= null;		//離開阻止畫面

	//private bool						bCurTipState		= false;	//取得按下切換狀態前提示板的顯示狀態

	private S_ItemData					showInfoData		= null;
	private string						showInfoStr			= null;

	private const int					abilityStrNumBegin  = 1012;
	private int 						MainGbID 			= 0;

	public SpinWithMouse				LimitRotateModule	= null;
    public UILabel						lbSaleMoney		    = null;		// 販售金額

	[Header("Enhance UI")]
	[NonSerialized] public UI_SpeedUpBoard2		m_uiSpeedUpBoard2			= null;
	private const string		m_UISpeedUpBoard2Name		= "UI_SpeedUpBoard2";
	[Header("New Guide")]
	//-------------------裝備物品教學--------------------------	
	public UIPanel				panelGuide					= null; 	//導引相關集合
	public UIButton				btnTopFullScreen			= null; 	//最上層的全螢幕按鈕
	public UIButton				btnFullScreen				= null; 	//任意鍵繼續導引
	public UISprite				spGuideSelectItem			= null; 	//導引選擇物品
	public UILabel				lbGuideSelectItem			= null;
	public UISprite				spGuideItemInfo				= null; 	//導引物品資訊
	public UILabel				lbGuideItemInfo				= null;
	public UISprite				spGuideBotButton			= null; 	//導引Tip下方按鈕
	public UILabel				lbGuideBotButton			= null; 
	public UISprite				spGuideEnhance				= null; 	//導引強化按鈕
	public UILabel				lbGuideEnhance				= null; 
	public UISprite				spGuideCloseEnhance			= null; 	//導引關閉強化UI
	public UILabel				lbGuideCloseEnhance			= null;
	public UISprite				spGuideShowRolePower		= null; 	//導引戰力資訊
	public UILabel				lbGuideShowRolePower		= null;
	public UILabel				lbBagGuideEnd				= null; 	//導引背包教學結束
	public UISprite				spGuideSmtihBtn				= null; 	//導引鍛造清單中的按鈕
	public UILabel				lbGuideSmtihBtn				= null;
	public UISprite				spGuideQuit					= null; 	//導引離開背包
	public UILabel				lbGuideQuit					= null;
	public UIButton				btnFakeQuit					= null; 	//假的離開按鈕
	//-------------------隊伍2教學-第二隻寵物裝備物品--------------------------	
	public UISprite				spGuideChoosePet			= null; 	//導引選擇寵物2
	public UILabel				lbGuideChoosePet			= null;
	//-------------------------未出戰夥伴教學--------------------------	
	public UISprite				spGuideSelectNoUsePet		= null; 	//導引選擇第一個未出戰夥伴
	public UILabel				lbGuideSelectNoUsePet		= null;
	/*
	//---------------------裝備強化教學-----------------------------
	public UISprite					spriteGuideEnhanceItem	= null; //導引強化裝備
	public UILabel					lbGuideEnhanceItem		= null; 
	public UISprite					spriteGuideShowAllEquiped	= null; //導引裝備欄防具
	public UILabel					lbGuideShowAllEquiped	= null; 
	public UISprite					spriteGuideSpeedEnhance	= null; //導引一鍵強化
	public UILabel					lbGuideSpeedEnhance		= null; 
	public UILabel					lbBagGuideEnd			= null; //導引背包教學結束
	*/
	//-------------------------------------------------------------------------------
	public UIWidget					EquipContainer			= null;
	private const string			NonUseStr				= "--";
	[HideInInspector]
	public bool						bUpdateBagItemTipInfo	= false;
	[HideInInspector]
	public Vector3					StrengthUpLoc			= new Vector3();
	private bool					isTipAnimPlaying		= false;
	//-----------------------------------------------------------------------------------------------------
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_ItemBag";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_ItemBag() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		//載入玩家的裝備
		EquipedSelfState();
		// 收集顯示物品
		emSortPage = Enum_ItemClass.All;
		//		SortItemData();
		SortItemDataByType();
		//載入戰力
		lbRolesPower.text =ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetMainRolePower().ToString();
		// 計算行數
		double dRow = PoolSize/ColSize;
		RowSize		= Convert.ToInt32( Math.Ceiling( dRow ) );
		iMaxBagNums= Math.Max(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Count , SortItemList.Count);
		// 建立格子
		if(iMaxBagNums<PoolSize)
		{
			CreateItemListView(iMaxBagNums);
		}
		else
		{
			CreateItemListView(PoolSize);
		}
		//未裝備的物品(用以顯示在背包欄中)
		NonEquipedItemNum();
		tgAll.value = true;
		ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.SetUpdateBag(true);
		//先取得是否有設置出戰寵物
		LoadBattlePets();
		//
		SwitchTipBoard(false); //開始隱藏Tip
		//重置開啟動畫
		ResetTweenPosUse(spriteBagBackGround.gameObject);
		ResetTweenPosUse(lbRolesName.gameObject);

		//設定翅膀可升級提示
		spWingsTip.gameObject.SetActive(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckWingsCanUpgrade());
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		jumpNumber.InitialUI();

		//初始化裝備列中的篩選功能
		for(int i=0;i<bEnableKinds.Length;++i)
			bEnableKinds[i] = true;
		//儲存主玩家ID
		MainGbID = ARPGApplication.instance.m_tempGameObjectSystem.GetGameObjectIDByMain();

		//載入字串
		lbSaleMoney.text = GameDataDB.GetString(15073); //販售金額

		lbSwitchState.text	= GameDataDB.GetString(1031); //狀態
		lbAllPage.text		= GameDataDB.GetString(1061); //全部
		lbEquipsPage.text	= GameDataDB.GetString(1007); //裝備
		lbCosAllPage.text	= GameDataDB.GetString(1062); //時裝
		lbOtherPage.text	= GameDataDB.GetString(1006); //其他
		lbForsale.text		= GameDataDB.GetString(1010); //出售
		lbAllEnhance.text = GameDataDB.GetString(1011); //一鍵強化
		//btnSpeedEquip.GetComponentInChildren<UILabel>().text	= GameDataDB.GetString(1032); //一鍵換裝
		lbPetList.text											= GameDataDB.GetString(1151); //未上陣
		lbSellPrice.text										= NonUseStr;
		//主玩家角色名稱
		lbRolesName.text = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName;
		//角色數值面板- 屬性標題
		lbHPTitle.text			= GameDataDB.GetString(1027);//生命
		lbADTitle.text			= GameDataDB.GetString(1028);//物理攻擊
		lbArmorTitle.text		= GameDataDB.GetString(1030);//物理防禦
		lbAPTitle.text			= GameDataDB.GetString(1063);//技能攻擊
		lbMRTitle.text			= GameDataDB.GetString(1064);//技能防禦
		lbCriricalTitle.text	= GameDataDB.GetString(1025);//爆擊
		lbCriticalDmgTitle.text	= GameDataDB.GetString(1068);//爆傷
		lbTenacityTitle.text	= GameDataDB.GetString(1058);//韌性
		lbAttackSpeedTitle.text	= GameDataDB.GetString(1029);//攻速
		lbARPTitle.text			= GameDataDB.GetString(1065);//物理穿透
		lbMoveSpeedTitle.text	= GameDataDB.GetString(1019);//跑速
		lbMRPTitle.text			= GameDataDB.GetString(1066);//技能穿透

		//事件的指派
		//-----------------------------------------------------------------------------------------------------
		//分頁功能指定事件
		EventDelegate.Add(btnAll.onClick, 			AllClass);
		EventDelegate.Add(btnEquips.onClick, 		EquipsClass);
		EventDelegate.Add(btnCosAll.onClick,		CosAllClass);
		EventDelegate.Add(btnOther.onClick,			OtherClass);
		//-----------------------------------------------------------------------------------------------------
		//裝備欄同步調用分頁功能指定事件
		EventDelegate.Add (btnRolesWeapon.onClick, 		EquipedWeaponClick);
		EventDelegate.Add (btnRolesArmor.onClick, 		EquipedArmorClick);
		EventDelegate.Add (btnRolesNecklace.onClick, 	EquipedNecklaceClick);
		EventDelegate.Add (btnRolesRing.onClick, 		EquipedRingClick);
		EventDelegate.Add (btnRolesMagicWeapon.onClick, EquipedMagicWeaponClick);
		//EventDelegate.Add (btnRolesAmulet.onClick, 		EquipedAmuletClick);
		EventDelegate.Add (btnRolesCosClothes.onClick, 	EquipedCosClothesClick);
		//EventDelegate.Add (btnRolesCosBack.onClick, 	EquipedCosBackClick);
		//-----------------------------------------------------------------------------------------------------
		//EventDelegate.Add(btnForSale.onClick,OpenForSaleCountPanel);
		//生成物品資訊欄中的物品圖示(SlotItem)
		//CreateTipItemSlot();
		//-----------------------------------------------------------------------------------------------------
		//生成建立背包格子
		// 計算行數
		double dRow = PoolSize/ColSize;
		RowSize		= Convert.ToInt32( Math.Ceiling( dRow ) );
		iMaxBagNums= Math.Max(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Count , SortItemList.Count);
		// 建立格子
		if(iMaxBagNums<PoolSize)
		{
			CreateItemListView(iMaxBagNums);
		}
		else
		{
			CreateItemListView(PoolSize);
		}

		CreateEnhanceUI();
	}
	//-----------------------------------------------------------------------------------------------------
	//生成強化UI
	private void CreateEnhanceUI()
	{
		UI_SpeedUpBoard2 go = ResourceManager.Instance.GetGUI(m_UISpeedUpBoard2Name).GetComponent<UI_SpeedUpBoard2>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_ItemBag CreateEnhanceUI() error,path:{0}", "GUI/"+m_UISpeedUpBoard2Name) );
			return;
		}
		
		UI_SpeedUpBoard2 newgo = NGUITools.AddChild(this.gameObject,go.gameObject).GetComponent<UI_SpeedUpBoard2>();;
		newgo.InitialUI();
		newgo.InitialLabel(5138,5128,1334,5139);
		m_uiSpeedUpBoard2 = newgo;
	}
	//-----------------------------------------------------------------------------------------------------
	//如果有設罝出戰夥伴即載入
	public void LoadBattlePets()
	{
		int[] iRolePetData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BattlePetIDs;
		//將寵物的DBFID配置到各自的變數中(如果沒有會配置-1)
		Pet1DBFID = ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID;
		Pet2DBFID = ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID;
		//
		//有設出戰夥伴時將其PetData取出並存入相對應的S_PetData變數中
		if(Pet1DBFID!=-1 || Pet2DBFID!=-1)
		{
			for(int i=0;i<ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetListSize();++i)
			{
				if(Pet1DBFID == ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPet(i).iPetDBFID)
				{
					Pet1Data = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPet(i);
				}
				else if(Pet2DBFID == ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPet(i).iPetDBFID)
				{
					Pet2Data = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPet(i);
				}
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//設置切換圖
	public void SetSwitchShowIcon()
	{
		//更換切換圖
        swShowMainRole.SetSlot(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.iFace, ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.iFaceFrameID);
		//
		S_PetData  pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBattlePetData(0);
		if(pd!= null)
			CreateItemIcon(pd,swShowBattlePet1);
		else
			Utility.ChangeAtlasSprite(swShowBattlePet1,29);

		//
		pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetBattlePetData(ENUM_PET_USE.PET_2);
		if(pd!= null)
			CreateItemIcon(pd,swShowBattlePet2);
		else
			Utility.ChangeAtlasSprite(swShowBattlePet2,29);
	}
	//-----------------------------------------------------------------------------------------------------
	private void CreateItemIcon(S_PetData pd,UISprite sp)
	{
		int pItemID = pd.GetPetItemID();

		Slot_Item ItemPrefab = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		if(ItemPrefab == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		Slot_Item pIcon = Instantiate(ItemPrefab) as Slot_Item;
		pIcon.transform.parent 			= sp.transform;
		pIcon.transform.localScale		= Vector3.one;
		pIcon.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
		pIcon.transform.localPosition	= Vector3.zero;//itemSlotLocal[i].localPosition;
		pIcon.SetSlotWithCount(pItemID,1,false,true);
		pIcon.GetComponent<BoxCollider>().enabled = false;
		Utility.ChangeAtlasSprite(sp,-1);
	}
	//-----------------------------------------------------------------------------------------------------
	//紀錄強化時的跳數字位置
	public void RecordStrengthUpLoc()
	{
		StrengthUpLoc =	uiBagTip.btnEnhance.transform.position + new Vector3(0,0.2f,0);
	}
	//-----------------------------------------------------------------------------------------------------
	public void Update()
	{
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetUpdateBag()==true)
		{
			UpdateBagContent();
			UpdateTargetUpRankTip();
			CheckEmptyEquipTip();
			
			ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.SetUpdateBag(false);
		}

		if (isTipAnimPlaying && !uiBagTip.animTipBoard.IsPlaying(uiBagTip.m_TipOpenAnimName))
		{
			uiBagTip.RepostionOtherAttr();
			isTipAnimPlaying = false;
		}

		//增加出售數量事件
		if (bPlusPress)
		{
			if(ForSaleNum<((float)iItemCount))
			{
				Speedtime+=Time.deltaTime;
				if(Speedtime>1)
					ForSaleNum+=Time.deltaTime*10;
				//
				lbForSaleCounter.text = ((int)ForSaleNum).ToString()+"/"+iItemCount.ToString();
			}
		}
		
		//減少出售數量事件
		if (bMinusPress)
		{
			if(ForSaleNum>MinSaleValue)		//要出售數量大於最低限度的出售值才能動作
			{
				Speedtime+=Time.deltaTime;
				if(Speedtime>1)
					ForSaleNum-=Time.deltaTime*10;
				//
				lbForSaleCounter.text = ((int)ForSaleNum).ToString()+"/"+iItemCount.ToString();
			}
		}
		//
		if (!bPlusPress && !bMinusPress)
			Speedtime = 0;
#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.LeftControl) && uiBagTip.spriteTipBoard.gameObject.activeSelf)
		{
/*
			S_ItemData sitemdata = GetSortItemData(ClickIndex);
			uiBagTip.lbTipItemName.text = uiBagTip.lbTipItemName.text +" "+ sitemdata.iSerial.ToString();
*/
			if(ItemDBFID <= 0)
			{
				return ;
			}
			else
			{
				uiBagTip.lbTipItemName.text = uiBagTip.lbTipItemName.text +" "+ ItemDBFID.ToString();
			}
		}
#endif
		//lbRolesPower.text =ARPGApplication.instance.m_tempGameObjectSystem.GetPowerValue(MainGbID).ToString();
//		lbRolesPower.text =ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetMainRolePower().ToString();
	}
	//-----------------------------------------------------------------------------------------------------
	#region 背包物品相關
	//-----------------------------------------------------------------------------------------------------
	// 取得物件
	public Transform GetItemFromPool(int i)
	{
		if(i >= 0 && i< PoolSize)
		{
			itemsPool[i].gameObject.SetActive(true);
			return itemsPool[i];
		}
		else
			return null;
	}
	//-----------------------------------------------------------------------------------------------------
	// 建立物品格
	public void CreateItemListView(int iMaxBagNums)
	{
		if(itemsPool.Count < PoolSize)
		{
			// 重建格子
			RefreshPool();
		}
		else
		{
			for(int i=0; i< PoolSize; i++) 
			{
				itemsPool[i].gameObject.SetActive(false);
			}
		}

		dataTracker.Clear();
		
		int j = 0;
		for(int i=0; i<iMaxBagNums; i++)
		{
			Transform item = GetItemFromPool(j);
			if(j<SortItemList.Count)
			{
				InitListItemWithIndex(item,i,j);

				j++;
			}
			else // end of pool
			{
				item.gameObject.SetActive(false);
				j++;
			}
		}//for

		// 設定資料到格子內
		S_ItemData tempItem;
		j = 0;
		for(int i=0; i<SortItemList.Count; i++)
		{
			tempItem = GetSortItemData(i);

			BagCollection(tempItem,j);
			j++;
		}

		// at the moment we are repositioning the list after a delay... repositioning immediatly messes up the table when refreshing... no clue why...
		Invoke("RepositionList",0.1f);
	}

	//-----------------------------------------------------------------------------------------------------
	public S_ItemData GetSortItemData(int iIndex)
	{
		if(iIndex < 0|| iIndex >= SortItemList.Count)
			return null;

		ulong lSID = SortItemList[iIndex];
		S_ItemData tempItem = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(lSID);
		return tempItem;
	}
	//-----------------------------------------------------------------------------------------------------
	// 重新計算位置
	void RepositionList()
	{
		table.Reposition();
		draggablePanel.SetDragAmount(0,0, false);
	}
	//-----------------------------------------------------------------------------------------------------
	// 刷新物件
	void RefreshPool()
	{
		// 釋放原有物件
		for(int i=0; i< itemsPool.Count; i++) 
		{
			UnityEngine.Object.Destroy(itemsPool[i].gameObject);
		}
		itemsPool.Clear();

		ItemBagBlockBehavior BehaviorComponent;
		// 建立新物件
		for(int i=0; i< PoolSize; i++) // the pool will use itemPrefab as a default
		{
			GameObject item = NGUITools.AddChild(table.gameObject,itemPrefab.gameObject);
			BehaviorComponent = item.GetComponent<ItemBagBlockBehavior>();
			BehaviorComponent.itemNumber = i;
			BehaviorComponent.listPopulator = this;
			BehaviorComponent.panel = draggablePanel.panel;

			item.name = "item_"+i;
			item.transform.localScale = new Vector3(1,1,1);
			item.SetActive(false);
			itemsPool.Add(item.transform);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	// 格子物件設定
	 void InitListItemWithIndex(Transform item, int dataIndex, int poolIndex)
	{
		ItemBagBlockBehavior BehaviorComponent = item.GetComponent<ItemBagBlockBehavior>();
		BehaviorComponent.itemDataIndex = dataIndex;
		item.name = "item_"+dataIndex;
		
		dataTracker.Add(itemsPool[poolIndex].GetComponent<ItemBagBlockBehavior>().itemDataIndex,itemsPool[poolIndex].GetComponent<ItemBagBlockBehavior>().itemNumber);
	}
	//-----------------------------------------------------------------------------------------------------
	// 設定位移資料
	void PrepareListItemWithIndex(Transform item, int newIndex,int oldIndex)
	{
		// 計算位移量
		if(newIndex <oldIndex)
			item.localPosition += new Vector3(0,RowSize*cellHeight,0);
		else
			item.localPosition -= new Vector3(0,RowSize*cellHeight,0);
		
		item.GetComponent<ItemBagBlockBehavior>().itemDataIndex=newIndex;
		item.name = "item_"+(newIndex);
		
		dataTracker.Add(newIndex,(int)(dataTracker[oldIndex]));
		
		dataTracker.Remove(oldIndex);

		// 計算格子編號
		int iIndex = newIndex % PoolSize;
		// 設定新物品資料
		S_ItemData tempItem = GetSortItemData(newIndex);
		if(tempItem!=null)
			BagCollection(tempItem,iIndex);
		else
		{
			item.gameObject.SetActive(false);
		}
		
	}
	//-----------------------------------------------------------------------------------------------------
	// itme超出顯示範圍時呼叫
	public IEnumerator ItemIsInvisible(int itemNumber)
	{
		if(isUpdatingList) yield return null;
		isUpdatingList = true;
		
		// 是否超過最大值
/*		if(iMaxBagNums > PoolSize)
		{
			Transform item = itemsPool[itemNumber];
			int itemDataIndex = 0;
			if(item.tag.Equals(listItemTag))
				itemDataIndex = item.GetComponent<ItemBagBlockBehavior>().itemDataIndex;
			
			int indexToCheck=0;
			ItemBagBlockBehavior infItem = null;

			indexToCheck = 0;
			
			// 往上滾(編號變大)
			if(dataTracker.ContainsKey(itemDataIndex+ColSize))
			{
				infItem = itemsPool[(int)(dataTracker[itemDataIndex+1])].GetComponent<ItemBagBlockBehavior>();
				
				if(infItem != null && infItem.verifyVisibility())
				{
					// 
					indexToCheck = itemDataIndex - ShowIndex;
					
					if(dataTracker.ContainsKey(indexToCheck))
					{
						//依序將所有資料更新(遞減)
						for(int i = indexToCheck; i>=0; i--)
						{
							if(dataTracker.ContainsKey(i))
							{
								infItem = itemsPool[(int)(dataTracker[i])].GetComponent<ItemBagBlockBehavior>();
								
								if(infItem != null && !infItem.verifyVisibility())
								{
									item = itemsPool[(int)(dataTracker[i])];
									if((i)+PoolSize < iMaxBagNums && i>-1)
									{
										PrepareListItemWithIndex(item,i+PoolSize,i);
										
									}
								}
							}
							else
							{
								break;
							}
						}
					}
				}
			}
			// 往下滾(編號變小)
			if(dataTracker.ContainsKey(itemDataIndex-ColSize))
			{
				infItem = itemsPool[(int)(dataTracker[itemDataIndex-1])].GetComponent<ItemBagBlockBehavior>();

				if(infItem != null && infItem.verifyVisibility())
				{
					//dragging downwards check the item below
					indexToCheck = itemDataIndex + ShowIndex;
					
					if(dataTracker.ContainsKey(indexToCheck))
					{
						//依序將所有資料更新(遞增)
						for(int i = indexToCheck; i<iMaxBagNums; i++)
						{
							if(dataTracker.ContainsKey(i))
							{
								infItem = itemsPool[(int)(dataTracker[i])].GetComponent<ItemBagBlockBehavior>();
								
								if(infItem != null && !infItem.verifyVisibility())
								{
									item = itemsPool[(int)(dataTracker[i])];
									if((i)-PoolSize > -1 && (i) < iMaxBagNums)
									{
										PrepareListItemWithIndex(item,i-PoolSize,i);
									}
								}
							}
							else
							{
								break;
							}
						}
					}
				}
			}
		}
		*/
		isUpdatingList = false;
	}
	//-----------------------------------------------------------------------------------------------------
	//未裝備的物品(用以顯示在背包欄中)
	public void NonEquipedItemNum()
	{
		// 清除
		AllNonEquipList.Clear();
		//擷取相對應顯示的物品
		S_Item_Tmp ItemDBF;
		foreach(S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(tempItem==null)
				continue;
			
			// 只顯示未裝備
			if(tempItem.emWearPos!= ENUM_WearPosition.ENUM_WearPosition_None)
				continue;
			
			ItemDBF= GameDataDB.ItemDB.GetData (tempItem.ItemGUID);
			
			if(ItemDBF==null)
				continue;
			
			AllNonEquipList.Add(tempItem.ItemGUID);
		}

		//背包格數的顯示
		lbBlankNums.text = AllNonEquipList.Count.ToString() + "/" + ARPGApplication.instance.m_RoleSystem.iMaxBagNums.ToString (); 
	}
	//-----------------------------------------------------------------------------------------------------
	//給舊式教學用，重新生成物品Slot並排序
	public void RefreshPoolForGuide()
	{
		RefreshPool();
		// 建立格子
		if(iMaxBagNums<PoolSize)
		{
			CreateItemListView(iMaxBagNums);
		}
		else
		{
			CreateItemListView(PoolSize);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	#endregion
	//-----------------------------------------------------------------------------------------------------
	#region 篩選功能相關
	//-----------------------------------------------------------------------------------------------------
	/*//整理分類顯示的物品資訊
	void SortItemData()
	{
		// 清除
		SortItemList.Clear();

		//擷取相對應顯示的物品
		S_Item_Tmp ItemDBF;
		foreach(S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(tempItem==null)
				continue;
			
			// 只顯示未裝備
			if(tempItem.emWearPos!= ENUM_WearPosition.ENUM_WearPosition_None)
				continue;
			
			ItemDBF= GameDataDB.ItemDB.GetData (tempItem.ItemGUID);
			
			if(ItemDBF==null)
				continue;

			// 按照分頁顯示
			switch(emSortPage)
			{

			case Enum_ItemClass.Weapon:												//武器
				if(ItemDBF.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Weapon)
					SortItemList.Add(tempItem.iSerial);
//				SortItemDataByType();
				break;
			case Enum_ItemClass.Armor:												//防具
				if(ItemDBF.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Clothes)
					SortItemList.Add(tempItem.iSerial);
				break;
			case Enum_ItemClass.Necklace:											//項鍊
				if(ItemDBF.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Necklace)
					SortItemList.Add(tempItem.iSerial);
				break;
			case Enum_ItemClass.Ring:												//戒指
				if(ItemDBF.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Ring)
					SortItemList.Add(tempItem.iSerial);
				break;
			case Enum_ItemClass.MagicWeapon:										//法寶
				if(ItemDBF.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Talisman)
					SortItemList.Add(tempItem.iSerial);
				break;
			case Enum_ItemClass.Amulet:												//護符
				if(ItemDBF.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Amulet)
					SortItemList.Add(tempItem.iSerial);
				break;
			case Enum_ItemClass.NonEquip:											//非裝備
				if(ItemDBF.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Null)
					SortItemList.Add(tempItem.iSerial);
				break;
			}
		}// end foreach

	}*/

	//-----------------------------------------------------------------------------------------------------
	public void SortItemDataByType()
	{
		//List<long> 	tempList 		= new List<long>();

		List<S_ItemData> 	AllList 		= new List<S_ItemData>();	//全部
		List<S_ItemData> 	EquipsList 		= new List<S_ItemData>();	//裝備
		List<S_ItemData> 	CosAllList 		= new List<S_ItemData>();	//時裝
		List<S_ItemData> 	nonequipList 	= new List<S_ItemData>();	//非裝備
		
		// 清除
		SortItemList.Clear();
		
		//擷取相對應顯示的物品
		S_Item_Tmp ItemDBF;
		foreach(S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(tempItem==null)
				continue;
			
			// 只顯示未裝備
			if(tempItem.emWearPos!= ENUM_WearPosition.ENUM_WearPosition_None)
				continue;
			
			ItemDBF= GameDataDB.ItemDB.GetData (tempItem.ItemGUID);
			
			if(ItemDBF==null)
				continue;

			AllList.Add(tempItem);
			// 按照物品類型分類
			switch(ItemDBF.emEqPos)
			{
			case ENUM_ItemPosition.ENUM_ItemPosition_Weapon:
				EquipsList.Add(tempItem);
				break;
			case ENUM_ItemPosition.ENUM_ItemPosition_Clothes:
				EquipsList.Add(tempItem);
				break;
			case ENUM_ItemPosition.ENUM_ItemPosition_Necklace:
				EquipsList.Add(tempItem);
				break;
			case ENUM_ItemPosition.ENUM_ItemPosition_Ring:
				EquipsList.Add(tempItem);
				break;
			case ENUM_ItemPosition.ENUM_ItemPosition_Talisman:
				EquipsList.Add(tempItem);
				break;
			case ENUM_ItemPosition.ENUM_ItemPosition_Amulet:
				EquipsList.Add(tempItem);
				break;
			case ENUM_ItemPosition.ENUM_ItemPosition_CosClothes:
				CosAllList.Add(tempItem);
				break;
			case ENUM_ItemPosition.ENUM_ItemPosition_CosBack:
				CosAllList.Add(tempItem);
				break;
			case ENUM_ItemPosition.ENUM_ItemPosition_Null:
				nonequipList.Add(tempItem);
				break;
			default:
				UnityDebugger.Debugger.LogError("SortItemDataByType type error");
				break;
			}
		}// end foreach
/*
		AllList.Sort(CompareItemBySerialID);
		UnityDebugger.Debugger.Log("=====CompareItemBySerialID Begin");
		for(int i =0; i<AllList.Count; ++i)
		{
			UnityDebugger.Debugger.Log(string.Format("=====SerialID = {0} guid = {1}", AllList[i].iSerial, AllList[i].ItemGUID));
		}
		EquipsList.Sort(CompareItemBySerialID);

		CosAllList.Sort(CompareItemBySerialID);
		nonequipList.Sort(CompareItemBySerialID);
		*/
		// 按照分頁顯示
		switch(emSortPage)
		{
		case Enum_ItemClass.All:												//全部
//			AllList.Sort(CompareItemByItemGUID);
			AllList.Sort(CompareItemBySortRule);
			AddToSortItemList(AllList);
			break;
		case Enum_ItemClass.Equips:												//裝備
//			EquipsList.Sort(CompareItemByItemGUID);
			EquipsList.Sort(CompareItemBySortRule);
			AddToSortItemList(EquipsList);
			break;
		case Enum_ItemClass.CosAll:												//時裝
//			CosAllList.Sort(CompareItemByItemGUID);
			CosAllList.Sort(CompareItemBySortRule);
			AddToSortItemList(CosAllList);
			break;
		case Enum_ItemClass.NonEquip:
//			nonequipList.Sort(CompareItemByItemGUID);							//非裝備(其他)
			nonequipList.Sort(CompareItemBySortRule);
			AddToSortItemList(nonequipList);
			break;
		}
/*
		UnityDebugger.Debugger.Log("=====CompareItemByItemGUID AllList Begin");
		for(int i =0; i<AllList.Count; ++i)
		{
			UnityDebugger.Debugger.Log(string.Format("=====SerialID = {0} guid = {1}", AllList[i].iSerial, AllList[i].ItemGUID));
		}

		UnityDebugger.Debugger.Log("=====CompareItemByItemGUID EquipsList Begin");
		for(int i =0; i<EquipsList.Count; ++i)
		{
			UnityDebugger.Debugger.Log(string.Format("=====SerialID = {0} guid = {1}", EquipsList[i].iSerial, EquipsList[i].ItemGUID));
		}
*/
	}
	//----------------------------------------------------------------------------------------------------
	//(阿強畫押) 依指定順序排序(品階 > 類型(自定義 8禮包 > 2武器 > 3防具 > 1素材或材料 > 7經驗值道具 > 0 一般>4金錢 > 5寵物 > 6寵物碎片) > ID)
	private static int CompareItemBySortRule(S_ItemData x, S_ItemData y)
	{
		if(x == null || y ==null)
		{
			return 0;
		}
		else
		{
			S_Item_Tmp xTmp  = GameDataDB.ItemDB.GetData(x.ItemGUID);
			S_Item_Tmp yTmp  = GameDataDB.ItemDB.GetData(y.ItemGUID);
			if(xTmp.RareLevel != yTmp.RareLevel)
			{
				return -xTmp.RareLevel.CompareTo(yTmp.RareLevel);
			}
			else
			{
				int ReXtype = ResetItemPriority(xTmp.ItemType);
				int ReYtype = ResetItemPriority(yTmp.ItemType);

				if(ReXtype != ReYtype)
				{
					return -ReXtype.CompareTo(ReYtype);
				}
				else
				{
					return x.ItemGUID.CompareTo(y.ItemGUID);
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------------------
	//給排序用的重新索引排序權重
	private static int ResetItemPriority(ENUM_ItemType iType)
	{
		switch(iType)
		{
		case ENUM_ItemType.ENUM_ItemType_GiftBox:
			return 8;
			break;
		case ENUM_ItemType.ENUM_ItemType_Weapen:
			return 7;
			break;
		case ENUM_ItemType.ENUM_ItemType_Armor:
			return 6;
			break;
		case ENUM_ItemType.ENUM_ItemType_Material:
			return 5;
			break;
		case ENUM_ItemType.ENUM_ItemType_EXPItem:
			return 4;
			break;
		case ENUM_ItemType.ENUM_ItemType_Normal:
			return 3;
			break;
		case ENUM_ItemType.ENUM_ItemType_Money:
			return 2;
			break;
		case ENUM_ItemType.ENUM_ItemType_Pet:
			return 1;
			break;
		case ENUM_ItemType.ENUM_ItemType_PetPiece:
			return 0;
			break;
		default:
			return 0;
			break;
		}
	}
	//----------------------------------------------------------------------------------------------------
	// 暫時取消強化值高低排序
	private static int CompareItemByItemGUID(S_ItemData x, S_ItemData y)
	{
		if(x == null || y ==null)
		{
			return 0;
		}
		else
		{
			if(x.ItemGUID != y.ItemGUID)
			{
				return x.ItemGUID.CompareTo(y.ItemGUID);
			}
			else
			{
				return x.iSerial.CompareTo(y.iSerial);
			}
		}
	}

	//----------------------------------------------------------------------------------------------------
	// 暫時取消強化值高低排序
	private static int CompareItemByStrength(S_ItemData x, S_ItemData y)
	{
		if(x == null || y ==null)
		{
			return 0;
		}
		else
		{
//			if(x.ItemGUID != y.ItemGUID)
			{
				return x.ItemGUID.CompareTo(y.ItemGUID);
			}
//			else
			{
//				return x.iInherit[GameDefine.GAME_INHERIT_Streng].CompareTo(y.iInherit[GameDefine.GAME_INHERIT_Streng]);						
//				return -1;	//-1等於維持XY的順序不變 效果同return x.iSerial.CompareTo(y.iSerial);
			}
		}
	}

	//----------------------------------------------------------------------------------------------------
	// Serial ID排序
	private static int CompareItemBySerialID(S_ItemData x, S_ItemData y)
	{
		if(x == null || y ==null)
		{
			return 0;
		}
		else
		{
			if(x.iSerial != y.iSerial)
			{
				return x.iSerial.CompareTo(y.iSerial);
			}
			else
			{
//				return x.iInherit[GameDefine.GAME_INHERIT_Streng].CompareTo(y.iInherit[GameDefine.GAME_INHERIT_Streng]);
				return -1;
			}
		}
	}

	//-----------------------------------------------------------------------------------------------------
	void AddToSortItemList(S_ItemData val)
	{
		SortItemList.Add(val.iSerial);
	}

	//-----------------------------------------------------------------------------------------------------
	void AddToSortItemList(List<S_ItemData> list)
	{
/*
		for(int i =0; i<list.Count; ++i)
		{
			UnityDebugger.Debugger.Log(string.Format("=====list = {0}", list[i].iSerial));
		}
*/
		for(int i=0; i<list.Count; ++i)
		{
			SortItemList.Add(list[i].iSerial);
//			UnityDebugger.Debugger.Log("++"+SortItemList[i]+" "+ list[i].iSerial);
		}
	}

	//-----------------------------------------------------------------------------------------------------
	//顯示背包物品相對應圖片
	public void BagCollection(S_ItemData sItemData,int iIndex)
	{
		Transform item = GetItemFromPool(iIndex);
		
		if(item==null)
			return;

		if(sItemData==null)
			return;

		//擷取相對應的GUID值
		S_Item_Tmp dbffile = GameDataDB.ItemDB.GetData (sItemData.ItemGUID);
		
		if(dbffile==null)
			return;
		
		item.gameObject.SetActive(true);
		
		//指定clone的物品並賦予應顯示的物品圖案
		UIToggle EachItem = item.GetComponent<UIToggle>();

		ItemBagBlockBehavior itemBlock = item.gameObject.GetComponent<ItemBagBlockBehavior>();
		
		itemBlock.itemGUID = sItemData.ItemGUID;
		itemBlock.itemSerial = sItemData.iSerial;
		itemBlock.itemCount = sItemData.iCount;
		itemBlock.itemDataIndex = iIndex;
		
		// 設定Toggle顯示
		int SaveGroupNum = EachItem.group;
		EachItem.group = 0;
		if(Itemiserial == itemBlock.itemSerial)
		{
			BkToggle.value = false;
			EachItem.value = true;
		}
		else
		{
			//if(EachItem.value)
			BkToggle.value = true;
			EachItem.value = false;
		}
		EachItem.group = SaveGroupNum;


		//選定指定
		if(sItemData.iSerial == Itemiserial)
			EachItem.value = true;

		//更新內容
		Utility.ChangeAtlasSprite(itemBlock.Icon, dbffile.ItemIcon);			//更換物品圖案
//		dbffile.SetRareColor(itemBlock.Border , itemBlock.BG);				//層級換色
		//層級換色
		dbffile.SetItemRarity(itemBlock.Border , itemBlock.BG);				

		//更新物品堆疊數
		if(dbffile.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_Null)
		{
			itemBlock.lbCount.text = sItemData.iCount.ToString();	
		}
		else
		{
			itemBlock.lbCount.text = "";				
		}


		//更新物品強化數 
		if(sItemData.iInherit[0]>0 && dbffile.emEqPos != ENUM_ItemPosition.ENUM_ItemPosition_Null )
		{
			itemBlock.lbStrengthenNum.text = string.Format("+{0}",sItemData.iInherit[0]);	//LabelStrengthen
			itemBlock.Strengthen.alpha = 1.0f;						//SpriteLabelStrengthen
		}
		else
		{
			itemBlock.lbStrengthenNum.text = "";
			itemBlock.Strengthen.alpha = 0.0f;	
		}

		//更新吃經驗值顯示
		if(sItemData.iExp >0 || sItemData.iMeltingLV >0)
		{
			itemBlock.Up.alpha = 1.0f;
		}
		else
		{
			itemBlock.Up.alpha = 0.0f;
		}

		EventDelegate.Add (EachItem.onChange, ShowItemInfoWhenClick);					//Assign onChange事件到Toggle物件中(點到物品時中間物品資訊表也會變動
	}
	//-----------------------------------------------------------------------------------------------------
	//利用背包內每個格中有存的serialID去開啟Toggle
	/*public void SetSelectedBySerialID(long serialID)
	{
		for(int i=0;i<itemsPool.Count;++i)
		{
			if(itemsPool[i].gameObject.activeSelf == false)
				continue;

			Transform t = itemsPool[i];
			UIToggle tg = t.GetComponent<UIToggle>();
			ItemBagBlockBehavior itemBlock = t.GetComponent<ItemBagBlockBehavior>();
			if(serialID == itemBlock.itemSerial)
				tg.value = true;
		}
	}*/
	//-----------------------------------------------------------------------------------------------------
	//更新背包顯示
	public void UpdateBagContent()
	{
		NonEquipedItemNum();

		// 收集顯示物品
		SortItemDataByType();

		dataTracker.Clear();

		for(int i=0; i< itemsPool.Count; i++) 
		{
			itemsPool[i].gameObject.SetActive(false);
		}


		int j =0;
		
		// 設定資料到格子內
		S_ItemData tempItem;

		for(int i=0; i<SortItemList.Count; i++)
		{
			tempItem = GetSortItemData(i);
			
			BagCollection(tempItem,j);
			j++;
		}


	}
	//-----------------------------------------------------------------------------------------------------
	#endregion

	#region 裝備功能相關
	//-----------------------------------------------------------------------------------------------------
	//已裝備的物品資訊蒐集
	public void EquipedItemCollection()
	{
		//先清空三個佇列
		RoleEquipList.Clear();
		Pet1EquipList.Clear();
		Pet2EquipList.Clear();

		RoleEquipList2.Clear();
		Pet1EquipList2.Clear();
		Pet2EquipList2.Clear();

		int BattlePet1ID = ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID;
		int BattlePet2ID = ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID;

		//擷取相對應裝備的物品
		foreach(S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(tempItem==null)
				continue;
			
			//剔除未裝備的
			if(tempItem.emWearPos == ENUM_WearPosition.ENUM_WearPosition_None)
				continue;

			switch (tempItem.emTarget)	
			{
			case ENUM_WearTarget.ENUM_WearTarget_Self:
				RoleEquipList.Add(tempItem.emWearPos,tempItem.ItemGUID);
				RoleEquipList2.Add(tempItem.emWearPos, tempItem);
				break;
			case ENUM_WearTarget.ENUM_WearTarget_Pet:
				if(BattlePet1ID !=-1 && BattlePet1ID == tempItem.iTargetID)
				{
					Pet1EquipList.Add(tempItem.emWearPos,tempItem.ItemGUID);
					Pet1EquipList2.Add(tempItem.emWearPos, tempItem);
				}
				if(BattlePet2ID !=-1 && BattlePet2ID == tempItem.iTargetID)
				{
					Pet2EquipList.Add(tempItem.emWearPos,tempItem.ItemGUID);
					Pet2EquipList2.Add(tempItem.emWearPos, tempItem);
				}
				break;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//清掉裝備欄上所有的裝備圖案
	void InitEquipedList()
	{

		if(curTarget == Enum_Target.MainRole)
		{
			Equiped = tgRolesWeapon.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],1141);	//武器預設圖片
			Equiped = tgRolesArmor.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],1142);	//衣服預設圖片
			Equiped = tgRolesNecklace.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],1143);	//項鍊預設圖片
			Equiped = tgRolesRing.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],1144);	//戒指預設圖片
			Equiped = tgRolesMagicWeapon.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],1145);	//法寶預設圖片
			Equiped = tgRolesCosClothes.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],1147);	//時裝預設圖片
			Equiped = tgRolesCosBack.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],1146);	//背飾預設圖片
		}
		else
		{
			Equiped = tgRolesWeapon.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],2005);	//武器預設圖片
			Equiped = tgRolesArmor.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],2006);	//衣服預設圖片
			Equiped = tgRolesNecklace.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],2007);	//項鍊預設圖片
			Equiped = tgRolesRing.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],2008);	//戒指預設圖片
			Equiped = tgRolesMagicWeapon.GetComponentsInChildren<UISprite>();
			Utility.ChangeAtlasSprite(Equiped[0],1145);	//法寶預設圖片
		}
		//將裝備欄位外框顏色換回白色
		ResetEquipMask();
		//Equiped = tgRolesAmulet.GetComponentsInChildren<UISprite>();
		//Utility.ChangeAtlasSprite(Equiped[0],-1);
		//初始化所有裝備欄上的熔煉mark
		InitAllMeltingMark();

		InitEQInheritNum();
		//初始化裝備列中的篩選功能
		for(int i=0;i<bEnableKinds.Length;++i)
			bEnableKinds[i] = true;

	}
	//-----------------------------------------------------------------------------------------------------
	//清除所有裝備欄上的熔煉MARK
	private void InitAllMeltingMark()
	{
		UISprite[] EqSprites;
		if(curTarget == Enum_Target.MainRole)
		{
			EqSprites = tgRolesWeapon.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesArmor.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesNecklace.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesRing.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesMagicWeapon.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesCosClothes.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesCosBack.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
		}
		else
		{
			EqSprites = tgRolesWeapon.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesArmor.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesNecklace.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesRing.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
			EqSprites = tgRolesMagicWeapon.GetComponentsInChildren<UISprite>();
			EqSprites[5].alpha = 0.0f;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void InitEQInheritNum()
	{
		for(int i=0; i<lbEQInheritNum.Count; ++i)
		{
			lbEQInheritNum[i].text = "";
			//lbEQInheritNum[i].enabled = false;
			lbEQInheritNum[i].gameObject.SetActive(false);
			spriteEQbackground[i].gameObject.SetActive(false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void InitEQItemUpRankTip()
	{
		for(int i=0; i<btnUpRankTip.Count ;++i)
		{
			btnUpRankTip[i].gameObject.SetActive(false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//將裝備物品放至裝備欄上
//	private void SetupEquipedItems(Dictionary<ENUM_WearPosition,int> EquipList)
	private void SetupEquipedItems(Dictionary<ENUM_WearPosition, S_ItemData> EquipList)
	{
//		foreach(KeyValuePair<ENUM_WearPosition,int> re  in EquipList)
		foreach(KeyValuePair<ENUM_WearPosition, S_ItemData> re  in EquipList)
		{
			switch(re.Key)
			{
			case ENUM_WearPosition.ENUM_WearPosition_Weapon:
				Equiped = tgRolesWeapon.GetComponentsInChildren<UISprite>();
				EquipedSpriteList(Equiped,re.Value);
				bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Weapon] = false;
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Clothes:
				Equiped = tgRolesArmor.GetComponentsInChildren<UISprite>();
				EquipedSpriteList(Equiped,re.Value);
				bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Clothes] = false;
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Necklace:
				Equiped = tgRolesNecklace.GetComponentsInChildren<UISprite>();
				EquipedSpriteList(Equiped,re.Value);
				bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Necklace] = false;
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Ring:
				Equiped = tgRolesRing.GetComponentsInChildren<UISprite>();
				EquipedSpriteList(Equiped,re.Value);
				bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Ring] = false;
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Talisman:
				Equiped = tgRolesMagicWeapon.GetComponentsInChildren<UISprite>();
				EquipedSpriteList(Equiped,re.Value);
				bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Talisman] = false;
				break;
			/*case ENUM_WearPosition.ENUM_WearPosition_Amulet:
				Equiped = tgRolesAmulet.GetComponentsInChildren<UISprite>();
				EquipedSpriteList(Equiped,re.Value);
				bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Amulet] = false;
				break;*/
			case ENUM_WearPosition.ENUM_WearPosition_CosClothes:
				Equiped = tgRolesCosClothes.GetComponentsInChildren<UISprite>();
				EquipedSpriteList(Equiped,re.Value);
				bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_CosClothes] = false;
				break;
			case ENUM_WearPosition.ENUM_WearPosition_CosBack:
				Equiped = tgRolesCosBack.GetComponentsInChildren<UISprite>();
				EquipedSpriteList(Equiped,re.Value);
				bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_CosBack] = false;
				break;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetupEquipedWings()
	{
		Equiped = tgRolesCosBack.GetComponentsInChildren<UISprite>();
		EquipedSpriteList(Equiped,ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.iCosBack);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetEQEnhanceNumALL(Dictionary<ENUM_WearPosition, S_ItemData> EquipList)
	{
		//Init
		InitEQInheritNum();
	
		foreach(KeyValuePair<ENUM_WearPosition, S_ItemData> re  in EquipList)
		{
			switch(re.Key)
			{
			case ENUM_WearPosition.ENUM_WearPosition_Weapon:
				SetEQInheritNum(ENUM_WearPosition.ENUM_WearPosition_Weapon, re.Value);
				break;

			case ENUM_WearPosition.ENUM_WearPosition_Clothes:
				SetEQInheritNum(ENUM_WearPosition.ENUM_WearPosition_Clothes, re.Value);
				break;

			case ENUM_WearPosition.ENUM_WearPosition_Necklace:
				SetEQInheritNum(ENUM_WearPosition.ENUM_WearPosition_Necklace, re.Value);
				break;

			case ENUM_WearPosition.ENUM_WearPosition_Ring:
				SetEQInheritNum(ENUM_WearPosition.ENUM_WearPosition_Ring, re.Value);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Talisman:
				SetEQInheritNum(ENUM_WearPosition.ENUM_WearPosition_Talisman, re.Value);
				break;
			/*case ENUM_WearPosition.ENUM_WearPosition_Amulet:
				SetEQInheritNum(ENUM_WearPosition.ENUM_WearPosition_Amulet, re.Value);
				break;*/
			case ENUM_WearPosition.ENUM_WearPosition_CosBack:
				SetEQInheritNum(ENUM_WearPosition.ENUM_WearPosition_CosBack, re.Value);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_CosClothes:
				SetEQInheritNum(ENUM_WearPosition.ENUM_WearPosition_CosClothes, re.Value);
				break;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//取得裝備欄升階提示狀態
	private void GetEquipUpRankTip(Dictionary<ENUM_WearPosition, S_ItemData> EquipList)
	{
		foreach(KeyValuePair<ENUM_WearPosition, S_ItemData> re  in EquipList)
		{
			switch(re.Key)
			{
			case ENUM_WearPosition.ENUM_WearPosition_Weapon:
				GetItemUpRankStatus(ENUM_WearPosition.ENUM_WearPosition_Weapon, re.Value);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Clothes:
				GetItemUpRankStatus(ENUM_WearPosition.ENUM_WearPosition_Clothes, re.Value);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Necklace:
				GetItemUpRankStatus(ENUM_WearPosition.ENUM_WearPosition_Necklace, re.Value);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Ring:
				GetItemUpRankStatus(ENUM_WearPosition.ENUM_WearPosition_Ring, re.Value);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Talisman:
				GetItemUpRankStatus(ENUM_WearPosition.ENUM_WearPosition_Talisman, re.Value);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_CosBack:
				GetItemUpRankStatus(ENUM_WearPosition.ENUM_WearPosition_CosBack, re.Value);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_CosClothes:
				GetItemUpRankStatus(ENUM_WearPosition.ENUM_WearPosition_CosClothes, re.Value);
				break;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void GetItemUpRankStatus(ENUM_WearPosition type, S_ItemData itemdata)
	{
		int index = (int)type;
		if(index<0 || index>=btnUpRankTip.Count)
			return;
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(itemdata.ItemGUID);
		if(itemTmp == null)
			return;
		bool isUpRankActive = ARPGApplication.instance.CheckBlackSmithBtnIsActive(ENUM_BlackSmithButton.Button_UpRank,itemTmp,itemdata);

		if(ARPGApplication.instance.CheckItemUpRank(itemdata) && isUpRankActive)
		{
			btnUpRankTip[index].userData = itemdata;
			btnUpRankTip[index].gameObject.SetActive(true);
		}
		else
		{
			btnUpRankTip[index].gameObject.SetActive(false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void EquipedSelfState()
	{
		curTarget = Enum_Target.MainRole;	//把角色目標指給玩家

		InitEquipedList();
		InitEQItemUpRankTip();

		//當容器裡面是有東西時
		if(RoleEquipList.Count!=0)
		{
			//			SetupEquipedItems(RoleEquipList);
			SetupEquipedItems(RoleEquipList2);
			SetEQEnhanceNumALL(RoleEquipList2);
			GetEquipUpRankTip(RoleEquipList2);
		}
		SetupEquipedWings();

		CheckEmptyEquipTip();

		wgEnhanceTip.gameObject.SetActive(ARPGApplication.instance.CheckTargetEqipsUp(curTarget,false));
		//else			
			//ResetEquipMask();
	}
	//-----------------------------------------------------------------------------------------------------
	public void EquipedPet1State()
	{
		curTarget = Enum_Target.BattlePet1;	//把角色目標指給寵物1

		InitEquipedList();
		InitEQItemUpRankTip();
		//當容器裡面是有東西時
		if(Pet1EquipList.Count!=0)
		{
			//			SetupEquipedItems(Pet1EquipList);
			SetupEquipedItems(Pet1EquipList2);
			SetEQEnhanceNumALL(Pet1EquipList2);
			GetEquipUpRankTip(Pet1EquipList2);
		}
		CheckEmptyEquipTip();

		wgEnhanceTip.gameObject.SetActive(ARPGApplication.instance.CheckTargetEqipsUp(curTarget,false));
		//else
			//ResetEquipMask();
	}
	//-----------------------------------------------------------------------------------------------------
	public void EquipedPet2State()
	{
		curTarget = Enum_Target.BattlePet2;	//把角色目標指給寵物2

		InitEquipedList();
		InitEQItemUpRankTip();
		//當容器裡面是有東西時
		if(Pet2EquipList.Count!=0)
		{
			//			SetupEquipedItems(Pet2EquipList);
			SetupEquipedItems(Pet2EquipList2);
			SetEQEnhanceNumALL(Pet2EquipList2);
			GetEquipUpRankTip(Pet2EquipList2);
		}
		CheckEmptyEquipTip();

		wgEnhanceTip.gameObject.SetActive(ARPGApplication.instance.CheckTargetEqipsUp(curTarget,false));
		//else
			//ResetEquipMask();
	}
	
	//-----------------------------------------------------------------------------------------------------
	//更新升階TIP狀態
	public void UpdateTargetUpRankTip()
	{

		switch(curTarget)
		{
		case Enum_Target.MainRole:
			GetEquipUpRankTip(RoleEquipList2);
			break;
		case Enum_Target.BattlePet1:
			GetEquipUpRankTip(Pet1EquipList2);
			break;
		case Enum_Target.BattlePet2:
			GetEquipUpRankTip(Pet2EquipList2);
			break;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//取用物品DBFID找到相對應的DBF並把其中itemicon值指派給UISprite[]中的第一張圖套換
	//	void EquipedSpriteList(UISprite[] eqsp,int itemDBF)
	void EquipedSpriteList(UISprite[] eqsp, S_ItemData itemData)
	{
		S_Item_Tmp EquDBF = GameDataDB.ItemDB.GetData(itemData.ItemGUID);
//		EquDBF.SetRareColor(eqsp[4] , eqsp[6]);				//層級換色
		EquDBF.SetItemRarity(eqsp[4] , eqsp[6]);				//層級換色

		Utility.ChangeAtlasSprite(eqsp[0],EquDBF.ItemIcon);
		
		//更新吃經驗值顯示
		if(itemData.iExp >0 || itemData.iMeltingLV>0)
		{
			eqsp[5].alpha = 1.0f;
		}
		else
		{
			eqsp[5].alpha = 0.0f;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void EquipedSpriteList(UISprite[] eqsp, int itemID)
	{
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(itemID);
		if (itemTmp == null)
			return;

		itemTmp.SetItemRarity(eqsp[4] , eqsp[6]);				//層級換色
		
		Utility.ChangeAtlasSprite(eqsp[0],itemTmp.ItemIcon);
	}
	//-----------------------------------------------------------------------------------------------------
	//裝備欄位若無裝備物品時將外框顏色換回白色
	void ResetEquipMask()
	{
		Transform Mask = tgRolesWeapon.transform.FindChild("Sprite(Mask)");
		Mask.GetComponent<UISprite>().color = new Color((float)150/255, (float)150/255, (float)150/255);
		Utility.ChangeAtlasSprite(Mask.GetComponent<UISprite>(),17060);
		Mask = tgRolesArmor.transform.FindChild("Sprite(Mask)");
		Mask.GetComponent<UISprite>().color = new Color((float)150/255, (float)150/255, (float)150/255);
		Utility.ChangeAtlasSprite(Mask.GetComponent<UISprite>(),17060);
		Mask = tgRolesAmulet.transform.FindChild("Sprite(Mask)");
		Mask.GetComponent<UISprite>().color = new Color((float)150/255, (float)150/255, (float)150/255);
		Utility.ChangeAtlasSprite(Mask.GetComponent<UISprite>(),17060);
		Mask = tgRolesMagicWeapon.transform.FindChild("Sprite(Mask)");
		Mask.GetComponent<UISprite>().color = new Color((float)150/255, (float)150/255, (float)150/255);
		Utility.ChangeAtlasSprite(Mask.GetComponent<UISprite>(),17060);
		Mask = tgRolesNecklace.transform.FindChild("Sprite(Mask)");
		Mask.GetComponent<UISprite>().color = new Color((float)150/255, (float)150/255, (float)150/255);
		Utility.ChangeAtlasSprite(Mask.GetComponent<UISprite>(),17060);
		Mask = tgRolesRing.transform.FindChild("Sprite(Mask)");
		Mask.GetComponent<UISprite>().color = new Color((float)150/255, (float)150/255, (float)150/255);
		Utility.ChangeAtlasSprite(Mask.GetComponent<UISprite>(),17060);
		Mask = tgRolesCosClothes.transform.FindChild("Sprite(Mask)");
		Mask.GetComponent<UISprite>().color = new Color((float)150/255, (float)150/255, (float)150/255);
		Utility.ChangeAtlasSprite(Mask.GetComponent<UISprite>(),17060);
		Mask = tgRolesCosBack.transform.FindChild("Sprite(Mask)");
		Mask.GetComponent<UISprite>().color = new Color((float)150/255, (float)150/255, (float)150/255);
		Utility.ChangeAtlasSprite(Mask.GetComponent<UISprite>(),17060);
	}
	//-----------------------------------------------------------------------------------------------------
	void SetEQInheritNum(ENUM_WearPosition type, S_ItemData itemdata)
	{
		int index = (int)type;
		int plusval = itemdata.iInherit[GameDefine.GAME_INHERIT_Streng];

		if(plusval > 0)
		{
			lbEQInheritNum[index].gameObject.SetActive(true);
			spriteEQbackground[index].gameObject.SetActive(true);
			lbEQInheritNum[index].text = string.Format("+{0}", plusval);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//背包內物件裝備功能
	public void EquipItem()
	{
		//暫代用
		if(curTarget== Enum_Target.BattlePet1 && Pet1DBFID == -1)
		{
			panelTempPause.gameObject.SetActive(true);
			EventDelegate.Add(btnquitcur.onClick,Quitcurpage);
			return;
		}
		if(curTarget== Enum_Target.BattlePet2 && Pet2DBFID == -1)
		{
			panelTempPause.gameObject.SetActive(true);
			EventDelegate.Add(btnquitcur.onClick,Quitcurpage);
			return;
		}
		//
		if(/*ClickIndex !=-1 &&*/ Itemiserial!=0 && ItemDBFID!=-1)
		{
			//擷取相對應DBF列式
			ulong 	eIserial = Itemiserial;
			int 	eIdbfid	 = ItemDBFID;
			//防連點
			//ClickIndex = -1;
			//Itemiserial = -1;
			ItemDBFID = -1;

			if(bNUPetEquipTip == true)
				SendServerNonUsePetEquipReq(eIdbfid,eIserial);
			else
			{
				//
				S_Item_Tmp DBFforEquip = GameDataDB.ItemDB.GetData (eIdbfid);
				
				//
				switch(DBFforEquip.emEqPos)
				{
				case ENUM_ItemPosition.ENUM_ItemPosition_Weapon:				//武器
					//指定相對應裝備項目並暫存DBFID的值
					EqIcon = tgRolesWeapon.GetComponentsInChildren<UISprite>();
					tempWearPos = ENUM_WearPosition.ENUM_WearPosition_Weapon;
					break;
				case ENUM_ItemPosition.ENUM_ItemPosition_Clothes:				//防具
					//指定相對應裝備項目並暫存DBFID的值
					EqIcon = tgRolesArmor.GetComponentsInChildren<UISprite>();
					tempWearPos = ENUM_WearPosition.ENUM_WearPosition_Clothes;
					break;
				case ENUM_ItemPosition.ENUM_ItemPosition_Necklace:				//項鍊
					//指定相對應裝備項目並暫存DBFID的值
					EqIcon = tgRolesNecklace.GetComponentsInChildren<UISprite>();
					tempWearPos = ENUM_WearPosition.ENUM_WearPosition_Necklace;
					break;
				case ENUM_ItemPosition.ENUM_ItemPosition_Ring:					//戒指
					//指定相對應裝備項目並暫存DBFID的值
					EqIcon = tgRolesRing.GetComponentsInChildren<UISprite>();
					tempWearPos = ENUM_WearPosition.ENUM_WearPosition_Ring;
					break;
				case ENUM_ItemPosition.ENUM_ItemPosition_Talisman:				//法寶
					//指定相對應裝備項目並暫存DBFID的值
					EqIcon = tgRolesMagicWeapon.GetComponentsInChildren<UISprite>();
					tempWearPos = ENUM_WearPosition.ENUM_WearPosition_Talisman;
					break;
				/*case ENUM_ItemPosition.ENUM_ItemPosition_Amulet:				//護符
					//指定相對應裝備項目並暫存DBFID的值
					EqIcon = tgRolesAmulet.GetComponentsInChildren<UISprite>();
					tempWearPos = ENUM_WearPosition.ENUM_WearPosition_Amulet;
					break;*/
				case ENUM_ItemPosition.ENUM_ItemPosition_CosClothes:			//時裝
					//指定相對應裝備項目並暫存DBFID的值
					EqIcon = tgRolesCosClothes.GetComponentsInChildren<UISprite>();
					tempWearPos = ENUM_WearPosition.ENUM_WearPosition_CosClothes;
					break;
				case ENUM_ItemPosition.ENUM_ItemPosition_CosBack:				//背飾
					//指定相對應裝備項目並暫存DBFID的值
					EqIcon = tgRolesCosBack.GetComponentsInChildren<UISprite>();
					tempWearPos = ENUM_WearPosition.ENUM_WearPosition_CosBack;
					break;
				}// End Switch
				//送出request
				SendServerEquipReq(eIdbfid,eIserial);
			}//End if(bNUPetEquipTip == true)
		} // End if(ClickIndex !=-1 && Itemiserial!=-1 && ItemDBFID!=-1)
	}//End EquipItem
	//-----------------------------------------------------------------------------------------------------
	//呼叫server對未出戰寵物換裝需求並指定物品的DBFID
	void SendServerNonUsePetEquipReq(int eDBFid,ulong eiSerial)
	{
		//檢查是否為玩家專屬
		if(CheckPlayerOnlyState(eiSerial))
		{
			string str = string.Format("{0}{1}{2}", GameDataDB.GetString(1328), GameDataDB.GetString(1322), GameDataDB.GetString(1329)); //"夥伴無法使用這件裝備"
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(str);
			Itemiserial = eiSerial;
			ItemDBFID = eDBFid;
			return;
		}
		//
		if(iTargetPetID == -1)
		{
			string str = string.Format("{0}{1}{2}", GameDataDB.GetString(1328), GameDataDB.GetString(1406), GameDataDB.GetString(1329)); //"請選擇夥伴"
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(str);
			Itemiserial = eiSerial;
			ItemDBFID = eDBFid;
			return;
		}
		//判斷是否打開交換裝備開關
		CheckExchangeEqitem(eiSerial);

		JsonSlot_Item.Send_CtoM_WearItem(eiSerial.ToString(),ENUM_WearTarget.ENUM_WearTarget_Pet,iTargetPetID);

	}
	//-----------------------------------------------------------------------------------------------------
	//呼叫server換裝需求並指定物品的DBFID
	void SendServerEquipReq(int eDBFid,ulong eiSerial)
	{
		ENUM_WearTarget wearTarget = ENUM_WearTarget.ENUM_WearTarget_Null;
		int TargetID = 0;
		//呼叫server換裝需求
		switch(curTarget)
		{
		case Enum_Target.MainRole:
			wearTarget = ENUM_WearTarget.ENUM_WearTarget_Self;
			TargetID = ARPGApplication.instance.m_RoleSystem.m_RoleGUID;
			break;
		case Enum_Target.BattlePet1:
			wearTarget = ENUM_WearTarget.ENUM_WearTarget_Pet;
			TargetID = ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID;
			break;
		case Enum_Target.BattlePet2:
			wearTarget = ENUM_WearTarget.ENUM_WearTarget_Pet;
			TargetID = ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID;
			break;
		}
		if(wearTarget == ENUM_WearTarget.ENUM_WearTarget_Null)
			return;

		//檢查是否為寵物專屬
		if(CheckPetOnlyState(eiSerial))
		{
			string str = string.Format("{0}{1}{2}", GameDataDB.GetString(1328), GameDataDB.GetString(1321), GameDataDB.GetString(1329)); //"夥伴專屬"
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(str);
			Itemiserial = eiSerial;
			ItemDBFID = eDBFid;
			return;
		}
		//檢查是否為玩家專屬
		if(CheckPlayerOnlyState(eiSerial))
		{
			string str = string.Format("{0}{1}{2}", GameDataDB.GetString(1328), GameDataDB.GetString(1322), GameDataDB.GetString(1329)); //"夥伴無法使用這件裝備"
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(str);
			Itemiserial = eiSerial;
			ItemDBFID = eDBFid;
			return;
		}
		//判斷是否打開交換裝備開關
		CheckExchangeEqitem(eiSerial);

		JsonSlot_Item.Send_CtoM_WearItem(eiSerial.ToString(),wearTarget,TargetID);
		TempEqID = eDBFid;
		tempTarget = curTarget;

	}
	//-----------------------------------------------------------------------------------------------------
	//判斷是否打開交換裝備開關
	private void CheckExchangeEqitem(ulong iserialID)
	{
		S_ItemData EqItem = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(iserialID);
		if(EqItem.emTarget == ENUM_WearTarget.ENUM_WearTarget_Null)
			ExchangeEquip = true;
		else
			ExchangeEquip = false;
	}
	//-----------------------------------------------------------------------------------------------------
	//在回傳成功收到物品封包後再進行換圖的動作
	public void ChangEquipSprite()
	{
		if(TempEqID!=0)
		{
			S_Item_Tmp DBFforEquip = GameDataDB.ItemDB.GetData(TempEqID);
			//
			if(EqIcon!= null)
			{
//				DBFforEquip.SetRareColor(EqIcon[4] , EqIcon[6]);					//層級換色
				DBFforEquip.SetItemRarity(EqIcon[4] , EqIcon[6]);					//層級換色
				Utility.ChangeAtlasSprite(EqIcon[0],DBFforEquip.ItemIcon);
			}
			//
			switch(tempTarget)
			{
			case Enum_Target.MainRole:
				if(RoleEquipList.ContainsKey(tempWearPos))
					RoleEquipList[tempWearPos] = TempEqID;
				else 
					RoleEquipList.Add(tempWearPos, TempEqID);
				break;
			case Enum_Target.BattlePet1:
				if(Pet1EquipList.ContainsKey(tempWearPos))
					Pet1EquipList[tempWearPos] = TempEqID;
				else 
					Pet1EquipList.Add(tempWearPos, TempEqID);
				break;
			case Enum_Target.BattlePet2:
				if(Pet2EquipList.ContainsKey(tempWearPos))
					Pet2EquipList[tempWearPos] = TempEqID;
				else 
					Pet2EquipList.Add(tempWearPos, TempEqID);
				break;
			}
			//init
			SwitchTipBoard(false);
			NonEquipedItemNum();
		}
		//執行完換圖後要再重新蒐集一次已裝備物品集
		EquipedItemCollection();
	}
	//-----------------------------------------------------------------------------------------------------
	public void CheckEmptyEquipTip()
	{
		for(int i=0;i<spriteEmptyEqTip.Count;++i)
			spriteEmptyEqTip[i].gameObject.SetActive(false);

		//先依目標指定為主角 還是寵物
		ENUM_ItemOption iOption = ENUM_ItemOption.ENUM_ItemOption_Max;
		switch(curTarget)
		{
		case Enum_Target.MainRole:
			iOption = ENUM_ItemOption.ENUM_ItemOption_Player;
			break;
		case Enum_Target.BattlePet1:
		case Enum_Target.BattlePet2:
			iOption = ENUM_ItemOption.ENUM_ItemOption_Pet;
			break;
		}
		//如果上面沒有被指定到就直接return掉
		if(iOption == ENUM_ItemOption.ENUM_ItemOption_Max)
			return;
		//
		for(int i=0;i<bEnableKinds.Length;i++)
		{
			if(bEnableKinds[i])
			{	
				bool bShow = ShowCanEquipTip((ENUM_ItemPosition)i,iOption);
				spriteEmptyEqTip[i].gameObject.SetActive(bShow);
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private bool ShowCanEquipTip(ENUM_ItemPosition iPos,ENUM_ItemOption iOption)
	{
		foreach (S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(tempItem.emTarget == ENUM_WearTarget.ENUM_WearTarget_Null)
			{
				S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(tempItem.ItemGUID);

				if(itemTmp.emEqPos == iPos)
				{
					if(itemTmp.sItemOption.GetFlag(iOption))
						return true;
				}
			}
		}
		return false;
	}
	//-----------------------------------------------------------------------------------------------------
	//武器欄點選
	public void EquipedWeaponClick()
	{
		if(bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Weapon])
		{	
			//調動篩選功能
			tgRolesWeapon.value = true;
			EquipsClass();
		}
		//
		tgRolesWeapon.value = true;
		EquipedShowInfo(ENUM_WearPosition.ENUM_WearPosition_Weapon);
	}
	//-----------------------------------------------------------------------------------------------------
	//防具欄點選
	public void EquipedArmorClick()
	{
		if(bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Clothes])
		{
			//調動篩選功能
			tgRolesArmor.value= true;
			EquipsClass();
		}
		//
		tgRolesArmor.value= true;
		EquipedShowInfo(ENUM_WearPosition.ENUM_WearPosition_Clothes);
	}
	//-----------------------------------------------------------------------------------------------------
	//項鍊欄點選
	public void EquipedNecklaceClick()
	{	
		if(bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Necklace])
		{
			//調動篩選功能
			tgRolesNecklace.value = true;
			EquipsClass();
		}
		//
		tgRolesNecklace.value = true;
		EquipedShowInfo(ENUM_WearPosition.ENUM_WearPosition_Necklace);
	}
	//-----------------------------------------------------------------------------------------------------
	//戒指欄點選
	public void EquipedRingClick()
	{
		if(bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Ring])
		{
			//調動篩選功能
			tgRolesRing.value= true;
			EquipsClass();
		}
		//
		tgRolesRing.value= true;
		EquipedShowInfo(ENUM_WearPosition.ENUM_WearPosition_Ring);
	}
	//-----------------------------------------------------------------------------------------------------
	//法寶欄點選
	public void EquipedMagicWeaponClick()
	{
		if(bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Talisman])
		{
			//調動篩選功能
			tgRolesMagicWeapon.value = true; 
			EquipsClass();
		}
		//
		tgRolesMagicWeapon.value = true; 
		EquipedShowInfo(ENUM_WearPosition.ENUM_WearPosition_Talisman);
	}
	//-----------------------------------------------------------------------------------------------------
	/*//護符欄點選
	public void EquipedAmuletClick()
	{
		if(bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_Amulet])
		{
			//調動篩選功能
			tgRolesAmulet.value = true;
			EquipsClass ();
		}
		//
		tgRolesAmulet.value = true;
		EquipedShowInfo(ENUM_WearPosition.ENUM_WearPosition_Amulet);
	}*/
	//-----------------------------------------------------------------------------------------------------
	//時裝欄點選
	public void EquipedCosClothesClick()
	{
		if(bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_CosClothes])
		{
			//調動篩選功能
			tgRolesCosClothes.value = true;
			CosAllClass();
		}
		//
		tgRolesCosClothes.value = true;
		EquipedShowInfo(ENUM_WearPosition.ENUM_WearPosition_CosClothes);
	}
	//-----------------------------------------------------------------------------------------------------
	//背飾欄點選
	public void EquipedCosBackClick()
	{
		if(bEnableKinds[(int)ENUM_ItemPosition.ENUM_ItemPosition_CosBack])
		{
			//調動篩選功能
			tgRolesCosBack.value = true;
			CosAllClass();
		}
		//
		tgRolesCosBack.value = true;
		EquipedShowInfo(ENUM_WearPosition.ENUM_WearPosition_CosBack);
	}
	//-----------------------------------------------------------------------------------------------------
	#endregion

	#region Tip物品資訊欄相關
	//-----------------------------------------------------------------------------------------------------
	//開關物品資訊欄
	public void SwitchTipBoard(bool bSwitch)
	{
		if (bSwitch == false)
		{
			NonSelected();		//取消選擇
			ClearTipInfoData();	//清除選擇的物品資料
			NonEquipedItemNum();
			//重新更新頁面
			//ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.SetUpdateBag(true);
			bUpdateBagItemTipInfo = false;
//			if (uiBagTip.panelSmithingList.gameObject.activeSelf)
//				uiBagTip.OnBtnBlackSmithClick();
		}
		else
		{
			isTipAnimPlaying = true;
		}
		//spriteTipBoard.gameObject.SetActive(bSwitch);
		uiBagTip.gameObject.SetActive(bSwitch);
	}
	//---------------------------------------------------------------------------------------------------
	public void ClearTipInfoData()
	{
		showInfoData = null;
		ClickIndex 	= -1;
		Itemiserial = 0;		//紀錄物品序號
		ItemDBFID	= -1; 		//紀錄DBFID
		iItemCount	= -1; 		//紀錄堆疊數
		bCanSell	= false;	
		lbSellPrice.text = NonUseStr;
	}
	//-----------------------------------------------------------------------------------------------------
	public void ShowItemInfoWhenClick()
	{
		ItemBagBlockBehavior BlockData = UIToggle.current.gameObject.GetComponent<ItemBagBlockBehavior>();

		// 被點擊，就紀錄
		if(UIToggle.current.value == true)
		{
			ClickIndex 	= BlockData.itemDataIndex; 	//紀錄Index
			Itemiserial = BlockData.itemSerial;		//紀錄物品序號
			ItemDBFID	= BlockData.itemGUID;		//紀錄DBFID
			iItemCount	= BlockData.itemCount;		//紀錄堆疊數
		}

		if (!UIToggle.current.value)
			return;

		// 顯示強化數值用
		S_ItemData sitemdata = GetSortItemData(ClickIndex);
		if(sitemdata == null)
		{
//			UnityDebugger.Debugger.LogError(string.Format("ShowItemInfo S_ItemData == null itemDataIndex {0}", ClickIndex));
			return;
		}

		ShowItemInfo(sitemdata);
	}
	//-----------------------------------------------------------------------------------------------------
	//物品列中Tip顯示物品資訊
	public void ShowItemInfo(S_ItemData itemData)
	{
		if (itemData == null)
			return;

		//擷取相對應的GUID值
		S_Item_Tmp DBFforTip = GameDataDB.ItemDB.GetData (itemData.ItemGUID);
		if(DBFforTip==null)
		{
			return;
		}

		showInfoData = itemData;
		
		//道具賣價
		lbSellPrice.text = CalSellPrice(itemData,DBFforTip).ToString();						
		bCanSell = true;

		uiBagTip.SetTipInItemBag(itemData, DBFforTip, Enum_TipBoardPosition.Left);

		//打開為由背包物品更新的旗標
		bUpdateBagItemTipInfo = true;

		SwitchTipBoard((UIToggle.current != null)?UIToggle.current.value:true);
		
		//RecordStrengthUpLoc();
		ARPGApplication.instance.m_uiTopStateView.quitEnable = false;
	}
	//-----------------------------------------------------------------------------------------------------
	//裝備欄上有物品時才顯示資訊  無則隱藏提示框
	void EquipedShowInfo(ENUM_WearPosition WearPos)
	{
		//裝備欄上有物品時才顯示資訊
		switch(curTarget)
		{
		case Enum_Target.MainRole:
			if(RoleEquipList.ContainsKey(WearPos))
			{
				ShowEquipedItemInfo2(RoleEquipList2[WearPos]);
			}
			else
				SwitchTipBoard(false);
			break;
		case Enum_Target.BattlePet1:
			if(Pet1EquipList.ContainsKey(WearPos))
			{
				ShowEquipedItemInfo2(Pet1EquipList2[WearPos]);
			}
			else
				SwitchTipBoard(false);
			break;
		case Enum_Target.BattlePet2:
			if(Pet2EquipList.ContainsKey(WearPos))
			{
				ShowEquipedItemInfo2(Pet2EquipList2[WearPos]);
			}
			else
				SwitchTipBoard(false);
			break;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//裝備列中Tip顯示物品資訊
	public void ShowEquipedItemInfo2(S_ItemData itemdata)
	{
		//擷取相對應的GUID值
		S_Item_Tmp DBFforTip = GameDataDB.ItemDB.GetData(itemdata.ItemGUID);
		if(DBFforTip==null)
		{
//			UnityDebugger.Debugger.LogError(string.Format("ShowItemInfo S_Item_Tmp == null ItemDBFID {0}", ItemDBFID));
			SwitchTipBoard(false);
			return;
		}

		ClearTipInfoData();					//先清掉相關變數並把checkindex還原

		showInfoData = itemdata;
		Itemiserial = itemdata.iSerial;		//紀錄物品序號
		ItemDBFID	= itemdata.ItemGUID; 	//紀錄DBFID
		iItemCount	= itemdata.iCount; 		//紀錄堆疊數

		bool showEquip = true;
		bool updateTip = false;
		if(bNUPetEquipTip == false)
		{
			if(curTarget == Enum_Target.MainRole && 
			   DBFforTip.emEqPos != ENUM_ItemPosition.ENUM_ItemPosition_CosBack && 
			   DBFforTip.emEqPos != ENUM_ItemPosition.ENUM_ItemPosition_CosClothes)
			{
				showEquip = false;
			}
			if(bUpdateBagItemTipInfo == true)
			{
				updateTip = true;
			}
		}
		Enum_TipBoardPosition tipPos = Enum_TipBoardPosition.NoControl;
		if(bUpdateBagItemTipInfo == true && ExchangeEquip == false)
		{
			tipPos = Enum_TipBoardPosition.Left;
			bCanSell = true;
			//道具賣價
			lbSellPrice.text = CalSellPrice(itemdata,DBFforTip).ToString();
		}
		else
		{
			tipPos = Enum_TipBoardPosition.Right;
			bCanSell = false;
			//道具賣價
			lbSellPrice.text = NonUseStr;
		}

		uiBagTip.SetTipInEquipment(itemdata,
		                           DBFforTip,
		                           tipPos,
		                           bNUPetEquipTip,
		                           showEquip,
		                           updateTip);

		SwitchTipBoard(true);

		//RecordStrengthUpLoc();
		//
		ARPGApplication.instance.m_uiTopStateView.quitEnable = false;
	}
	//----------------------------------------------------------------------------------------------------
	#endregion
	//-----------------------------------------------------------------------------------------------------
	//一鍵強化
	public void OnClickAllEnhance()
	{
		//取消所有之前已選擇
		SwitchTipBoard(false);

		Dictionary<ENUM_WearPosition, S_ItemData> tempList = new Dictionary<ENUM_WearPosition, S_ItemData>();

		List<string> serials = new List<string>();

		Enum_Target target = GetCurTarget();

		if(target == Enum_Target.MainRole)
		{
			tempList = RoleEquipList2;
		}
		else if(target == Enum_Target.BattlePet1)
		{
			tempList = Pet1EquipList2;
		}
		else if(target == Enum_Target.BattlePet2)
		{
			tempList = Pet2EquipList2;
		}

		foreach(S_ItemData itemdata in tempList.Values)
		{
			serials.Add(itemdata.iSerial.ToString());
		}

		JsonSlot_Item.Send_CtoM_EqStrengthenEX(serials.ToArray());
	}
	//-----------------------------------------------------------------------------------------------------
	#region 背包出售功能相關
	//-----------------------------------------------------------------------------------------------------
	public void OpenForSaleCountPanel()
	{
		if(Itemiserial != 0 && iItemCount != -1)
		{
			//
			if(bCanSell == false)
			{
				string str = string.Format("{0}{1}{2}", GameDataDB.GetString(1328), GameDataDB.GetString(2717), GameDataDB.GetString(1329));
				ARPGApplication.instance.m_UIHUDmsg.SetMsg(str);			//"裝備物品不能販售"
				return;
			}
			//
			if(CheckDontSellState(Itemiserial))
			{
				string str = string.Format("{0}{1}{2}", GameDataDB.GetString(1328), GameDataDB.GetString(1320), GameDataDB.GetString(1329));
				ARPGApplication.instance.m_UIHUDmsg.SetMsg(str);			//"此物品不能販售"
				return;
			}

			S_ItemData itemData = GetShowingItemData();
			if(itemData == null)
			{
				string str = GameDataDB.GetString(1328)+GameDataDB.GetString(2125)+GameDataDB.GetString(1329);
				return;
			}

			//檢查是否為高階裝備
			if (ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.CheckIsHighRankEquip(itemData,true))
			{
				string str = GameDataDB.GetString(5126); //"此為[E150FF]紫色[-]以上品階或已強化、熔煉過的裝備，確定要出售？"
				if (itemData.GetEqEnhanceValue() > 0)
					str += "\n"+string.Format(GameDataDB.GetString(314),lbSellPrice.text);	//"(強化返還[00FF00]{0}[-]文錢)"
				ARPGApplication.instance.PushOkCancelBox(str,ConfirmSellItem,BackToUI,true,556,557); 
				return;
			}
			ConfirmSellItem();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void ConfirmSellItem()
	{
		if(iItemCount==1)	//當物品只有一項時 直接出售
		{
			JsonSlot_Item.Send_CtoM_SellItem(Itemiserial.ToString(),iItemCount);
		}
		else
		{
			//初始畫面與事件指定
			panelForSaleCounts.gameObject.SetActive(true);
			//出售介面字串指定
			btnQuitForSale.GetComponentInChildren<UILabel>().text 	= GameDataDB.GetString(1023); //否
			btnSendForSale.GetComponentInChildren<UILabel>().text 	= GameDataDB.GetString(1022); //是
			//事件指定
			EventDelegate.Add(btnQuitForSale.onClick, QuitForSale);						// 指定離開事件
			EventDelegate.Add(btnSendForSale.onClick, SendForSale);						// 指定送出事件
			UIEventListener.Get(btnPlus.gameObject).onPress += PlusForSaleCounter;		// 指定增加數量(按壓)
			UIEventListener.Get(btnPlus.gameObject).onClick += PlusOnClick;				// 指定增加數量(單擊)
			UIEventListener.Get(btnMinus.gameObject).onPress += MinusForSaleCounter;	// 指定減少數量(按壓)
			UIEventListener.Get(btnMinus.gameObject).onClick += MinusOnClick;			// 指定減少數量(單擊)
			//
			lbForSaleCounter.text = "1/"+iItemCount.ToString();							// 出售畫面預設顯示
			SwitchCrossModule(false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void BackToUI()
	{
		ARPGApplication.instance.PopState();
	}
	//-----------------------------------------------------------------------------------------------------
	//檢查是否為不可販售
	private bool CheckDontSellState(ulong iserialID)
	{
		S_ItemData iData =  ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(iserialID);
		S_Item_Tmp ItemTmp = GameDataDB.ItemDB.GetData(iData.ItemGUID);
		return ItemTmp.sItemOption.GetFlag(ENUM_ItemOption.ENUM_ItemOption_DontSell);
	}
	//-----------------------------------------------------------------------------------------------------
	//檢查是否為寵物專屬
	private bool CheckPetOnlyState(ulong iserialID)
	{
		if(curTarget == Enum_Target.MainRole)
		{
			S_ItemData iData =  ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(iserialID);
			S_Item_Tmp ItemTmp = GameDataDB.ItemDB.GetData(iData.ItemGUID);
			return ItemTmp.sItemOption.GetFlag(ENUM_ItemOption.ENUM_ItemOption_Pet);
		}
		return false;
	}
	//-----------------------------------------------------------------------------------------------------
	//檢查是否為玩家專屬
	private bool CheckPlayerOnlyState(ulong iserialID)
	{
		if(curTarget != Enum_Target.MainRole || bNUPetEquipTip == true)
		{
			S_ItemData iData =  ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(iserialID);
			S_Item_Tmp ItemTmp = GameDataDB.ItemDB.GetData(iData.ItemGUID);
			//先檢查是否為時裝或背飾
			if(ItemTmp.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_CosClothes || ItemTmp.emEqPos == ENUM_ItemPosition.ENUM_ItemPosition_CosBack)
				return true;

			return ItemTmp.sItemOption.GetFlag(ENUM_ItemOption.ENUM_ItemOption_Player);
		}
		return false;
	}

	//-----------------------------------------------------------------------------------------------------
	void SwitchCrossModule(bool sw)
	{
		btnSwitchState.gameObject.SetActive(sw);
		uiTopStateView.gameObject.SetActive(sw);
	}
	//-----------------------------------------------------------------------------------------------------
	public void QuitForSale()
	{
		ForSaleNum = 1;																// 數量回初始值
		panelForSaleCounts.gameObject.SetActive(false);								// 關閉出售頁面
		EventDelegate.Remove(btnQuitForSale.onClick, QuitForSale);					// 移掉離開事件
		EventDelegate.Remove(btnSendForSale.onClick, SendForSale);					// 移掉送出事件
		UIEventListener.Get (btnPlus.gameObject).onPress 	-= PlusForSaleCounter;	// 移掉增加數量(按壓)
		UIEventListener.Get(btnPlus.gameObject).onClick 	-= PlusOnClick;			// 移掉增加數量(單擊)
		UIEventListener.Get (btnMinus.gameObject).onPress 	-= MinusForSaleCounter;	// 移掉減少數量(按壓)
		UIEventListener.Get(btnMinus.gameObject).onClick 	-= MinusOnClick;		// 移掉減少數量(單擊)
		SwitchCrossModule(true);
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	private void SendForSale()
	{
		if(CheckDontSellState(Itemiserial))
		{
			string str = string.Format("{0}{1}{2}", GameDataDB.GetString(1328), GameDataDB.GetString(1320), GameDataDB.GetString(1329));
			ARPGApplication.instance.m_UIHUDmsg.SetMsg(str);			//"此物品不能販售"
		}
		else	
		{
			JsonSlot_Item.Send_CtoM_SellItem(Itemiserial.ToString(),((int)ForSaleNum));
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//販賣按鈕(增加數量)
	private void PlusForSaleCounter(GameObject gb,bool isdown)
	{
		bPlusPress = isdown;
	}
	//-----------------------------------------------------------------------------------------------------
	//販賣按鈕(減少數量)
	private void MinusForSaleCounter(GameObject gb,bool isdown)
	{
		bMinusPress = isdown;
	}
	//-----------------------------------------------------------------------------------------------------
	private void PlusOnClick(GameObject gb)
	{
		if(ForSaleNum<((float)iItemCount))
		{
			ForSaleNum += 1;
			lbForSaleCounter.text = ((int)ForSaleNum).ToString()+"/"+iItemCount.ToString();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void MinusOnClick(GameObject gb)
	{
		if(((int)ForSaleNum)>((int)MinSaleValue-1))		//要出售數量大於最低限度的出售值才能動作
		{
			ForSaleNum -= 1;
			lbForSaleCounter.text = ((int)ForSaleNum).ToString()+"/"+iItemCount.ToString();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public int CalSellPrice(S_ItemData itemData , S_Item_Tmp itemTmp)
	{
		int plusLv = itemData.GetEqEnhanceValue();
		int sellPrice = itemTmp.Sell + Mathf.CeilToInt(itemData.GetEqEnhanceMoney(plusLv) * 0.75f);

		return sellPrice;
	}
	//-----------------------------------------------------------------------------------------------------
	//TO DELETE
	public int CalDisItemReturnPrice(S_ItemData itemData)
	{
		int plusLv = itemData.GetEqEnhanceValue();
		int returnPrice = Mathf.CeilToInt(itemData.GetEqEnhanceMoney(plusLv) * 0.75f);
		
		return returnPrice;
	}
	//-----------------------------------------------------------------------------------------------------
	#endregion

	#region 分頁功能相關
	//-----------------------------------------------------------------------------------------------------
	//全部分頁
	public void AllClass()
	{
		if(emSortPage != Enum_ItemClass.All)
		{
			tgAll.value = true;
			emSortPage = Enum_ItemClass.All; 	//指定類別
			UpdateWithBegin1();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//裝備分頁
	public void EquipsClass()
	{
		if(emSortPage != Enum_ItemClass.Equips)
		{
			tgEquips.value = true;
			emSortPage = Enum_ItemClass.Equips;
			UpdateWithBegin1();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//時裝分頁
	void CosAllClass()
	{
		if(emSortPage != Enum_ItemClass.CosAll)
		{
			tgCosAll.value = true;
			emSortPage = Enum_ItemClass.CosAll;
			UpdateWithBegin1();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//其他分頁
	void OtherClass()
	{
		if(emSortPage != Enum_ItemClass.NonEquip)
		{
			tgOther.value = true;
			emSortPage = Enum_ItemClass.NonEquip;
			UpdateWithBegin1();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void NonSelected()
	{
		//取消物品選擇
		ClickIndex = -1;
		BkToggle.value = true;
		iItemCount = -1;
		Itemiserial = 0;
	}
	//-----------------------------------------------------------------------------------------------------
	void UpdateWithBegin1()
	{
		SwitchTipBoard(false);
		//蒐集資料
//		SortItemData();							
		SortItemDataByType();
		//顯示資訊
		CreateItemListView(SortItemList.Count);	
		BkToggle.value = true;
	}
	//-----------------------------------------------------------------------------------------------------
	#endregion

	#region 角色狀態相關function
	//-----------------------------------------------------------------------------------------------------
	public void OpenRoleStateTableView()
	{
		if(!InfoBoard.gameObject.activeSelf)
		{
			lbSwitchState.text	= GameDataDB.GetString(1007); //裝備
			//InvalidCrossModule(false);
			float timer = 0.5f;				//轉場時間
			//bCurTipState = spriteTipBoard.gameObject.activeSelf;	//儲存目前Tip的顯示狀態
			//隱藏不必要出現的物件
			spriteItemSwitchSet.gameObject.SetActive(false);	//隱藏分頁
			spriteBagBackGround.gameObject.SetActive(false);	//隱藏物品列表與相關功能鈕
			SwitchTipBoard (false);								//隱藏提示板
			btnAllEnhance.gameObject.SetActive (false);		//隱藏一鍵強化
			//btnSpeedEquip.gameObject.SetActive(false);			//隱藏一鍵換裝
			//顯示角色資訊列表
			InfoBoard.gameObject.SetActive (true);
			//星等初始化
			if(StateStarSet.Count == 0)
			{
				StateStarSet.Add(spriteStateStar1);
				StateStarSet.Add(spriteStateStar2);
				StateStarSet.Add(spriteStateStar3);
				StateStarSet.Add(spriteStateStar4);
				StateStarSet.Add(spriteStateStar5);
				StateStarSet.Add(spriteStateStar6);
				StateStarSet.Add(spriteStateStar7);
				for(int i=0;i<StateStarSet.Count;++i)
					StateStarSet[i].gameObject.SetActive(false);
			}
			//字串指定
			//標題
			lbExpTitle.text			= GameDataDB.GetString(1026);	//經驗值
			/*
			lbHPTitle.text			= GameDataDB.GetString(1012);	//生命力
			lbAttackTitle.text		= GameDataDB.GetString(1028);	//攻擊力
			lbAttackSpeedTitle.text = GameDataDB.GetString(1029);	//攻速
			lbDefenseTitle.text		= GameDataDB.GetString(1030);	//防禦力*/
			//數值載入
			switch(curTarget)
			{
			case Enum_Target.MainRole:
				SetRoleDataToState();
				break;
			case Enum_Target.BattlePet1:
				SetPetDataToState(Enum_Target.BattlePet1);
				break;
			case Enum_Target.BattlePet2:
				SetPetDataToState(Enum_Target.BattlePet2);
				break;
			}

			//移入效果
			TweenPosition movePos = panelInfoTable.gameObject.AddComponent<TweenPosition>();
			movePos.from = new Vector3(1500,-30,0);
			movePos.to = new Vector3(335,-30,0);
			movePos.duration = timer;
			Invoke ("RemoveTweenPos",timer);
			//
			ARPGApplication.instance.m_uiTopStateView.quitEnable = false;
		}
		else
		{
			lbSwitchState.text	= GameDataDB.GetString(1031); //狀態
			//移出角色資訊列表
			float timer = 0.5f;			//轉場時間
			TweenPosition movePos = panelInfoTable.gameObject.AddComponent<TweenPosition>();
			movePos.from = new Vector3(335,-30,0);
			movePos.to = new Vector3(1500,-30,0);
			movePos.duration = timer;
			//等移出後再執行恢復顯示物件
			Invoke ("DelayBackToBag",timer);
			//
			ARPGApplication.instance.m_uiTopStateView.quitEnable = true;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void RemoveTweenPos()
	{
		if(panelInfoTable.GetComponent<TweenPosition>()==null)
			return;
		Destroy(panelInfoTable.GetComponent<TweenPosition>());
	}
	//-----------------------------------------------------------------------------------------------------
	private void ResetTweenPosUse(GameObject gb)
	{
		TweenPosition twpos = gb.GetComponent<TweenPosition>();
		if(twpos==null)
			return;

		if (twpos.onFinished != null)
			twpos.onFinished.Clear();

		if(!twpos.isActiveAndEnabled)
		{
			twpos.ResetToBeginning();
			twpos.PlayForward();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void DelayBackToBag()
	{
		//
		//SwitchTipBoard(bCurTipState);
		//
		spriteItemSwitchSet.gameObject.SetActive(true);		//恢復顯示分頁
		spriteBagBackGround.gameObject.SetActive(true);		//恢復顯示物品列表與相關功能鈕
		btnAllEnhance.gameObject.SetActive (true);		//恢復顯示一鍵強化
		//btnSpeedEquip.gameObject.SetActive (true);			//恢復顯示一鍵換裝
		RemoveTweenPos();									//移掉效果
		InfoBoard.gameObject.SetActive (false);	//隱藏角色狀態表
		//InvalidCrossModule(true);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetRoleDataToState()
	{
		//HideAllStateStars (); //隱藏星等顯示
		gPetBreakLimit.gameObject.SetActive(false);	//關閉突破數
		//取得主玩家GameObjectID
		//ARPGCharacter character = ARPGApplication.instance.m_tempGameObjectSystem.GetARPGCharacterByGameID(MainGbID);
		//
		int currentLevel = ARPGApplication.instance.m_RoleSystem.iBaseLevel;	//等級為common角色表中的GUID值
		S_PlayerData_Tmp RoleDBF = GameDataDB.PlayerDB.GetData(currentLevel);
		//
		//character.CalRoleAttrTable(RoleDBF);

		lbStateRoleName.text 	= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName;						//角色名稱
		containerType.gameObject.SetActive(false);
		lbLevelValue.text 		= GameDataDB.GetString(1056)+" [FFFF00]"+currentLevel.ToString()+"[-]";						//等級
		lbExpValue.text 		= ARPGApplication.instance.m_RoleSystem.iBaseExp.ToString()+"/"+RoleDBF.iMaxExp.ToString();	//經驗值
		float expRatio 			= (float)ARPGApplication.instance.m_RoleSystem.iBaseExp / RoleDBF.iMaxExp;
		if (expRatio > 1) expRatio = 1;
		barRoleExp.value 		= expRatio;
		//
		int iTargetID = ARPGApplication.instance.m_RoleSystem.m_RoleGUID;
		S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Self,iTargetID);
		S_ItemData[] AllEQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems();
		S_FormationData formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
		//
		S_RoleAttr RoleAttr = ARPGCharacter.CreatePlayerRoleAttr(currentLevel,
		                                                         EQList,
		                                                         ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData,
		                                                         ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.SkillDataList,
		                                                         null,
		                                                         formationData,
		                                                         AllEQList,
		                                                         ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.WingDataList);
		
		//角色類型
		for(int i=0; i < EQList.Length; ++i)
		{
			if (EQList[i] == null)
				continue;
			if(EQList[i].emWearPos == ENUM_WearPosition.ENUM_WearPosition_Weapon)
			{
				S_Item_Tmp weaponTmp = GameDataDB.ItemDB.GetData( EQList[i].ItemGUID );
				if (weaponTmp != null)
				{
					switch(weaponTmp.emWeaponType)
					{
					case ENUM_WeaponType.ENUM_WeaponType_Pike:
						lbRoleType.text = "[FF8C28]"+GameDataDB.GetString(1082)+" "+GameDataDB.GetString(1074)+"-"+GameDataDB.GetString(1076)+"[-]";
						break;
					case ENUM_WeaponType.ENUM_WeaponType_Sword:
						lbRoleType.text = "[FF8C28]"+GameDataDB.GetString(1082)+" "+GameDataDB.GetString(1074)+"-"+GameDataDB.GetString(1077)+"[-]";
						break;
					case ENUM_WeaponType.ENUM_WeaponType_Bow:
						lbRoleType.text = "[00AAFF]"+GameDataDB.GetString(1082)+" "+GameDataDB.GetString(1075)+"-"+GameDataDB.GetString(1078)+"[-]";
						break;
					default:
						lbRoleType.text = "";
						break;
					}
				}
			}
		}

		//顯示整數
		lbHPValue.text			= "[FFFF00]"+(Math.Round(RoleAttr.sBattleFinial.fMaxHP,			0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//生命
		lbADValue.text			= "[FFFF00]"+(Math.Round(RoleAttr.sBattleFinial.fAttack,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理攻擊力
		lbArmorValue.text		= "[FFFF00]"+(Math.Round(RoleAttr.sBattleFinial.fDefense,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理防禦力
		lbAPValue.text			= "[FFFF00]"+(Math.Round(RoleAttr.sBattleFinial.fAbilityPower,	0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能攻擊力
		lbMRValue.text			= "[FFFF00]"+(Math.Round(RoleAttr.sBattleFinial.fMagicResist,	0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能防禦力
		lbCriricalValue.text	= "[FFFF00]"+(Math.Round(RoleAttr.sBattleFinial.fCritsRate,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//爆擊
		lbTenacityValue.text	= "[FFFF00]"+(Math.Round(RoleAttr.sBattleFinial.fTenacity,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//韌性
		double roleMRP = RoleAttr.sBattleFinial.fMagicResistPen;
		roleMRP = roleMRP * Mathf.Max(0.05f,(float)(1+RoleAttr.sBattleFinial.fMagicResistPen_Per));
		lbMRPValue.text			= "[FFFF00]"+(Math.Round(roleMRP,								0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能穿透	
		double roleARP = RoleAttr.sBattleFinial.fArmorPen;
		roleARP = roleARP * Mathf.Max(0.05f,(float)(1+RoleAttr.sBattleFinial.fArmorPen_Per));
		lbARPValue.text			= "[FFFF00]"+(Math.Round(roleARP,								0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理穿透
		//顯示"XXX%"且小數點後兩位																																	   	
		lbAttackSpeedValue.text	= "[FFFF00]"+(100.0f * Math.Round(RoleAttr.sBattleFinial.fAtkSpeed,			4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";	//攻速
		lbCriticalDmgValue.text	= "[FFFF00]"+(100.0f * Math.Round(RoleAttr.sBattleFinial.fCriticalDamage,	4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";	//爆傷
		lbMoveSpeedValue.text	= "[FFFF00]"+(100.0f * Math.Round(RoleAttr.sBattleFinial.fMoveSpeed_Per,	4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";	//跑速
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetPetDataToState(Enum_Target bPet)
	{
		switch(bPet)
		{
		case Enum_Target.BattlePet1:
			if(ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID == -1)
				SetEmptyStateInfo();
			else
				SetPetStateInfo(Pet1Data,bPet);
			break;
		case Enum_Target.BattlePet2:
			if(ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID == -1)
				SetEmptyStateInfo();
			else
				SetPetStateInfo(Pet2Data,bPet);
			break;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void SetPetStateInfo(S_PetData pd,Enum_Target pType)
	{
		S_PetData_Tmp PetDBF = GameDataDB.PetDB.GetData (pd.iPetDBFID);
		S_PetLevelUp_Tmp PetLevel = GameDataDB.PetLevelUpDB.GetData(pd.iPetLevel);
		//突破數顯示
		gPetBreakLimit.gameObject.SetActive(true);	//開啟突破數
		for(int i=0;i<spBreakArray.Length;++i)
		{
			if(i<pd.iPetLimitLevel)
				spBreakArray[i].gameObject.SetActive(true);
			else
				spBreakArray[i].gameObject.SetActive(false);
		}
		//面板上方屬性
		lbStateRoleName.text	= GameDataDB.GetString(PetDBF.iName);												//寵物名稱
		//
		containerType.gameObject.SetActive(true);
		lbTypeName.text			= GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(PetDBF.emCharType));
		Utility.ChangeAtlasSprite(spCalss,ARPGApplication.instance.GetPetCalssIconID(PetDBF.emCharClass));
		//PetDBF.SetRareColor(lbStateRoleName);
		lbLevelValue.text 		= GameDataDB.GetString(1056)+"[FFFF00]"+pd.iPetLevel.ToString()+"[-]";				//寵物等級
		lbExpValue.text 		= pd.iPetExp.ToString()+"/"+PetLevel.iExpRank[PetDBF.iRank-1].ToString();			//寵物經驗值
		float expRatio 			= (float)pd.iPetExp / PetLevel.iExpRank[PetDBF.iRank-1];
		if (expRatio > 1) expRatio = 1;
		barRoleExp.value 		= expRatio;
		//取得寵物角色資料
		S_FormationData formationData = ARPGApplication.instance.m_RoleSystem.StartUpFormation;
		S_RoleAttr PetRoleAttr;
		int BattlePetID = 0;
		if(pType == Enum_Target.BattlePet1)
			BattlePetID = ARPGApplication.instance.m_RoleSystem.iBattlePet1DBFID;
		else if(pType == Enum_Target.BattlePet2)
			BattlePetID = ARPGApplication.instance.m_RoleSystem.iBattlePet2DBFID;

		if(BattlePetID == 0)
			UnityDebugger.Debugger.Log("This BattlePetID is Error"+ BattlePetID.ToString() );

		S_ItemData[] EQList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems(ENUM_WearTarget.ENUM_WearTarget_Pet,BattlePetID);
		S_ItemData[] AllItemList = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetAllEqItems();
		PetRoleAttr = ARPGCharacter.CreatePetRoleAttr(pd,EQList,ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData,pd.GetTalentSkill(),null,formationData,AllItemList);
		//角色類型
		if (PetDBF.sAttrTable.fAtkDistance > 5)
			lbRoleType.text = "[00AAFF]"+GameDataDB.GetString(1082)+" "+GameDataDB.GetString(1075)+"-";
		else
			lbRoleType.text = "[FF8C28]"+GameDataDB.GetString(1082)+" "+GameDataDB.GetString(1074)+"-";
		switch(PetDBF.emType)
		{
		case ENUM_PET_TYPE.ENUM_PET_TYPE_ATTACK:
			lbRoleType.text += GameDataDB.GetString(1079)+"[-]";	//司戰
			break;
		case ENUM_PET_TYPE.ENUM_PET_TYPE_DEFEND:
			lbRoleType.text += GameDataDB.GetString(1081)+"[-]";	//持援
			break;
		case ENUM_PET_TYPE.ENUM_PET_TYPE_ASSIST:
			lbRoleType.text += GameDataDB.GetString(1080)+"[-]";	//利兵
			break;
		case ENUM_PET_TYPE.ENUM_PET_TYPE_PROTECTED:
		case ENUM_PET_TYPE.ENUM_PET_TYPE_STRONGPOINT:
			break;
		}
		//顯示整數
		lbHPValue.text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fMaxHP,			0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//生命
		lbADValue.text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fAttack,			0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理攻擊力
		lbArmorValue.text		= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fDefense,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理防禦力
		lbAPValue.text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fAbilityPower,	0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能攻擊力
		lbMRValue.text			= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fMagicResist,	0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能防禦力
		lbCriricalValue.text	= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fCritsRate,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//爆擊
		lbTenacityValue.text	= "[FFFF00]"+(Math.Round(PetRoleAttr.sBattleFinial.fTenacity,		0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//韌性
		double petMRP = PetRoleAttr.sBattleFinial.fMagicResistPen;
		petMRP = petMRP * Mathf.Max(0.05f,(float)(1+PetRoleAttr.sBattleFinial.fMagicResistPen_Per));
		lbMRPValue.text			= "[FFFF00]"+(Math.Round(petMRP,									0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//技能穿透
		double petARP = PetRoleAttr.sBattleFinial.fArmorPen;
		petARP = petARP * Mathf.Max(0.05f,(float)(1+PetRoleAttr.sBattleFinial.fArmorPen_Per));
		lbARPValue.text			= "[FFFF00]"+(Math.Round(petARP,									0,MidpointRounding.AwayFromZero)).ToString()+"[-]";					//物理穿透
		//顯示"XXX%"且小數點後兩位																																	   	
		lbAttackSpeedValue.text	= "[FFFF00]"+(100.0f * Math.Round(PetRoleAttr.sBattleFinial.fAtkSpeed,		4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";		//攻速
		lbCriticalDmgValue.text	= "[FFFF00]"+(100.0f * Math.Round(PetRoleAttr.sBattleFinial.fCriticalDamage,4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";		//爆傷
		lbMoveSpeedValue.text	= "[FFFF00]"+(100.0f * Math.Round(PetRoleAttr.sBattleFinial.fMoveSpeed_Per,	4,MidpointRounding.AwayFromZero)).ToString()+"%[-]";		//跑速
	}
	//-----------------------------------------------------------------------------------------------------
	void SetEmptyStateInfo()
	{
		//
		HideAllStateStars ();
		lbStateRoleName.text	= "未指派";		//寵物名稱
		lbLevelValue.text 		= "";			//寵物等級
		lbExpValue.text 		= "0/0";		//寵物經驗值
		lbHPValue.text			= "";			//寵物生命
		lbADValue.text			= "";			//攻擊力
		lbAttackSpeedValue.text	= "";			//攻速
		lbArmorValue.text		= "";			//防禦力
	}
	//-----------------------------------------------------------------------------------------------------
	public void ChangeToPetNameInBagUI(int i)
	{
		if(i==0)
		{
			if(Pet1DBFID!=-1)
			{
				S_PetData_Tmp PetDBF = GameDataDB.PetDB.GetData (Pet1DBFID);
				lbRolesName.text = GameDataDB.GetString(PetDBF.iName);							//寵物1名稱
				//PetDBF.SetRareColor(lbRolesName);
			}
			else
			{
				lbRolesName.text = "未指派寵物";
			}
		}
		else if(i==1)
		{
			if(Pet2DBFID!=-1)
			{
				S_PetData_Tmp PetDBF = GameDataDB.PetDB.GetData (Pet2DBFID);
				lbRolesName.text = GameDataDB.GetString(PetDBF.iName);							//寵物2名稱
				//PetDBF.SetRareColor(lbRolesName);
			}
			else
			{
				lbRolesName.text = "未指派寵物";
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//將星等先隱藏起來
	void  HideAllStateStars()
	{
		for(int i=0;i<StateStarSet.Count;++i)
		{
			StateStarSet[i].gameObject.SetActive(false);
		}
	}
	#endregion
	//暫時阻止畫面(暫代用)
	void Quitcurpage() 
	{
		panelTempPause.gameObject.SetActive(false);
		EventDelegate.Remove(btnquitcur.onClick,Quitcurpage);
	}

	//-----------------------------------------------------------------------------------------------------
	public Enum_Target GetCurTarget()
	{
		return curTarget;
	}
	//-----------------------------------------------------------------------------------------------------
	public void RefreshEQEnhance()
	{
		Enum_Target target = GetCurTarget();

		switch(target)
		{
		case Enum_Target.MainRole:
			EquipedSelfState();
			break;
		case Enum_Target.BattlePet1:
			EquipedPet1State();
			break;
		case Enum_Target.BattlePet2:
			EquipedPet2State();
			break;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void RefreshEquipedItemInfo2(ulong serialID)
	{
		S_ItemData tempdata = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(serialID);

		if(tempdata == null)
		{
//			UnityDebugger.Debugger.LogError(string.Format("RefreshEquipedItemInfo2 Error: serialID {0}", serialID));
			SwitchTipBoard(false);
		}
		else if (tempdata.emWearPos == ENUM_WearPosition.ENUM_WearPosition_None || tempdata.emTarget == ENUM_WearTarget.ENUM_WearTarget_Null)
			ShowItemInfo(tempdata);
		else
			ShowEquipedItemInfo2(tempdata);
		}
	//-----------------------------------------------------------------------------------------------------
	public S_ItemData GetShowingItemData()
	{
		return showInfoData;
	}
	//-----------------------------------------------------------------------------------------------------
	public ulong GetShowingItemSerialID()
	{
		return Itemiserial;
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	public void OnMeltingClick()
	{
		ulong serialID = GetShowingItemSerialID();
		if(serialID <= 0)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnClickUpRank() serialID == {0}", serialID));
			return;
		}

		S_ItemData data = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(serialID);
		if(data == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnClickUpRank() data == {0}", data));
			return;
		}

//		S_ItemFormula_Tmp formuladata = GameDataDB.ItemFormulaDB.GetData(data.ItemGUID);
//		if(formuladata == null)
//		{
//			UnityDebugger.Debugger.LogError(string.Format(" OnClickUpRank() formuladata == {0}", formuladata));
//			return;
//		}

		//取出現在的UI狀態
		EquipMeltingState upstarState = ARPGApplication.instance.GetGameStateByName(GameDefine.EQUIPMELTING_STATE) as EquipMeltingState;

		if(upstarState == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnClickUpRank() upstarState == {0}", upstarState));
			return;
		}

		upstarState.SetSelectSerialID(serialID);
		upstarState.SetSelectItemData(data);

		ARPGApplication.instance.PushState(GameDefine.EQUIPMELTING_STATE);
	}
	//-----------------------------------------------------------------------------------------------------
	public void OnFusionOrUprankClick(GameObject go)
	{
		ulong serialID = GetShowingItemSerialID();
		if(serialID <= 0)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnFusionOrUpRankClick() serialID == {0}", serialID));
			return;
		}
		
		S_ItemData data = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(serialID);
		if(data == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnFusionOrUpRankClick() data == {0}", data));
			return;
		}
		
		//取出現在的UI狀態
		FusionState fusionState = ARPGApplication.instance.GetGameStateByName(GameDefine.FUSION_STATE) as FusionState;
		
		if(fusionState == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnFusionOrUpRankClick() fusionState == {0}", fusionState));
			return;
		}
		
		//fusionState.SetCurrentItemSerial(serialID);
		fusionState.SetCurrentItemData(data);

		if (go == uiBagTip.btnFusion.gameObject)
			fusionState.m_UI_Mode = ENUM_UI_Mode.ENUM_Fusion;
		else if (go == uiBagTip.btnUpRank.gameObject)
			fusionState.m_UI_Mode = ENUM_UI_Mode.ENUM_UpRank;

		ARPGApplication.instance.PushState(GameDefine.FUSION_STATE);
	}
	//-----------------------------------------------------------------------------------------------------
	public void OnPurificationClick(GameObject go)
	{
		ulong serialID = GetShowingItemSerialID();
		if(serialID <= 0)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnPurificationClick() serialID == {0}", serialID));
			return;
		}
		
		S_ItemData data = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(serialID);
		if(data == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnPurificationClick() data == {0}", data));
			return;
		}
		
		//取出現在的UI狀態
		PurificationState purState = ARPGApplication.instance.GetGameStateByName(GameDefine.PURIFICATION_STATE) as PurificationState;
		
		if(purState == null)
		{
			UnityDebugger.Debugger.LogError(string.Format(" OnPurificationClick() purState == {0}", purState));
			return;
		}

		purState.SetCurrentItemData(data);
		
		ARPGApplication.instance.PushState(GameDefine.PURIFICATION_STATE);
	}
	//------------------------------------------------------------------------------------------------
/*	public void ChangEquipment()
	{
		//如果是在背包UI的狀態下才執行
		if(ARPGApplication.instance.CheckCurrentGameStates(GameDefine.ITEMBAG_STATE) == true)
		{
			//取出現在的UI狀態再去執行換圖
			ItemBagState itembagState = (ItemBagState)ARPGApplication.instance.GetGameStateByName(GameDefine.ITEMBAG_STATE);
			//執行2D裝備欄換圖
			ChangEquipSprite();
			//更新背包內容
			ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.SetUpdateBag(true);

			EquipedItemCollection();
			RefreshEQEnhance();
			//執行3D人物換裝
			if( tempTarget == ENUM_WearTarget.ENUM_WearTarget_Self 
			   &&(tempWearPos == ENUM_WearPosition.ENUM_WearPosition_Weapon || tempWearPos == ENUM_WearPosition.ENUM_WearPosition_Clothes)		
			   )
			{
				itembagState.RapidChangeEqipedMainRole();
			}
				
			//保留Tip指向剛裝好的裝備上
			switch(tempWearPos)
			{
			case ENUM_WearPosition.ENUM_WearPosition_Weapon:
				tgRolesWeapon.value=true;
				EquipedWeaponClick();
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Clothes:
				tgRolesArmor.value=true;
				EquipedArmorClick();
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Necklace:
				tgRolesNecklace.value=true;
				EquipedNecklaceClick();
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Ring:
				tgRolesRing.value=true;
				EquipedRingClick();
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Talisman:
				tgRolesMagicWeapon.value=true;
				EquipedMagicWeaponClick();
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Amulet:
				tgRolesAmulet.value=true;
				EquipedAmuletClick();
				break;
			}
		}
	}
*/
	//------------------------------------------------------------------------------------------------
	//裝備欄的強化特效
	public void ActiveUpEffect(ulong serialID)
	{
		if(ClickIndex==-1)
		{
			S_ItemData sitemdata = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(serialID);
			ENUM_WearPosition WearPos = sitemdata.emWearPos;
			int WearNum = -1;
			switch(WearPos)
			{
			case ENUM_WearPosition.ENUM_WearPosition_Weapon:
				WearNum = (int)ENUM_WearPosition.ENUM_WearPosition_Weapon;
				EffectAndSound(spriteUpEffect[WearNum].gameObject);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Clothes:
				WearNum = (int)ENUM_WearPosition.ENUM_WearPosition_Clothes;
				EffectAndSound(spriteUpEffect[WearNum].gameObject);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Necklace:
				WearNum = (int)ENUM_WearPosition.ENUM_WearPosition_Necklace;
				EffectAndSound(spriteUpEffect[WearNum].gameObject);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Ring:
				WearNum = (int)ENUM_WearPosition.ENUM_WearPosition_Ring;
				EffectAndSound(spriteUpEffect[WearNum].gameObject);
				break;
			case ENUM_WearPosition.ENUM_WearPosition_Talisman:
				WearNum = (int)ENUM_WearPosition.ENUM_WearPosition_Talisman;
				EffectAndSound(spriteUpEffect[WearNum].gameObject);
				break;
			default:
				UnityDebugger.Debugger.Log("No this Item in EquipList");
				//音效
				MusicControlSystem.PlaySound("Sound_System_022",1);
				break;
			}
		}
		else
		{
			GameObject UpGradeItem;
			UpGradeItem = NGUITools.AddChild(draggablePanel.gameObject,panelUpEffect.gameObject); //將特效的Prefab加到非裝備的物品欄中
			UpGradeItem.transform.position = itemsPool[ClickIndex].position;
			EffectAndSound(UpGradeItem);
		}
	}
	//------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------
	//強化特效+音效
	void EffectAndSound(GameObject gb)
	{
		Keyframe[] ks = new Keyframe[6];
		//特效曲線
		ks[0] = new Keyframe(0f, 0f, 0f, 1f);
		ks[1] = new Keyframe(0.05f, 0.3f, 0f, 1f);
		ks[2] = new Keyframe(0.2f, 0.6f, 0f, 1f);
		ks[3] = new Keyframe(0.5f, 0.9f, 0f, 1f);
		ks[4] = new Keyframe(0.7f, 0.95f, 0f, 1f);
		ks[5] = new Keyframe(1f, 1f, 1f, 0f);
		//
		FadeOut FadeoutEffect = gb.AddComponent<FadeOut>();
		FadeoutEffect.duration = 1.5f;
		FadeoutEffect.m_animationCurve = new AnimationCurve(ks);
		//音效
		MusicControlSystem.PlaySound("Sound_System_022",1);
	}
	//------------------------------------------------------------------------------------------------
	public string GetEquipEnhanceResult(ulong serialID)
	{
		string str = "";

		S_ItemData sitemdata = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(serialID);

		if(sitemdata == null)
		{
			return str;
		}
		int plusLV = sitemdata.iInherit[GameDefine.GAME_INHERIT_Streng];

		S_Item_Tmp sItemTemp = GameDataDB.ItemDB.GetData(sitemdata.ItemGUID);

		if(sItemTemp == null)
		{
			return str;
		}

		int emType = (int)sitemdata.GetEqStrengthenType();

		S_EqStrengthen_Tmp stTemp = GameDataDB.EqStrengthenDB.GetData(plusLV);

		if(stTemp == null)
		{
			return str;
		}

		int plusValue = stTemp.UpStrengthening_Value[emType];

		str = sItemTemp.GetEquipMainAttrString(false)+" +"+plusValue;

		return str;
	}

	//------------------------------------------------------------------------------------------------
	public string GetEqStrengthenMoney(ulong serialID)
	{
		string str = "";
		
		S_ItemData sitemdata = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemBagDataBySerial(serialID);
		
		if(sitemdata == null)
		{
			return str;
		}
		int plusLV = sitemdata.iInherit[GameDefine.GAME_INHERIT_Streng];
		
		S_Item_Tmp sItemTemp = GameDataDB.ItemDB.GetData(sitemdata.ItemGUID);
		
		if(sItemTemp == null)
		{
			return str;
		}
		
//		int emType = sItemTemp.GetEQTypeForStrengthen();
		int emType = (int)sitemdata.GetEqStrengthenType();

		S_EqStrengthen_Tmp stTemp = GameDataDB.EqStrengthenDB.GetData(plusLV);
		
		if(stTemp == null)
		{
			return str;
		}

		int plusMoney = stTemp.UpStrengthening_Price[emType];

		str = string.Format("{0} {1} {2}",GameDataDB.GetString(1334), plusMoney, GameDataDB.GetString(1335));	//花費 1334 元1335

		return str;
	}

	//------------------------------------------------------------------------------------------------
	public void RefreshEQIconList()
	{
		//執行2D裝備欄換圖
		ChangEquipSprite();
		//更新背包內容
		ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.SetUpdateBag(true);

		//收集裝備
		EquipedItemCollection();
		//更新裝備欄的強化數值
		RefreshEQEnhance();
			
		//保留Tip指向剛裝好的裝備上
		switch(tempWearPos)
		{
		case ENUM_WearPosition.ENUM_WearPosition_Weapon:
			tgRolesWeapon.value=true;
			EquipedWeaponClick();
			break;
		case ENUM_WearPosition.ENUM_WearPosition_Clothes:
			tgRolesArmor.value=true;
			EquipedArmorClick();
			break;
		case ENUM_WearPosition.ENUM_WearPosition_Necklace:
			tgRolesNecklace.value=true;
			EquipedNecklaceClick();
			break;
		case ENUM_WearPosition.ENUM_WearPosition_Ring:
			tgRolesRing.value=true;
			EquipedRingClick();
			break;
		case ENUM_WearPosition.ENUM_WearPosition_Talisman:
			tgRolesMagicWeapon.value=true;
			EquipedMagicWeaponClick();
			break;
		/*case ENUM_WearPosition.ENUM_WearPosition_Amulet:
			tgRolesAmulet.value=true;
			EquipedAmuletClick();
			break;*/
		case ENUM_WearPosition.ENUM_WearPosition_CosClothes:
			tgRolesCosClothes.value=true;
			EquipedCosClothesClick();
			break;
		case ENUM_WearPosition.ENUM_WearPosition_CosBack:
//			tgRolesCosBack.value=true;
//			EquipedCosBackClick();
			break;
		}

		if (tempTarget == Enum_Target.MainRole)
		{
			ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.UpdateCalPlayerAttribute();
		}
	}
	//----------------------------------------------------------------------------------------------
//	public void SetTempWearPos(ENUM_WearPosition type)
//	{
//		tempWearPos = type;
//	}

	//----------------------------------------------------------------------------------------------
	//利用物品的serialID去找到背包中相對應的index值
	public int GetSortItemIndex(ulong serialID)
	{
		if(serialID < 0 || !SortItemList.Contains(serialID) )
			return -1;
		
		return SortItemList.IndexOf(serialID);
	}
	//----------------------------------------------------------------------------------------------
	public void RefreshPowerValue()
	{
//		lbRolesPower.text =ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetMainRolePower().ToString();
		//戰力值更新用這個轉一手做表演
		jumpNumber.SetEndNumber(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetMainRolePower());
	}
}