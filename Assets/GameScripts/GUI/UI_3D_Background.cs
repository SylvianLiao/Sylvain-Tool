using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

public class UI_3D_Background : UI3DChildGUI
{
    public MeshRenderer m_meshBackground;
    public GameObject m_lockBackground;

    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
    }
    //-------------------------------------------------------------------------------------------------
    public void SetBackground(string matPath)
    {
        Softstar.Utility.ChangeMaterial(m_meshBackground, matPath);
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchLockBackground(bool bSwtich)
    {
        m_lockBackground.SetActive(bSwtich);
    }
}
