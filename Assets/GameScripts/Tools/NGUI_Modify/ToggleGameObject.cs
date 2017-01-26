//-----------------------------------------------------------------------------------------------------
//開關 GameObject
//-----------------------------------------------------------------------------------------------------
using UnityEngine;

public class ToggleGameObject : MonoBehaviour 
{
	public void Active()
	{
		gameObject.SetActive(true);
	}

	public void Deactive()
	{
		gameObject.SetActive(false);
	}


	public void Toggle()
	{
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
