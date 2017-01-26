using UnityEngine;
using System.Collections;

public class TouchSensor : MonoBehaviour {
	Vector2 StartPoint;
	Vector2 EndPoint;
	public delegate void DragEvent(Vector2 startPoint, Vector2 endPoint);
	public event DragEvent TriggerDragEvent;
	// Use this for initialization
	void Start ()
	{
		UIEventTrigger trigger = gameObject.AddComponent<UIEventTrigger>();
		EventDelegate.Add(trigger.onPress , OnPress);
		EventDelegate.Add(trigger.onRelease , OnRelease);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnPress()
	{
		UnityDebugger.Debugger.Log("OnPress");
		if (GetTouchPosition(out StartPoint))
		{
			UnityDebugger.Debugger.Log("OnPress:" + EndPoint.ToString());
        }
    }
    
	void OnRelease()
	{
		UnityDebugger.Debugger.Log("OnRelease");
		if (GetTouchPosition(out EndPoint))
		{
			UnityDebugger.Debugger.Log("OnRelease:" + StartPoint.ToString());
        }

		if (null != TriggerDragEvent)
		{
			TriggerDragEvent(StartPoint, EndPoint);
		}
    }
    
    void OnDragOver()
	{
		return;
		Vector2 point;
		if (GetTouchPosition(out point))
		{
			string position = point.ToString();
			UnityDebugger.Debugger.Log("OnDragOver:" + position);
		}
	}

	bool GetTouchPosition(out Vector2 outPosition)
	{
		bool result = true;
		Vector2 position = new Vector2();
	#if UNITY_EDITOR
		position.x = Input.mousePosition.x;
		position.y = Input.mousePosition.y;
	#elif UNITY_ANDROID || UNITY_IOS
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			position.x = touch.position.x;
			position.y = touch.position.y;
		}
		else
			result = false;
	#endif
		outPosition = position;
		return result;
	}
}
