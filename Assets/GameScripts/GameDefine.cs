
public class GameDefine
{
    public const int MUSCIC_LISTENING_LOADER_MAX = 5;
    public const int SHOP_MUISC_PAGE_LOADER_MAX = 5;

    public const float NOTE_TRIGGER_TIME = 0.60786f;
    public const float NOTE_DESTROY_TIME = 1.1f;
    public const float NOTE_SCORE_BOUND = 0.75f;
    public const float NOTE_INIT_SIZE = 0.3f;
    public const float NOTE_PRESSB_EndTIME = 0.63286f;
    public const float NOTE_NOTHANDLE_BOUND = 0.3f;

    // 點擊點的出現時間
    public const float NOTE_FADE_IN_TIME = 0.4f;
    // 點擊點的消失時間
    public const float NOTE_FADE_OUT_TIME = 0.7f;

    // 線段夾角度數
    public const float LINE_RADIAN = 10.35f; // 每條線夾角10.35度
    public const float LINE_RADIAN_DIFF = 3.14159f / 180.0f * LINE_RADIAN; // 轉換為徑度
    // 線段共通起始座標(World position)
    public const float LINE_COMMON_START_X = 0.0f;
    public const float LINE_COMMON_START_Y = 2.54f;
    // 旋轉點座標(world position)
    public const float LINE_ROTATE_X = 0.0f;
    public const float LINE_ROTATE_Y = 0.95f;
    // 線段長(下方弧線的Note線長度)
    public const float LINE_LOWER_PART_LEN = 5.42f;
    // 線段長(上方弧線的Note線長度)
    public const float LINE_UPPER_PART_LEN = 4.59f;

    // 遊戲線段beats數量
    public const float DEFAULT_FULL_LINE_BEATS = 6.0f;
    // 控制遊戲Timing
    public const float DEFAULT_PLAY_SPEED = 0.7f;
    // 遊戲Combo修正參數
    public const float DEFAULT_COMBO_PARAMETER = 50f;

    // Particle Scale
    public const float PARTICLE_MULTIPLY = 1.0f;

    //Combo動態背景演出條件
    public const int COMBO_EFFECT_CONDITION_1 = 30;
    public const int COMBO_EFFECT_CONDITION_2 = 60;
    public const int COMBO_EFFECT_CONDITION_3 = 100;
    //戰鬥動態背景Combo動畫狀態名稱
    public const string COMBO_ANIMATION_STATE_NAME_1 = "Combo";
    public const string COMBO_ANIMATION_STATE_NAME_2 = "Combo1";
    public const string COMBO_ANIMATION_STATE_NAME_3 = "Combo2";
    // Combo Mod數值
    public const int COMBO_MOD = 10;
    // 觸碰點Depth初始值
    public const int DEFAULT_HITPOINT_DEPTH = 1000;
    // 演出NOTE Depth值
    public const int DEFAULT_NOTE_DEPTH = 1050;
    // HitJudgeIcon Depth值
    public const int DEFAULT_JudgeIcon_DEPTH = 1100;

    // 觸碰點Sprite大小(區分主線段及補助線段，上層線段全為小Size)
    public const int DEFAULT_NOTE_SCALE_BIG = 110;
    public const int DEFAULT_NOTE_SCALE_SMALL = 90;

    //note上下線段大小
    public const int DEFAULT_NOTE_SCALE_UP = 90;
    public const int DEFAULT_NOTE_SCALE_Low = 100;

    //計時器Ticket
    public const string CDTIMER_ADJUST_TICKET = "AdjustPressTime";

    //長壓時間
    public const float PRESS_TIME = 0.2f;
    //Client端存檔用Key-----------------------------------------------------------------
    public const string SAVE_SETTING = "Setting_";
    public const string SAVE_SETTING_NOTE_ADJUST = SAVE_SETTING+"Note_Adjust";                              //本地端校正時間Key
    public const string SAVE_SETTING_SOUND = SAVE_SETTING+"Sound";                                          //音效開關key
    public const string SAVE_SETTING_LANGUAGE = SAVE_SETTING+"Language";                                    //語言key
    public const string SAVE_MUSIC_LISTENING = "Music_Listening_";                                          //音樂聆聽用開頭Key
    public const string SAVE_MUSIC_LISTENING_ALL_SONG_CHOSEN_ID = SAVE_MUSIC_LISTENING+"All_Song_Chosen";   //音樂聆聽所有歌曲選單中選取曲目Key
    public const string SAVE_MUSIC_LISTENING_FAVORITE_CHOSEN_ID = SAVE_MUSIC_LISTENING+"Favorite_Chosen";   //音樂聆聽最愛曲目選單中選取曲目Key
    public const string SAVE_MUSIC_LISTENING_MODE = SAVE_MUSIC_LISTENING+"Mode";                            //音樂聆聽播放模式
    public const string SAVE_PLAYER_LOGIN_DATA = "UserLoginData";                                           //玩家登入資料
    public const string SAVE_PLAYER_CHOSE_SONG = "PlayerChoseSong";                                         //玩家離開遊戲時停留的歌曲
    public const string SAVE_PLAYER_NEW_GUIDE = "PlayerNewGuide";                                           //玩家是否玩過新手教學
    public const string SAVE_PLAYER_FIRST_TIME_GUIDE = "PlayerFirstTimeGuide";                              //玩家是否玩第一次新手教學
    public const string SAVE_PLAYER_SKIP_GAME_START_GUIDE = "PlayerSkipGameStartGuide";                     //玩家是否玩跳過開始遊戲的教學
    public const string SAVE_PLAYER_SKIP_BATTLE_GUIDE = "PlayerSkipBattleGuide";                            //玩家是否玩跳過戰鬥教學
    //---------------------------------------------------------------------------------------------------
    //CheckBox HashTable Key
    public const string CHECK_BOX_TITLE_KEY = "Title";
    public const string CHECK_BOX_CONTENT_TITLE_KEY = "ContentTitle";
    public const string CHECK_BOX_CONTENT_KEY = "Content";
    public const string CHECK_BOX_OK_KEY = "OK";
    public const string CHECK_BOX_OK_EVENT_KEY = "OKEvent";
    public const string CHECK_BOX_CANCEL_EVENT_KEY = "CancelEvent";
    public const string CHECK_BOX_PARAM_KEY = "Param";
    public const string CHECK_BOX_OK_EVENT_PARAM_KEY = "OKEvent_Param";
    public const string CHECK_BOX_CANCEL_EVENT_PARAM_KEY = "CancelEvent_Param";
    public const string CHECK_BOX_ICON_ID_KEY = "IconID";
    public const string CHECK_BOX_IS_AUTO_POP_KEY = "IsAutoPop";
    //---------------------------------------------------------------------------------------------------
    public const string TICEKT_UPDATE_SCORE = "UpdateScore";
  
    //新手教學用---------------------------------------------------------
    public const int GUIDE_LAST_BATTLE_STEP_GUID = 60;                                  //戰鬥教學的最後一個步驟的guid
    public const string TICKEY_GUIDE_WAIT_FOR_TIME_NEXT_STEP = "GuideWaitForTime";      //教學註冊自動跳轉時間倒數的Key
    //---------------------------------------------------------------------------------------------------
    public const string FILEUPDATE_NEXTSTATE = "NextState"; //FileUpdate完成後欲跳轉頁面之名稱
}

#region Enum
public enum Enum_SongRank
{
    New = 0,
    D,
    C,
    B,
    A,
    S,
    G,
}
//-----------------------------------------------------------------------------------------------------------
public enum Enum_SongDifficulty
{
    Easy = 0,
    Normal,
    Hard,
    Max
}
//-----------------------------------------------------------------------------------------------------------
public enum Enum_StarStatus
{
   None = 0,
   Half,
   Full
}
//-----------------------------------------------------------------------------------------------------------
public enum NoteLineType : byte
{
    VerticalNote = 0,
    DoubleNote = 1,
    DrageNote = 2,
    VerticalNotePressB = 4,
}
//-----------------------------------------------------------------------------------------------------------
public enum Enum_CurrencyType
{
    Money = 0,
    Diamond = 1,
    TWD = 3,
}
//-----------------------------------------------------------------------------------------------------------
public enum emLineOrder
{
    L3 = 0, L2a, L2, L1a, L1, LR, R1, R1a, R2, R2a, R3,
    S_L3, S_L2a, S_L2, S_L1a, S_L1, S_LR, S_R1, S_R1a, S_R2, S_R2a, S_R3,
}
//-----------------------------------------------------------------------------------------------------------
public enum Enum_TimeFormat
{
    Second,
    Minute,
    Hour
}
//-----------------------------------------------------------------------------------------------------------
public enum ArcLine
{
    None,  // 尚未開始
    Lower, // 下段弧線
    Upper, // 上段弧線
    Both // 上下兩段弧線
}
//-----------------------------------------------------------------------------------------------------------
public enum Enum_SongLockStatus
{
    Lock,
    Unlock,
    WaitForUnlock,
}
//-----------------------------------------------------------------------------------------------------------
public enum Enum_DifficultyLockStatus
{
    Lock = 0,
    TrueUnlock,
    TempUnlock,
}
//----------------------------------------------------------------------------------
//教學說明板位置
public enum Enum_GuideFramePosition
{
    Top = 0,
    RightTop,
    Right,
    RightBottom,
    Bottom,
    LeftBottom,
    Left,
    LeftTop,
    Center,
    Center_Right,
    Max,
}
//----------------------------------------------------------------------------------
//教學點擊事件
public enum Enum_GuideNextStepCondition
{
    ClickTarget = 0,
    ClickAny,
    WaitForBattleGuideEnd,
    WaitForPlayerClick,
    WaitForTime,
    Max,
}
#endregion

