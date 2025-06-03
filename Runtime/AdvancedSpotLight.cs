using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEssentials
{
    [ExecuteAlways]
    [RequireComponent(typeof(Light))]
    public class AdvancedSpotLight : MonoBehaviour
    {
        [Range(0, 25)] public float ColorFringing = 10f;

        [HideInInspector] public Light MainLight;
        [HideInInspector] public Light RedLight;
        [HideInInspector] public Light GreenLight;
        [HideInInspector] public Light BlueLight;

        [HideInInspector] public HDAdditionalLightData MainLightHD;
        [HideInInspector] public HDAdditionalLightData RedLightHD;
        [HideInInspector] public HDAdditionalLightData GreenLightHD;
        [HideInInspector] public HDAdditionalLightData BlueLightHD;

#if UNITY_EDITOR
        public void Update()
        {
            if (Selection.activeGameObject == gameObject)
                UpdateChannelLights();
        }
#endif

        private void UpdateChannelLights()
        {
            float fringing = GetAutoColorFringing(MainLight.spotAngle);
            RedLight.spotAngle = Mathf.Clamp(MainLight.spotAngle + fringing, 1, 160);
            GreenLight.spotAngle = Mathf.Clamp(MainLight.spotAngle + fringing / 2, 1, 160 - fringing * 0.5f);
            BlueLight.spotAngle = Mathf.Clamp(MainLight.spotAngle + fringing / 4, 1, 160 - fringing * 0.75f);

            CopyLightProperties(MainLight, RedLight, Color.red);
            CopyLightProperties(MainLight, GreenLight, Color.green);
            CopyLightProperties(MainLight, BlueLight, Color.blue);

            CopyHDLightData(MainLight, RedLight, RedLightHD);
            CopyHDLightData(MainLight, GreenLight, GreenLightHD);
            CopyHDLightData(MainLight, BlueLight, BlueLightHD);
        }

        private float GetAutoColorFringing(float spotAngle)
        {
            const float minSpotAngle = 10f;
            const float maxSpotAngle = 120f;

            float t = Mathf.InverseLerp(minSpotAngle, maxSpotAngle, spotAngle);
            return Mathf.Lerp(0f, ColorFringing, t);
        }

        private void CopyLightProperties(Light source, Light target, Color color)
        {
            // Basic properties
            target.type = source.type;
            target.color = color * source.color;
            target.colorTemperature = source.colorTemperature;
            target.useColorTemperature = source.useColorTemperature;
            target.lightUnit = source.lightUnit;
            target.intensity = source.intensity;
            target.range = source.range;
            target.innerSpotAngle = source.innerSpotAngle;
            target.enableSpotReflector = source.enableSpotReflector;

            // Rendering
            target.renderMode = source.renderMode;
            target.cullingMask = source.cullingMask;
            target.renderingLayerMask = source.renderingLayerMask;
            target.flare = source.flare;
            target.bounceIntensity = source.bounceIntensity;
            target.lightmapBakeType = source.lightmapBakeType;
            target.areaSize = source.areaSize;
            target.renderingLayerMask = source.renderingLayerMask;

            // Shadows
            target.shadows = source.shadows;
            target.shadowStrength = source.shadowStrength;
            target.shadowResolution = source.shadowResolution;
            target.shadowBias = source.shadowBias;
            target.shadowNormalBias = source.shadowNormalBias;
            target.shadowNearPlane = source.shadowNearPlane;
        }

        private void CopyHDLightData(Light source, Light target, HDAdditionalLightData hdData)
        {
            // Basic properties
            hdData.EnableShadows(source.shadows != LightShadows.None);
            hdData.SetShadowLightLayer(MainLightHD.lightlayersMask);
            hdData.SetShadowResolution(GetResolutionFromIndex(MainLightHD.shadowResolution.level));

            hdData.innerSpotPercent = Mathf.Min(95, MainLightHD.innerSpotPercent);

            // Shadows
            hdData.shadowUpdateMode = MainLightHD.shadowUpdateMode;
            hdData.shadowDimmer = MainLightHD.shadowDimmer;
            hdData.shadowNearPlane = MainLightHD.shadowNearPlane;
            hdData.maxDepthBias = MainLightHD.maxDepthBias;
            hdData.normalBias = MainLightHD.normalBias;
            hdData.volumetricDimmer = MainLightHD.volumetricDimmer;
            hdData.shadowTint = MainLightHD.shadowTint;
            hdData.fadeDistance = MainLightHD.fadeDistance;
            hdData.softnessScale = MainLightHD.softnessScale;
            hdData.blockerSampleCount = MainLightHD.blockerSampleCount;
            hdData.blockerSampleCount = MainLightHD.blockerSampleCount;
            hdData.filterSampleCount = MainLightHD.filterSampleCount;
            hdData.lightShadowRadius = MainLightHD.lightShadowRadius;
            hdData.rayTraceContactShadow = MainLightHD.rayTraceContactShadow;

            // IES
            if (MainLightHD.IESTexture != null)
            {
                hdData.IESTexture = MainLightHD.IESTexture;
                hdData.spotIESCutoffPercent = MainLightHD.spotIESCutoffPercent;

                if (source.cookie != null)
                {
                    hdData.SetCookie(source.cookie);
                    target.cookie = source.cookie;
                    target.cookieSize = source.cookieSize;
                }
            }
        }

        public static int GetResolutionFromIndex(int index) =>
            index switch
            {
                0 => 512,
                1 => 1024,
                2 => 2048,
                3 => 4096,
                4 => 8192,
                _ => 2048
            };
    }
}