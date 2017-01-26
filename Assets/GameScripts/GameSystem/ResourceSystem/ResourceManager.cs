using UnityEngine;
//using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameScripts.GameSystem.ResourceSystem;
using Softstar;

/// <summary> 資源類型列舉</summary>
public enum Enum_ResourcesType
{
    Atlas = 0,
    Material,
    Particle,
    Songs,
    Sound,
    Notes,
    Texture,
    Scene,
    GUI,
    Max
}

/// <summary>非同步讀取的Operater，用來監控讀取進度</summary>
public class AsyncLoadOperation : IEnumerator
{
    public object Current
    {
        get {return null;}
    }
    public bool MoveNext()
    {
        return !(m_bIsDone);
    }

    public void Reset() {}

    public string m_strAssetName;
    public System.Type m_Type;
    public Object m_assetObject;
    public bool m_bIsDone;
    public bool m_bCanacel;

    public AsyncLoadOperation(string name, System.Type type)
    {
        m_strAssetName = name;
        m_Type = type;
        m_assetObject = null;
        m_bIsDone = false;
        m_bCanacel = false;
    }
    public void CancelLoad()
    {
        m_bCanacel = true;
        m_bIsDone = true;
    }
}

public class ResourceManager
{
	private Dictionary<Enum_ResourcesType, ResourceLoader> m_resDict;

    private Dictionary<string,Object> m_preloadList = new Dictionary<string,Object>();

    //-----------------------------------------------------------------------------------------------------
    public ResourceManager(string assetbundleFolderPath)
    {
        Caching.CleanCache();
        AssetBundleManager.SetAssetbundleFolderPath(assetbundleFolderPath);
        AssetBundleManager.InitializeSync(Softstar.Utility.GetAssetBundleFolderName());

        m_resDict = new Dictionary<Enum_ResourcesType, ResourceLoader>();
//         string txtPath = Application.dataPath + "/../" + "AssetbundleLog.txt";
//         File.Delete(txtPath);
    }

    //-----------------------------------------------------------------------------------------------------
    public void SetAllPriorityLoadPath(string path)
    {
        foreach (ResourceLoader loader in m_resDict.Values)
        {
            loader.SetPriorityLoadPath(path);
        }
    }
    //-----------------------------------------------------------------------------------------------------
    public void SetAllPriorityLoadPath(int index, string path)
    {
        foreach (ResourceLoader loader in m_resDict.Values)
        {
            loader.SetPriorityLoadPath(index, path);
        }
    }

    //-----------------------------------------------------------------------------------------------------
    //Manage Resource Dictionary
    public void AddLoader(MonoBehaviour mono, Enum_ResourcesType type, string path)
	{
		m_resDict [type] = new ResourceLoader(mono, path);
    }
    public bool RemoveLoader(Enum_ResourcesType type)
    {
        if (m_resDict.ContainsKey(type) == false)
            return false;
        return m_resDict.Remove(type);
    }
    public bool CheckLoaderResource(Enum_ResourcesType type, string name)
    {
        if (m_resDict.ContainsKey(type) == false)
            return false;
        return m_resDict[type].Check(name);
    }
    //-----------------------------------------------------------------------------------------------------
    //同步讀取資源
    public GameObject GetResourceSync(Enum_ResourcesType type, string name)
    {
        Object obj = m_resDict[type].GetResourceObj<GameObject>(name);

        if (obj == null)
            return null;
        else
        {
            GameObject go = obj as GameObject;
            return go;
        }     
    }

    public T GetResourceSync<T>(Enum_ResourcesType type, string name)
    {
        object obj = m_resDict[type].GetResourceObj<T>(name);

        if (obj == null)
            return default(T);
        else
        {
            return (T)obj;
        }      
    }
    //-----------------------------------------------------------------------------------------------------
    //非同步讀取資源
    /// <param name="onFinish">讀取完成時事件(若讀取中取消需求並不會執行此事件)</param>
    public AsyncLoadOperation GetResourceASync(Enum_ResourcesType rType, string name, System.Type sType, ResourceLoader.ASyncLoadEvent onFinish)
    {
        return m_resDict[rType].GetResourceRequest(name, sType, onFinish);
    }
   
    //-----------------------------------------------------------------------------------------------------
	UnityEngine.Object CloneObj(UnityEngine.Object sourceObj)
	{
		UnityEngine.Object newObj = Object.Instantiate(sourceObj);
		newObj.name = sourceObj.name;
		return newObj;
	}

    UnityEngine.Object CloneObj(UnityEngine.Object sourceObj, Vector3 position)
    {
        UnityEngine.Object newObj = Object.Instantiate(sourceObj, position, Quaternion.identity);
        newObj.name = sourceObj.name;
        return newObj;
    }

	UnityEngine.Object CloneObj(UnityEngine.Object sourceObj, Vector3 position, Quaternion rotation)
	{
		UnityEngine.Object newObj = Object.Instantiate(sourceObj, position, rotation);
		newObj.name = sourceObj.name;
		return newObj;
	}

	// 釋放所有資源
	public void ReleaseAll()
	{
		foreach(KeyValuePair<Enum_ResourcesType, ResourceLoader> kvp in m_resDict)
		{
			if(kvp.Value!=null)
			{
				kvp.Value.Clear();
			}
		}
		m_resDict.Clear();
	}
}
