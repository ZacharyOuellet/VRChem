#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ForceParentSelection
{
    static ForceParentSelection()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }

    static void OnSelectionChanged()
    {
        if (Selection.activeTransform == null)
            return;

        // Check if we clicked inside something with PreventChildSelection
        var parentMarker = Selection.activeTransform.GetComponentInParent<PreventChildSelection>();
        if (parentMarker == null)
            return;

        var target = parentMarker.transform;

        if (Selection.activeTransform == target)
            return;

        // MUST delay to the next editor update to override Unity
        EditorApplication.delayCall += () =>
        {
            Selection.activeTransform = target;
        };
    }
}
#endif
