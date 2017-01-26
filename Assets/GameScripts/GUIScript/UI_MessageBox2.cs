using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
public class GiftData
{
    public int itemID;
    public int iCount;

    public GiftData()
	{
        itemID = -1;
        iCount = -1;
    }
}

public class UI_MessageBox2 : NGUIChildGUI 
{
    public UILabel		labTitle		= null;
    public UILabel		lbButtonOk      = null;
	public GameObject	goGift		    = null;
    public UIGrid       GridGiftList    = null;
    public UIButton     ButtonOk        = null;
	//public GameObject	goUpgradeItem	= null;
    public List<Slot_Item> GiftList     = null;

    private const string m_SlotName = "Slot_Item";
	
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_MessageBox2";
	//-------------------------------------------------------------------------------------------------
	private UI_MessageBox2() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
        GiftList = new List<Slot_Item>();
		base.Initialize();
		lbButtonOk.text = GameDataDB.GetString(299);
	}
    //-------------------------------------------------------------------------------------------------	
    public void OpenGiftBox(string title)
    {
		if (string.IsNullOrEmpty(title))
       		labTitle.text = GameDataDB.GetString(155);
		else
			labTitle.text = title;
        goGift.SetActive(true);
        //goUpgradeItem.SetActive(false);
        Show();
    }
//     //-------------------------------------------------------------------------------------------------	
//     void OpenUpgradeItem()
//     {
//         goGift.SetActive(false);
//         goUpgradeItem.SetActive(true);
//         Show();
//     }
	//-------------------------------------------------------------------------------------------------	
	public override void Show()
	{
		base.Show();
	}
	//-------------------------------------------------------------------------------------------------
	public  override void Hide()
	{
		base.Hide();
	}
	//-------------------------------------------------------------------------------------------------
    public void SetGiftList(List<GiftData> giftDataList)
    {
        GiftList.Clear();

        foreach (GiftData element in giftDataList)
        {
            if(element.itemID == 0)
                continue;
            CreateItemSlot();
        }
        for (int i = 0; i < giftDataList.Count; ++i)
        {
            if(i >= GiftList.Count)
                return;

            GiftList[i].SetSlotWithCount(giftDataList[i].itemID,giftDataList[i].iCount,true);
            GiftList[i].ButtonSlot.userData = giftDataList[i];
            UIEventListener.Get(GiftList[i].ButtonSlot.gameObject).onClick += AddItemOnClick;
        }

    }
    //-------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------
    //生成物品資訊欄中的物品圖示(SlotItem)
    private void CreateItemSlot()
    {
        Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();

        if (go == null)
        {
            UnityDebugger.Debugger.LogError(string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/" + m_SlotName));
            return;
        }

        Slot_Item newgo = Instantiate(go) as Slot_Item;

        newgo.transform.parent = GridGiftList.transform;
        newgo.transform.localScale = Vector3.one;
        newgo.transform.localRotation = new Quaternion(0, 0, 0, 0);

        newgo.gameObject.SetActive(true);
        GiftList.Add(newgo);
    }
    //-----------------------------------------------------------------------------------------------------
    public void AddItemOnClick(GameObject go)
    {
        GiftData giftData = (GiftData)go.GetComponent<UIButton>().userData;
        ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(giftData.itemID, giftData.iCount);
    }
}
