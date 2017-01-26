using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Softstar
{
	//-----------------------------------------------------------------------------------------------------
    // for NGUI的基本子GUI    
    public abstract class NGUIChildGUI : MonoBehaviour, NChildGUI
    {
        private string m_UIName;                // UI名稱(UIManager的key)
        private bool m_Initialized;

        public List<UIPanel> m_allPanelList;

        public List<EventDelegate.Callback> OnFadeInFinish;
        public List<EventDelegate.Callback> OnFadeOutFinish;
        //-----------------------------------------------------------------------------------------------------              
        public NGUIChildGUI()
        {
            OnFadeInFinish = new List<EventDelegate.Callback>();
            OnFadeOutFinish = new List<EventDelegate.Callback>();
            m_allPanelList = new List<UIPanel>();
        }
		//-----------------------------------------------------------------------------------------------------
        // 初始化       
        public virtual void Initialize()
        {
            m_Initialized = true;
			Hide();
        }
		//-----------------------------------------------------------------------------------------------------
        public bool IsInitialize()
		{
			return m_Initialized;
		}
        //-----------------------------------------------------------------------------------------------------        
        public virtual void UiUpdate()
        {
        }
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
        public void StopFade()
        {
            TweenAlpha ta = this.gameObject.GetComponent<TweenAlpha>();
            if (ta == null)
                return;

            ta.onFinished.Clear();
            ta.enabled = false;
            ta.ResetToBeginning();
        }
        //-----------------------------------------------------------------------------------------------------
        // 刪除介面        
        public virtual void GUIDestroy()
        {
            GameObject.Destroy(gameObject);
        }
		//-----------------------------------------------------------------------------------------------------
        public virtual string GetUIName()
        {
            return m_UIName;
        }
		//-----------------------------------------------------------------------------------------------------
        public virtual void SetUIName(string UIName)
        {
            m_UIName = UIName;
        }
        //-----------------------------------------------------------------------------------------------------
        // 是否作用中                
        public bool IsVisible()
        {
            return this.gameObject.activeInHierarchy;
        }
    }
}