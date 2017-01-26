using System;
using System.Collections;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class UI_PlayerInfo : NGUIChildGUI
{
	public TweenPosition twposAnimSprite	= null;
	public  UIButton BtnChangeHead  = null;
    public  UIButton BtnChangeFrame = null;
    public  UIButton BtnSystem      = null;
    public  UIButton BtnClose       = null;
	public	UIButton BtnSerialReward= null;
	public 	UIButton BtnForeBulletinBoard = null;
    public  UILabel LabelChangeHead = null;
    public  UILabel LabelChangeFrame = null;
    public  UILabel LabelSystem = null;
	public	UILabel LabelSerialReward = null;
	public  UILabel	LabelForeBulletinBoard = null;
      
    public  Slot_RoleIcon roleIcon  = null;

    public  UILabel LabelTitle      = null; 
    public  UILabel LabelName       = null;
    public  UILabel LabelPower      = null;
    public  UILabel LabelLevel      = null;
    public  UILabel LabelExp        = null;
    public  UILabel LabelID         = null;
    public  UILabel LabelNameValue  = null;
    public  UILabel LabelPowerValue = null;
    public  UILabel LabelLevelValue = null;
    public  UILabel LabelExpValue   = null;
    public  UILabel LabelIDValue    = null;  
    // 更換資訊
    public  GameObject ChangeListContainer  = null;
    public  UIScrollView SVChangeList       = null;
    public  UIGrid GridChangeList           = null;
    public  UIScrollView SVFrameList        = null;
    public  UIGrid GridFrameList            = null;
    public  List<UIButton> SlotChangeList = new List<UIButton>(); 

    private const string 	m_SlotName			="Slot_RoleIcon";
       
	//-------------------教學相關元件---------------------------
	public UIPanel			panelGuide				= null; //指引集合
	public UIButton			btnTopFullScreen		= null; //最上層的全螢幕按鈕
	public UIButton			btnFullScreen 			= null; //全螢幕按鍵
	public UILabel			lbGuideIntroduce		= null; //導引介紹頭像資訊
	public UISprite			spGuideSetting			= null; //導引系統設定
	public UILabel			lbGuideSetting			= null; 
	public UISprite			spGuideChangeHead		= null; //導引更換頭像
	public UILabel 			lbGuideChangeHead		= null; 

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_PlayerInfo";
	//-----------------------------------------------------------------------------------------------------
	private UI_PlayerInfo() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
    //-----------------------------------------------------------------------------------------------------
    // 初始化       
    public override void Initialize()
    {
        LabelTitle.text = GameDataDB.GetString(2520);     
        LabelName.text = GameDataDB.GetString(2521);       
        LabelPower.text = GameDataDB.GetString(2522);      
        LabelLevel.text = GameDataDB.GetString(2523);       
        LabelExp.text = GameDataDB.GetString(2524);
        LabelID.text = "ID";
        LabelChangeHead.text = GameDataDB.GetString(2526);
        LabelChangeFrame.text = GameDataDB.GetString(2527);
        LabelSystem.text = GameDataDB.GetString(2528);
		LabelSerialReward.text = GameDataDB.GetString(9760);
		LabelForeBulletinBoard.text = GameDataDB.GetString(481); //遊戲公告
    }
    public void SetIconListData(int iFrameID)
    {
        List<int> PetIDList = new List<int>(); 
        foreach (S_PetData pd in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData)
        {
            if (pd.iPetLevel > 0)
            {
                PetIDList.Add(pd.iPetDBFID);
            }
        }
        PetIDList.Sort((x, y) =>
        {
            S_PetData_Tmp xPet = GameDataDB.PetDB.GetData(x);
            S_PetData_Tmp yPet = GameDataDB.PetDB.GetData(y);
            if (xPet != null && yPet != null)
            {
                if(xPet.iRank > yPet.iRank)
                    return -xPet.iRank.CompareTo(yPet.iRank);
            }
            return 0;
        });

        List<int> IconIDList = new List<int>();
        //玩家職業大頭圖
        IconIDList.Add(ARPGApplication.instance.m_RoleSystem.GetVocationIconID());
        foreach (int PetID in PetIDList)
        {
            S_PetData_Tmp PetData = GameDataDB.PetDB.GetData(PetID);
            if(PetData == null)
                continue;
            IconIDList.Add(PetData.AvatarIcon);
        }
        for (int i = 0; i < IconIDList.Count; ++i)
        {
            GameObject go = ResourceManager.Instance.GetGUI(m_SlotName);
            GameObject newgo= Instantiate(go) as GameObject;
            newgo.transform.parent = GridChangeList.transform;
            newgo.transform.localPosition = Vector3.zero;
            newgo.transform.localScale = new Vector3(1.5f,1.5f,1.0f);
            newgo.SetActive(true);

            BoxCollider BC = newgo.GetComponent<BoxCollider>();
            BC.size = BC.size * 1.2f;
            Slot_RoleIcon roleIcon = newgo.GetComponent<Slot_RoleIcon>();

            roleIcon.ButtonSlot.userData = IconIDList[i];

            roleIcon.SetSlot(IconIDList[i],iFrameID);

            SlotChangeList.Add(roleIcon.ButtonSlot);
        }
        SVChangeList.ResetPosition();
        GridChangeList.enabled = true;
    }
    public void SetFrameListData(int iFace)
    {
        GameObject go = ResourceManager.Instance.GetGUI(m_SlotName);
        int size = GameDataDB.VIPLVDB.GetDataSize();
        for (int i = 0; i < size; i++)
        {
            S_VIPLV_Tmp VIPLV = GameDataDB.VIPLVDB.GetData(i + 1);
            if (VIPLV != null)
            {
                if (VIPLV.HeadFrame > 0)
                {
                    GameObject newgo = Instantiate(go) as GameObject;
                    newgo.transform.parent = GridFrameList.transform;
                    newgo.transform.localPosition = Vector3.zero;
                    newgo.transform.localScale = new Vector3(1.5f,1.5f,1.0f);
                    newgo.SetActive(true);
                    
                    BoxCollider BC = newgo.GetComponent<BoxCollider>();
                    BC.size = BC.size * 1.2f;
                    Slot_RoleIcon roleIcon = newgo.GetComponent<Slot_RoleIcon>();

                    roleIcon.ButtonSlot.userData = VIPLV.GetGUID();

                    roleIcon.SetSlot(iFace, VIPLV.HeadFrame);

                    SlotChangeList.Add(roleIcon.ButtonSlot);
                }
            }
        }
        SVFrameList.ResetPosition();
        GridFrameList.enabled = true;
    }
    public void ClearChangeList()
    {
        for (int i = 0; i < SlotChangeList.Count; ++i)
        {
            GameObject.Destroy(SlotChangeList[i].gameObject);
        }
        SlotChangeList.Clear();
    }
    public void SetPlayerInfo()
    {
        int currentLevel = ARPGApplication.instance.m_RoleSystem.iBaseLevel;	//等級為common角色表中的GUID值
        S_PlayerData_Tmp RoleDBF = GameDataDB.PlayerDB.GetData(currentLevel);

        LabelNameValue.text = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName;
        LabelPowerValue.text = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetMainRolePower().ToString();
        LabelLevelValue.text = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel().ToString();
        LabelExpValue.text = ARPGApplication.instance.m_RoleSystem.iBaseExp.ToString()+"/"+RoleDBF.iMaxExp.ToString();	//經驗值

        LabelIDValue.text = ARPGApplication.instance.m_RoleSystem.m_RoleGUID.ToString();  
    }
    public void SetIcon(int iFace,int iFaceFrame)
    {
        roleIcon.SetSlot(iFace,iFaceFrame);
    }
}

