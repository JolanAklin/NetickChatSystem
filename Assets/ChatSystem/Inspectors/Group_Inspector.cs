using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.Progress;

namespace ChatSystem.Inspectors
{
    [CustomEditor(typeof(Group))]
    public class Group_Inspector : Editor
    {

        SerializedProperty maskField;

        private void OnEnable()
        {
            // Fetch the serialized property of the mask field
            maskField = serializedObject.FindProperty("_mask");
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object
            serializedObject.Update();

            // Display default inspector property fields
            string[] toMask = new string[] { "_mask" };
            DrawPropertiesExcluding(serializedObject, toMask);

            // Draw the mask field using EditorGUILayout.MaskField
            if(ChatSystemManager.ChatSystemConfig == null)
            {
                EditorGUILayout.LabelField("Create a gameobject with ChatSystemManager and set the ChatSystemConfig");
            }
            else{
                maskField.intValue = EditorGUILayout.MaskField("Can see messages from", maskField.intValue, ChatSystemManager.ChatSystemConfig?.getGroupList());
            }

            // Apply changes to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
    }
}
