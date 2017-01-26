using System;
using UnityEngine;
using System.Collections;
using GameFramework;

class UI_Login : NGUIChildGUI
{
	#region 帳密長度設定
	private const int MIN_NAME_LENGHT = 2;
	private const int MAX_NAME_LENGHT = 20;
	private const int MIN_PASSWORD_LENGHT = 2;
	private const int MAX_PASSWORD_LENGHT = 20;
	
	private const string ACCOUNT_PASSWORD_PATTERN = "^[0-9a-zA-Z_]*$";
	#endregion

	public UIButton	BtnOtherLogin		= null;
	public UIButton	BtnSpeedLogin		= null;
    public UILabel	lbCreateRole		= null;
	public UIInput	InputAccount		= null;
	public UIInput	InputPW				= null;
	public UIToggle	ToggleCreateRole	= null;
    public UILabel  LabelVersion    	= null;

	public UILabel	lbServerListTitle	= null;
	public UIButton ButtonServerList 	= null;
	public UILabel  LabelServerList		= null;
	public UISprite	SpriteBusyR			= null;

	public UIButton ButtonController	= null;
	public UILabel  LabelController		= null;
	public Transform Mask;
    public UILabel	Lable_Warring		= null;
	public UILabel	Lable_Message		= null;

	//
	public float	saveDoubleClick	= 0;		//鎖連點機制
				
	public DateTime slClick;					//sl鎖連點機制
	public TimeSpan ts;

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Login";
	private Coroutine lastCoroutine;
	//-----------------------------------------------------------------------------------------------------
	private UI_Login() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	public void Start()
	{
		lbServerListTitle.text = GameDataDB.GetString(993);		//伺服器
		lbCreateRole.text = GameDataDB.GetString(15072);	//強制創角

		BtnSpeedLogin.enabled = true;

		//更新Toggle
		ToggleCreateRole.value = ARPGApplication.instance.m_EnforceCreateRole;

		ButtonController.gameObject.SetActive(false);

		if (ARPGApplication.instance.m_DebugVersion == false)
		{
			ToggleCreateRole.gameObject.SetActive(false);
		}
#if UNITY_CMGE
		if (ARPGApplication.instance.m_ChannelSDKSystem.CheckSpecialUI(ENUM_CHANNEL_SPECIALUI.UserManager))
			ButtonController.gameObject.SetActive(true);

		LabelController.text = GameDataDB.GetString(15074);	//用戶中心
        Lable_Warring.text = GameDataDB.GetString(2190);   //防沉迷
#endif
		Lable_Message.text = GameDataDB.GetString(15082);
	}
	//-----------------------------------------------------------------------------------------------------
	public void Update()
	{
		//鎖連點機制
		if(saveDoubleClick > 0.0f)
		{
			if(BtnSpeedLogin.enabled == true)
				BtnSpeedLogin.enabled = false;

			saveDoubleClick -= Time.deltaTime;

			if(saveDoubleClick <= 0.0f)
			{
				BtnSpeedLogin.enabled = true;
				saveDoubleClick = 0.0f;
			}
		}

		ts = DateTime.Now - slClick;

	}
	//-----------------------------------------------------------------------------------------------------
	public bool IsCheckInput()
	{
		//LoginBtn判斷可否按下
		if(InputPW.value.Length>MIN_PASSWORD_LENGHT && InputAccount.value.Length>MIN_NAME_LENGHT)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	public override void Show ()
	{
		base.Show ();
		MaskDisable();
	}

	public void MaskEnable()
	{
		Mask.gameObject.SetActive(true);
		UnityDebugger.Debugger.Log("MaskEnable");
		if (null != lastCoroutine)
			StopCoroutine(lastCoroutine);
		lastCoroutine = StartCoroutine (DelayMaskDisable (60.0f));
	}

	public void MaskDisable()
	{
		Mask.gameObject.SetActive(false);
	}

	IEnumerator DelayMaskDisable (float s)
	{
		yield return new WaitForSeconds(s);
		MaskDisable();
		lastCoroutine = null;
	}
}

