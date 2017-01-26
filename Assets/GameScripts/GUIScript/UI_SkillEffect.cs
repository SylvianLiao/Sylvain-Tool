using System;
using UnityEngine;
using GameFramework;

public class UI_SkillEffect : NGUIChildGUI
{
	public UISprite		spriteSkill1 = null;
	public UISprite		spriteSkill2 = null;
	public UISprite		spriteSkill3 = null;
	public bool			bGesture = true;		//啟動圖形手勢開關
	public int			iBeginGesture = 0;		//手勢開關編號
	public GameObject	mainPlayer = null;		//主玩家物件
	//
	private const string GUI_SMARTOBJECT_NAME = "UI_SkillEffect";
	
	//-----------------------------------------------------------------------------------------------------
	private UI_SkillEffect() : base(GUI_SMARTOBJECT_NAME)
	{		
	}
	//-----------------------------------------------------------------------------------------------------
	private void Start()
	{
		iBeginGesture = 0;
	}
	//-----------------------------------------------------------------------------------------------------
	private void Update()
	{
		UpdateGesture();
	}
	//-----------------------------------------------------------------------------------------------------
	//更新圖形手勢流程
	private void UpdateGesture()
	{
		//開關開啟
		if(bGesture == false)
			return;
		
		//gesture
		if(mainPlayer == null)
			return;
		Gesture gs = mainPlayer.GetComponent<Gesture>();
		if(gs && gs.isCheckOK>0)
		{
			switch(GestureRecognizer.gestureChosen)
			{
			// square
			case 0:
			case 8:
				break;
			// Circle
			case 1:
			case 9:
			case 16:
			{
			}
				break;
			// triangle
			case 2:
			case 10:
			case 17:
				break;
			//cross
			case 3:
			case 11:
				{
					DungeonBaseState state = DungeonBaseState.getNowDungeonBaseState();
					if(state != null)
						OnPlayerSkill(state.getSkill01Btn, true, ARPGSkill.ENUM_SkillIndex.skill1);
				}
				break;
			// a
			case 4:
			case 12:
				{
					DungeonBaseState state = DungeonBaseState.getNowDungeonBaseState();
					if(state != null)
						state.OnStartEXSkill(null);
				}
				break;
			// Z
			case 5:
			case 13:
				{
					DungeonBaseState state = DungeonBaseState.getNowDungeonBaseState();
					if(state != null)
					{
						//取得寵物1身上技能
						ARPGBattle pet1Battle = ARPGApplication.instance.m_tempGameObjectSystem.GetARPGBattleByMain().petsBattle[0];
						OnPetSkill(pet1Battle, 2, state.getPet1SkillBtn, true);
					}
				}
				break;
				//line
			case 6:
			case 14:
				break;
				//wave
			case 7:
			case 15:
				break;
				// lightning
			case 18:
			case 19:
				{
					DungeonBaseState state = DungeonBaseState.getNowDungeonBaseState();
					if(state != null)
					{
						//取得寵物2身上技能
						ARPGBattle pet2Battle = ARPGApplication.instance.m_tempGameObjectSystem.GetARPGBattleByMain().petsBattle[1];
						OnPetSkill(pet2Battle, 3, state.getPet2SkillBtn, true);
					}
				}
				break;
			}
			
			gs.isCheckOK = 0;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public void PlaySkillTween(int index)
	{
		UISprite sprite = null;

		switch(index)
		{
		case 1:		sprite = spriteSkill1;		break;
		case 2:		sprite = spriteSkill2;		break;
		case 3:		sprite = spriteSkill3;		break;
		}

		TweenAlpha ta = sprite.gameObject.GetComponent<TweenAlpha>();
		if(ta)
		{
			ta.from = 1.0f;
			ta.to = 0.0f;
			ta.ResetToBeginning();
			ta.enabled = true;
		}

		TweenScale ts = sprite.gameObject.GetComponent<TweenScale>();
		if(ts)
		{
			ts.ResetToBeginning();
			ts.enabled = true;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//發出主玩家技能
	public bool OnPlayerSkill(GameObject btn, bool bShow, ARPGSkill.ENUM_SkillIndex index)
	{
		bool isCast = false;

		//取得主玩家身上技能
		ARPGBattle compBattle = ARPGApplication.instance.m_tempGameObjectSystem.GetARPGBattleByMain();
		ARPGSkill compSkill = compBattle.compSkills[index];
		if(compSkill)
		{
			isCast = compSkill.Cast(btn);
		}

		if(isCast)
		{
			if(bShow)
				PlaySkillTween(1);
			UnityDebugger.Debugger.Log("casting player skill GUID=[" + compSkill.iDBFGUID.ToString() + "]!!");
		}

		return isCast;
	}
	//-----------------------------------------------------------------------------------------------------
	//發出主玩家爆魂技能
	public bool OnPlayerEXSkill(GameObject btn, bool bShow)
	{
		bool isCast = false;
		
		//取得主玩家身上技能
		ARPGBattle compBattle = ARPGApplication.instance.m_tempGameObjectSystem.GetARPGBattleByMain();
		ARPGSkill compSkill = compBattle.compSkills[ARPGSkill.ENUM_SkillIndex.EXSkill];
		if(compSkill)
		{
			isCast = compSkill.Cast(btn);
		}
		
		if(isCast)
		{
			if(bShow)
				PlaySkillTween(1);
			UnityDebugger.Debugger.Log("casting player EXSkill GUID=[" + compSkill.iDBFGUID.ToString() + "]!!");
		}
		
		return isCast;
	}
	//-----------------------------------------------------------------------------------------------------
	//發出主玩家閃躲技能
	public bool OnPlayerDodgeSkill(GameObject btn, bool bShow)
	{
		bool isCast = false;

		//取得主玩家身上技能
		ARPGBattle compBattle = ARPGApplication.instance.m_tempGameObjectSystem.GetARPGBattleByMain();
		ARPGSkill compSkill = compBattle.compSkills[ARPGSkill.ENUM_SkillIndex.DodgeSkill];
		if(compSkill)
		{
			isCast = compSkill.Cast(btn);
		}
		
		if(isCast)
		{
			if(bShow)
				PlaySkillTween(1);
			UnityDebugger.Debugger.Log("casting player DodgeSkill GUID=[" + compSkill.iDBFGUID.ToString() + "]!!");
		}

		return isCast;
	}
	//-----------------------------------------------------------------------------------------------------
	//發出寵物技能
	public bool OnPetSkill(ARPGBattle petBattle, int skillTween, GameObject btn, bool bShow)
	{
		bool isCast = false;

		if(petBattle!=null && petBattle.gameObject.activeSelf && petBattle.compSkills!=null)
		{
			isCast = petBattle.compSkills[ARPGSkill.ENUM_SkillIndex.skill1].Cast(btn);
		}

		if(isCast)
		{
			if(bShow)
				PlaySkillTween(skillTween);
			UnityDebugger.Debugger.Log("casting player pet's skill GUID=[" + petBattle.compSkills[ARPGSkill.ENUM_SkillIndex.skill1].iDBFGUID.ToString() + "]!!");
		}
		
		return isCast;
	}
}
