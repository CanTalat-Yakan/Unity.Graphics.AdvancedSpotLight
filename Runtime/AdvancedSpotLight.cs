using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEssentials
{
    [ExecuteAlways]
    [RequireComponent(typeof(Light))]
    public class AdvancedSpotLight : MonoBehaviour
    {
        [Range(0, 10)] public float _colorFringing = 5f;

        private Light _mainLight;
        private Light _redLight;
        private Light _greenLight;
        private Light _blueLight;

        private HDAdditionalLightData _mainLightHD;
        private HDAdditionalLightData _redLightHD;
        private HDAdditionalLightData _greenLightHD;
        private HDAdditionalLightData _blueLightHD;

        public void Start()
        {
            SetupChannelLights();
            UpdateChannelLights();
        }

#if UNITY_EDITOR
        public void Update()
        {
            if (Selection.activeGameObject == gameObject)
                UpdateChannelLights();
        }

        public void OnValidate() =>
            UpdateChannelLights();
#endif

        private void SetupChannelLights()
        {
            _mainLight = GetComponent<Light>();
            _mainLight.enabled = false;

            const string RedChannelName = "RedChannel";
            const string GreenChannelName = "GreenChannel";
            const string BlueChannelName = "BlueChannel";

            // Create or find child lights
            GetOrCreateChannelLight(BlueChannelName, Color.blue, out _blueLight, out _blueLightHD);
            GetOrCreateChannelLight(GreenChannelName, Color.green, out _greenLight, out _greenLightHD);
            GetOrCreateChannelLight(RedChannelName, Color.red, out _redLight, out _redLightHD);
        }

        private void GetOrCreateChannelLight(string name, Color color, out Light light, out HDAdditionalLightData hdData)
        {
            Transform child = transform.Find(name);
            GameObject go;
            if (child != null)
                go = child.gameObject;
            else
            {
                go = new GameObject(name);
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                //go.hideFlags = HideFlags.HideInHierarchy;
            }

            light = go.GetComponent<Light>();
            if (light == null)
                light = go.AddComponent<Light>();

            light.type = LightType.Spot;
            light.color = color;

            hdData = go.GetComponent<HDAdditionalLightData>();
        }

        private void UpdateChannelLights()
        {
            if (_mainLight == null || _redLight == null || _greenLight == null || _blueLight == null)
                return;

            CopyLightProperties(_mainLight, _redLight, Color.red);
            CopyLightProperties(_mainLight, _greenLight, Color.green);
            CopyLightProperties(_mainLight, _blueLight, Color.blue);

            CopyHDLightData(_mainLight, _redLight, _redLightHD);
            CopyHDLightData(_mainLight, _greenLight, _greenLightHD);
            CopyHDLightData(_mainLight, _blueLight, _blueLightHD);

            float fringing = GetAutoColorFringing(_mainLight.spotAngle);

            _redLight.spotAngle = Mathf.Clamp(_mainLight.spotAngle + fringing, 1, 160);
            _greenLight.spotAngle = Mathf.Clamp(_mainLight.spotAngle + fringing / 2, 1, 160 - fringing * 0.5f);
            _blueLight.spotAngle = Mathf.Clamp(_mainLight.spotAngle + fringing / 4, 1, 160 - fringing * 0.75f);
        }

        private float GetAutoColorFringing(float spotAngle)
        {
            const float minSpotAngle = 10f;
            const float maxSpotAngle = 120f;

            float t = Mathf.InverseLerp(minSpotAngle, maxSpotAngle, spotAngle);
            return Mathf.Lerp(0f, _colorFringing, t);
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
            _mainLightHD ??= source.GetComponent<HDAdditionalLightData>();
            if (_mainLightHD == null)
                return;

            hdData ??= target.GetComponent<HDAdditionalLightData>();

            // Basic properties
            hdData.innerSpotPercent = _mainLightHD.innerSpotPercent;

            // Shadows
            hdData.EnableShadows(true);
            hdData.shadowUpdateMode = _mainLightHD.shadowUpdateMode;
            hdData.SetShadowResolution(GetResolutionFromIndex(_mainLightHD.shadowResolution.level));
            hdData.shadowDimmer = _mainLightHD.shadowDimmer;
            hdData.volumetricDimmer = _mainLightHD.volumetricDimmer;
            hdData.shadowTint = _mainLightHD.shadowTint;
            hdData.shadowNearPlane = _mainLightHD.shadowNearPlane;

            // IES
            hdData.spotIESCutoffPercent = _mainLightHD.spotIESCutoffPercent;
            if (_mainLightHD.IESTexture != null)
                hdData.IESTexture = _mainLightHD.IESTexture;
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
