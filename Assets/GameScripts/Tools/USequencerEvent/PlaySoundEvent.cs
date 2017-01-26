using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WellFired;

[USequencerFriendlyName("播放音效")]
[USequencerEvent("Softstar/播放音樂音效/播放音效")]
public class PlaySoundEvent : USEventBase
{
	public int	SoundID;
    public bool Loop = false;
    public bool ForcePlay = false;
    private SoundSystem m_soundSystem;

    public override void FireEvent()
	{
        if (m_soundSystem == null)
        {
            try
            {
                GetSoundSystem();
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return;
            }
        }      

        if (Loop)
        {
            m_soundSystem.PlayLoopSound(SoundID, ForcePlay);
        }
        else
        {
            m_soundSystem.PlaySound(SoundID);
        }
	}
    //---------------------------------------------------------------------------------------------------
    public override void ProcessEvent(float runningTime)
	{

	}
    //---------------------------------------------------------------------------------------------------
    private void GetSoundSystem()
    {
        MusicApplication app = GameObject.FindObjectOfType<MusicApplication>();
        m_soundSystem = app.APP.GetSystem<SoundSystem>();
    }
}

