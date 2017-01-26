using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

public class Slot_SomeoneChat : MonoBehaviour {
	//L----
	public Slot_RoleIcon    L_spriteIcon	= null;
	public UILabel	        L_labelName		= null;
	public UILabel	        L_labelContents	= null;
	public UIButton	        L_btnIcon		= null;
	//R----
	public Slot_RoleIcon	R_spriteIcon	= null;
	public UILabel	        R_labelName		= null;
	public UILabel	        R_labelContents	= null;
	public UIButton	        R_btnIcon		= null;

	public UIButton			btnPlaySound	= null;
	public UISprite			spPlaySound		= null;

	public ulong	ui64Serial		= 0;
	public int 		iSpeakerID		= 0;
	public string	sSprakerName	= null;
	public int 		iIconID			= 0;
    public int      iFaceFrameID    = 0;
	public string	sContents		= null;
	public string	Targetname		= null;
	public DateTime	savetime;
	public ENUM_MESSAGESLOTTYPE	emMsgSlotType;
	public ENUM_MESSAGEBOARDTYPE emMsgBdType;

	public int		iLevel			= 0;
	public int		iPower			= 0;

	//---------------------------------------------------------------------
	void Start () 
	{
	}
	//---------------------------------------------------------------------
	//初始化
	public void Clear()
	{
		ui64Serial		= 0;
		iSpeakerID		= 0;
		sSprakerName	= null;
		iIconID			= 0;
        iFaceFrameID    = 0;
		sContents		= null;
		Targetname		= null;
		savetime		= DateTime.Now;
		emMsgSlotType 	= ENUM_MESSAGESLOTTYPE.ENUM_MESSAGESLOT_NORMAL;
		emMsgBdType	 	= ENUM_MESSAGEBOARDTYPE.ENUM_MESSAGEBOARD_NULL;
		iLevel			= 0;
		iPower			= 0;
		btnPlaySound.gameObject.SetActive(false);
	}
	//---------------------------------------------------------------------
	//設定SLOT內容
	public void SetSlot(S_chatSlot sChatSlot,ChatSlotRLType RLType)
	{
		ui64Serial		= sChatSlot.ui64Serial;
		iSpeakerID		= sChatSlot.iSpeakerID;
		sSprakerName	= sChatSlot.sSprakerName;
		iIconID			= sChatSlot.iIconID;
        iFaceFrameID    = sChatSlot.iFaceFrameID;
		sContents		= sChatSlot.sContents;
		Targetname		= sChatSlot.sTargetname;
		savetime		= sChatSlot.time;
		emMsgSlotType	= sChatSlot.emMsgSlotType;
		emMsgBdType		= sChatSlot.emMsgBdType;
		iLevel			= sChatSlot.iLevel;
		iPower			= sChatSlot.iPower;

		SetDisPlay(RLType);
	}
	//---------------
	public void SetSlot(Slot_SomeoneChat slot,ChatSlotRLType RLType)
	{
		ui64Serial		= slot.ui64Serial;
		iSpeakerID		= slot.iSpeakerID;
		sSprakerName	= slot.sSprakerName;
		iIconID			= slot.iIconID;
        iFaceFrameID    = slot.iFaceFrameID;
		sContents		= slot.sContents;
		Targetname		= slot.Targetname;
		savetime		= slot.savetime;
		emMsgSlotType 	= slot.emMsgSlotType;
		emMsgBdType		= slot.emMsgBdType;
		iLevel			= slot.iLevel;
		iPower			= slot.iPower;

		SetDisPlay(RLType);
	}
	//---------------------------------------------------------------------
	//設定顯示
	public void SetDisPlay(ChatSlotRLType RLType)
	{
		//依類型改變顯示方式
		switch(RLType)
		{
		case ChatSlotRLType.L_Type:
			SetShowHide(true,false);
			if(this != null)
			{
				spPlaySound.flip = UIBasicSprite.Flip.Nothing;
                L_spriteIcon.SetSlot(iIconID,iFaceFrameID);
				L_labelName.text = sSprakerName;
				CheckContentsType(L_labelContents,btnPlaySound,sContents);
				//L_labelContents.text = sContents;
			}
			break;
		case ChatSlotRLType.R_Type:
			SetShowHide(false,true);
			if(this != null)
			{
				if(emMsgBdType == ENUM_MESSAGEBOARDTYPE.ENUM_MESSAGEBOARD_PERSON)
				{
					sSprakerName = string.Format(GameDataDB.GetString(253),Targetname);
				}
				spPlaySound.flip = UIBasicSprite.Flip.Horizontally;
                R_spriteIcon.SetSlot(iIconID,iFaceFrameID);
				R_labelName.text = sSprakerName;

				CheckContentsType(R_labelContents,btnPlaySound,sContents);
				//R_labelContents.text = sContents;
			}
			break;
		case ChatSlotRLType.NULL:
			break;
		}
	}
	//---------------------------------------------------------------------
	void SetShowHide(bool _L,bool _R)
	{
		L_btnIcon.gameObject.SetActive(_L);
		L_labelName.gameObject.SetActive(_L);
		L_labelContents.transform.parent.gameObject.SetActive(_L);
		
		R_btnIcon.gameObject.SetActive(_R);
		R_labelName.gameObject.SetActive(_R);
		R_labelContents.transform.parent.gameObject.SetActive(_R);
	}
	//---------------------------------------------------------------------
	void CheckContentsType(UILabel lb,UIButton btn,string str)
	{
		if(str.Contains(GameDefine.YunVaVoice_Title))
		{
			lb.transform.parent.gameObject.SetActive(false);
			btn.gameObject.SetActive(true);
			str = str.Replace(GameDefine.YunVaVoice_Title,String.Empty);
			sContents = str;
			lb.text = str;
		}
		else
		{
			lb.transform.parent.gameObject.SetActive(true);
			btn.gameObject.SetActive(false);
			lb.text = str;
		}
	}


}
