using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Softstar;

class FileUpdateState : CustomBehaviorState 
{
    //ui
    public UI_FileUpdate m_uiFileUpdate;      

    private FileUpdateSystem m_FileUpdateSys;

    //-----------------------------------------------------------------------------------------
    public FileUpdateState(GameScripts.GameFramework.GameApplication app) : base(StateName.FILE_UPDATE_STATE, StateName.FILE_UPDATE_STATE, app)
    {
    }
    //-----------------------------------------------------------------------------------------
    public override void begin()
    {
        //Set the GUI witch is this state want to use.
        SetGUIType(typeof(UI_FileUpdate));

        UnityDebugger.Debugger.Log("FileUpdateState begin");
        base.begin();

        m_uiFileUpdate = m_guiManager.AddGUI<UI_FileUpdate>(typeof(UI_FileUpdate).Name);
        m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());

        //=======================================================
        m_FileUpdateSys = m_mainApp.GetSystem<FileUpdateSystem>();
        m_FileUpdateSys.DownloadFinishEvent += DownloadFinish;

        if(!m_FileUpdateSys.IsDownloadIng)
            m_FileUpdateSys.BeginDownload();
    }
    //-----------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_gui3DManager.Initialize();
    }
    //-----------------------------------------------------------------------------------------
    public override void end()
    {
        base.end();
    }
    //-----------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();

        switch (m_FileUpdateSys.DownloadState)
        {
            case FileUpdateSystem.State.Init:
                {
                    m_uiFileUpdate.m_lbMessage.text = "GetUpdateList";
                }
                break;
            case FileUpdateSystem.State.GetUpdateList:
                {
                    m_uiFileUpdate.m_lbMessage.text = "CheckFileList";
                }
                break;
            case FileUpdateSystem.State.CheckFileList:
                break;
            case FileUpdateSystem.State.StartDownload:
                {
                    //顯示UI                
                    m_uiFileUpdate.Show();
                }
                break;
            case FileUpdateSystem.State.WaitDownload:
                {
                    //顯示UI                
                    m_uiFileUpdate.Show();

                    m_uiFileUpdate.m_lbMessage.text = string.Format("Download: {0:P}", m_FileUpdateSys.CompletePercent);
                    m_uiFileUpdate.m_lbUpdateCount.text = string.Format("Update: {0}/{1}", m_FileUpdateSys.FinishJob, m_FileUpdateSys.TotalJob);
                }
                break;
            case FileUpdateSystem.State.FinishDownload:
                {
                    m_uiFileUpdate.m_lbUpdateCount.text = string.Format("Update: {0}/{0}", m_FileUpdateSys.TotalJob);
                    m_uiFileUpdate.m_lbMessage.text = "Download finish";

                    Hashtable table = new Hashtable();
                    table.Add(Enum_StateParam.LoadGUIAsync, false);
                    table.Add(Enum_StateParam.DelayDeleteGUIName, GetDelayDeleteGUIName());

                    if (userData != null)
                    {
                        if (userData.ContainsKey(GameDefine.FILEUPDATE_NEXTSTATE))
                            m_mainApp.ChangeStateByScreenShot((string)userData[GameDefine.FILEUPDATE_NEXTSTATE], table);
                        else
                            m_mainApp.ChangeStateByScreenShot(StateName.THEME_STATE, table);
                    }
                    else
                    {
                        m_mainApp.ChangeStateByScreenShot(StateName.THEME_STATE, table);
                    }
                }
                break;
            default:
                break;
        }
    }
    //-----------------------------------------------------------------------------------------
    public void DownloadFinish()
    {           
        
    }    
    //-----------------------------------------------------------------------------------------
    private string[] GetDelayDeleteGUIName()
    {
        return new string[] { typeof(UI_FileUpdate).Name };
    }
}
