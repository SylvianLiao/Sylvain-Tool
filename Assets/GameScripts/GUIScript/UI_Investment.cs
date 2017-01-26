using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;


public class UI_Investment : NGUIChildGUI 
{
	public UIPanel		panelBase			= null; 
	public UIButton		btnClose			= null; //關閉
	public UILabel		lbDiamondBankTitle	= null; //標題
	//未選擇投資時
	public UIWidget		ChoseSet			= null; //未選擇時頁面集合
	public UIButton		btnNote				= null; //開啟投資內容細目
	public UILabel		lbContentTitle		= null; //投資標題
	public UILabel		lbTimeTip			= null; //投資限時(目前暫時隱藏中)
	public UILabel		lbPlanTitle			= null; //投資類型
	public UILabel		lbSubTitle			= null; //投資副標題
	public UILabel		lbPlanNote			= null; //投資內文
	public UIButton		btnUse				= null; //選擇投資按鈕
	public UILabel		lbUse				= null; //選擇投資字樣
	public UIScrollView	svNoteContent		= null; //內容細目說明scrollview
	public UIPanel		panelNoteContent	= null; //內容細目說明scrollview
	public UIWidget		NoteContent			= null; //內容細目說明
	public UILabel		lbNoteContent		= null; //內容細目說明
	//已選擇投資時
	public UIWidget		ChoosedList			= null; //已選擇時頁面集合
	public UILabel		lbListContent		= null; //已選擇內容說明
	public UILabel		lbLevel				= null; //玩家等級
	public UIScrollView	svGetList			= null; //領取列表scrollview
	public UIGrid		Grid				= null; //領取列表Grid
	public BankGetItem	GetItemPrefab		= null; //領取項目生成
	[System.NonSerialized]
	public List<BankGetItem>	BkGetItems	= new List<BankGetItem>();
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UILabel		lbGuideIntroduce				= null;
	public UISprite		spGuideClickBtnNote				= null; //導引點擊說明按鈕
	public UILabel		lbGuideClickBtnNote				= null;
	public UISprite		spGuideShowNote					= null; //導引介紹說明
	public UILabel		lbGuideShowNote					= null;
	//-------------------------------------------------------------------------------------------------
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_DiamondBank";
	
	//-------------------------------------------------------------------------------------------------
	private UI_Investment() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		//lbDiamondBankTitle.text	= GameDataDB.GetString(15207); // 投資理財
		lbPlanTitle.text 		= GameDataDB.GetString(15217); // 豪華理財計畫
		lbSubTitle.text			= GameDataDB.GetString(15218); // 投資1000寶石
		lbPlanNote.text			= GameDataDB.GetString(15219); // 最高返還13500寶石
		lbUse.text				= GameDataDB.GetString(15221); // 投資
		lbContentTitle.text     = GameDataDB.GetString(15208); // 每個人都擁有一次寶貴的投資機會，最高將會帶來10倍的寶石返利
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-------------------------------------------------------------------------------------------------
	//設定說明內文
	public void SetNoteContent(List<S_Investment_Tmp> InvestItems)
	{
		string str = null;
		for(int i=0;i<InvestItems.Count;++i)
		{
			if(i == 0)
			{
				str += string.Format(GameDataDB.GetString(15224),InvestItems[i].iCount);
				continue;
			}
			str += "\n" + string.Format(GameDataDB.GetString(15225), InvestItems[i].iLevel,InvestItems[i].iCount);
		}
		lbNoteContent.text = str;
	}
	//-------------------------------------------------------------------------------------------------
	public void CreateInvestItemSlots(List<S_Investment_Tmp> InvestItems)
	{
		if (GetItemPrefab == null)
		{
			UnityDebugger.Debugger.LogError("Prefab reference Error");
			return;
		}
		BkGetItems.Clear();
		//生成商品Slot
		for(int i=0;i<InvestItems.Count;++i)
		{
			BankGetItem newgo = GameObject.Instantiate(GetItemPrefab) as BankGetItem;
			newgo.transform.parent = Grid.transform;
			newgo.transform.localScale = Vector3.one;
			newgo.transform.localRotation = Quaternion.identity;
			newgo.transform.localPosition = Vector3.one;
			newgo.name = "InvestmentItem" + i.ToString();
			newgo.SetItemData(InvestItems[i],i);
			BkGetItems.Add(newgo);
		}
		Grid.cellHeight = 85f;
		Grid.repositionNow = true;
		GetItemPrefab.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	public void UpdateAllItemButton()
	{
		for(int i=0;i<BkGetItems.Count;++i)
			BkGetItems[i].UpdateButtonState();
	}
	//-------------------------------------------------------------------------------------------------
}
