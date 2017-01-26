using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;
using Softstar.GamePacket;
using System;

public class GMCommandState : CustomBehaviorState
{
    private UI_GMCommand m_uiGMCommand;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private NetworkSystem m_networkSystem;
    private PacketHandler_PlayerData m_packetPlayerData;
    private PacketHandler_ItemMall m_packetItemmall;

    private string m_lastCommand;
    private string m_CommandLog;

    private bool m_bWaitCommandRes = false;

    //    
    public delegate void LocalCommandFun(string[] cmd);
    private Dictionary<string, LocalCommandFun> m_dictLocalCommand = new Dictionary<string, LocalCommandFun>();

    public GMCommandState(GameScripts.GameFramework.GameApplication app) : base(StateName.GM_COMMAND_STATE, StateName.GM_COMMAND_STATE, app)
    {
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_networkSystem = m_mainApp.GetSystem<NetworkSystem>();
        m_packetPlayerData = m_networkSystem.GetPaketHandler(typeof(PacketHandler_PlayerData).Name) as PacketHandler_PlayerData;
        m_packetItemmall = m_networkSystem.GetPaketHandler(typeof(PacketHandler_ItemMall).Name) as PacketHandler_ItemMall;

        RegisterLocalCommand();
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("GMCommandState begin");

        //Set the GUI witch is this state want to use.
        base.SetGUIType(typeof(UI_GMCommand));

        base.begin();

        if (m_bIsAync == false)
        {
            m_uiGMCommand = m_guiManager.AddGUI<UI_GMCommand>(typeof(UI_GMCommand).Name);
            m_mainApp.MusicApp.StartCoroutine(CheckScreenShotBeforeInit());
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        m_uiGMCommand = m_guiManager.GetGUI(typeof(UI_GMCommand).Name) as UI_GMCommand;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();

        m_uiGMCommand.SetGMResultLabel(m_CommandLog);

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
    }

    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        m_uiGMCommand = null;

        m_guiManager.DeleteGUI(typeof(UI_GMCommand).Name);
        base.end();
        UnityDebugger.Debugger.Log("GMCommandState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        RemoveCallBack();
        base.suspend();
        UnityDebugger.Debugger.Log("GMCommandState suspend");
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        AddCallBack();
        base.resume();
        UnityDebugger.Debugger.Log("GMCommandState resume");
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendCommondString(null);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_uiGMCommand.m_gmInput.value = m_lastCommand;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_uiGMCommand.m_gmInput.value = "";
        }
    }

    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        UIEventListener.Get(m_uiGMCommand.m_buttonSend.gameObject).onClick = SendCommondString;
    }

    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        UIEventListener.Get(m_uiGMCommand.m_buttonSend.gameObject).onClick = null;
    }
    //---------------------------------------------------------------------------------------------------
    private void OnGMButtonClick(GameObject go)
    {
        m_uiGMCommand.m_objGMCommand.SetActive(!m_uiGMCommand.m_objGMCommand.activeSelf);
    }
    //---------------------------------------------------------------------------------------------------
    private void SendCommondString(GameObject obj)
    {
        if (m_bWaitCommandRes)
        {
            NotifyCommandStatus(false, "Command is Handling!!");
            return;
        }

        if (m_uiGMCommand.m_gmInput.value.Length <= 0)
            return;

        if (m_uiGMCommand.m_gmInput.value[0] == '/')
        {
            HandleLocalCommand(m_uiGMCommand.m_gmInput.value);
        }
        else
        {
            m_packetPlayerData.SendPacket_GMCommand(m_uiGMCommand.m_gmInput.value);

            m_lastCommand = m_uiGMCommand.m_gmInput.value;
            m_uiGMCommand.m_gmInput.value = string.Empty;

            m_bWaitCommandRes = true;
        }
    }
    //---------------------------------------------------------------------------------------------------
    public void NotifyCommandStatus(bool isSucess, string msg = "")
    {
        string res = "";

        if (isSucess)
            res = string.Format("[00FF00]{0} - Success !![-]", m_lastCommand);
        else
            res = string.Format("[FF0000]{0} - Fail !![-]", m_lastCommand);

        m_CommandLog += ('\n' + res);

        if (msg != "")
            m_CommandLog += ('\n' + msg);

        if (m_uiGMCommand != null)
        {
            m_uiGMCommand.SetGMResultLabel(m_CommandLog);
        }
    }
    //---------------------------------------------------------------------------------------------------    
    public void CommandResultProcess(GMCommandResPacket pk)
    {
        NotifyCommandStatus(pk.result, pk.msg);

        if (pk.result)
        {
            string[] stringSeparators = { " " };
            string[] strResult = pk.command.Split(stringSeparators, StringSplitOptions.None);
            switch (strResult[0])
            {
                case "setmoney":
                    int diamond;
                    int.TryParse(strResult[1], out diamond);
                    int diamond1;
                    int.TryParse(strResult[2], out diamond1);
                    PlayerDataSystem dataSys = m_mainApp.GetSystem<PlayerDataSystem>();
                    dataSys.SetPlayerDiamond(diamond + diamond1);
                    m_uiPlayerInfo.SetDiamondUI(diamond+ diamond1);
                    break;
            }
            UnityDebugger.Debugger.Log(pk.command);
        }
        else
        {

        }

        m_bWaitCommandRes = false;
    }

    //---------------------------------------------------------------------------------------------------   
    //---------------------------------------------------------------------------------------------------   
    //LocalCommand
    public void RegisterLocalCommand()
    {
        m_dictLocalCommand.Clear();
        m_dictLocalCommand.Add("zatest", ZaTest);
        m_dictLocalCommand.Add("getaccount", GMGetAccount);
        m_dictLocalCommand.Add("cleardb", GMClearFileUpdateDB);
        m_dictLocalCommand.Add("newguide", GMSetNewGuide);
    }

    public void HandleLocalCommand(string command)
    {
        command = command.Remove(0, 1);
        string[] stringSeparators = { " " };
        string[] strResult = command.Split(stringSeparators, StringSplitOptions.None);

        m_bWaitCommandRes = true;
        m_lastCommand = m_uiGMCommand.m_gmInput.value;

        if (m_dictLocalCommand.ContainsKey(strResult[0]))
        {
            m_dictLocalCommand[strResult[0]](strResult);
        }
        else
        {
            LocalCommandResult(false, "Bad Command");
        }

        m_uiGMCommand.m_gmInput.value = string.Empty;
    }

    public void LocalCommandResult(bool bResult, string msg)
    {
        NotifyCommandStatus(bResult, msg);

        m_bWaitCommandRes = false;
    }

    public void GMGetAccount(string[] cmd)
    {
        RecordSystem recordSystem = m_mainApp.GetSystem<RecordSystem>();
        string strUserData = recordSystem.GetPlayerLoginData();
        PlayerLoginData playerLoginData = JsonUtility.FromJson<PlayerLoginData>(strUserData);

        string msg = string.Format("ID: {0}\nPW: {1}\nTOKEN: {2}", playerLoginData.UserId, playerLoginData.Password, playerLoginData.Token);
        UnityDebugger.Debugger.Log(msg);
        LocalCommandResult(true, msg);
    }

    public void GMClearFileUpdateDB(string[] cmd)
    {
        FileUpdateSystem sys = m_mainApp.GetSystem<FileUpdateSystem>();
        sys.ClearDB();
        string msg = "FileUpdate DB is cleared!!";
        LocalCommandResult(true, msg);
    }

    public void GMSetNewGuide(string[] cmd)
    {
        RecordSystem recordSystem = m_mainApp.GetSystem<RecordSystem>();
        string msg = "";
        if (cmd.Length > 1)
        {
            if (cmd[1] == "on")
            {
                recordSystem.SetIsGuideFinished(false);
                recordSystem.SetIsSkipBattleGuide(false);
                recordSystem.SetIsSkipGameStartGuide(false);

                msg = "New Guide is on!!";
                LocalCommandResult(true, msg);
            }
            else if (cmd[1] == "off")
            {
                recordSystem.SetIsGuideFinished(true);
                recordSystem.SetIsSkipBattleGuide(true);
                recordSystem.SetIsSkipGameStartGuide(true);

                msg = "New Guide is off!!";
                LocalCommandResult(true, msg);
            }
            else
            {
                msg = "New Guide param is Error!!";
                LocalCommandResult(true, msg);
                return;
            }
        }
        else
        {
            msg = "New Guide param is Error!!";
            LocalCommandResult(true, msg);
        }
    }

    public void ZaTest(string[] cmd)
    {
        m_packetItemmall.SendPacket_GetItemmallData();
        LocalCommandResult(true, "");
    }
}
    

