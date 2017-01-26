using System;
using System.Collections;
using UnityEngine;
using Softstar.GamePacket;
using Softstar;

public class PacketHandler_GamePlay : IPacketHandler
{
    private MainApplication m_mainApp;

    public PacketHandler_GamePlay(GameScripts.GameFramework.GameApplication app) : base(app)
    {
        m_mainApp = app as MainApplication;
    }

    public override void RegisterPacketHandler()
    {
        m_networkSystem.RegisterCallback(PacketId.PACKET_START_STAGE_RES, HandlerPacket_StartStage);
        m_networkSystem.RegisterCallback(PacketId.PACKET_BATTLE_RESULT_RES, HandlerPacket_BattleResult);
    }

    /// <summary>檢查關卡是否合法的封包</summary>
    public void SendPacket_StartStage(int stageID)
    {
        StartStagePacket pk = new StartStagePacket();
        pk.stage_id = stageID;

        m_networkSystem.Send(pk);
    }
    private void HandlerPacket_StartStage(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlerPacket_StartStage : " + strResponse);
        StartStageResPacket pk = JsonUtility.FromJson<StartStageResPacket>(strResponse);

        if (pk.result)
        {
            ChooseDifficultyState cdState = m_mainApp.GetGameStateByName(StateName.CHOOSE_DIFFICULTY_STATE) as ChooseDifficultyState;
            cdState.PrepareToBattle();
            UnityDebugger.Debugger.Log("Game Can Start By Server");
        }
        else
            UnityDebugger.Debugger.Log("Game Can Not Start By Server Error: " + pk.msg);
    }

    /// <summary>傳送遊玩記錄資料封包</summary>
    public void SendPacket_BattleResult(int stageID, int score, int combo)
    {
        BattleResultPacket pk = new BattleResultPacket();

        pk.stage_id = stageID;
        pk.score = score;
        pk.combo = combo;

        m_networkSystem.Send(pk);
    }

    private void HandlerPacket_BattleResult(string strResponse)
    {
        UnityDebugger.Debugger.Log("HandlerPacket_BattleResult : " + strResponse);
        BattleResultResPacket pk = JsonUtility.FromJson<BattleResultResPacket>(strResponse);

        BattleResultState brState = m_mainApp.GetGameStateByName(StateName.BATTLE_RESULT_STATE) as BattleResultState;
        if (pk.result)
        {
            PlayerDataSystem dataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
            if (pk.unlock_list.Length > 0)
            {
                //顯示解鎖歌曲視窗
                m_mainApp.MusicApp.StartCoroutine(brState.ShowUnlockSongBox(pk.unlock_list));
                //將解鎖資訊寫進歌曲資料中
                SongUnlockSystem unlockSys = m_mainApp.GetSystem<SongUnlockSystem>();
                for (int i = 0, iCount = pk.unlock_list.Length; i < iCount; ++i)
                {
                    unlockSys.UnlockSongDifficulty(pk.unlock_list[i]);
                    /*
                    SongDifficultyData unlockDiffData = dataSystem.GetSongDifficultyDataBySongGUID();
                    unlockDiffData.LockStatus = Enum_DifficultyLockStatus.TrueUnlock;
                    SongData unlockSongData = dataSystem.GetSongDataBySongGUID(pk.unlock_list[i]);
                    unlockSongData.IsNewSong = true;
                    unlockSongData.LockStatus = Enum_SongLockStatus.WaitForUnlock;*/
                }
            }
            //判斷是否刷新記錄並更新UI
            brState.HandlePlayData(pk.update_score, pk.update_combo);

            UnityDebugger.Debugger.Log("BattleResult Is Done By Server");
        }
        else
            UnityDebugger.Debugger.Log("BattleResult Error By Server Error: " + pk.msg);
    }
}
