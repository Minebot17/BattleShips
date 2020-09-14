using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapInspector : Editor {
    private SerializedProperty Size;
    private SerializedProperty SpawnPoints;
    private Transform spawnPointsParent;

    void OnEnable()
    {
        Size = serializedObject.FindProperty("Size");
        SpawnPoints = serializedObject.FindProperty("spawnPoints");
        spawnPointsParent = ((Map) target).transform.Find("SpawnPoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(Size);
        EditorGUILayout.PropertyField(SpawnPoints);

        GUILayout.Space(10);
        GUILayout.Label("Load spawn point from child:");
        spawnPointsParent = (Transform) EditorGUILayout.ObjectField(spawnPointsParent, typeof(Transform), true);
        if (GUILayout.Button("Set spawn points")) {
            SpawnPoints.ClearArray();
            SpawnPoints.arraySize = spawnPointsParent.childCount;
            for (int i = 0; i < spawnPointsParent.childCount; i++)
                SpawnPoints.GetArrayElementAtIndex(i).vector2Value = spawnPointsParent.GetChild(i).localPosition.ToVector2();
        }

        serializedObject.ApplyModifiedProperties();
    }
}