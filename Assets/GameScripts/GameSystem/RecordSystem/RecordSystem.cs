using UnityEngine;
using System.Collections;
using Softstar;

public class RecordSystem : BaseSystem
{
    public RecordSystem(GameScripts.GameFramework.GameApplication app) : base(app) { }

    public override void Initialize()
    {
    }

    public override void Update()
    {
    }
    #region PlayerPrefs
    private void SetPlayerPrefsInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    private void SetPlayerPrefsFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
    private void SetPlayerPrefsString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    private int GetPlayerPrefsInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    private float GetPlayerPrefsFloat(string key, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    private string GetPlayerPrefsString(string key, string defaultValue = null)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public bool PlayerPrefsCheckHasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    #endregion

    #region Musci Listening
    //-------------------------------------------------------------------------------------------------
    public void SetAllSongChosenID(SongData songData)
    {
        int id = (songData == null) ? -1 : songData.SongGroupID;
        SetPlayerPrefsInt(GameDefine.SAVE_MUSIC_LISTENING_ALL_SONG_CHOSEN_ID, id);
    }
    //-------------------------------------------------------------------------------------------------
    public int GetAllSongChosenID()
    {
        string key = GameDefine.SAVE_MUSIC_LISTENING_ALL_SONG_CHOSEN_ID;
        if (!PlayerPrefsCheckHasKey(key))
            return -1;

        return GetPlayerPrefsInt(key);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetFavoriteChosenID(SongData songData)
    {
        int id = (songData == null) ? -1 : songData.SongGroupID;
        SetPlayerPrefsInt(GameDefine.SAVE_MUSIC_LISTENING_FAVORITE_CHOSEN_ID, id);
    }
    //-------------------------------------------------------------------------------------------------
    public int GetFavoriteChosenID()
    {
        string key = GameDefine.SAVE_MUSIC_LISTENING_FAVORITE_CHOSEN_ID;
        if (!PlayerPrefsCheckHasKey(key))
            return -1;

        return GetPlayerPrefsInt(key);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetSongIsFavorite(SongData songData, bool isFavorite)
    {
        string key = GameDefine.SAVE_MUSIC_LISTENING + songData.BelongThemeID + "_" + songData.SongGroupID;
        SetPlayerPrefsInt(key, (isFavorite) ? 1 : 0);
    }
    //-------------------------------------------------------------------------------------------------
    public bool GetSongIsFavorite(SongData songData)
    {
        string key = GameDefine.SAVE_MUSIC_LISTENING + songData.BelongThemeID + "_" + songData.SongGroupID;
        if (!PlayerPrefsCheckHasKey(key))
            return false;

        return (GetPlayerPrefsInt(key) == 1) ? true : false;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetMusicPlayMode(Enum_MusicPlayMode mode)
    {
        SetPlayerPrefsInt(GameDefine.SAVE_MUSIC_LISTENING_MODE, (int)mode);
    }
    //-------------------------------------------------------------------------------------------------
    public Enum_MusicPlayMode GetMusicPlayMode()
    {
        string key = GameDefine.SAVE_MUSIC_LISTENING_MODE;
        if (!PlayerPrefsCheckHasKey(key))
            return Enum_MusicPlayMode.None;

        return (Enum_MusicPlayMode)GetPlayerPrefsInt(key);
    }
    #endregion
    #region Shop
  
    #endregion
    #region Setting
    //-------------------------------------------------------------------------------------------------
    public void SetSoundSwitch(bool bSwitch)
    {
        string key = GameDefine.SAVE_SETTING_SOUND;
        SetPlayerPrefsInt(key, (bSwitch) ? 1 : 0);
    }
    //-------------------------------------------------------------------------------------------------
    public bool GetSoundSwitch()
    {
        string key = GameDefine.SAVE_SETTING_SOUND;
        if (!PlayerPrefsCheckHasKey(key))
            return true;
        
        return GetPlayerPrefsInt(key) == 1;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetNoteAdjust(float value)
    {
        string key = GameDefine.SAVE_SETTING_NOTE_ADJUST;
        SetPlayerPrefsFloat(key, value);
    }
    //-------------------------------------------------------------------------------------------------
    public float GetNoteAdjust()
    {
        string key = GameDefine.SAVE_SETTING_NOTE_ADJUST;
        if (!PlayerPrefsCheckHasKey(key))
            return 0.0f;

        return GetPlayerPrefsFloat(key);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetLanguage(ENUM_LanguageType lang)
    {
        string key = GameDefine.SAVE_SETTING_LANGUAGE;
        SetPlayerPrefsInt(key, (int)lang);
    }
    //-------------------------------------------------------------------------------------------------
    public ENUM_LanguageType GetLanguage()
    {
        string key = GameDefine.SAVE_SETTING_LANGUAGE;
        if (!PlayerPrefsCheckHasKey(key))
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.ChineseTraditional:
                case SystemLanguage.Chinese:
                    return ENUM_LanguageType.TraditionalChinese;
                case SystemLanguage.ChineseSimplified:
                    return ENUM_LanguageType.SimplifiedChinese;
                case SystemLanguage.English:
                    return ENUM_LanguageType.English;
                default:
                    return ENUM_LanguageType.TraditionalChinese;
            }
        }
        return (ENUM_LanguageType)GetPlayerPrefsInt(key);
    }
    #endregion

    #region Login
    //-------------------------------------------------------------------------------------------------
    public void SetPlayerLoginData(string value)
    {
        string key = GameDefine.SAVE_PLAYER_LOGIN_DATA;
        SetPlayerPrefsString(key, value);
    }
    //-------------------------------------------------------------------------------------------------
    public string GetPlayerLoginData()
    {
        string key = GameDefine.SAVE_PLAYER_LOGIN_DATA;
        return GetPlayerPrefsString(key, JsonUtility.ToJson(new PlayerLoginData()));
    }
    #endregion

    #region Player
    //-------------------------------------------------------------------------------------------------
    public void SetChoseSongGroupID(int groupID)
    {
        string key = GameDefine.SAVE_PLAYER_CHOSE_SONG;
        SetPlayerPrefsInt(key, groupID);
    }
    //-------------------------------------------------------------------------------------------------
    public int GetChoseSongGroupID()
    {
        string key = GameDefine.SAVE_PLAYER_CHOSE_SONG;
        return GetPlayerPrefsInt(key);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetIsGuideFinished(bool played)
    {
        string key = GameDefine.SAVE_PLAYER_NEW_GUIDE;
        SetPlayerPrefsInt(key, (played)?1:0);
    }
    //-------------------------------------------------------------------------------------------------
    public bool GetIsGuideFinished()
    {
        string key = GameDefine.SAVE_PLAYER_NEW_GUIDE;
        return (GetPlayerPrefsInt(key)==1)?true:false;
    }
    /*
    //-------------------------------------------------------------------------------------------------
    public void SetIsFirstTimeGuide(bool isFirst)
    {
        string key = GameDefine.SAVE_PLAYER_FIRST_TIME_GUIDE;
        SetPlayerPrefsInt(key, (isFirst) ? 1 : 0);
    }
    //-------------------------------------------------------------------------------------------------
    public bool GetIsFirstTimeGuide()
    {
        string key = GameDefine.SAVE_PLAYER_FIRST_TIME_GUIDE;
        return (GetPlayerPrefsInt(key, 1) == 1) ? true : false;
    }
    */
    //-------------------------------------------------------------------------------------------------
    public void SetIsSkipGameStartGuide(bool isFirst)
    {
        string key = GameDefine.SAVE_PLAYER_SKIP_GAME_START_GUIDE;
        SetPlayerPrefsInt(key, (isFirst) ? 1 : 0);
    }
    //-------------------------------------------------------------------------------------------------
    public bool GetIsSkipGameStartGuide()
    {
        string key = GameDefine.SAVE_PLAYER_SKIP_GAME_START_GUIDE;
        return (GetPlayerPrefsInt(key) == 1) ? true : false;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetIsSkipBattleGuide(bool isFirst)
    {
        string key = GameDefine.SAVE_PLAYER_SKIP_BATTLE_GUIDE;
        SetPlayerPrefsInt(key, (isFirst) ? 1 : 0);
    }
    //-------------------------------------------------------------------------------------------------
    public bool GetIsSkipBattleGuide()
    {
        string key = GameDefine.SAVE_PLAYER_SKIP_BATTLE_GUIDE;
        return (GetPlayerPrefsInt(key) == 1) ? true : false;
    }
    #endregion

    #region Offline Save Data
    public const string OFFLINE_SAVE_TITLE = "LSH_";
    public const string OFFLINE_SAVE_MAX_SCORE = "_Score";
    public const string OFFLINE_SAVE_MAX_COMBO = "_Combo";
    public const string OFFLINE_SAVE_MAX_RANK = "_Rank";
    public const string OFFLINE_SAVE_NORMAL_MONEY = "_NormalMoney";
    public const string OFFLINE_SAVE_MALL_MONEY = "_MallMoney";

    public void SetLocalPlayData(SongPlayData playData, SongData songData)
    {
        string key = OFFLINE_SAVE_TITLE + songData.SongGroupID + playData.SongDifficulty.ToString();
        if (GetMaxScore(songData, playData.SongDifficulty) < playData.Score)
            SetPlayerPrefsInt(key + OFFLINE_SAVE_MAX_SCORE, playData.Score);
        if (GetMaxCombo(songData, playData.SongDifficulty) < playData.Combo)
            SetPlayerPrefsInt(key + OFFLINE_SAVE_MAX_COMBO, playData.Combo);
        if ((int)GetRank(songData, playData.SongDifficulty) < (int)playData.Rank)
            SetPlayerPrefsInt(key + OFFLINE_SAVE_MAX_RANK, (int)playData.Rank);
    }
    public int GetMaxScore(SongData songData, Enum_SongDifficulty songDif)
    {
        string key = OFFLINE_SAVE_TITLE + songData.SongGroupID + songDif.ToString();
        return GetPlayerPrefsInt(key + OFFLINE_SAVE_MAX_SCORE);
    }
    public int GetMaxCombo(SongData songData, Enum_SongDifficulty songDif)
    {
        string key = OFFLINE_SAVE_TITLE + songData.SongGroupID + songDif.ToString();
        return GetPlayerPrefsInt(key + OFFLINE_SAVE_MAX_COMBO);
    }
    public Enum_SongRank GetRank(SongData songData, Enum_SongDifficulty songDif)
    {
        string key = OFFLINE_SAVE_TITLE + songData.SongGroupID + songDif.ToString();
        return (Enum_SongRank)GetPlayerPrefsInt(key + OFFLINE_SAVE_MAX_RANK);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetPlayerNormalMoney(int money)
    {
        string key = OFFLINE_SAVE_TITLE+OFFLINE_SAVE_NORMAL_MONEY;
        SetPlayerPrefsInt(key, money);
    }
    //-------------------------------------------------------------------------------------------------
    public int GetPlayerNormalMoney()
    {
        string key = OFFLINE_SAVE_TITLE + OFFLINE_SAVE_NORMAL_MONEY;
        return GetPlayerPrefsInt(key);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetPlayerMallMoney(int money)
    {
        string key = OFFLINE_SAVE_TITLE + OFFLINE_SAVE_MALL_MONEY;
        SetPlayerPrefsInt(key, money);
    }
    //-------------------------------------------------------------------------------------------------
    public int GetPlayerMallMoney()
    {
        string key = OFFLINE_SAVE_TITLE + OFFLINE_SAVE_MALL_MONEY;
        return GetPlayerPrefsInt(key);
    }
    #endregion

}
