using System.Collections;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;

class UI_SummonPlay : NGUIChildGUI
{
	public bool				checkEndPlay = true;	//測試用，確認是否進入結束流程
	//
	public UIButton			btnSpeedShow = null;	//快速撥放按鈕
    public UIButton         btnPass = null;			//略過按鈕
    public UISprite         spriteCard = null;      //抽出的卡片卡面
    public UILabel          labelCardName = null;	//抽出的卡片的名稱
    public UISprite         spriteCardBG = null;	//抽出的卡片的邊框
    public List<UISprite>   spriteStar = null;	    //抽出的卡片星等
    public UITexture        summonAnim = null;      //
	//
	public UILabel			lbGuide = null;	//導引說明文字
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_SummonPlay";

	//-----------------------------------------------------------------------------------------------------
	private UI_SummonPlay() : base(GUI_SMARTOBJECT_NAME)
	{
	}
    public void SetCard(int DBID)
    {
        spriteCard.gameObject.SetActive(true);
        UnityDebugger.Debugger.Log("DBID:" + DBID);
        S_PetData_Tmp PetDBF = GameDataDB.PetDB.GetData(DBID); // 找出相對應的DBF
        labelCardName.text 	= GameDataDB.GetString(PetDBF.iName); //設定名稱
		Utility.ChangeAtlasSprite(spriteCard,PetDBF.Texture); //設定顯示2D圖

        //先初始化把星等都隱藏起來
        for (int i = 0; i < spriteStar.Count; ++i)
        {
            spriteStar[i].gameObject.SetActive(false);
        }
        //設定顯示星級
        for (int i = 0; i < PetDBF.iRank; ++i)
        {
            spriteStar[i].gameObject.SetActive(true);
        } //end for

        //選擇使用哪個資訊框
        if (PetDBF.iRank > 0 && PetDBF.iRank < 4)
            Utility.ChangeAtlasSprite(spriteCardBG, 1010);	//夥伴普卡1~3星用
        if (PetDBF.iRank > 3 && PetDBF.iRank < 6)
            Utility.ChangeAtlasSprite(spriteCardBG, 1009);	//夥伴銀卡4~5星用
        if (PetDBF.iRank > 5 && PetDBF.iRank < 7)
            Utility.ChangeAtlasSprite(spriteCardBG, 1008);	//夥伴金卡6~7星用
        if (PetDBF.iRank < 0 || PetDBF.iRank > 7)
            return;												//非正常狀況時
    }

    public void SetPetInfo(int iPetDBFID)
    {
        if (iPetDBFID == 0)
            return;
        //載入PetDBF資訊跟SkillDBF資訊
        S_PetData_Tmp PetDBF = GameDataDB.PetDB.GetData(iPetDBFID);

        Utility.ChangeAtlasSprite(spriteCard, PetDBF.FullAvatar);
        spriteCard.MakePixelPerfect();		//自動調整圖的比例(Base在Height為1024上)
        //spriteRoleFullyIcon.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
/*        if (Card.height > 720)*/
            spriteCard.SetDimensions((int)(spriteCard.aspectRatio * 240), 240);
            spriteCard.gameObject.SetActive(true);
    }
}