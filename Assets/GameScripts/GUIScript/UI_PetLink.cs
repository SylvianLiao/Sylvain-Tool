using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_PetLink : NGUIChildGUI  
{
	public UIPanel		panelBase					= null;
	public UIPanel		panelScrollViewTeam			= null;
	public UIScrollView	ScrollViewTeam				= null;
	public UIGrid		GridTeam					= null;
	//
	public UISprite		spSkillNote					= null;
	public UILabel		lbSkillName					= null;
	public UIPanel		panelSkillContent			= null;
	public UIScrollView	SkillContent				= null;
	public UIWidget		LinkSkillContent			= null;
	public UILabel[]	lbLinkSkills				= new UILabel[3];
	public UILabel[]	lbConditions				= new UILabel[3];
	public UIButton		btnPrevious					= null;
	public UILabel		lbPrevious					= null;
	public UIButton		btnNext						= null;
	public UILabel		lbNext						= null;
	//
	public UIButton		btnPetLinkList				= null;
	public UIWidget		UISwitch					= null;
	public UIScrollView	scrollLinkList				= null;
	public UIGrid		GridList					= null;
	public UIButton		LinkPrefab					= null;
	//
	public GameObject	LinkPetPrefab				= null;
	[HideInInspector] public List<GameObject>		LinkPetTeam			= new List<GameObject>();
	[HideInInspector] public List<SetPetPicItemBehavior>	SetLinkPets	= new List<SetPetPicItemBehavior>();
	[HideInInspector] public List<UIButton>			btnPetRleases	= new List<UIButton>();
	//
	[HideInInspector] public List<UIButton>		btnLinkTeams		= new List<UIButton>();
	[HideInInspector] public List<UISprite>		spriteBorders		= new List<UISprite>();
    [System.NonSerialized]
    public List<S_PetLink_Tmp> PetLinkDatas = new List<S_PetLink_Tmp>();
	//
	[HideInInspector] public List<PetPicture>		PetPictureDatas	= new List<PetPicture>(); //紀錄按下哪一組的所有寵物PetPicture
	private List<PetPicture> AllPetDatas = new List<PetPicture>(); 
	[HideInInspector] public int				SelectTeamIndex = -1;
	private PetLinkSkillManager pkManager = new PetLinkSkillManager();
	//儲存圖鑑按鈕正常顏色
	private Color				BtnNormalColor	= new Color(0,0.51f,0.87f);

	//-------------------------------------新手教學用-------------------------------------
	public UIPanel					panelGuide							= null; //教學集合
	public UIButton					btnTopFullScreen					= null; //最上層的全螢幕按鈕
	public UIButton					btnFullScreen						= null; //全螢幕按鈕
	public UISprite					spGuideShowPet						= null; //導引介紹關係寵物
	public UILabel					lbGuideShowPet						= null;
	public UISprite					spGuideShowBonus					= null; //導引介紹關係加強內容
	public UILabel					lbGuideShowBonus					= null;
	public UISprite					spGuideAllLinkBtn					= null; //導引介紹關係索引按鈕
	public UILabel					lbGuideAllLinkBtn					= null;
	public UILabel					lbGuideFinish						= null;

	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "UI_PetLink";
	//-------------------------------------------------------------------------------------------------
	private UI_PetLink() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		//先clone出5個關係寵物
		CloneLinkPetGameobject();
		//再clone出關係按鈕
		CloneAllTeamBtns();
		//蒐集所有寵物資訊
		CollectAllPetDatas();
	}
	//-------------------------------------------------------------------------------
	void Start()
	{
		SkillContent.enabled = false;
		lbPrevious.text 	= GameDataDB.GetString(2712);		//"上一組"
		lbNext.text			= GameDataDB.GetString(2713);		//"下一組"

		//第一次進去
		//LoadTeamData(0);
	}
	//-------------------------------------------------------------------------------------------------
	public void CloneLinkPetGameobject()
	{
		LinkPetTeam.Clear();
		SetLinkPets.Clear();
		btnPetRleases.Clear();

		Transform t;
		//生成關係寵物物件
		for(int i=0;i<GameDefine.MAX_PET_LINKCOUNT;++i)
		{
			GameObject gb = NGUITools.AddChild(GridTeam.gameObject,LinkPetPrefab);
			//
			SetPetPicItemBehavior setUI = gb.GetComponent<SetPetPicItemBehavior>();
			SetLinkPets.Add(setUI);
			//
			t = gb.transform.FindChild("Sprite(PetReleaseBG)");
			t.GetComponent<UIButton>().userData = i;
			btnPetRleases.Add(t.GetComponent<UIButton>());
			//
			LinkPetTeam.Add(gb);
		}
	}
	//-------------------------------------------------------------------------------------------------
	private void CloneAllTeamBtns()
	{
		if(GameDataDB.PetLinkDB.GetDataSize()==0)
			return;

		GameDataDB.PetLinkDB.ResetByOrder();
		PetLinkDatas.Clear();
		btnLinkTeams.Clear();
		spriteBorders.Clear();
		Transform t;
		for(int i=0;i<GameDataDB.PetLinkDB.GetDataSize();i++)
		{
			S_PetLink_Tmp petlink_tmp = GameDataDB.PetLinkDB.GetDataByOrder();
			PetLinkDatas.Add(petlink_tmp);
			//
			GameObject gb = NGUITools.AddChild(GridList.gameObject,LinkPrefab.gameObject);
			if(i<10)
				gb.name = "TeamButton0" + i.ToString();
			else
				gb.name = "TeamButton" + i.ToString();
			btnLinkTeams.Add(gb.GetComponent<UIButton>());
			gb.GetComponent<UIButton>().userData = i;
			//
			t = gb.transform.FindChild("Sprite(Border)");
			spriteBorders.Add(t.GetComponent<UISprite>());
			//
			t = gb.transform.FindChild("Label");
			t.GetComponent<UILabel>().text = GameDataDB.GetString(petlink_tmp.iName);
		}
		LinkPrefab.gameObject.SetActive(false);
	}
	public string StringParsing(string str)
	{
		//位索取技能資料&無等級變化  (ﾟ∀ﾟ)＜(需要的話請自己修改帶入參數)
		return ARPGApplication.instance.m_StringParsing.Parsing(str,null,SkillLevelType.Now);

	}

	//-------------------------------------------------------------------------------------------------
	public void LoadTeamData(int index)
	{
		string TeamAllName = "";
		//關係名稱
		lbSkillName.text = GameDataDB.GetString(PetLinkDatas[index].iName);
		//技能說明

		S_SkillData_Tmp skilltmp = GameDataDB.SkillDB.GetData(PetLinkDatas[index].iGetPetLinkSkillID);
		lbLinkSkills[(int)Enum_TeamSkillType.TeamGetSkill].text = StringParsing(GameDataDB.GetString(skilltmp.iNote));
		skilltmp = GameDataDB.SkillDB.GetData(PetLinkDatas[index].iPetLvLinkSkillID);
		lbLinkSkills[(int)Enum_TeamSkillType.TeamLevelSkill].text = StringParsing(GameDataDB.GetString(skilltmp.iNote));
		skilltmp = GameDataDB.SkillDB.GetData(PetLinkDatas[index].iLimitBreakLVLinkSkillID);
		lbLinkSkills[(int)Enum_TeamSkillType.TeamLimitBreakSkill].text = StringParsing(GameDataDB.GetString(skilltmp.iNote));
		//條件說明
		S_PetData_Tmp pDataTmp;
		for(int i=0;i<GameDefine.MAX_PET_LINKCOUNT;++i)
		{
			pDataTmp = GameDataDB.PetDB.GetData(PetLinkDatas[index].iLinkPetID[i]);
			if(pDataTmp == null)
				continue;
			TeamAllName += (TeamAllName=="" ? GameDataDB.GetString(pDataTmp.iName) : "、"+GameDataDB.GetString(pDataTmp.iName));

		}
		lbConditions[(int)Enum_TeamSkillType.TeamGetSkill].text 		= string.Format(GameDataDB.GetString(2709),TeamAllName);
		lbConditions[(int)Enum_TeamSkillType.TeamLevelSkill].text		= string.Format(GameDataDB.GetString(2710),TeamAllName,PetLinkDatas[index].iPetLv.ToString());
		lbConditions[(int)Enum_TeamSkillType.TeamLimitBreakSkill].text	= string.Format(GameDataDB.GetString(2711),TeamAllName,PetLinkDatas[index].iLimitBreakLV.ToString());
		//設定顯示狀態
		UpdateUnlockSkillContent(index);
		//重置文字位置
		//SkillContent.ResetPosition();

		//設定寵物
		SetLinkPetData(PetLinkDatas[index].iLinkPetID);
		//組別中的選擇框
		for(int i=0;i<spriteBorders.Count;++i)
		{
			spriteBorders[i].gameObject.SetActive(false);
			if(i==index)
				spriteBorders[i].gameObject.SetActive(true);
		}
		//紀錄組別
		SelectTeamIndex = index;
	}
	//-------------------------------------------------------------------------------------------------
	//顯示文字技能說明解鎖狀態(TeamIndex)
	public void UpdateUnlockSkillContent(int index)
	{
		//設定顯示狀態
		PetLinkStatus TeamStatus = pkManager.GetPetLinkStatus(PetLinkDatas[index].GUID);
		if(TeamStatus != null)
		{
			lbConditions[(int)Enum_TeamSkillType.TeamGetSkill].color 		= Color.white;
			lbConditions[(int)Enum_TeamSkillType.TeamLevelSkill].color 		= (TeamStatus.bLvSkill ? Color.white : Color.gray);
			lbConditions[(int)Enum_TeamSkillType.TeamLimitBreakSkill].color	= (TeamStatus.bLimitBreakSkill ? Color.white : Color.gray);
		}
		else
		{
			lbConditions[(int)Enum_TeamSkillType.TeamGetSkill].color 		= Color.gray;
			lbConditions[(int)Enum_TeamSkillType.TeamLevelSkill].color 		= Color.gray;
			lbConditions[(int)Enum_TeamSkillType.TeamLimitBreakSkill].color	= Color.gray;
		}
	}
	//-------------------------------------------------------------------------------------------------
	//設定寵物資料
	public void SetLinkPetData(int[] LinkPetID)
	{
		PetPictureDatas.Clear();

		for(int i=0;i<GameDefine.MAX_PET_LINKCOUNT;++i)
		{
			if(LinkPetID[i] <=0)
			{
				LinkPetTeam[i].SetActive(false);
				PetPictureDatas.Add(null);
				continue;
			}
			//找出那隻寵物
			foreach(PetPicture pd in AllPetDatas)
			{
				if(pd.iPetGuid == LinkPetID[i])
				{
					AssingPetDataToObj(pd,i);
					LinkPetTeam[i].SetActive(true);
					PetPictureDatas.Add(pd);
					break;
				}
			}
		}
		//重置寵物位置
		GridTeam.Reposition();
		panelScrollViewTeam.GetComponent<UIScrollView>().ResetPosition();
	}
	//-------------------------------------------------------------------------------------------------
	private void AssingPetDataToObj(PetPicture petdata,int i)
	{
		string			strLevel;
		string			strLimitLevel;
		string			strPieceCount;
		string			strBtnRelease;
		int 			iCurrentPiece 	= 0;	//現有碎片數
		int				iConditionPiece = 0;	//條件碎片數
		string			strLimitCondition;
		Color 			color;
		Color			colorInfoBG;
		Color			colorBorder;
		float			PieceCountScale	= 1.0f;
		S_PetData_Tmp petTmp = GameDataDB.PetDB.GetData(petdata.iPetGuid);
		//同步灰階
		UISprite spritePetInfoBG 	= SetLinkPets[i].spritePetInfoBG;
		UISprite spriteBorder		= SetLinkPets[i].spriteBorder;
		//寵物圖像更換
		UISprite		PetIcon = LinkPetTeam[i].GetComponent<UISprite>();
		Utility.ChangeAtlasSprite(PetIcon,petdata.iPetTexture);
		//資訊更換
		SetLinkPets[i].lbPetName.text 			= GameDataDB.GetString(petdata.iPetName);
		iCurrentPiece 	= petdata.iPetPieceCount;
		iCurrentPiece 	= petdata.iPetPieceCount;
		if(petdata.iPetLevel == 0)
		{
			//灰階變化
			color 		= new Color(0.0f, PetIcon.color.g, PetIcon.color.b);
			colorInfoBG = new Color(0.0f, spritePetInfoBG.color.g, spritePetInfoBG.color.b);
			colorBorder = new Color(0.0f, spriteBorder.color.g, spriteBorder.color.b);
			SetLinkPets[i].spriteBtnSprite.color = Color.gray;
			strLevel 		= "--";
			strLimitLevel 	= "--";
			iConditionPiece = GameDataDB.PetChipDB.GetData(petdata.iPetRank).iSummonCostPetChip;
			strPieceCount	= iCurrentPiece.ToString() + "/" + iConditionPiece.ToString();
			strBtnRelease	= GameDataDB.GetString(2601);		//"解鎖"
			SetLinkPets[i].lbPetName.color = Color.white;
		}
		else
		{
			//反灰階變化
			color 		= new Color(1.0f, PetIcon.color.g, PetIcon.color.b);
			colorInfoBG = new Color(1.0f, spritePetInfoBG.color.g, spritePetInfoBG.color.b);
			colorBorder = new Color(1.0f, spriteBorder.color.g, spriteBorder.color.b);
			SetLinkPets[i].spriteBtnSprite.color = BtnNormalColor;
			strLevel 		= petdata.iPetLevel.ToString ();
			strLimitLevel 	= petdata.iPetLimitLevel.ToString ();
			if(petdata.iPetLimitLevel >= GameDefine.MAX_PET_LIMITLEVEL)
				strLimitCondition = "---";
			else
			{
				iConditionPiece		= GameDataDB.PetChipDB.GetData(petdata.iPetRank).iLimitBreakCostPetChip[petdata.iPetLimitLevel];
				strLimitCondition 	= iConditionPiece.ToString();
			}
			strPieceCount	= iCurrentPiece.ToString() + "/" + strLimitCondition;
			strBtnRelease	= GameDataDB.GetString(2602);		//"突破"
			if (petTmp != null)
				petTmp.SetRareColorString(SetLinkPets[i].lbPetName,true);
		}
		PetIcon.color = color;
		//
		spritePetInfoBG.color 	= colorInfoBG;
		spriteBorder.color		= colorBorder;
		//
		SetLinkPets[i].lbPetLevel.text 					= strLevel;
		SetLinkPets[i].lbPetLimitBreakCount.text		= strLimitLevel;
		SetLinkPets[i].lbCount.text						= strPieceCount;
		SetLinkPets[i].iDBFID							= petdata.iPetGuid;
		SetLinkPets[i].lbPetRelease.text				= strBtnRelease;
		SetLinkPets[i].lbTypeName.text					= GameDataDB.GetString(ARPGApplication.instance.GetPetTypeNameID(petTmp.emCharType));
		//提示是否開啟
		if(iConditionPiece!=0 && iCurrentPiece>=iConditionPiece)
		{
			SetLinkPets[i].spriteOptionMark.gameObject.SetActive(true);
			PieceCountScale = 1.0f;
			//如果是解鎖
			if(strBtnRelease == GameDataDB.GetString(2601))
				SetLinkPets[i].spriteBtnSprite.color = BtnNormalColor;
		}
		else
		{
			SetLinkPets[i].spriteOptionMark.gameObject.SetActive(false);
			PieceCountScale = (float)iCurrentPiece / (float)iConditionPiece;
		}
		SetLinkPets[i].pgPieceCount.value = PieceCountScale;
		//設定星等產生數,突破等級與資訊框選擇
		UISprite[] spriteRankStars	= SetLinkPets[i].spriteRankStars;
		UISprite spritePetType		= SetLinkPets[i].spritePetType;
		//寵物型態圖
		int iTypeDBID = ARPGApplication.instance.GetPetCalssIconID(petdata.PetType);
		
		if(iTypeDBID !=0)
			Utility.ChangeAtlasSprite(spritePetType,iTypeDBID);
		//星等
		for(int j=0;j<spriteRankStars.Length;++j)
		{
			if(j<petdata.iPetLimitLevel)
				spriteRankStars[j].gameObject.SetActive(true);
			else
				spriteRankStars[j].gameObject.SetActive(false);
		}
		
		//選擇使用哪個資訊框
		int RealRank = ARPGApplication.instance.GetPetRealRankWithLimitBreak(petdata.iPetGuid);
		if(RealRank>0 && RealRank<4)
		{
			Utility.ChangeAtlasSprite(spriteBorder,1006);		//夥伴普卡1~3星用純框
			Utility.ChangeAtlasSprite(spritePetInfoBG,1010);	//夥伴普卡1~3星用資訊框
		}
		if(RealRank>3 && RealRank<6)
		{
			Utility.ChangeAtlasSprite(spriteBorder,1005);		//夥伴銀卡4~5星用純框
			Utility.ChangeAtlasSprite(spritePetInfoBG,1009);	//夥伴銀卡4~5星用資訊框
		}
		if(RealRank>5 && RealRank<=7)
		{
			Utility.ChangeAtlasSprite(spriteBorder,1004);		//夥伴金卡6~7星用純框
			Utility.ChangeAtlasSprite(spritePetInfoBG,1008);	//夥伴金卡6~7星用資訊框
		}
			return;												//非正常狀況時


	}
	//-------------------------------------------------------------------------------------------------
	//蒐集全寵物資訊
	public void CollectAllPetDatas()
	{
		pkManager.SetPetData(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.PetData);
		//
		AllPetDatas.Clear();
		GameDataDB.PetDB.ResetByOrder();
		for(int i=0; i<GameDataDB.PetDB.GetDataSize(); ++i)
		{
			S_PetData_Tmp spd = GameDataDB.PetDB.GetDataByOrder();

			//排除掉不顯示圖鑑
			if(spd.PetPictureDisplay == ENUM_PET_DISPLAY.ENUM_PET_DISPLAY_HIDE)
				continue;
			
			PetPicture PetInfoData = new PetPicture();
			PetInfoData.iPetGuid 	= spd.GUID;
			PetInfoData.iPetTexture = spd.Texture;
			PetInfoData.iPetRank	= spd.iRank;
			PetInfoData.iPetName	= spd.iName;
			PetInfoData.PetType		= spd.emCharClass;
			//
			S_PetData pd = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(spd.GUID);
			if(pd != null)
			{
				PetInfoData.iPetLevel 		= pd.iPetLevel;
				PetInfoData.iPetLimitLevel 	= pd.iPetLimitLevel;
				PetInfoData.iPetPieceCount	= pd.iMaterialCount;
			}
			//
			AllPetDatas.Add(PetInfoData);
		}// end for
	}
	//-------------------------------------------------------------------------------------------------
}
