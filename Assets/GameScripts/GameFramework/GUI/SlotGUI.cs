using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotGUI : MonoBehaviour
{
    public List<EventDelegate.Callback> OnFadeInFinish;
    public List<EventDelegate.Callback> OnFadeOutFinish;
    //-----------------------------------------------------------------------------------------------------
    // 初始化       
    public virtual void Initialize(){}
    //-----------------------------------------------------------------------------------------------------
    // 顯示介面        
    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }
    //-----------------------------------------------------------------------------------------------------
    // 隱藏介面        
    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }
    //-----------------------------------------------------------------------------------------------------
    // 淡入介面        
    public void FadeIn()
    {
        TweenAlpha ta = null;
        ta = this.gameObject.GetComponent<TweenAlpha>();
        if (ta == null)
            ta = this.gameObject.AddComponent<TweenAlpha>();

        if (OnFadeInFinish != null)
        {
            foreach (EventDelegate.Callback callback in OnFadeInFinish)
            {
                EventDelegate.Add(ta.onFinished, callback, true);
            }
            OnFadeInFinish.Clear();
        }

        Show();

        ta.from = 0.0f;
        ta.to = 1.0f;
        ta.duration = 0.3f;
        ta.ResetToBeginning();
        ta.PlayForward();
    }
    //-----------------------------------------------------------------------------------------------------
    // 淡出介面        
    public void FadeOut()
    {
        TweenAlpha ta = null;
        ta = this.gameObject.GetComponent<TweenAlpha>();
        if (ta == null)
            ta = this.gameObject.AddComponent<TweenAlpha>();

        EventDelegate.Add(ta.onFinished, Hide, true);

        if (OnFadeOutFinish != null)
        {
            foreach (EventDelegate.Callback callback in OnFadeOutFinish)
            {
                EventDelegate.Add(ta.onFinished, callback, true);
            }
            OnFadeOutFinish.Clear();
        }

        ta.from = 1.0f;
        ta.to = 0.0f;
        ta.duration = 0.3f;
        ta.ResetToBeginning();
        ta.PlayForward();
    }
    //-----------------------------------------------------------------------------------------------------
    // 是否作用中                
    public bool IsVisible()
    {
        return this.gameObject.activeInHierarchy;
    }
}
