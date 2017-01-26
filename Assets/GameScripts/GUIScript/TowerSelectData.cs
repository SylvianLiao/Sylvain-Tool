using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//通天塔選擇層數/買的Buff選項
public class TowerSelectData : MonoBehaviour 
{
	public UIToggle 	tgSelect 			= null; //選擇
	public UILabel		lbContent			= null; //選擇內容
	public UISprite		spriteIcon			= null; //幣別
	public UILabel		lbValue				= null; //價值
	[System.NonSerialized]
	public int			DungeonCounts		= 0;
	[System.NonSerialized]
	public List<int>	DungeonIDs			= new List<int>();
	//-----------------------------------------------------------------------------------------------
	void Start()
	{
	}
	//-----------------------------------------------------------------------------------------------
	void Update()
	{
		
	}
	//-----------------------------------------------------------------------------------------------
	void Init()
	{
	}
	//-----------------------------------------------------------------------------------------------
}
