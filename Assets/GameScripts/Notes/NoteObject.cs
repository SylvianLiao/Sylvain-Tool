using UnityEngine;
using System.Collections;

public abstract class NoteObject : MonoBehaviour
{
    public AbstractNote Note { set; get; }
    public GameLine Line { set; get; }
    public GameObject MarkObject;

    public virtual void SetPosition(float fTime)
    {
        Vector3 pos = Line.GetPosition(fTime);
        transform.position = pos;
    }

    public virtual void SetSpriteDepth(int d)
    {
        UISprite sprite = MarkObject.GetComponent<UISprite>();
        sprite.depth = d + 1;
    }

    public virtual void SetUpperMark()
    {
        UISprite sprite = MarkObject.GetComponent<UISprite>();
        if (sprite != null)
        {
            sprite.spriteName = "note_tg_up";
        }
    }
    public void SetLowerMark()
    {
        UISprite sprite = MarkObject.GetComponent<UISprite>();
        if (sprite != null)
        {
            sprite.spriteName = "note_tg_down";
        }
    }

    public void ClearMark()
    {
        UISprite sprite = MarkObject.GetComponent<UISprite>();
        if (sprite != null)
        {
            sprite.spriteName = "";
        }
    }

    public void ShowMark()
    {
        MarkObject.SetActive(true);
    }
    public void HideMark()
    {
        MarkObject.SetActive(false);
    }
}
