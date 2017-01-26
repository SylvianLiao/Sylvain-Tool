using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagNonUsePetData : MonoBehaviour 
{
	public UILabel		lbPetName			= null;					//名稱
	public UILabel		lbPetLevel			= null;					//等級
	public UISprite[]	Stars				= new UISprite[4];		//突破星等
	public UIWidget		IconLoc				= null;
	public UIToggle[]	tgEquips			= new UIToggle[5];		//Toggle
	public UISprite[]	EquipIcons			= new UISprite[5];		//裝備Icon
	public UILabel[]	lbStrengthens		= new UILabel[5];		//強化數
	public UISprite[]	Meltings			= new UISprite[5];		//熔煉顯示
	public UISprite[]	Masks				= new UISprite[5];		//邊框
	public UISprite[]	BackGrounds			= new UISprite[5];		//背景圖
	public UISprite		spriteSelectBorder	= null;					//選擇框
	//
	public UIToggle		BkToggle			= null;		// 隱藏用toggle(讓畫面上不顯示click)
	//[HideInInspector]
	//bool			bSelected			= false;
	[HideInInspector]
	public Slot_Item	PetIcon;
	//
	[HideInInspector]
	public int			iListIndex			=-1;
	//
	[HideInInspector]
	public S_PetData pData	= null;
	//
	private Dictionary<ENUM_WearPosition,S_ItemData> PetEquipList	= new Dictionary<ENUM_WearPosition, S_ItemData>();
	//

	public delegate void OnClickEquipBlock(ENUM_WearPosition wp,S_ItemData iData,int iIndex,S_PetData pd);
	public OnClickEquipBlock onClickEquipBlock;
	public delegate void OnSelectThisSlot(int iIndex,S_PetData pd);
	public OnSelectThisSlot onSelectThisSlot;
	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		for(int i=0;i<tgEquips.Length;++i)
		{
			ENUM_WearPosition wp = ENUM_WearPosition.ENUM_WearPosition_None;
			UIEventListener.Get(tgEquips[i].gameObject).onClick		+= ClickEquipBlockEvent;
			switch(i)
			{
			case 0:
				wp = ENUM_WearPosition.ENUM_WearPosition_Weapon;
				break;
			case 1:
				wp = ENUM_WearPosition.ENUM_WearPosition_Clothes;
				break;
			case 2:
				wp = ENUM_WearPosition.ENUM_WearPosition_Necklace;
				break;
			case 3:
				wp = ENUM_WearPosition.ENUM_WearPosition_Ring;
				break;
			case 4:
				wp = ENUM_WearPosition.ENUM_WearPosition_Talisman;
				break;
			}
			tgEquips[i].gameObject.GetComponent<UIButton>().userData = wp;
		}
		//
		UIEventListener.Get(this.gameObject).onClick			+= SelectThisSlotEvent;
	}
	//-------------------------------------------------------------------------------------------------
	private void SelectThisSlotEvent(GameObject gb)
	{
		if(onSelectThisSlot != null)
			onSelectThisSlot(iListIndex,pData);

		spriteSelectBorder.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	private void ClickEquipBlockEvent(GameObject gb)
	{
		S_ItemData ItemData = null;
		ENUM_WearPosition wp = (ENUM_WearPosition)gb.GetComponent<UIButton>().userData;


		if (wp == ENUM_WearPosition.ENUM_WearPosition_None)
			return;

		if(PetEquipList.ContainsKey(wp))
			ItemData = PetEquipList[wp];
		else
		{
			ItemData = null;
			wp = ENUM_WearPosition.ENUM_WearPosition_None;
		}

		//
		if(onClickEquipBlock != null)
			onClickEquipBlock(wp,ItemData,iListIndex,pData);

		if(ItemData!= null)
			gb.GetComponent<UIToggle>().value = true;
		else
			BkToggle.value= true;

		spriteSelectBorder.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//設定顯示資料
	public void SetData(S_PetData pd,int iRealIndex)
	{
		if(pd == null)
			return;

		iListIndex = iRealIndex;
		//
		pData = pd;
		lbPetName.text 		= GameDataDB.GetString(pd.GetPetName());
		lbPetLevel.text		= GameDataDB.GetString(1056)+" "+pd.iPetLevel.ToString();
		//突破星數
		for(int i=0;i<Stars.Length;++i)
		{
			if(i<pd.iPetLimitLevel)
				Stars[i].gameObject.SetActive(true);
			else
				Stars[i].gameObject.SetActive(false);
		}
		//設定寵物圖像
		//CreatePetIcon();
		int ItemID = pData.GetPetItemID();
		PetIcon.SetSlotWithCount(ItemID,1,false,true);
		//設定收集寵物擁有的裝備
		SetCollectPetEquipDatas();
		//設定同一個Group
		for(int i=0;i<tgEquips.Length;++i)
			tgEquips[i].group = 20+iRealIndex;

		BkToggle.group = 20+iRealIndex;
		BkToggle.startsActive = true;
		BkToggle.value = true;
	}
	//-------------------------------------------------------------------------------------------------
	/*private void CreatePetIcon()
	{
		//先清掉IconLoc底下的物件
		for(int i = 0;i<IconLoc.transform.childCount;i++){
			GameObject gb = IconLoc.transform.GetChild(i).gameObject;
			Destroy(gb);
		}

		int ItemID = pData.GetPetItemID();
		//生成
		Slot_Item go = ResourceManager.Instance.GetGUI("Slot_Item").GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Item load prefeb error") );
			return;
		}
		
		Slot_Item newgo= Instantiate(go) as Slot_Item;

		newgo.transform.parent			= IconLoc.transform;
		newgo.transform.localScale		= Vector3.one;
		newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);
		newgo.transform.localPosition	= Vector3.zero;
		
		newgo.gameObject.SetActive(true);
		newgo.SetDepth(20);
		//設定
		newgo.SetSlotWithCount(ItemID,1,false);
	}*/
	//-------------------------------------------------------------------------------------------------
	public void SetCollectPetEquipDatas()
	{
		//收集
		PetEquipList.Clear();
		//擷取相對應裝備的物品
		foreach(S_ItemData tempItem in ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.ItemBag.Values)
		{
			if(tempItem==null)
				continue;
			//剔除未裝備的
			if(tempItem.emWearPos == ENUM_WearPosition.ENUM_WearPosition_None)
				continue;

			if(tempItem.iTargetID == pData.iPetDBFID)
				PetEquipList.Add(tempItem.emWearPos,tempItem);
		}
		//先清除圖 強化數 熔煉 層級換色
		for(int i=0;i<EquipIcons.Length;++i)
		{
			Utility.ChangeAtlasSprite(EquipIcons[i],-1);
			lbStrengthens[i].gameObject.SetActive(false);
			Meltings[i].gameObject.SetActive(false);
			Masks[i].color = Color.white;
			BackGrounds[i].color = Color.white;
			EquipIcons[i].gameObject.SetActive(false);
			Utility.ChangeAtlasSprite(Masks[i],17060);
			//
			switch(i)
			{
			case 0:
				Utility.ChangeAtlasSprite(BackGrounds[i],2005);	//武器預設圖片
				break;
			case 1:
				Utility.ChangeAtlasSprite(BackGrounds[i],2006);	//衣服預設圖片
				break;
			case 2:
				Utility.ChangeAtlasSprite(BackGrounds[i],2007);	//項鍊預設圖片
				break;
			case 3:
				Utility.ChangeAtlasSprite(BackGrounds[i],2008);	//戒指預設圖片
				break;
			case 4:
				Utility.ChangeAtlasSprite(BackGrounds[i],1145);	//法寶預設圖片
				break;
			}
		}
		//再設定
		if(PetEquipList.Count !=0)
		{
			foreach(ENUM_WearPosition wp in PetEquipList.Keys)
			{
				S_Item_Tmp itemTmp = GameDataDB.ItemDB.GetData(PetEquipList[wp].ItemGUID);
				//換圖
				EquipIcons[(int)wp].gameObject.SetActive(true);
				Utility.ChangeAtlasSprite(EquipIcons[(int)wp],itemTmp.ItemIcon);
				//層級換色
//				itemTmp.SetRareColor(Masks[(int)wp],BackGrounds[(int)wp]);
				itemTmp.SetItemRarity(Masks[(int)wp],BackGrounds[(int)wp]);

				//物品強化數 
				if(PetEquipList[wp].iInherit[0]>0)
				{
					lbStrengthens[(int)wp].gameObject.SetActive(true);
					lbStrengthens[(int)wp].text = string.Format("+{0}",PetEquipList[wp].iInherit[0]);	//LabelStrengthen
				}
				//熔煉顯示
				if(PetEquipList[wp].iExp >0 || PetEquipList[wp].iMeltingLV>0)
				{
					Meltings[(int)wp].gameObject.SetActive(true);
				}
				else
				{
					Meltings[(int)wp].gameObject.SetActive(false);
				}
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void CancelSelect()
	{
		BkToggle.group = 0;
		BkToggle.value = true;
		for(int i=0;i<tgEquips.Length;++i)
		{
			tgEquips[i].group = 0;
			tgEquips[i].value = false;
		}



		spriteSelectBorder.gameObject.SetActive(false);
		//bSelected = false;
	}
	//-------------------------------------------------------------------------------------------------
	public void CancelToggleSelect()
	{
		BkToggle.group = 0;
		BkToggle.value = true;
		for(int i=0;i<tgEquips.Length;++i)
		{
			tgEquips[i].group = 0;
			tgEquips[i].value = false;
		}

	}
	//-------------------------------------------------------------------------------------------------
	public void SetToggleSelect(ENUM_WearPosition wearPos)
	{
		int RecordGroup = BkToggle.group;
		BkToggle.group = 0;
		BkToggle.value = false;
		if(wearPos == ENUM_WearPosition.ENUM_WearPosition_None)
			BkToggle.value = true;

		for(int i=0;i<tgEquips.Length;++i)
		{
			tgEquips[i].group = 0;
			tgEquips[i].value = false;
			if(i == (int)wearPos)
				tgEquips[i].value = true;
		}

		for(int i=0;i<tgEquips.Length;++i)
		{
			tgEquips[i].group = RecordGroup;
		}
		BkToggle.group = RecordGroup;
	}
	//-------------------------------------------------------------------------------------------------
}
