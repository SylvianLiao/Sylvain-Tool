using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Callbacks;
using System.IO;

public static class ProjectBuilder
{
	static string assetBundlesPath = "AssetBundles/";
	private static string[] excludeDelDir = new string[] {"movie", "DBF"};

	private static void EnsureDirectory(string dir) {
		if (!Directory.Exists (dir)) {
			Directory.CreateDirectory (dir);
		}
	}

	private static void CopyDirectory(string source, string target) {
		EnsureDirectory (target);
		
		foreach (var file in Directory.GetFiles(source)) {
			File.Copy (file, Path.Combine (target, Path.GetFileName (file)));
		}

		foreach (var directory in Directory.GetDirectories(source)) {
			CopyDirectory (directory, Path.Combine (target, Path.GetFileName (directory)));
		}
	}

	private static void DeleteDirectory(string dir) {
		string[] strFileAry = Directory.GetFiles (dir);
		foreach (string file in strFileAry) {
			File.Delete(file);
		}

		string[] strDirAry = Directory.GetDirectories (dir);
		foreach (string strDir in strDirAry) {
			bool bExclude = false;
			string[] strSplitAry = strDir.Split(new string[]{"/", "\\"}, System.StringSplitOptions.RemoveEmptyEntries);
			string strCheckDir = strSplitAry[strSplitAry.Length-1];
			for(int i=0;i<excludeDelDir.Length;i++)
			{
				if(strCheckDir == excludeDelDir[i])
				{
					bExclude = true;
					break;
				}
			}
			if(!bExclude)
			{
				DirectoryInfo dirInfo = new DirectoryInfo(strDir);
				dirInfo.Delete(true);
			}
		}
	}

	private static string[] GetScenePaths()
	{
		string[] scenes = new string[EditorBuildSettings.scenes.Length];
		
		for(int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}
		
		return scenes;
	}

	[MenuItem("Funfia/Build Asset Bundles")]
	public static void BuildForCurrentPlatform()
	{
		BuildForCurrentPlatform (EditorUserBuildSettings.activeBuildTarget);
	}
	
	public static void BuildForCurrentPlatform(BuildTarget target)
	{
		string assetBundleDir = GetAssetBundleDir (target);

		EnsureDirectory (assetBundleDir);
		
		AssetDatabase.RemoveUnusedAssetBundleNames ();
		
		BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
		
		BuildPipeline.BuildAssetBundles (assetBundleDir, options, target);

		DeleteDirectory (Application.streamingAssetsPath);
		CopyDirectory (assetBundleDir, Application.streamingAssetsPath);
	}

	[MenuItem("Funfia/Custom Build")]
	public static void CustomBuild() {
		string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

		BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

		DeleteDirectory (Application.streamingAssetsPath);
		string assetBundleDir = GetAssetBundleDir (target);
		CopyDirectory(assetBundleDir, Application.streamingAssetsPath);
		BuildPipeline.BuildPlayer (GetScenePaths (), path, target, BuildOptions.None);
	}

	static string GetAssetBundleDir(BuildTarget target){
		
		if (target.ToString ().Contains ("Standalone"))
		{
			return assetBundlesPath + "default/";
		}
		else if (target == BuildTarget.iOS)
		{
			return assetBundlesPath + "ios/";
			
		}
		else if (target == BuildTarget.WebGL)
		{
			return assetBundlesPath + "webgl/";
			
		}
		else if (target == BuildTarget.Android)
		{
			return assetBundlesPath + "android/";
		}
		throw new UnityException("Platform not implemented (asset bundle dir)");
	}
}
