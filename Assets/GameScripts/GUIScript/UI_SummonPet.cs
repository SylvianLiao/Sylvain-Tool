using System;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class UI_SummonPet : NGUIChildGUI 
{
	public UIPanel					panelBase				= null; //召喚介面
	//金錢召喚
	public UILabel		lbMoneyNote						= null;
	public UIButton		btnM_SummonOnce					= null; //金錢召喚一次
	public UILabel		lbM_SummonOnceStatus			= null;
	public UILabel		lbM_OnceBtnText					= null;
	public UILabel		lbM_OncePrice					= null;
	public UIButton		btnM_MultiSummon				= null; //金錢召喚十次
	public UILabel		lbM_MultiBtnText				= null;
	public UILabel		lbM_MultiPrice					= null;
	//寶石召喚
	public UISprite		spriteDiamondSummonSet			= null; //寶石召喚群組
	public UILabel		lbDiamondNote					= null;
	public UIButton		btnD_SummonOnce					= null; //寶石召喚一次
    public UILabel		lbD_SummonOnceNote1	    		= null;
    public UILabel		lbD_SummonOnceNote2		    	= null;
    public UILabel		lbD_SummonOnceNote3		    	= null;    
	public UILabel		lbD_SummonOnceStatus			= null;
	public UILabel		lbD_OnceBtnText					= null;
	public UILabel		lbD_OncePrice					= null;
	public UIButton		btnD_MultiSummon				= null; //寶石召喚十次
	public UILabel		lbD_MultiStatus					= null;
	public UILabel		lbD_MultiBtnText				= null;
	public UILabel		lbD_MultiPrice					= null;
	//VIP召喚
	public UISprite		spriteVIPSummonSet				= null; //VIP召喚群組
	public UILabel		lbVIPNote						= null;
	public UISprite		spritePetIcon					= null; //主題夥伴頭像
	public UIButton		btnV_SummonOnce					= null; //VIP召喚一次
	public UILabel		lbV_OnceStatus					= null;
	public UILabel		lbV_OnceBtnText					= null;
	public UILabel		lbV_OncePrice					= null;
	[System.NonSerialized]
	public int			VIPLimitLV						= 99;
	//UI Tween的物件
	public TweenWidth	tweenBackGround					= null;
	//Grid
	public UIGrid		GridSummonBoard					= null;
	public UIGrid		GridSummonCard					= null;
	public GameObject	VIPCard							= null;
	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UISprite		spGuideMoneySummon				= null; //導引每日可免費青銅召喚一次
	public UILabel		lbGuideMoneySummon				= null;
	public UISprite		spGuideSummon					= null; //導引點擊免費青銅召喚
	public UILabel		lbGuideSummon					= null;
	public UISprite		spGuideDiamondSummon			= null; //導引每日可免費史詩召喚一次
	public UILabel		lbGuideDiamondSummon			= null;
	public UILabel		lbGuideFinish					= null;	//導引教學結束
	public UISprite		spGuideQuit						= null;	//導引點擊離開召喚UI
	public UILabel		lbGuideQuit						= null;
	public UIButton		btnFakeQuit						= null;	//新手教學專用離開按鈕
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_SummonPet";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_SummonPet() : base(GUI_SMARTOBJECT_NAME)
	{		
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
	}
	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		Init ();
	}
	//-----------------------------------------------------------------------------------------------------
	void Init()
	{
		//找出開放主題召喚的VIP等級
		SetVIPsummonVIPLV();
		//
		lbMoneyNote.text 			= GameDataDB.GetString(1106); //隨機獲得1~3星物品
		//lbDiamondNote.text			= GameDataDB.GetString(1107); //隨機獲得3~6星物品
		lbVIPNote.text				= GameDataDB.GetString(1108); //隨機獲得主題夥伴或4星以上夥伴碎片
		//按鈕字樣
		lbM_OnceBtnText.text 		= GameDataDB.GetString(1101); //召喚一次(錢幣)
		lbM_MultiBtnText.text		= GameDataDB.GetString(1102); //召喚十次(錢幣)
		lbD_OnceBtnText.text		= GameDataDB.GetString(1101); //召喚一次(寶石)
		lbD_MultiBtnText.text		= GameDataDB.GetString(1102); //召喚十次(寶石)
		lbV_OnceBtnText.text		= GameDataDB.GetString(2505); //召喚(VIP)
		if(VIPLimitLV >= 99)
			lbV_OnceStatus.gameObject.SetActive(false);
		else
			lbV_OnceStatus.text			= string.Format(GameDataDB.GetString(1111),VIPLimitLV); //VIP?召喚
        lbD_SummonOnceNote1.text	= GameDataDB.GetString(1116); //再招換
        lbD_SummonOnceNote2.text	= GameDataDB.GetString(1117); //次
        lbD_SummonOnceNote3.text	= GameDataDB.GetString(1118); //必得橙色夥伴
		//
		//價格
		lbM_MultiPrice.text			= GameDefine.ITEMMALL_PETLOTTERY_EACH_FPEX.ToString();  	  //90000
		lbD_MultiPrice.text			= (GetEpicSummonCost(GameDefine.ITEMMALL_BUY_PET_ID)*GameDefine.ITEMMALL_PETLOTTERY_EX_COUNT*0.9).ToString();  //2800
		lbV_OncePrice.text			= GetEpicSummonCost(GameDefine.ITEMMALL_BUY_VIP_ID).ToString();
	}
	//-----------------------------------------------------------------------------------------------------
	public int GetEpicSummonCost(int IdNum)
	{
		S_ShopPrize_Tmp shopTmp = GameDataDB.ShopPrizeDB.GetData(IdNum);
		if (shopTmp != null)
		{
			int summonPetCost = shopTmp.GetPrize(0);
			if (summonPetCost > 0)
				return summonPetCost;
			else
				return 0;
		}
		else
			return 0;
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	//找出開放主題召喚的VIP等級
	private void SetVIPsummonVIPLV()
	{
		GameDataDB.VIPLVDB.ResetByOrder();
		for(int i=1;i<=GameDataDB.VIPLVDB.GetDataSize();++i)
		{
			S_VIPLV_Tmp vipTmp = GameDataDB.VIPLVDB.GetData(i);
			if(vipTmp.PetSummonSwitch == GameDefine.VIP_FUNCTION_ON)
			{
				VIPLimitLV = vipTmp.GUID-1;
				break; 
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------
}
