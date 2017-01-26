using UnityEngine;
using System.Collections;

public class DoubleFadeOut : MonoBehaviour {

    public UISprite NoteA = null;
    public UISprite NoteB = null;
    public LineRenderer LineR = null;
    public TweenAlpha twAlpha = null;
    public TweenRotation twRoataion = null;

    public bool bActive = false;
    
    private Vector3 v1;
    private Vector3 v2;
    private Color col = Color.white;
    void Start () {
        bActive = false;        
    }
		
	void Update ()
    {
        col.a = twAlpha.value;
        LineR.SetColors(col, col);
        v1 = NoteA.transform.position;
        v2 = NoteB.transform.position;
        LineR.SetPosition(0, new Vector3(v1.x, v1.y, -1.5f));
        LineR.SetPosition(1, new Vector3(v2.x, v2.y, -1.5f));
    }

    public void Init()
    {
        this.gameObject.SetActive(false);
        bActive = twRoataion.enabled = twAlpha.enabled = false;
        twAlpha.AddOnFinished(Release);
        twRoataion.to.z = Mathf.Abs(twRoataion.to.z);
    }

    public void StartUp(Vector3 vecA,Vector3 vecB,Vector3 vecCenter,int depth)
    {
        NoteA.depth = NoteB.depth = depth;

        Vector3 v = new Vector3((vecA.x + vecB.x) / 2, (vecA.y + vecB.y) / 2, vecA.z);
        if (v.x < vecCenter.x)
            twRoataion.to.z *= -1;

        this.gameObject.transform.position = v;
        NoteA.transform.position = vecA;
        NoteB.transform.position = vecB;

        this.gameObject.SetActive(true);
        bActive = true;
        twAlpha.enabled = true;
        twRoataion.enabled = true;
    }

    public void Release()
    {
        GameObject.Destroy(this.gameObject);
    }
}
