using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_GuildBossBattleResult : NGUIChildGUI  
{
	public UILabel			LabelGuildBossBattle	= null;

	public UIButton			ButtonGuildBossBattleClose	= null;
	public UILabel			LabelGuildBossBattleClose	= null;
	
	public UILabel			LabelPlayerTitle		= null;
	public UILabel			LabelTargetTitle		= null;

	public UILabel			LabelPlayerName			= null;
	public UILabel			LabelTargetName			= null;

	public UILabel			LabelTotalPowerTitle	= null;
	public UILabel			LabelTotalPowerValue	= null;

	public UILabel			LabelSurplusHPTitle		= null;
	public UILabel			LabelSurplusHPValue		= null;

	public UILabel			LabelRoundTitle			= null;
	public UILabel			LabelRoundValue			= null;

	public UILabel			LabelPlayerAtkTitle		= null;
	public UILabel			LabelPlayerAtkValue		= null;

	public UILabel			LabelPlayerPointTitle	= null;
	public UILabel			LabelPlayerPointValue	= null;

	public UIGrid			GridGetPoint			= null;

	public Transform		ANIMDamagePoint			= null;
	public UILabel			LabelDamagePointTitle	= null;	//傷害積分
	public UILabel			LabelDamagePointValue	= null;

	public Transform		ANIMNemesisPoint		= null;
	public UILabel			LabelNemesisPointTitle	= null;	//剋星積分
	public UILabel			LabelNemesisPointValue	= null;

	public Transform		ANIMRevengePoint		= null;
	public UILabel			LabelRevengePointTitle	= null;	//復仇積分
	public UILabel			LabelRevengePointValue	= null;

	public Transform		ANIMBonusPoint			= null;
	public UILabel			LabelBonusPointTitle	= null;	//出戰積分
	public UILabel			LabelBonusPointValue	= null;

	public Transform		ANIMRoundPoint			= null;
	public UILabel			LabelRoundPointTitle	= null;	//回合積分
	public UILabel			LabelRoundPointValue	= null;

	public Transform		ANIMKillPoint			= null;
	public UILabel			LabelKillPointTitle		= null;	//擊殺積分
	public UILabel			LabelKillPointValue		= null;

	public UISprite			SpriteFinish			= null;

	public List<UISprite>	PetSpriteList			= new List<UISprite>();
	public UITexture		TextureBoss				= null;
	public UITexture		TextureBoss2			= null;

	public UITexture		TexturePlayer			= null;

	public string			BattleResultMusic		= "";

	public bool				showAllPoint			= true;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_GuildBossBattleResult";
	
	//-------------------------------------------------------------------------------------------------
	private UI_GuildBossBattleResult() : base(GUI_SMARTOBJECT_NAME)
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
		LabelGuildBossBattle.text	= GameDataDB.GetString(8700);	//攻防戰報

		LabelGuildBossBattleClose.text	= GameDataDB.GetString(8701);	//結束
     
		LabelPlayerTitle.text	= GameDataDB.GetString(8702);	//攻方
		LabelTargetTitle.text	= GameDataDB.GetString(8703);	//守方
    
		LabelPlayerName.text	= "";
		LabelTargetName.text	= "";

		LabelTotalPowerTitle.text	= GameDataDB.GetString(8704);	//隊伍總戰力
		LabelTotalPowerValue.text	= "";

		LabelSurplusHPTitle.text	= GameDataDB.GetString(8705);	//剩餘血量
		LabelSurplusHPValue.text	= "";
    
		LabelRoundTitle.text		= GameDataDB.GetString(8706);	//進行回合數
		LabelRoundValue.text		= "";

		LabelPlayerAtkTitle.text	= GameDataDB.GetString(8707);	//攻方造成傷害
		LabelPlayerAtkValue.text	= "";

		LabelPlayerPointTitle.text	= GameDataDB.GetString(8708);	//攻方獲得積分
		LabelPlayerPointValue.text	= "";

		LabelDamagePointTitle.text	= GameDataDB.GetString(8709);	//傷害積分
		LabelDamagePointValue.text	= "";

		LabelNemesisPointTitle.text	= GameDataDB.GetString(8710);	//相剋加分
		LabelNemesisPointValue.text	= "";

		LabelRevengePointTitle.text	= GameDataDB.GetString(8711);	//仇恨加分
		LabelRevengePointValue.text	= "";

		LabelBonusPointTitle.text	= GameDataDB.GetString(8712);	//出戰加分
		LabelBonusPointValue.text	= "";

		LabelRoundPointTitle.text	= GameDataDB.GetString(8713);	//回合積分
		LabelRoundPointValue.text	= "";

		LabelKillPointTitle.text	= GameDataDB.GetString(8714);	//擊殺積分
		LabelKillPointValue.text	= "";

		SpriteFinish.gameObject.SetActive(false);

		for(int i=0; i<PetSpriteList.Count; ++i)
		{
			Utility.ChangeAtlasSprite(PetSpriteList[i], -1);
		}

		Utility.ChangeTexture(TextureBoss, -1);
		Utility.ChangeTexture(TexturePlayer, -1);

	}

	//-------------------------------------------------------------------------------------------------
//	public void SetUI(JSONPG_MtoC_AttackGuildKingResult data)
	public void SetUI()
	{
		GuildBattleResult data = ARPGApplication.instance.m_GuildSystem.GetGuildBattleResult();

		//玩家名稱和圖像
		LabelPlayerName.text		= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName;
		int textureID = ARPGApplication.instance.m_RoleSystem.GetRoleTexture();
		Utility.ChangeTexture(TexturePlayer, textureID);

		//隊伍戰力
		LabelTotalPowerValue.text	= data.iTotlePower.ToString();

		//BOSS剩餘血量
		if(data.iGuildKingHP <0 )
		{
			LabelSurplusHPValue.text	= "0";
		}
		else
		{
			LabelSurplusHPValue.text	= data.iGuildKingHP.ToString();
		}

		//中間戰鬥資訊
		LabelRoundValue.text		= data.iRound.ToString();

		LabelPlayerAtkValue.text	= data.iTotleDamage.ToString();
		LabelPlayerPointValue.text	= data.iGetScore.ToString();
		LabelDamagePointValue.text	= data.iDamageScore.ToString();
		LabelNemesisPointValue.text	= data.iPropertyScore.ToString();
		LabelRevengePointValue.text	= data.iEnemyScore.ToString();

		LabelBonusPointValue.text	= data.iAttackScore.ToString();
		LabelRoundPointValue.text	= data.iRoundScore.ToString();
		LabelKillPointValue.text	= data.iKillScore.ToString();

		if(!showAllPoint)
		{
/*			if(data.iTotleDamage <= 0)
			{
				LabelPlayerAtkValue.gameObject.SetActive(false);
			}
			if(data.iGetScore <= 0)
			{
				LabelPlayerPointValue.gameObject.SetActive(false);
			}
*/
			if(data.iDamageScore <= 0)
			{
				ANIMDamagePoint.gameObject.SetActive(false);
			}
			if(data.iPropertyScore <= 0)
			{
				ANIMNemesisPoint.gameObject.SetActive(false);
			}
			if(data.iEnemyScore <= 0)
			{
				ANIMRevengePoint.gameObject.SetActive(false);
			}
			if(data.iAttackScore <= 0)
			{
				ANIMBonusPoint.gameObject.SetActive(false);
			}
			if(data.iRoundScore <= 0)
			{
				ANIMRoundPoint.gameObject.SetActive(false);
			}
			if(data.iKillScore <= 0)
			{
				ANIMKillPoint.gameObject.SetActive(false);
			}
		}

		GridGetPoint.Reposition();
		//戰勝圖片
		SpriteFinish.gameObject.SetActive(data.bIsWin);

		//BOSS圖和名子
		S_GuildBossValue_Tmp bossTmp = GameDataDB.GuildBossValueDB.GetData(data.iGuildBossID);
		if(bossTmp == null)
		{
			LabelTargetName.text	= "";
			Utility.ChangeTexture(TextureBoss, -1);
			Utility.ChangeTexture(TextureBoss2, -1);
		}
		else
		{
			LabelTargetName.text	= GameDataDB.GetString(bossTmp.iGuildBossName);

			Utility.ChangeTexture(TextureBoss, bossTmp.iGuildBossFullAvatar);

			if(data.bIsWin)
			{
				Utility.ChangeTexture(TextureBoss2, bossTmp.iGuildBossFullAvatar);

			}
			else
			{
				Utility.ChangeTexture(TextureBoss2, -1);
			}
		}

		//寵物圖像
		int[] petID = ARPGApplication.instance.m_GuildSystem.GetBossBattlePet();
		S_PetData_Tmp petDBF = null;

		for(int i=0; i<PetSpriteList.Count; ++i)
		{
			if(petID.Length > i)
			{
				petDBF = GameDataDB.PetDB.GetData(petID[i]);
				if(petDBF != null)
				{
					Utility.ChangeAtlasSprite(PetSpriteList[i], petDBF.AvatarIcon);
				}
				else
				{
					Utility.ChangeAtlasSprite(PetSpriteList[i], -1);
				}
			}
			else
			{
				Utility.ChangeAtlasSprite(PetSpriteList[i], -1);
			}
		}

	}

	//-------------------------------------------------------------------------------------------------
	public void PlayBattleResultMusic()
	{
		MusicControlSystem.PlaySound(BattleResultMusic, 1);
	}
}
