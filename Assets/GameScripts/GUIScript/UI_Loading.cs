using System;
using UnityEngine;
using System.Collections;
using GameFramework;

class UI_Loading : NGUIChildGUI
{
	//
	public GameObject	ProgressBar			= null;	//進度條集合
	public UISprite		spriteProgressBar	= null;	//LoadingBar
	public UISprite 	spriteLoadingBG 	= null; //Loading背景圖片
	public UILabel		lbLoadingString 	= null; //Loading時所顯示字串
	//
	public GameObject 	LoadingProgress		= null; //讀取進度文字集合
	public UILabel 		Label_NowCount		= null;
	public UILabel 		Label_LimitCount	= null;
	public UILabel 		Label_NowSpeed		= null;
	public UILabel 		Label_Progress		= null;
	public UILabel 		Label_Message		= null;
	public UILabel		Lable_Warring		= null;
    public UILabel		Lable_Warring2		= null;
	//
	public UISprite	LOGO 				= null; //崑崙鏡字樣圖
	public UITexture	BG_Sait 			= null;	//賽特底圖
	public UITexture	BG_Nico 			= null;	//妮可底圖
	public UITexture	BG_DBFLoad			= null; //DBF載入底圖
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Loading";
	
	private UI_Loading(): base(GUI_SMARTOBJECT_NAME)
	{	
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		InitSpecialUI();
		spriteProgressBar.fillAmount = 0.0f;
		lbLoadingString.text = "";
		BG_Sait.gameObject.SetActive(false);
		BG_Nico.gameObject.SetActive(false);
		BG_DBFLoad.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	public void InitSpecialUI()
	{	
		//CMGE 特殊UI
#if UNITY_CMGE
		Lable_Warring2.text = GameDataDB.GetString(2190);   //防沉迷
#endif

		return;
	}
	//-----------------------------------------------------------------------------------------------------
	public void ShowLoadingProgress()
	{
		LoadingProgress.SetActive(true);
		ProgressBar.SetActive(true);
		Label_Message.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	public void ShowMessage(string Message)
	{
		LoadingProgress.SetActive(false);
		ProgressBar.SetActive(false);
		Label_Message.gameObject.SetActive(true);
		Label_Message.text = Message;
	}
	//-----------------------------------------------------------------------------------------------------
	//依狀態選擇顯示項目
	public void BeginChooseDisplay(GameState CurrentState)
	{
		string NowState = CurrentState.name;

		switch(NowState)
		{
			case GameDefine.LOADING_STATE:
				LoadingDisplay();
				break;
			case GameDefine.ASSETBUNDLEUPDATE_STATE:
				AssestDisplay();
				break;
			case GameDefine.DBFLOAD_STATE:
				DBFDisplay();
				break;
			case GameDefine.FIRSTASSETBUNDLEUPDATE_STATE:
				AssestDisplay();
				break;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//Loading
	private void LoadingDisplay()
	{
		ProgressBar.SetActive(true);
		Label_Message.text = "";
		Lable_Warring.text = "";

		LoadingProgress.SetActive(false);
		ChangeBG(BG_Nico.gameObject);
	}
	//-----------------------------------------------------------------------------------------------------
	//DBFLoading
	private void DBFDisplay()
	{
		ProgressBar.SetActive(true);
		Label_Message.text = GameDataDB.GetString(15052);   //如下載長時間沒有進度，請關閉遊戲程式並重新啟動
		Lable_Warring.text = "";

		LoadingProgress.SetActive(false);
		ChangeBG(BG_Sait.gameObject);
	}
	//-----------------------------------------------------------------------------------------------------
	//AssestLoading
	private void AssestDisplay()
	{
		ProgressBar.SetActive(true);
		LoadingProgress.SetActive(true);
		Lable_Warring.text = GameDataDB.GetString(15052);   //如下載長時間沒有進度，請關閉遊戲程式並重新啟動
		Label_Message.text = "";

		ChangeBG(BG_Sait.gameObject);
	}
	//-----------------------------------------------------------------------------------------------------
	//更換背景圖2選1
	private void ChangeBG(GameObject go)
	{
		if(go == BG_Sait.gameObject)
		{
			BG_Sait.gameObject.SetActive(!BG_DBFLoad.gameObject.activeSelf && true);
			BG_Nico.gameObject.SetActive(false);
			LOGO.gameObject.SetActive(false);
			lbLoadingString.gameObject.SetActive(false);
		}
		else
		{
			BG_Nico.gameObject.SetActive(!BG_DBFLoad.gameObject.activeSelf && true);
			BG_Sait.gameObject.SetActive(false);
			LOGO.gameObject.SetActive(true);
			lbLoadingString.gameObject.SetActive(true);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetDBFLoadingBG(int BGID)
	{
		if(BGID>0)
		{
			BG_DBFLoad.gameObject.SetActive(true);
			Utility.ChangeTexture(BG_DBFLoad,BGID);
		}
		else
		{
			BG_DBFLoad.gameObject.SetActive(false);
		}
	}
	//-----------------------------------------------------------------------------------------------------
}

