using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class ScriptCreator : Editor
{
    private static string TEMPLATE_SCRIPT_PATH = Application.dataPath + "/Editor/Softstar/";
    private const string TEMPLATE_SCRIPT_STATE = "NewState.cs";
    private const string TEMPLATE_SCRIPT_GUI = "UI_New.cs";

    [MenuItem("Assets/Create/Softstar/C# State Script")]
    private static void CreateStateScript()
    {
        CreateScript(TEMPLATE_SCRIPT_PATH + TEMPLATE_SCRIPT_STATE);
    }
    //---------------------------------------------------------------------------------------------------
    [MenuItem("Assets/Create/Softstar/C# GUI Script")]
    private static void CreateGUIScript()
    {
        CreateScript(TEMPLATE_SCRIPT_PATH+ TEMPLATE_SCRIPT_GUI);
    }
    //---------------------------------------------------------------------------------------------------
    private static void CreateScript(string targetPath)
    {
        Object[] objs = Selection.objects;
        if (objs.Length == 0)
        {
            UnityDebugger.Debugger.Log("Please choose the destination path!!");
            return;
        }
        for (int i = 0, iCount = objs.Length; i < iCount; ++i)
        {
            string creatPath = AssetDatabase.GetAssetPath(objs[i]);
            string directoryPath = (string.IsNullOrEmpty(Path.GetExtension(creatPath))) ? creatPath : Path.GetDirectoryName(creatPath);
            string creatFullPath = Softstar.Utility.GetFullPathByAssetPath(directoryPath) + "/" + Path.GetFileName(targetPath);

            if (Softstar.Utility.CopyFile(targetPath, creatFullPath))
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                return;
            }
        }
    }
}
