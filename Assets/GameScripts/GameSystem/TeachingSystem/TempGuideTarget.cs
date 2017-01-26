using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISprite))]
public class TempGuideTarget : MonoBehaviour
{
    public UISprite m_spriteGuideTarget;
    public int m_iNoteID;
    public Enum_GuideFramePosition m_notePosition;
}
