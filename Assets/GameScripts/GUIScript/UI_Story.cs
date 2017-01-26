using System.Collections;
using UnityEngine;
using GameFramework;

public enum ENUM_STORY_ROLE_SPRITE
{
	ENUM_SRS_Left	= 0,
	ENUM_SRS_Middle	= 1,
	ENUM_SRS_Right	= 2,
}

class UI_Story : NGUIChildGUI
{
	//
	public UITexture	m_textureBackground			= null;
	public UIPanel		m_panelContent				= null;
	public UILabel		m_lbName					= null;
	public UILabel		m_lbContent					= null;
	public UISprite		m_spriteTarget				= null;
	//
	public UISprite		m_spriteContentBackground	= null;	//對話框的框
	public UITexture	m_textureRoleLeft			= null;
	public UITexture	m_textureRoleMiddle			= null;
	public UITexture	m_textureRoleRight			= null;
	public UISprite		m_spriteName				= null;	//名稱的框

    public UIButton     m_btnSkip                   = null; //跳過劇情對話內容
    public UILabel      m_lbSkip                    = null; //跳過劇情
	//
	public int			m_charsPerSecond	= 10;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Story";
	private int			m_NextGUID;							//下一筆劇情GUID
	private bool		m_isFinishEnd;						//是否已結束目前劇情流程
	private S_Dialogue_Tmp	currentDBF				= null;	//目前處理的劇情DBF
	private int			currentBackgroundGUID		= 0;

	public bool			m_showBGFlag 				= false;	//首次獲得寵物的劇情要顯示背景
	//
	private const string me							= "[me]";
	private const string contentme					= "[playername]";
	//private S_SpriteCoordinate_Tmp	m_spriteCoordinate; //接收圖的座標資訊並指派給圖	

	public UIWidget		m_petgetinContainer			= null;	//首次獲得寵物專用背景

	//提供外部資訊
	public int			getNowGUID	{get{return (currentDBF!=null ? currentDBF.GetGUID() : -1);}	}
	public int			getNextGUID	{get{return m_NextGUID;		}									}
	public bool			getFinishEnd{get{return m_isFinishEnd;	}									}

	public float musicVolume = 0.0f;

	//-----------------------------------------------------------------------------------------------------
	private UI_Story() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-----------------------------------------------------------------------------------------------------
	public void Update()
	{
		if(m_isFinishEnd==false				&&
		   isFinishTypeWriteEffect()==true	)
			m_isFinishEnd = true;

		m_spriteTarget.gameObject.SetActive(m_isFinishEnd);
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();

		//清空
		Utility.ChangeTexture(m_textureBackground,		0);
		Utility.ChangeTexture(m_textureRoleLeft,		0);
		Utility.ChangeTexture(m_textureRoleMiddle,		0);
		Utility.ChangeTexture(m_textureRoleRight,		0);
		//
		m_lbName.text		= "";
		m_lbContent.text	= "";
		//
		m_NextGUID			= -1;
		m_isFinishEnd		= true;

		m_textureRoleLeft.gameObject.SetActive(false);
		m_textureRoleMiddle.gameObject.SetActive(false);
		m_textureRoleRight.gameObject.SetActive(false);
		m_panelContent.gameObject.SetActive(false);


	}
	//-----------------------------------------------------------------------------------------------------
	public override void Show()
	{	
		base.Show();
        m_lbSkip.text = GameDataDB.GetString(125);	//"跳過對話"
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	//開啟劇情系統，請呼叫這個
	public void BeginStory(int iGUID)
	{
		Initialize();

		//關閉劇情系統
		if(iGUID <= 0)
			return;

		Show();

		m_NextGUID = iGUID;
		m_NextGUID = LoadStoryDBF();
	}
	//-----------------------------------------------------------------------------------------------------
	//關閉劇情系統，請呼叫這個
	public void CloseStory()
	{
		Initialize();
		Hide();
	}
	//-----------------------------------------------------------------------------------------------------
	//手動呼叫下一筆劇情資料
	public void ManualLoadNextStory()
	{
		UnityDebugger.Debugger.Log("手動呼叫劇情資料!!");

		if(StoryEffectBase.bCheckExeScript(this.gameObject) == false)
		{
			StartCoroutine( LoadNextStory() );
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//執行下一筆劇情資料
	public IEnumerator LoadNextStory()
	{
		while(m_isFinishEnd == false)
			yield return null;

		m_NextGUID = LoadStoryDBF();
	}
	//-----------------------------------------------------------------------------------------------------
	private int LoadStoryDBF()
	{
		UnityDebugger.Debugger.Log("目前讀取劇情GUID=" + m_NextGUID.ToString());

		//停止音效
		MusicControlSystem.StopAllLoopSound();

		//讀取劇情DBF
		currentDBF = GameDataDB.DialogueDB.GetData(m_NextGUID);
		if(currentDBF == null)
		{
			if(m_NextGUID > 0)
				UnityDebugger.Debugger.LogError("storyDBF[" + m_NextGUID.ToString() + "] Error!!");

			return -1;
		}

		//開始流程
		m_isFinishEnd = false;

		//動畫流程
		if(currentDBF.GetGUID() == 114600)
			ProcessMovieStory();
		//劇情對話流程
		else
			ProcessRoleStory();

		return currentDBF.iNextDialogID;
	}
	//-----------------------------------------------------------------------------------------------------
	//處理BossStory流程(目前不使用)
	private void ProecessBossStory()
	{
		//預設先隱藏所有圖
		InitDisableAllSprite();

		//移除舊文字特效
		TypeEffect[] wes = GetComponentsInChildren<TypeEffect>();
		foreach(TypeEffect we in wes)
		{
			DestroyImmediate(we);
		}

		//設定背景
		if(m_textureBackground != null)
		{
			if (currentDBF.BackGround != currentBackgroundGUID)
				Utility.ChangeTexture(m_textureBackground, currentDBF.BackGround);
			currentBackgroundGUID = currentDBF.BackGround;
			//首次獲得寵物的劇情要顯示背景
			if(m_showBGFlag)
			{
				m_textureBackground.alpha = 0.01f;
				//顯示參戰
				m_petgetinContainer.gameObject.SetActive(true);
			}
			else
			{
				m_textureBackground.alpha = 0.4f;
				m_petgetinContainer.gameObject.SetActive(false);
			}
		}

		//設定中圖
		if(m_textureRoleMiddle != null)
		{
			int MiddleRoleSprite = currentDBF.MiddleSprite;
			if(MiddleRoleSprite == (int)ENUM_SpriteRole.ENUM_SpriteRole_MainRole) //如果中圖為主角
			{	
//				MiddleRoleSprite = SetToMainRoleSprite();
				MiddleRoleSprite = ARPGApplication.instance.m_RoleSystem.GetRoleTexture();
			}

			Utility.ChangeTexture(m_textureRoleMiddle, MiddleRoleSprite);
            //角色圖不做MakePixelPerfect
			//m_textureRoleMiddle.MakePixelPerfect ();//自動調整圖的比例(Base在Height為1024上)
            //m_spriteCoordinate = GameDataDB.SpriteCoordinate.GetData (MiddleRoleSprite);
            //m_textureRoleMiddle.transform.localPosition = new Vector3(m_spriteCoordinate.MiddleSpriteX,m_spriteCoordinate.MiddleSpriteY);
		}//endif設定中圖

		m_textureRoleMiddle.gameObject.SetActive(true);

		StartCoroutine( ProcessStory() );
	}
	//-----------------------------------------------------------------------------------------------------
	//處理角色Story流程
	private void ProcessRoleStory()
	{
		//預設先隱藏所有圖
		InitDisableAllSprite();

		//移除舊文字特效
		TypeEffect[] wes = GetComponentsInChildren<TypeEffect>();
		foreach(TypeEffect we in wes)
		{
			DestroyImmediate(we);
		}

		//設定背景
		if(m_textureBackground != null)
		{
			if (currentDBF.BackGround != currentBackgroundGUID)
				Utility.ChangeTexture(m_textureBackground, currentDBF.BackGround);

			currentBackgroundGUID = currentDBF.BackGround;
			//首次獲得寵物的劇情要顯示背景
			if(m_showBGFlag)
			{
				m_textureBackground.alpha = 1.0f;
				//不顯示參戰
				m_petgetinContainer.gameObject.SetActive(false);
				MusicControlSystem.PlaySound("Sound_System_019", 1);
				// 淡出原背景音樂
				MusicControlSystem.Fade_StopBackgroundMusic(0.0f);
			}
			else
			{
				m_textureBackground.alpha = 0.4f;
				//不顯示參戰
				m_petgetinContainer.gameObject.SetActive(false);
			}
		}

		//設定圖
		setRoleSprite(ENUM_STORY_ROLE_SPRITE.ENUM_SRS_Left);
		setRoleSprite(ENUM_STORY_ROLE_SPRITE.ENUM_SRS_Middle);
		setRoleSprite(ENUM_STORY_ROLE_SPRITE.ENUM_SRS_Right);
		//
		ARPGApplication.instance.SetAndPlayOSSpeech(currentDBF.iOSID);

		StartCoroutine( ProcessStory() );
	}
	//-----------------------------------------------------------------------------------------------------
	//初始關閉sprite
	private void InitDisableAllSprite()
	{
		m_textureRoleMiddle.gameObject.SetActive(false);
		m_textureRoleLeft.gameObject.SetActive(false);
		m_textureRoleRight.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	//設定各sprite 
	private void setRoleSprite(ENUM_STORY_ROLE_SPRITE srs)
	{
		int			roleSpriteID		= -1;	//圖編號
		UITexture	roleSpriteTexture	= null;	//圖物件
		float		roleSpriteX			= 0.0f;	//圖座標
		float		roleSpriteY			= 0.0f;	//圖座標

		switch(srs)
		{
		case ENUM_STORY_ROLE_SPRITE.ENUM_SRS_Left:
			roleSpriteID		= currentDBF.LeftSprite;
			roleSpriteTexture	= m_textureRoleLeft;
			break;
		case ENUM_STORY_ROLE_SPRITE.ENUM_SRS_Middle:
			roleSpriteID		= currentDBF.MiddleSprite;
			roleSpriteTexture	= m_textureRoleMiddle;
			break;
		case ENUM_STORY_ROLE_SPRITE.ENUM_SRS_Right:
			roleSpriteID		= currentDBF.RightSprite;
			roleSpriteTexture	= m_textureRoleRight;
			break;
		}

		if(roleSpriteID < 0)
			return;

		//設定為主角圖
		if(roleSpriteID == (int)ENUM_SpriteRole.ENUM_SpriteRole_MainRole)
			roleSpriteID = ARPGApplication.instance.m_RoleSystem.GetRoleTexture();		       
		SetTexture(roleSpriteTexture, roleSpriteID);
		roleSpriteTexture.gameObject.SetActive(true);
	}
	//-----------------------------------------------------------------------------------------------------
	//設定主角色圖
	private int SetToMainRoleSprite()
	{
		//所需參數宣告
		ENUM_Vocation career; // 職業
		ENUM_SexType sextype; // 性別
		//取得主玩家顯像資料      
		S_BaseRoleData pRole = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData;	//基本角色資料
		career 	= pRole.emVocation;
		sextype = pRole.emSex;

		int returnValue = -1;

		if(sextype == ENUM_SexType.ENUM_SexType_Man)
		{
			switch(career)
			{
			case ENUM_Vocation.ENUM_Vocation_1: //男武者
				returnValue = 1114;
				break;
			case ENUM_Vocation.ENUM_Vocation_2: //男劍俠
				returnValue = 1115;
				break;
			case ENUM_Vocation.ENUM_Vocation_3: //男獵魔
				returnValue = 1116;
				break;
			}
		}
		else if(sextype == ENUM_SexType.ENUM_SexType_Woman)
		{
			switch(career)
			{
			case ENUM_Vocation.ENUM_Vocation_1: //女武者
				returnValue = 1112;
				break;
			case ENUM_Vocation.ENUM_Vocation_2: //女劍俠
				returnValue = 1111;
				break;
			case ENUM_Vocation.ENUM_Vocation_3: //女獵魔
				returnValue = 1113;
				break;
			}
		}
		//上面都不成立的話
		return returnValue;//空圖
	}
	//-----------------------------------------------------------------------------------------------------
	//剩餘Story處理
	private IEnumerator ProcessStory()
	{
		//等待效果結束
		while(StoryEffectBase.bCheckExeScript(m_textureRoleMiddle.gameObject) == true)
			yield return null;

		//設定框型
		switch(currentDBF.iFrame)
		{
		case ENUM_DialogFrame.ENUM_DialogFrame_Normal:
			Utility.ChangeAtlasSprite(m_spriteContentBackground, 1000);
			//對話名稱框
			Utility.ChangeAtlasSprite(m_spriteName, 1129);
			break;
		case ENUM_DialogFrame.ENUM_DialogFrame_Boss:
			Utility.ChangeAtlasSprite(m_spriteContentBackground, 1000);
			//對話名稱框
			Utility.ChangeAtlasSprite(m_spriteName, 1130);
			break;
		case ENUM_DialogFrame.ENUM_DialogFrame_Max:
			break;
		}
		
		//對話人名稱
		m_lbName.text = SetToSelfRoleName();

		TypeEffect new_we = m_lbContent.gameObject.AddComponent<TypeEffect>();
		new_we.charsPerSecond = m_charsPerSecond;
		//對話內容
		new_we.text = JudgeRoleNameToReplace();
		m_lbContent.text = "";

		m_panelContent.gameObject.SetActive(true);
	}
	//-----------------------------------------------------------------------------------------------------
	//對話人名稱設為自選角色名
	private string SetToSelfRoleName()
	{
		string TempGetName = GameDataDB.GetString( currentDBF.iDialogueName );
		if(TempGetName == me)
			TempGetName = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName;

		return TempGetName;
	}
	//-----------------------------------------------------------------------------------------------------
	//檢查對話內容是否有[玩家名稱]並加以取代
	private string JudgeRoleNameToReplace()
	{
		string tempContent 	= GameDataDB.GetString(currentDBF.iDialogueText); 
		string RoleName		= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName;

		return tempContent.Replace(contentme,RoleName);
	}
	//-----------------------------------------------------------------------------------------------------
	//設定Texture
	private void SetTexture(UITexture t, int id)
	{
		Shader shaderData = null;

		//灰階處理
		if(currentDBF.PicGray==(int)ENUM_StoryGray_Type.ENUM_SGT_Left && t==m_textureRoleLeft)			//左
			shaderData = Shader.Find("Unlit/EOA4 Transparent Colored");
		else if(currentDBF.PicGray==(int)ENUM_StoryGray_Type.ENUM_SGT_Right && t==m_textureRoleRight)	//右
			shaderData = Shader.Find("Unlit/EOA4 Transparent Colored");
		else if(currentDBF.PicGray==(int)ENUM_StoryGray_Type.ENUM_SGT_Center && t==m_textureRoleMiddle)	//中
			shaderData = Shader.Find("Unlit/EOA4 Transparent Colored");
		else if(currentDBF.PicGray == (int)ENUM_StoryGray_Type.ENUM_SGT_All)							//全
			shaderData = Shader.Find("Unlit/EOA4 Transparent Colored");
		Utility.ChangeTexture(t, id, shaderData);

		t.depth = 0;
		if(currentDBF.Brighten==(int)ENUM_StoryPic_Type.ENUM_SPT_LeftLight && t==m_textureRoleLeft)			//左
		{
			t.color	= Color.white;
			t.depth	+= 1;
		}	
		else if(currentDBF.Brighten==(int)ENUM_StoryPic_Type.ENUM_SPT_RightLight && t==m_textureRoleRight)	//右
		{
			t.color	= Color.white;
			t.depth	+= 1;
		}
		else if(currentDBF.Brighten==(int)ENUM_StoryPic_Type.ENUM_SPT_CenterLight && t==m_textureRoleMiddle)//中
		{
			t.color	= Color.white;
			t.depth	+= 1;
		}
		else if(currentDBF.Brighten == (int)ENUM_StoryPic_Type.ENUM_SPT_AllLight)							//全亮
		{
			t.color = Color.white;
		}
		else
		{
			t.color = new Color(0.4f, 0.4f, 0.4f);
		}

		if(shaderData != null)
			t.color = new Color(0.0f, t.color.g, t.color.b);
	}
	//-----------------------------------------------------------------------------------------------------
	//是否已結束對話文字特效
	public bool isFinishTypeWriteEffect()
	{
		TypeEffect[] wes = gameObject.GetComponentsInChildren<TypeEffect>();
		if(wes.Length > 0)
			return false;

		return true;
	}
	//-----------------------------------------------------------------------------------------------------
	//加快對話文字特效
	public void speedUpTypeWriteEffect()
	{
		TypeEffect[] wes = gameObject.GetComponentsInChildren<TypeEffect>();
		foreach(TypeEffect we in wes)
		{
			we.charsPerSecond = 999;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//處理動畫流程
	private void ProcessMovieStory()
	{
		StoryState ss = ARPGApplication.instance.GetGameStateByName(GameDefine.STORY_STATE) as StoryState;

		UnityDebugger.Debugger.Log("Story Movie is playing!!");

		ss.suspend();

		//Fade
		ARPGApplication.instance.StartToFade(null, 0.0f, 1.0f, 2.0f, ss.PreparePlayMovie);
	}
}