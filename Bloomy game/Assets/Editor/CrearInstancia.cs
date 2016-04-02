using UnityEngine;
using UnityEditor;
using System.Collections;

public class CrearInstancia : Editor {

    [MenuItem("Window/Crear nuevo archivo", false, 150)]
    public static void CreateAsset() {
        ListaItem asset = ScriptableObject.CreateInstance<ListaItem>();
        AssetDatabase.CreateAsset(asset, "Assets/items.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
