using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public enum ENUM_PVPOpera_Side
{
	ENUM_Left =0,		// 左邊
	ENUM_Right			// 右邊
}

public class UI_ValuePVPOpera : NGUIChildGUI  
{
	public UIButton		btnTemp = null;		
	public UILabel		lbtemp	= null;
    public UITexture    TextureBG = null;

    public int totalRound = 5;
    public float beginDelayTime = 0.0f;
    public float endDelayTime = 0.0f;

	// 參數
	public int          attackStringBegin = 201001; //攻擊字串起始值
	public int			attackStringMax   = 10;		//有效攻擊字串個數
	public int			resultStringBegin = 201501; //結果字串起始值
	public int			resultStringMax   = 10;		//有效結果字串個數

	public List<string> motionName		  = new List<string>(); //動作名稱
	public string		nameColor		  = "[0000C6]";			//訊息列角色名稱顏色
	public float		actionWaitTime	  = 0.5f;

	// 元件
	public UITextList		textList 	= null;
//	List<string>			history 	= new List<string>();
	public List<UIWidget>   playerBase 	= new List<UIWidget>(); //角色基底 BaseLeft BaseRight 
	public List<UISlider>	hpBar 		= new List<UISlider>(); //血條 ProgressBarLeft ProgressBarRight 

	public List<UIWidget>   iconBase	= new List<UIWidget>();	//頭像基底 IconBaseLeft IconBaseRight 
	public List<UISprite>	playerIcon 	= new List<UISprite>();	//玩家頭像 SpritePlayerLeft SpritePlayerRight 
	public List<UISprite>	pet1Icon 	= new List<UISprite>();	//pet1頭像 SpritePet1Left SpritePet1Right
	public List<UISprite>	pet2Icon 	= new List<UISprite>();	//pet2頭像 SpritePet2Left Spritepet2Right 
	public List<UILabel>	playerName 	= new List<UILabel>();	//玩家名稱 LabelPlayerNameLeft LabelPlayerNameRight
	public List<UISprite>	hurtEffect	= new List<UISprite>();	//傷害特效
	public List<TweenAlpha> twAlpha		= new List<TweenAlpha>();

	public Animation		effAction 	= null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_ValuePVPOpera";

	public UIButton  test =null;
	//-------------------------------------------------------------------------------------------------
	private UI_ValuePVPOpera() : base(GUI_SMARTOBJECT_NAME)
	{		
	}

	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		base.Initialize();
		textList.Clear();
//		UnityDebugger.Debugger.Log("+++++UI_ValuePVPOpera Initialize");
	}

	//-------------------------------------------------------------------------------------------------
	public void SetDisplayMsg(string attname, string defname)
	{
		if(string.IsNullOrEmpty(attname) || string.IsNullOrEmpty(defname))
		{
			UnityDebugger.Debugger.LogError("SetDisplayMsg attname || defname null");
		}
		else
		{
			string str;

			attname = string.Format("{0}{1}{2}", nameColor, attname, "[-]");
			defname = string.Format("{0}{1}{2}", nameColor, defname, "[-]");
			str = string.Format("{0} {1} {2} {3}", 
			                    attname, 
			                    GameDataDB.GetString(attackStringBegin+UnityEngine.Random.Range(0,attackStringMax-1)),
			                    defname,
			                    GameDataDB.GetString(resultStringBegin+UnityEngine.Random.Range(0,resultStringMax-1))
			                    );
			textList.Add(str);
		}
	}

	//---------------------------------------------------------------------------------------------------
/*	public void AddPlayer(ValuePVPOpponentValue val)
	{
		playerList.Add(val);
	}

	//-------------------------------------------------------------------------------------------------
	public ValuePVPOpponentValue GetPlayer(int index)
	{
		return playerList[index];
	}
*/
	//-------------------------------------------------------------------------------------------------
	public void InitialPlayer(int side, ValuePVPOpponentValue playerdata)
	{
		//
		hpBar[side].value = 1;
		//
// 		Utility.ChangeAtlasSprite(playerIcon[side], playerdata.Face);
// 		//
// 		SetPetIcon(pet1Icon[side],  playerdata.PetDBID1);
// 		SetPetIcon(pet2Icon[side],  playerdata.PetDBID2);
		//
		playerName[side].text = playerdata.RoleName;

		UnityDebugger.Debugger.Log("+++++++++++InitialPlayer()");
	}

	//-------------------------------------------------------------------------------------------------
	void SetPetIcon(UISprite sprite, int id)
	{
		S_PetData_Tmp petDBF = GameDataDB.PetDB.GetData(id);
		if(petDBF != null)	
		{
			Utility.ChangeAtlasSprite(sprite, petDBF.AvatarIcon);
		}
		else
		{
			sprite.gameObject.SetActive(false);
//			UnityDebugger.Debugger.LogError(string.Format("+++datapvp set pet icon error ID: {0}", id));
			UnityDebugger.Debugger.Log(string.Format("+++datapvp set pet icon error ID: {0}", id));
		}

	}
	//-------------------------------------------------------------------------------------------------
	public void SetHPBar(int side, int val)
	{
		hpBar[side].value -= (float)val/100;

		UnityDebugger.Debugger.Log(string.Format("side {0} hert {1} now {2}", side, (float)val/100, hpBar[side].value));
	}

	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
}