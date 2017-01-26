using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_DungeonChapter : MonoBehaviour  {

	public UIPanel 		panelBase 			= null;
	/// <summary>
	/// Prefab中路線圖命名方式 Sprite+副本DBF.GUID
	/// </summary>
	public GameObject 	gLineContainer 		= null;
	public UITexture 	textChapterMap 		= null;
	//
	public List<UISprite>	RoadList		= new List<UISprite>();
	public List<Transform>	NodeList		= new List<Transform>();
	//
	//-------------------------------------------------------------------------------------------------
	void Awake()
	{
		InitialUI();
	}
	
	//-------------------------------------------------------------------------------------------------
	void InitialUI()
	{
		
	}
}
