using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveSystemExperimental))]
public class SaveSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (SaveSystemExperimental)target;

        #region Delete Saves

        var style = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fixedHeight = 40f
        };

        style.normal.textColor = Color.red;

        if(GUILayout.Button("Delete All Saves", style))
        {
            script.DeleteSave(0);
        }

        if(GUILayout.Button("Delete Level Progress Saves", style))
        {
           script.DeleteSave(1);
        }

        if(GUILayout.Button("Delete Money Saves", style))
        {
            script.DeleteSave(2);
        }

        #endregion

        #region Set Saves

        style.normal.textColor = Color.blue;

        if(GUILayout.Button("Set Level", style))
        {
            script.SaveCurrentLevel();
        }

        /*if(GUILayout.Button("Unlock, Select, Save a Costume", style))
        {
            script.UnlockSelect_and_SaveCostume();
        }*/

        #endregion

    }
}
#endif
