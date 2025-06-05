// Assets/Editor/OrderDataEditor.cs
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OrderData))]
public class OrderDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector UI
        DrawDefaultInspector();

        // Mark the object as dirty if anything changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);          // Mark the asset as dirty
            AssetDatabase.SaveAssets();              // Save changes to disk
        }
    }
}
