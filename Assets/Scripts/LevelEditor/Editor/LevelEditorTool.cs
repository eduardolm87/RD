using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(Stage))]
public class LevelEditorTool : Editor
{
    Stage stage;
    SerializedProperty Start;
    SerializedProperty Finish;
    SerializedProperty Music;
    SerializedProperty NextStage;


    static bool fold_goals = true;
    static bool fold_metadata = true;
    static bool fold_tiletool = false;



    string HelpboxText = "";

    public Stage StageInScene = null;


    void OnEnable()
    {
        Start = serializedObject.FindProperty("StartPoint");
        Finish = serializedObject.FindProperty("PlaceToReach");
        Music = serializedObject.FindProperty("Music");
        NextStage = serializedObject.FindProperty("NextStagesUnlocked");


        StageInScene = GameObject.FindObjectsOfType<Stage>().ToList().FirstOrDefault(stg => stg == target);
    }

    public override void OnInspectorGUI()
    {
        stage = (Stage)target;

        Header();

        fold_metadata = EditorGUILayout.Foldout(fold_metadata, "Meta");
        if (fold_metadata)
        {
            Meta();
        }

        fold_goals = EditorGUILayout.Foldout(fold_goals, "Goals");
        if (fold_goals)
        {
            Goals();
        }

        fold_tiletool = EditorGUILayout.Foldout(fold_tiletool, "Tile Tools");
        if (fold_tiletool)
        {
            if (GUILayout.Button("Config"))
            {
                ShowTileTool();
            }

            if (GUILayout.Button("Use"))
            {
                UseTileTool();
            }
        }

        if (GUILayout.Button("Autocheck"))
        {
            Autocomplete();
        }

        //if (GUILayout.Button((EditorApplication.isSceneDirty ? "*" : "") + "Save"))
        //{
        //    Save();
        //}

        Helpbox();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(stage);
        }

        serializedObject.ApplyModifiedProperties();

        //DrawDefaultInspector();
    }

    void Header()
    {
        EditorGUILayout.LabelField("ID: " + stage.name, EditorStyles.boldLabel);
    }

    void Meta()
    {
        stage.Name = EditorGUILayout.TextField("Display name", stage.Name);
        stage.Difficulty = EditorGUILayout.IntSlider("Difficulty", stage.Difficulty, 1, 5);
        EditorGUILayout.PropertyField(Music);
    }

    void Goals()
    {
        EditorGUILayout.PropertyField(Start);
        EditorGUILayout.PropertyField(Finish);
        EditorGUILayout.PropertyField(NextStage, true);
    }

    void Save()
    {
        EditorApplication.SaveScene();
    }















    void Helpbox()
    {
        if (!string.IsNullOrEmpty(HelpboxText))
            EditorGUILayout.HelpBox(HelpboxText, MessageType.Info);
    }



    void Autocomplete()
    {
        CheckNextStage();
        MakeWallsStatic();
        FindStart();
        FindEnd();
        CheckTileTool();
    }


    void FindStart()
    {
        if (Start.objectReferenceValue == null)
        {
            List<Transform> starts = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "Start" && t.root == stage.transform).ToList();
            if (starts.Count < 1)
            {
                HelpLog("Missing Start Position", MessageType.Error);
            }
            else
            {
                Start.objectReferenceValue = starts.First();
            }
        }
    }

    void FindEnd()
    {
        if (Finish.objectReferenceValue == null)
        {
            HelpLog("Finish goal cannot be found", MessageType.Error);
        }
    }

    void MakeWallsStatic()
    {
        List<Collider2D> collidersWithoutRB = GameObject.FindObjectsOfType<Collider2D>().Where(c =>
            c.transform.root == stage.transform &&
            (c.name.StartsWith("Wall") || c.name.StartsWith("wall")) &&
            c.GetComponent<Rigidbody2D>() == null).ToList();

        collidersWithoutRB.ForEach(c => c.gameObject.isStatic = true);
    }

    void CheckNextStage()
    {
        if (NextStage.objectReferenceValue == null)
        {
            HelpLog("Next stage unassigned!", MessageType.Warning);
        }
    }

    void CheckTileTool()
    {
        MonobehaviorEditorTool tool = StageInScene.gameObject.GetComponent<MonobehaviorEditorTool>();
        if (tool != null)
        {
            Destroy(tool);
            HelpLog("Tile tool removed.", MessageType.Warning);
        }
    }

    void HelpLog(string zMsg, MessageType zType = MessageType.Info)
    {
        HelpboxText = zMsg;
    }

    void ShowTileTool()
    {
        if (StageInScene == null)
        {
            Debug.LogError("Error.");
            return;
        }

        MonobehaviorEditorTool tool = StageInScene.gameObject.GetComponent<MonobehaviorEditorTool>();

        if (tool == null)
        {
            StageInScene.gameObject.AddComponent<MonobehaviorEditorTool>();
        }
    }

    void UseTileTool()
    {
        MonobehaviorEditorTool tool = StageInScene.gameObject.GetComponent<MonobehaviorEditorTool>();
        if (tool == null)
        {
            ShowTileTool();
            tool = StageInScene.gameObject.GetComponent<MonobehaviorEditorTool>();
        }

        if (tool.ReferenceSprite == null)
        {
            Debug.LogError("Error: Reference sprite not configured.");
            return;
        }

        List<SpriteRenderer> FloorTiles = StageInScene.GetComponentsInChildren<SpriteRenderer>().ToList();

        FloorTiles.RemoveAll(t => !tool.Tiles.Any(x => x.Graphic == t.sprite) && t != tool.ReferenceSprite);
        //FloorTiles.RemoveAll(t => t != tool.ReferenceSprite);
        tool.Apply(FloorTiles);
    }

}
