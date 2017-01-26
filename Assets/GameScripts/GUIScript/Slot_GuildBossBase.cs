using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_GuildBossBase : MonoBehaviour 
{
	public UIButton 		btnBossBase		 		= null;
	public UILabel 			lbBaseOwner				= null;
	public UILabel 			lbRemainTime			= null;
	public UILabel 			lbProtectionTime		= null;
	public UISprite 		spBossBase				= null;
	public UISprite 		spBaseBG				= null;
	public UIProgressBar	barBossHP				= null;
	public UILabel 			lbBossHP				= null;
	public TweenAlpha 		taBossHP				= null;
	public UIProgressBar	barBossShield			= null;
	public UILabel 			lbBossShield			= null;
	public UILabel 			lbBossLV				= null;
	public Animation 		animOccupy	 			= null;	//佔領動畫
	public Animation 		animRelease		 		= null;	//解除動畫
	public UISprite 		spStatusAtk				= null;
	public UISprite 		spStatusDef				= null;
	public UILabel 			lbDamage				= null;	//受傷文字(樣版)
	public UISprite 		spBattleFog				= null;
	public UISprite 		spMasterProperty		= null;
	public GameObject 		gAddShieldEffect		= null;
	//---------------------------各類資料--------------------------------
	[HideInInspector] public ENUM_BossBaseStatus 	BaseStatus	 		= ENUM_BossBaseStatus.NoOwner;
	[HideInInspector] public int					m_BossInspireCost	= 0;
	[HideInInspector] public int					m_BossHP			= int.MinValue;
	[HideInInspector] public int					m_BossShield		= int.MaxValue;
	private int 			m_BaseID				= 0;
	public int BaseID	{get {return m_BaseID;} set {m_BaseID = value;}}
	private int 			m_BossID				= 0;
	public int BossID	{get {return m_BossID;} set {m_BossID = value;}}
	private int 			m_GuildOwnerID			= 0;
	public int GuildOwnerID	{get {return m_GuildOwnerID;} set {m_GuildOwnerID = value;}}
	//
	[HideInInspector]public CdTimer 		m_BossRemainCDTimer		= null;
	[HideInInspector]public CdTimer 		m_ProtectCDTimer		= null;
	[NonSerialized]private S_GuildWars_Tmp  m_GuildWarsTmp			= null;
	[NonSerialized]private S_GuildLevel_Tmp m_GuildBuildingTmp		= null;
	[NonSerialized]public UI_GuildBoss 		m_uiGuildBoss			= null;
	private bool 							m_IsDataReady			{get{return ARPGApplication.instance.m_GuildSystem.m_IsGuildBossDataReady;}}
	//--------------------------執行用變數----------------------------
	[HideInInspector] public int 			SlotIndex 				= -1;	//該Slot於管理器中是第幾個
	private const string 					m_OccupyAnimName		= "UI_GuildBossSeal";
	private const string 					m_ReleaseAnimName		= "UI_GuildBossSeal";
	private bool							m_IsPlayOccupyAnim		= false;
	public bool								IsPlayOccupyAnim {get{return m_IsPlayOccupyAnim;} set{m_IsPlayOccupyAnim = value;}}
	private bool							m_IsOtherGuildOccupy	= false;
	public bool								IsOtherGuildOccupy {get{return m_IsOtherGuildOccupy;} set{m_IsOtherGuildOccupy = value;}}
	private bool							m_IsPlayReleaseAnim		= false;
	public bool								IsPlayReleaseAnim {get{return m_IsPlayReleaseAnim;} set{m_IsPlayReleaseAnim = value;}}
	//-------------------------------------------------------------------------------------------------
	public void InitialUI()
	{
		lbBaseOwner.text = "";
		lbRemainTime.text = "";
		lbRemainTime.gameObject.SetActive(false);
		lbProtectionTime.gameObject.SetActive(false);
		spStatusDef.gameObject.SetActive(false);

		m_GuildWarsTmp = GameDataDB.GuildWarsDB.GetData(GameDefine.GUILDWAR_DBF_GUID);
		m_GuildBuildingTmp = ARPGApplication.instance.m_GuildSystem.GetGuildBuildingTmp(ENUM_GUILD_BUILD.EMUM_GUILD_BUILDE_Inn);
//		spMasterProperty.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//切換據點狀態
	public void SwitchBaseState(ENUM_BossBaseStatus baseStatus)
	{
		if (BaseStatus == baseStatus)
			return;
		//自己公會佔領消失
		if (BaseStatus == ENUM_BossBaseStatus.MyGuildOwn)
		{
			m_uiGuildBoss.SwitchMinuteGainScore(false);
		}
		BaseStatus = baseStatus;

		switch(baseStatus)
		{
		case ENUM_BossBaseStatus.NoOwner:
			lbRemainTime.gameObject.SetActive(false);
			spStatusAtk.gameObject.SetActive(true);
			if (m_IsDataReady)
				m_IsPlayReleaseAnim = true;
			break;
		case ENUM_BossBaseStatus.MyGuildOwn:
			lbRemainTime.gameObject.SetActive(true);
			spStatusAtk.gameObject.SetActive(false);
			if (m_IsDataReady)
				m_IsPlayOccupyAnim = true;
			break;
		case ENUM_BossBaseStatus.OtherGuildOwn:
			lbRemainTime.gameObject.SetActive(true);
			spStatusAtk.gameObject.SetActive(true);
			if (m_IsDataReady)
				PlayBattleAnim();
			break;
		}
		spStatusDef.gameObject.SetActive(!spStatusAtk.gameObject.activeSelf);
	}
	//-------------------------------------------------------------------------------------------------
	//設定據點資料(資料由Server給予)
	public void SetBossBase(JSONPG_MtoC_GuildKingData bossBaseData)
	{
		if (bossBaseData == null)
			return;
		S_GuildBossValue_Tmp bossTmp = GameDataDB.GuildBossValueDB.GetData(bossBaseData.iGUID);
		if (bossTmp == null)
		{
			UnityDebugger.Debugger.Log("bossTmp = "+bossTmp+" when SetBossBase in GuildBossState");
			return;
		}
		if (m_GuildWarsTmp == null)
		{
			UnityDebugger.Debugger.Log("m_GuildWarsTmp = "+m_GuildWarsTmp+" when SetBossBase in GuildBossState");
			return;
		}
		if (m_GuildBuildingTmp == null)
		{
			UnityDebugger.Debugger.Log("m_GuildBuildingTmp = "+m_GuildBuildingTmp+" when SetBossBase in GuildBossState");
			return;
		}

		m_BossID = bossBaseData.iGUID;

		//處理BOSS血量、防護值，及戰鬥即時動態(跳血、戰鬥圖)
		if (m_BossHP != bossBaseData.iHP)
		{
			int damage = bossBaseData.iHP - m_BossHP;
			if (damage < 0 && m_IsDataReady)
			{
				CreateJumppingLabel(damage);
				PlayBattleAnim();
			}
			SetHpBarValue(bossBaseData.iHP,bossTmp.iGuildBossHP);
		}
		m_BossHP = bossBaseData.iHP;
		if (bossBaseData.iShield != m_BossShield)
		{
			int shield = bossBaseData.iShield - m_BossShield;
			if (m_IsDataReady)
			{
				CreateJumppingLabel(shield);
				if (shield > 0)
					PlayAddShieldEffect();
				else if (shield < 0)
					PlayBattleAnim();
			}
			SetShieldBarValue(bossBaseData.iShield,m_GuildBuildingTmp.InnBossShield);
		}
		m_BossShield = bossBaseData.iShield;

		//設定Boss資料
		if (bossBaseData.iFaceID > 0)
			Utility.SetButtonPic(btnBossBase, bossBaseData.iFaceID,bossBaseData.iFaceID,bossBaseData.iFaceID,bossBaseData.iFaceID);
		else
			Utility.SetButtonPic(btnBossBase, bossTmp.iGuildBossAvaterIcon,bossTmp.iGuildBossAvaterIcon,bossTmp.iGuildBossAvaterIcon,bossTmp.iGuildBossAvaterIcon);

		if (bossTmp.iMasterPropertiesGroup > 0)
		{
			Utility.ChangeAtlasSprite(spMasterProperty, ARPGApplication.instance.GetPetCalssIconID((ENUM_CHARACTER_CALSS)bossTmp.iMasterPropertiesGroup));
			spMasterProperty.gameObject.SetActive(true);
		}
		else
			spMasterProperty.gameObject.SetActive(false);

		lbBossLV.text = bossBaseData.iLevel.ToString();

		//公會BOSS被佔領
		if (bossBaseData.iGuildID > 0)
		{
			//檢查據點是被什麼公會佔領
			GuildBaseData selfGuildData = ARPGApplication.instance.m_GuildSystem.GetGuildBaseData();
			if (selfGuildData != null)
			{
				//被自己公會佔領
				if (selfGuildData.iGuildID == bossBaseData.iGuildID)
				{
					lbBaseOwner.text = GameDataDB.GetString(8656)+bossBaseData.sGuildName+GameDataDB.GetString(1329);
					m_uiGuildBoss.SetMinuteGainScore(bossTmp.iMinuteIntegralValue);
					SwitchBaseState(ENUM_BossBaseStatus.MyGuildOwn);
				}
				//被其他公會佔領
				else
				{
					if (bossBaseData.iEnemyValue > 0)
						lbBaseOwner.text = GameDataDB.GetString(8655)+bossBaseData.sGuildName+GameDataDB.GetString(1329);
					else
						lbBaseOwner.text = bossBaseData.sGuildName;
					SwitchBaseState(ENUM_BossBaseStatus.OtherGuildOwn);
				}
			}

			//處理計時器
			//Server的BOSS上次生成時間
			DateTime bossUpdateTime = new DateTime( 1970, 1, 1, 0, 0, 0 );
			UInt64 uIntDate = UInt64.Parse(bossBaseData.tBornTime);
			bossUpdateTime = bossUpdateTime.AddSeconds(Convert.ToDouble(uIntDate));

			CountDownSystem cdSystem = ARPGApplication.instance.m_CountDownTimerSystem;
			int cdTypeIndex = (int)(Enum_CdTimerType.Enum_CdTimerType_GuildBossRemainTime1)+SlotIndex;
			if (cdTypeIndex >= (int)(Enum_CdTimerType.Enum_CdTimerType_GuildBossRemainTime1) &&
			    cdTypeIndex <= (int)(Enum_CdTimerType.Enum_CdTimerType_GuildBossRemainTime6))
			{
				//註冊攻打CD計時器
				if (m_BossRemainCDTimer == null)
				{
					float occupySecs = bossTmp.iOccupyMinute * 60.0f;
					Enum_CdTimerType cdType = (Enum_CdTimerType)(cdTypeIndex);
					
					m_BossRemainCDTimer = cdSystem.StartCountDown(bossUpdateTime,occupySecs,BossTimesUp,cdType);
					m_BossRemainCDTimer.SyncCdTime(bossUpdateTime,true);
				}
				else
					m_BossRemainCDTimer.SyncCdTime(bossUpdateTime,true);
			
			}
			double secs = Math.Abs(cdSystem.GetAdjustClientTime().Subtract(bossUpdateTime).TotalSeconds);
			if (secs < 30.0f)
			{
				cdTypeIndex = (int)(Enum_CdTimerType.Enum_CdTimerType_GuildBaseProtectTime1)+SlotIndex;
				if (cdTypeIndex >= (int)(Enum_CdTimerType.Enum_CdTimerType_GuildBaseProtectTime1) &&
				    cdTypeIndex <= (int)(Enum_CdTimerType.Enum_CdTimerType_GuildBaseProtectTime6))
				{
					//註冊保護CD計時器
					if (m_ProtectCDTimer == null)
					{
						Enum_CdTimerType cdType = (Enum_CdTimerType)(cdTypeIndex);
						m_ProtectCDTimer = cdSystem.StartCountDown(bossUpdateTime,m_GuildWarsTmp.iProtectionTime,ProtectionTimesUp,cdType);
						m_ProtectCDTimer.SyncCdTime(bossUpdateTime,true);
						SwitchProtectionUI(true);
					}
					else
						m_ProtectCDTimer.SyncCdTime(bossUpdateTime,true);
				}
			}
		}
		//公會BOSS變回野生
		else
		{
			lbBaseOwner.text = GameDataDB.GetString(bossTmp.iGuildBossName);
			SwitchBaseState(ENUM_BossBaseStatus.NoOwner);
		}
		m_GuildOwnerID = bossBaseData.iGuildID;
	}
	//-------------------------------------------------------------------------------------------------
	//設定BOSS HP BAR條
	private void SetHpBarValue(int currentHP, int maxHP)
	{
		float nowRate = (float)currentHP / maxHP;

		if (nowRate > 1.0f)
			nowRate = 1.0f;

		if (nowRate <= 0.1f)
			taBossHP.PlayForward();
		else
			taBossHP.enabled = false;
		//切換毀壞據點圖
		Utility.ChangeAtlasSprite(spBaseBG,(nowRate <= 0.5f)?GameDefine.GUILDWAR_DESTORY_BASE_TEXTURE_ID:GameDefine.GUILDWAR_BASE_TEXTURE_ID); ;

		barBossHP.alpha = (nowRate > 0.0f ? 1:0);
		barBossHP.value = nowRate;

		lbBossHP.text = SetNumberForm(currentHP);
	}
	//-------------------------------------------------------------------------------------------------
	//設定BOSS護盾BAR條
	private void SetShieldBarValue(int currentShield, int maxShield)
	{
		float nowRate = (float)currentShield / maxShield;
		
		if (nowRate > 1.0f)
			nowRate = 1.0f;

		barBossShield.alpha = (nowRate > 0.0f ? 1:0);
		barBossShield.value = nowRate;
		
		lbBossShield.text = SetNumberForm(currentShield);
	}
	//-------------------------------------------------------------------------------------------------
	//設定中文進位字串格式
	private string SetNumberForm(int currentValue)
	{
		currentValue = Math.Abs(currentValue);
		float hundredMillion = (float)Mathf.FloorToInt(((float)currentValue / 100000000) * 10) / 10;
		if (hundredMillion > 1.0f)
			return hundredMillion.ToString()+GameDataDB.GetString(8647);	//億
		else
		{
			float tenThousand = (float)Mathf.FloorToInt(((float)currentValue / 10000) * 10) / 10;
			if (tenThousand > 1.0f)
				return tenThousand.ToString()+GameDataDB.GetString(8646);	//萬
			else
				return currentValue.ToString();
		}
	}
	//-------------------------------------------------------------------------------------------------
	public void UpdateBossRemainTime()
	{
		if (m_BossRemainCDTimer == null)
			return;

		lbRemainTime.text = m_BossRemainCDTimer.ShowTime(ENUM_CDTime_ShowTime.Minute);
	}
	//-------------------------------------------------------------------------------------------------
	public void UpdateProtectTime()
	{
		if (m_ProtectCDTimer == null)
			return;
		
		lbProtectionTime.text = m_ProtectCDTimer.ShowTime(ENUM_CDTime_ShowTime.Second);
	}
	//-------------------------------------------------------------------------------------------------
	//佔領動畫和音校
	public void PlayOccupyAnim()
	{
		//
		TweenColor tc = spBossBase.GetComponent<TweenColor>();
		if (tc != null)
		{
			tc.ResetToBeginning();
			tc.PlayForward();
		}
		//
		animOccupy.Stop();
		animOccupy.Play(m_OccupyAnimName);
		MusicControlSystem.StopOnceSound("Sound_System_025");
		MusicControlSystem.PlaySound("Sound_System_025",1);
		m_IsPlayOccupyAnim = false;
	}
	//-------------------------------------------------------------------------------------------------
	//變回野生動畫和音校
	public void PlayReleaseAnim()
	{
		animRelease.Stop();
		animRelease.Play(m_ReleaseAnimName);
		MusicControlSystem.StopOnceSound("Sound_System_029");
		MusicControlSystem.PlaySound("Sound_System_029",1);
		m_IsPlayReleaseAnim = false;
	}
	//-------------------------------------------------------------------------------------------------
	//生成跳血或防護值文字
	private void PlayBattleAnim()
	{
		spBattleFog.gameObject.SetActive(false);
		spBattleFog.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//生成跳血或防護值文字
	private void PlayAddShieldEffect()
	{
		gAddShieldEffect.SetActive(false);
		gAddShieldEffect.SetActive(true);
	}
	//-------------------------------------------------------------------------------------------------
	//關閉保護效果
	public void ProtectionTimesUp(Enum_CdTimerType cdType)
	{
		SwitchProtectionUI(false);
		m_IsOtherGuildOccupy = false;
		if (m_ProtectCDTimer == null)
			return;
		m_ProtectCDTimer.CloseCountDown();
		m_ProtectCDTimer = null;	
	}
	//-------------------------------------------------------------------------------------------------
	//關閉保護效果
	public void BossTimesUp(Enum_CdTimerType cdType)
	{
		if (m_BossRemainCDTimer == null)
			return;
		m_BossRemainCDTimer.CloseCountDown();
		m_BossRemainCDTimer = null;	
	}
	//-------------------------------------------------------------------------------------------------
	//關閉保護效果
	private void SwitchProtectionUI(bool bSwitch)
	{
		lbProtectionTime.gameObject.SetActive(bSwitch);
		if (bSwitch)
		{
			if (BaseStatus == ENUM_BossBaseStatus.MyGuildOwn)
				Utility.ChangeAtlasSprite(spStatusDef,GameDefine.GUILDWAR_LIGHT_SHIELD_TEXTURE_ID);
			else if (BaseStatus == ENUM_BossBaseStatus.OtherGuildOwn)
				Utility.ChangeAtlasSprite(spStatusAtk,GameDefine.GUILDWAR_DARK_SWORD_TEXTURE_ID);
		}
		else
		{
			Utility.ChangeAtlasSprite(spStatusDef,GameDefine.GUILDWAR_SHIELD_TEXTURE_ID);
			Utility.ChangeAtlasSprite(spStatusAtk,GameDefine.GUILDWAR_SWORD_TEXTURE_ID);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//生成跳血或防護值文字
	private void CreateJumppingLabel(int value)
	{
		if (value == 0)
			return;
		UILabel lbDmg = GameObject.Instantiate(lbDamage) as UILabel;
		TweenPosition tp = lbDmg.GetComponent<TweenPosition>();
		if (tp == null)
		{
			GameObject.Destroy(lbDmg);
			return;
		}
		TweenAlpha ta = lbDmg.GetComponent<TweenAlpha>();
		if (ta == null)
		{
			GameObject.Destroy(lbDmg);
			return;
		}
		lbDmg.transform.parent = lbDamage.transform.parent;
		lbDmg.transform.localPosition = Vector3.zero;
		lbDmg.transform.localScale = Vector3.one;
		lbDmg.transform.localRotation = Quaternion.identity;
		string strValue = SetNumberForm(value);
		lbDmg.text = (value >= 0)?GameDataDB.GetString(8668)+"+"+strValue+GameDataDB.GetString(1329):GameDataDB.GetString(8655)+"-"+strValue+GameDataDB.GetString(1329);
		tp.PlayForward();
		ta.PlayForward();
		GameObject.Destroy(lbDmg.gameObject,ta.duration);
		lbDmg.gameObject.SetActive(true);
	}
}
