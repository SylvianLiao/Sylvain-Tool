using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Softstar;

//----------------------------------------------------------------------------------------------
// 負責資源載入
public class ResourceLoader
{
    private MonoBehaviour m_mono;			
	//資源庫
	private Dictionary<string, Object> m_ResourceObj;
    //優先讀取路徑
    private List<string> m_priorityLoadPath = new List<string>();
    //資源資料夾名稱
    public string m_strResourceFolderName = "";

    //ASync Load Finish Delegate
    public delegate void ASyncLoadEvent(AsyncLoadOperation load);
    //------------------------------------------------------------------------------------------
    public ResourceLoader(MonoBehaviour mono, string path)
    {
        m_mono = mono;
        m_strResourceFolderName = path;
        m_ResourceObj = new Dictionary<string, Object>();
    }

    //------------------------------------------------------------------------------------------
    //設定優先讀取AssetBundle的路徑
    public void SetPriorityLoadPath(string path)
    {
        m_priorityLoadPath.Add(path);
    }
    public void SetPriorityLoadPath(int index, string path)
    {
        if (index >= m_priorityLoadPath.Count)
            return;
        m_priorityLoadPath[index] = path;
    }
    public void RemovePriorityLoadPath(string path)
    {
        m_priorityLoadPath.Remove(path);
    }
    public void ClearPriorityLoadPath()
    {
        m_priorityLoadPath.Clear();
    }
    public List<string> GetPriorityLoadPath()
    {
        return m_priorityLoadPath;
    }
    #region Load With Sync
    //------------------------------------------------------------------------------------------
    //同步讀取-取得資源料件
    public Object GetResourceObj<T>(string name)
    {
        for (int i = 0, iCount = m_priorityLoadPath.Count; i < iCount; ++i)
        {
            Object obj = null;
            obj = GetResourceObj<T>(m_priorityLoadPath[i], name);

            if (obj != null)
                return obj;
        }

        return null;
    }

    private Object GetResourceObj<T>(string priorityPath, string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        Object obj = null;

        if (m_ResourceObj.ContainsKey(name) && m_ResourceObj[name] != null)
        {
            obj = m_ResourceObj[name];

            if (obj == null)
            {
                m_ResourceObj.Remove(name);
            }
        }

        if (obj == null)
        {
            obj = LoadFromAssetBundleSync<T>(priorityPath, name);
        }

        if (obj == null)
        {
            obj = LoadFromResourcesSync<T>(priorityPath, name);
        }


        return obj;
    }
    //------------------------------------------------------------------------------------------
    //從Resource資料夾載入資源
    private Object LoadFromResourcesSync<T>(string priorityPath, string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        string resourcePath = priorityPath + m_strResourceFolderName + "/" + name;

        Object asset = Resources.Load(resourcePath, typeof(T));

        if (asset == null)
        {
            Resources.UnloadAsset(asset);
            UnityDebugger.Debugger.LogWarning("Resources.Load == null : " + resourcePath);
            return null;
        }
        else
        {
            m_ResourceObj.Add(name, asset);
            return asset;
        }
    }
    //------------------------------------------------------------------------------------------
    //從AssetBundleManager載入資源
    private Object LoadFromAssetBundleSync<T>(string priorityPath, string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        //組合AssetBundleName
        string assetBundleName = priorityPath + m_strResourceFolderName + "/" + name;
        assetBundleName = assetBundleName.ToLower();

        //RecordABLoad("Start Load " + assetBundleName + "\n");
        //讀取
        AssetBundle assetBundle = AssetBundleManager.LoadAssetSync(assetBundleName);

        if (assetBundle == null)
            return null;

        //從AssetBundle取出物件
        Object asset = assetBundle.LoadAsset(assetBundle.GetAllAssetNames()[0], typeof(T));

        if (asset == null)
        {
            UnityDebugger.Debugger.LogWarning("assetBundle.LoadAsset == null : " + assetBundleName);
            return null;
        }
        else
        {
#if UNITY_EDITOR
            if (asset is GameObject)
            {
                GameObject go = asset as GameObject;
                Softstar.Utility.ReApplyShaders(go);
            }      
            else if (asset is Material)
            {
                Material mat = asset as Material;
                Softstar.Utility.ReApplyMaterialShader(mat);
            }
#endif
            m_ResourceObj.Add(name, asset);
            UnityDebugger.Debugger.Log("Load Success " + assetBundleName);
            //RecordABLoad("Load Success " + assetBundleName + "\n");
            return asset;
        }
    }
#endregion

#region Load With ASync
    //------------------------------------------------------------------------------------------
    //非同步讀取-取得資源要求
    public AsyncLoadOperation GetResourceRequest(string name, System.Type sType, ASyncLoadEvent onFinish)
    {
        AsyncLoadOperation loadOP = new AsyncLoadOperation(name, sType);

        if (m_ResourceObj.ContainsKey(name))
        {
            loadOP.m_assetObject = m_ResourceObj[name];
            loadOP.m_bIsDone = true;
            if (onFinish != null)
                onFinish(loadOP);
        }
        else
        {
            m_mono.StartCoroutine(GetResourceRequest(name, onFinish, loadOP));
        }
        return loadOP;
    }
    //------------------------------------------------------------------------------------------
    public IEnumerator GetResourceRequest(string name,ASyncLoadEvent onFinish, AsyncLoadOperation loadOP)
    {
        //Check load request is canceled or not before sending request.
        if (loadOP.m_bCanacel)
        {
            yield break;
        }

        //Start Load Resource
        for (int i = 0, iCount = m_priorityLoadPath.Count; i < iCount; ++i)
        {
            yield return m_mono.StartCoroutine(GetResourceRequest(m_priorityLoadPath[i], name, onFinish, loadOP));
        }

        loadOP.m_bIsDone = true;

        if (onFinish != null)
            onFinish(loadOP);
    }
    //------------------------------------------------------------------------------------------
    public IEnumerator GetResourceRequest(string priorityPath, string name, ASyncLoadEvent onFinish, AsyncLoadOperation loadOP)
    {
        if (m_ResourceObj.ContainsKey(name) == false)
        {
            yield return m_mono.StartCoroutine(LoadFromAssetBundleASync(priorityPath, name, loadOP));
        }

        if (m_ResourceObj.ContainsKey(name) == false)
        {
            yield return m_mono.StartCoroutine(LoadFromResourcesASync(priorityPath, name, loadOP));
        }

        if (!m_ResourceObj.ContainsKey(name))
            yield break;

        Object obj = m_ResourceObj[name];
        if (obj != null)
        {
            //Check load request is canceled or not after request load is done.
            if (loadOP.m_bCanacel)
            {
                obj = null;
            }
            else
            {
                loadOP.m_assetObject = obj;
            }
        }

        if (obj == null)
        {
            m_ResourceObj.Remove(name);
        }
    }
    //------------------------------------------------------------------------------------------
    private IEnumerator LoadFromResourcesASync(string priorityPath, string name, AsyncLoadOperation loadOP)
    {
        if (string.IsNullOrEmpty(name) || loadOP.m_bCanacel)
            yield break;

        string resourcePath = priorityPath + m_strResourceFolderName + "/" + name;

        //讀取
        ResourceRequest request = Resources.LoadAsync(resourcePath);
        if (request == null)
        {
            UnityDebugger.Debugger.LogWarning("ResourceRequest == null : " + resourcePath);
            yield break;
        }

        yield return request;

        yield return request.isDone;

        Object asset = request.asset;
        if (asset == null)
        {
            Resources.UnloadAsset(asset);
            UnityDebugger.Debugger.LogWarning("ResourceRequest.asset " + resourcePath + " == null");
            yield break;
        }
        else if (!loadOP.m_bCanacel)
        {
            m_ResourceObj.Add(name, asset);          
        }
    }
    //------------------------------------------------------------------------------------------
    private IEnumerator LoadFromAssetBundleASync(string priorityPath, string name, AsyncLoadOperation loadOP)
    {
        if (string.IsNullOrEmpty(name) || loadOP.m_bCanacel)
            yield break;

        //組合AssetBundleName
        string assetBundleName = priorityPath + m_strResourceFolderName + "/" + name;
        assetBundleName = assetBundleName.ToLower();
        //資源名稱"name"有可能包含路徑，但讀取Assetbundle時用的"AssetName"為不含路徑的檔案名稱，故在此做特別處理
        string assetName = name.Substring(name.LastIndexOf("/") + 1);

        //RecordABLoad("Start Load " + assetBundleName + "\n");
        //讀取
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(UnityEngine.Object));
        if (request == null)
        {
            UnityDebugger.Debugger.LogWarning("AssetBundleRequest == null : " + assetBundleName);
            yield break;
        }

        yield return request;

        UnityDebugger.Debugger.Log("LoadFromAssetBundleASync request is done: " + assetBundleName);
        Object asset = request.GetAsset<Object>();
        if (asset == null)
        {
            UnityDebugger.Debugger.LogWarning("AssetBundleLoadAssetOperation.GetAsset()" + assetBundleName + " == null");
            yield break;
        }
        else if (!loadOP.m_bCanacel)
        {
#if UNITY_EDITOR
            if (asset is GameObject)
            {
                GameObject go = asset as GameObject;
                Softstar.Utility.ReApplyShaders(go);
            }
            else if (asset is Material)
            {
                Material mat = asset as Material;
                Softstar.Utility.ReApplyMaterialShader(mat);
            }
#endif
            m_ResourceObj.Add(name, asset);
            UnityDebugger.Debugger.Log("Load Success " + assetBundleName);
            //RecordABLoad("Load Success " + assetBundleName + "\n");
        }
    }
#endregion
    //------------------------------------------------------------------------------------------
    //Manage Resource Container
    public void Add(string name, Object obj)
    {
        m_ResourceObj[name] = obj;
    }
    public bool Check(string name)
    {
        return m_ResourceObj.ContainsKey(name);
    }
    //清除已載入的資源
    public void Clear()
	{
		m_ResourceObj.Clear();
	}

    public void RecordABLoad(string abName)
    {
        string strFilter = "3d";
        if (!abName.Contains(strFilter))
            return;
        FileStream file;
        string originContent = "";
        string txtPath = Application.dataPath + "/../" + "AssetbundleLog.txt";
        if (!File.Exists(txtPath))
        {
            file = new FileStream(txtPath, FileMode.Create);
        }   
        else
        {
            //file = File.Open(txtPath, FileMode.Open);
            StreamReader reader = File.OpenText(txtPath);
            originContent = reader.ReadToEnd();
            reader.Close();
            file = File.OpenWrite(txtPath);
        }     
        StreamWriter writer = new StreamWriter(file);
        writer.Write(originContent);
        writer.Write(abName);
        writer.Close();
    }
}




