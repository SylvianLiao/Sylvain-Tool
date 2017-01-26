using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;



public class UI_SplashImage : NGUIChildGUI
{
    public UITexture TextureLogo = null;

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_SplashImage";

	//-----------------------------------------------------------------------------------------------------
	private UI_SplashImage() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
    public override void Initialize()
    {
        base.Initialize();
    }

    public IEnumerator PlayLogo(string[] LogoList)
    {
        int i = 0;
        while (LogoList.Length > 0)
        {
            TextureLogo.mainTexture = Resources.Load("Logo/" + LogoList[i]) as Texture;
            Show();
            yield return new WaitForSeconds(1.5f);
            i++;
            if (i >= LogoList.Length)
            {
                Hide();
                break;
            }
        }


    }
}

