using System;
using System.Collections;

public enum Enum_StateParam
{
    LoadGUIAsync,           //是否透過非同步方式讀取UI，其Value值為是否使用Loading UI(True or False)
    DelayDeleteGUIName,     //延後至下一個State才刪除的GUI名稱
    ChangeByScreenShot,     //透過截圖方式切換UI
    UseTopBarUI,            //是否使用TopBar UI
    UsePlayerInfoUI,        //是否使用PlayerInfo UI
    SongDifficulty,         //目前玩家選擇的歌曲難度
    Tutorial,               //是否為教學關卡(只用於LoadGameState)
    EnterOTP,               //開啟輸入換機密碼UI (切換裝置UI用)
    GetOTP,               //開啟取得換機密碼UI (切換裝置UI用)
}

public abstract class GameState
{
    public string name { get; private set; }
	public bool updated;		//state內的update狀態是否需要更新
	public bool isPlaying
	{
		get{return m_Playing;}
		set
		{
			m_Playing = value;
			if(GameStatePlayingChange != null)
			{
				GameStatePlayingChange(m_Playing);
			}
		}
	}	
	private bool m_Playing;
	public Hashtable userData;	//state啟動時可能會用到的參數
	public bool isParent = false;

	public delegate	void OnPlayingChange(bool flag);
	public event OnPlayingChange GameStatePlayingChange;

    public GameState(string name)
    {
        this.name = name;
    }

    public abstract void begin();

    public abstract void update();

    public abstract void onGUI();

    public abstract void end();

    public abstract void suspend();

    public abstract void resume();
}

