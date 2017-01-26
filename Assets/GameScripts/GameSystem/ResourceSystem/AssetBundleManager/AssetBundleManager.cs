using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
/*
 	In this demo, we demonstrate:
	1.	Automatic asset bundle dependency resolving & loading.
		It shows how to use the manifest assetbundle like how to get the dependencies etc.
	2.	Automatic unloading of asset bundles (When an asset bundle or a dependency thereof is no longer needed, the asset bundle is unloaded)
	3.	Editor simulation. A bool defines if we load asset bundles from the project or are actually using asset bundles(doesn't work with assetbundle variants for now.)
		With this, you can player in editor mode without actually building the assetBundles.
	4.	Optional setup where to download all asset bundles
	5.	Build pipeline build postprocessor, integration so that building a player builds the asset bundles and puts them into the player data (Default implmenetation for loading assetbundles from disk on any platform)
	6.	Use WWW.LoadFromCacheOrDownload and feed 128 bit hash to it when downloading via web
		You can get the hash from the manifest assetbundle.
	7.	AssetBundle variants. A prioritized list of variants that should be used if the asset bundle with that variant exists, first variant in the list is the most preferred etc.
*/
namespace Softstar
{
    public enum Enum_ReadResouceType
    {
        Net = 0,
        Local,
    }

    // Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
    public class LoadedAssetBundle
    {
        public AssetBundle m_AssetBundle;
        public int m_ReferencedCount;

        public LoadedAssetBundle(AssetBundle assetBundle)
        {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = 1;
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------
    // Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
    public class AssetBundleManager : MonoBehaviour
    {
        public enum LogMode { All, JustErrors };
        public enum LogType { Info, Warning, Error };

        static LogMode m_LogMode = LogMode.All;
        static string m_BaseDownloadingURL = "";
        static string[] m_ActiveVariants = { };
        static AssetBundleManifest m_AssetBundleManifest = null;

        static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        static Dictionary<string, WWW> m_DownloadingWWWs = new Dictionary<string, WWW>();
        static Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string>();
        static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();
        static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

        static string m_AssetbundleFolderPath;
        //------------------------------------------------------------------------------------------------------------------------------
        public static LogMode logMode
        {
            get { return m_LogMode; }
            set { m_LogMode = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // The base downloading url which is used to generate the full downloading url with the assetBundle names.
        public static string BaseDownloadingURL
        {
            get { return m_BaseDownloadingURL; }
            set { m_BaseDownloadingURL = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // Variants which is used to define the active variants.
        public static string[] ActiveVariants
        {
            get { return m_ActiveVariants; }
            set { m_ActiveVariants = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
        public static AssetBundleManifest AssetBundleManifestObject
        {
            set { m_AssetBundleManifest = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------
        private static void Log(LogType logType, string text)
        {
            if (logType == LogType.Error)
                UnityDebugger.Debugger.LogError("[AssetBundleManager] " + text);
            else if (m_LogMode == LogMode.All)
                UnityDebugger.Debugger.Log("[AssetBundleManager] " + text);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        private static string GetStreamingAssetsPath()
        {
            if (Application.isEditor)
                return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
            else if (Application.isWebPlayer)
                return System.IO.Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/") + "/StreamingAssets";
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
                return Application.streamingAssetsPath;
            else // For standalone player.
                return "file://" + Application.streamingAssetsPath;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        public static void SetSourceAssetBundleDirectory(string relativePath)
        {
            BaseDownloadingURL = GetStreamingAssetsPath() + relativePath;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        public static void SetSourceAssetBundleURL(string absolutePath)
        {
            BaseDownloadingURL = absolutePath + Softstar.Utility.GetAssetBundleFolderName() + "/";
        }
        //------------------------------------------------------------------------------------------------------------------------------
        public static void SetSourceAssetBundleLocal(string path)
        {
            BaseDownloadingURL = "file:///" + path;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        public static void SetAssetbundleFolderPath(string path)
        {
            m_AssetbundleFolderPath = path;
            if (!Directory.Exists(m_AssetbundleFolderPath))
            {
                UnityDebugger.Debugger.Log("Error: AssetbundleFolderPath-------------- [" + m_AssetbundleFolderPath + "] dosent exist!!");
#if UNITY_EDITOR
                Directory.CreateDirectory(m_AssetbundleFolderPath);
#endif
                return;
            }

            SetSourceAssetBundleLocal(m_AssetbundleFolderPath);

            UnityDebugger.Debugger.Log("AssetbundleFolderPath-------------- [" + m_AssetbundleFolderPath + "]");
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // Get loaded AssetBundle, only return valid object when all the dependencies are downloaded successfully.
        static public LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
        {
            if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
                return null;

            LoadedAssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
                return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                if (m_DownloadingErrors.TryGetValue(dependency, out error))
                    return bundle;

                // Wait all the dependent assetBundles being loaded.
                LoadedAssetBundle dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                    return null;
            }

            return bundle;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        static public bool InitializeSync(string assetBundleName)
        {
            var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
            DontDestroyOnLoad(go);

            float startTime = Time.realtimeSinceStartup;

            AssetBundle ab = LoadAssetBundleInternalSync(assetBundleName, true);
            if (ab == null)
            {
                UnityDebugger.Debugger.LogWarning("AssetBundleManager Initialize is fail! Manifest AssetBundle is null! AssetbundleName = "+ assetBundleName);
                return false;
            }

            AssetBundleManifestObject = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

            if (m_AssetBundleManifest == null)
            {
                UnityDebugger.Debugger.LogError("AssetBundleManager Initialize is fail! Manifest LoadAsset is null!");
                return false;
            }

            float elapsedTime = Time.realtimeSinceStartup - startTime;
            //UnityDebugger.Debugger.Log("Finished AssetBundleManagerInit in " + elapsedTime + " seconds");

            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Load AssetBundleManifest.
        static public AssetBundleLoadManifestOperation InitializeASync(string assetBundleName)
        {
            var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
            DontDestroyOnLoad(go);

            LoadAssetBundle(assetBundleName, true);
            var operation = new AssetBundleLoadManifestOperation(assetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
            m_InProgressOperations.Add(operation);
            return operation;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // Load AssetBundle and its dependencies.
        static protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
        {
            Log(LogType.Info, "Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);

            if (!isLoadingAssetBundleManifest)
            {
                if (m_AssetBundleManifest == null)
                {
                    UnityDebugger.Debugger.LogWarning("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                    return;
                }
            }

            // Check if the assetBundle has already been processed.
            bool isAlreadyProcessed = LoadAssetBundleInternalASync(assetBundleName, isLoadingAssetBundleManifest);

            // Load dependencies.
            if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
                LoadDependencies(assetBundleName);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // Remaps the asset bundle name to the best fitting asset bundle variant.
        static protected string RemapVariantName(string assetBundleName)
        {
            if (m_AssetBundleManifest == null)
                return assetBundleName;

            //取出所有 有變體的包
            string[] bundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();
            //將要開啟的包名去除變體綴字
            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                //將所有 有變體的包名去除變體綴字
                string[] curSplit = bundlesWithVariant[i].Split('.');
                //逐一比對 取到同一個包
                if (curSplit[0] != split[0])
                    continue;
                //取得變體包的綴字INDEX
                int found = System.Array.IndexOf(m_ActiveVariants, curSplit[1]);

                // If there is no active variant found. We still want to use the first
                //如果沒找到啟用中的變體綴字(會取得第一個找到的變體包名)
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue - 1)
            {
                UnityDebugger.Debugger.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
            }

            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // Where we actuall call WWW to download the assetBundle.
        static protected bool LoadAssetBundleInternalASync(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            // Already loaded.
            LoadedAssetBundle bundle = null;
            //檢查是否已載入過
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                //增加計數
                bundle.m_ReferencedCount++;
                return true;
            }

            // @TODO: Do we need to consider the referenced count of WWWs?
            // In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
            // But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
            if (m_DownloadingWWWs.ContainsKey(assetBundleName))
                return true;

            WWW download = null;
            string url = m_BaseDownloadingURL + "/" + assetBundleName;

            // For manifest assetbundle, always download it as we don't have hash for it.
            if (isLoadingAssetBundleManifest)
                download = new WWW(url);
            else
                download = new WWW(url);
            //download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(assetBundleName), 0);

            //Log(LogType.Info, "Download Asset Bundle:"+ assetBundleName + " by WWW");
            m_DownloadingWWWs.Add(assetBundleName, download);

            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // Where we get all the dependencies and load them all.
        static protected void LoadDependencies(string assetBundleName)
        {
            if (m_AssetBundleManifest == null)
            {
                UnityDebugger.Debugger.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }

            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
                return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            m_Dependencies.Add(assetBundleName, dependencies);
            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundleInternalASync(dependencies[i], false);
                //Log(LogType.Info, "Loading Asset Bundle " + assetBundleName+ "'s dependencies: "+ dependencies[i]);
        }
              
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // Unload assetbundle and its dependencies.
        static public void UnloadAssetBundle(string assetBundleName)
        {
            //UnityDebugger.Debugger.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);

            UnloadAssetBundleInternal(assetBundleName);
            UnloadDependencies(assetBundleName);

            //UnityDebugger.Debugger.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + assetBundleName);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        static protected void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }

            m_Dependencies.Remove(assetBundleName);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        static protected void UnloadAssetBundleInternal(string assetBundleName)
        {
            string error;
            LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle == null)
                return;

            if (--bundle.m_ReferencedCount == 0)
            {
                bundle.m_AssetBundle.Unload(false);
                m_LoadedAssetBundles.Remove(assetBundleName);

                Log(LogType.Info, assetBundleName + " has been unloaded successfully");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------
        void Update()
        {
            // Collect all the finished WWWs.
            var keysToRemove = new List<string>();
            foreach (var keyValue in m_DownloadingWWWs)
            {
                WWW download = keyValue.Value;

                // If downloading fails.
                if (download.error != null)
                {
                    string error = string.Format("Failed downloading bundle {0} from {1}: {2}", keyValue.Key, download.url, download.error);
                    if (m_DownloadingErrors.ContainsKey(keyValue.Key))
                        m_DownloadingErrors[keyValue.Key] = error;
                    else
                        m_DownloadingErrors.Add(keyValue.Key, error);
                    keysToRemove.Add(keyValue.Key);
                    continue;
                }

                // If downloading succeeds.
                if (download.isDone)
                {
                    AssetBundle bundle = download.assetBundle;
                    if (bundle == null)
                    {
                        string error = string.Format("{0} is not a valid asset bundle.", keyValue.Key);
                        if (m_DownloadingErrors.ContainsKey(keyValue.Key))
                            m_DownloadingErrors[keyValue.Key] = error;
                        else
                            m_DownloadingErrors.Add(keyValue.Key, error);
                        keysToRemove.Add(keyValue.Key);
                        continue;
                    }

                    //UnityDebugger.Debugger.Log("Downloading " + keyValue.Key + " is done at frame " + Time.frameCount);
                    m_LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle));
                    keysToRemove.Add(keyValue.Key);
                }
            }

            // Remove the finished WWWs.
            foreach (var key in keysToRemove)
            {
                WWW download = m_DownloadingWWWs[key];
                m_DownloadingWWWs.Remove(key);
                download.Dispose();
            }

            // Update all in progress operations
            for (int i = 0; i < m_InProgressOperations.Count;)
            {
                if (!m_InProgressOperations[i].Update())
                {
                    m_InProgressOperations.RemoveAt(i);
                }
                else
                    i++;
            }
        }


        //------------------------------------------------------------------------------------------------------------------------------
        // Load asset from the given assetBundle.
        static public AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
        {
            if (m_AssetBundleManifest == null)
            {
                UnityDebugger.Debugger.LogWarning("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return null;
            }

            Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");

            AssetBundleLoadAssetOperation operation = null;

            assetBundleName = RemapVariantName(assetBundleName);
            LoadAssetBundle(assetBundleName);
            operation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);

            m_InProgressOperations.Add(operation);

            return operation;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // Load level from the given assetBundle.
        static public AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
        {
            Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");

            AssetBundleLoadOperation operation = null;

            assetBundleName = RemapVariantName(assetBundleName);
            LoadAssetBundle(assetBundleName);
            operation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);

            m_InProgressOperations.Add(operation);

            //預載入場景者 保留此物件以讀取進度
            return operation;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 取得場景載入進度，查無資料時回傳-1
        /// </summary>
        /// <param name="assetBundleName">預載入場景之名稱</param>
        /// <returns></returns>
        public static float GetLoadLevelprogressByAssetName(string assettName)
        {
            for (int i = 0; i < m_InProgressOperations.Count; i++)
            {
                if (m_InProgressOperations[i].GetAssetName() == assettName)
                {
                    AssetBundleLoadLevelOperation assLoadOperation = m_InProgressOperations[i] as AssetBundleLoadLevelOperation;
                    if (assLoadOperation != null)
                    {
                        return assLoadOperation.Progress;
                    }
                }
            }
            return -1f;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 同步方式載入物件
        /// </summary>
        /// <param name="assetBundleName">完整包名</param>
        /// <returns></returns>
        static public AssetBundle LoadAssetSync(string assetBundleName)
        {
            //UnityDebugger.Debugger.Log("Loading " + assetBundleName + " bundle");
            //AssetBundle名子檢查
            assetBundleName = RemapVariantName(assetBundleName);
            //讀取
            AssetBundle bundle = LoadAssetBundleInternalSync(assetBundleName);

            if (bundle != null)
            {
                UnityDebugger.Debugger.Log("Loading " + assetBundleName + " bundle is done!!");
                return bundle;
            }

            UnityDebugger.Debugger.LogWarning("Loading " + assetBundleName + " bundle is Fail!!");
            return null;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        static private AssetBundle LoadAssetBundleInternalSync(string assetBundleName,bool isManifest = false)
        {	
            string assetBundlesFolder = m_AssetbundleFolderPath;    //<---指定打包資料夾名稱
            //先檢查是否已載入過                                                                                           
            LoadedAssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                //直接取出並增加計數
                bundle.m_ReferencedCount++;
                return bundle.m_AssetBundle;
            }
            //檢查Manifest檔
            if(!isManifest)
            {
                if (m_AssetBundleManifest == null)
                {
                    UnityDebugger.Debugger.LogWarning("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                    return null;
                }

                //載入相依性檔案
                string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
                if (dependencies.Length != 0)
                {
                    string dpsPath = "";
                    //重組名子
                    for (int i = 0; i < dependencies.Length; i++)
                        dependencies[i] = RemapVariantName(dependencies[i]);
                    //加入管理
                    m_Dependencies.Add(assetBundleName, dependencies);
                    //逐個載入
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        bundle = null;
                        //先檢查是否已載入過
                        m_LoadedAssetBundles.TryGetValue(dependencies[i], out bundle);
                        if (bundle != null)
                        {
                            //增加計數
                            bundle.m_ReferencedCount++;
                            continue;
                        }
                        //檢查檔案路徑存在
                        dpsPath = Path.Combine(assetBundlesFolder, dependencies[i]);
                        if (!File.Exists(dpsPath))
                        {
                            UnityDebugger.Debugger.Log("Loading AssetBundle dependencies " + dependencies[i] + " Not Found File!");
                            return null;
                        }

                        AssetBundle assetBundle = AssetBundle.LoadFromFile(dpsPath);
                        if (assetBundle == null)
                        {
                            UnityDebugger.Debugger.Log("Loading AssetBundle dependencies " + dependencies[i] + " is null!!");
                            return null;
                        }
                        UnityDebugger.Debugger.Log("Loading AssetBundle dependencies " + dependencies[i] + " is Done!!");
                        //加入管理
                        m_LoadedAssetBundles.Add(dependencies[i], new LoadedAssetBundle(assetBundle));
                    }
                }
            }
 
            //檢查檔案路徑存在
            string assetBundlesPath = Path.Combine(assetBundlesFolder, assetBundleName);
            if (!File.Exists(assetBundlesPath))
            {
                UnityDebugger.Debugger.Log("Loading AssetBundle " + assetBundleName + " Not Found File!");
                return null;
            }

            AssetBundle m_AB = AssetBundle.LoadFromFile(assetBundlesPath);
            if (m_AB == null)
            {
                UnityDebugger.Debugger.Log("Loading AssetBundle " + assetBundleName + " is null!!");
                return null;
            }
            //加入管理
            m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(m_AB));

            return m_AB;
        }


    } // End of AssetBundleManager.
}
