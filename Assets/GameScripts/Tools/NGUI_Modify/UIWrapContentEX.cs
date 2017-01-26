using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWrapContentEX : UIWrapContent 
{
	public delegate void OnResetChildPosition();
	public OnResetChildPosition onResetChildPosition;
    //-----------------------------------------------------------------------------------------------------
    public List<Transform> GetChildren()
    {
        return mChildren;
    }
    //-----------------------------------------------------------------------------------------------------
    public void ResetChildPositionsEX() 
	{
        bool cacheScrllEnable = mScroll.enabled;
        //重置所有元件位置
        ResetChildPositions();

        //將SCROLLVIEW重置
        mScroll.enabled = true;
        mScroll.ResetPosition();
        mScroll.enabled = cacheScrllEnable;

        if (onResetChildPosition != null)
            onResetChildPosition();
	}
    //-----------------------------------------------------------------------------------------------------
    /// <summary>根據RealIndex將WrapContent移至適當的位置</summary>
    /// <param name="realIndex">通常是資料的Index</param>
    /// <param name="panelOriginPos">Panel原本的位置(若SCroll方向是縱向請給Y，橫向請給X)</param>
    /// <param name="panelOriginOffset">Panel原本的ClipOffset(若SCroll方向是縱向請給Y，橫向請給X)</param>
    public void MoveTo(int realIndex, float panelOriginPos, float panelOriginOffset)
    {
        //SpringPanel會自動滑移Panel的位置，故必須先關閉
        SpringPanel sp = mPanel.GetComponent<SpringPanel>();
        if (sp != null && sp.enabled)
            sp.enabled = false;

        realIndex = Mathf.Abs(realIndex);
        //塞資料的方向: 由上往下為遞減故*-1，反之遞增*1
        int direction = (minIndex < 0) ? -1 : 1;
        //根據資料數量算出模擬的總物件長度(+1是由於index 0也算一個物件)
        int totalLength = (itemSize * Mathf.Abs(maxIndex - minIndex + 1)) * direction;
        //欲移動至的絕對位置
        float moveDistance = realIndex * itemSize * direction;
        //Panel的可視範圍
        float panelViewSize = (mHorizontal) ? mPanel.GetViewSize().x : mPanel.GetViewSize().y;

        //判斷欲移動至的絕對位置是否超過底線
        if (Mathf.Abs(totalLength) - Mathf.Abs(moveDistance) < panelViewSize)
        {
            //有超過則移至最後面的位置即可
            moveDistance = totalLength - (panelViewSize * direction);
        }
        //計算出panel需移動至的位置 & offest需調整多少
        float panelMovePos = panelOriginPos + moveDistance * -1;    //panel位置調整值與offset相反，故*-1
        float panelMoveOffest = panelOriginOffset + moveDistance;

        //將計算完的結果輸入至Panel
        //移動Panel，WrapContent就會自動移動
        Vector3 vec3 = mPanel.transform.localPosition;
        Vector2 vec2 = new Vector2 (mPanel.clipOffset.x, mPanel.clipOffset.y);
        if (mHorizontal)
        {
            vec3.x = panelMovePos;
            vec2.x = panelMoveOffest;
        }
        else
        {
            vec3.y = panelMovePos;
            vec2.y = panelMoveOffest;
        }
        mPanel.transform.localPosition = vec3;
        mPanel.clipOffset = vec2;

        WrapContent();
    }
}
