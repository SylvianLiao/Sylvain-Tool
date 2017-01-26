using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
 
public class UI_WebBulletinBoard : NGUIChildGUI
{
	private const string GUI_SMARTOBJECT_NAME = "UI_WebBulletinBoard";
	public string url;				// 公告url
	public UIButton 	CloseButton;
	public UITexture	Background;
	UniWebView webview;
	public int left,right,top,bottom;
	public UISprite 	Waiting;
	public UITexture 	Texture2;
	public UIPanel 		uiPanel;
	//-------------------------------------------------------------
	private UI_WebBulletinBoard()
		: base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-------------------------------------------------------------
	void Start()
	{
		StartCoroutine (OpenURL());
	}
	//-------------------------------------------------------------
	IEnumerator OpenURL()
	{
		yield return new WaitForSeconds (0.5f);
		webview.Load(url);
		Waiting.gameObject.SetActive(true);
	}
	//-------------------------------------------------------------
	public override void Initialize ()
	{
		base.Initialize ();
		GameObject view = new GameObject("WebView");

		view.transform.parent = transform;
		view.transform.localPosition = Vector3.zero;
		view.transform.localRotation = Quaternion.identity;

		webview = view.GetComponent<UniWebView>();
		if (webview == null) 
		{
			webview = view.AddComponent<UniWebView>();
			webview.OnReceivedMessage += OnReceivedMessage;
			webview.OnLoadComplete += OnLoadComplete;
			webview.OnWebViewShouldClose += OnWebViewShouldClose;
			webview.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
			webview.InsetsForScreenOreitation += InsetsForScreenOreitation;
		}

		Waiting.gameObject.SetActive(false);
	}
	//-------------------------------------------------------------
	void OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
	{

	}
	//-------------------------------------------------------------
	void OnLoadComplete (UniWebView webView, bool success, string errorMessage)
	{
		if (success)
		{
			webView.Show();
			Waiting.gameObject.SetActive(false);
		}
		else
		{
			UnityDebugger.Debugger.Log("Something wrong in webview loading: " + errorMessage);
			//_errorMessage = errorMessage;
		}
		webview.CleanCache();
	}
	//-------------------------------------------------------------
	bool OnWebViewShouldClose (UniWebView webView)
	{
		if (webview == webView)
		{
			webView = null;
			return true;
		}
		return false;
	}
	//-------------------------------------------------------------
	void OnEvalJavaScriptFinished (UniWebView webView, string result)
	{
	}
	//-------------------------------------------------------------
	UniWebViewEdgeInsets InsetsForScreenOreitation (UniWebView webView, UniWebViewOrientation orientation)
	{

		Vector3[] Corners = Background.worldCorners;

		int layer = LayerManager.GetLayerUI ();
		Camera camera = NGUITools.FindCameraForLayer(layer);

		if (null != camera)
		{
			for(int i=0;i <Corners.Length;i++)
			{
				Corners[i] = camera.WorldToScreenPoint(Corners[i]);
			}
			float w = camera.pixelWidth;
			float h = camera.pixelHeight;

			return new UniWebViewEdgeInsets (
				(int)(camera.pixelHeight - Corners[2].y),
				(int)Corners[0].x,
				(int)Corners[0].y,
				(int)(camera.pixelWidth - Corners[2].x));
		}
		else
		{
			return new UniWebViewEdgeInsets (0, 0, 0, 0);
		}
	}
	//-------------------------------------------------------------
	public override void Hide ()
	{
		if (null != webview)
			webview.Hide();
		base.Hide ();
	}
	//-------------------------------------------------------------
}
