using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetbundles
{
    private const string ASSETBUNDLE_MENUITEM = Softstar.Utility.RESOURCE_PATH+"/Assetbundles/";
    public CreateAssetbundles() { }
    #region Build All
    [MenuItem(ASSETBUNDLE_MENUITEM + "Build/AllBuild/LZMA", false, 0)]
    static void BuildAssetBundlesWithLZMA()
    {
        DoBuildAllAssetBundle(BuildAssetBundleOptions.None);
    }
    //-------------------------------------------------------------------------------------
    [MenuItem( ASSETBUNDLE_MENUITEM+"Build/AllBuild/LZ4" , false , 0 )]
    static void BuildAssetBundlesWithLZ4()
    {
        DoBuildAllAssetBundle( BuildAssetBundleOptions.ChunkBasedCompression );
    }
    #endregion
    #region Create Assetbundles
    //-------------------------------------------------------------------------------------
    static void DoBuildAllAssetBundle(BuildAssetBundleOptions buildOptions)
    {
        string abPath = Application.dataPath + "/../" + Softstar.Utility.GetAssetBundleFolderName();
        if (Directory.Exists(abPath) == false)
            Directory.CreateDirectory(abPath);

        BuildPipeline.BuildAssetBundles(Softstar.Utility.GetAssetBundleFolderName(),
                                        buildOptions | BuildAssetBundleOptions.DeterministicAssetBundle,
                                        EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();
        UnityDebugger.Debugger.Log("Build Selected AssetBundles Is Done!!");
    }
    //-------------------------------------------------------------------------------------
    static void DoBuildSpecifiedAssetBundle(AssetBundleBuild[] buildmap, BuildAssetBundleOptions buildOptions)
    {
        string abPath = Application.dataPath + "/../" + Softstar.Utility.GetAssetBundleFolderName();
        if (Directory.Exists(abPath) == false)
            Directory.CreateDirectory(abPath);

        BuildPipeline.BuildAssetBundles(Softstar.Utility.GetAssetBundleFolderName(),
                                        buildmap,
                                        buildOptions | BuildAssetBundleOptions.DeterministicAssetBundle,
                                        EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();
        UnityDebugger.Debugger.Log( "Build Selected AssetBundles Is Done!!" );
    }
    //-------------------------------------------------------------------------------------
    private static AssetBundleBuild[] CreateBuildMap(UnityEngine.Object[] selectObjs)
    {
        List<UnityEngine.Object> allNeedBuildObjs = new List<UnityEngine.Object>();
        allNeedBuildObjs.AddRange(selectObjs);
        //Add legal asset
        for (int i = 0, iCount = selectObjs.Length; i < iCount; ++i)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectObjs[i]);
            //Add denpendencies assets
            string[] dpsPath = AssetDatabase.GetDependencies( new string[] { assetPath });
            for (int m = 0, mCount = dpsPath.Length; m < mCount; ++m)
            {
                UnityEngine.Object dpsObj = AssetDatabase.LoadMainAssetAtPath(dpsPath[m]);
                if (dpsObj == null || allNeedBuildObjs.Contains(dpsObj))
                    continue;
                else if (CheckAssetIsCorrectToBuild(dpsObj) == false)
                    continue;
                allNeedBuildObjs.Add(dpsObj);
            }
        }

        List<AssetBundleBuild> buildMapList = new List<AssetBundleBuild>();
        for (int i = 0, count = allNeedBuildObjs.Count; i < count; ++i)
        {
            AssetBundleBuild buildmap = CreateSingleBuildMap(allNeedBuildObjs[i]);
            if (string.IsNullOrEmpty(buildmap.assetBundleName))
                continue;
            buildMapList.Add(buildmap);
        }

        return buildMapList.ToArray();
    }
    //-------------------------------------------------------------------------------------
    private static AssetBundleBuild CreateSingleBuildMap(string assetPath)
    {
        AssetBundleBuild buildmap = new AssetBundleBuild();
        if (string.IsNullOrEmpty(assetPath))
            return buildmap;

        AssetImporter importer = AssetImporter.GetAtPath(assetPath);

        if (string.IsNullOrEmpty(importer.assetBundleName))
        {
            Debug.LogError("Asset=[" + assetPath + "] dosnt have the assetbundle name!!");
            return buildmap;
        }

        buildmap.assetBundleName = importer.assetBundleName;
        buildmap.assetBundleVariant = importer.assetBundleVariant;

        string[] assetsName = new string[1];
        assetsName[0] = assetPath;
        buildmap.assetNames = assetsName;
        return buildmap;
    }
    //-------------------------------------------------------------------------------------
    private static AssetBundleBuild CreateSingleBuildMap(UnityEngine.Object obj)
    {
        if (obj == null)
            return new AssetBundleBuild();

        string assetPath = AssetDatabase.GetAssetPath(obj);

        return CreateSingleBuildMap(assetPath);
    }
    //-------------------------------------------------------------------------------------
    private static UnityEngine.Object[] FindAllCanBuiltObjs()
    {
        List<UnityEngine.Object> allAssetObjs = new List<UnityEngine.Object>();

        for (int i = 0, iCount = (int)Enum_ResourcesType.Max; i < iCount; ++i)
        {
            UnityEngine.Object[] objs = FindOneTypeCanBuiltObjs((Enum_ResourcesType)i);
            if (objs != null)
            {
                allAssetObjs.AddRange(objs);
            }
        }

        return allAssetObjs.ToArray();
    }
    //-------------------------------------------------------------------------------------
    private static UnityEngine.Object[] FindOneTypeCanBuiltObjs(Enum_ResourcesType type)
    {
        string typeFolderName = Softstar.Utility.GetResourceTypeFolder(type);
        if (string.IsNullOrEmpty(typeFolderName))
            return null;

        //Define the specified folder to build.
        string folderPath = Application.dataPath + "/" + Softstar.Utility.RESOURCE_PATH + "/" + typeFolderName;
        if (!Directory.Exists(folderPath))
        {
            Debug.Log("Resource folder ["+ folderPath+"] doesn't exist!");
            return null;
        }
        DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
        FileInfo[] filesInfo = folderInfo.GetFiles("*", SearchOption.AllDirectories);

        //Find out all asset objs in specified folder.
        List<UnityEngine.Object> assetObjs = new List<UnityEngine.Object>();
        for (int i = 0, count = filesInfo.Length; i < count; ++i)
        {
            string assetPath = "Assets" + filesInfo[i].FullName.Substring(Application.dataPath.Length);
            UnityEngine.Object assetObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (assetObj == null)
                continue;
            assetObjs.Add(assetObj);
        }

        return GetCorrectAssets(type, assetObjs.ToArray());
    }
    //-------------------------------------------------------------------------------------
    private static UnityEngine.Object[] GetCorrectAssets(Enum_ResourcesType type, UnityEngine.Object[] selectObjs)
    {
        List<UnityEngine.Object> selectObjsList = new List<UnityEngine.Object>();
        for (int i = 0, iCount = selectObjs.Length; i < iCount; ++i)
        {
            UnityEngine.Object obj = selectObjs[i];
            string assetPath = AssetDatabase.GetAssetPath(obj);
            switch (type)
            {
                case Enum_ResourcesType.Atlas:
                case Enum_ResourcesType.GUI:
                case Enum_ResourcesType.Notes:
                    if (obj is GameObject == false)
                        continue;
                    break;
                case Enum_ResourcesType.Songs:
                    if (obj is AudioClip == false && obj is TextAsset == false)
                        continue;
                    break;
                case Enum_ResourcesType.Sound:
                    if (obj is AudioClip == false)
                        continue;
                    break;
                case Enum_ResourcesType.Material:
                    if (obj is Material == false)
                        continue;
                    break;
                case Enum_ResourcesType.Scene:
                    if (obj is SceneAsset == false)
                        continue;
                    break;
                case Enum_ResourcesType.Texture:
                    if (obj is Texture2D == false)
                        continue;
                    break;
                default:
                    break;
            }
            selectObjsList.Add(selectObjs[i]);
        }

        return selectObjsList.ToArray();
    }
    //-------------------------------------------------------------------------------------
    private static bool CheckDependenciesIsCorrectToName(string assetPath)
    {
        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (obj == null)
            return false;

        //Filtered the script 
        if (obj.GetType().Equals(typeof(MonoScript)))
            return false;

        //Filtered the asset which already have AB name, then we dont rename it.
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        if (string.IsNullOrEmpty(importer.assetBundleName) == false)
            return false;

        return true;
    }
    //-------------------------------------------------------------------------------------
    private static bool CheckAssetIsCorrectToBuild(UnityEngine.Object obj)
    {
        string assetPath = AssetDatabase.GetAssetPath(obj);
        return CheckAssetIsCorrectToBuild(assetPath);
    }
    //-------------------------------------------------------------------------------------
    private static bool CheckAssetIsCorrectToBuild(string assetPath)
    {
        //Filtered the asset which have no AB name
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        if (string.IsNullOrEmpty(importer.assetBundleName))
            return false;

        return true;
    }
    #endregion
    #region Build Constant Directory
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "Build/ConstantBuild/LZMA", false, 0)]
    static void BuildConstantAssetBundlesWithLZMA()
    {
        BuildConstantAssetBundles(BuildAssetBundleOptions.None);
    }
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "Build/ConstantBuild/LZ4", false, 0)]
    static void BuildConstantAssetBundlesLZ4()
    {
        BuildConstantAssetBundles(BuildAssetBundleOptions.ChunkBasedCompression);
    }
    //-------------------------------------------------------------------------------------
    static void BuildConstantAssetBundles(BuildAssetBundleOptions buildOptions)
    {
        AssetBundleBuild[] buildmap = CreateBuildMap(FindAllCanBuiltObjs());

        DoBuildSpecifiedAssetBundle(buildmap, buildOptions);
    }
    #endregion
    /*
    #region Build Selected Directory
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "Build/SelectedBuild/LZMA", false, 0)]
    static void BuildSelectAssetBundleWithLZMA()
    {
        BuildSelectAssetBundle(BuildAssetBundleOptions.None);
    }
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "Build/SelectedBuild/LZ4", false, 0)]
    static void BuildSelectAssetBundleWithLZ4()
    {
        BuildSelectAssetBundle(BuildAssetBundleOptions.ChunkBasedCompression);
    }
    //-------------------------------------------------------------------------------------
    static void BuildSelectAssetBundle(BuildAssetBundleOptions buildOptions)
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }

        List<UnityEngine.Object> selectObjsList = new List<UnityEngine.Object>();
        for (int i = 0, iCount = selectObjs.Length; i < iCount; ++i)
        {
            //Add selected asset
            if (selectObjs[i] == null || selectObjsList.Contains(selectObjs[i]))
                continue;
            else if (CheckAssetIsCorrectToBuild(selectObjs[i]) == false)
                continue;
            selectObjsList.Add(selectObjs[i]);
        }

        AssetBundleBuild[] buildmap = CreateBuildMap(selectObjsList.ToArray());

        DoBuildSpecifiedAssetBundle(buildmap, buildOptions);
    }
    #endregion
    */
    #region Set AssetBundle Name
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetSingleName/Atlas", false, 0)]
    static void SetAtlasAssetBundlesName()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }
        SetAssetBundlesName(GetCorrectAssets(Enum_ResourcesType.Atlas, selectObjs));
    }
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetSingleName/GUI", false, 1)]
    static void SetGUIAssetBundlesName()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }
        SetAssetBundlesName(GetCorrectAssets(Enum_ResourcesType.GUI, selectObjs));
    }
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetSingleName/Songs", false, 2)]
    static void SetSongsAssetBundlesName()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }
        SetAssetBundlesName(GetCorrectAssets(Enum_ResourcesType.Songs, selectObjs));
    }
  
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetSingleName/Sound", false, 3)]
    static void SetSoundAssetBundlesName()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }
        SetAssetBundlesName(GetCorrectAssets(Enum_ResourcesType.Sound, selectObjs));
    }
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetSingleName/Texture", false, 4)]
    static void SetTextureAssetBundlesName()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }
        SetAssetBundlesName(GetCorrectAssets(Enum_ResourcesType.Texture, selectObjs));
    }
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetSingleName/Material", false, 5)]
    static void SetMaterialAssetBundlesName()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }
        SetAssetBundlesName(GetCorrectAssets(Enum_ResourcesType.Material, selectObjs));
    }
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetSingleName/Scene", false, 6)]
    static void SetSceneAssetBundlesName()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }
        SetAssetBundlesName(GetCorrectAssets(Enum_ResourcesType.Scene, selectObjs));
    }
    //-------------------------------------------------------------------------------------
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetSingleName/Notes", false, 7)]
    static void SetNotesAssetBundlesName()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectObjs.Length <= 0)
        {
            Debug.Log("Selection is empty!");
            return;
        }
        SetAssetBundlesName(GetCorrectAssets(Enum_ResourcesType.Notes, selectObjs));
    }
    //-------------------------------------------------------------------------------------
    //Set AssetBundles Name of all
    [MenuItem(ASSETBUNDLE_MENUITEM + "SetName/SetAllFilesName", false, 0)]
    static void SetAllAssetBundlesName()
    {
        RemoveAllAssetBundleName();
        SetAssetBundlesName(FindAllCanBuiltObjs());
    }
    //-------------------------------------------------------------------------------------
    static void SetAssetBundlesName(UnityEngine.Object[] selectObjs)
    {
        string resourcesPath = "Assets/" + Softstar.Utility.RESOURCE_PATH + "/";
        string dpsFolderName = "Dependencies/";
        string manifest = ".manifest";

        for (int i = 0, iCount = selectObjs.Length; i < iCount; ++i)
        {
            if (selectObjs[i] == null)
                continue;
            string assetPath = AssetDatabase.GetAssetPath(selectObjs[i]);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);

            //Make AssetBundle Name--------------------------------------------------
            //Use full file name to give a name to assetbundle name. 
            string abName = assetPath.Replace(resourcesPath, "");
            string realFilePath = Application.dataPath + "/../" + assetPath;
            string fileExtension = Path.GetExtension(realFilePath);
            abName = abName.Replace(fileExtension, "");
            string realFileName = Path.GetFileNameWithoutExtension(realFilePath);

            //Delete the original AssetBundle and its manifest before creating a new one
            if (string.IsNullOrEmpty(importer.assetBundleName) == false && importer.assetBundleName.Equals(abName))
            {
                string abPath = Application.dataPath + "/../" + Softstar.Utility.GetAssetBundleFolderName() + Path.DirectorySeparatorChar + importer.assetBundleName;
                if (File.Exists(abPath))
                    File.Delete(abPath);
                abPath += manifest;
                if (File.Exists(abPath))
                {
                    File.Delete(abPath);
                    Debug.Log("Delete Assetbundle File=[" + realFileName + "] AssetBunleName=[" + importer.assetBundleName + "]");
                }
            }
            importer.assetBundleName = abName.Trim();              //Normal AB Name
            Debug.Log("Asset=[" + realFileName + "] AssetBunleName=[" + importer.assetBundleName + "]");

            //--------------TODO Set VariantsName--------------

            //Set Dependencies AssetBundle Name
            string[] dpsPath = AssetDatabase.GetDependencies(new string[] { assetPath });
            for (int m = 0, mCount = dpsPath.Length; m < mCount; ++m)
            {
                importer = AssetImporter.GetAtPath(dpsPath[m]);

                if (CheckDependenciesIsCorrectToName(dpsPath[m]) == false)
                    continue;

                //Make AssetBundle Name
                string filePath = Application.dataPath + "/../" + dpsPath[m];
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                importer.assetBundleName = dpsFolderName + AssetDatabase.AssetPathToGUID(dpsPath[m]) + "_" + fileName; //GUID AB Name
                importer.assetBundleName = importer.assetBundleName.Trim();
                //Debug.Log( "Asset=["+selectObjs[i].name+"] Dependencies Asset=["+realFileName+"] Dependencies AssetBunleName=["+importer.assetBundleName+"]" );

                //--------------TODO Set VariantsName--------------

            }
            Debug.Log("Asset=[" + selectObjs[i].name + "] Dependencies Count=[" + dpsPath.Length + "]");
        }

        //Delete AB name which is unused.
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }
    //-------------------------------------------------------------------------------------
    //[MenuItem(ASSETBUNDLE_MENUITEM + "SetName/RemoveAllAssetBundleName", false)]
    private static void RemoveAllAssetBundleName()
    {
        string[] allABNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0, count = allABNames.Length; i < count; ++i)
        {
            AssetDatabase.RemoveAssetBundleName(allABNames[i], true);
        }
    }
    #endregion
    private static Dictionary<string, string> guidList = new Dictionary<string, string>();
    //-------------------------------------------------------------------------------------
    private static void CheckSameGUID(string path)
    {
        string guid = AssetDatabase.AssetPathToGUID(path);
        if (guidList.ContainsKey(guid))
        {
            Debug.Log("Asset=[" + path + "] has same guid with [" + guidList[guid] + "]");
            return;
        }

        guidList.Add(guid, path);
    }
}


