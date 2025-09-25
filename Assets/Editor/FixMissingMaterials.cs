using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class FixAllTilesAndMaterials
{
    [MenuItem("Tools/Fix Missing Tile & Sprite Materials")]
    public static void FixTilesAndMaterials() {
        Material defaultMat = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
        int fixedCount = 0;

        // 修復場景內
        foreach (var tr in Object.FindObjectsOfType<TilemapRenderer>())
        {
            if (tr.sharedMaterial == null || tr.sharedMaterial.shader == null)
            {
                tr.sharedMaterial = defaultMat;
                fixedCount++;
            }
        }

        // 修復專案內所有 Tile Asset
        string[] tileGuids = AssetDatabase.FindAssets("t:TileBase");
        foreach (var guid in tileGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var tile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
            if (tile == null) continue;

            SerializedObject so = new SerializedObject(tile);
            var matProp = so.FindProperty("m_Material"); // TileBase 裡的 material 欄位
            if (matProp != null && (matProp.objectReferenceValue == null))
            {
                matProp.objectReferenceValue = defaultMat;
                so.ApplyModifiedProperties();
                fixedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ 修復完成，共修復 {fixedCount} 個 Tile/Renderer。");
    }
}
