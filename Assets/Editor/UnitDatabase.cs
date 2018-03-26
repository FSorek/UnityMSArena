using UnityEngine;
using UnityEditor;
using System.Collections;

public class UnitDatabase : EditorWindow {

    [MenuItem("Window/Unit Database")]
    public static void ShowWindow()
    {
        GetWindow(typeof(UnitDatabase));
    }
    void OnGUI()
    {
         
    }
}
