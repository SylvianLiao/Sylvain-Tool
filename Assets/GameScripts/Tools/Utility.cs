using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;


namespace Softstar
{
    public class Utility
    {
        public const string RESOURCE_PATH = "Resources";

        private static ResourceManager  m_resourceManager;
        private static GameDataDB       m_gameDataDB;
        //-----------------------------------------------------------------------------
        public static void SetResourceManager(MainApplication mainApp)
        {
            m_resourceManager = mainApp.GetResourceManager();
            m_gameDataDB = mainApp.GetGameDataDB();
        }
        //-----------------------------------------------------------------------------
        private static Dictionary<ENUM_LanguageType, string> staticLocalizationMapping = null;
        public static string GetLocalizationFolder(ENUM_LanguageType type)
        {
            if (staticLocalizationMapping == null)
            {
                staticLocalizationMapping = new Dictionary<ENUM_LanguageType, string>();
                staticLocalizationMapping[ENUM_LanguageType.None] = "";
                staticLocalizationMapping[ENUM_LanguageType.TraditionalChinese] = "localization/zh-tw/";
                staticLocalizationMapping[ENUM_LanguageType.SimplifiedChinese] = "localization/zh-cn/";
                staticLocalizationMapping[ENUM_LanguageType.English] = "localization/eng-us/";
            }
            string typeFolderName = null;
            if (staticLocalizationMapping.ContainsKey(type))
            {
                typeFolderName = staticLocalizationMapping[type];
            }
            return typeFolderName;
        }
        //-----------------------------------------------------------------------------
        public static string GetAssetBundleFolderName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
        }
#if UNITY_EDITOR
        //-----------------------------------------------------------------------------
        private static Dictionary<BuildTarget, string> staticBuildTargetMapping = null;
        private static string GetPlatformForAssetBundles(BuildTarget buildTarget)
        {
            if (staticBuildTargetMapping == null)
            {
                staticBuildTargetMapping = new Dictionary<BuildTarget, string>();
                staticBuildTargetMapping[BuildTarget.Android]               = "AssetBundles_Android";
                staticBuildTargetMapping[BuildTarget.iOS]                   = "AssetBundles_iOS";
                staticBuildTargetMapping[BuildTarget.WebGL]                 = "AssetBundles_WebGL";
                staticBuildTargetMapping[BuildTarget.WebPlayer]             = "AssetBundles_WebPlayer";
                staticBuildTargetMapping[BuildTarget.StandaloneWindows]     = "AssetBundles_Windows";
                staticBuildTargetMapping[BuildTarget.StandaloneOSXIntel]    = "AssetBundles_OSX";
                staticBuildTargetMapping[BuildTarget.StandaloneOSXIntel64]  = "AssetBundles_OSX";
                staticBuildTargetMapping[BuildTarget.StandaloneOSXUniversal] = "AssetBundles_OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            }
            string strRet = null;
            if (staticBuildTargetMapping.ContainsKey(buildTarget))
            {
                strRet = staticBuildTargetMapping[buildTarget];
            }
            return strRet;
        }
#endif
        //-----------------------------------------------------------------------------
        private static Dictionary<RuntimePlatform, string> staticRuntimePlatformMapping = null;
        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            if(staticRuntimePlatformMapping==null)
            {
                staticRuntimePlatformMapping = new Dictionary<RuntimePlatform, string>();
                staticRuntimePlatformMapping[RuntimePlatform.Android]           = "AssetBundles_Android";
                staticRuntimePlatformMapping[RuntimePlatform.IPhonePlayer]      = "AssetBundles_iOS";
                staticRuntimePlatformMapping[RuntimePlatform.WebGLPlayer]       = "AssetBundles_WebGL";
                staticRuntimePlatformMapping[RuntimePlatform.OSXWebPlayer]      = "AssetBundles_WebPlayer";
                staticRuntimePlatformMapping[RuntimePlatform.WindowsWebPlayer]  = "AssetBundles_WebPlayer";
                staticRuntimePlatformMapping[RuntimePlatform.WindowsPlayer]     = "AssetBundles_Windows";
                staticRuntimePlatformMapping[RuntimePlatform.OSXPlayer]         = "AssetBundles_OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            }
            string strRet = null;
            if(staticRuntimePlatformMapping.ContainsKey(platform))
            {
                strRet = staticRuntimePlatformMapping[platform];
            }
            return strRet;
        }
        //-----------------------------------------------------------------------------
        private static Dictionary<Enum_ResourcesType, string> staticResourceTypeMapping = null;
        public static string GetResourceTypeFolder(Enum_ResourcesType type)
        {
            if(staticResourceTypeMapping==null)
            {
                staticResourceTypeMapping = new Dictionary<Enum_ResourcesType, string>();
                staticResourceTypeMapping[Enum_ResourcesType.Atlas]     = "Atlas";
                staticResourceTypeMapping[Enum_ResourcesType.Material]  = "Material";
                staticResourceTypeMapping[Enum_ResourcesType.Particle]  = "Particle";
                staticResourceTypeMapping[Enum_ResourcesType.Scene]     = "Scene";
                staticResourceTypeMapping[Enum_ResourcesType.Songs]     = "Songs";
                staticResourceTypeMapping[Enum_ResourcesType.Sound]     = "Sound";
                staticResourceTypeMapping[Enum_ResourcesType.Notes]     = "Notes";
                staticResourceTypeMapping[Enum_ResourcesType.Texture]   = "Texture";
                staticResourceTypeMapping[Enum_ResourcesType.GUI]       = "GUI";
            }
            string typeFolderName = null;
            if(staticResourceTypeMapping.ContainsKey(type))
            {
                typeFolderName = staticResourceTypeMapping[type];
            }
            return typeFolderName;
        }
        //------------------------------------------------------------------------------------------------------------------------------
        public static string GetAssetBundleFolderPath(Enum_LoadAssetbundlePath pathType)
        {
            string localPath = Application.dataPath + "/../" + Softstar.Utility.GetAssetBundleFolderName();
            string netPath = Application.persistentDataPath + Path.DirectorySeparatorChar + Softstar.Utility.GetAssetBundleFolderName();
            //string netPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + Softstar.Utility.GetAssetBundleFolderName();
#if UNITY_EDITOR
            switch (pathType)
            {
                case Enum_LoadAssetbundlePath.Local:
                    return localPath;
                case Enum_LoadAssetbundlePath.Net:
                    return netPath;
            }
#elif UNITY_ANDROID
            return netPath;
#elif UNITY_IOS
            return netPath;
#endif
            return netPath;
        }
        #region File Controll
        //---------------------------------------------------------------------------------------------------
        public static string GetFullPathByAssetPath(string assetPath)
        {
            string fullPath = Application.dataPath + "/../" + assetPath;
            return fullPath;
        }
        //---------------------------------------------------------------------------------------------------
        public static bool CopyFile(string source, string target)
        {
            if (!File.Exists(source))
                return false;

            File.Copy(source, target, false);

            return File.Exists(target);
        }
        //---------------------------------------------------------------------------------------------------
        private static void CheckAndCreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        //---------------------------------------------------------------------------------------------------
        public static void CopyDirectory(string source, string target)
        {
            CheckAndCreateDirectory(target);

            foreach (var file in Directory.GetFiles(source))
            {
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
            }

            foreach (var directory in Directory.GetDirectories(source))
            {
                CopyDirectory(directory, Path.Combine(target, Path.GetFileName(directory)));
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static void DeleteDirectory(string dir)
        {
            string[] strFileAry = Directory.GetFiles(dir);
            foreach (string file in strFileAry)
            {
                File.Delete(file);
            }

            string[] strDirAry = Directory.GetDirectories(dir);
            foreach (string strDir in strDirAry)
            {
                Directory.Delete(strDir);
            }
        }
        #endregion
        #region UI Utility
        //-----------------------------------------------------------------------------------------------------
        //提供更換UI Texture的流程
        public static bool ChangeTexture(UITexture uiTexture, int textureTmpID)
        {
            if (uiTexture == null)
                return false;

            S_TextureTable_Tmp textureTmp = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(textureTmpID);
            if (textureTmp == null)
            {
                UnityDebugger.Debugger.LogError("No S_TextureTable_Tmp Data with [" + textureTmpID + "]!!");
                return false;
            }
            return ChangeTexture(uiTexture, textureTmp.strTextureName, uiTexture.shader);
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeTexture(UITexture uiTexture, string texturePath)
        {
            if (uiTexture == null)
                return false;

            return ChangeTexture(uiTexture, texturePath, uiTexture.shader);
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeTexture(UITexture uiTexture, string textureName, Shader defaultShader)
        {
            if (uiTexture == null)
                return false;
            if (string.IsNullOrEmpty(textureName))
                return false;

            Texture texture = m_resourceManager.GetResourceSync<Texture>(Enum_ResourcesType.Texture, textureName);
            uiTexture.mainTexture = texture;
            uiTexture.shader = defaultShader;
            return true;
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeAtlasSprite(UISprite uiSprite, int textureTmpID)
        {
            if (uiSprite == null)
                return false;

            S_TextureTable_Tmp textureTmp = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(textureTmpID);
            if (textureTmp == null)
            {
                UnityDebugger.Debugger.LogError("No S_TextureTable_Tmp Data with ["+ textureTmpID + "]!!");
                return false;
            }
            return ChangeAtlasSprite(uiSprite, textureTmp.strAtlasName, textureTmp.strSpriteName);
        }
        //-----------------------------------------------------------------------------------------------------
        //提供更換AtlasSprite的流程
        //提供設定空圖功能
        public static bool ChangeAtlasSprite(UISprite uiSprite, string spriteName)
        {
            //設定空圖的條件
            if (string.IsNullOrEmpty(spriteName))
            {
                uiSprite.atlas = null;
                uiSprite.spriteName = null;
                uiSprite.SetDirty();
                return true;
            }

            //設定
            return ChangeAtlasSprite(uiSprite, uiSprite.atlas.name, spriteName);
        }
        //-----------------------------------------------------------------------------------------------------
        //提供更換AtlasSprite的流程
        //提供設定空圖功能
        public static bool ChangeAtlasSprite(UISprite uiSprite, string atlasName, string spriteName)
        {
            if (uiSprite == null)
            {
                UnityDebugger.Debugger.LogError("無UISprite!!");
                return false;
            }

            //先清空
            uiSprite.atlas = null;
            uiSprite.spriteName = null;

            //設定空圖的條件
            if ((string.IsNullOrEmpty(atlasName) || atlasName == "-1") && string.IsNullOrEmpty(spriteName))
                return true;

            GameObject go = m_resourceManager.GetResourceSync(Enum_ResourcesType.Atlas, atlasName);
            UIAtlas changeAtlas = go.GetComponent<UIAtlas>();
            if (changeAtlas == null)
            {
                UnityDebugger.Debugger.LogError(atlasName + " 讀取更換用Atlas失敗!!");
                return false;
            }

            BetterList<string> spriteNames = changeAtlas.GetListOfSprites();
            if (spriteName == null || spriteName.Length <= 0)
                spriteName = spriteNames[0];

            //設定
            uiSprite.atlas = changeAtlas;
            uiSprite.spriteName = spriteName;

            return true;
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeButtonSprite(UIButton uiButton, int textureTmpID)
        {
            S_TextureTable_Tmp textureTmp = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(textureTmpID);
            if (textureTmp == null)
            {
                UnityDebugger.Debugger.LogError("No S_TextureTable_Tmp Data with [" + textureTmpID + "]!!");
                return false;
            }

            return ChangeButtonSprite(uiButton, textureTmp.strSpriteName, textureTmp.strSpriteName, textureTmp.strSpriteName, textureTmp.strSpriteName, textureTmp.strAtlasName);
        }
        //-----------------------------------------------------------------------------------------------------
        //提供更換Button Sprite的流程
        public static bool ChangeButtonSprite(UIButton uiButton, string normalSprite, string hoverSprite, string pressdSprite, string disabledSprite, string atlasName)
        {
            if (uiButton == null)
            {
                UnityDebugger.Debugger.LogError("無UIButton!!");
                return false;
            }

            UISprite uiSprite = uiButton.tweenTarget.GetComponent<UISprite>();
            if (string.IsNullOrEmpty(atlasName))
            {
                uiSprite.atlas = null;
                return false;
            }

            GameObject go = m_resourceManager.GetResourceSync(Enum_ResourcesType.Atlas, atlasName);
            if (go == null)
                return false;

            UIAtlas changeAtlas = go.GetComponent<UIAtlas>();
            if (changeAtlas == null)
            {
                UnityDebugger.Debugger.LogError(atlasName + " 讀取更換用Atlas失敗!!");
                return false;
            }

            if (uiButton.tweenTarget == null)
                return false;

            //設定
            uiSprite.atlas = changeAtlas;
            uiSprite.SetDirty();

            return ChangeButtonSprite(uiButton, normalSprite, hoverSprite, pressdSprite, disabledSprite);
        }
        public static bool ChangeButtonSprite(UIButton uiButton, string normalSprite, string hoverSprite, string pressdSprite, string disabledSprite)
        {
            if (uiButton == null)
            {
                UnityDebugger.Debugger.LogError("無UIButton!!");
                return false;
            }

            uiButton.normalSprite = normalSprite;
            uiButton.hoverSprite = hoverSprite;
            uiButton.pressedSprite = pressdSprite;
            uiButton.disabledSprite = disabledSprite;

            return true;
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeButtonPicColor(UIButton uiButton, Color colorNormal, Color colorHover, Color colorPressed, Color colorDisabled)
        {
            if (uiButton == null)
            {
                UnityDebugger.Debugger.LogError("無UIButton!!");
                return false;
            }
            if (uiButton.tweenTarget == null)
            {
                UnityDebugger.Debugger.LogError("無tweenTarget!!");
                return false;
            }

            //設定按鈕四態圖示
            //Normal
            uiButton.defaultColor = colorNormal;
            //Hover
            uiButton.hover = colorHover;
            //Pressed
            uiButton.pressed = colorPressed;
            //Disabled
            uiButton.disabledColor = colorDisabled;
            return true;
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeSongDifficultyTexture(UITexture uiTexture, Enum_SongDifficulty difficulty)
        {
            int textureID = 0;
            switch (difficulty)
            {
                case Enum_SongDifficulty.Easy:  textureID = 8; break;
                case Enum_SongDifficulty.Normal:textureID = 9; break;
                case Enum_SongDifficulty.Hard:  textureID = 10;break;
                default:
                    break;
            }

            return ChangeTexture(uiTexture,textureID);
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeSongRankSprite(UISprite uiSprite, Enum_SongRank rank)
        {
            int textureID = 0;
            switch (rank)
            {
                case Enum_SongRank.A:  textureID = 35; break;
                case Enum_SongRank.B:  textureID = 36; break;
                case Enum_SongRank.C:  textureID = 37; break;
                case Enum_SongRank.D:  textureID = 38; break;
                case Enum_SongRank.S:  textureID = 39; break;
                case Enum_SongRank.G:  textureID = 40; break;
                case Enum_SongRank.New:textureID = 7;  break;
                default:
                    break;
            }

            return ChangeAtlasSprite(uiSprite, textureID);
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeStarSprite(UISprite uiSprite, Enum_StarStatus star)
        {
            int textureID = 0;
            switch (star)
            {
                case Enum_StarStatus.None: textureID = 15; break;
                case Enum_StarStatus.Half: textureID = 16; break;
                case Enum_StarStatus.Full: textureID = 17; break;
                default:
                    break;
            }

            return ChangeAtlasSprite(uiSprite, textureID);
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeScoreNumberSprite(UISprite uiSprite, int number)
        {
            if (number > 9 || number < 0)
                return false;

            int textureId = 59 + number;

            return ChangeAtlasSprite(uiSprite, textureId);
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeMaterial(MeshRenderer mesh, int textureTmpID)
        {
            S_TextureTable_Tmp textureTmp = m_gameDataDB.GetGameDB<S_TextureTable_Tmp>().GetData(textureTmpID);
            if (textureTmp == null)
            {
                UnityDebugger.Debugger.LogError("No S_TextureTable_Tmp Data with [" + textureTmpID + "]!!");
                return false;
            }

            return ChangeMaterial(mesh, textureTmp.strMaterialName);
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool ChangeMaterial(MeshRenderer mesh, string materialPath)
        {
            if (mesh == null)
                return false;
            if (string.IsNullOrEmpty(materialPath))
                return false;

            Material mat = m_resourceManager.GetResourceSync<Material>(Enum_ResourcesType.Material, materialPath);
            if (mat == null)
            {
                UnityDebugger.Debugger.Log("Change Material Failed!!");
                return false;
            }

            mesh.material = mat;

            return true;
        }
        #endregion

        /// <summary>將秒數換算成顯示24小時格式的時間字串後回傳,小數點會無條件捨去</summary>
        public static string GetShowTime(Enum_TimeFormat emType, float time)
        {
            int hours = 0;
            int minute = 0;
            int seconds = 0;
            string strTime = string.Empty;
            switch (emType)
            {
                case Enum_TimeFormat.Second:
                    strTime = ((int)time).ToString();
                    break;
                case Enum_TimeFormat.Minute:
                    minute = (int)time / 60;
                    seconds = (int)time % 60;
                    strTime = minute.ToString() + ":" + string.Format("{0:00}", seconds);
                    break;
                case Enum_TimeFormat.Hour:
                    hours = (int)time / 3600;
                    minute = ((int)time % 3600) / 60;
                    seconds = ((int)time % 3600) % 60;
                    strTime = string.Format("{0:00}:{1:00}:{2:00}", hours, minute, seconds);
                    break;
            }
            return strTime;
        }

        /// <summary>將數字拆成位數並存成List回傳</summary>
        /// <param name="saveAtSmall">從最小位數還是最大位數開始存</param>
        public static List<int> SeparateNumberToDigit(long number, bool saveAtSmall)
        {
            List<int> digitNumberList = new List<int>();
            long quotient = number * 10;
            while (quotient >= 10)
            {
                quotient /= 10;
                int digit = (int)(quotient % 10);
                digitNumberList.Add(digit);  //EX: 個,十,百
            }    

            if (!saveAtSmall)
                digitNumberList.Reverse();  //EX: 百,十,個

            return digitNumberList;
        }
        public static void ReApplyShaders(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (var rend in renderers)
            {
                Material[] materials = rend.sharedMaterials;
                string[] shaders = new string[materials.Length];

                for (int i = 0; i < materials.Length; i++)
                {
                    ReApplyMaterialShader(materials[i]);
                }
            }
        }
        public static void ReApplyMaterialShader(Material mat)
        {
            if (mat == null)
                return;
            string shaderName = mat.shader.name;
            mat.shader = Shader.Find(shaderName);
        }
    }
}

