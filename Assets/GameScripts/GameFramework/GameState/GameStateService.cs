using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

public class GameStateService
{
	Dictionary<string, GameState> availableStates = new Dictionary<string, GameState>();

	Stack<GameState> activeStates = new Stack<GameState>();
		
	bool clearactiveStates = false;

	public delegate	void OnStateChange();
	public event OnStateChange GameStateChange;

	private bool isActiveStatesEmpty()
	{
		return activeStates.Count == 0;
	}
		
	//------------------------------------------------------------------------------------------
	public void update()
	{
		clearactiveStates = false;
		GameState[] RuntimeState = activeStates.ToArray();

		foreach (GameState state in RuntimeState)
		{
            //Only playing state can be updated.
            if (state.isPlaying == false)
                continue;

            state.update();
			state.updated = true;
			if (clearactiveStates)
				break;

            break;
		}
	}

	//------------------------------------------------------------------------------------------
	/// <summary>
	/// 加入受管理的 Game State
	/// </summary>
	/// <param name="state"></param>
	public void addState(GameState newState)
	{
		if (newState == null)
			throw new ArgumentNullException();
		else 
			availableStates.Add(newState.name, newState);
	}

	//------------------------------------------------------------------------------------------
    public bool hasState(string stateName)
    {
		return availableStates.ContainsKey(stateName);
	}

	//------------------------------------------------------------------------------------------
    public GameState getState(string stateName)
    {
		if (hasState(stateName))
			return availableStates[stateName];
		else
			return null;
    }
		
	//------------------------------------------------------------------------------------------
	public GameState getCurrentState()
	{
		if (isActiveStatesEmpty())
			return null;
		else
			return activeStates.Peek();
	}
		
	//------------------------------------------------------------------------------------------
	public int getActiveStatesCount()        
	{          
		return activeStates.Count;        
	}

	//------------------------------------------------------------------------------------------
	public void onGUI()
	{
		clearactiveStates = false;
		GameState[] RuntimeState = activeStates.ToArray();
		foreach (GameState state in RuntimeState)
		{
			if (state.updated)
			{
				state.onGUI();
				state.updated = false;
			}
			if (clearactiveStates)
				break;
		}
	}
		
	//------------------------------------------------------------------------------------------
	public void changeState(string nextStateName, Hashtable hashtable = null)
	{
		if (!hasState(nextStateName)) 
			return;

		clearAllActiveStates();
		activeNextState(nextStateName, hashtable);
	}

	//------------------------------------------------------------------------------------------
	public bool pushState(string pushedStateName, Hashtable hashtable)
	{
		if (!hasState(pushedStateName)) return false;

		if (!isActiveStatesEmpty())
		{          
			GameState currentState = getCurrentState();
			currentState.isPlaying = false;      
			currentState.suspend();   
		}
			
		activeNextState(pushedStateName, hashtable);
			
		return true;
	}
		
	//------------------------------------------------------------------------------------------   
	public void popState()    
	{
        popState(null);
    }
    //------------------------------------------------------------------------------------------   
    public void popState(Hashtable table)
    {
        if (isActiveStatesEmpty()) return;

        GameState poppedState = activeStates.Pop();

        poppedState.end();
        poppedState.isPlaying = false;

        if (!isActiveStatesEmpty())
        {
            GameState nextState = getCurrentState();
            //若有傳入userdata則將data傳入resume的state
            if (table != null)
            {
                if (nextState.userData == null)
                    nextState.userData = table;
                else
                {
                    foreach (var key in table.Keys)
                    {
                        if (nextState.userData.ContainsKey(key))
                            nextState.userData[key] = table[key];
                        else
                            nextState.userData.Add(key, table[key]);
                    }
                }
            }
            
            nextState.isPlaying = true;
            nextState.resume();
        }

        if (GameStateChange != null)
        {
            GameStateChange();
        }
    }
    //------------------------------------------------------------------------------------------		
    private void activeNextState(string nextStateName, Hashtable hashtable = null)       
	{          
		Debug.Assert(hasState(nextStateName));
        
		GameState nextState = getState(nextStateName);       
		activeStates.Push(nextState);
		nextState.isPlaying = true;
		nextState.userData = hashtable;
		         
		nextState.begin();    

		if(GameStateChange != null)
		{
			GameStateChange();
		}
	}

	//------------------------------------------------------------------------------------------
	public void clearAllActiveStates()
	{
		foreach (GameState state in activeStates)
		{
			state.end();
			state.isPlaying = false;
		}

		activeStates.Clear();
		clearactiveStates = true;
	}
				
	//------------------------------------------------------------------------------------------		
	public bool checkActiveStates(string stateName)		
	{
		if(isActiveStatesEmpty() == true)
			return false;	
		
		if(string.IsNullOrEmpty(stateName) == true)
			return false;
						
		foreach(GameState gs in activeStates)
		{				
			if(gs.name == stateName)
				return true;
		}
						
		return false;
	}
		
	//------------------------------------------------------------------------------------------  
}

