using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerData
{
    private string m_strName;
    public string Name
    {
        get { return m_strName; }
        set { m_strName = value; }
    }
    private int m_iDiamond;
    public int Diamond
    {
        get { return m_iDiamond; }
        set { m_iDiamond = value; }
    }

    public PlayerData()
    {
        m_strName = "";
        m_iDiamond = 0;

    }
    public PlayerData(string name)
    {
        m_strName = name;
    }
    //---------------------------------------------------------------------------------
}
