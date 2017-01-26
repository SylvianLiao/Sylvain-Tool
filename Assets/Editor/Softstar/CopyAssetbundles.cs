using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class CopyAssetbundles : MonoBehaviour
{
    [MenuItem(Softstar.Utility.RESOURCE_PATH + "/CopyAssetBundles/ToPersistentDataPath")]
    //------------------------------------------------------------------------------------------------------------------------------
    public static void CopyAssetBundleToPersistentDataPath()
    {
        string sourcePath = Softstar.Utility.GetAssetBundleFolderPath(Enum_LoadAssetbundlePath.Local);
        string targetPath = Softstar.Utility.GetAssetBundleFolderPath(Enum_LoadAssetbundlePath.Net);

        if (!Directory.Exists(sourcePath))
            return;

        if (Directory.Exists(targetPath))
            Softstar.Utility.DeleteDirectory(targetPath);

       Softstar.Utility.CopyDirectory(sourcePath, targetPath);
    }

    [MenuItem(Softstar.Utility.RESOURCE_PATH + "/CopyAssetBundles/ToStreamingAssets")]
    //------------------------------------------------------------------------------------------------------------------------------
    public static void CopyAssetBundleToStreamingAssets()
    {
        string sourcePath = Softstar.Utility.GetAssetBundleFolderPath(Enum_LoadAssetbundlePath.Local);
        string targetPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + Softstar.Utility.GetAssetBundleFolderName();

        if (!Directory.Exists(sourcePath))
            return;

        if (Directory.Exists(targetPath))
            Softstar.Utility.DeleteDirectory(targetPath);

        Softstar.Utility.CopyDirectory(sourcePath, targetPath);
    }
}
