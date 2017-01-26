using UnityEngine;
using System.Collections;

namespace Softstar
{
    // 子GUI的interface    
    public interface NChildGUI
    {
        string GetUIName();
        void SetUIName(string UIName);
        void Initialize();
        void UiUpdate();
        void Show();
        void Hide();
        void GUIDestroy();
        bool IsInitialize();
        bool IsVisible();
    }
}

