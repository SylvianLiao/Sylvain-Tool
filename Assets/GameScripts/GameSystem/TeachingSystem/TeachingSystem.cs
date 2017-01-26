using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

//戰鬥教學的enum順序即為教學順序
public enum Enum_NewGuideType
{
    None = 0,
    ThemeGuide,
    ChooseSongGuide,
    ChooseDifficultyGuide,
    ChooseGuideType = 4,
    Adjust,
    Single = 6,
    LongPressA,
    LongPressB,
    Slide,
    Double,
    CrossSlide,
    PressSlideA,
    PressSlideB = 13,
    AllBattleGuide,
    Max,
}

public class TeachingSystem : BaseSystem
{
    private Enum_NewGuideType m_currentGuideType;
    public bool IsNewGuiding;

    private MainApplication m_mainApp;
    private RecordSystem m_recordSystem;
    private T_GameDB<S_NewGuide_Tmp> m_newGuideDB;
    private GUIManager m_gUIManager;

    private UI_GuideStep m_uiGuideStep;
    private S_NewGuide_Tmp m_newGuideTmp;
    private GameObject m_easyTouchGo;
    private CdTimer m_autoNextStepCdTimer;

    private Dictionary<Enum_NewGuideType, int> m_sortByGuideTypeMap;    //每個教學類型的第一個教學步驟
    private Dictionary<string, Enum_NewGuideType> m_sortByStateMap;     //特定狀態機的教學階段

    public TeachingSystem(GameScripts.GameFramework.GameApplication gameApplication) : base(gameApplication){}
    //-------------------------------------------------------------------------------------------------
    public override void Initialize()
    {
        m_mainApp = gameApplication as MainApplication;
        m_recordSystem = m_mainApp.GetSystem<RecordSystem>();
        m_newGuideDB = m_mainApp.GetGameDataDB().GetGameDB<S_NewGuide_Tmp>();
        m_gUIManager = m_mainApp.GetGUIManager();
        m_sortByGuideTypeMap = new Dictionary<Enum_NewGuideType, int>();
        m_sortByStateMap = new Dictionary<string, Enum_NewGuideType>();

        m_uiGuideStep = null;
        m_newGuideTmp = null;
        m_easyTouchGo = GameObject.Find("Easytouch");
        m_currentGuideType = Enum_NewGuideType.None;
        IsNewGuiding = false;
        m_autoNextStepCdTimer = null;

        SortNewGuideDbByType();
    }
    //-------------------------------------------------------------------------------------------------
    public override void Update()
    {
#if DEVELOP
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            m_recordSystem.SetIsGuideFinished(false);
            m_recordSystem.SetIsSkipBattleGuide(false);
            m_recordSystem.SetIsSkipGameStartGuide(false);
        }
#endif
    }
    //-------------------------------------------------------------------------------------------------
    private void AddCallBack()
    {
        AddNextGuideStepCallBack();
        UIEventListener.Get(m_uiGuideStep.m_buttonSkip.gameObject).onClick = OnButtonSkipClick;

        UIEventListener.Get(m_uiGuideStep.m_buttonChooseGuide.gameObject).onClick = OnButtonChooseGuideClick;
        UIEventListener.Get(m_uiGuideStep.m_buttonResume.gameObject).onClick = OnButtonResumeClick;
        UIEventListener.Get(m_uiGuideStep.m_buttonLeave.gameObject).onClick = OnButtonLeaveClick;
        for (int i = 0; i < m_uiGuideStep.m_buttonGuideTypes.Length; i++)
        {
            UIEventListener.Get(m_uiGuideStep.m_buttonGuideTypes[i].gameObject).onClick = OnButtonGuideTypeClick;
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void AddNextGuideStepCallBack()
    {
        UICamera.onClick = NextStep;
    }
    //-------------------------------------------------------------------------------------------------
    private void RemoveCallBack()
    {
        RemoveNextGuideStepCallBack();
        UIEventListener.Get(m_uiGuideStep.m_buttonSkip.gameObject).onClick = null;
        UIEventListener.Get(m_uiGuideStep.m_buttonChooseGuide.gameObject).onClick = null;
        UIEventListener.Get(m_uiGuideStep.m_buttonResume.gameObject).onClick = null;
        UIEventListener.Get(m_uiGuideStep.m_buttonLeave.gameObject).onClick = null;
        for (int i = 0; i < m_uiGuideStep.m_buttonGuideTypes.Length; i++)
        {
            UIEventListener.Get(m_uiGuideStep.m_buttonGuideTypes[i].gameObject).onClick = null;
        }
    }
    //-------------------------------------------------------------------------------------------------
    private static void RemoveNextGuideStepCallBack()
    {
        UICamera.onClick = null;
    }

    #region Initail Function
    /// <summary>以教學類型為key，重新排列新手教學資料</summary>---------------------------------------------
    private void SortNewGuideDbByType()
    {
        m_newGuideDB.ResetByOrder();
        for (int i = 0, iCount = m_newGuideDB.GetDataSize(); i < iCount; ++i)
        {
            S_NewGuide_Tmp guideTmp = m_newGuideDB.GetDataByOrder();
            if (m_sortByGuideTypeMap.ContainsKey(guideTmp.iType))
                continue;
            m_sortByGuideTypeMap.Add(guideTmp.iType, guideTmp.GUID);
        }
    }
    /// <summary>以State為key，重新排列新手教學資料</summary>---------------------------------------------
    public void SortNewGuideDbByState()
    {
        m_sortByStateMap.Clear();
        m_newGuideDB.ResetByOrder();
        for (int i = 0, iCount = m_newGuideDB.GetDataSize(); i < iCount; ++i)
        {
            S_NewGuide_Tmp guideTmp = m_newGuideDB.GetDataByOrder();
            CustomBehaviorState state = gameApplication.GetGameStateByName(guideTmp.strStateName) as CustomBehaviorState;
            if (state == null)
                continue;
            if (m_sortByStateMap.ContainsKey(guideTmp.strStateName))
                continue;

            Enum_NewGuideType type;
            //特例: 如果教學完成過，TUTORIAL_STATE對應的教學類型是Choose Guide Type，反之則為All Battle Guide
            if (guideTmp.strStateName == StateName.TUTORIAL_STATE)
                type = (CheckIsGuideFinished()) ? Enum_NewGuideType.ChooseGuideType : Enum_NewGuideType.AllBattleGuide;
            else
                type = guideTmp.iType;

            m_sortByStateMap.Add(guideTmp.strStateName, type);
        }
    }
    /// <summary>設定教學委派事件給狀態機</summary>---------------------------------------------
    public void SetGuideEventToState(bool isSetGameStartGuide)
    {
        foreach (var data in m_sortByStateMap)
        {
            CustomBehaviorState state = gameApplication.GetGameStateByName(data.Key) as CustomBehaviorState;
            if (state == null)
                continue;

            S_NewGuide_Tmp tmp = m_newGuideDB.GetData(m_sortByGuideTypeMap[data.Value]);
            //區分兩階段教學用的狀態機
            if (isSetGameStartGuide == CheckGuideIsWhichStep(tmp.iType))
                state.OnUIShowFinish += StartGuideTypeByState;
        }
    }
    /// <summary>生成教學UI</summary>---------------------------------------------
    private void CreateGuideUI()
    {
        m_uiGuideStep = m_gUIManager.AddGUI<UI_GuideStep>(typeof(UI_GuideStep).Name);
        m_uiGuideStep.InitialLabel(m_mainApp.GetStringTable());
        m_uiGuideStep.SwitchSkipButton(!CheckIsGuideFinished());
        m_uiGuideStep.Hide();
    }
    #endregion
    #region Guide Process
    /// <summary>開始新手教學</summary>---------------------------------------------
    public void NewGuideStart()
    {
        IsNewGuiding = true;
        SortNewGuideDbByState();
        CreateGuideUI();
        m_easyTouchGo.SetActive(false);
        UnityDebugger.Debugger.Log("-------------NewGuide Start");
    }
    /// <summary>切換新手教學</summary>---------------------------------------------
    public void StartGuideTypeByState()
    {
        if (!IsNewGuiding)
            return;

        CustomBehaviorState state = gameApplication.GetCurrentGameState() as CustomBehaviorState;
        if (state == null)
            return;
        m_newGuideTmp = m_newGuideDB.GetData(m_sortByGuideTypeMap[m_sortByStateMap[state.name]]);
        if (m_newGuideTmp == null)
            return;
        StartGuideType(m_newGuideTmp.iType);

        state.OnUIShowFinish -= StartGuideTypeByState;
    }
    /// <summary>切換新手教學</summary>---------------------------------------------
    public void StartGuideType(Enum_NewGuideType gudie)
    {
        if (!IsNewGuiding)
            return;

        m_currentGuideType = gudie;

        if (m_currentGuideType == Enum_NewGuideType.None)
            return;
        CustomBehaviorState state = gameApplication.GetCurrentGameState() as CustomBehaviorState;
        if (state == null)
            return;
        if (m_newGuideTmp == null)
            m_newGuideTmp = m_newGuideDB.GetData(m_sortByGuideTypeMap[m_currentGuideType]);

        state.RemoveCallBack();
        AddCallBack();

        m_mainApp.MusicApp.StartCoroutine(SetAndShowUIGuideStep());

        UnityDebugger.Debugger.Log("-------------ChangeGuideType: [" + m_currentGuideType + "], GuideGUID = "+ m_newGuideTmp.GUID);
    }
    /// <summary>下一個教學步驟</summary>---------------------------------------------
    public void NextStep(GameObject go)
    {
        if (!IsNewGuiding)
            return;
        //Skip
        if (go != null && go.Equals(m_uiGuideStep.m_buttonSkip.gameObject))
            return;
        //若點擊目標為四邊黑底則停止
        if (m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.ClickTarget && m_uiGuideStep.CheckClickFourBlackBG(go))
            return;
        //若點擊目標為全螢幕黑底BG則停止
        else if (m_uiGuideStep.CheckClickFullBlackBG(go))
            return;

        //恢復點擊監聽事件
        if (m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForPlayerClick ||
             m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForBattleGuideEnd ||
             m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForTime)
            AddNextGuideStepCallBack();

        m_mainApp.MusicApp.StartCoroutine(CheckAndSetNextGuide());
    }
    /// <summary>檢查是否有下一筆教學步驟資料並採取相對應動作</summary>---------------------------------------------
    private IEnumerator CheckAndSetNextGuide()
    {
        yield return null;
        if (m_newGuideTmp == null)
            yield break;

        //判斷玩家戰鬥教學的點擊結果
        int lastGuideGUID = m_newGuideTmp.GUID;
        if (lastGuideGUID == GameDefine.GUIDE_LAST_BATTLE_STEP_GUID)
        {
            m_newGuideTmp.iNextGUID = (m_uiGuideStep.IsPlayerClickSuccess) ? 0 : m_newGuideTmp.iNextGUID = m_sortByGuideTypeMap[m_currentGuideType];
        }
        S_NewGuide_Tmp tmp = m_newGuideDB.GetData(m_newGuideTmp.iNextGUID);
        if (tmp == null)
        {        
            int nextGuideIndex = (int)m_currentGuideType + 1;
            if (nextGuideIndex == (int)Enum_NewGuideType.Max)
                NewGuideOver(true);                                 //全部教學結束
            else 
            {
                if (nextGuideIndex == (int)Enum_NewGuideType.ChooseDifficultyGuide)
                    m_recordSystem.SetIsSkipGameStartGuide(true);   //開始教學階段結束
                if (!CheckIsGuideFinished() || (CheckIsGuideFinished() && m_currentGuideType == Enum_NewGuideType.Adjust))
                    CloseNewGuideStep();                                //關閉此階段教學
            }

            //處理短版戰鬥教學
            if (lastGuideGUID == GameDefine.GUIDE_LAST_BATTLE_STEP_GUID)
            {
                //若已完成過新手教學，每次單一戰鬥教學結束就回到選擇教學畫面
                if (CheckIsGuideFinished())
                    OnButtonChooseGuideClick(null);
                //下一個戰鬥教學
                else if (nextGuideIndex <= (int)Enum_NewGuideType.PressSlideB)
                    StartGuideType((Enum_NewGuideType)nextGuideIndex);
            }
            yield break;
        }
        else
            m_newGuideTmp = tmp;
        UnityDebugger.Debugger.Log("NewGuide Next Step, GuideGUID = " + m_newGuideTmp.GUID);

        //若下一個教學步驟仍在同個狀態下則顯示並繼續
        if (gameApplication.GetCurrentGameState().name.Equals(m_newGuideTmp.strStateName))
        {
            m_mainApp.MusicApp.StartCoroutine(SetAndShowUIGuideStep());
        }
        else
        {
            CloseNewGuideStep();
        }
    }
    /// <summary>關閉新手教學階段</summary>---------------------------------------------
    private void CloseNewGuideStep()
    {
        SomethingAboutStateCheck(true);

        StartGuideType(Enum_NewGuideType.None);

        m_uiGuideStep.Hide();
        RemoveCallBack();
        m_newGuideTmp = null;
        UnityDebugger.Debugger.Log("-------------NewGuide Close");
    }
    /// <summary>新手教學整個結束</summary>---------------------------------------------
    private void NewGuideOver(bool isFinish)
    {
        CloseNewGuideStep();
        m_gUIManager.DeleteGUI(typeof(UI_GuideStep).Name);
        m_easyTouchGo.SetActive(true);
        if (isFinish)
        {
            m_recordSystem.SetIsGuideFinished(true);
            ReturnToChooseDifficulty();
            /*
            Hashtable table = new Hashtable();
            table.Add(Enum_StateParam.DelayDeleteGUIName, new string[] { typeof(UI_Tutorial).Name });
            table.Add(GameDefine.FILEUPDATE_NEXTSTATE, StateName.CHOOSE_DIFFICULTY_STATE);
            m_mainApp.ChangeState(StateName.FILE_UPDATE_STATE, table);
            */
        }

        Initialize();
        UnityDebugger.Debugger.Log("-------------NewGuide Over");
    }
    #endregion

    /// <summary>確認是否使用State原有監聽功能</summary>---------------------------------------------
    private void SomethingAboutStateCheck(bool isUse)
    {
        if (m_newGuideTmp != null)
        {
            CustomBehaviorState gameState = gameApplication.GetGameStateByName(m_newGuideTmp.strStateName) as CustomBehaviorState;
            //檢查是否需要點擊觸發原有功能
            if ((!isUse && m_newGuideTmp.UseOriginalFunction) ||
                (isUse && !m_newGuideTmp.UseOriginalFunction))
                gameState.AddCallBack();
            else if ((!isUse && !m_newGuideTmp.UseOriginalFunction) ||
                (isUse && m_newGuideTmp.UseOriginalFunction))
                gameState.RemoveCallBack();
        }        
    }

    #region Inquire Function
    /// <summary>檢察欲顯示教學的UI是否準備好</summary>---------------------------------------------
    private bool CheckUIReady()
    {
        NGUIChildGUI ui = m_gUIManager.GetGUI(m_newGuideTmp.strUIName) as NGUIChildGUI;
        if (ui == null)
            return false;
        if (!ui.IsVisible())
            return false;
        return true;
    }
    /// <summary>檢查新手教學是否完成</summary>---------------------------------------------
    public bool CheckIsGuideFinished()
    {
        return m_recordSystem.GetIsGuideFinished();
    }
    /// <summary>檢查是否跳過開始遊戲的新手教學</summary>---------------------------------------------
    public bool CheckIsSkipGameStartGuide()
    {
        return m_recordSystem.GetIsSkipGameStartGuide();
    }
    /// <summary>檢查是否跳過戰鬥新手教學</summary>---------------------------------------------
    public bool CheckIsSkipBattleGuide()
    {
        return m_recordSystem.GetIsSkipBattleGuide();
    }
    /// <summary>檢查教學是哪一個階段(true = 開始遊戲 / false = 戰鬥)</summary>---------------------------------------------
    public bool CheckGuideIsWhichStep(Enum_NewGuideType type)
    {
        int guideIndex = (int)type;
        return (guideIndex > 0 && guideIndex < (int)Enum_NewGuideType.ChooseGuideType);
    }
    /// <summary>查詢目前教學類型</summary>---------------------------------------------
    public Enum_NewGuideType InquireCurrentGuideType()
    {
        return m_currentGuideType;
    }
    /// <summary>查詢目前教學類型的下一個步驟條件</summary>---------------------------------------------
    public Enum_GuideNextStepCondition InquireGuideNextStepCondition()
    {
        return m_newGuideTmp.iNextCondition;
    }
    #endregion
    #region Guide UI Setting

    /// <summary>設定並顯示教學UI</summary>---------------------------------------------
    private IEnumerator SetAndShowUIGuideStep()
    {
        SomethingAboutStateCheck(false);

        //若點擊事件為等待玩家點擊成功 或是 等待戰鬥教學結束 則移除點擊監聽事件
        if (m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForPlayerClick ||
            m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForBattleGuideEnd ||
            m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForTime)
            RemoveNextGuideStepCallBack();

        int count = 0;
        //等待欲教學的UI準備好
        while (!CheckUIReady())
        {
            if (count > 99999)
            {
                NewGuideOver(false);
                yield break;
            }
            count++;
            yield return null;
        }

        //處理教學目標
        List<TempGuideTarget> targets = null;
        if (m_newGuideTmp.iNextCondition != Enum_GuideNextStepCondition.WaitForPlayerClick)
        {
            targets = FindGuideTarget();
            if (targets == null)
            {
                UnityDebugger.Debugger.Log("FindGuideTarget() failed! NewGuide GUID = " + m_newGuideTmp.GUID);
                NewGuideOver(false);
                yield break;
            }
        }
        else if (m_currentGuideType == Enum_NewGuideType.ChooseGuideType)
        {
            OnButtonChooseGuideClick(null);
        }

        NotyifyStartBattleGuide();

        SetGuideChooseButton();
        m_uiGuideStep.Show();
        m_uiGuideStep.SetData(m_newGuideTmp, targets);

        //處理自動跳轉
        if (m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForTime)
        {
            CountDownSystem cdSys = m_mainApp.GetSystem<CountDownSystem>();
            m_autoNextStepCdTimer = cdSys.StartCountDown(GameDefine.TICKEY_GUIDE_WAIT_FOR_TIME_NEXT_STEP, m_newGuideTmp.fWaitSeconds, WaitForTimeAutoToNextStep);
        }
    }
    /// <summary>觸發戰鬥教學中的遊玩步驟</summary>---------------------------------------------
    private void NotyifyStartBattleGuide()
    {
        if (!m_newGuideTmp.StartBattleGuide)
            return;
        else if (!m_mainApp.GetCurrentGameState().name.Equals(StateName.TUTORIAL_STATE))
            return;

        if ((int)m_currentGuideType >= (int)Enum_NewGuideType.Single &&
            (int)m_currentGuideType <= (int)Enum_NewGuideType.AllBattleGuide)
        {
            bool isAuto = !(m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForPlayerClick);
            TutorialState ts = m_mainApp.GetGameStateByName(StateName.TUTORIAL_STATE) as TutorialState;
            m_mainApp.MusicApp.StartCoroutine(ts.StartBattleGuide(m_currentGuideType, isAuto));
        }
    }

    /// <summary>搜尋教學目標</summary>---------------------------------------------
    private List<TempGuideTarget> FindGuideTarget()
    {
        NGUIChildGUI ui = m_gUIManager.GetGUI(m_newGuideTmp.strUIName);
        if (ui == null)
            return null;
        //尋找教學目標
        GuideStep[] guideEvent = ui.transform.GetComponentsInChildren<GuideStep>();
        if (guideEvent == null)
            return null;
        //找出UI中GuideID相同的教學目標
        for (int i = 0; i < guideEvent.Length; ++i)
        {
            if (guideEvent[i].GuideGUID == m_newGuideTmp.GUID)
            {
                return guideEvent[i].GetGuideTarget();
            }
        }
        return null;
    }
    /// <summary>設定切換戰鬥教學按鈕的顯示</summary>---------------------------------------------
    private void SetGuideChooseButton()
    {
        bool isBattleGuide = ((int)m_currentGuideType >= (int)Enum_NewGuideType.Single &&
                            (int)m_currentGuideType <= (int)Enum_NewGuideType.PressSlideB &&
                            (m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForBattleGuideEnd ||
                            m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForPlayerClick));

        m_uiGuideStep.SwitchChooseGuideButton((isBattleGuide && CheckIsGuideFinished()));
    }
    #endregion

    #region Button Events
    /// <summary>跳過按鈕</summary>---------------------------------------------
    private void OnButtonSkipClick(GameObject go)
    {
        if (m_autoNextStepCdTimer != null)
            m_autoNextStepCdTimer.IsStart = false;
        RemoveNextGuideStepCallBack();
        m_mainApp.PushSystemCheckBox(318, 319, 41, SkipNewGuide, CancelSkipNewGuide, false);//"提醒","中斷教學將無法得到完成獎勵!"
    }
    /// <summary>跳過教學</summary>---------------------------------------------
    public void SkipNewGuide()
    {
        if (CheckGuideIsWhichStep(m_currentGuideType))
        {
            if (m_currentGuideType == Enum_NewGuideType.ChooseDifficultyGuide)
            {
                ChooseDifficultyState cdState = m_mainApp.GetGameStateByName(StateName.CHOOSE_DIFFICULTY_STATE) as ChooseDifficultyState;
                cdState.RemoveCallBack();
            }
            NewGuideOver(false);
            m_mainApp.PopState();
            m_recordSystem.SetIsSkipGameStartGuide(true);
        }
        else
        {
            m_mainApp.PopState();
            NewGuideOver(true);
            m_recordSystem.SetIsSkipBattleGuide(true);
        }

    }
    /// <summary>取消跳過教學</summary>---------------------------------------------
    public void CancelSkipNewGuide()
    {
        m_mainApp.PopState();

        if (m_newGuideTmp.iNextCondition != Enum_GuideNextStepCondition.WaitForPlayerClick &&
            m_newGuideTmp.iNextCondition != Enum_GuideNextStepCondition.WaitForBattleGuideEnd &&
            m_newGuideTmp.iNextCondition != Enum_GuideNextStepCondition.WaitForTime)
            AddNextGuideStepCallBack();
        else if (m_mainApp.GetCurrentGameState().name == StateName.TUTORIAL_STATE)
        {
            TutorialState ts = m_mainApp.GetCurrentGameState() as TutorialState;
            ts.ResumeBGM();
        }
        if (m_autoNextStepCdTimer != null)
            m_autoNextStepCdTimer.IsStart = true;

        SomethingAboutStateCheck(false);
    }

    //---------------------------------------------------------------------------------------------------
    private void OnButtonResumeClick(GameObject go)
    {
        m_uiGuideStep.SwitchChooseGuideUI(false);
        m_uiGuideStep.SwitchChooseGuideButton(true);
        m_uiGuideStep.SwitchBtnFullScreen(m_newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForBattleGuideEnd);
        TutorialState ts = m_mainApp.GetGameStateByName(StateName.TUTORIAL_STATE) as TutorialState;
        ts.ResumeBGM();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonLeaveClick(GameObject go)
    {
        if (m_mainApp.GetCurrentGameState().name != StateName.TUTORIAL_STATE)
            return;

        NewGuideOver(false);
        ReturnToStateWhichStartGuide();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonChooseGuideClick(GameObject go)
    {
        m_uiGuideStep.SwitchChooseGuideUI(true);
        m_uiGuideStep.SwitchBtnFullScreen(false);
        m_uiGuideStep.SwitchNoteBoard(false);
        m_uiGuideStep.SwitchFourBlackBG(false);
        m_uiGuideStep.SwitchFullBlackBG(false);

        TutorialState ts = m_mainApp.GetGameStateByName(StateName.TUTORIAL_STATE) as TutorialState;
        ts.PauseByChooseGuideUI = true;
        ts.PauseBGM();
    }
    //---------------------------------------------------------------------------------------------------
    private void OnButtonGuideTypeClick(GameObject go)
    {
        UIButton btn = go.GetComponent<UIButton>();
        if (btn == null)
            return;

        CloseNewGuideStep();
        m_uiGuideStep.SwitchChooseGuideUI(false);
        TutorialState ts = m_mainApp.GetCurrentGameState() as TutorialState;
        ts.SetIsSuspendByFullScreenState();
        ts.StopBGM();
        ts.PauseByChooseGuideUI = false;
        Enum_NewGuideType type = (Enum_NewGuideType)btn.userData;
        if (type == Enum_NewGuideType.Adjust)
        {
            Hashtable table = new Hashtable();
            table.Add(Enum_StateParam.LoadGUIAsync, true);
            m_mainApp.PushStateByScreenShot(StateName.ADJUST_STATE, table);
        }
        else
            StartGuideType(type);
    }
    #endregion

    #region Battle Guide
    /// <summary>玩家點擊音軌按鈕成功</summary>---------------------------------------------
    public void OnPlayerClickNoteSuccess()
    {
        m_uiGuideStep.IsPlayerClickSuccess = true;
        NextStep(null);
    }
    /// <summary>玩家點擊音軌按鈕失敗</summary>---------------------------------------------
    public void OnPlayerClickNoteFailed()
    {
        m_uiGuideStep.IsPlayerClickSuccess = false;
        NextStep(null);
    }
    /// <summary>自動跳轉時間到的監聽事件</summary>---------------------------------------------
    private void WaitForTimeAutoToNextStep(string ticket)
    {
        m_autoNextStepCdTimer = null;
        CountDownSystem cdSys = m_mainApp.GetSystem<CountDownSystem>();
        cdSys.CloseCountDown(ticket);
        NextStep(null);
        //UnityDebugger.Debugger.Log("New Guide Times Up ! current NewGuide Guid = "+m_newGuideTmp.GUID);
    }
    #endregion

    #region State Change
    /// <summary>回到選擇難度畫面</summary>---------------------------------------------
    private void ReturnToChooseDifficulty()
    {
        /*
        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.DelayDeleteGUIName, new string[] { typeof(UI_Tutorial).Name });
        table.Add(GameDefine.FILEUPDATE_NEXTSTATE, StateName.THEME_STATE);
        m_mainApp.ChangeState(StateName.FILE_UPDATE_STATE, table);
        */

        Hashtable table = new Hashtable();
        table.Add(Enum_StateParam.LoadGUIAsync, true);
        table.Add(Enum_StateParam.DelayDeleteGUIName, new string[] { typeof(UI_Tutorial).Name, typeof(UI_Adjust).Name });
        m_mainApp.ChangeStateByScreenShot(StateName.CHOOSE_DIFFICULTY_STATE, table);
    }
    /// <summary>回到啟動開始教學的狀態機</summary>---------------------------------------------
    private void ReturnToStateWhichStartGuide()
    {
        if (m_mainApp.GetCurrentGameState().name == StateName.ADJUST_STATE)
            m_mainApp.PopState();
        m_gUIManager.DeleteGUI(typeof(UI_Tutorial).Name);
        m_mainApp.PopState();
        if (m_mainApp.GetCurrentGameState().name == StateName.LOAD_GAME_STATE)
            m_mainApp.PopState();
    }
    #endregion

}
