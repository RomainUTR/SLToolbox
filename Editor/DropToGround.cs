using UnityEngine;
using UnityEditor;
using System.Linq;

namespace RomainUTR.SLToolbox.Editor
{
    public static class DropToGround
    {
        [MenuItem("Tools/SL Toolbox/Drop To Ground &d")]
        public static void Drop()
        {
            Transform[] selectedTransforms = Selection.transforms;

            if (selectedTransforms.Length == 0) return;

            Undo.RecordObjects(selectedTransforms, "Drop To Ground");

            int movedCount = 0;

            foreach (Transform t in selectedTransforms)
            {
                Collider col = t.GetComponent<Collider>();

                float startY = t.position.y + 10f;
                if (col != null) startY = col.bounds.max.y + 2f;

                Vector3 rayOrigin = new Vector3(t.position.x, startY, t.position.z);

                Debug.DrawRay(rayOrigin, Vector3.down * 20f, Color.yellow, 2f);

                RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, 100f);

                RaycastHit groundHit = new RaycastHit();
                bool foundGround = false;

                foreach (var hit in hits.OrderBy(h => h.distance))
                {
                    if (hit.transform == t || hit.transform.IsChildOf(t))
                        continue;

                    groundHit = hit;
                    foundGround = true;
                    break;
                }

                if (foundGround)
                {
                    float distPivotToBottom = 0f;
                    if (col != null)
                    {
                        distPivotToBottom = t.position.y - col.bounds.min.y;
                    }

                    t.position = new Vector3(t.position.x, groundHit.point.y + distPivotToBottom, t.position.z);
                    movedCount++;
                }
            }

            if (movedCount > 0) Debug.Log($"Dropped {movedCount} objects.");
        }
    }
}