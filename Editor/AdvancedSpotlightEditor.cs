#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class AdvancedSpotlightEditor
    {
        [MenuItem("GameObject/Essentials/Advanced Spot Light", false, priority = 121)]
        private static void InstantiateAdvancedSpotLight(MenuCommand menuCommand)
        {
            var go = new GameObject("Advanced Spot Light");
            var advancedSpotLight = go.AddComponent<AdvancedSpotLight>();
            var light = go.GetComponent<Light>();
            light.type = LightType.Spot;

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Advanced Spot Light");
            Selection.activeObject = go;
        }
    }
}
#endif