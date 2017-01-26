using UnityEngine;
using System.Collections;
using WellFired;
using Softstar;

public class UI_3D_Opening : UI3DChildGUI
{
    public Animator m_animator;
    public Animator m_animItem;
    public USSequencer m_sequencer;
    //-------------------------------------------------------------------------------------------------
    private UI_3D_Opening() : base()
    {
       
    }

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
    }
    //-------------------------------------------------------------------------------------------------
    public override void Hide()
    {
        base.Hide();
    }
    //-------------------------------------------------------------------------------------------------
    public override void UiUpdate()
    {
        base.UiUpdate();
    }
    //-------------------------------------------------------------------------------------------------
    public void SetTriggerEnterTheme()
    {
        m_animator.SetTrigger("EnterTheme");
    }
    //-------------------------------------------------------------------------------------------------
    public void SetTriggerEnterChooseSong()
    {
        m_animator.SetTrigger("EnterChooseSong");
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchChooseSongFlag(bool bSwitch)
    {
        m_animator.SetBool("EnterChooseSong", bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchThemeFlag(bool bSwitch)
    {
        m_animator.SetBool("EnterTheme", bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchChooseSongFinishFlag(bool bSwitch)
    {
        m_animator.SetBool("EnterChooseSongFinish", bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public bool GetChooseSongFinishFlag()
    {
        return m_animator.GetBool("EnterChooseSongFinish");
    }
    //---------------------------------------------------------------------------------------------------
    /// <summary>若不等一個Frame的時間，則無法從Login動畫直接切換置Theme動畫</summary>
    public IEnumerator PlayThemeOpening()
    {
        yield return null;
        //Play Animation
        SwitchThemeFlag(true);
        //Play sound
        m_sequencer.Play();
    }
}
