using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Softstar;

[Serializable]
public class GameDataDB
{
    // 存放DBF資料T_GameDB<T>的Dictionary
    private Dictionary<string, object> m_GameDBMap;

    // 檔案放置路徑
    private string m_DBFPath;

    private IConverter m_Conevrter;

    private MainApplication m_mainApp;
    //-----------------------------------------------------------------------------------------
    public GameDataDB(GameScripts.GameFramework.GameApplication app) 
    {
        m_GameDBMap = new Dictionary<string, object>();
        m_mainApp = app as MainApplication;
    }

    //-----------------------------------------------------------------------------------------
    public void SetConevrter(IConverter conevrter)
    {
        m_Conevrter = conevrter;
    }

    //-----------------------------------------------------------------------------------------
    public void SetDBFPath(string path)
    {
        m_DBFPath = path;
    }

    //-------------------------------------------------------------------------------------------
    // 取得T_GameDB
    public T_GameDB<T> GetGameDB<T>() where T : I_BaseDBF
    {
        string strName = typeof(T).Name;
        if (m_GameDBMap.ContainsKey(strName))
        {
            return m_GameDBMap[strName] as T_GameDB<T>;
        }
        return null;
    }

    //-------------------------------------------------------------------------------------------
    // 新增一個T_GameDB
    public void AddGameDB<T>(string fileName, bool clearData) where T : I_BaseDBF
    {
        string strName = typeof(T).Name;
        T_GameDB<T> gameDB;

        if (m_GameDBMap.ContainsKey(strName))
        {
            gameDB = m_GameDBMap[strName] as T_GameDB<T>;

            if (LoadFromFile<T>(gameDB, m_DBFPath + fileName, clearData))
                m_GameDBMap[strName] = gameDB;
        }
        else
        {
            gameDB = new T_GameDB<T>();
            if (LoadFromFile<T>(gameDB, m_DBFPath + fileName, clearData))
                m_GameDBMap.Add(strName, gameDB);
        }
    }

    //-------------------------------------------------------------------------------------------
    // 讀入一個樣版檔
    private bool LoadFromFile<T>(T_GameDB<T> GameDB, string DataPath, bool ClearData) where T : I_BaseDBF
    {
        if (ClearData)
            GameDB.Clear();

        string name = typeof(T).Name;
        string Msg = "Load DBF[" + name + "] From [" + DataPath + "] ";
        //UnityDebugger.Debugger.Log(Msg + " Loading");
        UnityDebugger.Debugger.Log(Msg + " Loading");

        // 檢查有沒有dbf檔是否存在        
        if (!File.Exists(DataPath))
        {
            UnityDebugger.Debugger.LogWarning(DataPath + "Read Failed!!");
            return false;
        }

        StreamReader stream = new StreamReader(DataPath);
        if (stream != null)
        {
            string str = stream.ReadToEnd();

            List<T> datas = CovertData<T>(str);
            GameDB.AddDataFromList(datas);
            //UnityDebugger.Debugger.Log(Msg + "OK");

            return true;
        }

        UnityDebugger.Debugger.LogError(Msg + " can not be loaded");
        return false;
    }

    //-----------------------------------------------------------------------------------------
    // 將DBF檔中的資料取出後存在List中
    private List<T> CovertData<T>(string txt) where T : I_BaseDBF
    {
        List<T> datas = new List<T>();
        string sp = "\r\n";
        string[] strs = txt.Split(sp.ToCharArray());
        Type type = typeof(T);

        foreach (string str in strs)
        {
            if (str == "")
                continue;

			try
			{
	            T data = m_Conevrter.deserializeObject<T>(str);
                data.ParseJson(str, m_Conevrter, data);
	            datas.Add((T)data);
			}
			catch (System.Exception ex)
			{
				//UnityDebugger.Debugger.LogError(ex.ToString());
                //UnityDebugger.Debugger.LogError("deserialize Error:" + str);
			}
        }

        return datas;
    }
    //<歌曲群組編號, 歌曲樣版資料清單(以難度分類)>
    public Dictionary<int, List<S_Songs_Tmp>> m_songTmpDict = new Dictionary<int, List<S_Songs_Tmp>>();
    //-----------------------------------------------------------------------------------------
    public void SortSongTmpList()
    {
        T_GameDB<S_Songs_Tmp> songDB = GetGameDB<S_Songs_Tmp>();
        songDB.ResetByOrder();
        for (int i = 0; i < songDB.GetDataSize(); ++i)
        {
            S_Songs_Tmp songTmp = songDB.GetDataByOrder();
            if (m_songTmpDict.ContainsKey(songTmp.iGroupID) == false)
                m_songTmpDict.Add(songTmp.iGroupID, new List<S_Songs_Tmp>());
            m_songTmpDict[songTmp.iGroupID].Add(songTmp);
        }
    }
}

//========================================================================================
//可以取得GUID的Interface
public interface I_BaseDBF
{
    // 取得GUID
    int GetGUID();
    // 需要額外Parse時會由  GameDataDB.CovertData() 中呼叫
    void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record);
}

//========================================================================================
// DBF樣版
public class T_GameDB<T> where T : I_BaseDBF
{
    SortedDictionary<int, T> m_ContainerObject = new SortedDictionary<int, T>();
    SortedDictionary<int, T>.Enumerator m_UseIter;

    public T_GameDB()
    { }
    //-----------------------------------------------
    // 從List中加入資料
    public void AddDataFromList(List<T> datas)
    {
        foreach (T newData in datas)
            AddData(newData);
    }
    //-----------------------------------------------
    // 直接加入一筆
    public void AddData(T data)
    {
        if (data == null)
            return;

        int id = data.GetGUID();
        if (m_ContainerObject.ContainsKey(id))
            UnityDebugger.Debugger.LogWarning(typeof(T) + " has repeat GUID:" + id);
        else
            m_ContainerObject.Add(id, data);
    }
    //------------------------------------------------
    // 取得
    public T GetData(int id)
    {
        if (m_ContainerObject.ContainsKey(id))
        {
            return m_ContainerObject[id];
        }
        return default(T);
    }
    //------------------------------------------------
    // 筆數
    public int GetDataSize()
    {
        return m_ContainerObject.Count;
    }

    //------------------------------------------------
    // 清空
    public void Clear()
    {
        m_ContainerObject.Clear();
    }

    //--------------------------------------------------------------------
    // 快速走訪使用
    public void ResetByOrder()
    {
        m_UseIter = m_ContainerObject.GetEnumerator();
    }
    //--------------------------------------------------------------------
    public T GetDataByOrder()
    {
        if (m_UseIter.MoveNext() == false)
            return default(T);

        return m_UseIter.Current.Value;
    }
}
