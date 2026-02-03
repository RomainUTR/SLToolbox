using UnityEditor;
using RomainUTR.SLToolbox;

namespace RomainUTR.SLToolbox.Editor
{
    // "true" signifie : Applique cet éditeur à SLBehaviour ET tous ses enfants
    [CustomEditor(typeof(SLBehaviour), true)]
    [CanEditMultipleObjects]
    public class SLBehaviourEditor : SLEditor
    {
        // Vide : Il hérite de SLEditor qui fait déjà tout le travail
    }
}