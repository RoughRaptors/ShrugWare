using System;
using UnityEditor;
using UnityEngine;

namespace JZ.SCENE
{
    /// <summary>
    /// <para>Tells the SceneTransitioner class how to behave during a transition</para>
    /// </summary>
    [Serializable]
    public struct SceneTransitionData
    {
        [Tooltip("Set to none for a non-Async transition")] public AnimType animationType;
        public bool additiveLoad;
        public bool unloadMyScene;
        [Tooltip("Only relevant if additive loading.")] public string[] scenesToUnload;
        [HideInInspector] public int targetSceneIndex;
        [HideInInspector] public int mySceneIndex;
        [HideInInspector] public string mySceneName;
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneTransitionData))]
    public class SceneTransitionDataDrawer : PropertyDrawer
    {
        static int rectNo = 0;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.isExpanded) 
                return EditorGUIUtility.singleLineHeight * (rectNo + 1);
            else 
                return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            rectNo = 0;
            EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUI.Foldout(NextRect(position), property.isExpanded, label);
            if(!property.isExpanded) return;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.PropertyField(NextRect(position), property.FindPropertyRelative("animationType"));
            var isAdditive = property.FindPropertyRelative("additiveLoad");
            EditorGUI.PropertyField(NextRect(position), isAdditive);
            

            if(isAdditive.boolValue)
            {
                EditorGUI.PropertyField(NextRect(position), property.FindPropertyRelative("unloadMyScene"));
                var sceneArray = property.FindPropertyRelative("scenesToUnload");
                EditorGUI.PropertyField(NextRect(position), sceneArray, true);
                if(sceneArray.isExpanded)
                {
                    EditorGUI.LabelField(NextRect(position), "");
                    EditorGUI.LabelField(NextRect(position), "");
                    for(int ii = 0; ii < sceneArray.arraySize; ii++)
                    {
                        EditorGUI.LabelField(NextRect(position), "");
                    }
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private Rect NextRect(Rect position)
        {
            var rect = new Rect(position.x, position.y + rectNo * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            rectNo++;
            return rect;
        }
    }
    #endif
}