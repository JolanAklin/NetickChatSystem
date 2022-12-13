using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(ChatMasks))]
public class ChatMasksEditor : Editor {

    public VisualTreeAsset _visual;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        _visual.CloneTree(root);
        return root;
    }
}