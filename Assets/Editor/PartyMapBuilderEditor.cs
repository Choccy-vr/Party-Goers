#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
[CustomEditor(typeof(PartyMapBuilder))]
public class PartyMapBuilderEditor : Editor
{
    static PartyMapBuilderEditor()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PartyMapBuilder builder = (PartyMapBuilder)target;

        GUILayout.Space(15);
        GUI.backgroundColor = Color.cyan;

        if (GUILayout.Button("➕ Add Next Party Space", GUILayout.Height(40)))
        {
            GlobalAddNewSpace();
        }

        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("🧹 Clear Map Layout", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Clear Map?", "Delete all generated spaces under this builder?", "Yes", "No"))
            {
                ClearAllSpaces(builder);
            }
        }
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(10, 10, 180, 50));
        GUI.backgroundColor = Color.cyan;

        if (GUILayout.Button("➕ New Party Space (Shift+G)", GUILayout.Height(35)))
        {
            GlobalAddNewSpace();
        }

        GUILayout.EndArea();

        Handles.EndGUI();
    }

    [MenuItem("Tools/Party Game/Add Next Space #g")]
    public static void GlobalAddNewSpace()
    {
        PartyMapBuilder builder = FindAnyObjectByType<PartyMapBuilder>();

        if (builder == null)
        {
            Debug.LogWarning("Could not find a PartyMapBuilder GameObject in the active scene!");
            return;
        }

        if (builder.spacePrefab == null)
        {
            Debug.LogError("Assign a Space Prefab to the PartyMapBuilder component first!");
            return;
        }

        builder.editorSpaces.RemoveAll(item => item == null);

        Vector3 spawnPosition = builder.transform.position;
        Quaternion spawnRotation = Quaternion.identity;
        PartySpace sourceSpace = null;

        // 1. Context Aware Placement: Check if the user is selecting an existing space
        if (Selection.activeGameObject != null)
        {
            sourceSpace = Selection.activeGameObject.GetComponent<PartySpace>();
        }

        // 2. Calculate position and direction based on selection
        if (sourceSpace != null)
        {
            spawnPosition = sourceSpace.transform.position + (sourceSpace.transform.forward * 3.0f);
            spawnRotation = sourceSpace.transform.rotation;
        }
        else if (builder.editorSpaces.Count > 0)
        {
            // Fallback to the last built item if nothing is selected
            sourceSpace = builder.editorSpaces[builder.editorSpaces.Count - 1];
            spawnPosition = sourceSpace.transform.position + (sourceSpace.transform.forward * 3.0f);
            spawnRotation = sourceSpace.transform.rotation;
        }

        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(builder.spacePrefab, builder.transform);
        newObj.transform.position = spawnPosition;
        newObj.transform.rotation = spawnRotation;

        Undo.RegisterCreatedObjectUndo(newObj, "Add Party Space");

        PartySpace newSpace = newObj.GetComponent<PartySpace>();
        if (newSpace == null) newSpace = newObj.AddComponent<PartySpace>();

        int newId = builder.editorSpaces.Count;
        newObj.name = $"Space_{newId:D2}";

        Undo.RecordObject(newSpace, "Setup Party Space");
        newSpace.spaceID = newId;

        if (newSpace.nextSpace == null)
        {
            newSpace.nextSpace = new List<PartySpace>();
        }
        else
        {
            newSpace.nextSpace.Clear();
        }

        // 3. Proximity check
        Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, builder.detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            PartySpace nearbySpace = hitCollider.GetComponent<PartySpace>();
            if (nearbySpace != null && nearbySpace != newSpace)
            {
                if (!newSpace.nextSpace.Contains(nearbySpace))
                {
                    newSpace.nextSpace.Add(nearbySpace);
                }
            }
        }

        // 4. Connect the source node to this new node (Handles Linear OR Split Paths)
        if (sourceSpace != null)
        {
            Undo.RecordObject(sourceSpace, "Link Party Space");

            if (sourceSpace.nextSpace == null)
            {
                sourceSpace.nextSpace = new List<PartySpace>();
            }

            sourceSpace.nextSpace.RemoveAll(item => item == null);

            if (!sourceSpace.nextSpace.Contains(newSpace))
            {
                sourceSpace.nextSpace.Add(newSpace);
            }
            EditorUtility.SetDirty(sourceSpace);
        }

        Undo.RecordObject(builder, "Update Builder List");
        builder.editorSpaces.Add(newSpace);

        Selection.activeGameObject = newObj;

        EditorUtility.SetDirty(builder);
        EditorUtility.SetDirty(newSpace);
    }

    private void ClearAllSpaces(PartyMapBuilder builder)
    {
        Undo.RecordObject(builder, "Clear All Spaces");
        for (int i = builder.transform.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(builder.transform.GetChild(i).gameObject);
        }
        builder.editorSpaces.Clear();
        EditorUtility.SetDirty(builder);
    }
}
#endif