using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LevelEditorToolEditor : Editor
{
    [MenuItem("Level Editor/Open Level Editor")]
    static void OpenLevelEditor()
    {
        if (EditorApplication.isSceneDirty)
        {
            EditorApplication.SaveCurrentSceneIfUserWantsTo();
        }

        EditorApplication.OpenScene("Assets/Scenes/LevelEditor.unity");
    }
}
