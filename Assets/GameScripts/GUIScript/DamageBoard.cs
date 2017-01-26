using UnityEngine;
using System.Collections;

public class DamageBoard : MonoBehaviour {

    public TweenScale m_TweenScale;

    public TweenAlpha m_TweenAlpha;

    public TweenPosition m_TweenPosition;

    public UILabel m_UILabel;

	public UISprite m_UIEffect;	// 特效

	public GameObject m_MyGameObject;

    public Transform m_MyTransform;

    // 從開啟到關閉的時間
    public float m_DisableTime = 1.5f;

    // 用來計算關閉時間的變數
    private float m_CurrentDisableTime;

    void Update()
    {
        //時間小於0就把物件關閉
        m_CurrentDisableTime -= Time.deltaTime;

        if (m_CurrentDisableTime <= 0)
            m_MyGameObject.SetActive(false);
    }

    // 設定要顯示的數值
    public void Show(Transform t, int value, Color c, int fontSize, int effectID)
    {
        m_MyGameObject.SetActive(true);
        m_CurrentDisableTime = m_DisableTime;

        m_UILabel.text = value.ToString();
	    m_UILabel.gradientTop = c;
	    m_UILabel.gradientBottom = c;
	    m_UILabel.fontSize = fontSize;

        Vector3 p = Camera.main.WorldToScreenPoint(t.position + t.up * 2.5f);
        p.x -= Screen.width / 2;
        p.y -= Screen.height / 2;
        m_MyTransform.localPosition = p;

        m_TweenScale.ResetToBeginning();
        m_TweenScale.PlayForward();

        m_TweenAlpha.ResetToBeginning();
        m_TweenAlpha.PlayForward();

        m_TweenPosition.from = p;
        m_TweenPosition.to = p + new Vector3(0, 150, 0);
        m_TweenPosition.ResetToBeginning();
        m_TweenPosition.PlayForward();

		Utility.ChangeAtlasSprite(m_UIEffect, effectID);
		m_UIEffect.gameObject.SetActive(effectID >0);
    }
}
