using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DetectSpriteSpawner))]
public class DetectSpriteSpawnerEditor : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();
        var spawner = (DetectSpriteSpawner)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShapeType"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTransform"), new GUIContent("父目標"));
        if (spawner.ShapeType == DetectShapeType.Rectangle) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rectangleSprite"), new GUIContent("矩形 Sprite"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rangeX"), new GUIContent("Range X"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rangeY"), new GUIContent("Range Y"));
        }
        else if (spawner.ShapeType == DetectShapeType.Circle) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("circleSprite"), new GUIContent("圓形 Sprite"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"), new GUIContent("Radius"));
        }

        EditorGUILayout.Space();
        //if (GUILayout.Button("重新生成範圍")) {
        //    spawner.SpawnShape();
        //}

        serializedObject.ApplyModifiedProperties();
    }
}
