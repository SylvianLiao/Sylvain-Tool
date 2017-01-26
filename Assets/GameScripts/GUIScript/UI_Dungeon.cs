 using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

//BuffManager類型
public enum ENUM_BuffManager_Type
{
	ENUM_BM_PLAYER		= 0,	//玩家
	ENUM_BM_PLAYER_PET1,		//玩家寵
	ENUM_BM_PLAYER_PET2,		//玩家寵
	ENUM_BM_ENEMY,				//敵玩家
	ENUM_BM_ENEMY_PET1,			//敵寵
	ENUM_BM_ENEMY_PET2,			//敵寵
	ENUM_BM_BOSS,				//BOSS

	ENUM_BM_MAX
}

public class UI_Dungeon : NGUIChildGUI
{
	public UIButton		btnSkill 				= null; //普攻技能鈕
	public UIButton		btnSkill01 				= null; //技能1按鈕
	public UIButton		btnSkill02 				= null; //技能2按鈕
	public UIButton		btnSkill03 				= null; //技能3按鈕
	public UILabel		labelSkill01CD			= null; //技能1 CD倒數
	public UILabel		labelSkill02CD			= null; //技能2 CD倒數
	public UILabel		labelSkill03CD			= null; //技能3 CD倒數
	public UILabel		labelSkillEXCD			= null;	//EX技  CD倒數
	public UILabel		labelSkillDodgeCD		= null;	//閃躲技 CD倒數
	public UIWidget		widgetDodgePrompt		= null;	//閃躲技提示
	public UIWidget		widgetLockSkill01		= null; //技能1鎖圖
	public UIWidget		widgetLockSkill02		= null; //技能2鎖圖
	public UIWidget		widgetLockSkill03		= null; //技能3鎖圖
	public UILabel		lbSkill01LimitLV		= null; //技能1等級鎖
	public UILabel		lbSkill02LimitLV		= null; //技能2等級鎖
	public UILabel		lbSkill03LimitLV		= null; //技能3等級鎖
	public UIButton		btnPet1Skill			= null;	//寵物1技能
	public UIButton		btnPet2Skill			= null;	//寵物2技能
	public UILabel		labelPet1SkillCD		= null; //寵物1技能CD倒數
	public UILabel		labelPet2SkillCD		= null; //寵物2技能CD倒數
	public UIWidget		widgetPetSkillBG		= null;	//技能2與3按鈕的背景
	public UIButton		btnEXButton				= null;	//爆魂技按鈕
	public UIButton		btnDodgeButton			= null;	//閃躲技按鈕
	public UIButton		btnReturnToLobby 		= null; //返回大廳按鈕(測試用)
	public UIButton		btnReturnToLobby2		= null;	//返回大廳按鈕
	public UILabel		lbReturnToLobby			= null;	//返回字樣
	public UIToggle		toggleAuto 				= null; //自動戰鬥
	public UISprite		tgBackground			= null; //自動戰鬥底圖
	public UISprite		tgActive				= null; //自動戰鬥啟動圖
    public UILabel		lbActive				= null; //自動戰鬥
	public UIButton		btnJoystickSwitch 		= null; //切換搖桿模式
	public UISprite		spriteJoyStickSwitch	= null; //切換操作圖示
	public UISprite		SpriteGesture 			= null; //啟動手勢發動的圖示
	public UISprite		spriteSPFullStatus 		= null; //SP滿量圖(環狀)
	public UISprite		spriteSPCanUseStatus	= null; //SP滿量圖(圖狀)
	public UISprite		spriteSPWorkStatus		= null; //SP技能可發動圖
	public GameObject	GestureLine 			= null; //手勢軌跡線
	public SplineTrailRenderer	GestureTrail	= null; //手勢軌跡線Script
	//
	public UIWidget		PlayerInfo				= null;	//玩家資訊
	public Slot_RoleIcon	PlayerFace			= null;	//玩家大頭圖
	public UISprite		Pet1Face				= null;	//寵物大頭圖
	public UISprite		Pet2Face				= null;	//寵物大頭圖
	public UILabel		PlayerName				= null;	//玩家名稱
	public UISprite		PlayerFullBlood			= null; //玩家滿血血條
	public UISprite		Pet1FullBlood			= null;	//寵物血條
	public UISprite		Pet2FullBlood			= null;	//寵物血條
	//
	public UIWidget		EnemyInfo				= null;	//敵玩家資訊
	public Slot_RoleIcon	EnemyFace			= null;	//敵玩家大頭圖
	public UISprite		EnemyPet1Face			= null;	//敵玩家寵物大頭圖
	public UISprite		EnemyPet2Face			= null;	//敵玩家寵物大頭圖
	public UILabel		EnemyName				= null;	//敵玩家名稱
	public UISprite		EnemyFullBlood			= null;	//敵玩家滿血血條
	public UISprite		EnemyPet1FullBlood		= null;	//敵寵物血條
	public UISprite		EnemyPet2FullBlood		= null;	//敵寵物血條
	//
	public UIWidget		BossInfo				= null;	//Boss資訊
	public UISprite		BossFace				= null;	//Boss大頭圖
	public UILabel		BossName				= null;	//Boss名稱
	public UISprite		BossFullBlood			= null;	//Boss滿血血條
	public UIPanel		BossComeType2			= null;	//Boss登場
	public UIPanel		BossComeType3			= null;	//Boss登場
	//
	public UIWidget		PVPEInfo				= null;	//PVPE用準備UI
	public UIButton		btnPVPEAuto				= null;	//PVPE用自動按鈕
	public UILabel		lbPVPEAuto				= null;	//PVPE用自動按鈕
	public UIButton		btnPVPEManual			= null;	//PVPE用手動按鈕
	public UILabel		lbPVPEManual			= null;	//PVPE用手動按鈕
	public UIWidget		PVPEPlay				= null;	//PVPE演出
	//
	public UIButton		btnGM					= null; //GM按鈕
	public UIButton		btnTeamStyle			= null; //隊形變化
	public UILabel		lbTeamStyle				= null; //隊形字樣
	public UISprite		spriteTeamStyle			= null; //隊形圖示
	public UILabel		lbLimitTime				= null; //剩餘時間
	//
	public UILabel		lbWaveInfo				= null;	//波段顯示
	//
	public UIWidget		TeachJoyStick			= null;	//Joystick教學
	public UISprite		spriteTeachJoystick		= null;	//Joystick教學
	//護駕按鈕用
	public UIWidget		wgProtecter				= null;	//護駕按鈕用集合
	public UIButton		btnProtecter			= null;	//主要召喚護駕按鈕
	public UISprite		spProtecter				= null;	//主要召喚護駕按鈕圖示
	public UILabel		lbProtecter				= null;	
	public UISprite		spProtecterCD			= null;	//主要召喚護駕按鈕CD圖示
	public UIButton		btnChooseRightPet		= null;	//選取右邊護駕按鈕
	public BoxCollider	colliderChooseRightPet	= null;
	public UIButton		btnChooseLeftPet		= null;	//選取左邊護駕按鈕
	public BoxCollider	colliderChooseLeftPet	= null;	
	public UIButton []			m_BtnProtecterArray	= new UIButton[6]; 				//護駕按鈕陣列
	public GameObject []		m_ProtecterPatern	= new GameObject[5]; 			//護駕按鈕定位用模板
	//PVPE3用
	public UIWidget		wgPVPE3					= null;
	public UIGrid		gPVPE3					= null;
	public UISprite		spritePlayerWin1		= null;
	public UISprite		spritePlayerWin2		= null;
	public UISprite		spriteEnemyWin1			= null;
	public UISprite		spriteEnemyWin2			= null;
	//
	[HideInInspector]public string		m_OpenProtecterAnimName			= "UI_Dungeon_OpenProtecter";	//開啟護駕按鈕動畫名稱
	[HideInInspector]public string		m_CloseProtecterAnimName		= "UI_Dungeon_CloseProtecter";	//關閉護駕按鈕動畫名稱
	//
	[SerializeField]private BuffManager						PlayerBuffManager		= null;	//玩家BuffManager
	[SerializeField]private BuffManager						PlayerPet1BuffManager	= null;	//玩家寵BuffManager
	[SerializeField]private BuffManager						PlayerPet2BuffManager	= null;	//玩家寵BuffManager
	[SerializeField]private BuffManager						EnemyBuffManager		= null;	//敵玩家BuffManager
	[SerializeField]private BuffManager						EnemyPet1BuffManager	= null;	//敵玩家寵BuffManager
	[SerializeField]private BuffManager						EnemyPet2BuffManager	= null;	//敵玩家寵BuffManager
	[SerializeField]private BuffManager						BossBuffManager			= null;	//Boss BuffManager
	private Dictionary<ENUM_BuffManager_Type, BuffManager>	buffManagers			= new Dictionary<ENUM_BuffManager_Type, BuffManager>();		//所有BuffManager管理器
	//
	private const string	GUI_SMARTOBJECT_NAME = "UI_Dungeon";
	private ARPGBattle		mainBattle				= null;	//主玩家
	private Transform		lockSkillT				= null;
	private bool 			m_isProtecterBtnClose 	= false;
	//關閉普攻按鈕時執行
	public delegate void CanNotUseNormalAttack();
	public CanNotUseNormalAttack	OnCanNotUseNormalAttack;
	public bool 					IsProtecterBtnClose
	{
		get{return m_isProtecterBtnClose;}
		set{m_isProtecterBtnClose = value;}
	}

	//-------------------------------------新手教學用-------------------------------------
	public UIPanel		panelGuide						= null; //教學集合
	public UIButton		btnTopFullScreen				= null; //最上層的全螢幕按鈕
	public UIButton		btnFullScreen					= null; //全螢幕按鈕
	public UISprite		spGuideFocusAttack				= null; //導引分散集中攻擊按鈕
	public UILabel		lbGuideFocusAttack				= null;
	public UISprite		spGuideProtecterBtn				= null; //導引召喚護駕
	public UILabel		lbGuideProtecterBtn				= null;
	//-----------------------------------------------------------------------------------------------------
	private UI_Dungeon() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	private  void SetString()
	{
		lbPVPEAuto.text		= GameDataDB.GetString(9560);	//"自動操作"
		lbPVPEManual.text	= GameDataDB.GetString(9561);	//"手動操作"
        lbActive.text   	= GameDataDB.GetString(2173);	//"自動操作"
		lbReturnToLobby.text = GameDataDB.GetString(1951);	//"離開"

	}
	//-----------------------------------------------------------------------------------------------------
	private void Update()
	{
		if(mainBattle == null)
			mainBattle = ARPGApplication.instance.m_tempGameObjectSystem.GetARPGBattleByMain();
		if(btnSkill!=null && lockSkillT==null)
			lockSkillT = btnSkill.transform.FindChild("Sprite(Lock)");

		if(btnSkill.gameObject.activeSelf == true)
		{
			if(mainBattle!=null && lockSkillT!=null)
			{
				//自動模式鎖定普攻按鈕
				if(mainBattle.AutoAttack==ENUM_AUTO_ATTACK.ENUM_AA_OPEN		||
				   mainBattle.AutoAttack==ENUM_AUTO_ATTACK.ENUM_AA_PAUSE	)
				{
					if(lockSkillT.gameObject.activeSelf == false)
					{
						lockSkillT.gameObject.SetActive(true);
						//灰階變化
						GrayButton(btnSkill,true);
						if(OnCanNotUseNormalAttack != null)
							OnCanNotUseNormalAttack();
					}
				}
				else
				{
					if(lockSkillT.gameObject.activeSelf == true)
					{
						lockSkillT.gameObject.SetActive(false);
						//回復變化
						GrayButton(btnSkill,false);
						if(OnCanNotUseNormalAttack != null)
							OnCanNotUseNormalAttack();
					}
				}
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void HideAllControlUI()
	{
		showPetSkillBG(false);
		showBtnSkill(false);
		showBtnSkill1(false);
		showBtnSkill2(false);
		showBtnSkill3(false);
		showBtnPet1Skill(false);
		showBtnPet2Skill(false);
		showBtnEXButton(false);
		showBtnDodgeButton(false);
		showToogleAuto(false);
		showJoystickSwitch(false);
		showTeachJoystick(false);
		showGestureLine(false);
		showPlayerInfo(false);
		showEnemyInfo(false);
		showBossInfo(false);
		showPVPEInfo(false);
		showPVPEPlay(false);
		showPVPE3(false);
		showTeamGroup(false);
		showProtecter(false);
		showDodgePrompt(false);

		if(OnCanNotUseNormalAttack != null)
			OnCanNotUseNormalAttack();
	}
	//-----------------------------------------------------------------------------------------------------
	public void showPetSkillBG(bool bShow)		{	widgetPetSkillBG.gameObject.SetActive( bShow );								}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnSkill(bool bShow)		{	btnSkill.transform.parent.gameObject.SetActive( bShow );					
													if(OnCanNotUseNormalAttack != null)
														OnCanNotUseNormalAttack();												}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnSkill1(bool bShow)		{	btnSkill01.transform.parent.gameObject.SetActive( bShow );					}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnSkill2(bool bShow)		{	btnSkill02.transform.parent.gameObject.SetActive(bShow);					}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnSkill3(bool bShow)		{	btnSkill03.transform.parent.gameObject.SetActive(bShow);					}
	//-----------------------------------------------------------------------------------------------------
	public void showLockSkill1(bool bShow)		{	widgetLockSkill01.gameObject.SetActive( bShow );	btnSkill01.gameObject.SetActive( !bShow );}
	//-----------------------------------------------------------------------------------------------------
	public void showLockSkill2(bool bShow)		{	widgetLockSkill02.gameObject.SetActive( bShow );	btnSkill02.gameObject.SetActive( !bShow );}
	//-----------------------------------------------------------------------------------------------------
	public void showLockSkill3(bool bShow)		{	widgetLockSkill03.gameObject.SetActive( bShow );	btnSkill03.gameObject.SetActive( !bShow );}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnPet1Skill(bool bShow)	{	btnPet1Skill.transform.parent.gameObject.SetActive( bShow );				}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnPet2Skill(bool bShow)	{	btnPet2Skill.transform.parent.gameObject.SetActive( bShow );				}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnEXButton(bool bShow)		{	btnEXButton.transform.parent.gameObject.SetActive( bShow );					}
	//-----------------------------------------------------------------------------------------------------
	public void LockBtnEXButton(bool bLock)		{	btnEXButton.isEnabled = bLock;												}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnDodgeButton(bool bShow)	{	btnDodgeButton.transform.parent.gameObject.SetActive( bShow );				}
	//-----------------------------------------------------------------------------------------------------
	public void showToogleAuto(bool bShow)		{	toggleAuto.transform.parent.gameObject.SetActive( bShow );					}
	//-----------------------------------------------------------------------------------------------------
	public void showJoystickSwitch(bool bShow)	{	btnJoystickSwitch.transform.parent.gameObject.SetActive( bShow );			}
	//-----------------------------------------------------------------------------------------------------
	public void showTeachJoystick(bool bShow)	{	TeachJoyStick.transform.gameObject.SetActive( bShow );						}
	//-----------------------------------------------------------------------------------------------------
	public void showGestureLine(bool bShow)		{	GestureLine.SetActive( bShow );												}
	//-----------------------------------------------------------------------------------------------------
	public void showPlayerInfo(bool bShow)		{	PlayerInfo.gameObject.SetActive( bShow );									}
	//-----------------------------------------------------------------------------------------------------
	public void showEnemyInfo(bool bShow)		{	EnemyInfo.gameObject.SetActive( bShow );									}
	//-----------------------------------------------------------------------------------------------------
	public void showBossInfo(bool bShow)		{	BossInfo.gameObject.SetActive( bShow );										}
	//-----------------------------------------------------------------------------------------------------
	public void showPVPEInfo(bool bShow)		{	PVPEInfo.gameObject.SetActive( bShow );										}
	//-----------------------------------------------------------------------------------------------------
	public void showPVPEPlay(bool bShow)		{	PVPEPlay.gameObject.SetActive( bShow );										}
	//-----------------------------------------------------------------------------------------------------
	public void showPVPE3(bool bShow)			{	wgPVPE3.gameObject.SetActive( bShow );										}
	//-----------------------------------------------------------------------------------------------------
	public void showDodgePrompt(bool bShow)		{	widgetDodgePrompt.gameObject.SetActive (bShow);								}
	//-----------------------------------------------------------------------------------------------------
	public void showBtnGM(bool bShow)			{	if(btnGM!=null)	btnGM.gameObject.SetActive( bShow );						}
	//-----------------------------------------------------------------------------------------------------
	public void showReturnToLobby(bool bShow)	{	if(btnReturnToLobby!=null)	btnReturnToLobby.gameObject.SetActive( bShow );	}
	//-----------------------------------------------------------------------------------------------------
	public void showReturnToLobby2(bool bShow)
	{
		if(btnReturnToLobby2 == null)
			return;
		DungeonBaseState dbs = DungeonBaseState.getNowDungeonBaseState();
		if(dbs == null)
			return;

		if(bShow == true)
		{
			if(dbs is PVPBaseState)
				btnReturnToLobby2.gameObject.SetActive(false);
			else
				btnReturnToLobby2.gameObject.SetActive(true);
		}
		else
			btnReturnToLobby.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	public void showTeamGroup(bool bShow)	
	{
		btnTeamStyle.gameObject.SetActive(bShow);
		lbTeamStyle.gameObject.SetActive(bShow);
		spriteTeamStyle.gameObject.SetActive(bShow);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetReturnbtnStr(string str)		{	lbReturnToLobby.text = str;													}
	//-----------------------------------------------------------------------------------------------------
	public void showBossCome(bool bShow, int type)
	{
		UIPanel BossCome = null;
		switch(type)
		{
		case 2:	BossCome = BossComeType2;	break;
		case 3:	BossCome = BossComeType3;	break;
		}
		EnableFalse enableFalseEffect = BossCome.gameObject.AddComponent<EnableFalse>();
		enableFalseEffect.duration = 2.2f;
		BossCome.gameObject.SetActive( bShow );
	}
	//-----------------------------------------------------------------------------------------------------
	public void showProtecter(bool bShow)		
	{
		DungeonBaseState dbs = DungeonBaseState.getNowDungeonBaseState();
		if(dbs == null)
			return;
		if(bShow == true)
			dbs.SetButtonProtecter(false);
		else
			wgProtecter.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	public bool getBossInfoActive()				{	return BossInfo.gameObject.activeSelf;										}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize ();

		buffManagers.Add(ENUM_BuffManager_Type.ENUM_BM_PLAYER, 		PlayerBuffManager		);
		buffManagers.Add(ENUM_BuffManager_Type.ENUM_BM_PLAYER_PET1,	PlayerPet1BuffManager	);
		buffManagers.Add(ENUM_BuffManager_Type.ENUM_BM_PLAYER_PET2,	PlayerPet2BuffManager	);
		buffManagers.Add(ENUM_BuffManager_Type.ENUM_BM_ENEMY,		EnemyBuffManager		);
		buffManagers.Add(ENUM_BuffManager_Type.ENUM_BM_ENEMY_PET1,	EnemyPet1BuffManager	);
		buffManagers.Add(ENUM_BuffManager_Type.ENUM_BM_ENEMY_PET2,	EnemyPet2BuffManager	);
		buffManagers.Add(ENUM_BuffManager_Type.ENUM_BM_BOSS,		BossBuffManager			);

		Utility.ChangeAtlasSprite(tgBackground,10007);
		Utility.ChangeAtlasSprite(tgActive,10007);
		Utility.ChangeAtlasSprite(spriteTeamStyle,(ARPGApplication.instance.m_TeamGroup ? 10009:10010));
		Utility.ChangeAtlasSprite(spriteJoyStickSwitch,10008);
		SetString();
	}
	//-----------------------------------------------------------------------------------------------------
	public void ClearBuff(ENUM_BuffManager_Type type)
	{
		if (buffManagers.ContainsKey(type))
		{
			if(buffManagers[type] != null)
				buffManagers[type].Clear();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void ClearBuff(ENUM_BuffManager_Type type, int GUID)
	{
		if (buffManagers.ContainsKey(type))
		{
			if(buffManagers[type] != null)
			buffManagers[type].RemoveBuff(GUID);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void AddBuff(ENUM_BuffManager_Type type, int GUID)
	{
		if (buffManagers.ContainsKey(type))
		{
			if(buffManagers[type] != null)
				buffManagers[type].AddBuff(GUID);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//控制頭像反灰與否
	public void GrayIcon(UISprite sp,bool sw)
	{
		if(sp == null)
			return;
		
		float fGrayValue = (sw ? 0.0f:1.0f); 
		//灰階與否變化
		Color Graycolor = new Color(fGrayValue, sp.color.g, sp.color.b);
		
		sp.color = Graycolor;
	}
	//-----------------------------------------------------------------------------------------------------
	//控制按鈕反灰與否
	public void GrayButton(UIButton btn,bool sw)
	{
		if(btn == null)
			return;
		
		float fGrayValue = (sw ? 0.0f:1.0f); 
		//灰階與否變化
		if(!sw)
			btn.isEnabled = true;
		
		Color Graycolor = new Color(fGrayValue, btn.defaultColor.g, btn.defaultColor.b);
		btn.defaultColor = Graycolor;
		
		btn.isEnabled = false;
		btn.isEnabled = true;
		
		if(sw)
			btn.isEnabled = false;
	}
	//-----------------------------------------------------------------------------------------------------
	//控制護駕按鈕反灰與否
	public void GrayProtecterButton(bool sw)
	{
		m_isProtecterBtnClose = sw;

		btnProtecter.isEnabled = !sw;
		btnChooseLeftPet.isEnabled = !sw;
		btnChooseRightPet.isEnabled = !sw;
		colliderChooseLeftPet.enabled = !sw;
		colliderChooseRightPet.enabled = !sw;
	}
}
