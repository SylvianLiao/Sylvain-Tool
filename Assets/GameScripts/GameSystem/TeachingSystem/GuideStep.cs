using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideStep : MonoBehaviour 
{
	public int		GuideGUID = 0;
    private List<TempGuideTarget> m_guideTargetsList;

    private void Start()
    {
        m_guideTargetsList = new List<TempGuideTarget>();
        TempGuideTarget[] targetArray = GetComponentsInChildren<TempGuideTarget>();
        m_guideTargetsList.AddRange(targetArray);
    }

    public List<TempGuideTarget> GetGuideTarget()
	{
		return m_guideTargetsList;
	}
}
