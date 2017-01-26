using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class UI_DamageBoard : NGUIChildGUI
{
    public static UI_DamageBoard Instance;

    // smartObjectName
    private const string GUI_SMARTOBJECT_NAME = "UI_DamageBoard";

    private Transform m_Transform;

    // 用來複製的Prefab
    public GameObject m_DamageBoardPrefab;

    // 最多產生多少個DamageBoard
    public int m_DamageBoardCount = 10;

    // 存放DamageBoard的陣列
    private DamageBoard[] m_DamageBoardList;

	//-----------------------------------------------------------------------------------------------------
    private UI_DamageBoard()
        : base(GUI_SMARTOBJECT_NAME)
	{
	}

    void Awake() 
    {
        if (Instance == null)
            Instance = this;

        m_Transform = this.transform;

        if (m_DamageBoardPrefab == null)
            return;

        m_DamageBoardList = new DamageBoard[m_DamageBoardCount];

        //預先產生需要的數量
        for (int i = 0; i < m_DamageBoardList.Length; i++)
        {
            GameObject damageBoardObject = Instantiate(m_DamageBoardPrefab) as GameObject;
            damageBoardObject.transform.parent = m_Transform;
            m_DamageBoardList[i] = damageBoardObject.GetComponent<DamageBoard>();
        }
    }
    
    //------------------------------------------------------------------------------------
    // 顯示傷害數字
	public void ShowDamage(Transform t, int value, Color c, int fontSize, int effectID)
    {
        for (int i = 0; i < m_DamageBoardList.Length; i++)
        {
            if (m_DamageBoardList[i].m_MyGameObject.activeInHierarchy == false)
            {
				m_DamageBoardList[i].Show(t, value, c, fontSize, effectID);
                break;
            }
        }
    }
}