using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class PetPairData
{
	public S_PetData 	PetData1;
	public S_PetData	PetData2;
	//
	public PetPairData()
	{
		PetData1 = null;
		PetData2 = null;
	}
}


public class UI_ChangePetList : NGUIChildGUI 
{
	[Header("CommmonFrame")]
	public UILabel			lbTitle 		= null; //標題文字
	public UIButton			btnClose		= null; //關閉按鈕
	[Header("Button(Sequence)")]
	public UIButton			btnRankSort		= null; //品階篩選
	public UIButton			btnLevelSort	= null; //等級篩選
	public UIButton			btnCareerSort	= null; //職業篩選
	public UIButton			btnRaceSort		= null; //種族篩選
	public UILabel			lbRankSort		= null; //品階篩選文字
	public UILabel			lbLevelSort		= null; //等級篩選文字
	public UILabel			lbCareerSort	= null; //職業篩選文字
	public UILabel			lbRaceSort		= null; //種族篩選文字
	public UIToggle[]		tgSort			= new UIToggle[4];
	[Header("ScrollViewPetList")]
	public UIScrollView		svSelectPetList	= null; //寵物列表
	public UIWrapContentEX	uiWrapContent	= null; //WrapContent
	public Slot_PetPair		PetPairPrefab	= null; //Prefab
	//
	public List<Slot_PetPair> PetPairList	= new List<Slot_PetPair>();
	//
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ChangePetList";
	
	//-------------------------------------------------------------------------------------------------
	private UI_ChangePetList() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		AssignSortBtnText();
		CreatePairPetList();
	}
	//-------------------------------------------------------------------------------------------------
	private void AssignSortBtnText()
	{
		lbRankSort.text 	= GameDataDB.GetString(5023); //品階
		lbLevelSort.text 	= GameDataDB.GetString(5024); //等級
		lbCareerSort.text	= GameDataDB.GetString(5025); //職業
		lbRaceSort.text		= GameDataDB.GetString(5026); //種族
	}
	//-------------------------------------------------------------------------------------------------
	private void CreatePairPetList()
	{
		if(PetPairPrefab == null)
		{
			UnityDebugger.Debugger.LogError("Slot_PetPair load prefeb error");
			return;
		}
		PetPairList.Clear();
		for(int i=0;i<6;++i)
		{
			//createPetPair
			Slot_PetPair newgo= Instantiate(PetPairPrefab) as Slot_PetPair;
			newgo.transform.parent			= uiWrapContent.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.name 			= "PetPair"+i.ToString();
			newgo.gameObject.SetActive(false);
			PetPairList.Add(newgo);
		}
		PetPairPrefab.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
}