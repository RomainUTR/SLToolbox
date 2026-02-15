using UnityEngine;

namespace RomainUTR.SLToolbox
{
    [System.Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        public UnityEditor.SceneAsset sceneAsset;
#endif

        [SerializeField] private string scenePath;

        // Permet de convertir automatiquement en string
        public static implicit operator string(SceneReference sceneRef)
        {
            return sceneRef.scenePath;
        }

        // Avant que Unity sauvegarde la scène ou le prefab, on met à jour le chemin texte
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (sceneAsset != null)
            {
                string path = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
                scenePath = path;
            }
#endif
        }

        public void OnAfterDeserialize() { }
    }
}