using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum ENUM_BossBattlePetTitle
{
	Vangaurd = 0,
	Middle,
	MiddleGeneral,
	SecondGeneral,
	FirstGeneral,
}

public class HistoryProperty
{
	public int ID 			= 0;
	public DateTime Time 	= DateTime.MinValue;
	public string Content 	= null;
}

public class UI_GuildBoss : NGUIChildGUI 
{
	public UIPanel 		panelBase 				= null;
	public UILabel 		lbGuildBoss 			= null;
	public UIButton 	btnClose 				= null;
	public GameObject	gFullScreenCollider		= null;

	[Header("TopUI")]
	public UILabel 		lbScoreTitle 			= null;
	public UILabel 		lbPersonalScoreTitle	= null;
	public UILabel 		lbPersonalScore			= null;
	public UIProgressBar barScore				= null;

	public UIGrid 		gdRewardItem 			= null;
	public Slot_Item[] 	slotRewardItem 			= null;
	public UIButton 	btnGuildRanking 		= null;
	public UILabel 		lbGuildRanking 			= null;
	public UIButton 	btnScoreReward 			= null;
	public UILabel 		lbScoreReward  			= null;
	public GameObject 	gScoreReachEffect		= null;
	public UISprite 	spTotalReach 			= null;
	public Animation	animTotalReach			= null;
	public UIButton 	btnChat					= null;
	public UILabel 		lbChat	  				= null;
	public UILabel 		lbGuildBossTimeTitle 	= null;
	public UILabel 		lbGuildBossTime 		= null;
	public UILabel 		lbGuildGainScore		= null;
	public UILabel 		lbGuildScoreTitle		= null;
	public UILabel 		lbGuildTotalScore		= null;
	public GameObject 	gTopHistory				= null;
	public UILabel 		lbTopHistory1			= null;
	public TweenAlpha 	taTopHistory1			= null;

	[Header("BoseBase")]
	public UILabel 		lbReadyTitle 			= null;
	public UIProgressBar barReadyTime			= null;
	public UILabel 		lbReadyTime 			= null;
	public UILabel 		lbBattleCount			= null;
	public UIButton 	btnBuyBattleCount		= null;
	public UILabel 		lbBuyBattleCount		= null;
	public UILabel 		lbBuyBattleCountCost	= null;

	[Header("BottomUI")]
	public UILabel 		lbHistoryTitle 			= null;
	public UILabel 		lbHistory 				= null;
	public UIButton 	btnOneInspire			= null;
	public UILabel 		lbOneInspire 			= null;
	public UILabel 		lbOneInspireCost 		= null;
	public UIButton 	btnTenInspire 			= null;
	public UILabel 		lbTenInspire			= null;
	public UILabel 		lbTenInspireCost		= null;
	public UIButton 	btnAutoSet 				= null;
	public UILabel 		lbAutoSet				= null;
	public UIButton 	btnInspireNote			= null;
	public UILabel 		lbInspireAtkValue		= null;
	public UILabel 		lbInspireDefValue		= null;
	public UILabel 		lbPlayerGuildMoney		= null;
	public UILabel 		lbPlayerDiamond			= null;
	public UILabel 		lbPetTotalPower			= null;
	public AddNumber  	jumpNumber				= null;		// 戰力更新跳數值表演用，掛在lbPetTotalPower下
	
	[Header("InspireNote")]
	public UIPanel		panelInspireNoteUI		= null;
	public UIButton 	btnCloseInspireNote		= null;
	public UILabel 		lbNormalInspire			= null;
	public UILabel 		lbNormalInspireAtk		= null;
	public UILabel 		lbFormationInspire		= null;
	public UILabel 		lbFormationInspireAtk	= null;
	public UILabel 		lbPavilionInspire		= null;
	public UILabel 		lbPavilionInspireAtk	= null;

	[Header("ReadyPetList")]
	public UIPanel 			panelReadyPetList	= null;
	public UIScrollView 	svReadyPetList		= null;
	public UIWrapContentEX 	wcReadyPetList		= null;
	public UIButton 		btnCloseReadyPet	= null;

	[Header("Explaination")]
	public UIPanel 			panelExplaination		= null;
	public UILabel 			lbExplaination 			= null;
	public UIButton 		btnCloseExplaination	= null;
	public UIButton 		btnExplaination			= null;

	//-----------------------------------暫存用--------------------------------------------------------------
	private int 			m_NormalInspireAtk		= 0;
	private int 			m_FormationInspireAtk	= 0;
	private int 			m_PavilionInspireAtk	= 0;
	private int 			m_PetTotalPower			= 0;
	public int 				PetTotalPower			{get{return m_PetTotalPower;}}
	[NonSerialized]  public S_Activity m_GuildActivity	= null;
	//------------------------------------各種費用---------------------------------------------------------
	//鼓舞費用
	[HideInInspector]public int								m_OneInspireCost	= 0;
	[HideInInspector]public int								m_TenInpireCost		= 0;
	//取消攻打CD費用										  	
	[HideInInspector]public int								m_ClearReadyCost	= 0;
	//購買攻打次數費用										  	
	[HideInInspector]public int								m_BuyBattleCountCost= 0;
	//-------------------------------------------------------------------------------------------------
	public List<Slot_PetData> 								m_SlotPetList		= new List<Slot_PetData>();
	[HideInInspector]	public List<S_PetData> 				m_PlayerPetDataList	= new List<S_PetData>();
	//
	public List<Transform>									m_BossBasePosition	= new List<Transform>();
	[HideInInspector]	public List<Slot_GuildBossBase>		m_BossBaseList		= new List<Slot_GuildBossBase>();
	//
	public List<Transform>									m_BossPetPosition	= new List<Transform>();
	[HideInInspector]	public List<Slot_GuildBossBattlePet>m_BossBattlePetList	= new List<Slot_GuildBossBattlePet>();
	//
	[HideInInspector] 	public List<HistoryProperty>		m_PlayerHistory		= new List<HistoryProperty>();	
	[HideInInspector] 	public List<HistoryProperty>		m_SystemHistory		= new List<HistoryProperty>();	
	[HideInInspector] 	public List<HistoryProperty>		m_HistoryContent	= new List<HistoryProperty>();	
	//
	[HideInInspector]	public CdTimer 						m_BattleReadyTimer	= null;
	[HideInInspector]	public CdTimer 						m_GuildBossTimer	= null;
	//-------------------------------------------------------------------------------------------------
	private const string 	m_SlotBossBaseName		= "Slot_GuildBossBase";
	private const string 	m_SlotBossPetName		= "Slot_GuildBossBattlePet";
	private const int 		m_SlotItemAdjustDepth	= 5;
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_GuildBoss";
	
	//-------------------------------------------------------------------------------------------------
	private UI_GuildBoss() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		CreatBossBaseSlot();
		CreatAndSetPetSlot();
		jumpNumber.InitialUI();
		InitialLabel();		
		InitialData();
	}
	//-------------------------------------------------------------------------------------------------
	private void InitialData()
	{
		//
		wcReadyPetList.onInitializeItem += AssignPetData;
		//
		gdRewardItem.gameObject.SetActive(false);
		lbGuildGainScore.gameObject.SetActive(false);
		lbGuildScoreTitle.gameObject.SetActive(false);
		lbReadyTitle.alpha = 0.0f;
		//
		for(int m=0; m<slotRewardItem.Length; ++m)
		{
			slotRewardItem[m].SetDepth(m_SlotItemAdjustDepth);
		}
		//計算戰陣鼓舞加成
		S_GuildWars_Tmp guildWarTmp = GameDataDB.GuildWarsDB.GetData(GameDefine.GUILDWAR_DBF_GUID);
		if (guildWarTmp != null)
		{
			Dictionary<ENUM_FormationType,S_FormationData> tempFormationData = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.FormationData;
			foreach(S_FormationData data in tempFormationData.Values)
			{
				m_FormationInspireAtk += data.iLV;
			}
			m_FormationInspireAtk	= Mathf.FloorToInt(m_FormationInspireAtk * guildWarTmp.fPerFormationLvFactor);
			lbFormationInspireAtk.text = string.Format(GameDataDB.GetString(8659),m_FormationInspireAtk.ToString());
		}
		//計算經閣鼓舞加成
		S_GuildLevel_Tmp buildTmp = ARPGApplication.instance.m_GuildSystem.GetGuildBuildingTmp(ENUM_GUILD_BUILD.EMUM_GUILD_BUILDE_Pavilion);
		if (buildTmp != null)
		{
			m_PavilionInspireAtk = Mathf.FloorToInt(buildTmp.PavilionEffect*100);
			lbPavilionInspireAtk.text = string.Format(GameDataDB.GetString(8659),m_PavilionInspireAtk.ToString());
		}

		//註冊公會王活動倒數計時器
		S_Activity_Tmp actTmp = GameDataDB.ActivityDB.GetData(m_GuildActivity.iActivityDBID);
		CountDownSystem cdSystem = ARPGApplication.instance.m_CountDownTimerSystem;
		if (actTmp != null)
		{
			//將GMT+8轉換成UTC
			int utcEndHour = actTmp.EndHour-8;
			if (utcEndHour < 0)
				utcEndHour = 24 + utcEndHour;
			DateTime endDate = new DateTime(DateTime.UtcNow.Year, 
			                                DateTime.UtcNow.Month, 
			                                DateTime.UtcNow.Day, 
			                                utcEndHour,0,0);
			//計算公會戰活動剩餘時間
			float actRemainTime = (float)DateTime.UtcNow.Subtract(endDate).TotalSeconds;
			if (actRemainTime > 0.0f)
				actRemainTime = 24*3600.0f - actRemainTime;
			else
				actRemainTime = Math.Abs(actRemainTime);
			m_GuildBossTimer = ARPGApplication.instance.m_CountDownTimerSystem.StartCountDown(actRemainTime,
			                                                                                  GuildBossTimesUp, 
			                                                                                  Enum_CdTimerType.Enum_CdTimerType_GuildBossActivityRemain);                                                                                
		}                                                                               
	}
	//-------------------------------------------------------------------------------------------------
	private void InitialLabel()
	{
		lbGuildBoss.text = GameDataDB.GetString(8615);			//公會王
		lbScoreTitle.text = GameDataDB.GetString(1580);			//積分
		lbPersonalScoreTitle.text = GameDataDB.GetString(8616);	//個人積分
		lbGuildRanking.text = GameDataDB.GetString(8619);		//公會排名列表
		lbScoreReward.text = GameDataDB.GetString(8632);		//達標清單
		lbChat.text = GameDataDB.GetString(8103);				//聊天
		lbHistoryTitle.text = GameDataDB.GetString(9744);		//歷程
		lbGuildBossTimeTitle.text = GameDataDB.GetString(8657);	//"活動倒計時"
		lbGuildScoreTitle.text = GameDataDB.GetString(8663);	//"公會總積分"
		//
		lbReadyTitle.text = GameDataDB.GetString(8604);			//準備:
		//
		lbOneInspire.text 		= GameDataDB.GetString(8654);	//一次鼓舞
		lbTenInspire.text 		= GameDataDB.GetString(8601);	//十次鼓舞
		lbAutoSet.text 			= GameDataDB.GetString(8605);	//一鍵上陣
		lbBuyBattleCount.text	= GameDataDB.GetString(8620);	//購買
		lbHistory.text = "";
		//
		lbNormalInspire.text	= GameDataDB.GetString(8602);	//鼓舞
		lbFormationInspire.text	= GameDataDB.GetString(15070);	//戰陣
		lbPavilionInspire.text	= GameDataDB.GetString(8109);	//經閣
		//
		lbExplaination.text = GameDataDB.GetString(8649);		//公會說明文

		lbTopHistory1.text = null;
	}
	//-------------------------------------------------------------------------------------------------
	private void CreatBossBaseSlot()
	{
		Slot_GuildBossBase go = ResourceManager.Instance.GetGUI(m_SlotBossBaseName).GetComponent<Slot_GuildBossBase>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_GuildBoss load prefeb error,path:{0}", "GUI/"+m_SlotBossBaseName) );
			return;
		}

		for(int i=0; i<m_BossBasePosition.Count; ++i) 
		{
			Slot_GuildBossBase newgo	= Instantiate(go) as Slot_GuildBossBase;
			
			newgo.transform.parent			= m_BossBasePosition[i];
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition	= Vector3.zero;
			
			newgo.name = string.Format("Slot_GuildBossBase{0:00}",i);
			newgo.SlotIndex = i;
			newgo.BaseID = GameDefine.GUILDWAR_BASE_1_ID+i;
			newgo.InitialUI();
			newgo.m_uiGuildBoss = this;

			m_BossBaseList.Add(newgo);
			newgo.gameObject.SetActive(true);
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void CreatAndSetPetSlot()
	{
		Slot_GuildBossBattlePet go = ResourceManager.Instance.GetGUI(m_SlotBossPetName).GetComponent<Slot_GuildBossBattlePet>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("UI_GuildBoss load prefeb error,path:{0}", "GUI/"+m_SlotBossPetName) );
			return;
		}

		//初始化出戰寵物資料
		ARPGApplication.instance.m_GuildSystem.InitBossBattlePet();
		int[] battlePetID = ARPGApplication.instance.m_GuildSystem.GetBossBattlePet();
		if (battlePetID == null)
			return;

		if (battlePetID.Length == m_BossPetPosition.Count)
		{
			for(int i=0; i<m_BossPetPosition.Count; ++i) 
			{
				Slot_GuildBossBattlePet newgo	= Instantiate(go) as Slot_GuildBossBattlePet;
				
				newgo.transform.parent			= m_BossPetPosition[i];
				newgo.transform.localScale		= Vector3.one;
				newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);//Quaternion.AngleAxis(0, Vector3.zero);
				newgo.transform.localPosition	= Vector3.zero;
				
				newgo.name = string.Format("Slot_GuildBossBattlePet{0:00}",i);
				newgo.SlotIndex = i;
				newgo.lbPartyNumber.text = (i+1).ToString();
				newgo.InitialUI();
				//設定玩家之前已設置過的寵物
				newgo.SetSlot(battlePetID[i]);

				m_BossBattlePetList.Add(newgo);
				newgo.gameObject.SetActive(true);
			}
		}
	}
	#region 設定UI相關
	//-------------------------------------------------------------------------------------------------
	public void SetGuildBossUI()
	{
		SetBattleCount();
		SetPetInspire();
//		SetPlayerMoney();
		SyncBattleCdTime();
		SetPetTotalPower();
//		for(int i=0;i<m_BossBaseList.Count; ++i)
//		{
//			m_BossBaseList[i].UpdateBossInspireCost();
//		}
	}
	//-------------------------------------------------------------------------------------------------
	//積分值跑條時更新顯示的UI
	public void UpdateScoreBar(int currentScore, int scoreGoal, bool isReach)
	{
		if (isReach)
			lbPersonalScore.text = currentScore.ToString();
		else
			lbPersonalScore.text = currentScore.ToString()+" / "+scoreGoal.ToString();
		float expRatio = (float)currentScore / scoreGoal;
		if (expRatio > 1)
			expRatio = 1;
		barScore.value = expRatio;
	}
	//-------------------------------------------------------------------------------------------------
//	public void SetPlayerMoney()
//	{
//		RoleSystem roleSys= ARPGApplication.instance.m_RoleSystem;
//		lbPlayerGuildMoney.text = roleSys.iMemberMoney.ToString();
//		lbPlayerDiamond.text 	= roleSys.m_PlayerRoleData.GetItemMallMoney().ToString();
//	}
	//-------------------------------------------------------------------------------------------------
	public void SetBattleHistory()
	{
		if (m_HistoryContent.Count <= 0)
			return;

		lbHistory.text = "";

		for(int i=0; i<m_HistoryContent.Count; ++i)
		{
			if (m_HistoryContent[i] == null)
				continue;

			if (i == m_HistoryContent.Count-1)
				lbHistory.text += m_HistoryContent[i].Content;
			else
				lbHistory.text += m_HistoryContent[i].Content+"\n";
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetBossBase(JSONPG_MtoC_GuildKingData bossBaseData)
	{
		if (bossBaseData == null)
			return;
		
		for(int i=0; i<m_BossBaseList.Count; ++i)
		{
			Slot_GuildBossBase slotBoss = m_BossBaseList[i];
			if (slotBoss.BaseID == bossBaseData.iGuildKingID)
			{
				slotBoss.SetBossBase(bossBaseData);
				break;
			}
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SetBattleCount()
	{
		S_Activity actData = ARPGApplication.instance.m_ActivityMgrSystem.GetActivityByActType(EMUM_ACTIVITY_TYPE.EMUM_ACTIVITY_TYPE_GuildWar);
		if (actData == null)
			return;
		S_ActivityInfo_Tmp actInfoTmp = GameDataDB.ActivityInfoDB.GetData(actData.iActivityInfoDBID);
		if (actInfoTmp == null)
			return;

		C_RoleDataEx roleDataEx = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData;
		int battleCount = roleDataEx.GetAP((ENUM_APType)actInfoTmp.iVariablePos);

		S_ShopPrize_Tmp shopTmp = GameDataDB.ShopPrizeDB.GetData(GameDefine.ITEMMALL_BUY_GUILDBOSS_BATTLECOUNT_ID);
		if (shopTmp == null)
			return;
		m_BuyBattleCountCost = shopTmp.GetPrize(roleDataEx.sRoleItemMallData.iBuyGuildWarBattleCount);

		if (battleCount > 0)
		{
			lbBattleCount.text = battleCount.ToString();
			lbBattleCount.gameObject.SetActive(true);
			btnBuyBattleCount.gameObject.SetActive(false);
		}
		else
		{
			lbBattleCount.gameObject.SetActive(false);
			btnBuyBattleCount.gameObject.SetActive(true);
			lbBuyBattleCountCost.text = m_BuyBattleCountCost.ToString();
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SyncBattleCdTime()
	{
		S_GuildWars_Tmp guildWarTmp = GameDataDB.GuildWarsDB.GetData(GameDefine.GUILDWAR_DBF_GUID);
		if (guildWarTmp == null)
			return;
		CountDownSystem countSystem = ARPGApplication.instance.m_CountDownTimerSystem;
		float subSecs = (float)countSystem.GetAdjustClientTime().Subtract(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLastGuildWarATKTime()).TotalSeconds;
		float reaminSecs = guildWarTmp.iReadyTime - subSecs;

//		if (reaminSecs <= 0.0f)
//		{
//			BattleCdTimesUp(Enum_CdTimerType.Enum_CdTimerType_GuildBossBattleCD);
//		}
		if (reaminSecs > 0.0f)
		{
			if (reaminSecs > guildWarTmp.iReadyTime)
				reaminSecs = guildWarTmp.iReadyTime;


			//重新註冊計時器
			if (m_BattleReadyTimer == null)
			{
				m_BattleReadyTimer = countSystem.StartCountDown(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLastGuildWarATKTime(),
				                           						guildWarTmp.iReadyTime,
				                           						BattleCdTimesUp,
				                           						Enum_CdTimerType.Enum_CdTimerType_GuildBossBattleCD);
			}
			else
				m_BattleReadyTimer.SyncCdTime(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLastGuildWarATKTime());
			SwitchReadyBar(true);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void UpdateBattleCdTime()
	{
		if (m_BattleReadyTimer == null)
			return;

		//無條件捨去至小數點第一位
		float nowRate = Mathf.Floor((m_BattleReadyTimer.countDownTime/m_BattleReadyTimer.countDownLimit)*10)/10;
		if (nowRate > 1.0f)
			nowRate = 1.0f;
		
		//barReadyTime.alpha = (nowRate > 0.0f ? 1:0);
		barReadyTime.value = nowRate;
		
		lbReadyTime.text = m_BattleReadyTimer.ShowTime(ENUM_CDTime_ShowTime.Minute);
	}
	//-------------------------------------------------------------------------------------------------
	public void UpdateGuildBossTime()
	{
		if (m_GuildBossTimer == null)
			return;
		
		lbGuildBossTime.text = m_GuildBossTimer.ShowTime(ENUM_CDTime_ShowTime.Hour);
	}
	//-------------------------------------------------------------------------------------------------
//	public void UpdateBossInspireCost(int baseID, ENUM_InspireType inspireType)
//	{
//		for(int i=0;i<m_BossBaseList.Count; ++i)
//		{
//			if (m_BossBaseList[i].BaseID == baseID)
//			{
//				m_BossBaseList[i].UpdateBossInspireCost();
//				m_BossBaseList[i].PlayInspireEffect(inspireType);
//				break;
//			}
//		}
//	}
	//-------------------------------------------------------------------------------------------------
	public void SetPetInspire()
	{
		GuildBaseData guildData = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		if (guildData == null)
			return;
		S_GuildWars_Tmp guildWarTmp = GameDataDB.GuildWarsDB.GetData(GameDefine.GUILDWAR_DBF_GUID);
		if (guildWarTmp == null)
			return;
		m_NormalInspireAtk = Mathf.RoundToInt(guildData.iATKBuffLv*guildWarTmp.fInspirePerValue*100);
		lbNormalInspireAtk.text = string.Format(GameDataDB.GetString(8659),m_NormalInspireAtk);
		lbInspireAtkValue.text = string.Format(GameDataDB.GetString(8659),
		                                       m_NormalInspireAtk+m_FormationInspireAtk+m_PavilionInspireAtk/*,
		                                       GameDefine.GUILDWAR_BUFF_ATK_POWER_RATIO*100*/);	//"{0}%"

		int playerDiamond = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetItemMallMoney();
		S_RoleItemMallData roleItemMall =  ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.sRoleItemMallData;
		S_ShopPrize_Tmp shopTmp = GameDataDB.ShopPrizeDB.GetData(GameDefine.ITEMMALL_BUY_DIAMOND_INSPIRE_ID);
		m_OneInspireCost = shopTmp.GetPrize(roleItemMall.iGuildWarInspireDiamond);
		lbOneInspireCost.text = (playerDiamond>=m_OneInspireCost)?m_OneInspireCost.ToString():GameDataDB.GetString(8655)+m_OneInspireCost.ToString()+GameDataDB.GetString(1329);
		m_TenInpireCost = 0;
		for(int i=0; i<GameDefine.GUILDWAR_BUYINSPIRE_TEN; ++i)
		{
			m_TenInpireCost += shopTmp.GetPrize(roleItemMall.iGuildWarInspireDiamond+i);
		}
		lbTenInspireCost.text = (playerDiamond>=m_TenInpireCost)?m_TenInpireCost.ToString():GameDataDB.GetString(8655)+m_TenInpireCost.ToString()+GameDataDB.GetString(1329);
	}
	//-------------------------------------------------------------------------------------------------
	//更新寵物總戰力
	public void SetPetTotalPower()
	{
		GuildBaseData guildData = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
		if (guildData == null)
			return;
		int[] totalPower = ARPGApplication.instance.m_GuildSystem.CollectBossBattlePetPower();
		m_PetTotalPower = 0;
		for(int i=0; i<totalPower.Length; ++i)
		{
			m_PetTotalPower += totalPower[i];
		}
		float inspireRatio = (m_NormalInspireAtk+m_FormationInspireAtk+m_PavilionInspireAtk)/100.0f;
		m_PetTotalPower = Mathf.FloorToInt(m_PetTotalPower*(1+inspireRatio));
		jumpNumber.SetEndNumber(m_PetTotalPower);
	}
	//-------------------------------------------------------------------------------------------------
	//若自己公會有佔領據點時更新每分鐘可獲得積分字串
	public void SetMinuteGainScore(int score)
	{
		lbGuildGainScore.text = string.Format(GameDataDB.GetString(8660),score);	//公會獲得積分: {0}/分鐘
		SwitchMinuteGainScore(score > 0);
	}
	//-------------------------------------------------------------------------------------------------
	//設定公會總積分
	public void SetGuildTotalScore(int score)
	{
		lbGuildTotalScore.text = score.ToString();
		lbGuildScoreTitle.gameObject.SetActive(score >= 0);
	}
	#endregion
	//-------------------------------------------------------------------------------------------------
	private void BattleCdTimesUp(Enum_CdTimerType cdType)
	{
		SwitchReadyBar(false);
		if (m_BattleReadyTimer == null)
			return;
		m_BattleReadyTimer.CloseCountDown();
		m_BattleReadyTimer = null;
	}
	//-------------------------------------------------------------------------------------------------
	private void GuildBossTimesUp(Enum_CdTimerType cdType)
	{
		if (m_GuildBossTimer == null)
			return;
		m_GuildBossTimer.CloseCountDown();
		m_GuildBossTimer = null;
		GuildBossState gbState = ARPGApplication.instance.GetGameStateByName(GameDefine.GUILDBOSS_STATE) as GuildBossState;
		if (gbState != null)
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(8658)); 	//"公會王戰爭活動結束"
			gbState.OnBtnCloseClick(null);
		}
	}
	//-------------------------------------------------------------------------------------------------
	///<summary>未上陣寵物wrap更新事件</summary>
	public void AssignPetData(GameObject go,int wrapIndex,int RealIndex)
	{
		RealIndex = Mathf.Abs(RealIndex);
		
		if(RealIndex >= m_PlayerPetDataList.Count)
		{
			go.SetActive(false);
		}
		else
		{
			S_PetData petdata = m_PlayerPetDataList[RealIndex]; 		//從List中取出來
			Slot_PetData sPet = go.GetComponent<Slot_PetData>();
			sPet.spInfoBG.gameObject.SetActive(false);
			sPet.spInfoMask.gameObject.SetActive(false);
			sPet.lbInfo.text = "";
			sPet.SetSlot(petdata);
//			sPet.lbBattleNumber.gameObject.SetActive(false);
			for (int i = 0; i < m_BossBattlePetList.Count; i++) 
			{
				if(m_BossBattlePetList[i].PetID == petdata.iPetDBFID)
				{
					sPet.spriteBorder.gameObject.SetActive(true);
//					sPet.lbBattleNumber.text = m_BossBattlePetList[i].lbPartyNumber.text;
//					sPet.lbBattleNumber.gameObject.SetActive(true);
					break;
				}
			}
			go.SetActive(true);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchReadyPetUI(bool bSwitch)
	{
		panelReadyPetList.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchInspireNoteUI(bool bSwitch)
	{
		panelInspireNoteUI.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchFullCollider(bool bSwitch)
	{
		gFullScreenCollider.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchExplainationUI(bool bSwitch)
	{
		panelExplaination.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchMinuteGainScore(bool bSwitch)
	{
		lbGuildGainScore.gameObject.SetActive(bSwitch);
	}
	//-------------------------------------------------------------------------------------------------
	public void SwitchReadyBar(bool bSwitch)
	{
		TweenAlpha ta = lbReadyTitle.GetComponent<TweenAlpha>();
		if (ta != null)
		{
			if (!bSwitch)
			{
				ta.ResetToBeginning();
				ta.PlayForward();
			}
			else
			{
				ta.enabled = false;
				ta.ResetToBeginning();
				lbReadyTitle.alpha = 1.0f;
			}
		}
		else
		{
			lbReadyTitle.gameObject.SetActive(bSwitch);
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void PlayPetInspireEffect()
	{
		for(int i=0;i<m_BossBattlePetList.Count; ++i)
		{
			if (m_BossBattlePetList[i].PetID <= 0)
				continue;
			m_BossBattlePetList[i].PlayInspireEffect();
		}
		TweenScale tc = lbInspireAtkValue.GetComponent<TweenScale>();;
		if (tc != null)
		{
			tc.ResetToBeginning();
			tc.PlayForward();
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void PlayBossBaseAnim()
	{
		//播放佔領或解除動畫
		for(int i=0; i<m_BossBaseList.Count; ++i)
		{
			if (m_BossBaseList[i].IsPlayOccupyAnim)
				m_BossBaseList[i].PlayOccupyAnim();
			else if (m_BossBaseList[i].IsPlayReleaseAnim)
				m_BossBaseList[i].PlayReleaseAnim();
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void CloseTopHistory()
	{
		gTopHistory.SetActive(false);
		lbTopHistory1.text = null;
	}
	//-------------------------------------------------------------------------------------------------
	//檢查是否關閉公會獲得積分字串
//	public void CheckCloseGainScore()
//	{
//		bool myGuild = false;
//		for(int i=0; i<m_BossBaseList.Count; ++i)
//		{
//			if (m_BossBaseList[i].BaseStatus == ENUM_BossBaseStatus.MyGuildOwn)
//			{
//				myGuild = true;
//				break;
//			}
//		}
//		if (!myGuild)
//			SwitchMinuteGainScore(false);
//	}
}
