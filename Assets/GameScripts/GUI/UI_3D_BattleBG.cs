using UnityEngine;
using System.Collections;
using Softstar;
/*
using UnityEditor.Animations;
*/

public class UI_3D_BattleBG : UI3DChildGUI
{
    public Animator m_animator;
    /*
    void Start()
    {
        ChangeAnimatorContentForDevelop();
    }
     //For Develop-------------------------------------------------------------------------------------------------
    private void ChangeAnimatorContentForDevelop()
    {
        AnimatorController ac = m_animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        AnimatorControllerLayer layer = ac.layers[0];
        AnimatorStateMachine sm = layer.stateMachine;

        //Debug.Log("Layer " + i + " is: " + layer.name + " and has " + sm.states.Length + " states");

        for (int n = 0; n < sm.states.Length; n++)
        {
            AnimatorState state = sm.states[n].state;

            switch (state.name)
            {
                case "Combo10":
                    state.name = "Combo";
                    break;
                case "Combo30":
                    state.name = "Combo1";
                    break;
                case "Combo60":
                    state.name = "Combo2";
                    break;
                default:
                    break;
            }

            if (state.name == "Normal")
            {
                for (int j = 0; j < state.transitions.Length; j++)
                {
                    AnimatorStateTransition transition = state.transitions[j];
                    switch (transition.destinationState.name)
                    {
                        case "Combo10":
                        case "Combo30":
                        case "Combo60":
                        case "Combo":
                        case "Combo1":
                        case "Combo2":
                            state.RemoveTransition(transition);
                            break;
                        default:
                            break;
                    }

                    //Debug.Log("Transition: " + transition.name + " is " + transition.GetHashCode());
                }
            }
        }
    }
    */

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
  
    //-------------------------------------------------------------------------------------------------
    public void SwitchChooseDifficulty(bool isSelect)
    {
        m_animator.SetBool("ChooseDifficulty", isSelect);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchGameStart(bool isStart)
    {
        m_animator.SetBool("GameStart", isStart);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchGamePause(bool isPause)
    {
        m_animator.SetBool("GamePause" , isPause);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchResume(bool isResume)
    {
        m_animator.SetBool("Resume", isResume);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetMusicProgress(float progress)
    {
        m_animator.SetFloat("MusicProgress", progress);
    }
}
