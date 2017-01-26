using UnityEngine;
using System.Collections;

public class BattleAnimBehavior : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        UI_3D_BattleBG ui3DBG = animator.GetComponent<UI_3D_BattleBG>();
        if (animatorStateInfo.IsName("ChooseDifficulty"))
        {
            ui3DBG.SwitchChooseDifficulty(false);
        }
        else if (animatorStateInfo.IsName("Start"))
        {
            ui3DBG.SwitchGameStart(false);
        }
        else if (animatorStateInfo.IsName("Pause"))
        {
            ui3DBG.SwitchResume(false);
        }
    }
}
