// Assets/Editor/FoodItemEditor.cs
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FoodItem))]
public class FoodItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // If any GUI field was changed, mark dirty and save
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);      // Mark asset dirty
            AssetDatabase.SaveAssets();          // Write to disk
        }
    }
}
