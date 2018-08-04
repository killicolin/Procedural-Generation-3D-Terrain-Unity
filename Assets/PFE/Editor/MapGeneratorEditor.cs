using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor {
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        //si les parametres ont été modifié et autoUpdate coché
        if(DrawDefaultInspector() & mapGen.autoUpdate)
        {
            // on génère
            mapGen.GenerateMap();
        }
        //si on appi sur le boutton générate
        if (GUILayout.Button("Generate"))
        {
            // on génère
            mapGen.GenerateMap();
        }
    }
}
