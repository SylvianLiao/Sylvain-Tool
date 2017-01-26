using UnityEngine;
using System.Collections;

public class OpeningAnimBehavior : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        UI_3D_Opening ui3DOP = animator.GetComponent<UI_3D_Opening>();
        if (animatorStateInfo.IsName("EnterChooseSong") && animatorStateInfo.normalizedTime >= 0.99f)
        {
            ui3DOP.SwitchChooseSongFinishFlag(true);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        UI_3D_Opening ui3DOP = animator.GetComponent<UI_3D_Opening>();
        if (animatorStateInfo.IsName("Theme"))
        {
            ui3DOP.SwitchChooseSongFlag(false);
        }
        else if (animatorStateInfo.IsName("Login"))
        {
            ui3DOP.SwitchThemeFlag(false);
        }
    }
}
