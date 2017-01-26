using UnityEngine;
using System.Collections;

public class BuffData : MonoBehaviour
{
	public UISprite Icon;
	public int	GUID;
	float	TodoTriggerTime = 0.0f;
	[HideInInspector]public int 	SerialNo;
	//int Height;
	//int Width;
	public void SetData (S_BuffData_Tmp buff_tmp, int newSerialNo)
	{
		gameObject.name = newSerialNo.ToString();

		Utility.ChangeAtlasSprite(Icon, buff_tmp.iIcon);
		SerialNo = newSerialNo;
		GUID = buff_tmp.GUID;

		if (buff_tmp.fEffectTime >0)
		{
			float triggerTime = buff_tmp.fEffectTime - 5.0f;

			if (triggerTime <= 0.0f)
				triggerTime = buff_tmp.fEffectTime - 2.0f;
			if (gameObject.activeInHierarchy)
				StartCoroutine(NotifyDisappear(triggerTime));
			else
				TodoTriggerTime = triggerTime;
		}
	}

	IEnumerator NotifyDisappear(float triggerTime)
	{
		yield return new WaitForSeconds(triggerTime);
		UIPlayTween pt = GetComponent<UIPlayTween>();

		if (pt)
			pt.Play(true);
	}

	void Update()
	{
		if (TodoTriggerTime > 0)
		{
			StartCoroutine(NotifyDisappear(TodoTriggerTime));
			TodoTriggerTime = 0.0f;
		}
	}
}
