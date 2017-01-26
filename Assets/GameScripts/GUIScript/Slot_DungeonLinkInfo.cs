using UnityEngine;
using System.Collections;

public class Slot_DungeonLinkInfo: MonoBehaviour
{
	public UIButton			btnInfo				= null;
	public UISprite			spriteIcon			= null;	//副本圖樣
	public UISprite			spritelock			= null; //鎖圖
	//public UISprite[]		spriteStar			= new UISprite[3]; //星級評價
	public UILabel			lbLVLimit			= null; //等級限制
	public UILabel			lbDungeonName		= null; //關卡名稱
	//
	[HideInInspector]
	public int				index				= -1;	//特定的DungeonInfo內容 (DungeonInfo[index])
	[System.NonSerialized]
	public S_Dungeon_Tmp	dbfData				= null;
	//
	private const int 			iBossLock		= 129000;
	private const int 			iNormalLock		= 1150;
	private bool			bDungeonLock	= false;
	private bool			bLVLimitLock	= false;
	//-----------------------------------------------------------------------------------------------------
	void Awake()
	{
		UIEventListener.Get(spritelock.gameObject).onClick += AlertNotYetOpenDungeon;
	}
	//-----------------------------------------------------------------------------------------------------
	private void init()
	{
		dbfData = null;
		index = -1;
		bDungeonLock = false;
		bLVLimitLock = false;
		Utility.ChangeAtlasSprite(spritelock,iNormalLock);
		spritelock.SetDimensions(50 , 50);
		spriteIcon.color = new Color(0.4f,0.4f,0.4f);
		spriteIcon.SetDimensions(100 , 100);
		btnInfo.isEnabled=true;
		lbLVLimit.gameObject.SetActive(false);
		spritelock.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetLinkData(S_Dungeon_Tmp dbf)
	{
		if(dbf == null)
		{
			btnInfo.gameObject.SetActive(false);
			return;
		}
		init();
		dbfData = dbf;
		lbDungeonName.text = GameDataDB.GetString(dbf.iName);
		//
		lbLVLimit.text = string.Format(GameDataDB.GetString(1595), dbf.iLevelLimit.ToString());
		//設定鎖圖
		switch(dbf.DungeonIconSize)
		{
		case ENUM_DungeonIconSize.ENUM_DungeonIconSize_Small:
			//spriteIcon.SetDimensions(100 , 100);
			break;
		case ENUM_DungeonIconSize.ENUM_DungeonIconSize_Medium:
			//spriteIcon.SetDimensions(120 , 120);
			break;
		case ENUM_DungeonIconSize.ENUM_DungeonIconSize_big: //BOSS關
			Utility.ChangeAtlasSprite(spritelock,iBossLock);
			//spriteIcon.SetDimensions(190 , 190);
			spritelock.SetDimensions(100 , 100);
			break;
		}
		S_TextureManager_Tmp texManager =  GameDataDB.TextureManagerDB.GetData(dbf.iDungeonIcon);
		if(texManager == null)
		{
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox("DungeonDBFID"+dbf.GUID+"關卡非正規");
			btnInfo.gameObject.SetActive(false);
			return;
		}
		Utility.ChangeAtlasSprite(spriteIcon,dbf.iDungeonIcon);

		//檢查關卡解鎖條件, 等級不足= 鎖住+不防點擊、任務未解= 鎖住+防點擊
		S_QuestData_Tmp questDBF = null;
		if(dbf.iUnlockQuestID >= 0)
		{
			questDBF = GameDataDB.QuestDB.GetData(dbf.iUnlockQuestID);
			//判斷任務是否已達解鎖關卡
			if(questDBF!=null && questDBF.iPreFlag>=0)
			{
				if (ARPGApplication.instance.m_RoleSystem.sBaseQuestFlag.Get(questDBF.iPreFlag))
				{
					if(dbf.iGroupRank == ENUM_GroupRank_Type.ENUM_Rank_Hard)
					{
						if(ARPGApplication.instance.CheckHardRankUnlockStatus(dbf.iGroup))
							spriteIcon.color = Color.white;
						else
						{
							bDungeonLock = true;
							spritelock.gameObject.SetActive(true);
							btnInfo.isEnabled=false;
						}
					}
					else
						spriteIcon.color = Color.white;

				}
				else
				{
					bDungeonLock = true;
					spritelock.gameObject.SetActive(true);
					btnInfo.isEnabled=false;
				}
			}
			//判斷等級是否足夠
			if(ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.GetLevel()<dbf.iLevelLimit)
			{
				bLVLimitLock = true;
				spritelock.gameObject.SetActive(true);
				lbLVLimit.gameObject.SetActive(true);
				btnInfo.isEnabled=false;
			}
		}
		//防堵例外
		else
		{
			spriteIcon.color = Color.white;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	private void AlertNotYetOpenDungeon(GameObject gb)
	{
		if(bDungeonLock)
		{
			string str = GameDataDB.GetString(2107);					//"關卡未解鎖"
			ARPGApplication.instance.m_uiMessageBox.SetMsgBox(str);
			return;
		}
		//未達等級限制時
		if(bLVLimitLock)
		{
			string WarningStr = string.Format(GameDataDB.GetString(1811),dbfData.iLevelLimit);
			ARPGApplication.instance.PushOkBox(WarningStr,null,false);
			return;
		}
	}
	//-----------------------------------------------------------------------------------------------------
}
