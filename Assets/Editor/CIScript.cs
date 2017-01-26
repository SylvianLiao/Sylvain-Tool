using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.IO;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CIScript : MonoBehaviour {

	// currently build android only
	/*
	[MenuItem("Funfia/Daily Build (For Test)")]
	public static void DailyBuild() {
		// build assets first
		ProjectBuilder.BuildForCurrentPlatform (BuildTarget.Android);

		string append = DateTime.Now.ToString("yyyyMMdd");
		string[] scenes = { "Assets/Scene/Title.unity" };
		BuildPipeline.BuildPlayer (scenes, "SpaceToneDailyBuild" + append + ".apk", BuildTarget.Android, BuildOptions.None);
	}
	*/

	[MenuItem("Softstar CI/Android Build")]
	public static void Android_Build()
	{
        PlayerSettings.Android.keystoreName = "./ssmg.keystore";
        PlayerSettings.Android.keystorePass = "softstar";
        PlayerSettings.Android.keyaliasName = "ssmg";
        PlayerSettings.Android.keyaliasPass = "softstar";

		ProjectBuilder.BuildForCurrentPlatform (BuildTarget.Android);
		string[] scenes = { "Assets/Resources/Scene/NewTitle.unity" };
		BuildOptions opt = BuildOptions.SymlinkLibraries | BuildOptions.Development | BuildOptions.ConnectWithProfiler;
		BuildPipeline.BuildPlayer(scenes, "ssmg.apk", BuildTarget.Android, opt);
	}

	[MenuItem("Softstar CI/iOS Build")]
	public static void iOS_Build()
	{
		BuildTarget buildTarget = BuildTarget.iOS;
		// Build iOS AssetBundle
		ProjectBuilder.BuildForCurrentPlatform(buildTarget);

		// Build for xcode project
		string outputPath = "XCode";
		string[] scenes = { "Assets/Resources/Scene/NewTitle.unity" };
		BuildOptions opt = BuildOptions.None;//BuildOptions.SymlinkLibraries | BuildOptions.Development | BuildOptions.ConnectWithProfiler;
		BuildPipeline.BuildPlayer(scenes, outputPath, buildTarget, opt);
	}

	/// <summary>
	/// Reference from https://bitbucket.org/Unity-Technologies/iosnativecodesamples/src/a0bc90e7d6358e456caf25d717134864218740a7/NativeIntegration/Misc/UpdateXcodeProject/Assets/Editor/XcodeProjectUpdater.cs?at=stable&fileviewer=file-view-default
	/// </summary>
	/// <param name="srcPath">Source path.</param>
	/// <param name="dstPath">Dst path.</param>
	internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
	{
		if (Directory.Exists(dstPath))
			Directory.Delete(dstPath);
		if (File.Exists(dstPath))
			File.Delete(dstPath);

		Directory.CreateDirectory(dstPath);

		foreach (var file in Directory.GetFiles(srcPath))
			File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

		foreach (var dir in Directory.GetDirectories(srcPath))
			CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
	}

	[PostProcessBuild(100)]
	public static void OnPostProcessBuild(BuildTarget buildTarget, string strPath)
	{
		if(buildTarget == BuildTarget.iOS)
		{
			//Debug.Log ("======================POST PROCESS iOS");
			// Copy Google-iOS library
			CopyAndReplaceDirectory("Google-iOS/GoogleOpenSource.framework", Path.Combine(strPath, "Google-iOS/GoogleOpenSource.framework"));
			CopyAndReplaceDirectory("Google-iOS/GoogleSignIn.bundle", Path.Combine(strPath, "Google-iOS/GoogleSignIn.bundle"));
			CopyAndReplaceDirectory("Google-iOS/GoogleSignIn.framework", Path.Combine(strPath, "Google-iOS/GoogleSignIn.framework"));
			CopyAndReplaceDirectory("Google-iOS/gpg.bundle", Path.Combine(strPath, "Google-iOS/gpg.bundle"));
			CopyAndReplaceDirectory("Google-iOS/gpg.framework", Path.Combine(strPath, "Google-iOS/gpg.framework"));

			// Modify XCode project setting
			ModifyPBXProject(GetProjectPath(strPath), PBXProject.GetUnityTargetName());
		}
	}

	private static string GetProjectPath(string strPath)
	{
		string strProjPath = PBXProject.GetPBXProjectPath(strPath);
		Debug.Log("PBXProjectPath: " + strProjPath);
		string strXCodeProj = ".xcodeproj";
		if(strProjPath.IndexOf(strXCodeProj)<0) // find xcodeproj
		{
			// Not found
			int index = strProjPath.LastIndexOf(Path.DirectorySeparatorChar);
			//Debug.Log ("PathSeparator: "+Path.DirectorySeparatorChar+" lastIndex: "+index);
			if(index>=0)
			{
				strProjPath = strProjPath.Insert(index, strXCodeProj);
			}
		}
		//Debug.Log ("GetProjectPath: " + strProjPath);
		return strProjPath;
	}
	private static void ModifyPBXProject(string strProjPath, string strTargetName)
	{
		//Debug.Log ("======================MODIFY PBXPROJ");
		PBXProject proj = new PBXProject();
		proj.ReadFromFile(strProjPath);
		string strTargetGUID = proj.TargetGuidByName (strTargetName);
		//Debug.Log ("Target: "+strTargetName+" GUID: "+strTargetGUID);

		//string strBuildConfig = proj.BuildConfigByName(strTargetGUID, "Release");
		//Debug.Log ("BuildConfig: "+strBuildConfig);

		//Debug.Log ("Add Google to build");
		// Add Google to build
		proj.AddFileToBuild(strTargetGUID, proj.AddFile("Google-iOS/GoogleOpenSource.framework", "Google-iOS/GoogleOpenSource.framework", PBXSourceTree.Source));
		proj.AddFileToBuild(strTargetGUID, proj.AddFile("Google-iOS/GoogleSignIn.bundle", "Google-iOS/GoogleSignIn.bundle", PBXSourceTree.Source));
		proj.AddFileToBuild(strTargetGUID, proj.AddFile("Google-iOS/GoogleSignIn.framework", "Google-iOS/GoogleSignIn.framework", PBXSourceTree.Source));
		proj.AddFileToBuild(strTargetGUID, proj.AddFile("Google-iOS/gpg.bundle", "Google-iOS/gpg.bundle", PBXSourceTree.Source));
		proj.AddFileToBuild(strTargetGUID, proj.AddFile("Google-iOS/gpg.framework", "Google-iOS/gpg.framework", PBXSourceTree.Source));
		//proj.AddFileToBuild(strTargetGUID, proj.AddFile("../Assets/Facebook/Editor/iOS/FbUnityInterface.mm", "Facebook/FbUnityInterface.mm", PBXSourceTree.Source));

		//Debug.Log ("Add iOS Library to build");
		proj.AddFrameworkToProject(strTargetGUID, "AddressBook.framework", false);
		proj.AddFrameworkToProject(strTargetGUID, "AssetsLibrary.framework", false);
		proj.AddFrameworkToProject(strTargetGUID, "CoreData.framework", false);
		proj.AddFrameworkToProject(strTargetGUID, "CoreTelephony.framework", false);
		proj.AddFrameworkToProject(strTargetGUID, "CoreText.framework", false);
		proj.AddFrameworkToProject(strTargetGUID, "Security.framework", false);
		proj.AddFrameworkToProject(strTargetGUID, "SafariServices.framework", false);
		//proj.AddFileToBuild(strTargetGUID, proj.AddFile("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));
		proj.AddFileToBuild(strTargetGUID, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
		//proj.AddFrameworkToProject(strTargetGUID, "libc++.tbd", false);
		//proj.AddFrameworkToProject(strTargetGUID, "libz.tbd", false);

		proj.SetBuildProperty(strTargetGUID, "ENABLE_BITCODE", "NO");
		proj.AddBuildProperty(strTargetGUID, "HEADER_SEARCH_PATHS", "$(inherited)");
		proj.AddBuildProperty(strTargetGUID, "OTHER_CFLAGS", "$(inherited)");
		proj.SetBuildProperty(strTargetGUID, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
		proj.AddBuildProperty(strTargetGUID, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Google-iOS");
		proj.AddBuildProperty(strTargetGUID, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/FacebookSDK/Plugins/iOS");
		
		//proj.AddFrameworkToProject(strTargetGUID, "GoogleSignIn.framework", false);
		//proj.AddFrameworkToProject(strTargetGUID, "GoogleOpenSource.framework", false);
		//proj.AddFrameworkToProject(strTargetGUID, "gpg.framework", false);
		
		proj.AddBuildProperty(strTargetGUID, "OTHER_LDFLAGS", "$(inherited)");
		proj.AddBuildProperty(strTargetGUID, "OTHER_LDFLAGS", "-ObjC");

		/*
		string strFile = proj.FindFileGuidByProjectPath("Facebook/FbUnityInterface.mm");
		bool bFound = false;
		if (strFile != null) {
			bFound = true;
		} else {
			Debug.LogError("========FbUnityInterface.mm can't be found! Try another.========");
			strFile = proj.FindFileGuidByRealPath("../Assets/Facebook/Editor/iOS/FbUnityInterface.mm");
			if (strFile != null) {
				bFound = true;
			} else {
				Debug.LogError("========FbUnityInterface.mm can't be found!========");
			}
		}
		if (bFound) {
			var flags = proj.GetCompileFlagsForFile (strTargetGUID, strFile);
			flags.Add ("-fno-objc-arc");
			proj.SetCompileFlagsForFile (strTargetGUID, strFile, flags);
		}
		*/

		Debug.Log ("Write project file: " + strProjPath);
		proj.WriteToFile(strProjPath);
	}

	//[MenuItem("Funfia/Test PBXProject class")]
	public static void TestPBXProject()
	{
		string strProjPath = PBXProject.GetPBXProjectPath("XCode");
		string strTargetName = PBXProject.GetUnityTargetName();

		Debug.Log ("PBXProjectPath: " + strProjPath);
		Debug.Log ("UnityTargetName: " + strTargetName);
		Debug.Log ("UnityTestTargetName: " + PBXProject.GetUnityTestTargetName());
		//Debug.Log ("IsBuildable: " + PBXProject.IsBuildable("extname"));
		//Debug.Log ("IsKnownExtension: " + PBXProject.IsKnownExtension("extname"));

		//ModifyPBXProject(strProjPath, strTargetName);
		ModifyPBXProject(GetProjectPath("XCode"), strTargetName);
	}
}
