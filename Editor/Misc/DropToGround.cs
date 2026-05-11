using UnityEngine;
using UnityEditor;

namespace RomainUTR.SLToolbox.Editor
{
    public static class DropToGround
    {
        [MenuItem("SL Toolbox/Drop To Ground &d")]
        public static void Drop()
        {
            Transform[] selectedTransforms = Selection.transforms;

            if (selectedTransforms.Length == 0) return;

            Undo.RecordObjects(selectedTransforms, "Drop To Ground");

            int movedCount = 0;

            foreach (Transform t in selectedTransforms)
            {
                Renderer rend = t.GetComponentInChildren<Renderer>();

                if (rend == null)
                {
                    Debug.LogWarning($"SL Toolbox: Cannot drop {t.name} to ground. No Renderer found on the object or its children.");
                    continue;
                }

                bool rendWasEnabled = rend.enabled;
                rend.enabled = false;

                Vector3 rayOrigin = t.position;
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 1000f))
                {
                    float distPivotToBottom = t.position.y - rend.bounds.min.y;

                    t.position = new Vector3(t.position.x, hit.point.y + distPivotToBottom, t.position.z);
                    movedCount++;
                }

                rend.enabled = rendWasEnabled;
            }

            if (movedCount > 0) Debug.Log($"SL Toolbox: Dropped {movedCount} objects cleanly to the ground.");
        }
    }
}