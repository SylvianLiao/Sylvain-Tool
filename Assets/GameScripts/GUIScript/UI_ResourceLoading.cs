using System;
using UnityEngine;
using System.Collections;
using GameFramework;

public class UI_ResourceLoading : NGUIChildGUI
{
	public UISprite	spriteProgressBar	= null;	//LoadingBar
    public UILabel Label_NowCount;
    public UILabel Label_LimitCount;
    public UILabel Label_NowSpeed;
    public UILabel Label_Progress;
	public UILabel Label_Message;
	public GameObject LoadingProgress;
	// smartObjectName
    private const string GUI_SMARTOBJECT_NAME = "UI_ResourceLoading";

    private UI_ResourceLoading()
        : base(GUI_SMARTOBJECT_NAME)
	{	
	}

	public void ShowLoadingProgress()
	{
		LoadingProgress.SetActive(true);
		Label_Message.gameObject.SetActive(false);
	}

	public void ShowMessage(string Message)
	{
		LoadingProgress.SetActive(false);
        Label_Message.gameObject.SetActive(true);
		Label_Message.text = Message;
	}
	//-----------------------------------------------------------------------------------------------------

	//-----------------------------------------------------------------------------------------------------

	//-----------------------------------------------------------------------------------------------------

}

