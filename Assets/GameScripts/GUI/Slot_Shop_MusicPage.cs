using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class Slot_Shop_MusicPage : SlotGUI
{
    [Header("Left UI")]
    public UIButton m_buttonBuy;
    public UILabel m_labelGoodsName;
    public UILabel m_labelComposer;
    public UILabel m_labelComposerName;
    public UITexture m_textureGoods;
    public UILabel m_labelMoneyTitle;
    public UILabel m_labelMoney;

    [Header("Right UI")]
    public UIScrollView m_svGoodsMenu;
    public UIWrapContentEX m_wcGoodsMenu;
    public UISprite m_spriteHighLight;
    public UILabel m_labelNoGoods;

    //Const Value
    public const int m_iEachPageGoodsCount = 9;
    //RunTime Data
    public List<Slot_Goods> m_slotGoodsObjList;
    private float m_fWCPanelPosY;
    private float m_fWCPanelOffsetY;
    //-------------------------------------------------------------------------------------------------
    private Slot_Shop_MusicPage() : base(){}
    //-------------------------------------------------------------------------------------------------
    public override void Initialize()
    {
        m_slotGoodsObjList = new List<Slot_Goods>();
        m_spriteHighLight.gameObject.SetActive(false);
        SwitchNoGoodsLabel(false);
        m_fWCPanelPosY = m_svGoodsMenu.GetComponent<UIPanel>().transform.localPosition.y;
        m_fWCPanelOffsetY = m_svGoodsMenu.GetComponent<UIPanel>().clipOffset.y;
    }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI(StringTable st)
    {
        m_labelComposer.text = st.GetString(25);    //"作曲"
        m_labelMoneyTitle.text = st.GetString(342); //"鑽石"
        m_labelNoGoods.text = st.GetString(361);    //"目前無歌曲可販售"

        m_labelGoodsName.text = "";
        m_labelComposer.text = "";
        m_labelComposerName.text = "";
        m_labelMoney.text = "";
    }
    //-------------------------------------------------------------------------------------------------
    public void InitWrapContentValue(int dataCount)
    {
        m_wcGoodsMenu.minIndex = (dataCount - 1) * (-1);
        m_wcGoodsMenu.maxIndex = 0;

        m_svGoodsMenu.enabled = dataCount > m_iEachPageGoodsCount;
    }
    //-------------------------------------------------------------------------------------------------
    public void CreateSlotGoods(GameObject obj)
    {
        if (obj == null)
            return;

        for (int i = 0, iCount = (m_iEachPageGoodsCount + 2); i < iCount; ++i)
        {
            GameObject go = NGUITools.AddChild(m_svGoodsMenu.gameObject, obj);
            go.name = typeof(Slot_Goods).Name + "_" + i.ToString();
            Slot_Goods slot = go.GetComponent<Slot_Goods>();
            UIDragScrollView dsv = go.GetComponent<UIDragScrollView>();
            if (dsv != null)
                dsv.scrollView = m_svGoodsMenu;

            m_slotGoodsObjList.Add(slot);
        }
    }
    #region UI Setting
    //-------------------------------------------------------------------------------------------------
    public void SetGoodsName(string name)
    {
        m_labelGoodsName.text = name;
    }
    public void SetSongComposer(string composerName)
    {
        m_labelComposerName.text = composerName;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetGoodsPrice(int price)
    {
        m_labelMoney.text = price.ToString();
    }
    public void SetGoodsTexture(Texture texture)
    {
        m_textureGoods.mainTexture = texture;
    }
    #endregion
    //-------------------------------------------------------------------------------------------------
    public void SortWrapContent()
    {
        m_wcGoodsMenu.ResetChildPositionsEX();
    }
    //-------------------------------------------------------------------------------------------------
    public Slot_Goods GetSlotGoodsByGUID(int id)
    {
        for (int i = 0, iCount = m_slotGoodsObjList.Count; i < iCount; ++i)
        {
            Slot_Goods slotGoods = m_slotGoodsObjList[i];
            if (slotGoods.m_goodsGUID == id)
            {
                return slotGoods;
            }
        }
        return null;
    }
    //-------------------------------------------------------------------------------------------------
    public bool IsOwnHighLightSongObj(GameObject slotObj)
    {
        return (m_spriteHighLight.transform.parent.gameObject).Equals(slotObj);
    }
    //-------------------------------------------------------------------------------------------------
    public void MoveWrapContentTo(int realIndex)
    {
        m_wcGoodsMenu.MoveTo(realIndex, m_fWCPanelPosY, m_fWCPanelOffsetY);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchNoGoodsLabel(bool bSwitch)
    {
        m_labelNoGoods.gameObject.SetActive(bSwitch);
    }
}
