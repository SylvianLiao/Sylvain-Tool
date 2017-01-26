using UnityEngine;
using System.Collections;

public class BuffManager : MonoBehaviour
{
	public UIGrid Buffs;
	public UIGrid Debuffs;
	public BuffData Template;
	bool isLeft;
	void Awake()
	{
		Template.gameObject.SetActive(false);
	}

	// 新增Buff
	public void AddBuff(int GUID)
	{
		S_BuffData_Tmp buff_tmp = GameDataDB.BuffDataDB.GetData(GUID);

		if (null == buff_tmp)
			return;

		if (null == Template)
			return;

		UIGrid parent;
		if (buff_tmp.iType == 0)
		{
			parent = Buffs;
		}
		else if (buff_tmp.iType == 1)
		{
            parent = Debuffs;
        }
        else
			return;

		//int lastSerialNo = GetLastBuffSerialNo(parent.gameObject);
		GameObject newBuffGameObject = NGUITools.AddChild(parent.gameObject, Template.gameObject);
		newBuffGameObject.SetActive(true);
		BuffData newBuffData = newBuffGameObject.GetComponent<BuffData>();

		if (null != newBuffData)
		{
			newBuffData.SetData(buff_tmp, 0);
			parent.repositionNow = true;
		}

		if (parent.pivot == UIWidget.Pivot.Left)
		{
			SortSerialNo_B2S(parent.transform);
		}
		else if(parent.pivot == UIWidget.Pivot.Right)
		{
			SortSerialNo_S2B(parent.transform);
		}


	}

	void SortSerialNo_S2B (Transform transform)
	{
		int count = transform.childCount;
		for(int i =0 ;i < count;i++)
		{
			transform.GetChild(i).name = i.ToString();
		}
	}

	void SortSerialNo_B2S (Transform transform)
	{
		int count = transform.childCount;
		for(int i =0 ;i < count;i++)
		{
			transform.GetChild(i).name = (count - i).ToString();
		}
	}

	// 清除Buff 資訊
	public void RemoveBuff(int GUID)
    {
		BuffData buff;
		buff = GetBuffData(Buffs.gameObject, GUID);

		if (null != buff)
		{
			buff.gameObject.SetActive(false);
			GameObject.Destroy(buff.gameObject);
			Buffs.repositionNow = true;
		}
		else
		{
			buff = GetBuffData(Debuffs.gameObject, GUID);
			if (null != buff)
			{
				buff.gameObject.SetActive(false);
				GameObject.Destroy(buff.gameObject);
				Debuffs.repositionNow = true;
            }
		}
    }

	// 清除Buff 資訊
	public void Clear()
	{
		if (null != Buffs)
		{
			BuffData[] allbuffs = Buffs.GetComponentsInChildren<BuffData>();

			foreach(BuffData data in allbuffs)
			{
				GameObject.Destroy(data.gameObject);
				data.gameObject.SetActive(false);
			}
			Buffs.repositionNow = true;
		}

		if (null != Debuffs)
		{
			BuffData[] allbuffs = Debuffs.GetComponentsInChildren<BuffData>();
			
			foreach(BuffData data in allbuffs)
			{
                GameObject.Destroy(data.gameObject);
				data.gameObject.SetActive(false);
			}
			Debuffs.repositionNow = true;
        }

	}

	int GetLastBuffSerialNo (GameObject parent)
	{
		BuffData[] allbuffs = parent.GetComponentsInChildren<BuffData>();

		int max = 0;
		foreach(BuffData data in allbuffs)
		{
			max = Mathf.Max(max, data.SerialNo);
        }
		return max;
    }

	BuffData GetBuffData (GameObject parent, int GUID)
	{
		BuffData[] allbuffs = parent.GetComponentsInChildren<BuffData>();
		
		int max = 0;
		foreach(BuffData data in allbuffs)
		{
			if(data.GUID == GUID)
				return data;
        }
        return null;
    }
    
}
