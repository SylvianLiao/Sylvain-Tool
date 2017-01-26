using UnityEngine;
using System.Collections;
using Softstar.GamePacket;
using Softstar;

public class Slot_Goods : MonoBehaviour
{
    public int m_goodsGUID = -1;

    public UIButton m_buttonGoods;
    public UILabel m_labelGoodsName;
    public UILabel m_labelDisCount;
    public UILabel m_labelTag;
    //-------------------------------------------------------------------------------------------------
    private Slot_Goods() { }
    //-------------------------------------------------------------------------------------------------
    public void InitializeUI()
    {
    }
    //-------------------------------------------------------------------------------------------------
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    //-------------------------------------------------------------------------------------------------
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    //-------------------------------------------------------------------------------------------------
    public void SetData(StringTable st, ItemmallData data, string bgSpriteName)
    {
        m_labelGoodsName.text = st.GetString(data.iName) + "_" + data.GUID.ToString();
        m_labelDisCount.text = data.fDiscount.ToString() + st.GetString(356);   //"折"
        m_labelTag.text = st.GetString(data.iTag);
        SetBackground(bgSpriteName);

        m_goodsGUID = data.GUID;
    }
    //-------------------------------------------------------------------------------------------------
    public void ClearData()
    {
        m_labelGoodsName.text = "";
        m_labelDisCount.text = "";
        m_labelTag.text = "";
        m_goodsGUID = -1;
    }
    //-------------------------------------------------------------------------------------------------
    public void SetBackground(string bgSpriteName)
    {
        m_buttonGoods.normalSprite = bgSpriteName;
    }
}
