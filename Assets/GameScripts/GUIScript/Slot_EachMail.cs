using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_EachMail : MonoBehaviour  
{
	public GameObject				gMailItemPos			= null;
	public UISprite					spNewMailMask			= null;
	public UIWidget					wgNewMailTip			= null;
	public UILabel  				lbMailTitle				= null;
	public UILabel 					lbMailContent			= null;
	public UIButton					btnGetMail				= null;
	public UILabel					lbGetMail				= null;
	public bool						m_IsNewMail				= false;
	public S_RewardData				m_MaildData				= null;

	public ulong iSerial;

	[HideInInspector]
	private int						m_CreateMailCount		= 0;			//由MailBox呼叫複製信件時傳入告知這是第幾次複製
	public int MailCount
	{
		get{return m_CreateMailCount;}
		set
		{
			m_CreateMailCount = value;
			slotMailItem.gameObject.name = string.Format("slotItem{0:00}",m_CreateMailCount);
		}
	}
	private const string 			m_SlotName				= "Slot_Item";
	public int						m_AdjustSlotDepth		= 15;			//調整SlotItem的Depth
	//-------------------------------------------------------------------------------------------------
	[HideInInspector]
	public Slot_Item				slotMailItem				= null;		//每個信件中自己的SlotItem
	//-------------------------------------------------------------------------------------------------
	public void InitialEachMail()
	{
		lbGetMail.text      = GameDataDB.GetString(2155);	//"收取"
		CreateMailItemSlot();
	}
	//-------------------------------------------------------------------------------------------------
	void CreateMailItemSlot()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		Slot_Item newgo= Instantiate(go) as Slot_Item;
		
		newgo.transform.parent			= this.transform;
		newgo.transform.localScale		= new Vector3(0.8f , 0.8f , 1.0f);	//SlotItem預設100x100，這裡需求80x80
		newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		newgo.transform.localPosition	= gMailItemPos.transform.localPosition;
		newgo.gameObject.SetActive(true);

		slotMailItem = newgo;
	}
}
