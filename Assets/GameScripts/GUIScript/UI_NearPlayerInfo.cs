using System;
using UnityEngine;
using GameFramework;

public class UIPRObject
{
	//multipartners
	public UIPanel					m_PartnerInfo				= null;
	public UILabel					m_PRName 					= null;
	public UILabel					m_PRLevel					= null;
	public UILabel					m_PRLVTitle					= null;
	public UILabel					m_PRStarRankTitle			= null;
	public UISprite					m_PRIcon					= null;
	public UISprite					m_PRStar					= null;
	
	//-----------------------------------------------------------------------------------------------------
	public void SetData(UIPanel mPR)
	{
		if(mPR == null)
			return;
		
		Transform temp;
		
		m_PartnerInfo = mPR;
		//取得同伴名稱
		temp = m_PartnerInfo.transform.FindChild("Label(PRIname)");
		m_PRName = temp.GetComponent<UILabel>();
		//取得同伴等級
		temp = m_PartnerInfo.transform.FindChild("Label(LevelNum)");
		m_PRLevel = temp.GetComponent<UILabel>();
		//取得同伴等級標題
		temp = m_PartnerInfo.transform.FindChild("Label(LVTitle)");
		m_PRLVTitle = temp.GetComponent<UILabel>();
		//取得同伴星級標題
		temp = m_PartnerInfo.transform.FindChild("Label(StarRankTitle)");
		m_PRStarRankTitle = temp.GetComponent<UILabel>();
		//取得同伴頭像
		temp = m_PartnerInfo.transform.FindChild("Sprite(PartnerIcon)");
		m_PRIcon = temp.GetComponent<UISprite>();
		//取得同伴星級
		temp = m_PartnerInfo.transform.FindChild("Sprite(Star)");
		m_PRStar = temp.GetComponent<UISprite>();
	}
}

public class UI_NearPlayerInfo : NGUIChildGUI 
{
	public UIButton 				btnCloseNearPlayerInfo 		= null;		//關閉詳細
	public UIButton 				btnChallenge				= null;		//挑戰
	public UIButton 				btnAddFriend 				= null;		//添加好友
	//Player Detail Info and related data
	public UIPanel 					panelPlayerInfo				= null;
	public UILabel					lbPNameTitle				= null;		//Title的玩家名稱
	public UILabel					lbPNameDetail				= null;		//Detail裡的玩家名稱
	public UILabel					lbLevelTitle				= null;		//等級標題
	public UILabel					lbPVPRankTitle				= null;		//PVP排行標題
	public UILabel					lbBakcupDataTitle			= null;		//備存資訊標題
	public UILabel					lbLevelNumber 				= null;		//等級
	public UILabel					lbPVPRank					= null;		//PVP排行
	public UILabel					lbBackupData				= null;		//備存資訊
	//Partner Detail Info and related data
	public UIPanel 					panelPartnerInfo			= null;		
	//生成同伴的模組
	public UIPRObject[]				panelOtherPR				= null;
	//生成同伴個數
	public int 						iPRNum						= 2;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_NearPlayerInfo";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_NearPlayerInfo() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	void Awake()
	{
		DynamicGeneratePR(iPRNum);
	}
	//-----------------------------------------------------------------------------------------------------
	//動態生成同伴數目
	public void DynamicGeneratePR(int addPRnum)
	{
		if(panelOtherPR==null && panelPartnerInfo!=null) 
		{
			panelOtherPR = new UIPRObject[addPRnum];
			int i;
			for(i=0; i<addPRnum; ++i)
			{
				panelOtherPR[i] = new UIPRObject();
			}
			i = 0;
			foreach(UIPRObject t in panelOtherPR)
			{
				GameObject newPR = Instantiate(panelPartnerInfo.gameObject) as GameObject;
				newPR.transform.parent = panelPartnerInfo.transform.parent;
				UIPanel uiPR = newPR.GetComponent<UIPanel>();

				//設定生成複數同伴的位置，大小，旋轉等資訊
				newPR.transform.localPosition = new Vector3(panelPartnerInfo.transform.localPosition.x,
				                                            panelPartnerInfo.transform.localPosition.y - 65*i,
				                                            panelPartnerInfo.transform.localPosition.z				);
				newPR.transform.rotation = panelPartnerInfo.transform.rotation;
				newPR.transform.localScale = panelPartnerInfo.transform.localScale;

				t.SetData(uiPR);
				++i;
			}
			panelPartnerInfo.gameObject.SetActive(false);
		}//if
	}
}