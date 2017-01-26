using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_Shop : NGUIChildGUI
{
    public Slot_Shop_MusicPage m_musicPage;
    public GameObject m_loadingUI;

    //Runtime Data
    private Dictionary<Enum_GoodsType, SlotGUI> m_storeTweenMap;
    //-------------------------------------------------------------------------------------------------
    private UI_Shop() : base() {}

    //-------------------------------------------------------------------------------------------------
    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        m_musicPage.Initialize();
        m_storeTweenMap = new Dictionary<Enum_GoodsType, SlotGUI>();
        m_storeTweenMap.Add(Enum_GoodsType.Music, m_musicPage);
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(StringTable st)
    {
        m_musicPage.InitializeUI(st);
    }
    //-------------------------------------------------------------------------------------------------
    public override void Show()
    {
        base.Show();
    }
    //-------------------------------------------------------------------------------------------------
    public override void Hide()
    {
        base.Hide();
    }
    //-------------------------------------------------------------------------------------------------
    public override void UiUpdate()
    {
        base.UiUpdate();
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchLoadingUI(bool bSwitch)
    {
        m_loadingUI.SetActive(bSwitch);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchShopPage(Enum_GoodsType type)
    {
        if (!m_storeTweenMap.ContainsKey(type))
            return;

        SlotGUI slotPage = m_storeTweenMap[type];
        foreach (var data in m_storeTweenMap)
        {
            if (data.Value.gameObject.activeInHierarchy && !data.Value.Equals(slotPage))
            {
                data.Value.OnFadeOutFinish.Add(slotPage.FadeIn);
                data.Value.FadeOut();
                break;
            }      
        }
    }
}
