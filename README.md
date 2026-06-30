# 💊 PharmaSort VR: Clinical Precision Training ✨

Welcome to **PharmaSort VR**! This is an immersive virtual reality simulation designed for the Meta Quest and Windows Standalone VR platforms using Unity3D and the XR Interaction Toolkit. 

Intersecting the fields of medical informatics and VR design, this game tasks players with serving as a high-tech pharmacy technician under strict time constraints. Grab your headset, manage the queue, and sort physics-enabled prescription bottles to save the day (and make a profit)! 🩺🥽

---

## 🏥 Introduction
In **PharmaSort VR**, you must manage, identify, and sort randomized prescription bottles and pills based on incoming patient orders. The simulation emphasizes muscle memory, structural order processing, and spatial UI layout optimization dedicated to spatial computing.

---

## 🔄 Core Gameplay Loop
The game runs on a frantic, round-based time loop designed to simulate high-stress clinical pharmacy settings:

1. **⏰ Shift Initialization:** Start your shift with a countdown timer visible on your diegetic laboratory monitor.
<img width="689" height="450" alt="Picture2" src="https://github.com/user-attachments/assets/ea3b89b5-186b-49c9-a6d4-deb4c437fd21" />

   
2. **🚶 Dynamic Queue System:** Up to 3 randomized patient NPCs spawn at random intervals (10–30s) to form a line at the counter. *Ding!* A physical entrance bell sounds when they arrive.
<img width="601" height="355" alt="Picture4" src="https://github.com/user-attachments/assets/2deeff25-27b0-46da-a250-d4cae9fc123f" />


3. **📋 Prescription Fulfillment:** When a patient reaches the front, their prescription order appears on a sleek holographic spatial UI.

4. **🫴 Spatial Manipulation (XR Interaction):** Use your VR controllers to grab empty bottles from spawn points and fill them with the correct amount of 4 different pill types. Mix and match as needed!
<img width="647" height="286" alt="Picture5" src="https://github.com/user-attachments/assets/56a61929-c820-4df8-a561-e1785c2ed591" />


5. **✅ Submission & Validation:** Place the filled bottle onto the delivery socket and physically smash the low-poly Red Button!
   * **🟢 Correct Order:** *Ding!* You earn cash, trigger a light haptic buzz, and the patient leaves happily.
   * **🔴 Incorrect Order:** *Buzzer!* You receive a financial penalty for malpractice and feel a heavy haptic vibration.
<img width="689" height="450" alt="Picture2" src="https://github.com/user-attachments/assets/eb90f8a5-27b4-4087-93fb-87663e870269" />



6. **🗑️ Trash / Disposal:** Messed up? Toss ruined items into the physical trash can for a satisfying *clunk* sound effect.
<img width="482" height="358" alt="Picture9" src="https://github.com/user-attachments/assets/7e152e75-1d55-4498-a7e5-cc535ae4208f" />


7. **⏸️ Pause System:** Hit the pause button on your monitor to freeze time, halt AI movement, and cast a dark, semi-transparent focus overlay over your vision.
<img width="598" height="322" alt="Picture3" src="https://github.com/user-attachments/assets/81bf8e3f-7f0a-494c-bd15-e1d525330376" />


8. **📊 Shift Resolution:** When the timer hits zero, the shift ends. Remaining patients sigh and leave. Your monitor displays gross earnings, malpractice penalties, and net profit!
<img width="721" height="355" alt="Picture7" src="https://github.com/user-attachments/assets/0d0f0ae0-aca4-4baf-b595-a8aa8c4fb085" />


---

## 🎮 Control Scheme
Fully mapped for standard OpenXR-compliant VR controllers (e.g., Meta Quest Touch):
* **Left/Right Thumbstick:** Smooth locomotion and turning around the workspace.
* **Left/Right Grip Button (Hold):** Activates the `XR Grab Interactor`. Squeeze to pick up bottles, pills, and props. Release to drop or throw!
* **Left/Right Index Trigger (Press):** Interacts with world-space UI buttons via the `Tracked Device Graphic Raycaster`.
* **Controller Haptics:** Feel the success (or failure) of your orders with dynamic impulse vibrations.

---

## ⚙️ Game Systems Built in Unity

### 🎬 Scene Management
* **`01_MainMenu`:** A relaxing skybox environment. Features a world-space UI to select save slots (tracking your banked cash), reset slots to $0, or exit.
* **`02_MainGameplay`:** The core lab featuring procedural AI queues, complex item generation, dynamic score counters, and physics interactions.

<img width="702" height="450" alt="Picture1" src="https://github.com/user-attachments/assets/06d1beb0-04ad-486c-8bb5-9577888033ac" />


### 🧲 Physics & Economy System
* **XR Sockets:** Utilize layer masks to only accept specific items (like bottles).
* **Validation Formula:** Automatically calculates payouts based on base prices minus penalties for extra/wrong pills submitted.

### 💻 XR UI & Informatics
* **Diegetic Monitor UI:** A physical in-game computer displaying timers, pause options, and financial breakdowns.
* **Patient Hologram Order:** A dynamic world-space canvas that floats near the patient, showing visual pill icons and required quantities.

---

## 🎨 Asset Requirements & Sourcing
This project relies on optimized, stylized low-poly medical assets for smooth VR performance (72Hz+).

**Visual Assets:**
* **Characters:** Animated Women & Men Packs (Quaternius | CC0)
* **Props:** PBR Tablets/Capsules (VOiD1 Gaming), Red Button (matsoj), Coffee Cup (A-S-C-E-N-D), Trash Can (Oxygen).
* **Environment:** Medicine Chest Pharmacy Set (Tsunami)
* **UI:** Free Casual GUI (Sky Den Games)

**Audio Assets (Pixabay):**
* Pill shaking foley, Service bell rings, Error buzzers, Success dings, and Crowd sighs!

---

## 🚀 Future Updates & Improvements
* **🛒 Dynamic NPC Shopping:** Upgrading AI with pathfinding and Inverse Kinematics (IK) so patients can browse shelves and bring OTC products to the counter.
* **🧴 Physical Assembly:** Forcing players to physically grab and attach bottle caps, or use an ink stamper to approve orders.
* **📈 Difficulty Scaling:** Progressing to the "Next Day" will decrease spawn intervals, shorten shifts, and increase prescription complexity.
* **🛋️ Lobby Overhaul:** Replacing the Main Menu skybox with a fully enclosed, 3D medical waiting room.

---

## 📁 Project Directory Structure
```text
📦 PharmaSort VR
 ┣ 📂 Assets/XR_Config/ (Input maps, OpenXR pipeline)
 ┣ 📂 Assets/Audio/ (Spatial sound clips, bells, foley)
 ┣ 📂 Assets/Materials/ (URP materials, glass, neon holograms)
 ┣ 📂 Assets/Models/ (Lab tables, bottles, character models)
 ┣ 📂 Assets/Prefabs/ (XR Origin, grabbables, sorting triggers)
 ┣ 📂 Assets/Scenes/ (MainMenu.unity, MainGameplay.unity)
 ┣ 📂 Assets/Scripts/ (GameManager, PatientAIQueue, XRGrabbableItem)
 ┗ 📂 Assets/UI/ (Sprites, typography, medical panels)
