using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_NonUsePetList : NGUIChildGUI 
{
	public UIScrollView					nonUsePetList		= null;
	public UIWrapContentEX				UIWrapContent		= null;
	public List<BagNonUsePetData>		PetItems			= new List<BagNonUsePetData>();
	public UIButton						btnClose			= null;
	//
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_NonUsePetList";
	
	//-------------------------------------------------------------------------------------------------
	private UI_NonUsePetList() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		CreateSlotItemIcons();
	}
	//-------------------------------------------------------------------------------------------------
	//取消選擇框與Toggle
	public void InitSelectStatus()
	{
		for(int i=0;i<PetItems.Count;++i)
			PetItems[i].CancelSelect();
	}
	//-------------------------------------------------------------------------------------------------
	//只取消Toggle
	public void InitToggleSelectStatus()
	{
		for(int i=0;i<PetItems.Count;++i)
			PetItems[i].CancelToggleSelect();
	}
	//-------------------------------------------------------------------------------------------------
	public void CreateSlotItemIcons()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI("Slot_Item").GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Item load prefeb error") );
			return;
		}

		for(int i=0;i<PetItems.Count;++i)
		{
			//createPetItemIcon
			Slot_Item newgo= Instantiate(go) as Slot_Item;
			
			newgo.transform.parent			= PetItems[i].IconLoc.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.gameObject.SetActive(true);
			newgo.SetDepth(20);
			newgo.GetComponent<BoxCollider>().enabled = false;
			//設定
			PetItems[i].PetIcon = newgo;
		}
	}
	//-------------------------------------------------------------------------------------------------
}
