using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Configuration))]
public class ConfigurationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (Configuration)target;

        if (GUILayout.Button("Import CSV: Level Information", GUILayout.Height(40)))
        {
            script.ImportCSV(Enums.CSVTypeToLoad.levels);
        }

        if (GUILayout.Button("Import CSV: Item Information", GUILayout.Height(40)))
        {
            script.ImportCSV(Enums.CSVTypeToLoad.items);
        }
    }
}
#endif
