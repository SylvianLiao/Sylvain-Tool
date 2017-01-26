using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_DungeonStory : NGUIChildGUI 
{
	public Slot_RoleIcon	roleIcon		    = null; //角色圖示
	public UILabel			lbName				= null; //角色名稱
	public UILabel			lbContent			= null; //對話內容
	public UISprite			spriteFrame			= null; //對話框

	//
	[HideInInspector]
	public List<int>		StoryGUIDs			= new List<int>();
	//
	private const string me							= "[me]";
	private const string contentme					= "[playername]";

	private const string 	GUI_SMARTOBJECT_NAME = "UI_DungeonStory";
	
	//-------------------------------------------------------------------------------------------------
	private UI_DungeonStory() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
		StoryGUIDs.Clear();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		CheckStoryLists();
	}
	//-------------------------------------------------------------------------------------------------
	public override void Hide()
	{
		base.Hide();
		DirectRemoveAllIndex();
	}
	//-------------------------------------------------------------------------------------------------
	private void CheckStoryLists()
	{
		if(StoryGUIDs.Count<=0)
		{
			Hide();
			return;
		}
		int iStoryGUID = StoryGUIDs[0];
		S_SceneDialogue_Tmp scDialogTmp = GameDataDB.SceneDialogueDB.GetData(iStoryGUID);
		if(scDialogTmp == null)
			return;

		SetDungeonStoryContent(scDialogTmp);
	}
	//-------------------------------------------------------------------------------------------------
	//設定劇情對話內容
	private void SetDungeonStoryContent(S_SceneDialogue_Tmp sdlTmp)
	{
		//音效
		//MusicControlSystem.StopOnceSound("Sound_System_002");
		//MusicControlSystem.PlaySound("Sound_System_002",1);
		//OS
		ARPGApplication.instance.SetAndPlayOSSpeech(sdlTmp.iOSID);

		//
		int			roleSpriteID		= -1;	//圖編號
		roleSpriteID = sdlTmp.iSprite;
		//如果為主角圖代號即設定為主角頭像圖
		if(roleSpriteID == (int)ENUM_SpriteRole.ENUM_SpriteRole_RoleIcon)
			roleSpriteID = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.iFace;
		//設定頭像圖
		roleIcon.SetSlot(roleSpriteID,-1);
		//設定名稱
		lbName.text = SetToSelfRoleName(sdlTmp.iName);
		//設定內文
		lbContent.text = JudgeRoleNameToReplace(sdlTmp.iText);
		//開啟顯示延遲時間
		StartCoroutine(ShowDurationTime((float)sdlTmp.iDuration,sdlTmp));
	}
	//-------------------------------------------------------------------------------------------------
	IEnumerator ShowDurationTime(float seconds,S_SceneDialogue_Tmp sdlTmp)
	{
		yield return new WaitForSeconds(seconds);

		//
		if(sdlTmp.iNext<=0)
		{
			StoryGUIDs.RemoveAt(0);
			CheckStoryLists();
		}
		else
		{
			S_SceneDialogue_Tmp NextDialogTmp = GameDataDB.SceneDialogueDB.GetData(sdlTmp.iNext);
			if(NextDialogTmp != null)
				SetDungeonStoryContent(NextDialogTmp);
		}
	}
	//-------------------------------------------------------------------------------------------------
	//檢查對話內容是否有[玩家名稱]並加以取代
	private string JudgeRoleNameToReplace(int iContent)
	{
		string tempContent 	= GameDataDB.GetString( iContent ); 
		string RoleName		= ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName;
		
		return tempContent.Replace(contentme,RoleName);
	}
	//-------------------------------------------------------------------------------------------------
	//對話人名稱設為自選角色名
	private string SetToSelfRoleName(int iName)
	{
		string TempGetName = GameDataDB.GetString( iName );
		if(TempGetName == me)
			TempGetName = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.m_RoleName;
		
		return TempGetName;
	}
	//-------------------------------------------------------------------------------------------------
	private void DirectRemoveAllIndex()
	{
		if(StoryGUIDs.Count>0)
			StoryGUIDs.Clear();
	}
	//-------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------
}
