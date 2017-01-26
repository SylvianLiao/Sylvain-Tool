using System.Collections;
using System.Collections.Generic;

namespace GameScripts.GameFramework
{
	public abstract class GameApplication
	{
		public GameStateService gameStateService { get; protected set; }

		// 存放系統物件的Dictionary
		protected Dictionary<string, BaseSystem> m_SystemMap;

		public GameApplication()
		{
			gameStateService = new GameStateService();
			m_SystemMap = new Dictionary<string, BaseSystem>();

		}

		public abstract void Initialize();

		public virtual void Update()
		{
		}
		
		#region State切換
		
		//---------------------------------------------------------------------------------------------        
		public void AddGameState(GameState gameState)
		{
			gameStateService.addState(gameState);
		}
		//---------------------------------------------------------------------------------------------
		public void ChangeState(string nextStateName, Hashtable hashtable = null)
		{
			gameStateService.changeState(nextStateName, hashtable);
		}
		
		//---------------------------------------------------------------------------------------------
		public void PushState(string pushStateName)
		{
			Hashtable table = new Hashtable();
			gameStateService.pushState(pushStateName, table);
		}
		
		//---------------------------------------------------------------------------------------------
		public void PushState(string pushStateName, Hashtable hashtable)
		{
			gameStateService.pushState(pushStateName, hashtable);
		}
		
		//---------------------------------------------------------------------------------------------
		public void PopState()
		{
            PopState(null);
        }
        //---------------------------------------------------------------------------------------------
        public void PopState(Hashtable table)
        {
            gameStateService.popState(table);
        }
        //---------------------------------------------------------------------------------------------
        // 取得指定名稱的GameState        
        public GameState GetGameStateByName(string stateName)
		{
			return gameStateService.getState(stateName);
		}
		
		//---------------------------------------------------------------------------------------------
		// 取得目前運作的GameState
		public GameState GetCurrentGameState()
		{
			return gameStateService.getCurrentState();
		}

		//------------------------------------------------------------------------------------------
		// 啟動封包lock機制，會自動send封包
		//public virtual void CommandSync(JSONPG_RequestBase data, PacketID PKID)
		//{
		//}
		
		//------------------------------------------------------------------------------------------
		//組合Hashtable (塞資料給即將推起來的狀態用)
		public virtual Hashtable Hash(params object[] args)
		{
			Hashtable hashTable = new Hashtable(args.Length / 2);
			if (args.Length % 2 != 0)
			{
				//UnityDebugger.Debugger.LogError("Tween Error: Hash requires an even number of arguments!");
				return null;
			}
			else
			{
				int i = 0;
				while (i < args.Length - 1)
				{
					hashTable.Add(args[i], args[i + 1]);
					i += 2;
				}
				return hashTable;
			}
		}

		#endregion State切換

		#region 存取System物件
		//---------------------------------------------------------------------------------------------
		// 取得System物件
		public T GetSystem<T>() where T : BaseSystem
		{
			string strName = typeof(T).Name;
			if (m_SystemMap.ContainsKey(strName))
			{
				return m_SystemMap[strName] as T;
			}
			return null;
		}

		//---------------------------------------------------------------------------------------------
		// 新增System物件
		public void AddSystem<T>(T system) where T : BaseSystem
		{
			string strName = typeof(T).Name;
			if (m_SystemMap.ContainsKey(strName))
			{
				m_SystemMap[strName] = system;
			}
			else
			{
				m_SystemMap.Add(strName, system);
			}

			system.Initialize();
		}
		#endregion 存取System物件
	}
}
