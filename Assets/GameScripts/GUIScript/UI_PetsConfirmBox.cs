using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_PetsConfirmBox : NGUIChildGUI 
{
	public UILabel			lbNote			= null; //功能詢問內容
    [Header("標題文字")]
	public UILabel			lbOptionMessage	= null; //標題文字
	public UIToggle			tgOption		= null;	//
	//
	public UIButton			btnCloseVerify	= null;
	public UIButton			btnVerify		= null;
	public UILabel			lbVerify		= null;
	public UIGrid			gridShowPets	= null;
	[HideInInspector]
	public Slot_Pet[] 		ShowPets		= new Slot_Pet[2];
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_PetConfirmBox";
	
	//-------------------------------------------------------------------------------------------------
	private UI_PetsConfirmBox() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		CreatePairPetList();
		lbVerify.text 	= GameDataDB.GetString(982);	//確定
	}
	//-------------------------------------------------------------------------------------------------
	private void CreatePairPetList()
	{
		Slot_Pet go = ResourceManager.Instance.GetGUI("Slot_Pet").GetComponent<Slot_Pet>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Pet load prefeb error") );
			return;
		}
		for(int i=0;i<ShowPets.Length;++i)
		{
			//createSlotPet
			Slot_Pet newgo= Instantiate(go) as Slot_Pet;
			
			newgo.transform.parent			= gridShowPets.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
			newgo.transform.localPosition	= Vector3.zero;
			newgo.name = "Pet"+i.ToString();
			ShowPets[i] = newgo;
		}
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
}