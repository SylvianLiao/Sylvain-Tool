using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WellFired;

[USequencerFriendlyName("PlayNumberPicker")]
[USequencerEvent("Softstar/ShowBattleResult/PlayNumberPicker")]
public class BattleResultEvent : USEventBase
{
    public UI_BattleResultAnim m_uiBattleResultAnim;
    public SoundSystem m_soundSystem;
    public bool PlayScore;

    // Use this for initialization
    public override void FireEvent()
    {
        if (m_uiBattleResultAnim == null)
            return;

        if (PlayScore)
        {
            m_uiBattleResultAnim.PlayScoreNumberPicker();
            if (m_soundSystem != null)
                m_soundSystem.PlaySound(22);
        }
        else
        {
            m_uiBattleResultAnim.PlayComboNumberPicker();
            if (m_soundSystem != null)
                m_soundSystem.PlaySound(23);
        }         
    }
    //---------------------------------------------------------------------------------------------------
    public override void ProcessEvent(float runningTime)
    {

    }
}

