using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_OperationalReward : NGUIChildGUI  
{
	public UIPanel		panelBase					= null;
	public UIPanel		panelInfo1			        = null;
	public UIScrollView	ScrollViewInfo1				= null;
	public UIGrid		GridInfo1					= null;
    public UILabel      lbNevin1                    = null;

    public GameObject	panelInfo2			        = null;
    public UILabel      lbNevin2                    = null;
    public UILabel      lbTitle2                    = null;
    public UISprite     spritItem                   = null;
    public UIButton     btnGet2                     = null;
    public UISprite     spriteGet2BG                = null;
    public UILabel      lbGet2                      = null;
    public Slot_Item    m_SlotItem                  = null;		//紀錄物品Slot
	//
	public UIScrollView	scrollTitle				    = null;
	public UIGrid		GridTitle					= null;
    public UISprite     spriteBorder                = null;
    public UIButton     btnClose                    = null;

    public UILabel      lbValue                     = null;
    //clone prefab
	public UIButton		ButtonOperational    		= null;
	public GameObject	OperationalPrefab			= null;
	[HideInInspector] public List<UIButton>		    AllButtonOperational    = new List<UIButton>();
	[HideInInspector] public List<UILabel>		    AllLabelOperational	    = new List<UILabel>();
	//
	[HideInInspector] public List<Slot_Operational>	OperationalData	        = new List<Slot_Operational>();
    [HideInInspector] public const string m_SlotItemName = "Slot_Item";		//物品Slot名稱
    [HideInInspector] public const string m_SlotOperationalName = "Slot_Operational";		//Operational Slot名稱

    Color NonGetColor = new Color((float)157 / (float)255, (float)157 / (float)255, (float)157 / (float)255);
    Color CanGetColor = new Color((float)255 / (float) 255,(float)146 / (float)255, (float)36 /  (float)255);
    Color GetedColor  = new Color((float)115 / (float)255, (float)115 / (float)255, (float)185 / (float)255);
    [HideInInspector] public int iGetOperationalRewardID = 0;
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
//	public UISprite		spGuideGetReward				= null; //導引領取獎勵
//	public UILabel		lbGuideGetReward				= null;
	public UILabel		lbGuideIntroduce					= null;
	//-------------------------------------------------------------------------------------------------

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_OperationalReward";
	//-------------------------------------------------------------------------------------------------
	private UI_OperationalReward() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
	}
    //-------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
    }
	//-------------------------------------------------------------------------------------------------
	//設定&產生營運標題
    public void SetAllOperationalTitle(SortedDictionary<int,int> OpenOperationalGroupList)
	{
		AllButtonOperational.Clear();
		AllLabelOperational.Clear();
        foreach (int key in OpenOperationalGroupList.Keys)
        {
            Transform t;
            GameObject gb = NGUITools.AddChild(GridTitle.gameObject, ButtonOperational.gameObject);
            AllButtonOperational.Add(gb.GetComponent<UIButton>());
            gb.GetComponent<UIButton>().userData = key;
            t = gb.transform.FindChild("Label");

            S_OperationalReward_Tmp ORDbf = GameDataDB.OperationalRewardDB.GetData(OpenOperationalGroupList[key]);
            t.GetComponent<UILabel>().text = GameDataDB.GetString(ORDbf.iName);
            
            t = gb.transform.FindChild("Sprite(NumBG)");
            t.gameObject.SetActive(CheckOperationalTip(ORDbf));         
            gb.SetActive(true);
        }
        //重置Title
        GridTitle.Reposition();
        scrollTitle.ResetPosition();

        CreateItemSlot();
	}


	//-------------------------------------------------------------------------------------------------
	//設定營運活動內容
	public void SetOperationalData(int GUID,List<int> OpenOperationalList)
	{
        S_OperationalReward_Tmp ORDbf = GameDataDB.OperationalRewardDB.GetData(GUID);
        if (ORDbf == null)
            return;

        if (ORDbf.iUIStyle == 0)
        {
            panelInfo1.gameObject.SetActive(true);
            panelInfo2.gameObject.SetActive(false);
            lbNevin1.text = GameDataDB.GetString(ORDbf.iNevin);
            lbNevin1.gameObject.SetActive(true);
            int size = OpenOperationalList.Count;
            GameDataDB.OperationalRewardDB.ResetByOrder();
            int index = 0;
            for (int i = 0; i < size; ++i)
            {
                S_OperationalReward_Tmp tmpDbf = GameDataDB.OperationalRewardDB.GetData(OpenOperationalList[i]);
                if (tmpDbf != null)
                {
                    if (tmpDbf.iGroup == ORDbf.iGroup)
                    {
                        SetButtonState(tmpDbf,OperationalData[index].spBG,OperationalData[index].ButtonGet,OperationalData[index].lbGet);
                        index++;
                    }
                }
            }

            for (int i = 0; i < OperationalData.Count; ++i)
            {
                UIEventListener.Get(OperationalData[i].ButtonGet.gameObject).onClick = GetReward;
            }
            //刷新
            GridInfo1.Reposition();
            ScrollViewInfo1.ResetPosition();
        }
        else if (ORDbf.iUIStyle == 1)
        {
            m_SlotItem.SetSlotWithCount(ORDbf.iItemID, ORDbf.iCount, true);
            lbTitle2.text = GameDataDB.GetString(ORDbf.iTitle);
            lbNevin2.text = GameDataDB.GetString(ORDbf.iNevin);
            lbNevin1.gameObject.SetActive(false);
            panelInfo1.gameObject.SetActive(false);
            panelInfo2.gameObject.SetActive(true);
            btnGet2.userData = ORDbf.GUID;
            UIEventListener.Get(btnGet2.gameObject).onClick = GetReward;

            SetButtonState(ORDbf, spriteGet2BG, btnGet2, lbGet2);
        }
        S_OperationalReward ORData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetOperationalReward(ORDbf.emType);
        switch (ORDbf.emType)
        {
            case ENUM_OperationalRewardType.ENUM_OperationalRewardType_StoredTotal:
            lbValue.text = string.Format(GameDataDB.GetString(15255),ORData.iValue);
            lbValue.gameObject.SetActive(true);
            break;
            case ENUM_OperationalRewardType.ENUM_OperationalRewardType_DayStored:
            lbValue.text = string.Format(GameDataDB.GetString(15256),ORData.iValue);
            lbValue.gameObject.SetActive(true);
	        break;
            case ENUM_OperationalRewardType.ENUM_OperationalRewardType_DayLogin:
            lbValue.gameObject.SetActive(false);
	        break;
            case ENUM_OperationalRewardType.ENUM_OperationalRewardType_Lv:
            lbValue.text = string.Format(GameDataDB.GetString(15257),ORData.iValue);
            lbValue.gameObject.SetActive(true);
	        break;
            case ENUM_OperationalRewardType.ENUM_OperationalRewardType_VIPLv:
            lbValue.text = string.Format(GameDataDB.GetString(15258),ORData.iValue);
            lbValue.gameObject.SetActive(true);
	        break;
            case ENUM_OperationalRewardType.ENUM_OperationalRewardType_Item:
            lbValue.gameObject.SetActive(false);
		    break;
            case ENUM_OperationalRewardType.ENUM_OperationalRewardType_Diamond:
            lbValue.text = string.Format(GameDataDB.GetString(15259),ORData.iValue);
            lbValue.gameObject.SetActive(true);
	        break;
        }

	}
    //-------------------------------------------------------------------------------------------------
    public void CreateOperationalSlot(S_OperationalReward_Tmp tmpDbf)
    {
        Slot_Operational go = ResourceManager.Instance.GetGUI(m_SlotOperationalName).GetComponent<Slot_Operational>();
        if (go == null)
        {
            UnityDebugger.Debugger.LogError(string.Format("Slot_Operational load prefeb error,path:{0}", "GUI/" + m_SlotOperationalName));
            return;
        }
        //生成商品Slot
        Slot_Operational newgo = GameObject.Instantiate(go) as Slot_Operational;
        newgo.transform.parent = GridInfo1.transform;
        newgo.transform.localScale = Vector3.one;
        newgo.transform.localRotation = new Quaternion(0, 0, 0, 0);
        newgo.transform.localPosition = Vector3.zero;
        newgo.gameObject.SetActive(true);
        newgo.CreateItemSlot(tmpDbf.iItemID,tmpDbf.iCount);
        newgo.lbTitleName.text = GameDataDB.GetString(tmpDbf.iTitle);
        newgo.ButtonGet.GetComponent<UIButton>().userData = tmpDbf.GUID;
        OperationalData.Add(newgo);

        SetButtonState(tmpDbf, newgo.spBG,newgo.ButtonGet,newgo.lbGet);
        return;
    }

    private void SetButtonState(S_OperationalReward_Tmp tmpDbf,UISprite spBG, UIButton ButtonGet, UILabel lbGet)
    {
        //setData
        if (tmpDbf.emType == ENUM_OperationalRewardType.ENUM_OperationalRewardType_Item)
        {
            bool CanGet = true;
            S_Fusion_Tmp fusionDbf = GameDataDB.FusionDB.GetData(tmpDbf.iCost);
            if (fusionDbf == null)
            {
                return;
            }
            for (int i = 0; i < fusionDbf.MaterialItem.Length; ++i)
            {
                if (fusionDbf.MaterialItem[i].iItemID != 0 && fusionDbf.MaterialItem[i].iCount != 0)
                {
                    int iCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemCountByGUID(fusionDbf.MaterialItem[i].iItemID);
                    if (iCount < fusionDbf.MaterialItem[i].iCount)
                    {
                        CanGet = false;
                    }
                }
            }

            if (CanGet)
            {
                lbGet.text = GameDataDB.GetString(15205);
                ButtonGet.GetComponent<BoxCollider>().enabled = true;
                spBG.color = CanGetColor;
                //顯示可兌換
            }
            else
            {
                lbGet.text = GameDataDB.GetString(15200);
                ButtonGet.GetComponent<BoxCollider>().enabled = false;
                spBG.color = NonGetColor;
                //顯示道具不足
            }
        }
        else
        {
            S_OperationalReward ORData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetOperationalReward(tmpDbf.emType);

            switch (tmpDbf.emType)
            {
                case ENUM_OperationalRewardType.ENUM_OperationalRewardType_Lv:
                ORData.iValue = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel();
                break;
                case ENUM_OperationalRewardType.ENUM_OperationalRewardType_VIPLv:
                ORData.iValue = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetVIPRank();
                break;
            }

            if (ORData.sFlag.GetFlag(tmpDbf.iFlag))
            {
                lbGet.text = GameDataDB.GetString(15202);
                ButtonGet.GetComponent<BoxCollider>().enabled = false;
                spBG.color = GetedColor;
                //顯示已領取
            }
            else
            {
                if (ORData.iValue >= tmpDbf.iCost)
                {
                    lbGet.text = GameDataDB.GetString(15201);
                    ButtonGet.GetComponent<BoxCollider>().enabled = true;
                    spBG.color = CanGetColor;
                    //顯示可領取
                }
                else
                {
                    lbGet.text = GameDataDB.GetString(15200);
                    ButtonGet.GetComponent<BoxCollider>().enabled = false;
                    spBG.color = NonGetColor;
                    //顯示未達條件
                }
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void CreateItemSlot()
    {
        Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotItemName).GetComponent<Slot_Item>();
        if (go == null)
        {
            UnityDebugger.Debugger.LogError(string.Format("Slot_Operational load prefeb error,path:{0}", "GUI/" + m_SlotItemName));
            return;
        }
        //生成商品Slot
        Slot_Item newgo = GameObject.Instantiate(go) as Slot_Item;
        //newgo.gameObject.SetActive(false);
        newgo.transform.parent = panelInfo2.transform;
        newgo.transform.localScale = Vector3.one;
        newgo.transform.localRotation = new Quaternion(0, 0, 0, 0);
        newgo.transform.localPosition = spritItem.transform.localPosition;
        newgo.gameObject.SetActive(true);
        m_SlotItem = newgo;
    }
    public void ClearCloneGameObject()
    {
        for(int i = 0; i < OperationalData.Count; ++i)
        {
            UIEventListener.Get(OperationalData[i].ButtonGet.gameObject).onClick -= GetReward;
            NGUITools.Destroy(OperationalData[i].gameObject);
        }
        OperationalData.Clear();
    }

    //-----------------------------------------------------------------------------------------------------
    private void GetReward(GameObject go)
    {
		//背包已滿時
		if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.IsBagFull())
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(1319)); 	//背包已滿，請清理背包空間
			return;
		}
        UIButton btn = go.GetComponent<UIButton>();
        if (btn == null)
            return;
        iGetOperationalRewardID = (int)btn.userData;

        JsonSlot_Reward.Send_CtoM_GetOperationalReward(iGetOperationalRewardID);
    }
    public void CreateALLOperationalSlot(int GUID,List<int> OpenOperationalList)
    {
        S_OperationalReward_Tmp ORDbf = GameDataDB.OperationalRewardDB.GetData(GUID);
        if (ORDbf == null)
            return;

        if (ORDbf.iUIStyle == 0)
        {
            ClearCloneGameObject();
            int size = OpenOperationalList.Count;
            GameDataDB.OperationalRewardDB.ResetByOrder();
            for (int i = 0; i < size; ++i)
            {
                S_OperationalReward_Tmp tmpDbf = GameDataDB.OperationalRewardDB.GetData(OpenOperationalList[i]);
                if (tmpDbf != null)
                {
                    if (tmpDbf.iGroup == ORDbf.iGroup)
                    {
                        CreateOperationalSlot(tmpDbf);
                    }
                }
            }
        }
        else if (ORDbf.iUIStyle == 1)
        {
        }
    }

    //-----------------------------------------------------------------------------------------------------
    //營運活動領取提示顯示
    public bool CheckOperationalTip(S_OperationalReward_Tmp ORDbf)
    {
        bool isShowTip = false;
        int size = GameDataDB.OperationalRewardDB.GetDataSize();
        GameDataDB.OperationalRewardDB.ResetByOrder();
        for (int i = 1; i <= size; ++i)
        {
            S_OperationalReward_Tmp tmpDbf = GameDataDB.OperationalRewardDB.GetDataByOrder();

            if (tmpDbf.iGroup == ORDbf.iGroup && tmpDbf.StartTime <= DateTime.Now && tmpDbf.EndTime >= DateTime.Now)
            {
                if (tmpDbf.emType == ENUM_OperationalRewardType.ENUM_OperationalRewardType_Item)
                {
                    bool CanGet = true;
                    S_Fusion_Tmp fusionDbf = GameDataDB.FusionDB.GetData(tmpDbf.iCost);
                    if (fusionDbf == null)
                    {
                        return false;
                    }
                    for (int j = 0; j < fusionDbf.MaterialItem.Length; ++j)
                    {
                        if (fusionDbf.MaterialItem[j].iItemID != 0 && fusionDbf.MaterialItem[j].iCount != 0)
                        {
                            int iCount = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemCountByGUID(fusionDbf.MaterialItem[j].iItemID);
                            if (iCount < fusionDbf.MaterialItem[j].iCount)
                            {
                                CanGet = false;
                            }
                        }
                    }

                    if (CanGet)
                    {
                        //顯示可兌換
                        isShowTip = CanGet;
                    }
                }
                else
                {
                    S_OperationalReward ORData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetOperationalReward(tmpDbf.emType);

                    switch (tmpDbf.emType)
                    {
                        case ENUM_OperationalRewardType.ENUM_OperationalRewardType_Lv:
                        ORData.iValue = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel();
                        break;
                        case ENUM_OperationalRewardType.ENUM_OperationalRewardType_VIPLv:
                        ORData.iValue = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetVIPRank();
                        break;
                    }

                    if (!ORData.sFlag.GetFlag(tmpDbf.iFlag))
                    {
                        if (ORData.iValue >= tmpDbf.iCost)
                        {
                            //顯示可領取
                            isShowTip = true;
                            break;
                        }
                    }

                }
            }
 
        }
        return isShowTip;
    }
}
