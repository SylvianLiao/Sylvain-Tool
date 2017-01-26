using UnityEngine;
using UnityEditor;
using System.Collections;

public class DelPlayerPrefs : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[MenuItem("Funfia/Clear PlayerPrefs")]
	static public void ClearPlayerPrefs()
	{
		PlayerPrefs.DeleteAll ();
		Debug.Log ("Clear all PlayerPrefs");
	}
}
