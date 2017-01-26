using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_LevelUpInfo : NGUIChildGUI 
{
	public UITexture		TexturePlayer	= null;
	public UISprite			SpritePlayerBG	= null;

	public UILabel			LabelLVTitle	= null;
	public UILabel			LabelLVValue	= null;
	public UILabel			LabelLVValueNew	= null;

	public UILabel			LabelAttTitle	= null;
	public UILabel			LabelAttValue	= null;

	public UILabel			LabelDefTitle	= null;
	public UILabel			LabelDefValue	= null;

	public UILabel			LabelHPTitle	= null;
	public UILabel			LabelHPValue	= null;
	
	public Transform		PlayerContainer = null;
	public UILabel			NameLabel		= null;

	public UILabel			LabelAPTitle	= null;
	public UILabel			LabelAPValue	= null;
	public UILabel			LabelAPValueNew	= null;
	public UISprite			SpriteArrow		= null;

	//突破
	public UISprite			SpritePlayerBG2	= null;
	
	public UILabel			LabelLVTitle2	= null;
	public UILabel			LabelLVValue2	= null;
	public UILabel			LabelLVValueNew2	= null;
	
	public UILabel			LabelAttTitle2	= null;
	public UILabel			LabelAttValue2	= null;
	
	public UILabel			LabelDefTitle2	= null;
	public UILabel			LabelDefValue2	= null;
	
	public UILabel			LabelHPTitle2	= null;
	public UILabel			LabelHPValue2	= null;
	
	public UISprite			spriteUnlockSkillBase = null;
	public UILabel			LabelSkillName	= null;
	public UILabel			LabelSkillNote	= null;

	public UILabel			NameLabel2		= null;

	public TweenPosition	tpPlayer		= null;

	float    from_X	= 0;
	float    from_Y	= 0;
	float    to_X	= 0;
	float    to_Y	= 0;

	public float delayTime = 2.0f;

	public List<TweenPosition>	tpList		= new List<TweenPosition>();
	public List<TweenAlpha>	taList			= new List<TweenAlpha>();
	public List<TweenScale>	tsList			= new List<TweenScale>();

	//
	public ENUM_LevelUpType	UpLevelType		= ENUM_LevelUpType.ENUM_LevelUpInfo;
	//
	private bool 			bShowSkillData	= false;

	public string			LevelUpMusic 		= "Sound_System_017";	//升級音效
	public string			LimitLevelUpMusic 	= "Sound_System_018";	//夥伴突破音樂

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_LevelUpInfo";

	//-------------------------------------------------------------------------------------------------
	private UI_LevelUpInfo() : base(GUI_SMARTOBJECT_NAME)
	{

	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		SpritePlayerBG2.gameObject.SetActive(false);
		SpritePlayerBG.gameObject.SetActive(true);
//		BoxCollider collider = SpritePlayerBG.gameObject.GetComponentInChildren<BoxCollider>();
//		collider.enabled = false;
		LabelLVTitle.text 	= GameDataDB.GetString(2451);	//等級   2451
		LabelAttTitle.text	= GameDataDB.GetString(2452);	//攻擊力 2452
		LabelDefTitle.text	= GameDataDB.GetString(2453);	//防禦力 2453
		LabelHPTitle.text	= GameDataDB.GetString(2454);	//血量   2454
		LabelAPTitle.text	= GameDataDB.GetString(2475);	//體力

		spriteUnlockSkillBase.gameObject.SetActive(false);
		from_X = PlayerContainer.position.x;
		to_X = tpPlayer.to.x;

//		tpPlayer.from = new Vector3(from_X, locDBF.LeftSpriteY);
		//設定終點
//		tpPlayer.to	= new Vector3(to_X,locDBF.LeftSpriteY);
	}

	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		if(UpLevelType == ENUM_LevelUpType.ENUM_LimitLevelUp)
		{
			SpritePlayerBG.gameObject.SetActive(false);
			SpritePlayerBG2.gameObject.SetActive(true);
			LabelLVTitle2.text 		= GameDataDB.GetString(2602);			//突破 2602
			LabelAttTitle2.text		= GameDataDB.GetString(2452);			//攻擊力 2452
			LabelDefTitle2.text		= GameDataDB.GetString(2453);			//防禦力 2453
			LabelHPTitle2.text		= GameDataDB.GetString(2454);			//血量   2454
			spriteUnlockSkillBase.gameObject.SetActive(bShowSkillData);
		}
	}

	//-------------------------------------------------------------------------------------------------
	public void SetLevelUpInfoPlayer(int oldLV, int newLV)
	{
		UnityDebugger.Debugger.Log(string.Format("Player LevelUp Info {0} {1}", oldLV, newLV));

		// 設定自己的大圖
		//texture編號
		int textureIndex = ARPGApplication.instance.m_RoleSystem.GetRoleTexture();

		//設定texture位置
		//S_SpriteCoordinate_Tmp locDBF = GameDataDB.SpriteCoordinate.GetData(textureIndex);
		//if(locDBF == null)
		//{
		//	UnityDebugger.Debugger.LogError(string.Format("十三漏填DBF! 十三甲賽! SpriteCoordinate null textureIndex {0}", textureIndex));
		//}
		//else
		{
			//設定起點
            //tpPlayer.from.y = locDBF.LeftSpriteY;
            //tpPlayer.to.y = locDBF.LeftSpriteY;
			PlayerContainer.localPosition = tpPlayer.from;
		}
		
		//換圖
		Utility.ChangeTexture(TexturePlayer, textureIndex);
        //角色圖不做MakePixelPerfect
		//TexturePlayer.MakePixelPerfect();體

		// 查表找基本值
		S_PlayerData_Tmp oldDbf = GameDataDB.PlayerDB.GetData(oldLV);
		S_PlayerData_Tmp newDbf = GameDataDB.PlayerDB.GetData(newLV);
		if(oldDbf == null || newDbf ==null)
		{
			return;
		}

		// LV
		LabelLVValue.text 		= string.Format("{0}",oldLV);
		LabelLVValueNew.text 	= string.Format("{0}",newLV);

		//ATK
		LabelAttValue.text		= string.Format("+ {0}",newDbf.sAttrTable.iAttack - oldDbf.sAttrTable.iAttack);

		//Def
		LabelDefValue.text		= string.Format("+ {0}",newDbf.sAttrTable.iDefense - oldDbf.sAttrTable.iDefense);

		//HP
		LabelHPValue.text		= string.Format("+ {0}",newDbf.sAttrTable.iMaxHP - oldDbf.sAttrTable.iMaxHP);

		//Name
		NameLabel.text			= string.Format("{0}",ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName);

		//AP
		LabelAPValue.text		= oldDbf.iMaxAP.ToString();
		LabelAPValueNew.text	= newDbf.iMaxAP.ToString();
		SpriteArrow.gameObject.SetActive(true);
		LabelAPTitle.gameObject.SetActive(true);
	}

	//-------------------------------------------------------------------------------------------------
	public void SetLevelUpInfoPet(S_PetData oldData, S_PetData newData)
	{
		UnityDebugger.Debugger.Log(string.Format("Pet LevelUp Info {0} {1}", oldData.iPetLevel, newData.iPetLevel));

		bool debugFlag;

		// 設定自己的大圖
		//texture編號
		int textureIndex = oldData.GetPetFullAvatar();
		//換圖
		debugFlag = Utility.ChangeTexture(TexturePlayer, textureIndex); 
		if(!debugFlag)
		{
			UnityDebugger.Debugger.LogError(string.Format("十三漏填DBF! 十三甲賽! S_TextureManager_Tmp null textureIndex {0}", textureIndex));

		}

		//設定texture位置
		//S_SpriteCoordinate_Tmp locDBF = GameDataDB.SpriteCoordinate.GetData(textureIndex);
		//if(locDBF == null)
		//{
		//	UnityDebugger.Debugger.LogError(string.Format("十三漏填DBF! 十三甲賽! SpriteCoordinate null textureIndex {0}", textureIndex));
		//}
		//else
		{
//			PlayerContainer.transform.localPosition = new Vector3(locDBF.LeftSpriteX,locDBF.LeftSpriteY);
//			PlayerContainer.localPosition = new Vector3(from_X, locDBF.LeftSpriteY);
            //tpPlayer.from.y = locDBF.LeftSpriteY;
            //tpPlayer.to.y = locDBF.LeftSpriteY;
			PlayerContainer.localPosition = tpPlayer.from;
		}
        //角色圖不做MakePixelPerfect
		//TexturePlayer.MakePixelPerfect();

		// 寵物不用查表找基本值
		// LV
		LabelLVValue.text 		= string.Format("{0}",oldData.iPetLevel);
		LabelLVValueNew.text 	= string.Format("{0}",newData.iPetLevel);
		//
		S_RoleAttr OldPetAttrValue = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetAttrValue(oldData);
		S_RoleAttr NewPetAttrValue = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetAttrValue(newData);

		int iDiffValue;
		//ATK
		iDiffValue = (int)(Math.Round(NewPetAttrValue.sBattleFinial.fAttack - OldPetAttrValue.sBattleFinial.fAttack,0,MidpointRounding.AwayFromZero));
		LabelAttValue.text		= string.Format("+ {0}",iDiffValue);
		
		//Def
		iDiffValue = (int)(Math.Round(NewPetAttrValue.sBattleFinial.fDefense - OldPetAttrValue.sBattleFinial.fDefense,0,MidpointRounding.AwayFromZero));
		LabelDefValue.text		= string.Format("+ {0}",iDiffValue);
		
		//HP
		iDiffValue = (int)(Math.Round(NewPetAttrValue.sBattleFinial.fMaxHP - OldPetAttrValue.sBattleFinial.fMaxHP,0,MidpointRounding.AwayFromZero));
		LabelHPValue.text		= string.Format("+ {0}",iDiffValue);

		//Name
		int nameID = oldData.GetPetName();
		if(nameID == -1)
		{
			UnityDebugger.Debugger.LogError(string.Format("十三漏填DBF! 十三甲賽! ldData.GetPetName() {0}", oldData.iPetDBFID));
		}

		string name = null;
		name = GameDataDB.GetString(nameID);
		if(name == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("十三漏填字串表! 十三甲賽! nameID {0}", nameID));

		}
		NameLabel.text			= string.Format("{0}", name);

		//寵物不用顯示AP
		LabelAPValue.text		= "";
		LabelAPValueNew.text	= "";
		SpriteArrow.gameObject.SetActive(false);
		LabelAPTitle.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------\
	//突破
	public void SetPetLimitLevelUp(S_PetData oldData, S_PetData newData)
	{
		UnityDebugger.Debugger.Log(string.Format("Pet LevelUp Info {0} {1}", oldData.iPetLevel, newData.iPetLevel));
		
		bool debugFlag;
		
		// 設定自己的大圖
		//texture編號
		int textureIndex = oldData.GetPetFullAvatar();
		//換圖
		debugFlag = Utility.ChangeTexture(TexturePlayer, textureIndex); 
		if(!debugFlag)
		{
			UnityDebugger.Debugger.LogError(string.Format("十三漏填DBF! 十三甲賽! S_TextureManager_Tmp null textureIndex {0}", textureIndex));
			
		}
		
		//設定texture位置
		//S_SpriteCoordinate_Tmp locDBF = GameDataDB.SpriteCoordinate.GetData(textureIndex);
		//if(locDBF == null)
		//{
		//	UnityDebugger.Debugger.LogError(string.Format("十三漏填DBF! 十三甲賽! SpriteCoordinate null textureIndex {0}", textureIndex));
		//}
		//else
		{
            //tpPlayer.from.y = locDBF.LeftSpriteY;
            //tpPlayer.to.y = locDBF.LeftSpriteY;
			PlayerContainer.localPosition = tpPlayer.from;
		}
        //角色圖不做MakePixelPerfect
		//TexturePlayer.MakePixelPerfect();
		
		// 寵物不用查表找基本值
		// LV
		LabelLVValue2.text 		= string.Format("{0}",oldData.iPetLimitLevel);
		LabelLVValueNew2.text 	= string.Format("{0}",newData.iPetLimitLevel);
		
		S_RoleAttr OldPetAttrValue = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetAttrValue(oldData);
		S_RoleAttr NewPetAttrValue = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetAttrValue(newData);

		int iDiffValue;
		
		//ATK
		iDiffValue = (int)(Math.Round(NewPetAttrValue.sBattleFinial.fAttack - OldPetAttrValue.sBattleFinial.fAttack,0,MidpointRounding.AwayFromZero));
		LabelAttValue2.text		= string.Format("+ {0}",iDiffValue);
		
		//Def
		iDiffValue = (int)(Math.Round(NewPetAttrValue.sBattleFinial.fDefense - OldPetAttrValue.sBattleFinial.fDefense,0,MidpointRounding.AwayFromZero));
		LabelDefValue2.text		= string.Format("+ {0}",iDiffValue);
		
		//HP
		iDiffValue = (int)(Math.Round(NewPetAttrValue.sBattleFinial.fMaxHP - OldPetAttrValue.sBattleFinial.fMaxHP,0,MidpointRounding.AwayFromZero));
		LabelHPValue2.text		= string.Format("+ {0}",iDiffValue);

		if(newData.GetPetUnlockLimitLVSkillName() == -1)
			bShowSkillData = false;
		else
		{
			LabelSkillName.text		= GameDataDB.GetString(newData.GetPetUnlockLimitLVSkillName());
			LabelSkillNote.text		= ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(newData.GetPetUnlockLimitLVSkillNote()),newData.GetTalentSkill(),SkillLevelType.Now);
			//LabelSkillNote.text		= GameDataDB.GetString(newData.GetPetUnlockLimitLVSkillNote());
			bShowSkillData = true;
		}
		
		//Name
		int nameID = oldData.GetPetName();
		if(nameID == -1)
		{
			UnityDebugger.Debugger.LogError(string.Format("十三漏填DBF! 十三甲賽! ldData.GetPetName() {0}", oldData.iPetDBFID));
		}
		
		string name = null;
		name = GameDataDB.GetString(nameID);
		if(name == null)
		{
			UnityDebugger.Debugger.LogError(string.Format("十三漏填字串表! 十三甲賽! nameID {0}", nameID));
			
		}
		NameLabel2.text			= string.Format("{0}", name);
		
	}
	
}
