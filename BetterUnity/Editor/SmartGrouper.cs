using UnityEngine;
using UnityEditor;

namespace RomainUTR.SLToolbox.BetterUnity.Editor
{
    public static class SmartGrouper
    {
        [MenuItem("Tools/SL Toolbox/ Group Selected %g")]
        private static void GroupSelected()
        {
            if (Selection.gameObjects.Length == 0) return;

            Undo.IncrementCurrentGroup();
            int undoGroupIndex = Undo.GetCurrentGroup();

            Vector3 centerPos = Vector3.zero;
            foreach (var go in Selection.gameObjects)
            {
                centerPos += go.transform.position;
            }
            centerPos /= Selection.gameObjects.Length;

            GameObject newParent = new GameObject("New Group");
            newParent.transform.position = centerPos;

            Undo.RegisterCreatedObjectUndo(newParent, "Create Group");

            var selectedObjects = Selection.gameObjects;
            System.Array.Sort(selectedObjects, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

            foreach (var go in Selection.gameObjects)
            {
                Undo.SetTransformParent(go.transform, newParent.transform, "Group Selected");
            }

            Selection.activeGameObject = newParent;

            Undo.CollapseUndoOperations(undoGroupIndex);
        }
    }
}