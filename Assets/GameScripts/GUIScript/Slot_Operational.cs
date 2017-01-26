using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_Operational : NGUIChildGUI  
{
    public UIButton		ButtonGet		= null;
    public UILabel		lbGet		    = null;
    public UISprite     spBG            = null;		//商品背景
    public UILabel      lbTitleName    = null;		//獎勵名稱
    public UISprite     spItemIcon      = null;		//商品圖案

    //-------------------------------------------------------------------------------------------------
    public Slot_Item    m_SlotItem      = null;		//紀錄物品Slot
    public int          m_RewardID      = 0; 		//此Slot所存的VIP商品編號
    //-----------------------執行用變數--------------------------------------------------------------
    [HideInInspector]
    public const string m_SlotItemName = "Slot_Item";		//物品Slot名稱

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_Operational";

	//-------------------------------------------------------------------------------------------------
	private Slot_Operational() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
	}

	//-------------------------------------------------------------------------------------------------
	public void CreateItemSlot(int itemGUID, int itemCount)
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
        newgo.transform.parent = this.transform;
        newgo.transform.localScale = Vector3.one;
        newgo.transform.localRotation = new Quaternion(0, 0, 0, 0);
        newgo.transform.localPosition = spItemIcon.transform.localPosition;
        newgo.gameObject.SetActive(true);
        newgo.SetSlotWithCount(itemGUID,itemCount,true);
        newgo.ButtonSlot.userData = itemGUID;
        m_SlotItem = newgo;
        UIEventListener.Get(m_SlotItem.ButtonSlot.gameObject).onClick	+= ChestCheck;
	}
    //-----------------------------------------------------------------------------------------------------
    //物品按鈕資訊導引
    private void ChestCheck(GameObject go)
    {
        int itemGUID = (int)go.GetComponent<UIButton>().userData;
        S_Item_Tmp dbf = GameDataDB.ItemDB.GetData(itemGUID);
        if (dbf == null)
            return;

        ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(dbf.GUID, 1);
        EventDelegate.Add(ARPGApplication.instance.m_uiItemTip.ButtonFullScreen.onClick, CloseItemInfo);
    }
    //-----------------------------------------------------------------------------------------------------
    //關閉物品資訊,由UI_ItemTip呼叫
    public void CloseItemInfo()
    {
        EventDelegate.Remove(ARPGApplication.instance.m_uiItemTip.ButtonFullScreen.onClick, CloseItemInfo);
    }
}
