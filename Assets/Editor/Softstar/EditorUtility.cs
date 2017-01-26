using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Softstar
{
    public class EditorUtility : Editor
    {
        #region Unity Controll
        [MenuItem("Assets/Apply Prefabs(s)", false, 10000)]
        static void ApplyPrefabs()
        {
            var selections = Selection.gameObjects;
            if (!CheckApplyPrefabs(selections))
            {

                return;
            }

            string log = null;
            foreach (var go in selections)
            {
                var prefabType = PrefabUtility.GetPrefabType(go);
                if (prefabType == PrefabType.PrefabInstance ||
                    prefabType == PrefabType.DisconnectedPrefabInstance)
                {
                    var goRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot(go);
                    var prefabParent = PrefabUtility.GetPrefabParent(goRoot);
                    PrefabUtility.ReplacePrefab(goRoot, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(goRoot.scene);
                    log += "Prefab ["+ prefabParent.name +"] \n";
                }
            }
            UnityEditor.EditorUtility.DisplayDialog("Apply Prefabs Succeed!", log , "OK");
        }
        [MenuItem("Assets/Apply Prefabs(s)", true, 10000)]
        static bool ApplyPrefabs_Validate()
        {
            if (AnimationMode.InAnimationMode())
            {
                return false;
            }
            return Selection.gameObjects.Length > 1;
        }

        static bool CheckApplyPrefabs(GameObject[] selectGO)
        {
            List<Object> parentObjs = new List<Object>();
            foreach (GameObject go in selectGO)
            {
                var prefabType = PrefabUtility.GetPrefabType(go);
                if (prefabType == PrefabType.PrefabInstance ||
                    prefabType == PrefabType.DisconnectedPrefabInstance)
                {
                    var goRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot(go);
                    var prefabParent = PrefabUtility.GetPrefabParent(goRoot);
                    if (parentObjs.Contains(prefabParent))
                    {
                        UnityEditor.EditorUtility.DisplayDialog("Apply Prefabs Failed!", "Some prefabs have same parent prefab (" + prefabParent.name + ").", "OK");
                        return false;
                    }
                    else
                    {
                        parentObjs.Add(prefabParent);
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
