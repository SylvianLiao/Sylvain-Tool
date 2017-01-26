using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class BattleResultState : CustomBehaviorState
{
    private UI_BattleResult m_uiBattleResult = null;
    private UI_BattleResultAnim m_uiBattleResultAnim = null;
    private UI_3D_Rank_Scroll m_ui3D_Rank_Scroll;
    private UI3DChildGUI m_ui3DRank = null;

    private ResourceManager m_resourceManager;
    private GameDataDB m_gameDataDB;
    private PlayerDataSystem m_dataSystem;
    private SoundSystem m_soundSystem;
    private PacketHandler_GamePlay m_packetGamePlay;

    private SongData m_currentSongData
    {
        get { return m_dataSystem.GetCurrentSongData(); }
        set { m_dataSystem.SetCurrentSong(value); }
    }

    public BattleResultState(GameScripts.GameFramework.GameApplication app) : base(StateName.BATTLE_RESULT_STATE, StateName.BATTLE_RESULT_STATE, app)
    {
        m_mainApp = app as MainApplication;
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_guiManager = m_mainApp.GetGUIManager();
        m_dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        m_soundSystem = m_mainApp.GetSystem<SoundSystem>();
        m_packetGamePlay = m_mainApp.GetSystem<NetworkSystem>().GetPaketHandler(typeof(PacketHandler_GamePlay).Name) as PacketHandler_GamePlay;
    }
    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        UnityDebugger.Debugger.Log("BattleResultState begin");

        //特別處理: 要先讀取資料才知道要讀取哪個RankUI
        base.SetGUIType(Get3DRankUIType(m_dataSystem.GetCurrentSongPlayData().Rank), true);
        base.SetGUIType(typeof(UI_3D_Rank_Scroll), true);
        base.SetGUIType(typeof(UI_BattleResultAnim));
        base.SetGUIType(typeof(UI_BattleResult));
        userData.Add(Enum_StateParam.UseTopBarUI, true);
        base.begin();

        if (m_bIsAync == false)
        {
            m_uiBattleResult = m_guiManager.AddGUI<UI_BattleResult>(typeof(UI_BattleResult).Name);
            m_uiBattleResultAnim = m_guiManager.AddGUI<UI_BattleResultAnim>(typeof(UI_BattleResultAnim).Name);
            StateInit();
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_Type.Equals(typeof(UI_BattleResult)))
            m_uiBattleResult = m_guiManager.GetGUI(typeof(UI_BattleResult).Name) as UI_BattleResult;
        else if(operater.m_Type.Equals(typeof(UI_BattleResultAnim)))
            m_uiBattleResultAnim = m_guiManager.GetGUI(typeof(UI_BattleResultAnim).Name) as UI_BattleResultAnim;
        else if (operater.m_Type.Equals(typeof(UI_3D_Rank_Scroll)))
            m_ui3D_Rank_Scroll = m_gui3DManager.GetGUI(typeof(UI_3D_Rank_Scroll).Name) as UI_3D_Rank_Scroll;
        else if (operater.m_Type.Equals(Get3DRankUIType(m_dataSystem.GetCurrentSongPlayData().Rank)))
            m_ui3DRank = m_gui3DManager.GetGUI(Get3DRankUIType(m_dataSystem.GetCurrentSongPlayData().Rank).Name) as UI3DChildGUI;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //UI初始化
        m_guiManager.Initialize();
        m_uiBattleResult.InitializeUI(m_dataSystem, m_gameDataDB, m_mainApp.GetStringTable());
        m_uiBattleResultAnim.InitializeUI(m_dataSystem, m_gameDataDB);
        m_uiBattleResultAnim.InitSequencer(m_soundSystem);
#if DEVELOP
        UIEventListener.Get(m_uiBattleResultAnim.m_buttonSkip.gameObject).onClick = OnButtonSkipAnimClick;
#endif
        SongPlayData playData = m_dataSystem.GetCurrentSongPlayData();
        if (m_mainApp.IsLogin())
        {
            //將遊玩資料送至Server
            S_Songs_Tmp songTmp = m_dataSystem.GetSongTmp(m_currentSongData, playData.SongDifficulty);
            m_packetGamePlay.SendPacket_BattleResult(songTmp.GUID, playData.Score, playData.Combo);
        }
        else
        {
            //將遊玩資料記錄至本地端
            RecordSystem recordSys = m_mainApp.GetSystem<RecordSystem>();
            recordSys.SetLocalPlayData(playData, m_currentSongData);
            HandlePlayData( recordSys.GetMaxScore(m_currentSongData, playData.SongDifficulty),
                            recordSys.GetMaxCombo(m_currentSongData, playData.SongDifficulty));
        }

        m_uiBattleResultAnim.OnAnimPlayFinish = ChangeToUIBattleResult;
        //開始播結算動畫
        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
        RemoveGUIType(typeof(UI_BattleResultAnim));
        HideBeSetGUI();
        m_uiTopBar.Hide();
    }
    //---------------------------------------------------------------------------------------------------
    private void ChangeToUIBattleResult()
    {
        //刪除結算動畫
        m_guiManager.DeleteGUI(typeof(UI_BattleResultAnim).Name);
        m_uiBattleResultAnim = null;
        //設定於GUI TYPE中的UI顯示
        ShowBeSetGUI();
        m_uiTopBar.Show();
        //播放結算音效
        m_soundSystem.PlayBattleEndSound();
        //判斷是否顯示解鎖視窗
        if (!m_mainApp.IsLogin())
            DeterShowUnlockSongBox_Offline();
        //播放數字滾動效果
        m_mainApp.MusicApp.StartCoroutine(CheckPlayNumberPicker());
    }
    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        UnityDebugger.Debugger.Log("BattleResultState End");

        //清除目前遊玩中資料
        m_dataSystem.SetCurrentSongPlayData(new SongPlayData());

        m_uiBattleResult = null;

        base.end();
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        base.suspend();
        UnityDebugger.Debugger.Log("BattleResultState suspend");
        if (m_bIsSuspendByFullScreenState)
        {
            if (m_uiBattleResult != null)
                m_uiBattleResult.Hide();
            if (m_uiBattleResultAnim != null)
                m_uiBattleResultAnim.Hide();
        }
        RemoveCallBack();
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        base.resume();
        UnityDebugger.Debugger.Log("BattleResultState resume");

        if (m_uiBattleResult != null)
        {
            if (m_bIsSuspendByFullScreenState)
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
            else
            {
                if (m_uiBattleResultAnim == null)
                    CheckActiveCommonGUI();
                AddCallBack();
            }
            m_bIsSuspendByFullScreenState = false;
        }
    }
    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
    }
    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        //Common UI Delegate
        m_uiTopBar.m_onButtonBackClick = OnButtonBackClick;
        m_uiTopBar.AddCallBack();

        UIEventListener.Get(m_uiBattleResult.m_buttonPlayAgain.gameObject).onClick = OnButtonPlayAgainClick;
        UIEventListener.Get(m_uiBattleResult.m_buttonRank.gameObject).onClick = OnButtonRankClick;
    }
    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        //Common UI Delegate
        m_uiTopBar.m_onButtonBackClick = null;
        m_uiTopBar.RemoveCallBack();

        UIEventListener.Get(m_uiBattleResult.m_buttonPlayAgain.gameObject).onClick = null;
        UIEventListener.Get(m_uiBattleResult.m_buttonRank.gameObject).onClick = null;
    }
#region ButtonEvent
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackClick()
    {
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, GetDelayDeleteGUIName());
        m_mainApp.ChangeStateByScreenShot(StateName.CHOOSE_SONG_STATE, table);
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonPlayAgainClick(GameObject go)
    {
        PlayAgain();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonRankClick(GameObject go)
    {
        return;
    }
    //---------------------------------------------------------------------------------------------------
    //Test
    private void OnButtonSkipAnimClick(GameObject go)
    {
        m_uiBattleResultAnim.m_animator.Play("fin");
    }
#endregion

    //---------------------------------------------------------------------------------------------------
    private void PlayAgain()
    {
        m_mainApp.PopStateByScreenShot();
        m_guiManager.DeleteGUI(typeof(UI_BattleResult).Name);
        m_gui3DManager.DeleteGUI(typeof(UI_3D_Rank_Scroll).Name);
        m_gui3DManager.DeleteGUI(m_ui3DRank.GetUIName());
        GamePlayState gpState = m_mainApp.GetGameStateByName(StateName.GAME_PLAY_STATE) as GamePlayState;
        gpState.m_bReplay = true;
    }
    //---------------------------------------------------------------------------------------------------
    private System.Type Get3DRankUIType(Enum_SongRank rank)
    {
        System.Type type = null;
        switch (rank)
        {
            case Enum_SongRank.A: type = typeof(UI_3D_Rank_A); break;
            case Enum_SongRank.B: type = typeof(UI_3D_Rank_B); break;
            case Enum_SongRank.C: type = typeof(UI_3D_Rank_C); break;
            case Enum_SongRank.D: type = typeof(UI_3D_Rank_D); break;
            case Enum_SongRank.S: type = typeof(UI_3D_Rank_S); break;
            case Enum_SongRank.G: type = typeof(UI_3D_Rank_S); break;
            case Enum_SongRank.New:type = typeof(UI_3D_Rank_D);break;
            default:              type = typeof(UI_3D_Rank_D);break;
        }
        return type;
    }
    //---------------------------------------------------------------------------------------------------
    //處理遊玩資料
    public void HandlePlayData(int score, int combo)
    {
        //取得玩家目前遊玩的資料
        Enum_SongDifficulty difficulty = m_dataSystem.GetCurrentSongPlayData().SongDifficulty;
        //判斷是否刷新記錄並更新UI
        Enum_SongRank rank = m_dataSystem.InquireSongRankByScore(m_currentSongData.SongGroupID, difficulty, score);
        m_uiBattleResult.SetNewUI(  m_dataSystem.InquireScoreIsHighest(m_currentSongData.SongGroupID, difficulty, score),
                                    m_dataSystem.InquireComboIsHighest(m_currentSongData.SongGroupID, difficulty, combo),
                                    m_dataSystem.InquireRankIsHighest(m_currentSongData.SongGroupID, difficulty, rank));

        //更新資料至玩家歌曲遊玩資料中
        SongDifficultyData diffData = m_currentSongData.GetSongDifficultyData(difficulty);
        diffData.UpdateData(score, combo, rank);
        m_currentSongData.IsNewSong = false;
    }
    //---------------------------------------------------------------------------------------------------
    //判斷解鎖新歌曲視窗是否顯示(離線模式功能)
    private bool DeterShowUnlockSongBox_Offline()
    {
        SongUnlockSystem unlockSys = m_mainApp.GetSystem<SongUnlockSystem>();
        S_Songs_Tmp songTmp = m_dataSystem.GetSongTmp(m_currentSongData, m_dataSystem.GetCurrentSongPlayData().SongDifficulty);
        int[] unlockSongID;
        //解鎖下一首歌曲難度
        if (!unlockSys.UnlockNextSong_Offline(songTmp.GUID, out unlockSongID))
            return false;

        m_mainApp.MusicApp.StartCoroutine(ShowUnlockSongBox(unlockSongID));
        return true;
    }
    //---------------------------------------------------------------------------------------------------
    public IEnumerator ShowUnlockSongBox(int[] unlockSongID)
    {
        //等待結算動畫結束才能顯示視窗
        while(m_uiBattleResultAnim != null)
            yield return null;

        string content = m_mainApp.GetString(44);   //新的{0}已經解鎖！！
        string content1 = m_mainApp.GetString(50);   //歌曲{0}難度{1}！！
        string param = null;
        for (int i = 0, iCount = unlockSongID.Length; i < iCount; ++i)
        {
            S_Songs_Tmp songTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(unlockSongID[i]);
            string songName = m_mainApp.GetString(songTmp.iShowName);
            string diffString = m_mainApp.GetString(47 + songTmp.iDifficulty);    //"簡單"
            string songDiffString = string.Format(content1, songName, diffString);
            param += (i == 0) ? songDiffString : "、" + songDiffString;
        }
        content = string.Format(content, param);
        //39: 解鎖新的歌曲難度, 41: 確定
        m_mainApp.PushIconCheckBox(m_mainApp.GetString(43), null, content, m_mainApp.GetString(41), null, null, 106); //解鎖系統用圖案
    }

    //---------------------------------------------------------------------------------------------------
    private string[] GetDelayDeleteGUIName()
    {
        return new string[] { typeof(UI_BattleResult).Name,
            /*Get3DRankUIType(m_dataSystem.GetCurrentSongPlayData().Rank).Name,*/
            typeof(UI_3D_BattleBG).Name,
            typeof(UI_GamePlay).Name ,
            m_ui3DRank.GetUIName() ,
            typeof(UI_3D_Rank_Scroll).Name };
    }
    //---------------------------------------------------------------------------------------------------
    private IEnumerator CheckPlayNumberPicker()
    {
        while (!isPlaying)
            yield return null;

        m_uiBattleResult.PlayNumberPicker();
    }
    //---------------------------------------------------------------------------------------------------
    public void SetNewUI(bool isScoreNew, bool isComboNew, bool isRankNew)
    {
        m_uiBattleResult.SetNewUI(isScoreNew, isComboNew, isRankNew);
    }
}

