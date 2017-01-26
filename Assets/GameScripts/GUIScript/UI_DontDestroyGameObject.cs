using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_DontDestroyGameObject : MonoBehaviour
{
	public GameObject childPanel;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
	
	void Update()
	{
	}
}

