using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISprite))]
public class RealGuideTarget : MonoBehaviour
{
    public UISprite m_spriteGuideTarget;
    public GameObject[] m_gLabelPosition;
    //-------------------------------------------------------------------------------------------------
    public const string GUIDE_FRAME_NAME = "Sprite(GuideFrame)";
    public const string GUIDE_LABEL_NAME = "Sprite(GuideFrame)/Label(Explanation)";
    public const string GUIDE_CENTER_LABEL_NAME = "Label(Explanation)";
}
