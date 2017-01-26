using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.Linq;


public class UI_Lottery : NGUIChildGUI
{
	public UIPanel Platform;			//
	public UISprite	RewardItem;			// 道具樣板
	public UISprite Indicator;			// 指針
	public UIButton BtnRunning;			// 啟動按鈕
	public float Radius;				// 半徑
	public float Duration;				// 執行時間
	public delegate void RunDone(LotteryReward item_Reward);
	public delegate void RotateDone();
	public event RotateDone RotateDoneEvent;
	public event RunDone StartToRunEvent;
	public UISprite ConsumableItem;		// 消耗品
	public bool Clockwise = true;		// 順時針
	public float Speed = 30.0f;
	public UILabel Countdown;

	public DateTime Cooldown;

	float Direction;

	float CurrentSpeed;
	[System.NonSerialized]
	public List<LotteryReward> RewardItems				= new List<LotteryReward>();
	int Index;
	Dictionary<UIButton,LotteryReward> ItemMap;

	bool Running;
	float CurrentDuration;

	public UIButton		btnCloseLottery			= null; //關閉轉盤

	public UILabel		lbDailyResetTip			= null; //隔日會清除的提示
	[Header("TopLeft")]
	public UILabel		lbAlreadyGetStars		= null; //已獲得星數字樣
	public UISprite[]	spStars					= new UISprite[3];
	[Header("Info")]
	public UILabel		lbMainTitle				= null; //主標題
	public UILabel		lbLotteryContent		= null; //轉盤說明文
	public UISprite[]	Rewards					= new UISprite[3]; 	//星等獎勵
	public UIButton[]	btnExchanges			= new UIButton[3];	//兌換按鈕
	public UILabel[]	lbExchanges				= new UILabel[3];	//兌換按鈕字樣
	[System.NonSerialized]
	public List<Slot_Item>	ExchangeRewards		= new List<Slot_Item>();
	public UILabel		lbRun					= null; //抽獎按鈕字樣
	public UILabel		lbLotteryTimes			= null; //抽獎次數顯示
	[Header("LotteryBody")]
	public UILabel		lbWord					= null; //指示說明
	public UISprite		spriteHighligh			= null; //得到物品的高亮

	//
	private float		RdIndicatorEular		= 0;
	private const string	m_SlotName			= "Slot_Item";
	// smartObjectName
	private const string GUI_SMARTOBJECT_NAME = "UI_Lottery";

	//-----------------------------------------------------------------------------------------------------
	private UI_Lottery() : base(GUI_SMARTOBJECT_NAME)
	{
	}
	//-----------------------------------------------------------------------------------------------------
	public override void Initialize ()
	{
		base.Initialize ();

		AssignString();
		CreateExchangeRewardSlots();

		//spriteHighligh.gameObject.SetActive(false);

		if (null != RewardItem)
			RewardItem.gameObject.SetActive(false);

		Cooldown = DateTime.UtcNow;
	}
	//-----------------------------------------------------------------------------------------------------
	private void CreateExchangeRewardSlots()
	{
		ExchangeRewards.Clear();

		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Item load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}

		for(int i=0;i<Rewards.Length;++i)
		{
			Slot_Item newgo					= GameObject.Instantiate(go) as Slot_Item;
			newgo.transform.parent			= Rewards[i].transform.parent;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= Quaternion.identity;
			newgo.transform.localPosition 	= Rewards[i].transform.localPosition;
			//
			ExchangeRewards.Add(newgo);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//指派字串
	private void AssignString()
	{
		lbMainTitle.text 		= GameDataDB.GetString(940); //幸運轉輪
		lbLotteryContent.text	= GameDataDB.GetString(941); //轉輪活動內容
		lbRun.text				= GameDataDB.GetString(942); //開始許願
		for(int i=0;i<lbExchanges.Length;++i)
			lbExchanges[i].text	= GameDataDB.GetString(943); //兌換

		lbAlreadyGetStars.text	= GameDataDB.GetString(949); //已獲得星星
		lbWord.text				= GameDataDB.GetString(950); //祝君中獎
		//
		lbDailyResetTip.text	= GameDataDB.GetString(948); //隔日清除的提示文字
	}
	//-----------------------------------------------------------------------------------------------------
	public void SetRewardItems(LotteryReward[] items, S_Item_Tmp Consumable_tmp)
	{
		// 建立UI
		float angle = 360.0f / items.Length * Mathf.PI / 180.0f;
		RewardItems.Clear();
		for(int i=0;i<items.Length;++i)
			RewardItems.Add(items[i]);

		ItemMap = new Dictionary<UIButton,LotteryReward>();
		ItemMap.Clear();

		//亂序排列
		RewardItems = RewardItems.Distinct().OrderBy(x =>  System.Guid.NewGuid().ToString()).ToList();
		//生成獎勵並指派位置
		Slot_Item go = ResourceManager.Instance.GetGUI(m_SlotName).GetComponent<Slot_Item>();
		if(go == null)
		{
			UnityDebugger.Debugger.LogError( string.Format("Slot_Item load prefeb error,path:{0}", "GUI/"+m_SlotName) );
			return;
		}

		for(int i=0;i<RewardItems.Count;i++)
		{
			float x = Radius * Mathf.Sin(-angle*i);
			float y = Radius * Mathf.Cos(-angle*i);

			Slot_Item newgo					= GameObject.Instantiate(go) as Slot_Item;
			newgo.transform.parent			= Platform.transform;
			newgo.transform.localScale		= Vector3.one;
			newgo.transform.localRotation	= new Quaternion(0, 0, 0, 0);	//Quaternion.AngleAxis(0, Vector3.zero);
			newgo.transform.localPosition 	= new Vector3(x,y);
			newgo.name = string.Format("slotItem{0:00}",i);

			//設定
			newgo.SetSlotWithCount(RewardItems[i].iItemGUID,RewardItems[i].iCount,false);

			//指派事件
			newgo.ButtonSlot.userData = RewardItems[i].iItemGUID;
			UIEventListener.Get(newgo.ButtonSlot.gameObject).onClick	+= ChestCheck;
		}

		if (null != Consumable_tmp)
			Utility.ChangeAtlasSprite(ConsumableItem, Consumable_tmp.ItemIcon);
		else 
			ConsumableItem.gameObject.SetActive(false);
        
    }
	//-----------------------------------------------------------------------------------------------------
	//物品按鈕資訊導引
	public void ChestCheck(GameObject go)
	{
		int itemGUID = (int)go.GetComponent<UIButton>().userData;
		S_Item_Tmp dbf = GameDataDB.ItemDB.GetData(itemGUID);
		if (dbf == null)
			return;
		
		ARPGApplication.instance.m_uiItemTip.ShowItemTmpWithCount(dbf.GUID, 1);
		EventDelegate.Add(ARPGApplication.instance.m_uiItemTip.ButtonFullScreen.onClick, CloseItemInfo);
	}
	//-----------------------------------------------------------------------------------------------------
	//關閉物品資訊,由UI_ItemTip呼叫
	public void CloseItemInfo()
	{
		EventDelegate.Remove(ARPGApplication.instance.m_uiItemTip.ButtonFullScreen.onClick, CloseItemInfo);
	}
	//-----------------------------------------------------------------------------------------------------
	/*public bool Run(S_Item_Tmp item_tmp)
	{
		if (Running)
			return false;
		CurrentSpeed = Speed;
		Running = true;
		if (Clockwise)
			Direction = -1;
		else
			Direction = 1;
		CurrentDuration = 0;
		int rewardItemIndex = -1;
		for(int i=0;i<RewardItems.Length;i++)
		{
			if (RewardItems[i] == item_tmp)
			{
				rewardItemIndex = i;
				break;
			}
		}

		if ((rewardItemIndex < 0)||(rewardItemIndex > RewardItems.Length))
			return false;

		//if (rewardItemIndex > RewardItems.Length)
		//	return false;

		Index = rewardItemIndex;

		Indicator.transform.rotation = Quaternion.identity;

		StartCoroutine(WaitingForStop(Duration));
		return true;
	}*/
	//-----------------------------------------------------------------------------------------------------
	public bool Run(int index)
	{
		if (Running)
			return false;
		CurrentSpeed = Speed;
		Running = true;
		if (Clockwise)
			Direction = -1;
		else
			Direction = 1;

		CurrentDuration = 0;
		
		if ((index < 0)||(index > RewardItems.Count))
			return false;
		
		Index = index;
		
		//Indicator.transform.rotation = Quaternion.identity;
		
		StartCoroutine(WaitingForStop(Duration));
		return true;
	}
	//-----------------------------------------------------------------------------------------------------
	IEnumerator WaitingForStop (float duration)
	{
		float CurrentDuration = duration + UnityEngine.Random.Range(0.0f, (duration / 5.0f * 4.0f));

		float constantSpeedTime = 1.0f;
		float constantSpeed = 20.0f;

		CurrentSpeed = constantSpeed;

		while (true)
		{
			yield return null;

			constantSpeedTime -= Time.deltaTime;
			if (constantSpeedTime <= 0.0f)
				break;
		}

		constantSpeedTime = 1.0f;
		while (true)
		{
			yield return null;

			CurrentSpeed*=0.95f;
			if (CurrentSpeed < 10.0f)
				CurrentSpeed = 10.0f;
						
			constantSpeedTime -= Time.deltaTime;
			if (constantSpeedTime <= 0.0f)
				break;
		}


		float angle = 360.0f/ RewardItems.Count * Index;


		float curAngle = Indicator.transform.rotation.eulerAngles.z;

		while (true)
		{
			yield return null;

			curAngle = Indicator.transform.rotation.eulerAngles.z;

			CurrentSpeed*=0.95f;
			if (CurrentSpeed < 5.0f)
				CurrentSpeed = 5.0f;

			if (curAngle - angle > 0 && curAngle - angle < 40)
			{
				break;
			}
		}

		CurrentSpeed = 0.0f;

		iTween.RotateTo(Indicator.gameObject, new Vector3(0.0f, 0.0f, angle), 1.0f);
		yield return new WaitForSeconds(1.0f);

		/*
		iTween.ValueTo(
			gameObject,
			iTween.Hash(
			"time", CurrentDuration,
			"from", CurrentSpeed,
			"to", 5.0f,
			"onupdate",	"updateFromValue"
			)
			);


		float angle = 360.0f/ RewardItems.Count * Index;

		float lastZ = Indicator.transform.rotation.eulerAngles.z;
		while(true)
		{
			yield return null;

			CurrentDuration -= Time.deltaTime;

			if (CurrentDuration > 0.0f)
			{
				lastZ = Indicator.transform.rotation.eulerAngles.z;
				continue;
			}

			float z = Indicator.transform.rotation.eulerAngles.z;

			if (!Clockwise)
			{
				if (lastZ > z)
				{

					// 經過0
					if ((lastZ < angle) && (360 >= angle))
						break;
					else if ((0 <= angle) && (z >= angle))
						break;
				}
				else
				{
					if ((lastZ < angle) && (z >= angle))
						break;
				}
			}
			else
			{
				if (lastZ < z)
				{

					// 經過0
					if ((z < angle) && (360 >= angle))
						break;
					else if ((0 <= angle) && (lastZ >= angle))
						break;
				}
				else
				{
					if ((lastZ > angle) && (z <= angle))
						break;
				}
			}
			lastZ = z;
		}
			*/



		Indicator.transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
		Running = false;

		OnFinished();
	}
	//-----------------------------------------------------------------------------------------------------
	void updateFromValue(float newValue)
	{
		CurrentSpeed = newValue;
	}
	//-----------------------------------------------------------------------------------------------------
	void OnFinished ()
	{
		if (null != RotateDoneEvent)
			RotateDoneEvent();
	}
	//-----------------------------------------------------------------------------------------------------
	void OnClick()
	{
		UIButton target = UIButton.current;
		if (null != StartToRunEvent)
		{
			LotteryReward item_Reward;
			if (ItemMap.TryGetValue(target,out item_Reward ))
			{
				//StartToRunEvent(item_tmp);
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void Update()
	{
		//指針轉動
		if (Running)
		{
			Indicator.transform.Rotate(0,0, Direction * CurrentSpeed, Space.Self);
			LotteryActiveSoundEffect();
		}

		//冷卻時間
		TimeSpan diff = Cooldown.Subtract(DateTime.UtcNow);

		if (diff.Seconds <= 0)
		{
			BtnRunning.state = UIButtonColor.State.Normal;
			Countdown.text = "00:00:00";
		}
		else
		{
			BtnRunning.state = UIButtonColor.State.Disabled;

			long ticks = (diff.Ticks >0)?diff.Ticks:0;
			DateTime a = new DateTime(ticks);
			// 更新時間
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(a.Hour);
			stringBuilder.Append(":");
			stringBuilder.Append(a.Minute);
			stringBuilder.Append(":");
			stringBuilder.Append(a.Second);
			Countdown.text = string.Format("{0:00}:{1:00}:{2:00}", a.Hour,a.Minute,a.Second);
		}
		
	}
	//-----------------------------------------------------------------------------------------------------
	//轉盤旋轉時的音效顯示
	private void LotteryActiveSoundEffect()
	{
		//先紀錄下目前的角度
		if(RdIndicatorEular == 0)
		{
			RdIndicatorEular = Indicator.transform.rotation.eulerAngles.z;
		}
		//判斷是否有到需要的角度差
		float diffangle = (360.0f / RewardItems.Count)-2.0f; //角度差-誤差值
		
		if(Mathf.Abs(Indicator.transform.rotation.eulerAngles.z - RdIndicatorEular) >= diffangle)
		{
			RdIndicatorEular = 0;
			MusicControlSystem.PlayUISound("Sound_System_001",1);
		}
	}
	//-----------------------------------------------------------------------------------------------------
}