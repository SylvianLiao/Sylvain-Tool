using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using LitJson;
using System;

public class UI_ForeBulletinBoard : NGUIChildGUI
{
	public UILabel			lbBoardTitle		= null;		//公告標頭
	//左側類型
	public UIScrollView		svTitleList			= null;		//公告類型scroll view
	public UIGrid			gdTitleList			= null;		//公告類型排序
	public UIToggle			tgPrefab			= null;		//公告類型prefab
	public UILabel			lbTitle				= null;		//公告類型名稱
	public UIButton			btnPrefab			= null;		//公告類型btn
	//右側內容
	public UILabel			lbTitleName			= null;		//公告類型名稱
	public UIScrollView		svContent			= null;		//公告內容scroll view
	public UILabel			lbContent			= null;		//公告內容
	//底下
	public UIButton			btnQuit				= null;		//離開按鈕
	public UILabel			lbQuit				= null;		//離開按鈕字樣
	//
	public UIPanel			panelLoading		= null;		//載入中頁面
	private Dictionary<int,EnumBulletinKinds>	 BulletinInfos = new Dictionary<int, EnumBulletinKinds>();
	private int				RecordNoteGUID		= 0;
	private List<UIToggle>	tgLists				= new List<UIToggle>();
	//防止濫網路卡死 5秒解鎖定用
	bool timeOutFlag = false;

	private const string GUI_SMARTOBJECT_NAME = "UI_ForeBulletinBoard";

	//-------------------------------------------------------------------------------------------
	private UI_ForeBulletinBoard(): base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-------------------------------------------------------------------------------------------
	public override void Initialize ()
	{
		base.Initialize ();
		//從WebServer下載並載入資料
		//StartCoroutine(DownloadAndSetDBFData());
		//string url = C_AssetBundleMgr.GetMainWebServiceIP() +"/GameAnnouncement/GameAnnouncement.dbf"

		tgLists.Clear();
		RecordNoteGUID = 0;
		lbBoardTitle.text 	= GameDataDB.GetString(481);		//公告
		lbQuit.text			= GameDataDB.GetString(486);		//確認
		//CreateTitleSlotAndStoreInfo();
	}
	//-------------------------------------------------------------------------------------------
	IEnumerator DownloadAndSetDBFData()
	{
		//防止濫網路卡死 5秒解鎖定用
		DateTime beginTime = DateTime.Now;
		TimeSpan ts;
		timeOutFlag = false;

		string GameAnnouncementPath = Application.persistentDataPath+"/DBF/NoticeBoard.dbf";
		//Download
		string url = C_AssetBundleMgr.NoticeBoardURL;

		WWW latestNote = new WWW(url);
		//
		yield return null;

		while(!latestNote.isDone && !timeOutFlag)
		{
			ts = DateTime.Now - beginTime;
			if(ts.TotalSeconds > 5)
			{
				timeOutFlag = true;
			}
			yield return null;
		}
			


		if(!string.IsNullOrEmpty(latestNote.error) || !latestNote.isDone)
		{
			UnityDebugger.Debugger.Log(latestNote.error);
			tgPrefab.gameObject.SetActive(false);
			lbTitleName.gameObject.SetActive(false);
			lbContent.gameObject.SetActive(false);
			panelLoading.gameObject.SetActive(false);
			yield break;
		}
		/*else
		{
			string Folder = Path.GetDirectoryName(GameAnnouncementPath);
			if (!Directory.Exists(Folder))
				Directory.CreateDirectory(Folder);
			FileStream fileStream = new FileStream(GameAnnouncementPath,FileMode.Create);
			fileStream.Write(latestNote.bytes,0,latestNote.bytes.Length);
			fileStream.Close();
		}*/

		// Load Dbf
		bool usingWWW = (ARPGApplication.instance.readDBFTYpe == ENUM_LOAD_DBF.ENUM_FROM_LOCAL);
		if (usingWWW)
		{
			C_AssetBundleMgr.DBF_Encode = false;
			yield return StartCoroutine(GameDataDB.LoadFromFile(GameDataDB.GameAnnouncementDB, GameDataDB.GetPath_DBF("NoticeBoard.dbf"), true, usingWWW, true));
			C_AssetBundleMgr.DBF_Encode = true;
		}
		else
		{
			/*
			string sp = "\r\n";
			string[] lineStr = latestNote.text.Split(sp.ToCharArray());
			foreach (string str in lineStr)
			{
				S_GameAnnouncement_Tmp noticeData = JsonMapper.ToObject<S_GameAnnouncement_Tmp>(str);
				GameDataDB.GameAnnouncementDB.AddData(noticeData);
			}
			*/
			S_GameAnnouncement_Tmp [] noticeData = JsonMapper.ToObject<S_GameAnnouncement_Tmp[]> (latestNote.text);
			//List<S_GameAnnouncement_Tmp> sgt = new List<S_GameAnnouncement_Tmp>(noticeData);
			GameDataDB.GameAnnouncementDB.Clear();
			for (int i = 0 ; i < noticeData.Length ; i++)
			{
				GameDataDB.GameAnnouncementDB.AddData(noticeData[i]);
			}
		}


		CreateTitleSlotAndStoreInfo();
	}
	//-------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------
	public override void Show()
	{
		base.Show();
		//
		panelLoading.gameObject.SetActive(true);
		//從WebServer下載並載入資料
		StartCoroutine(DownloadAndSetDBFData());
	}
	//-------------------------------------------------------------------------------------------
	//依照DBF中公告的數量產生出相對應的公告數並儲存
	private void CreateTitleSlotAndStoreInfo()
	{
		//清掉前次公告留下初始的prefab
		if(tgLists.Count>0)
		{
			for(int i=0;i<tgLists.Count;++i)
			{
				if(i == 0)
					continue;
				//
				GameObject.DestroyImmediate(tgLists[i].gameObject);
			}
		}

		tgLists.Clear();
		BulletinInfos.Clear();
		RecordNoteGUID = 0;
		GameDataDB.GameAnnouncementDB.ResetByOrder();
		for(int i=0; i < GameDataDB.GameAnnouncementDB.GetDataSize(); ++i)
		{
			S_GameAnnouncement_Tmp GATmp = GameDataDB.GameAnnouncementDB.GetDataByOrder();
			if (GATmp == null)
				continue;

			BulletinInfos.Add(GATmp.GUID,GATmp.iTypeID);
		}
		if(BulletinInfos.Count<=0)
		{
			tgPrefab.gameObject.SetActive(false);
			lbTitleName.gameObject.SetActive(false);
			lbContent.gameObject.SetActive(false);
			panelLoading.gameObject.SetActive(false);
			return;
		}
		int a =0;
		UILabel lbtClone;
		UIToggle tgClone;
		UIButton btnClone;
		//如果有資訊即生成物件
		foreach(int GUID in BulletinInfos.Keys)
		{
			if(a==0)
			{
				lbTitle.text 		= MatchGUIDToTypeString(GUID);
				btnPrefab.userData 	= GUID;
				UIEventListener.Get(btnPrefab.gameObject).onClick		= SwitchBulletinKindsEvent;
				tgPrefab.startsActive = true;
				tgLists.Add(tgPrefab);
				//
				++a;
				continue;
			}
			tgClone = Instantiate(tgPrefab) as UIToggle;
			tgClone.startsActive = false;
			tgClone.transform.parent			= gdTitleList.transform;
			tgClone.transform.localScale		= Vector3.one;
			tgClone.transform.localRotation		= Quaternion.identity;
			tgClone.transform.localPosition		= Vector3.zero;
			tgClone.transform.name				= "Toggle0"+a.ToString();
			lbtClone = tgClone.transform.FindChild("Label").GetComponent<UILabel>();
			if(lbtClone == null)
			{
				UnityDebugger.Debugger.Log("No Label");
				return;
			}
			lbtClone.text 						= MatchGUIDToTypeString(GUID);
			btnClone = tgClone.transform.GetComponent<UIButton>();
			if(btnClone == null)
			{
				UnityDebugger.Debugger.Log("No button");
				return;
			}
			btnClone.userData = GUID;
			UIEventListener.Get(btnClone.gameObject).onClick		+= SwitchBulletinKindsEvent;
			tgLists.Add(tgClone);
			//
			++a;
		}
		gdTitleList.repositionNow = true;
		svContent.ResetPosition();
		SwitchBulletinKindsEvent(btnPrefab.gameObject);
		panelLoading.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------------------------------------
	private string MatchGUIDToTypeString(int iGUID)
	{
		S_GameAnnouncement_Tmp GATmp = GameDataDB.GameAnnouncementDB.GetData(iGUID);
		if(GATmp == null)
			return "";

		return GATmp.sName;
	}
	//-------------------------------------------------------------------------------------------
	//切換公告類型事件
	private void SwitchBulletinKindsEvent(GameObject gb)
	{
		int NoteGUID = (int)(gb.GetComponent<UIButton>().userData);
		if(NoteGUID <=0)
			return;

		S_GameAnnouncement_Tmp GATmp = GameDataDB.GameAnnouncementDB.GetData(NoteGUID);
		if(GATmp == null)
			return;

		if(GATmp.GUID == RecordNoteGUID)
			return;

		//右側顯示
		lbTitleName.text 	= GATmp.sName;
		lbContent.text		= GATmp.sLiteral;
		svContent.ResetPosition();
		RecordNoteGUID = GATmp.GUID;
	}
	//-------------------------------------------------------------------------------------------
}
