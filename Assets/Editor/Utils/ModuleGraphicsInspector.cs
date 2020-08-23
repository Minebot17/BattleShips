using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModuleGraphics))]
public class ModuleGraphicsInspector : Editor {

    private string modulesTemplate = "Assets/Textures/Ship/Modules/ship_{prefix}-{index}.png";
    private string bloomTemplate = "Assets/Textures/Ship/Bloom/bloom_{prefix}-{index}.png";
    private SerializedProperty moduleSprites;
    private SerializedProperty bloomSprites;
    private SerializedProperty textureIndex;
    private SerializedProperty currentColor;
    
    void OnEnable()
    {
        moduleSprites = serializedObject.FindProperty("moduleSprites");
        bloomSprites = serializedObject.FindProperty("bloomSprites");
        textureIndex = serializedObject.FindProperty("textureIndex");
        currentColor = serializedObject.FindProperty("currentColor");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GUILayout.Label("Modules template:");
        modulesTemplate = GUILayout.TextField(modulesTemplate);
        
        GUILayout.Label("Bloom template:");
        bloomTemplate = GUILayout.TextField(bloomTemplate);
        EditorGUILayout.PropertyField(textureIndex);

        Array colors = Enum.GetValues(typeof(ShipColor));
        if (GUILayout.Button("Set textures")) {
            Sprite[] moduleSprites = new Sprite[colors.Length];
            Sprite[] bloomSprites = new Sprite[colors.Length];
            
            foreach (ShipColor color in colors) {
                moduleSprites[(int) color] = AssetDatabase.LoadAssetAtPath<Sprite>(
                    modulesTemplate
                        .Replace("{prefix}", color.GetPrefix())
                        .Replace("{index}", textureIndex.stringValue)
                );
                
                bloomSprites[(int) color] = AssetDatabase.LoadAssetAtPath<Sprite>(
                    bloomTemplate
                        .Replace("{prefix}", color.GetPrefix())
                        .Replace("{index}", textureIndex.stringValue)
                );
            }

            this.moduleSprites.arraySize = colors.Length;
            this.bloomSprites.arraySize = colors.Length;

            for (int i = 0; i < colors.Length; i++)
                this.moduleSprites.GetArrayElementAtIndex(i).objectReferenceValue = moduleSprites[i];
            
            for (int i = 0; i < colors.Length; i++)
                this.bloomSprites.GetArrayElementAtIndex(i).objectReferenceValue = bloomSprites[i];
            
            serializedObject.ApplyModifiedProperties();
            ((ModuleGraphics)target).SetColor((ShipColor) colors.GetValue(currentColor.enumValueIndex));
        }
        
        EditorGUILayout.PropertyField(currentColor);
        if (GUILayout.Button("Set color")) {
            ((ModuleGraphics) target).SetColor((ShipColor) colors.GetValue(currentColor.enumValueIndex));
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.PropertyField(moduleSprites);
        EditorGUILayout.PropertyField(bloomSprites);
        serializedObject.ApplyModifiedProperties();
    }
}