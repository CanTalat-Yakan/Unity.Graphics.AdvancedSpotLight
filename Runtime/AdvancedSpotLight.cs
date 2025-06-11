using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEssentials
{
    [ExecuteAlways]
    [RequireComponent(typeof(Light))]
    public class AdvancedSpotLight : MonoBehaviour
    {
        [Range(0, 1)] public float ColorFringing = 0.25f;
        [Range(-1, 1)] public float ColorShifting = 0.25f;

        [HideInInspector] public Light MainLight;
        [HideInInspector] public Light RedLight;
        [HideInInspector] public Light GreenLight;
        [HideInInspector] public Light BlueLight;

        [HideInInspector] public HDAdditionalLightData MainLightData;
        [HideInInspector] public HDAdditionalLightData RedLightData;
        [HideInInspector] public HDAdditionalLightData GreenLightData;
        [HideInInspector] public HDAdditionalLightData BlueLightData;

#if UNITY_EDITOR
        public void Update()
        {
            if (Selection.activeGameObject == gameObject)
                UpdateChannelLights();
        }
#endif

        private void UpdateChannelLights()
        {
            if(MainLight == null || RedLight == null || GreenLight == null || BlueLight == null)
                return;

            if(MainLightData == null || RedLightData == null || GreenLightData == null || BlueLightData == null)
                return;

            UpdateChannelParameters();

            CopyLightProperties(MainLight, RedLight, Color.red);
            CopyLightProperties(MainLight, GreenLight, Color.green);
            CopyLightProperties(MainLight, BlueLight, Color.blue);

            CopyHDLightData(MainLight, RedLight, RedLightData);
            CopyHDLightData(MainLight, GreenLight, GreenLightData);
            CopyHDLightData(MainLight, BlueLight, BlueLightData);
        }

        private void UpdateChannelParameters()
        {
            // Apply ColorFringing as a spot angle offset
            float t = Mathf.InverseLerp(10f, 120f, MainLight.spotAngle);
            float fringing = ColorFringing * 25f;
            RedLight.spotAngle = Mathf.Clamp(MainLight.spotAngle + fringing, 1, 160);
            GreenLight.spotAngle = Mathf.Clamp(MainLight.spotAngle + fringing * 0.5f, 1, 160 - fringing * 0.5f);
            BlueLight.spotAngle = Mathf.Clamp(MainLight.spotAngle + fringing * 0.25f, 1, 160 - fringing * 0.75f);

            // Apply ColorShifting as a rotation offset
            float shiftH = ColorShifting * 5;
            float shiftV = Mathf.Max(0, ColorShifting * -1) * 5;
            RedLight.transform.localEulerAngles = Vector3.zero;
            GreenLight.transform.localEulerAngles = new Vector3(shiftV, shiftH * 0.5f, 0);
            BlueLight.transform.localEulerAngles = new Vector3(-shiftV, shiftH, 0);
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
            hdData.SetShadowLightLayer(MainLightData.lightlayersMask);
            hdData.SetShadowResolution(GetResolutionFromIndex(MainLightData.shadowResolution.level));

            hdData.innerSpotPercent = Mathf.Min(95, MainLightData.innerSpotPercent);

            // Shadows
            hdData.shadowUpdateMode = MainLightData.shadowUpdateMode;
            hdData.shadowDimmer = MainLightData.shadowDimmer;
            hdData.shadowNearPlane = MainLightData.shadowNearPlane;
            hdData.maxDepthBias = MainLightData.maxDepthBias;
            hdData.normalBias = MainLightData.normalBias;
            hdData.volumetricDimmer = MainLightData.volumetricDimmer;
            hdData.shadowTint = MainLightData.shadowTint;
            hdData.fadeDistance = MainLightData.fadeDistance;
            hdData.softnessScale = MainLightData.softnessScale;
            hdData.blockerSampleCount = MainLightData.blockerSampleCount;
            hdData.blockerSampleCount = MainLightData.blockerSampleCount;
            hdData.filterSampleCount = MainLightData.filterSampleCount;
            hdData.lightShadowRadius = MainLightData.lightShadowRadius;
            hdData.rayTraceContactShadow = MainLightData.rayTraceContactShadow;

            // IES
            if (MainLightData.IESTexture != null)
            {
                hdData.IESTexture = MainLightData.IESTexture;
                hdData.spotIESCutoffPercent = MainLightData.spotIESCutoffPercent;

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