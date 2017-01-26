using UnityEngine;
using AnimationOrTween;
using System.Collections;
using System.Collections.Generic;

public class M_UIPlayTween : MonoBehaviour
{
    
	public GameObject tweenTarget;
    public Trigger trigger = Trigger.OnActivate;
    private UITweener[] mTweens;


    void Start()
    {

    }


    void OnEnable()
    {
        
        if (trigger == Trigger.OnActivate)
            Play(true);
    }

    void OnClick()
    {
        if (trigger == Trigger.OnClick)
        {
            Play(true);
        }
    }

    void OnDoubleClick()
    {
        if (trigger == Trigger.OnDoubleClick)
        {
            Play(true);
        }
    }

    void Update()
    {
    }

    public void Play(bool forward)
    {

        GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;

        if (!NGUITools.GetActive(go))
        {
            // Enable the game object before tweening it
            NGUITools.SetActive(go, true);
        }

        // Gather the tweening components
        mTweens = go.GetComponentsInChildren<UITweener>();
        if (mTweens.Length > 0)
        {
            // Run through all located tween components
            for (int i = 0, imax = mTweens.Length; i < imax; ++i)
            {
                UITweener tw = mTweens[i];

                // Ensure that the game objects are enabled
                if (!NGUITools.GetActive(go))
                {
                    NGUITools.SetActive(go, true);
                }

                tw.ResetToBeginning();
                tw.Play(true);               
            }
        }
    }  
}
