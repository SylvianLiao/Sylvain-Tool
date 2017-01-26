using System;
using UnityEngine;
using GameFramework;

public class UINearPlayerObj
{
	//multiplayers component
	public UIPanel					m_NearPlayer				= null;
	public UIButton					m_btnNearPlayer				= null;
	public UILabel					m_NPLevel					= null;
	public UILabel					m_NPLVnum					= null;
	public UILabel					m_NPName					= null;
	public UISprite					m_NPIcon					= null;
	
	//-----------------------------------------------------------------------------------------------------
	public void SetData(UIPanel mNP)
	{
		if(mNP == null)
			return;
		
		Transform temp;
		
		m_NearPlayer= mNP;
		//
		temp = m_NearPlayer.transform.FindChild("Button(EnterPlayerInfo)");
		m_btnNearPlayer = temp.GetComponent<UIButton>();
		//
		temp = m_NearPlayer.transform.FindChild("Label(LV)");
		m_NPLevel = temp.GetComponent<UILabel>();
		//
		temp = m_NearPlayer.transform.FindChild("Label(LVnumber)");
		m_NPLVnum = temp.GetComponent<UILabel>();
		//
		temp = m_NearPlayer.transform.FindChild("Label(PlayerName)");
		m_NPName = temp.GetComponent<UILabel>();
		//
		temp = m_NearPlayer.transform.FindChild("Sprite(PlayerIcon)");
		m_NPIcon = temp.GetComponent<UISprite>();
	}
}

class UI_NearPlayers : NGUIChildGUI 
{
	public UIButton					btnClosePlayerList 	= null;
	public UIPanel					panelNearPlayer		= null;
	public UIButton					btnNearPlayer		= null;

	public UINearPlayerObj[]		uiNearPlayer		= null;

	public int						iNPNum				=3;

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_NearPlayers";

	//-----------------------------------------------------------------------------------------------------
	private UI_NearPlayers() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	void Awake()
	{
		GenerateNearPlayers(iNPNum);
	}
	//-----------------------------------------------------------------------------------------------------
	//動態生成附近玩家數目
	public void GenerateNearPlayers(int addNPnum)
	{
		if(uiNearPlayer==null && panelNearPlayer!=null) 
		{
			uiNearPlayer = new UINearPlayerObj[addNPnum];
			int i;
			for(i=0; i<addNPnum; i++)
			{
				uiNearPlayer[i] = new UINearPlayerObj();
			}
			i = 0;
			foreach(UINearPlayerObj t in uiNearPlayer)
			{
				GameObject newNP = Instantiate(panelNearPlayer.gameObject) as GameObject;
				newNP.transform.parent = panelNearPlayer.transform.parent;
				UIPanel uiNP = newNP.GetComponent<UIPanel>();
				
				//設定生成附近玩家的位置，大小，旋轉等資訊
				newNP.transform.localPosition = new Vector3(panelNearPlayer.transform.localPosition.x,
				                                            panelNearPlayer.transform.localPosition.y - 86*i,
				                                            panelNearPlayer.transform.localPosition.z				);
				newNP.transform.rotation = panelNearPlayer.transform.rotation;
				newNP.transform.localScale = panelNearPlayer.transform.localScale;
				
				t.SetData(uiNP);
				++i;
			}
			panelNearPlayer.gameObject.SetActive(false);
		}//if
	}
}
