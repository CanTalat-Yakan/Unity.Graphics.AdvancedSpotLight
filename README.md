# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Advanced Spot Light

> Quick overview: HDRP composite spot light that splits a main light into tinted RGB channel lights to simulate chromatic aberration. Adjust fringing (spot angle) and shifting (orientation) and mirror most HDRP properties (shadows, IES, cookies, layers, resolution).

A lightweight helper that keeps three color‑separated channel lights (R, G, B) in sync with a main spot light, applying subtle color fringing and directional shifting to achieve a stylized or optical‑aberration look. It mirrors core Light settings and many `HDAdditionalLightData` properties, including IES, cookies, and shadow configuration.

![screenshot](Documentation/Screenshot.png)

## Features
- RGB channel mirroring
  - Duplicates a main HDRP Spot Light into Red, Green, and Blue channel lights
  - Tints each channel: `color = (channelColor * main.color)`
- Fringing and shifting controls
  - `ColorFringing` [0..1]: increases channel spot angles (reduces or expands beam per channel)
  - `ColorShifting` [-1..1]: offsets channel orientations (horizontal/vertical) for subtle separation
- Property sync from main to channels
  - Core Light: color temperature, unit, intensity, range, innerSpotAngle, reflector, cookie (size), culling/rendering masks, bake type (Editor)
  - Shadows: enablement, strength, resolution, bias, near plane, normal bias (Light)
  - HDRP data: shadow light layer, shadow resolution mapping, dimmers, near plane, depth/normal bias, volumetric dimmer, tint, fade distance, softness, sample counts, contact shadows, IES (texture and cutoff)
  - Inner spot percent clamped to sane max (≤ 95)
- Shadow resolution mapping
  - Maps main light shadow resolution level to an absolute resolution (512 → 8192)
- ExecuteAlways editor workflow
  - While selected in the Editor, channel lights update live as you tweak the main light

## Requirements
- Unity 6000.0+
- HDRP (uses `UnityEngine.Rendering.HighDefinition`)
- Four Light components in your rig:
  - Main (Spot) + three channel lights (Red, Green, Blue)
  - Each with `HDAdditionalLightData` (HDRP adds it automatically or add component)
- Editor‑time sync only by default
  - Updates happen in the Editor when the `AdvancedSpotLight` GameObject is selected
  - The script does not run in player builds as‑is (guarded by `#if UNITY_EDITOR` in Update)

## Usage

1) Create your lights
- Add a main HDRP Spot Light to a GameObject (this will host `AdvancedSpotLight`)
- Create three child Spot Lights under it for Red, Green, and Blue channels
- Ensure each light has `HDAdditionalLightData` (HDRP)

2) Add and wire the component
- Add `AdvancedSpotLight` to the main light’s GameObject
- In the inspector, assign references:
  - `MainLight` → your main Spot Light
  - `RedLight`, `GreenLight`, `BlueLight` → the channel lights
  - `MainLightData` → `HDAdditionalLightData` on the main
  - `RedLightData`, `GreenLightData`, `BlueLightData` → corresponding channel `HDAdditionalLightData`

3) Tweak look
- Adjust `ColorFringing` to widen/narrow channel beams (increases channel `spotAngle` relative to main)
- Adjust `ColorShifting` to offset channel directions (applies small local Euler rotation per channel)

4) Edit live
- Select the GameObject with `AdvancedSpotLight` in the Editor
- Modify main light properties (intensity, color, cookie, IES, shadows) → channels sync automatically

## Notes and Limitations
- Editor‑only sync: The code updates in the Unity Editor when the object is selected; it’s not active in player builds
- No auto‑instantiation: The script doesn’t create channel lights; set up and assign them manually (or via your prefab)
- Spot Light focus: Designed for spot lights; copying properties to other light types isn’t validated
- HDRP specifics: Property mirroring relies on HDRP APIs; behavior in URP/Builtin is unsupported
- Safe clamps: Inner spot percent is capped; fringing and shifting have sensible bounds

## Files in This Package
- `Runtime/AdvancedSpotLight.cs` – Core implementation (RGB channel sync, fringing/shifting, HDRP property mirroring)
- `Runtime/UnityEssentials.AdvancedSpotLight.asmdef` – Runtime assembly definition
- `package.json` – Package manifest metadata

## Tags
unity, hdrp, lighting, spotlight, rgb, chromatic-aberration, fringing, shifting, ies, cookie, shadows
