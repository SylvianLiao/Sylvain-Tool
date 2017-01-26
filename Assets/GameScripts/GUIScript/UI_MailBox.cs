using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class MailData
{
	public S_RewardData mailData = null;
	public bool isNewMail = false;
}

public class UI_MailBox : NGUIChildGUI 
{
	public UIPanel					panelMailsView					= null;
	//
	public UILabel					lbMailBoxTitle					= null;
	public UIButton					btnClose						= null;
	public UIButton					btnGetAll						= null;
	public UILabel					lbGetAll						= null;
	public UILabel					lbMailCapacity					= null;
	public UILabel					lbTipSentence					= null;
	public GameObject				gMailBody						= null;	//信件樣板
	//public UILabel					lbGetMail						= null;	//信件樣板中的領取按鈕字串
	//public UIGrid					gdGrid							= null;	//ScrollView排序用
	public UIWrapContentEX			wcEndlessScroll					= null;	//無限Loop ScrollView
	//-----------------------------------------------------------------------------------------------------
	[HideInInspector]
	public List<MailData>			m_MailDataList					= new List<MailData>();	//複製出的信件儲存清單
	public List<Slot_EachMail>		m_MailObjList					= new List<Slot_EachMail>();	//信件實體清單
	//-----------------------------------------------------------------------------------------------------
	private int 					m_MaxMailCapacity				= 100;
	private const int 				m_EachPageMailCount				= 5;	//單次頁面可顯示的信件數量
	private int 					m_RealMailHeight				= 0;	//實體信件高度
	//--------------------------------------指引教學相關元件---------------------------------------------------------------
	public UIPanel			panelGuide				= null; //指引集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen 			= null; //全螢幕按鍵
	public UISprite			spGuideReceiveMail		= null; //導引今日獎勵
	public UILabel			lbGuideReceiveMail		= null; 
	public UISprite			spGuideClose			= null; //導引整月獎勵內容
	public UILabel			lbGuideClose			= null; 
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_MailBox";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_MailBox() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialMailBox();
		m_RealMailHeight = m_MailObjList.Count * wcEndlessScroll.itemSize;
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
	void InitialMailBox()
	{
		//gdGrid.enabled = false;
		panelMailsView.GetComponent<UIScrollView>().enabled = false;
		gMailBody.SetActive(false);
		lbMailBoxTitle.text = GameDataDB.GetString(2151);	//"信箱"
		lbTipSentence.text 	= GameDataDB.GetString(2152);	//"信箱容量不足時，將自動刪除較舊信件"
		lbGetAll.text		= GameDataDB.GetString(2153);	//"全部收取"

		wcEndlessScroll.maxIndex = 0;
		GreateAllMail();
		wcEndlessScroll.onInitializeItem += AssignRewardData;
	}
	//-----------------------------------------------------------------------------------------------------
	public void UpdateMailBoxContent()
	{
		wcEndlessScroll.minIndex = (m_MailDataList.Count-1)*(-1);
		//顯示左下角信件數量及文字提示
		string mailNumber = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.RewardDatas.Count.ToString () + "/" + m_MaxMailCapacity.ToString();
		if (m_MailDataList.Count == m_MaxMailCapacity)
			mailNumber = string.Format ("[FF0000]"+mailNumber+"[-]");
		
		lbMailCapacity.text = mailNumber;
		//若信件數量沒超過一個頁面可顯示之數量便關閉Scrollview
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.RewardDatas.Count > m_EachPageMailCount)	
			panelMailsView.GetComponent<UIScrollView>().enabled = true; // 只有內容多於5個才會讓scrollView有作用
		else 				
			panelMailsView.GetComponent<UIScrollView>().enabled = false;
	}
	//-----------------------------------------------------------------------------------------------------
	//生成所有信件
	public void GreateAllMail()
	{
		if(gMailBody != null)
		{
			//初始化實體信件UI
			for(int i=0 ; i < m_MailObjList.Count; ++i)
			{
				m_MailObjList[i].InitialEachMail();
				m_MailObjList[i].slotMailItem.SetDepth(m_MailObjList[i].m_AdjustSlotDepth);
			}
			//排序信件
			wcEndlessScroll.enabled = true;
			wcEndlessScroll.SortAlphabetically();
			//暫存獎勵資料
			foreach(S_RewardData sRewardData in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.RewardDatas.Values)
			{
				MailData mail = new MailData();
				mail.mailData = sRewardData;						//給之後實體信件的SlotItem用

				m_MailDataList.Add(mail);
			}
			m_MailDataList.Reverse();
			//高亮新信件
			if (m_MailDataList.Count >= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.NewMailCount)
			{
				for (int i=0; i < ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.NewMailCount; ++i)
				{
					m_MailDataList[i].isNewMail = true;
				}
			}
			UpdateMailBoxContent();
		}//end if
	}
	//-----------------------------------------------------------------------------------------------------
	void AssignRewardData(GameObject mail , int wrapIndex , int realIndex)
	{
		realIndex = Mathf.Abs(realIndex);
		if (realIndex > m_MailDataList.Count-1)
		{
			mail.SetActive(false);
			return;
		}
		Slot_EachMail nextMail = mail.GetComponent<Slot_EachMail>();
		if (m_MailDataList[realIndex].mailData == null)
			return;
		//將暫存資料Assign給信件實體
		nextMail.m_MaildData = m_MailDataList[realIndex].mailData;
		nextMail.m_IsNewMail = m_MailDataList[realIndex].isNewMail;
		nextMail.iSerial = nextMail.m_MaildData.iSerial;
		nextMail.slotMailItem.SetSlotWithCount(nextMail.m_MaildData.iItemGUID , nextMail.m_MaildData.iItemCount , false);
		nextMail.lbMailTitle.text = nextMail.m_MaildData.strName;
		nextMail.lbMailContent.text = nextMail.m_MaildData.strText;
		nextMail.MailCount = realIndex;
		//高亮新信與否
		nextMail.spNewMailMask.gameObject.SetActive(nextMail.m_IsNewMail);
		nextMail.wgNewMailTip.gameObject.SetActive(nextMail.m_IsNewMail);
		mail.SetActive(true);
	}
	//-----------------------------------------------------------------------------------------------------
	public void RemoveMail (int iSerial)
	{
		if (m_MailDataList.Count <= 0)
			return;
		//刪除單一信件
		if (iSerial != 0)
		{
			//刪除暫存的信件資料
			int i;
			for(i=0 ; i< m_MailDataList.Count; ++i)
			{
				if (m_MailDataList[i].mailData.iSerial == (ulong)iSerial)
				{
					m_MailDataList.RemoveAt(i);
					break;
				}
			}

			if (m_MailDataList.Count > m_EachPageMailCount && 
			    m_MailDataList.Count - i < m_EachPageMailCount)
			{
				SpringPanel spPanel = panelMailsView.GetComponent<SpringPanel>();
				if (spPanel != null)
				{
					Vector3 vec3 = spPanel.target;
					vec3.y -= wcEndlessScroll.itemSize;
					SpringPanel.Begin(spPanel.gameObject , vec3 , spPanel.strength);
				}
			}
			wcEndlessScroll.UpdateAllItem();
		}
		//刪除全部信件
		else
		{
			for (int i=0; i < m_MailObjList.Count; ++i)
				m_MailObjList[i].gameObject.SetActive(false);
			m_MailDataList.Clear();
		}
		
	}
}
