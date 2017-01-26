using System;
using UnityEngine;
using GameFramework;
using System.Collections;
using System.Collections.Generic;

public class Slot_MopUpReward : NGUIChildGUI {

	public Slot_Item[]		m_RewardArray 		= new Slot_Item[6]; 		//獎勵物品背景圖
	//----------------------手動Assign----------------------------
	public GameObject		m_RewardList			= null;					//獎勵清單
	public AnimationClip	m_RewardAnimClip		= null;					//每個獎勵的出現特效
	//----------------------------------------------------------------
	private const string	m_SlotName				="Slot_Item";
	private const string	m_AnimClipName			="UI_MopUp_Reward";
	public GameObject[]		m_RewardPosArray		= new GameObject[6]; 	//各個獎勵物品位置
	// smartObjectName
	private const string 	GUI_SMARTOBJECT_NAME = "Slot_MopUpReward";
	
	//-------------------------------------------------------------------------------------------------
	private Slot_MopUpReward() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	
	//-------------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Initialize()
	{
		CreateItemSlot();
	}
	//-----------------------------------------------------------------------------------------------------
	private void CreateItemSlot()
	{
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_ActivityLimitTimeType load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}
		//Slot
		for(int i=0; i < m_RewardArray.Length; ++i)
		{
			Slot_Item newgo= Instantiate(go) as Slot_Item;
			newgo.transform.parent			= m_RewardList.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition = m_RewardPosArray[i].transform.localPosition;

			Animation rewardAnim = newgo.transform.gameObject.AddComponent<Animation>();
			rewardAnim.AddClip(m_RewardAnimClip , m_AnimClipName);
			rewardAnim.clip = m_RewardAnimClip;
			newgo.name = string.Format("slotItem{0:00}",i);
			newgo.gameObject.SetActive(true);
			m_RewardArray[i] = newgo;
		}
		DestroyRewardPos();
	}
	//-----------------------------------------------------------------------------------------------------
	private void DestroyRewardPos()
	{
		if (m_RewardPosArray.Length < 1)
			return;
		for(int i=0; i < m_RewardPosArray.Length; ++i)
			DestroyImmediate(m_RewardPosArray[i]);
	}
}
