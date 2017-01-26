using UnityEngine;
using System.Collections;

public class BankGetItem : MonoBehaviour
{
	public UILabel		lbContent 	= null;
	public UIButton		btnGet		= null;
	public UILabel		lbGet		= null;
	public UISprite		btnGetSprite= null;
	//
	private int			iInvestmentID;
	[System.NonSerialized]
	public S_Investment_Tmp		InvTemp;

	Color NonGetColor 	= new Color((157.0f/255.0f),(157.0f/255.0f),(157.0f/255.0f));
	Color CanGetColor 	= new Color((255.0f/255.0f),(146.0f/255.0f),(36.0f/255.0f));
	Color GetedColor	= new Color((115.0f/255.0f),(115.0f/255.0f),(185.0f/255.0f));
	// Use this for initialization
	void Start () 
	{
		UIEventListener.Get(btnGet.gameObject).onClick	+= SendGetPacket;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	//---------------------------------------------------------------------------------------------------------------------------------
	private void SendGetPacket(GameObject gb)
	{
		JsonSlot_Reward.Send_CtoM_GetInvestmentReward(iInvestmentID);
	}
	//---------------------------------------------------------------------------------------------------------------------------------
	//設定資料
	public void SetItemData(S_Investment_Tmp invTmp,int index)
	{
		btnGet.userData = index;
		InvTemp = invTmp;
		iInvestmentID = (int)invTmp.GUID;

		//如果返利等級為0時
		if(invTmp.iLevel == 0)
			lbContent.text = string.Format(GameDataDB.GetString(15213),invTmp.iCount);
		else
			lbContent.text = string.Format(GameDataDB.GetString(15214),invTmp.iLevel,invTmp.iCount);
		//更新按鈕狀態
		UpdateButtonState();
	}
	//---------------------------------------------------------------------------------------------------------------------------------
	//更新按鈕狀態
	public void UpdateButtonState()
	{
		int iCurrentLevel = ARPGApplication.instance.m_RoleSystem.iBaseLevel;
		bool isGet  = ARPGApplication.instance.m_RoleSystem.m_PlayerRoleData.BaseRoleData.sInvestment.sFlag.GetFlag(InvTemp.iFlag);
		if(isGet)
		{
			lbGet.text = GameDataDB.GetString(15202); //已領取
			btnGet.disabledColor = GetedColor;
			btnGet.isEnabled = false;
		}
		else
		{
			if(iCurrentLevel<InvTemp.iLevel)
			{
				lbGet.text = GameDataDB.GetString(15200); //未達成
				btnGet.disabledColor = NonGetColor;
				btnGet.isEnabled = false;
			}
			else
			{
				lbGet.text = GameDataDB.GetString(15201); //領取
				btnGetSprite.color = CanGetColor;
				btnGet.isEnabled = true;
			}
		}
	}
	//---------------------------------------------------------------------------------------------------------------------------------
}
