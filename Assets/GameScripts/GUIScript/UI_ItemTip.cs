using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_ItemTip : NGUIChildGUI
{
	public UIPanel			panelItemTip		= null;
	//一般道具
	public UISprite			SpriteItemInfoBG	= null;
	public UIButton			ButtonClose			= null;
	public UISprite			SpriteIcon			= null;
	public UILabel			LabelItemName		= null;
	public List<UISprite>	emptyStars 			= new List<UISprite>();
	public List<UISprite>	stars 				= new List<UISprite>();
	public UILabel			LabelItemNote		= null;
	public Transform		TSIconLocal			= null;
	public UILabel			lbItemNoteType		= null;
	public UIButton			btnItemLootInfo		= null;
	public UILabel			lbItemLootInfo		= null;
	//裝備
	public UISprite			SpriteEquipInfoBG	= null;
	public UISprite			SpriteEquipIcon		= null;
	public UILabel			LabelEquipName		= null;
	public UILabel			LabelEquipNote		= null;
	public Transform		EquipIconPos		= null;
	public UILabel			lbEquipNoteType		= null;
	public UIButton			btnEquipLootInfo	= null;
	public UILabel			lbEquipLootInfo		= null;
	//裝備屬性
	public UILabel			lbMainAttrTitle		= null;
	public UILabel			lbMainAttr			= null;
	public UILabel			lbMeltingLv			= null;
	public UITable			tbOtherAttr			= null;
	public UILabel			lbOtherAttrTitle	= null;
	public UILabel			lbOtherAttr			= null;
	public GameObject[]		gEquipSkill			= new GameObject[2];
	public EquipSkill[]		equipSkillArray		= new EquipSkill[2];
	public UIPanel			panelItemSkillNote	= null;
	public UIButton			btnCloseNote		= null;
	public UILabel			lbItemSkillNote		= null;
	[HideInInspector]public string			OtherAttrName 		= "Label(EqOtherAttr)";
	//
	public Slot_Item		itemSlot			= null;
	public Slot_Item		equipSlot			= null;
	[HideInInspector]public string			slotName 			= "Slot_Item";
	public UIButton			ButtonFullScreen	= null;
	//temp
	public List<int>		templList			= new List<int>(); //暫用特例排除不在道具格顯示數量清單
	//
	private int 			RecordItemDBFID		= 0;
	//
	private bool 			bTopBarESCFlag		= false;
	//
	[Header("PetUseCalssType")]
	public UIWidget			containerSet		= null;
	public UILabel			lbTypeName			= null;
	public UISprite			spCalss				= null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ItemTip";
	
	//-------------------------------------------------------------------------------------------------
	private UI_ItemTip() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		InitialUI();
		base.Initialize();
	}
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		//ReSetStar();
		CreateSlot();
		lbItemLootInfo.text = GameDataDB.GetString(472); 		// "掉落關卡"
		lbEquipLootInfo.text = GameDataDB.GetString(472); 		// "掉落關卡"
		lbMainAttrTitle.text = GameDataDB.GetString(1342); 	// "主要屬性"
		lbOtherAttrTitle.text = GameDataDB.GetString(1343); 	// "附加屬性"
		lbItemSkillNote.text = GameDataDB.GetString(5101); 		// "所有附加屬性達上限時啟動"
		UIEventListener.Get(btnItemLootInfo.gameObject).onClick			= OpenItemLootInfo;
		UIEventListener.Get(btnEquipLootInfo.gameObject).onClick		= OpenItemLootInfo;

		SpriteEquipInfoBG.gameObject.SetActive(true);

		if (gEquipSkill.Length == equipSkillArray.Length)
		{
			for(int i=0;i<gEquipSkill.Length;++i)
			{
				equipSkillArray[i] = new EquipSkill();
				Transform trans = gEquipSkill[i].transform.FindChild("Label(SkillNote)");
				if (trans != null)
				{
					equipSkillArray[i].lbEquipSkillName = trans.GetComponent<UILabel>();
				}
				trans = gEquipSkill[i].transform.FindChild("Button(ItemSkillNote)");
				if (trans != null)
				{
					equipSkillArray[i].btnEquipSkillNote = trans.GetComponent<UIButton>();
					UIEventListener.Get(equipSkillArray[i].btnEquipSkillNote.gameObject).onClick = OnBtnItemSkillClick;
				}
				trans = gEquipSkill[i].transform.FindChild("Sprite(SkillIcon)");
				if (trans != null)
				{
					equipSkillArray[i].spEquipSkillIcon = trans.GetComponent<UISprite>();
				}
				gEquipSkill[i].SetActive(false);
			}
		}

		UIEventListener.Get(btnCloseNote.gameObject).onClick = OnCloseItemSkillClick;

		SpriteEquipInfoBG.gameObject.SetActive(false);
		containerSet.gameObject.SetActive(false);
	}

	//-----------------------------------------------------------------------------------------------------
	public void Update()
	{
#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
			if(RecordItemDBFID > 0)
			{
				if(SpriteItemInfoBG.gameObject.activeSelf)
				{
					LabelItemName.text = LabelItemName.text +" "+ RecordItemDBFID.ToString();
				}
				else if(SpriteEquipInfoBG.gameObject.activeSelf)
				{
					LabelEquipName.text = LabelEquipName.text +" "+ RecordItemDBFID.ToString();
				}
			}
		}
#endif
	}

	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		EventDelegate.Add(ButtonFullScreen.onClick, HideItemTip);
	}
	//-------------------------------------------------------------------------------------------------
	void CreateSlot()
	{
		if(slotName == "")
		{
			slotName = "Slot_Item"; //GameDataDB.GetString(1305); //"Slot_GuildList";
		}
		
		Slot_Item go = ResourceManager.Instance.GetGUI(slotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Item load prefeb error,path:{0}", "GUI/"+slotName) );
			return;
		}

		//一般道具Slot
		itemSlot = Instantiate(go) as Slot_Item;
			
		itemSlot.transform.parent			= TSIconLocal;
		itemSlot.transform.localScale		= Vector3.one;
		itemSlot.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		itemSlot.transform.localPosition	= Vector3.zero;
		itemSlot.gameObject.SetActive(true);
			
		itemSlot.name = string.Format("slotItem00");
			
		itemSlot.InitialSlot();
		//暫用
		templList = itemSlot.specialList;

		//裝備Slot
		equipSlot = Instantiate(go) as Slot_Item;
		
		equipSlot.transform.parent			= EquipIconPos;
		equipSlot.transform.localScale		= Vector3.one;
		equipSlot.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		equipSlot.transform.localPosition	= Vector3.zero;
		equipSlot.gameObject.SetActive(true);
		
		equipSlot.name = string.Format("slotEquip00");
		
		equipSlot.InitialSlot();
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
		ARPGApplication.instance.m_uiTopStateView.quitEnable = bTopBarESCFlag;
	}
	//-----------------------------------------------------------------------------------------------------
	void ReSetStar()
	{
		for(int i=0; i<emptyStars.Count; ++i)
		{
			emptyStars[i].gameObject.SetActive(true);
			
			stars[i].gameObject.SetActive(false);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void HideItemTip()
	{
		//ReSetStar();
		SpriteItemInfoBG.gameObject.SetActive(false);
		SpriteEquipInfoBG.gameObject.SetActive(false);
		Hide();

	}
	//-----------------------------------------------------------------------------------------------------
	void ShowItemTmp(S_Item_Tmp dbf)
	{
		if(dbf == null)
			return;
		
		if(!IsVisible())
		{
//			Utility.ChangeAtlasSprite(SpriteIcon, dbf.ItemIcon);

			//檢查特殊顯示名稱
			if(templList.Contains(dbf.GUID) && itemSlot.iCount >0)
			{
				//還原Label顏色
				LabelItemName.color = Color.white;
				LabelItemName.text = string.Format("{0} {1}", itemSlot.iCount, dbf.GetNameWithColor());
			}
			else
			{
				LabelItemName.text = GameDataDB.GetString(dbf.iName);
				dbf.SetRareColorString(LabelItemName);
			}

			string str = GameDataDB.GetString(dbf.iNote);
			str = ARPGApplication.instance.m_StringParsing.Parsing(str,null,SkillLevelType.Now);
			LabelItemNote.text = str;
			/*
			//物品星等顯示
			for(int i=0; i<stars.Count; ++i)
			{
				stars[i].gameObject.SetActive(false);
				if(i<(int)dbf.RareLevel)
				{
					stars[i].gameObject.SetActive(true);
				}
			}*/

			//顯示
			Show();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//顯示一般道具
	void ShowItemTmp(S_Item_Tmp itemTmp,bool bShowLootInfo)
	{
		if(!IsVisible())
		{
			//預設先關閉寵物類型tag
			containerSet.gameObject.SetActive(false);

			string str;
			lbItemNoteType.gameObject.SetActive(false);
//			Utility.ChangeAtlasSprite(SpriteIcon, dbf.ItemIcon);

			//檢查特殊顯示名稱
			if(templList.Contains(itemTmp.GUID) && itemSlot.iCount >1)
			{
				//還原Label顏色
				LabelItemName.color = Color.white;
				LabelItemName.text = string.Format("{0} {1}", itemSlot.iCount, itemTmp.GetNameWithColor());
			}
			else
			{
				LabelItemName.text = GameDataDB.GetString(itemTmp.iName);
				itemTmp.SetRareColorString(LabelItemName);
			}
			//類型說明
			if(itemTmp.iItemNote>0)
			{
				str = GameDataDB.GetString(itemTmp.iItemNote);
				if(str != null)
				{
					lbItemNoteType.gameObject.SetActive(true);
					lbItemNoteType.text = str;
				}
			}

			str = GameDataDB.GetString(itemTmp.iNote);
			str = ARPGApplication.instance.m_StringParsing.Parsing(str,null,SkillLevelType.Now);
			LabelItemNote.text = str;
			//------------------------------------------------------------------
			//掉落關卡資訊
			if(bShowLootInfo)
			{
				//掉落連結按鈕顯示與否
				bool bshowLoot = true;
				//若Note為貨幣、稀有道具、玩家法寶、時裝、翅膀、夥伴法寶類型時即不顯示
				switch(itemTmp.iItemNote)
				{
				case 0:
				case 5000:
				case 5002:
				case 5011:
				case 5012:
				case 5013:
				case 5022:
					bshowLoot = false;
					break;
				}
				btnItemLootInfo.gameObject.SetActive(bshowLoot);
			}
			else
				btnItemLootInfo.gameObject.SetActive(bShowLootInfo);
			//例外處理 在符合上面條件時 但又沒有掉落資訊即隱藏按鈕
			if(bShowLootInfo)
			{
				bool bShowLinks = false;
				for(int i=0;i<GameDefine.ITEM_DROPDUNGEON_MAX;++i)
				{
					if(itemTmp.iDropDungeonID[i]>0)
						bShowLinks = true;
				}
				btnItemLootInfo.gameObject.SetActive(bShowLinks);
			}
			//碎片顯示型態tag
			if(itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Pet ||
			   itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece)
			{
				int PetID = ARPGApplication.instance.ItemIDSwitchToPetID(itemTmp.GUID);
				containerSet.gameObject.SetActive(PetID>0);
				if(PetID>0)
				{
					S_PetData_Tmp pdTmp = GameDataDB.PetDB.GetData(PetID);
					containerSet.gameObject.SetActive(pdTmp != null);
					if(pdTmp != null)
					{
						lbTypeName.text = GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(pdTmp.emCharType));
						Utility.ChangeAtlasSprite(spCalss,ARPGApplication.instance.GetPetCalssIconID(pdTmp.emCharClass));
					}
				}

			}
			//------------------------------------------------------------------
			/*
			//物品星等顯示
			if((int)dbf.RareLevel == 0)
			{
				for(int i=0; i<emptyStars.Count; ++i)
				{
					emptyStars[i].gameObject.SetActive(false);
					
					stars[i].gameObject.SetActive(false);
				}
			}
			else
			{
				for(int i=0; i<stars.Count; ++i)
				{
					stars[i].gameObject.SetActive(false);
					if(i<(int)dbf.RareLevel)
					{
						stars[i].gameObject.SetActive(true);
					}
				}
			}*/
			SpriteItemInfoBG.gameObject.SetActive(true);
			//顯示
			Show();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//顯示裝備
	void ShowEquipTmp(S_Item_Tmp itemTmp,bool bShowLootInfo = true)
	{
		if(!IsVisible())
		{
			//Hide寵物碎片型態資訊
			containerSet.gameObject.SetActive(false);

			lbEquipNoteType.gameObject.SetActive(false);
			//檢查特殊顯示名稱
			if(templList.Contains(itemTmp.GUID) && equipSlot.iCount >1)
			{
				//還原Label顏色
				LabelEquipName.color = Color.white;
				LabelEquipName.text = string.Format("{0} {1}", equipSlot.iCount, itemTmp.GetNameWithColor());
			}
			else
			{
				LabelEquipName.text = GameDataDB.GetString(itemTmp.iName);
				itemTmp.SetRareColorString(LabelEquipName);
			}
			string str;
			//類型說明
			if(itemTmp.iItemNote>0)
			{
				str = GameDataDB.GetString(itemTmp.iItemNote);
				if(str != null)
				{
					lbEquipNoteType.gameObject.SetActive(true);
					lbEquipNoteType.text = str;
				}
			}
			
			str = GameDataDB.GetString(itemTmp.iNote);
			str = ARPGApplication.instance.m_StringParsing.Parsing(str,null,SkillLevelType.Now);
			LabelEquipNote.text = str;
			//------------------------------------------------------------------
			//掉落關卡資訊
			if(bShowLootInfo)
			{
				//掉落連結按鈕顯示與否
				bool bshowLoot = true;
				//若Note為貨幣、稀有道具、玩家法寶、時裝、翅膀、夥伴法寶類型時即不顯示
				switch(itemTmp.iItemNote)
				{
				case 0:
				case 5000:
				case 5002:
				case 5011:
				case 5012:
				case 5013:
				case 5022:
					bshowLoot = false;
					break;
				}
				btnEquipLootInfo.gameObject.SetActive(bshowLoot);
			}
			else
				btnEquipLootInfo.gameObject.SetActive(bShowLootInfo);
			//例外處理 在符合上面條件時 但又沒有掉落資訊即隱藏按鈕
			if(bShowLootInfo)
			{
				bool bShowLinks = false;
				for(int i=0;i<GameDefine.ITEM_DROPDUNGEON_MAX;++i)
				{
					if(itemTmp.iDropDungeonID[i]>0)
						bShowLinks = true;
				}
				btnEquipLootInfo.gameObject.SetActive(bShowLinks);
			}
			//------------------------------------------------------------------
			//設定熔煉等級
			lbMeltingLv.text = string.Format(GameDataDB.GetString(256),0,itemTmp.iMeltingLimit);

			//裝備主要屬性
			lbMainAttr.text = itemTmp.GetEquipMainAttrString();

			//裝備附加屬性
			string otherAttr = itemTmp.GetEquipOtherAttrString();
			if (String.IsNullOrEmpty(otherAttr) == false)
			{
				lbOtherAttr.text = otherAttr;
			}
			lbOtherAttrTitle.gameObject.SetActive(String.IsNullOrEmpty(otherAttr)==false);

			SetItemSkillString(itemTmp);

			tbOtherAttr.enabled = true;
			tbOtherAttr.Reposition();

			SpriteEquipInfoBG.gameObject.SetActive(true);
			//顯示
			Show();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void ShowItemTmpByItemSlot(Slot_Item slot , bool bShowLootInfo = true)
	{
		//
		bTopBarESCFlag = ARPGApplication.instance.m_uiTopStateView.quitEnable;
		ARPGApplication.instance.m_uiTopStateView.quitEnable = false;
		//
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(slot.itemGUID);
		if (itemTmp == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("道具格顯示TIP錯誤 道具編號{0}", slot.itemGUID));
			return;
		}
		//紀綠下itemDBFID
		RecordItemDBFID = slot.itemGUID;
		
		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			equipSlot.itemGUID  = slot.itemGUID;
			equipSlot.iCount  	= slot.iCount;
			equipSlot.SetSlotWithCount(equipSlot.itemGUID, equipSlot.iCount , false);
			ShowEquipTmp(itemTmp,bShowLootInfo);
		}
		else
		{
			itemSlot.itemGUID  	= slot.itemGUID;
			itemSlot.iCount  	= slot.iCount;
			itemSlot.SetSlotWithCount(itemSlot.itemGUID, itemSlot.iCount , false);
			ShowItemTmp(itemTmp,bShowLootInfo);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void ShowItemTmpWithCount(int guid, int count, bool bShowLootInfo = true)
	{
		//
		bTopBarESCFlag = ARPGApplication.instance.m_uiTopStateView.quitEnable;
		ARPGApplication.instance.m_uiTopStateView.quitEnable = false;
		//
		S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(guid);
		if (itemTmp == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("道具格顯示TIP錯誤 道具編號{0}", guid));
			return;
		}
		//紀綠下itemDBFID
		RecordItemDBFID = guid;
		//
		ARPGApplication.instance.m_uiTopStateView.quitEnable = false;
		//
		if (itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Weapen || itemTmp.ItemType == ENUM_ItemType.ENUM_ItemType_Armor)
		{
			equipSlot.itemGUID  = guid;
			equipSlot.iCount  	= count;
			equipSlot.SetSlotWithCount(equipSlot.itemGUID, equipSlot.iCount , false);
			ShowEquipTmp(itemTmp,bShowLootInfo);
		}
		else
		{
			itemSlot.itemGUID  	= guid;
			itemSlot.iCount  	= count;
			itemSlot.SetSlotWithCount(itemSlot.itemGUID, itemSlot.iCount , false);
			ShowItemTmp(itemTmp,bShowLootInfo);
		}
			
	}
	//-----------------------------------------------------------------------------------------------------
	//合成&升階UI用
	private void SetItemSkillString(S_Item_Tmp itemTmp)
	{
		//裝備被動技能字串
		string [] strEqSkill = itemTmp.GetEquipSkillString();
		if (strEqSkill.Length != equipSkillArray.Length)
		{
			UnityDebugger.Debugger.Log("EquipSkill String Number "+strEqSkill.Length+" != EquipSkill UI "+equipSkillArray.Length);
		}
		else
		{
			for(int i=0;i<equipSkillArray.Length;++i)
			{
				if (String.IsNullOrEmpty(strEqSkill[i]) == false)
				{
					equipSkillArray[i].lbEquipSkillName.text = strEqSkill[i];
					equipSkillArray[i].spEquipSkillIcon.color = Color.white;
					
					S_SkillData_Tmp [] skillTmpArray = new S_SkillData_Tmp[GameDefine.ITEMSKILL_ACTIVE_LEVEL];
					if (i == 0)
					{
						for(int m=0; m < itemTmp.iSpecialAbilityArray_1.Length; ++m)
						{
							if (m >= skillTmpArray.Length)
								break;
							skillTmpArray[m] = itemTmp.GetItemSkillData_1(m);
						}
						int itemSkillLV = itemTmp.CheckSpecialSkill_1();
						if (itemSkillLV < 0)
						{
							gEquipSkill[i].SetActive(false);
						}
						else
						{
							List<string> skillNoteList = new List<string>();
							for(int m=0; m < skillTmpArray.Length; ++m)
							{
								if (skillTmpArray[m] != null)
								{
									int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_1(m);
									string strTitle = string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[m].SkillName));
									skillNoteList.Add(strTitle);
									string strNote = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skillTmpArray[m].iNote),null,SkillLevelType.Now);
									skillNoteList.Add(strNote);
								}
							}
							if (skillNoteList.Count > 0)
								skillNoteList[0] = "[88]"+skillNoteList[0];
							
							equipSkillArray[i].btnEquipSkillNote.userData = skillNoteList;
							Utility.ChangeAtlasSprite(equipSkillArray[i].spEquipSkillIcon,skillTmpArray[itemSkillLV].ACTField);
							gEquipSkill[i].SetActive(true);
						}
					}
					else if (i == 1)
					{
						for(int m=0; m < itemTmp.iSpecialAbilityArray_2.Length; ++m)
						{
							if (m >= skillTmpArray.Length)
								break;
							skillTmpArray[m] = itemTmp.GetItemSkillData_2(m);
						}
						int itemSkillLV = itemTmp.CheckSpecialSkill_2();
						if (itemSkillLV < 0)
						{
							gEquipSkill[i].SetActive(false);
						}
						else
						{
							List<string> skillNoteList = new List<string>();
							for(int m=0; m < skillTmpArray.Length; ++m)
							{
								if (skillTmpArray[m] != null)
								{
									int titleStringID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemSkillTitle_2(m);
									string strTitle = string.Format(GameDataDB.GetString(titleStringID), GameDataDB.GetString(skillTmpArray[m].SkillName));
									skillNoteList.Add(strTitle);
									string strNote = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(skillTmpArray[m].iNote),null,SkillLevelType.Now);
									skillNoteList.Add(strNote);
								}
							}
							if (skillNoteList.Count > 0)
								skillNoteList[0] = "[88]"+skillNoteList[0];
							
							equipSkillArray[i].btnEquipSkillNote.userData = skillNoteList;
							Utility.ChangeAtlasSprite(equipSkillArray[i].spEquipSkillIcon,skillTmpArray[itemSkillLV].ACTField);	
							gEquipSkill[i].SetActive(true);
						}
					}
				}
				else
					gEquipSkill[i].SetActive(false);
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
/*	public void ShowPetItemTmpWithCount(int petGuid, int count)
	{
//		S_Item_Tmp

		itemSlot.itemGUID  	  = guid;
		itemSlot.iCount  	  = count;
		//		itemSlot.specialList  = slot.specialList;
		
		itemSlot.SetSlotWithCount(itemSlot.itemGUID, itemSlot.iCount , false);
		
		ShowItemTmp(itemSlot.itemGUID);
	}
*/
	//-----------------------------------------------------------------------------------------------------
	//移動TIP位置
	public void MoveTipPosition(float x, float y, float z)
	{
		SpriteItemInfoBG.transform.localPosition = new Vector3(x,y,z);
	}
	//-----------------------------------------------------------------------------------------------------
	//移動TIP位置
	public void MoveTipPosition(Vector3 pos)
	{
		SpriteItemInfoBG.transform.localPosition = pos;
	}
	//-----------------------------------------------------------------------------------------------------
	//檢查特殊顯示名稱
//	public void MoveTipPosition(Vector3 pos)
//	{
//		SpriteItemInfoBG.transform.localPosition = pos;
//	}
	//-----------------------------------------------------------------------------------------------------
	private void OpenItemLootInfo(GameObject gb)
	{
		if(RecordItemDBFID <=0)
			return;

		ARPGApplication.instance.m_uiItemDungeonLink.ShowItemDungeonLink(RecordItemDBFID);
	}
	//-----------------------------------------------------------------------------------------------------
	private void OnBtnItemSkillClick(GameObject go)
	{
		UIButton btn = go.GetComponent<UIButton>();
		if (btn == null)
			return;
		if (btn.userData == null)
			return;
		List<string> stringList = btn.userData as List<string>;
		//填入字串前先清空
		lbItemSkillNote.text = "";
		for(int i=0; i < stringList.Count; ++i)
		{
			lbItemSkillNote.text += stringList[i]+"\n";
		}
		lbItemSkillNote.text += "\n[FF]"+GameDataDB.GetString(5141);
		panelItemSkillNote.gameObject.SetActive(true);
	}
	//-----------------------------------------------------------------------------------------------------
	private void OnCloseItemSkillClick(GameObject go)
	{
		panelItemSkillNote.gameObject.SetActive(false);
	}
}
