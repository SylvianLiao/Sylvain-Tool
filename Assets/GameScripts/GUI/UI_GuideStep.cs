using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_GuideStep : NGUIChildGUI 
{
	public GameObject       m_gFullBlackBG		= null;
	public GameObject		m_gFourBlackBG		= null;
	public UISprite []		m_spFourBlackBG		= null;
	//
	public UIButton			m_buttonFullScreen	= null;
	public List<RealGuideTarget>   m_realGuideTargets		= null;
    public UIButton         m_buttonSkip        = null;
    public UILabel          m_labelSkip         = null;
    public UIButton         m_buttonChooseGuide = null;

    [Header("Choose Guide UI")]
    public UIButton m_buttonLeave;
    public UILabel m_labelLeave;
    public UIButton m_buttonResume;
    public UILabel m_labelResume;
    public UIButton[] m_buttonGuideTypes;

    //RuntimeData
    private StringTable m_stringTable;
    public bool IsPlayerClickSuccess;
    //-----------------------------------------------------------------------------------------------------
    private UI_GuideStep() : base(){}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize()
	{
		base.Initialize();
    }
    //-----------------------------------------------------------------------------------------------------
    public void InitialLabel(StringTable st)
    {
        if (m_stringTable == null)
            m_stringTable = st;

        m_labelSkip.text = st.GetString(321);    //跳過
        m_labelLeave.text = st.GetString(314);  //"離開教學"
        m_labelResume.text = st.GetString(313);  //"回到教學"
        for (int i = 0; i < m_buttonGuideTypes.Length; i++)
        {
            m_buttonGuideTypes[i].userData = (int)Enum_NewGuideType.Adjust + i;
            UILabel label = m_buttonGuideTypes[i].GetComponentInChildren<UILabel>();
            label.text = st.GetString(330 + i);   //各個新手戰鬥教學名稱
        }
    }
    //-------------------------------------------------------------------------------------------------
    public void SetData(S_NewGuide_Tmp newGuideTmp, List<TempGuideTarget> tempTargets)
	{
        //StartCoroutine(SetUIProcess(newGuideTmp, tempTargets));
        SetUI(newGuideTmp, tempTargets);
    }
    /*
	//-------------------------------------------------------------------------------------------------
	private IEnumerator SetUIProcess(S_NewGuide_Tmp newGuideTmp, List<TempGuideTarget> tempTargets)
    {
		SwitchFullBlackBG(newGuideTmp.iWaitSeconds > 0);
		if (newGuideTmp.iWaitSeconds > 0)
		{
            SwitchBtnFullScreen(false);
            SwitchNoteBoard(false);
            SwitchFourBlackBG(false);
        }

		yield return new WaitForSeconds(newGuideTmp.iWaitSeconds); 

        SetUI(newGuideTmp, tempTargets);
    }
    */
	//-------------------------------------------------------------------------------------------------
	//設定教學UI
	private void SetUI(S_NewGuide_Tmp newGuideTmp, List<TempGuideTarget> tempTargets)
	{
        if (newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.WaitForPlayerClick)
        {
            SwitchBtnFullScreen(false);
            SwitchNoteBoard(false);
            SwitchFourBlackBG(false);
            SwitchFullBlackBG(newGuideTmp.UseBlackBG);
            return;
        }

        //除了WaitForPlayerClick的狀況外都需要教學目標，故在此檢查
        if (tempTargets.Count != m_realGuideTargets.Count)
            return;

        for (int i = 0, iCount = tempTargets.Count; i < iCount; ++i)
        {
            RealGuideTarget realTarget = m_realGuideTargets[i];
            TempGuideTarget tempTarget = tempTargets[i];

            //設定全螢幕按鈕
            if (newGuideTmp.iNextCondition == Enum_GuideNextStepCondition.ClickAny && !newGuideTmp.UseOriginalFunction)
                SwitchBtnFullScreen(true);
            else
                SwitchBtnFullScreen(false);

            if (tempTarget.m_notePosition != Enum_GuideFramePosition.Center &&
                tempTarget.m_notePosition != Enum_GuideFramePosition.Center_Right)
            {
                Transform tempTargetOriginParent = tempTarget.transform.parent;
                //設定位置
                tempTarget.transform.parent = this.transform;
                realTarget.transform.localPosition = tempTarget.transform.localPosition;
                //設定大小
                realTarget.m_spriteGuideTarget.height = tempTarget.m_spriteGuideTarget.height;
                realTarget.m_spriteGuideTarget.width = tempTarget.m_spriteGuideTarget.width;

                tempTarget.transform.parent = tempTargetOriginParent;

                SwitchFullBlackBG(false);
                SwitchFourBlackBG(newGuideTmp.UseBlackBG); 
            }
            else
            {
                realTarget.transform.localPosition = Vector3.zero;
                SwitchFullBlackBG(newGuideTmp.UseBlackBG);
                SwitchFourBlackBG(false);
            }

            for (int m = 0, mCount = realTarget.m_gLabelPosition.Length;m < mCount; ++m)
            {
                GameObject posObj = realTarget.m_gLabelPosition[m];
                posObj.SetActive(false);
                Enum_GuideFramePosition guidePos = (Enum_GuideFramePosition)m;
                if (guidePos != tempTarget.m_notePosition)
                    continue;

                Transform trans = null;

                //取得說明文物件
                if (guidePos != Enum_GuideFramePosition.Center &&
                    guidePos != Enum_GuideFramePosition.Center_Right)
                    trans = posObj.transform.FindChild(RealGuideTarget.GUIDE_LABEL_NAME);
                else
                    trans = posObj.transform.FindChild(RealGuideTarget.GUIDE_CENTER_LABEL_NAME);
                if (trans == null)
                    continue;
                UILabel lbNote = trans.GetComponent<UILabel>();
                if (lbNote == null)
                    continue;

                //設定說明板內容
                lbNote.text = "";
                if (newGuideTmp.GUID == GameDefine.GUIDE_LAST_BATTLE_STEP_GUID)
                    lbNote.text = (IsPlayerClickSuccess)?m_stringTable.GetString(316): m_stringTable.GetString(317);
                else if (tempTarget.m_iNoteID > 0)
                    lbNote.text = m_stringTable.GetString(tempTarget.m_iNoteID);
 
                SwitchNoteBoard(!string.IsNullOrEmpty(lbNote.text));
                posObj.SetActive(true);
            }
        }
    }
	//-------------------------------------------------------------------------------------------------
	//根據教學目標設定黑底圖Anchor
	private void SetBackGroundAnchor(Transform trans)
	{
		if (m_spFourBlackBG == null)
			return;
		for(int i=0; i<m_spFourBlackBG.Length; ++i)
		{
			m_spFourBlackBG[i].SetAnchor(trans);
		}
	}
    //-------------------------------------------------------------------------------------------------
    public void SwitchBtnFullScreen(bool bSwitch)
	{
		m_buttonFullScreen.gameObject.SetActive(bSwitch);
	}
    //-------------------------------------------------------------------------------------------------
    public void SwitchFourBlackBG(bool bSwitch)
	{
		m_gFourBlackBG.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchFullBlackBG(bool bSwitch)
	{
		m_gFullBlackBG.gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchNoteBoard(bool bSwitch)
    {
        foreach (var board in m_realGuideTargets)
        {
            board.gameObject.SetActive(bSwitch);
        }
    }
    //-------------------------------------------------------------------------------------------------
    public bool CheckClickFourBlackBG(GameObject go)
	{
		for(int i=0; i<m_spFourBlackBG.Length; ++i)
		{
			if (go == m_spFourBlackBG[i].gameObject)
				return true;
		}
		return false;
	}
    //-------------------------------------------------------------------------------------------------
    public bool CheckClickBtnFullScreen(GameObject go)
	{
		if (go == m_buttonFullScreen.gameObject)
			return true;
		return false;
	}
	//-------------------------------------------------------------------------------------------------
	public bool CheckClickFullBlackBG(GameObject go)
	{
		if (go == m_gFullBlackBG.gameObject)
			return true;
		return false;
	}
    //-------------------------------------------------------------------------------------------------
    public void SwitchSkipButton(bool bSwitch)
    {
        m_buttonSkip.gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchChooseGuideButton(bool bSwitch)
    {
        m_buttonChooseGuide.gameObject.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchChooseGuideUI(bool bSwitch)
    {
        m_allPanelList[1].gameObject.SetActive(bSwitch);
        SwitchChooseGuideButton(!bSwitch);
    }
}
