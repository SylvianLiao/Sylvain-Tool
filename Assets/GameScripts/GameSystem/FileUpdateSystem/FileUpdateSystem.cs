using UnityEngine;
using System;
using Softstar;
using GameScripts.GameFramework;
using System.Collections;
using System.Collections.Generic;

class FileUpdateSystem : BaseSystem
{
    public enum State
    {
        Init,
        GetUpdateList,
        CheckFileList,
        StartDownload,
        WaitDownload,
        FinishDownload
    }

    //local
    private MainApplication m_mainApp;

    //net
    private DownloadManager m_downloadManager;
    private FileChecker m_fileChecker;

    
    //目前下載狀態
    private State m_state;
    public State DownloadState  { get { return m_state; } }

    //是否下載中
    private bool m_bDownload;
    public bool IsDownloadIng   { get { return m_bDownload; } }

    //目前工作數量
    public int NowJob       { get { return m_downloadManager.NowJob(); } }
    //已完成工作數量
    public int FinishJob    { get { return m_downloadManager.FinishJob(); } }
    //總工作數量
    public int TotalJob     { get { return m_downloadManager.TotalJob(); } }
    //進度百分比
    public float CompletePercent    { get { return m_downloadManager.CompletePercent(); } }

    //整個下載流程完成後觸發事件
    public delegate void DownloadFinish();
    public event DownloadFinish DownloadFinishEvent = null;

    //-----------------------------------------------------------------------------------------------------------
    public FileUpdateSystem(GameApplication app) : base(app)
    {
        //local
        m_mainApp = app as MainApplication;

        //net
        m_downloadManager = new Softstar.DownloadManager();
        m_fileChecker = new Softstar.FileChecker();

        //event
        m_downloadManager.DownloadFinishEvent += HandleDownloadFinish;
    }
    //-----------------------------------------------------------------------------------------
    public override void Initialize()
    {
        base.Initialize();
        m_bDownload = false;
        m_downloadManager.BaseURL = "http://ssmgapp.softstar.com.tw/";        
    }
    //-----------------------------------------------------------------------------------------
    public override void Update()
    {
        base.Update();

        if(m_bDownload)
        {
            DownloadProcess();
        }
    }    
    //-----------------------------------------------------------------------------------------
    //清除下載DB資料
    public void ClearDB()
    {
        FileHashDatabase db = new FileHashDatabase();
        db.Connect();
        db.Truncate();
        UnityDebugger.Debugger.Log("DB Cleared ");
        db.Close();
    }
    //-----------------------------------------------------------------------------------------
    private void HandleDownloadFinish(DownloadJob downloadJob)
    {
        UnityDebugger.Debugger.Log("HandleDownloadFinish " + downloadJob.FileName);
        FileHash fileHash = m_fileChecker.GetFileHash(downloadJob.FileName);
        if (fileHash != null)
        {
            m_fileChecker.UpdateFile(fileHash.SHA1, fileHash.Path, fileHash.Length);
        }
        else
        {
            UnityDebugger.Debugger.LogError(string.Format("HandleDownloadFinish not update file : [{0}]", downloadJob.FileName));
        }
    }
    //-----------------------------------------------------------------------------------------
    //由外部呼叫開始下載流程
    public void BeginDownload()
    {
        m_downloadManager.AddJob("update.json");
        m_state = State.Init;
        m_bDownload = true;
    }
    //-----------------------------------------------------------------------------------------
    //主下載流程
    private void DownloadProcess()
    {
        m_downloadManager.Update();        
        switch (m_state)
        {
            case State.Init:
                if (m_downloadManager.NowJob() <= 0)
                {
                    m_state = State.GetUpdateList;
                }
                break;
            case State.GetUpdateList:
                {
                    m_fileChecker.ConnectDatabase();
                    m_fileChecker.LoadUpdateFile();
                    m_fileChecker.CheckFile(); // 實際使用要改成以非同步進行               
                    m_state = State.CheckFileList;
                }                
                break;
            case State.CheckFileList:
                {
                    if (m_fileChecker.CheckFile() > 0)
                    {
                        m_state = State.StartDownload;
                    }
                    else
                    {
                        UnityDebugger.Debugger.Log("No file need update");
                        m_state = State.FinishDownload;
                    }
                }                
                break;
            case State.StartDownload:
                {
                    m_downloadManager.Clear();
                    List<Softstar.FileHash> fileList = m_fileChecker.DownloadList;
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        m_downloadManager.AddJob(string.Format("{0}.{1}", fileList[i].Path, fileList[i].SHA1), fileList[i].Path);
                    }
                    m_state = State.WaitDownload;
                }                
                break;
            case State.WaitDownload:
                {
                    if (m_downloadManager.NowJob() > 0)
                    {
                    }
                    else
                    {
                        m_state = State.FinishDownload;
                    }
                }                
                break;
            case State.FinishDownload:
                {                    
                    if(DownloadFinishEvent != null)
                    {
                        DownloadFinishEvent();
                        DownloadFinishEvent = null;
                    }
                                        
                    m_bDownload = false;
                }
                break;
            default:
                break;
        }
    }
}
