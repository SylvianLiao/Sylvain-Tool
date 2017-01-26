using UnityEngine;
using System.Collections;

namespace Softstar
{
    public class UI3DChildGUI : MonoBehaviour, NChildGUI
    {
        private string m_UIName;                // UI名稱(UIManager的key)
        private bool m_Initialized;
        //-----------------------------------------------------------------------------------------------------              
        public UI3DChildGUI()
        {
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
