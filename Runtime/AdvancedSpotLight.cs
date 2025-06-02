using UnityEditor;
using UnityEngine;

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

        public void Awake()
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
            _blueLight = GetOrCreateChannelLight(BlueChannelName, Color.blue);
            _greenLight = GetOrCreateChannelLight(GreenChannelName, Color.green);
            _redLight = GetOrCreateChannelLight(RedChannelName, Color.red);
        }

        private Light GetOrCreateChannelLight(string name, Color color)
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
                go.hideFlags = HideFlags.HideInHierarchy;
            }

            var light = go.GetComponent<Light>();
            if (light == null)
                light = go.AddComponent<Light>();

            light.type = LightType.Spot;
            light.color = color;
            return light;
        }

        private void UpdateChannelLights()
        {
            if (_mainLight == null || _redLight == null || _greenLight == null || _blueLight == null)
                return;

            CopyLightProperties(_mainLight, _redLight, Color.red);
            CopyLightProperties(_mainLight, _greenLight, Color.green);
            CopyLightProperties(_mainLight, _blueLight, Color.blue);

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
            target.intensity = source.intensity;
            target.range = source.range;
            target.innerSpotAngle = source.innerSpotAngle;
            target.cookie = source.cookie;
            target.cookieSize = source.cookieSize;

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
    }
}
