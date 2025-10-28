using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfDetectTypeAttribute))]
public class ShowIfDetectTypeDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        //1️先抓到「所在父物件」的 DetectType 屬性
        var parent = property.serializedObject;
        SerializedProperty detectTypeProp = property.FindPropertyRelative("DetectType");

        //上面這行會找不到，因為當前 property 是 "DetectRadius" 或 "ConeRadius"，
        //所以要往上找 SkillTemplate 的根節點：
        detectTypeProp = property.serializedObject.FindProperty(property.propertyPath.Replace(property.name, "DetectType"));

        if (detectTypeProp == null) {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        //3取出屬性標籤與當前類型
        var showIf = (ShowIfDetectTypeAttribute)attribute;
        SkillDetectType currentType = (SkillDetectType)detectTypeProp.enumValueIndex;

        //3條件符合才畫
        if (currentType == showIf.ExpectedType) {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        SerializedProperty detectTypeProp = property.serializedObject.FindProperty(property.propertyPath.Replace(property.name, "DetectType"));

        if (detectTypeProp == null)
            return EditorGUI.GetPropertyHeight(property, label, true);

        var showIf = (ShowIfDetectTypeAttribute)attribute;
        SkillDetectType currentType = (SkillDetectType)detectTypeProp.enumValueIndex;

        return currentType == showIf.ExpectedType
            ? EditorGUI.GetPropertyHeight(property, label, true)
            : -EditorGUIUtility.standardVerticalSpacing; // 隱藏
    }
}
