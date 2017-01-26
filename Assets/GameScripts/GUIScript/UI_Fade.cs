using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

public class UI_Fade : NGUIChildGUI
{
	class FadeData
	{
		public FadeData(Texture2D _pic, float _from ,float _to, float _duration, onFinish _finishEvent)
		{
			pic = _pic;
			from = _from;
			to = _to;
			duration = _duration;
			finishEvent = _finishEvent;
		}
		public Texture2D pic;
		public float from;
		public float to;
		public float duration;
		public onFinish finishEvent;
	}

	public UITexture backgroundPic;
	//public BoxCollider colliderFullScreen;
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Fade";

	public delegate void onFinish();
	public event onFinish OnFinish;

	List<FadeData> FadeList = new List<FadeData>();
	TweenAlpha ta;
	TweenColor tc;
	void InnerOnFinish()
	{
		//colliderFullScreen.enabled = false;
		if (null != OnFinish)
		{
			OnFinish();
			OnFinish = null;
		}
	}
	private UI_Fade()
		: base(GUI_SMARTOBJECT_NAME)
	{

	}

	IEnumerator HandleAlpha(float from, float to, float duration)
	{
		float remain = duration;
		float delta = to - from;

		backgroundPic.color = Color.white;

		while(remain > 0)
		{
			float a = from + delta * (duration - remain)/duration;

			backgroundPic.color = Color.white * a;

			remain -= Time.deltaTime;
			yield return null;
		}

		InnerOnFinish();
	}
	public void  AddToFade(Texture2D pic, float from ,float to, float duration, onFinish finishEvent)
	{
		FadeData data = new FadeData(pic, from, to, duration, finishEvent);
		FadeList.Add(data);
	}
	void StartToFade(Texture2D pic, float from ,float to, float duration, onFinish finishEvent)
	{
        backgroundPic.gameObject.SetActive(true);
		//colliderFullScreen.enabled = true;
        if(pic)
		    backgroundPic.mainTexture = pic;
		backgroundPic.SetDirty();

        Show();
		OnFinish = finishEvent;

		ta = TweenAlpha.Begin(gameObject, duration, to);
		ta.from = from;
		ta.method = UITweener.Method.Linear;
		ta.SetOnFinished(InnerOnFinish);
		ta.ResetToBeginning();
		ta.PlayForward();

		tc = TweenColor.Begin(gameObject, duration, Color.white * to);
		tc.from = Color.white * from;
		tc.method = UITweener.Method.Linear;
		//tc.SetOnFinished(InnerOnFinish);
		tc.ResetToBeginning();
		tc.PlayForward();

	}

	public override void ForceUpdate ()
	{
		base.ForceUpdate ();
		if (0 < FadeList.Count)
		{
			ta = GetComponent<TweenAlpha>();
			if ((null == ta) || (false == ta.enabled))
			{
				StartToFade(
					FadeList[0].pic,
					FadeList[0].from,
					FadeList[0].to,
					FadeList[0].duration,
					FadeList[0].finishEvent );

				FadeList.RemoveAt(0);
			}
		}
	}
}
