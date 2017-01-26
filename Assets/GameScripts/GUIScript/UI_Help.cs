using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;


[Serializable]
public class HelpPage
{
	public enum ENUM_Style
	{
		LabelStyle,
		TextureStyle
	}

	[Serializable]
	public struct PageItem
	{
		public ENUM_Style style;
		public int ID;
	}

	public PageItem[] Datas;
}

class UI_Help : NGUIChildGUI
{
	public UILabel		LableStyle;
	public UITexture	TextureStyle;
	public UITable		TableRoot;
	public UIButton		ButtonLeft;
	public UIButton		ButtonRight;
	public UIButton		ButtonClose;
	public UIScrollView ScrollViewOBj;
	public UILabel		LableTitle;

	public UIGrid		GridPages;
	public UIToggle		TogglePage;
	UIToggle[] TotalPages;
    private const string GUI_SMARTOBJECT_NAME = "UI_Help";
	
	//---------------------------------------------------------------------------------------------------
	private UI_Help() : base(GUI_SMARTOBJECT_NAME)
	{

	}

	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();

		if (null != LableStyle)
			LableStyle.gameObject.SetActive(false);

		if (null != TextureStyle)
			TextureStyle.gameObject.SetActive(false);
	}

	// 清除所有資料
	public void Clear()
	{
		NGUITools.DestroyChildren(TableRoot.transform);
	}

	// 增加一組字串
	public void AddLabel(int StringID)
	{
		string Text = GameDataDB.GetString(StringID);
		GameObject newLabel = NGUITools.AddChild(TableRoot.gameObject, LableStyle.gameObject);
		newLabel.SetActive(true);

		UILabel uiLabel = newLabel.GetComponent<UILabel>();
		if (uiLabel)
			uiLabel.text = Text;
        TableRoot.Reposition();
	}

	// 增加一組字串
	public void AddTexture(int TextureID)
	{
		GameObject newTexture = NGUITools.AddChild(TableRoot.gameObject, TextureStyle.gameObject);
		newTexture.SetActive(true);
		UITexture uiTexture = newTexture.GetComponent<UITexture>();
		if (uiTexture)
		{
			if (Utility.ChangeTexture(uiTexture, TextureID))
			{
				uiTexture.MakePixelPerfect();
				TableRoot.Reposition();
			}
			else
			{
				GameObject.Destroy(newTexture);
            }
        }
    }

	public void SetPage (HelpPage onePage, uint currentIndex)
	{
		Clear();
		if (null == onePage)
			return;

		for(int i=0;i<onePage.Datas.Length;i++)
		{
			switch(onePage.Datas[i].style)
			{
			case HelpPage.ENUM_Style.LabelStyle:
				AddLabel(onePage.Datas[i].ID);
				break;
			case HelpPage.ENUM_Style.TextureStyle:
				AddTexture(onePage.Datas[i].ID);
				break;
			}
		}


		ScrollViewOBj.ResetPosition();

		TotalPages[currentIndex].value = true;
	}

	public void SetTitle(int stringID)
	{
		if (LableTitle)
			LableTitle.text = GameDataDB.GetString(stringID);
	}

	public void SetPageSize (int length)
	{
		NGUITools.DestroyChildren(GridPages.transform);

		TotalPages = new UIToggle[length];
		for(int i=0;i<length;i++)
		{
			GameObject newChild = NGUITools.AddChild(GridPages.gameObject,TogglePage.gameObject);
			newChild.SetActive(true);
			TotalPages[i] = newChild.GetComponent<UIToggle>();
		}

		GridPages.Reposition();

	}
}
