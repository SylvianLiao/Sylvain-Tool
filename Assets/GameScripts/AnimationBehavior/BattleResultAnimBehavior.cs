using UnityEngine;
using System.Collections;

public class BattleResultAnimBehavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        UI_BattleResultAnim uiBattle = animator.GetComponent<UI_BattleResultAnim>();
        if (animatorStateInfo.IsName("fin"))
        {
            if (uiBattle.OnAnimPlayFinish != null)
                uiBattle.OnAnimPlayFinish();
        }
    }
}
