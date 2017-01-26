using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

// class UISpriteShowStraight: MonoBehaviour
// {
// 	public bool				bTest 			= false;
// 	public bool				bRun 			= false;
// 	public float			effectTime 		= 1.5f;
// 	public iTween.EaseType	easetype 		= iTween.EaseType.linear;
// 	//
// 	private UISprite	sprite = null;
// 	
// 	//-----------------------------------------------------------------------------------------------------
// 	private void Start()
// 	{
// 		ExeShowStraight();
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	private void ExeShowStraight()
// 	{
// 		sprite = GetComponent<UISprite>();
// 		if(sprite == null)
// 		{
// 			UnityDebugger.Debugger.LogError("此效果需使用UISprite Component!!");
// 		}
// 		
// 		if(sprite!=null && bRun==false)
// 		{
// 			iTween.ValueTo(gameObject, iTween.Hash("from", 		0.0f,
// 			                                       "to",		1.0f,
// 			                                       "easetype",	easetype,
// 			                                       "loopType",	iTween.LoopType.none,
// 			                                       "onupdate",	"updateFillAmount",
// 			                                       "onstart",	"onStart",
// 			                                       "oncomplete","onComplete",
// 			                                       "time",		effectTime						));
// 		}
// 		
// 		bRun = true;
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	private void Update()
// 	{
// 		if(bTest==false && gameObject.GetComponent<iTween>()==null)
// 		{
// 			GameObject.Destroy(this);
// 		}
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	private void onStart()
// 	{
// 		if(sprite!=null)	sprite.fillAmount = 0.0f;
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	private void updateFillAmount(float newValue)
// 	{
// 		if(sprite!=null)	sprite.fillAmount = newValue;
// 	}
// 	//-----------------------------------------------------------------------------------------------------
// 	private void onComplete()
// 	{
// 		bRun = false;
// 	}
// }

class UI_CreateRole : NGUIChildGUI
{
    public List<UIButton> AllPlayer                 = new List<UIButton>();
	public UIButton btnCreate						= null;
	public UIButton btnRandomRoleName				= null;
	public UIInput	InputName 						= null;
    public UILabel  RoleName 						= null;
    public UILabel  EnterGame 						= null;
	//
	public UISprite spriteAxeName				= null;	//尊武職業名稱
    public UISprite spriteBowName				= null;	//獵魔職業名稱
    public UISprite spriteSwrodName				= null;	//劍俠職業名稱
	//
//    public UILabel  lbCareerText			        = null;
    public UISprite spriteCareerText			    = null;
	//
    public UISprite HighFrame                       = null;

    public UILabel  RoleName1 						= null;
    public UILabel  RoleName2 						= null;
    public UILabel  RoleName3 						= null;
    public UILabel  RoleName4						= null;
    public UILabel  RoleName5						= null;
    public UILabel  RoleName6						= null;

	private List<UITexture>	textureRoleSet			= new List<UITexture>();
	//
	public ENUM_SexType		iGender 				= ENUM_SexType.ENUM_SexType_Man;	//性別
	public ENUM_Vocation	iVocation				= ENUM_Vocation.ENUM_Vocation_1;	//職業
	public ENUM_RoleFace 	iRoleFace				= ENUM_RoleFace.ENUM_RoleFace_MalePike;
    public string           jumpName                = "";
	public float			SwitchTime				= 2;	
	//
    public string SwordAudio;
    public string AxeAudio;
    public string BowAudio;
	//
	public SpinWithMouse	LimitRotateModule		= null; //旋轉角色
	//角色位置
	private Vector3 	CenterLocation;				//主位
	private Vector3 	R1Location;					//右1
	private Vector3 	L1Location;					//左1
	private Vector3		BehindCenterLocation;		//隱藏到後面
	private Vector3 	R2Location;					//右2
	private Vector3 	L2Location;					//左2
	//紀錄左右旋的狀態
	private int 		RightSlideState;
	private int 		LeftSlideState;

    public GameObject nowPlayer = null;
    private GameObject newPlayer = null;

    private int nowPlayerID             = 0;


	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_CreateRole";

	//---------------------------------------------------------------------------------------------------
	private UI_CreateRole() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//---------------------------------------------------------------------------------------------------
	//確認命名規則
	public bool CheckName()
	{
		if(ARPGApplication.instance.CheckShieldingString(InputName.value))
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(407)); //角色名稱異常，創角失敗。
			return false;
		}

		if(InputName.value.Length>=GameDefine.MIN_NAME_LENGHT && InputName.value.Length <= GameDefine.MAX_NAME_LENGHT)
		{
			return true;
		}
		else
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(GameDataDB.GetString(400)); //角色名稱異常，創角失敗。
			return false;
		}

		return false;
	}
	//---------------------------------------------------------------------------------------------------
	public void Init()
	{
        RoleName1.text = GameDataDB.GetString(950001);
        RoleName2.text = GameDataDB.GetString(950003);
        RoleName3.text = GameDataDB.GetString(950002);
        RoleName4.text = GameDataDB.GetString(950001);
        RoleName5.text = GameDataDB.GetString(950003);
        RoleName6.text = GameDataDB.GetString(950002);

		// 翻譯字串
		RoleName.text = GameDataDB.GetString(15081);
		EnterGame.text = GameDataDB.GetString(15053);
	}


	//-----------------------------------------------------------------------------------------------------
    // index 從0開始 0、 1 、 2 = 女生 ， 3、 4、 5 = 男生 
    // 1.尊武 2.劍俠 3.獵魔
	public void SetData(int index, string jumpName)
	{
        if(index == nowPlayerID)
            return;

        nowPlayerID = index;
        int Vocation = index % 3;
		CareerNameShow ((ENUM_Vocation)Vocation);
        this.jumpName = jumpName;
		iGender		= (ENUM_SexType)(index / 3);
		iRoleFace	= (ENUM_RoleFace)(1105 + index);
        //MainRoleSetState ();
	}

	//-----------------------------------------------------------------------------------------------------
	void CareerNameShow(ENUM_Vocation CareerDiff)
	{
       // AudioClip Audio = null ; 
		switch(CareerDiff)
		{
		case ENUM_Vocation.ENUM_Vocation_1:
			//音效
            //Audio = ResourceManager.Instance.GetSound(AxeAudio);
			MusicControlSystem.PlaySound("AxeAudio",1);
			//
			iVocation	= CareerDiff;	//武者
			//職業標語

			SloganShowEffect (18502, spriteAxeName);
			break;
		case ENUM_Vocation.ENUM_Vocation_3:
			//
            //Audio = ResourceManager.Instance.GetSound(BowAudio); 
			MusicControlSystem.PlaySound("BowAudio",1);
			//
			iVocation	= CareerDiff;	//獵魔
			//職業標語
			SloganShowEffect (18503,spriteBowName);
			break;
		case ENUM_Vocation.ENUM_Vocation_2:
			//
            //Audio = ResourceManager.Instance.GetSound(SwordAudio); 
			MusicControlSystem.PlaySound("SwordAudio",1);
			//
			iVocation	= CareerDiff;	//劍俠
			//職業標語
			SloganShowEffect (18504, spriteSwrodName);
			break;
		}
        //audio.PlayOneShot (Audio);
	}
	//-----------------------------------------------------------------------------------------------------
	public void MainRoleSetState()
	{
        Destroy(nowPlayer);
        //主玩家角色生成
        //取得創角DBF
        S_CreateCharacter_Tmp ccDBF = GameDataDB.CreateCharacterDB.GetData((int)iVocation + 1000);
        if (ccDBF == null)
        {
            UnityDebugger.Debugger.LogError("創角DBF錯誤!! iVocation=[" + iVocation.ToString() + "]");
            return;
        }
        S_Item_Tmp m_bodyDBF	= GameDataDB.ItemDB.GetData(ccDBF.Body);
        S_Item_Tmp m_headDBF	= GameDataDB.ItemDB.GetData(ccDBF.Head);
        S_Item_Tmp m_weaponDBF	= GameDataDB.ItemDB.GetData(ccDBF.Weapon);
		S_Item_Tmp m_fashionDBF = null;
		S_Item_Tmp m_wingDBF	= GameDataDB.ItemDB.GetData(ccDBF.Wing);

        nowPlayer = ARPGCharacterFactory.CreateAvatar(iGender,
                                                      m_bodyDBF,
                                                      m_headDBF,
                                                      m_weaponDBF,
		                                              m_fashionDBF,
		                                              m_wingDBF		);


        //指定角色顯示高面數模型(有問題找小開)
        LODSwitcher[] switchers = nowPlayer.GetComponentsInChildren<LODSwitcher>();
        for (int i = 0; i < switchers.Length; i++)
            switchers[i].SetFixedLODLevel(0);

        Proc_CheckPlayerComponent(nowPlayer, 5,jumpName);
		LimitRotateModule.target = nowPlayer.transform;
	}

	//-----------------------------------------------------------------------------------------------------
	//Slogan顯示效果
	private void SloganShowEffect(int SloganID, UISprite SpriteCareer)
	{
        Utility.ChangeAtlasSprite(spriteCareerText,SloganID);
        //spriteCareerText.spriteName = SloganName;
        //lbCareerText.text = SloganName;

        spriteAxeName.gameObject.SetActive(false);	
        spriteBowName.gameObject.SetActive(false);	
        spriteSwrodName.gameObject.SetActive(false);	
        SpriteCareer.gameObject.SetActive(true);
		//
	}

    public void SetPlayerPosition(GameObject parent)
    {
        nowPlayer.transform.parent = parent.transform;
        nowPlayer.transform.localPosition = Vector3.zero;
        nowPlayer.transform.eulerAngles = new Vector3(0, 160.3011f, 0);
    }
    
    //--------------------------------------------------------------------------------
    //處理角色物件所有Component
    public bool Proc_CheckPlayerComponent(GameObject m_GameObject, int iLv, string jumpName)
    {
        if (m_GameObject == null)
            return false;

        int i;

        //         //設定tag
        //         m_GameObject.tag = GameDefine.TAG_PLAYER;

        //移除剛體(角色物件不使用)
        Rigidbody[] rbs = m_GameObject.GetComponentsInChildren<Rigidbody>();
        for (i = 0; i < rbs.Length; ++i)
        {
            GameObject.DestroyImmediate(rbs[i]);
        }

        //找出ARPGBattle物件
        ARPGBattle compBattle = m_GameObject.AddComponent<ARPGBattle>();
        if (compBattle == null)
        {
            UnityDebugger.Debugger.LogError("角色樣版缺少 ARPGBattle!!");
            return false;
        }
        //開啟autoAttack
		ENUM_AUTO_ATTACK saveAuto = ARPGApplication.instance.m_MainPlayerAuto;
        compBattle.AutoAttack = ENUM_AUTO_ATTACK.ENUM_AA_OPEN;
		ARPGApplication.instance.m_MainPlayerAuto = saveAuto;
        //設定遠程攻擊特效
        compBattle.strLineEffect = GetWeapon().MissileEffect;
        //開啟跳血特效
        compBattle.isPlayHurtEffect = false;

        //加掛ARPGAnimation
        PlayForCreateRole compAnimation = m_GameObject.AddComponent<PlayForCreateRole>();
        compAnimation.vocation = iVocation;
        //檢查目前武器
        S_AnimationSetting_Tmp asDB = null;
        switch (GetWeapon().emWeaponType)
        {
            case ENUM_WeaponType.ENUM_WeaponType_Sword:
            asDB = GameDataDB.AnimationSettingDB.GetData((int)ARPGAnimation.AnimDiff.AnimDiff_Sword);
            break;
            case ENUM_WeaponType.ENUM_WeaponType_Pike:
            asDB = GameDataDB.AnimationSettingDB.GetData((int)ARPGAnimation.AnimDiff.AnimDiff_Pike);
            break;
            case ENUM_WeaponType.ENUM_WeaponType_Bow:
            asDB = GameDataDB.AnimationSettingDB.GetData((int)ARPGAnimation.AnimDiff.AnimDiff_Bow);
            break;
            case ENUM_WeaponType.ENUM_WeaponType_Staff:
            asDB = GameDataDB.AnimationSettingDB.GetData((int)ARPGAnimation.AnimDiff.AnimDiff_Empty);
            break;
            default:
            asDB = GameDataDB.AnimationSettingDB.GetData((int)ARPGAnimation.AnimDiff.AnimDiff_Empty);
            break;
        }
        //設定ARPGAnimation
        compAnimation.setAnimationData(asDB);
        compAnimation.isPrepare = false;
        if (jumpName != "")
        {
            compAnimation.Play(jumpName);
        }
        compAnimation.UpdateAnimation();


        //加掛ARPGMovement
        ARPGMovement compMovement = m_GameObject.AddComponent<ARPGMovement>();
        //設定相關數值資料
        S_PlayerData_Tmp PlayerDBF = GameDataDB.PlayerDB.GetData(iLv);
        compMovement.runSpeed = PlayerDBF.sAttrTable.fMoveSpeed;

        //加掛ARPGCharacter
        ARPGCharacter compCharacter = m_GameObject.AddComponent<ARPGCharacter>();

        //設定相關數值資料
        compCharacter.currentHp = 50000000;
        compCharacter.m_bUpdata_CalAttr = true;
		compCharacter.enabled = false;

        ////加掛AudioSource
        //AudioSource compAudioSource = m_GameObject.AddComponent<AudioSource>();

        //紀錄在ARPGBattle
        compBattle.CollectComponentInfo();
        return true;
    }

	/* PoAn - 2015/07/06 - 未跑此流程先註解
    public void SetComponent(GameObject go)
    {
        nowPlayer = go;
        Proc_CheckPlayerComponent(nowPlayer, 5, jumpName);
        switch (iVocation)
        {
            case ENUM_Vocation.ENUM_Vocation_1:
            nowPlayer.animation.CrossFade("94");	//待機動作
            break;
            case ENUM_Vocation.ENUM_Vocation_2:
            nowPlayer.animation.CrossFade("109");
            break;
            case ENUM_Vocation.ENUM_Vocation_3:
            nowPlayer.animation.CrossFade("124");
            break;
        }
    }
    */

    public S_Item_Tmp GetWeapon()
    {
        S_CreateCharacter_Tmp ccDBF = GameDataDB.CreateCharacterDB.GetData((int)iVocation + 1000);
        if (ccDBF == null)
        {
            UnityDebugger.Debugger.LogError("創角DBF錯誤!! iVocation=[" + iVocation.ToString() + "]");
            return null;
        }
        return GameDataDB.ItemDB.GetData(ccDBF.Weapon);
    }
}
