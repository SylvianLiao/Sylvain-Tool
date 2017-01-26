using UnityEngine;
using System.Collections;

public class LineNoteObject : NoteObject
{
    public GameObject LineDirectionSprite;
    public GameObject PressB_Fbx;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    public virtual void SetLineDirectionSpriteDepth(int d)
    {
        UISprite sprite = LineDirectionSprite.GetComponent<UISprite>();
        sprite.depth = d;
    }

    public void SwitchPressBFbx(bool b)
    {
        PressB_Fbx.SetActive(b);
    }
}
