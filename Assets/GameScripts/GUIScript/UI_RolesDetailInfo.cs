using System;
using UnityEngine;
using GameFramework;

public class UI_RolesDetailInfo : NGUIChildGUI 
{
	public UIPanel			panelBase				= null;
	//
	public UITexture		textureRoleFullyIcon 	= null; //角色全身圖
	public UIButton			btnCloseButton			= null; //關閉按鈕
	//角色資訊相關
	public UILabel			lbRoleName				= null; //角色名稱
	public UILabel			lbRoleIntroduceContent	= null; //角色介紹
	public UILabel			lbRolesNote2			= null; //角色生平
	//技能資訊相關
	public UISprite			spriteSkillIcon			= null; //技能圖案
	public UILabel			lbSkillName				= null; //技能名稱
	public UILabel			lbSkillIntroduceContent = null; //技能內容
	//Effect Container
	public UISprite			spriteRoleIntroduceBorder	= null; //角色介紹區塊
	public UISprite			spriteSkillIntroduceBorder	= null; //技能介紹區塊
	//
	private S_PetData_Tmp 	PetDBF					= null; //PetDBF資訊
	private S_SkillData_Tmp SkillDBF				= null;	//SkillDBF資訊
	private S_PetData 		petdata					= null;
	private bool			isUpdated				= false;//是否更新
	private int				PreiPetDBFID			= 0;	//紀錄前一個寵物DBFID

	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_RoleDetailInfo";
	
	//-------------------------------------------------------------------------------------------------------------
	private UI_RolesDetailInfo() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------------------
	public void SetPetInfo(int iPetDBFID,bool isDisplayUsed = false)
	{
		if(iPetDBFID ==0)
			return;
		//載入PetDBF資訊跟SkillDBF資訊
		PetDBF 		= GameDataDB.PetDB.GetData(iPetDBFID);
        if(PetDBF == null)
        {
            UnityDebugger.Debugger.Log("PetDBF Error, ID:" + iPetDBFID);
            return;
        }
		SkillDBF 	= GameDataDB.SkillDB.GetData(PetDBF.iASkillID);
		if(SkillDBF == null)
        {
            UnityDebugger.Debugger.Log("SkillDBF Error, ID:" + PetDBF.iASkillID);
            return;
        }
		Utility.ChangeTexture(textureRoleFullyIcon, PetDBF.FullAvatar);
        //角色圖不做MakePixelPerfect
		//textureRoleFullyIcon.MakePixelPerfect();		//自動調整圖的比例(Base在Height為1024上)
		//spriteRoleFullyIcon.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
        //if(textureRoleFullyIcon.height>720)
        //    textureRoleFullyIcon.SetDimensions((int)(textureRoleFullyIcon.aspectRatio*720),720);
		lbRoleName.text 				= GameDataDB.GetString(PetDBF.iName);
		lbRoleIntroduceContent.text		= GameDataDB.GetString(PetDBF.Note1);
		lbRolesNote2.text				= GameDataDB.GetString(PetDBF.Note2);
		Utility.ChangeAtlasSprite (spriteSkillIcon, SkillDBF.ACTField);
		lbSkillName.text				= GameDataDB.GetString(SkillDBF.SkillName);

        string IntroduceContent = "";
		if(isDisplayUsed == true)
		{
			//先做一個假的PetData
			S_PetData DisplayPetData = new S_PetData(123,
			                                         iPetDBFID,
			                                         1,
			                                         0,
			                                         0,
			                                         1,
			                                         new int[GameDefine.MAX_PET_PSKILL],
			                                         new int[(int)ENUM_PetSkill.ENUM_PetSkill_Max],
			                                         0);
			//再餵給function做說明顯示
			IntroduceContent = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(SkillDBF.iNote),DisplayPetData.GetTalentSkill(),SkillLevelType.Now);
		}
		else
		{
			petdata = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetPetByDBID(iPetDBFID);
			IntroduceContent = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(SkillDBF.iNote),petdata.GetTalentSkill(),SkillLevelType.Now);
		}
		/*IntroduceContent += "\n";

        for(int i = 0; i <PetDBF.iLimitBreakPSkillID.Length; ++i)
        {
            if(PetDBF.iLimitBreakPSkillID[i] == 0)
                continue;
            SkillDBF 	= GameDataDB.SkillDB.GetData(PetDBF.iLimitBreakPSkillID[i]);
            if(SkillDBF != null)
			{
				string str = "";
				str = ARPGApplication.instance.m_StringParsing.Parsing(GameDataDB.GetString(SkillDBF.iNote),petdata.GetTalentSkill(),SkillLevelType.Now);
				IntroduceContent += "\n" + string.Format("{0}{1}:",GameDataDB.GetString(204),1+i)+str;
			}
        }*/

		lbSkillIntroduceContent.text	= IntroduceContent;
		//重置進入特效
		ResetTweenEffect(spriteRoleIntroduceBorder.gameObject);
		ResetTweenEffect(spriteSkillIntroduceBorder.gameObject);
		ResetTweenEffect(textureRoleFullyIcon.gameObject);
	}
	//-------------------------------------------------------------------------------------------------------------
	private void ResetTweenEffect(GameObject gb)
	{
		TweenPosition tp = gb.GetComponent<TweenPosition>();
		if(tp != null)
		{
			tp.ResetToBeginning();
			tp.PlayForward();
		}

		TweenScale ts = gb.GetComponent<TweenScale>();
		if(ts != null)
		{
			ts.ResetToBeginning();
			ts.PlayForward();
		}
		TweenAlpha ta = gb.GetComponent<TweenAlpha>();
		if(ta != null)
		{
			ta.ResetToBeginning();
			ta.PlayForward();
		}
	}
	//-------------------------------------------------------------------------------------------------------------
}
