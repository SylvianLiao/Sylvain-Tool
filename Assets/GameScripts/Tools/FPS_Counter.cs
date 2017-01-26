using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FPS_Counter : MonoBehaviour 
{
    public static FPS_Counter Instance;

    private GUIStyle guiStyle;
    public float updateInterval = 0.5f;
    public int lockFPS = 0;
    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    private string fpsString;
    private float fps;

	public bool isShowFPS = false;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        timeleft = updateInterval;
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 25;
        if (lockFPS > 0)
            Application.targetFrameRate = lockFPS;
    }

    // Update is called once per frame
    void Update()
    {
		UpdateInput();

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.smoothDeltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            fps = (accum / frames);
            fpsString = fps.ToString("#,##0.0 fps");
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

	void UpdateInput()
	{
#if (UNITY_EDITOR)
		if(Input.GetKeyDown(KeyCode.F12))
		{
			isShowFPS = !isShowFPS;
        }
#endif
    }

    void OnGUI()
    {
#if DEVELOP
        if (isShowFPS)
            GUI.Label(new Rect(10, 5, 50, 20), fpsString, guiStyle);
#endif
    }

    public float GetFPS()
    {
        return fps;
    }
}