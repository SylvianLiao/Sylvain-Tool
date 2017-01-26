using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

public class Slot_FormationNode : MonoBehaviour 
{
	public Slot_Item	btnPet;
	public UILabel		lbLvLock;
	public UILabel		lbEffect;
	public UISprite		spLock;
	public UISprite		spPlus;
	public UISprite		spBG;
	public ENUM_FormationNodeAbility emAttrType;

	public bool 		Lock;
	public int			SlotNumber;
	public int 			PetGUID;
	public string		sEffecf;

	//-------------------------------------------------------------
	void Start () 
	{
	}
	//-------------------------------------------------------------
	void Update () 
	{
	}
	//-------------------------------------------------------------
	public void Init()
	{
		lbLvLock.text = "";
		lbEffect.text = "";
		Lock = false;
		SlotNumber = 0;
		PetGUID = 0;
		sEffecf = "";
		emAttrType = ENUM_FormationNodeAbility.Null;

		btnPet.InitialSlot();
	}
	//-------------------------------------------------------------
	public void SetSlot(int num,string effect,int PetID,int ForLV,int BGID,ENUM_FormationNodeAbility emType)
	{
		SlotNumber = num;
		PetGUID = PetID;
		sEffecf = effect;
		emAttrType = emType;

		S_FormationCost_Tmp sForCost = GameDataDB.FormationCostDB.GetData(ForLV);
		if(sForCost == null)
		{
			Lock = num*10 > ForLV;//備案
		}
		else
		{
			Lock = (SlotNumber + 1) > sForCost.iNodeAmount;
		}
		
		Utility.ChangeAtlasSprite(spBG,BGID);
		SetSlotDisplay();
	}

	public void SetspPlus()
	{
		spPlus.gameObject.SetActive(PetGUID <= 0 && Lock == false);
	}
	//-------------------------------------------------------------
	private void SetSlotDisplay()
	{
		lbEffect.color = Color.white;
		spLock.gameObject.SetActive(Lock);
		SetspPlus();
		if(Lock)
		{
			//int count = SlotNumber*10;
			int count = 0;
			int iSize = GameDataDB.FormationCostDB.GetDataSize();
			S_FormationCost_Tmp dbf = null;

			for(int i=1; i <= iSize; ++i)
			{
				dbf = GameDataDB.FormationCostDB.GetData(i);

				if(dbf == null)
					continue;

				if(dbf.iNodeAmount == (SlotNumber + 1))
				{
					count = dbf.GUID;
					break;
				}
			}

			lbLvLock.text = string.Format(GameDataDB.GetString(265),count);
			lbEffect.color = Color.gray;

		}
		else if(PetGUID > 0)
		{

			btnPet.SetSlotWithPetID(PetGUID,false,true);
		}
		else
		{
			//Utility.ChangeAtlasSprite(spPetFace,PetGUID); //空的要放一個+號的圖
		}

		lbEffect.text = sEffecf;
	}
}
