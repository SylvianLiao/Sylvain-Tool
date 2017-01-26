using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

class UI_DataPVPResult : NGUIChildGUI
{
	public UIButton 	btnLeaveBattle			= null;	//離開按鈕
	public UILabel		lbLeaveBattle			= null;	//離開字樣
	//戰勝結算相關
	public UIPanel		panelWinState 			= null;	//戰勝畫面顯示
	//
	public GameObject		Treasure				= null; //寶箱Prefab
	public TreasureInfo[]	TreasureInfos			= null; //複制的寶箱資訊
	public UIButton[]		btnTreasures			= null;	//寶箱陣列
    public UILabel          labelWinText            = null; //戰勝句子

	//戰敗結算相關
 	public UIPanel		panelLoseState 			= null;	//戰敗畫面顯示
// 	public UISprite		spriteLoseBG			= null; //戰敗背景
    public UILabel      labelLaugh              = null;
    public UIButton     buttonEnhance           = null;	//進入強化介面按鈕
    public UILabel      labelEnhance            = null;	//進入強化介面按鈕
    public UIButton     ButtonBlack             = null;	//結仇按鈕
    public UILabel      labelBlack              = null;	//結怨
    public UISprite     spriteIcon              = null; //對手頭像
    public UILabel      labelName               = null; //對手名字

	//
	public const int	iTreasureNum			= 3;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_DataPVPResult";
	
	//-----------------------------------------------------------------------------------------------------
    private UI_DataPVPResult()
        : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
    public override void Show()
	{
        base.Show();
		lbLeaveBattle.text = GameDataDB.GetString(1951);								//"離開"
		//btnLeaveBattle.isEnabled = false;
		//btnLeaveBattle.gameObject.SetActive(false);

		//TreasureHideOrShow(false);							//隱藏寶箱
	}
	//-----------------------------------------------------------------------------------------------------
	void Update()
	{
	}
	//-----------------------------------------------------------------------------------------------------
	//戰勝畫面
	public void Win(UIEventListener.VoidDelegate cb)
	{
        GenerateTreasureInfo();								//生成寶箱
		TreasureHideOrShow(true);
		//TreasureShockEffect();
		//將寶箱指派事件
		for(int i=0;i<btnTreasures.Length;++i)
		{
			UIEventListener.Get(btnTreasures[i].gameObject).onClick += cb;
		}
        labelWinText.text = GameDataDB.GetString(1971);
        btnLeaveBattle.isEnabled = false;

        panelWinState.gameObject.SetActive(true);
        panelLoseState.gameObject.SetActive(false);
		btnLeaveBattle.gameObject.SetActive(true);

	}
    //-----------------------------------------------------------------------------------------------------
    //戰勝畫面
    public void Lose(int id,string name)
    {
        int rand = UnityEngine.Random.Range(0, 15); 
        labelLaugh.text = GameDataDB.GetString(1952 + rand);								//"嘲諷"
        labelEnhance.text = GameDataDB.GetString(1950);
        labelBlack.text = GameDataDB.GetString(1967);
       //UIEventListener.Get(btnTreasures[i].gameObject).onClick += OpenChestBoxEvent;
        panelWinState.gameObject.SetActive(false);
        panelLoseState.gameObject.SetActive(true);
        btnLeaveBattle.gameObject.SetActive(true);
        setOpponent(id,name);

		//目前沒功能先隱藏的按鈕
		buttonEnhance.gameObject.SetActive(false);
		ButtonBlack.gameObject.SetActive(false);
    }
	//-----------------------------------------------------------------------------------------------------
	//生成寶箱
	private void GenerateTreasureInfo()
	{
		if(Treasure!=null)
		{
			TreasureInfos = new TreasureInfo[iTreasureNum];
			btnTreasures = new UIButton[iTreasureNum];
			
			//動態產生DungeonInfo物件
			
			for(int i=0; i<TreasureInfos.Length; ++i)
			{
				TreasureInfos[i] = new TreasureInfo();
				GameObject newGO = Instantiate(Treasure) as GameObject;
				newGO.transform.parent = Treasure.transform.parent;
				
				//調整位置
				newGO.transform.localPosition = new Vector3(Treasure.transform.localPosition.x + 370.0f*i,
				                                            Treasure.transform.localPosition.y,
				                                            Treasure.transform.localPosition.z				);
				newGO.transform.rotation = Treasure.transform.rotation;
				newGO.transform.localScale = Treasure.transform.localScale;
				
				TreasureInfos[i] = newGO.GetComponent<TreasureInfo>();
				TreasureInfos[i].btnTreasureBox.userData = i;
				btnTreasures[i] = TreasureInfos[i].btnTreasureBox;
			}
			//隱藏原始模型
			Treasure.gameObject.SetActive(false);
		}
		/*if(TreasureInfos==null && panelTreasure!=null)
		{
			TreasureInfos = new TreasureInfo[iTreasureNum];
			btnTreasures = new UIButton[iTreasureNum];
			
			//動態產生DungeonInfo物件
			int i;
			for(i=0; i<TreasureInfos.Length; ++i)
			{
				TreasureInfos[i] = new TreasureInfo();
				GameObject newGO = Instantiate(panelTreasure.gameObject) as GameObject;
				newGO.transform.parent = panelTreasure.transform.parent;
				
				//調整位置
				newGO.transform.localPosition = new Vector3(panelTreasure.transform.localPosition.x + 450.0f*i,
				                                            panelTreasure.transform.localPosition.y,
				                                            panelTreasure.transform.localPosition.z				);
				newGO.transform.rotation = panelTreasure.transform.rotation;
				newGO.transform.localScale = panelTreasure.transform.localScale;
				
				TreasureInfos[i]= newGO.GetComponent<TreasureInfo>();
				TreasureInfos[i].btnTreasureBox.userData = i;
				btnTreasures[i] = TreasureInfos[i].btnTreasureBox;
				
				++i;
			}
			//隱藏原始模型
			panelTreasure.gameObject.SetActive(false);
		}*/
	}
	//-----------------------------------------------------------------------------------------------------
	//寶箱的顯示與隱藏
	private void TreasureHideOrShow(bool bsw)
	{
		if(TreasureInfos!=null)
		{
			for(int i=0;i<TreasureInfos.Length;++i)
				TreasureInfos[i].gameObject.SetActive(bsw);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//寶箱出現的效果
	/*private void TreasureShockEffect()
	{
		TweenScale ts;
		if(TreasureInfos!=null)
		{
			for(int i=0;i<TreasureInfos.Length;++i)
			{
				ts = TreasureInfos[i].btnTreasureBox.GetComponent<TweenScale>();
				ts.enabled = true;
				if(i==(iTreasureNum-1))
					ts.AddOnFinished(EnableClickFunction);
			}
		}
	}*/
	//-----------------------------------------------------------------------------------------------------
	private void EnableClickFunction()
	{
		if(TreasureInfos!=null)
		{
			for(int i=0;i<TreasureInfos.Length;++i)
				TreasureInfos[i].btnTreasureBox.GetComponent<BoxCollider>().enabled=true;
		}
	}
    public void setOpponent(int id,string name)
    {
        Utility.ChangeAtlasSprite(spriteIcon, id);
        labelName.text = name;
        
    }
}