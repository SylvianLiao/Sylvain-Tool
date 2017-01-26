using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

/// <summary>音效系統</summary>
public class SoundSystem : BaseSystem
{
    private ResourceManager m_resourceManager;
    private T_GameDB<S_Sound_Tmp> m_soundDataDB;
    private AudioController m_audioController;

    private List<int> m_commonSoundIDList;

    private Dictionary<int, int> m_nativeSoundMap;

    public SoundSystem(GameScripts.GameFramework.GameApplication app) : base(app){}
    //-----------------------------------------------------------------------------------------------------
    public override void Initialize()
    {
        MainApplication mainApp = gameApplication as MainApplication;
        m_audioController = mainApp.GetAudioController();
        m_resourceManager = mainApp.GetResourceManager();
        m_soundDataDB = mainApp.GetGameDataDB().GetGameDB<S_Sound_Tmp>();

        m_commonSoundIDList = new List<int>();
        m_nativeSoundMap = new Dictionary<int, int>();
        CreatCommonSoundList();

        //音效設定
        RecordSystem recordSys = mainApp.GetSystem<RecordSystem>();
        bool isSoundOn = recordSys.GetSoundSwitch();
        if (isSoundOn) SoundOn();
        else SoundOff();

        PreLoadAllSound();
    }
    //-----------------------------------------------------------------------------------------------------
    public override void Update()
    {
        base.Update();
    }
    //-----------------------------------------------------------------------------------------------------
    private void CreatCommonSoundList()
    {
        m_commonSoundIDList.Add(1);     //1~4為戰鬥單擊音效
        m_commonSoundIDList.Add(2);
        m_commonSoundIDList.Add(3);
        m_commonSoundIDList.Add(4);
        m_commonSoundIDList.Add(5);     //進入下一個畫面的音效
        m_commonSoundIDList.Add(6);     //回到上一個畫面的音效
        m_commonSoundIDList.Add(7);     //選歌滑動音效
        m_commonSoundIDList.Add(8);     //難度選擇箭頭音效
        m_commonSoundIDList.Add(9);     //歌曲開始(進戰鬥)音效
        m_commonSoundIDList.Add(10);    //畫面結算音效
        m_commonSoundIDList.Add(11);    //拖拉點按音效(綠色點)
        m_commonSoundIDList.Add(12);    //雙擊點按音效(紅色點)
        m_commonSoundIDList.Add(13);    //13~16為滑動小點音效 
        m_commonSoundIDList.Add(14);
        m_commonSoundIDList.Add(15);
        m_commonSoundIDList.Add(16);    
        m_commonSoundIDList.Add(17);    //軒劍主題開頭動畫音效(從右下方一閃開始)   
        m_commonSoundIDList.Add(18);    //軒劍主題開頭動畫音效(從十神器出現開始)   
        m_commonSoundIDList.Add(19);    //軒劍主題開頭動畫音效(從軒轅劍閃入開始) 
        m_commonSoundIDList.Add(20);    //進入歌曲選單畫面
        m_commonSoundIDList.Add(21);    //結算畫面轉場(第一段滑入+perfect/good/weak/miss)
        m_commonSoundIDList.Add(22);    //結算畫面轉場(分數跑馬)
        m_commonSoundIDList.Add(23);    //結算畫面轉場(連擊跑馬)
        m_commonSoundIDList.Add(24);    //24~33為低音版戰鬥音效
        m_commonSoundIDList.Add(25);
        m_commonSoundIDList.Add(26);
        m_commonSoundIDList.Add(27);
        m_commonSoundIDList.Add(28);
        m_commonSoundIDList.Add(29);
        m_commonSoundIDList.Add(30);
        m_commonSoundIDList.Add(31);
        m_commonSoundIDList.Add(32);
        m_commonSoundIDList.Add(33);
    }
    //-----------------------------------------------------------------------------------------------------
    private AudioClip GetAudioClip(int soundID)
    {
        S_Sound_Tmp soundTmp = m_soundDataDB.GetData(soundID);
        if (soundTmp == null)
        {
            UnityDebugger.Debugger.LogWarning("SoundSystem GetAudioClip Failed! S_Sound_Tmp["+ soundID + "]= "+ soundTmp);
            return null;
        }
        AudioClip clip = m_resourceManager.GetResourceSync<AudioClip>(Enum_ResourcesType.Sound, soundTmp.strSoundName);
        return clip;
    }
    //-----------------------------------------------------------------------------------------------------
    /// <summary>預載所有遊戲中會用到的音效 </summary>
    private void PreLoadAllSound()
    {
        foreach (int id in m_commonSoundIDList)
        {
            GetAudioClip(id);
        }
    }
    //-----------------------------------------------------------------------------------------------------
    public void PlaySound(int soundID)
    {
        m_audioController.PlaySE(GetAudioClip(soundID));
    }
    public void PlaySound(int soundID, float delay)
    {
        m_audioController.PlaySE(GetAudioClip(soundID), delay);
    }
    public void PlayLoopSound(int soundID, bool forcePlay)
    {
        m_audioController.PlayLoopSE(GetAudioClip(soundID), forcePlay);
    }
    public void PlayLoopSound(int soundID, bool forcePlay,float delay)
    {
        m_audioController.PlayLoopSE(GetAudioClip(soundID), forcePlay, delay);
    }
    #region CommonSound
    //-----------------------------------------------------------------------------------------------------
    public void PlayBattleEndSound()
    {
        m_audioController.PlaySE(GetAudioClip(10));
    }
    #endregion
    #region BattleSound
    public void LoadGamePlaySound()
    {
        // 1~4單擊音效
        m_nativeSoundMap[1] = AudioCenter.loadSound("tambourine1.mp3");
        m_nativeSoundMap[2] = AudioCenter.loadSound("tambourine2.mp3");
        m_nativeSoundMap[3] = AudioCenter.loadSound("tambourine3.mp3");
        m_nativeSoundMap[4] = AudioCenter.loadSound("tambourine4.mp3");
        // 拖拉點按音效(綠色點)
        m_nativeSoundMap[11] = AudioCenter.loadSound("se-dragnote.mp3");
        // 雙擊點按音效(紅色點)
        m_nativeSoundMap[12] = AudioCenter.loadSound("se-doublenote.mp3");
        // 13~16滑移音效
        m_nativeSoundMap[13] = AudioCenter.loadSound("smallnote1.mp3");
        m_nativeSoundMap[14] = AudioCenter.loadSound("smallnote2.mp3");
        m_nativeSoundMap[15] = AudioCenter.loadSound("smallnote3.mp3");
        m_nativeSoundMap[16] = AudioCenter.loadSound("smallnote4.mp3");

        //======================================================================
        // LowVolume
        // 1~4單擊音效
        string strLowVol = "LowVolume/";
        m_nativeSoundMap[24] = AudioCenter.loadSound(strLowVol + "tambourine1.mp3");
        m_nativeSoundMap[25] = AudioCenter.loadSound(strLowVol + "tambourine2.mp3");
        m_nativeSoundMap[26] = AudioCenter.loadSound(strLowVol + "tambourine3.mp3");
        m_nativeSoundMap[27] = AudioCenter.loadSound(strLowVol + "tambourine4.mp3");
        // 拖拉點按音效(綠色點)
        m_nativeSoundMap[28] = AudioCenter.loadSound(strLowVol + "se-dragnote.mp3");
        // 雙擊點按音效(紅色點)
        m_nativeSoundMap[29] = AudioCenter.loadSound(strLowVol + "se-doublenote.mp3");
        // 13~16滑移音效
        m_nativeSoundMap[30] = AudioCenter.loadSound(strLowVol + "smallnote1.mp3");
        m_nativeSoundMap[31] = AudioCenter.loadSound(strLowVol + "smallnote2.mp3");
        m_nativeSoundMap[32] = AudioCenter.loadSound(strLowVol + "smallnote3.mp3");
        m_nativeSoundMap[33] = AudioCenter.loadSound(strLowVol + "smallnote4.mp3");
    }
    public void UnloadGamePlaySound()
    {
        foreach(KeyValuePair<int, int> kv in m_nativeSoundMap)
        {
            if(kv.Value != -1)
            {
                AudioCenter.unloadSound(kv.Value);
            }
        }
        m_nativeSoundMap.Clear();
    }
    //-----------------------------------------------------------------------------------------------------
    /// <summary>單擊音效</summary>
    public void PlayOneClickNoteSound(bool isLowVolume = false)
    {
        int randomID = (isLowVolume)? Random.Range(24, 27) : Random.Range(1, 4);
        //m_audioController.PlaySE(GetAudioClip(randomID));
        AudioCenter.playSound(m_nativeSoundMap[randomID], m_audioController.AllVolume);
    }
    //-----------------------------------------------------------------------------------------------------
    /// <summary>綠色點拖拉音效</summary>
    public void PlayDragNoteSound(bool isLowVolume = false)
    {
        int soundId = (isLowVolume)?28:11;
        //m_audioController.PlaySE(GetAudioClip(soundId));
        AudioCenter.playSound(m_nativeSoundMap[soundId], m_audioController.AllVolume);
    }
    //-----------------------------------------------------------------------------------------------------
    /// <summary>紅色點雙擊音效</summary>
    public void PlayDoubleClickNoteSound(bool isLowVolume = false)
    {
        int soundId = (isLowVolume) ? 29 : 12;
        //m_audioController.PlaySE(GetAudioClip(soundId));
        AudioCenter.playSound(m_nativeSoundMap[soundId], m_audioController.AllVolume);
    }
    //-----------------------------------------------------------------------------------------------------
    /// <summary>小點滑動音效</summary>
    public void PlaySmallNoteSlideSound(bool isLowVolume = false)
    {
        int randomID = (isLowVolume) ? Random.Range(30, 33) : Random.Range(13, 16);
        //m_audioController.PlaySE(GetAudioClip(randomID));
        AudioCenter.playSound(m_nativeSoundMap[randomID], m_audioController.AllVolume);
    }
    #endregion
    //-----------------------------------------------------------------------------------------------------
    public void SoundOn()
    {
        m_audioController.ChannelOn(EChannel.LoopSound);
        m_audioController.ChannelOn(EChannel.Sound);
    }
    //-----------------------------------------------------------------------------------------------------
    public void SoundOff()
    {
        m_audioController.ChannelOff(EChannel.LoopSound);
        m_audioController.ChannelOff(EChannel.Sound);
    }
}
