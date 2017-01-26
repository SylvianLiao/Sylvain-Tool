using UnityEngine;
using System.Collections;

public class ShowDrawCall : MonoBehaviour
{
    public enum ObjectType
    {
        NGUI,
        GameObject,
    }

    private ObjectType m_ObjectType = ObjectType.GameObject;
    public bool m_ShowDrawCall = false;
    // Use this for initialization
    void FixedUpdate()
    {
        ShowDrawCalls();
    }

    private void ShowDrawCalls()
    {
        if (!m_ShowDrawCall)
            return;
        m_ShowDrawCall = false;
        switch (m_ObjectType)
        {
            case ObjectType.GameObject:
                Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    int queue = renderer.material.renderQueue;
                    UnityDebugger.Debugger.Log(renderer.name + " renderQueue = " + queue);
                }
                break;
        }
    }
    
}
