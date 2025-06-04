# 🌌 Spatial Demo

This is a Unity 6.0-based interactive spatial puzzle demo featuring responsive character movement, real-time shader effects, and immersive gameplay progression.

---

## 🛠️ Project Details

- **Unity Version**: `6.0.0 (6000.0.48f1)`
- **Input System**: [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html)
- **Character Controller**: [Kinematic Character Controller](https://assetstore.unity.com/packages/tools/physics/kinematic-character-controller-99131)
- **Visual Effects**:
  - **Dissolve Shader**: Implemented using **Shader Graph**
  - **Post-Processing**: Includes effects like **Color Grading**

---

## 🕹️ Gameplay

### 🔰 Zone 1: Tutorial

- You start in a tutorial area.
- A text UI explains your task: **throw the green block at the green wall**.
- If you miss and the block flies out of the playable area, a new block is automatically spawned.
- When you successfully hit the target:
  - ✅ You gain **+1 point**
  - 🚪 The door opens, allowing you to enter the next zone.
  - 
<img width="1437" alt="Screenshot 2025-06-04 at 4 16 17 AM" src="https://github.com/user-attachments/assets/6752e573-e08d-412e-b7ed-d8d6d60384fd" />

### 🎯 Zone 2: Color Match Challenge

- You now have **red, green, and blue blocks**.
- Throw blocks at walls of the **same color** to score points.
  - ✅ Matching color: **+1 point**
  - ❌ Wrong color: the block is **dissolved** using a shader effect (no point awarded)
- Continue matching to progress or practice coordination.
  
<img width="1432" alt="Screenshot 2025-06-04 at 4 17 17 AM" src="https://github.com/user-attachments/assets/035b2c70-1983-41a4-9294-d29119bd4e50" />
