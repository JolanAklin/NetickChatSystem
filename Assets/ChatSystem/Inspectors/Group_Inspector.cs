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
        string[] options = new string[] { "CanJump", "CanShoot", "CanSwim" };

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
            maskField.intValue = EditorGUILayout.MaskField("Can see messages from", maskField.intValue, Config.instance.getGroupList());

            // Apply changes to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
    }
}
