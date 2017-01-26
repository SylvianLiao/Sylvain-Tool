using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum Enum_PetComplexItems
{
	Status= 0,
	LvUp,
	SkillUp,
	LimitBreak,
	Max
}


public class UI_PetComplexBoard : NGUIChildGUI 
{
	public UILabel			lbName 			= null; //名稱
	public UIButton			btnIntroduce	= null; //生平
	public UILabel			lbIntroduce		= null; //生平文字
	public UIGrid			gdButton		= null; //Grid
	public UIToggle			tgTypeBtn		= null; //右列按鈕的prefab
	public UITexture		texFullIcon		= null; //全身圖
	public UIButton			btnClose		= null; //關閉鈕
	public UIWidget			RoleStory		= null; //角色生平集合
	public UILabel			lbStoryTitle	= null; //生平標題
	public UILabel			lbStoryNote1	= null; //生平介紹1
	public UILabel			lbStoryNote2	= null; //生平介紹2

	[System.NonSerialized]
	public List<UIToggle>	TypeBtns		= new List<UIToggle>();
	[System.NonSerialized]
	public List<UILabel>	lbTypeBtns		= new List<UILabel>();
	[System.NonSerialized]
	public List<UISprite>	spTips			= new List<UISprite>();
	//
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_PetComplexBoard";
	//-------------------------------------------------------------------------------------------------
	private UI_PetComplexBoard() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		AssignText();
		CreateTypeBtnList();
		RoleStory.gameObject.SetActive(false);

	}
	//-------------------------------------------------------------------------------------------------
	//指派文字
	private void AssignText()
	{
		lbStoryTitle.text 	= GameDataDB.GetString(992);
		lbIntroduce.text	= GameDataDB.GetString(992);
	}
	//-------------------------------------------------------------------------------------------------
	//生成右側按鈕列
	private void CreateTypeBtnList()
	{
		TypeBtns.Clear();
		lbTypeBtns.Clear();
		spTips.Clear();
		for(int i=0;i<(int)Enum_PetComplexItems.Max;++i)
		{
			//createTypeButton
			UIToggle newgo= Instantiate(tgTypeBtn) as UIToggle;
			
			newgo.transform.parent			= gdButton.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.name 	= "Type"+i.ToString();
			newgo.value = false;
			if(i==0)
				newgo.value = true;
			newgo.group = 20;
			TypeBtns.Add(newgo);

			UILabel lbType = newgo.transform.FindChild("Label1").GetComponent<UILabel>();
			if(lbType != null)
				lbTypeBtns.Add(lbType);

			UISprite spTip = newgo.transform.FindChild("Sprite(Tip)").GetComponent<UISprite>();
			if(spTip != null)
			{
				spTip.gameObject.SetActive(false);
				spTips.Add(spTip);
			}
		}
		tgTypeBtn.gameObject.SetActive(false);
		gdButton.repositionNow = true;
		AssignTypeBtnText();
	}
	//-------------------------------------------------------------------------------------------------
	//設定typeBtn的文字
	private void AssignTypeBtnText()
	{
		for(int i=0;i<lbTypeBtns.Count;++i)
		{
			Enum_PetComplexItems pItem = (Enum_PetComplexItems)i;
			switch(pItem)
			{
			case Enum_PetComplexItems.LvUp:
				lbTypeBtns[i].text = GameDataDB.GetString(261); //升級
				break;
			case Enum_PetComplexItems.SkillUp:
				lbTypeBtns[i].text = GameDataDB.GetString(2701); //技能
				break;
			case Enum_PetComplexItems.LimitBreak:
				lbTypeBtns[i].text = GameDataDB.GetString(1241); // 突破
				break;
			case Enum_PetComplexItems.Status:
				lbTypeBtns[i].text = GameDataDB.GetString(1031); // 狀態
				break;
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	//更換toggle的初始狀態
	public void SetInitToggle(Enum_PetComplexItems pComItem)
	{
		int OriginalGroup = TypeBtns[0].group;
		for(int i=0;i<TypeBtns.Count;++i)
		{
			TypeBtns[i].group = 0;
			TypeBtns[i].value = false;
			if(i == (int)pComItem)
				TypeBtns[i].value = true;
		}

		for(int i=0;i<TypeBtns.Count;++i)
		{
			TypeBtns[i].group = OriginalGroup;
		}
	}
	//-------------------------------------------------------------------------------------------------
}
