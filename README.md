# XNHALE

XNHALE Cyberdelic Hackathon is an immersive experience controlled by your breath.




# Cyberdelic Hackathon Boilerplate

A Unity 6 project template for rapid prototyping of cyberdelic mixed reality experiences.  
It combines audio-reactive visuals, passthrough color grading, and real-time NDI, MIDI, and OSC input for immersive, multi-sensory installations.

Developed for our first **[Cyberdelic Hackathon](https://www.cyberdelic.nexus/cyberdelic-hackathon)** — a gathering for XR developers, artists, musicians and designers to co-create a mixed-reality platform inspired by synesthesia.

---

## Overview

Based on the **Synedelica** codebase by **John Desnoyers-Stewart**, this project showcases a simple yet expressive **audioreactive passthrough color grading** system designed for **Meta Quest (OpenXR + Meta XR SDK)**.

### Scenes
- **Main Scene** – Clean boilerplate setup for your own audio-reactive passthrough experiments.  
- **Synedelica** – Original Synedelica demo scene demonstrating multiple visuals and transitions.
- **Webcam Shader Scene** – Manipulation of direct webcam pixel data with custom shader (only monoscopic)

---

## Unity Version

Developed and tested with **Unity 6 (6000.2.8f1)**.  
Requires **Android Build Support**, **Vulkan**, and **Meta XR SDK** enabled.

---

## Setup Instructions

1. **Clone or download this repository.**  
2. Open the project with **Unity 6 (6000.2.8f1)** or newer.  
3. In **File → Build Settings**, set the **Platform** to **Meta Quest** and click **Switch Platform**.  
4. Open **Edit → Project Settings → XR Plug-in Management → Android**  
   - Enable **OpenXR** as the only loader.  
   - Disable **Oculus** if present.  
   - Tick **Initialize XR on Startup** to enable **Quest Link** and proper OpenXR initialization.  
5. Go to **Meta XR → Tools → Project Setup** and click **Fix All** for both **Windows** and **Oculus/Android** configurations.  
   This ensures all required permissions, graphics APIs, and manifest entries are correctly set up.  
6. Connect your **Meta Quest** headset via Link or USB.  
7. Build and run the project on device.  
8. Use the **Main Scene** as a clean starting point for your own audio-reactive passthrough experiments.

**Note:**  
The `Audio Source` used for analysis must include a **Microphone Input** component.  
Set it to capture real-time audio input with the appropriate sample rate (typically `48000` Hz).  

---

## Passthrough Prefab

All essential systems are included in the **Passthrough prefab**.

The prefab contains the full passthrough pipeline used in Synedelica, including:
- **PassthroughControl** – Main script handling audio-reactive color grading and passthrough blending.  
- **TriggerBand** – Handles band-specific activation and threshold events for audio reactivity.  
- **OutlineAmplitude** – Drives outline and emission effects based on audio amplitude.  

The **Synedelica** scene demonstrates how these elements were combined, and how to switch between multiple visual setups.  

A **calibration system** is also included — originally developed before Meta’s built-in calibration tools.  
You can use it for aligning the passthrough view or even calibrating **two headsets in the same space** (though this version has no networking).

---

## Configuration Notes

When using the **PassthroughControl** script:
- Focus on the parameters **before “Passthrough Layer”**.  
- Adjust:
  - **Spectral Gradients**
  - **Band Colors**
  - **Audio Source**

These define the live color-reactive behavior.  
The remaining parameters relate to an older “scrolling gradient” mode, which currently only works in the **Editor** (using the **Sample Playback** object with a video URL).  
That feature is not functional on the headset yet but produces impressive effects if restored.

---

## Included Dependencies

This boilerplate integrates the following open-source repositories by [Keijiro Takahashi](https://github.com/keijiro):

- [**KlakNDI**](https://github.com/keijiro/KlakNDI) – NDI plugin for Unity (real-time network video IO)  
- [**Minis**](https://github.com/keijiro/Minis) – MIDI input extension for Unity  
- [**OscJack**](https://github.com/keijiro/OscJack) – Lightweight OSC communication library  

---

## License

This project is released under the **MIT License**.  
You are free to use, modify, and distribute it in your own projects.  
Attribution to the original authors is appreciated.

---

Between sound and light, between code and consciousness —  
we hack the infinite.

