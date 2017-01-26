using UnityEngine;
using System.Collections;

public class CharacterMoveEvent : MonoBehaviour
{
	UIButton m_button;
	public string m_ObjectName;
	public float m_Radius;
	public ENUM_AUTOOPENUI_TYPE Type;
	bool m_Running;
	delegate Transform FindClosestTransform(Vector3 position, string name);
	FindClosestTransform Find;

	void Awake()
	{
		m_button = GetComponent<UIButton>();
	}

	void Start()
	{

		LobbyState state = ARPGApplication.instance.GetGameStateByName(GameDefine.LOBBY_STATE) as LobbyState;

		Find = state.GetClosestGameObject;
	}

	public void EventStop()
	{
		StopCoroutine("IsArrival");

		if (m_Running)
		{
			ARPGBattle mainBattle =  ARPGApplication.instance.m_tempGameObjectSystem.GetARPGBattleByMain();
			if (mainBattle)
				mainBattle.compNavMeshAgent.Stop();
			m_Running = false;
		}
    }

	void OnDestroy()
	{
		if (m_button)
			EventDelegate.Remove(m_button.onClick, onClick);
	}

	void onClick()
	{
		//UnityDebugger.Debugger.Log("CharacterMoveEvent_onClick");
		StopCoroutine("TriggerMoveEvent");
		StartCoroutine("TriggerMoveEvent");
	}

	IEnumerator TriggerMoveEvent()
	{
		yield return null;
        yield return StartCoroutine("IsArrival");
		int i = 0;
	}

	IEnumerator IsArrival()
	{
		m_Running = true;
		ARPGBattle mainBattle =  ARPGApplication.instance.m_tempGameObjectSystem.GetARPGBattleByMain();

		Transform Target = null;
		if (null != Find)
		{
			Target = Find(mainBattle.transform.position, m_ObjectName);
		}
		else
		{
			GameObject obj = GameObject.Find(m_ObjectName);

			if (obj)
				Target = obj.transform;
		}
		

		if (Target)
		{
			Vector3 pos = Target.position;
			mainBattle.compMovement.SetTargetPosition(pos, m_Radius);
			while(mainBattle.compNavMeshAgent.pathPending)
				yield return null;
			pos = mainBattle.compNavMeshAgent.destination;
            while (true)
			{
				if ((pos - mainBattle.compNavMeshAgent.destination).sqrMagnitude > 1.0f)
					yield break;
				if (m_Radius >= mainBattle.compNavMeshAgent.remainingDistance)
				{
					mainBattle.compNavMeshAgent.Stop();
					ARPGApplication.instance.AutoOpenUI(Type);
					m_Running = false;
					yield break;
				}
				yield return null;
			}
		}
		m_Running = false;
	}

	void EventAdd()
	{
		//UnityDebugger.Debugger.Log("EventAdd" + m_ObjectName);
		EventDelegate.Add(m_button.onClick, onClick);
	}

	void EventRemove()
	{
		//UnityDebugger.Debugger.Log("EventRemove" + m_ObjectName);
		EventDelegate.Remove(m_button.onClick, onClick);
	}
}
