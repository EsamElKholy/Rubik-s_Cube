using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RubikGenerator))]
public class RubikGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Cube"))
        {
            var generator = target as RubikGenerator;

            generator.GenerateCube();
        }
    }
}
