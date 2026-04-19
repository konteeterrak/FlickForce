📝 README.md สำหรับ GitHub
markdown# 🎮 Horror FPS Tower Defense Game

เกม FPS แนว Horror ที่ผู้เล่นต้องขึ้นหอคอยและต่อสู้กับศัตรูที่โผล่มาจากกำแพง พัฒนาด้วย Unity

![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## 📋 สารบัญ

- [ฟีเจอร์หลัก](#-ฟีเจอร์หลัก)
- [ระบบในเกม](#-ระบบในเกม)
- [การติดตั้ง](#-การติดตั้ง)
- [วิธีเล่น](#-วิธีเล่น)
- [โครงสร้างโปรเจกต์](#-โครงสร้างโปรเจกต์)
- [Scripts หลัก](#-scripts-หลัก)
- [การปรับแต่ง](#-การปรับแต่ง)
- [ปัญหาที่พบบ่อย](#-ปัญหาที่พบบ่อย)
- [เครดิต](#-เครดิต)

---

## ✨ ฟีเจอร์หลัก

### 🔫 ระบบยิงปืน
- ยิงแบบ First Person Shooter
- Raycast + Physical Bullets
- ระบบรีโหลดกระสุน
- Recoil และ Spread
- Aim Down Sight (ADS)
- Crosshair แบบไดนามิก

### 👻 ระบบ Horror
- Head Bob (โยกหัวตอนเดิน)
- Camera Sway (กล้องมีน้ำหนัก)
- Breathing Effect (หายใจ)
- Fear System (ระบบความกลัว)
- Flashlight (ไฟฉาย)
- Light Flickering (ไฟกระพริบ)
- Heartbeat Sound (เสียงหัวใจเต้น)
- Fog Effect (หมอก)

### 🤖 AI ศัตรู
- Enemy AI ตามไล่ผู้เล่น
- Spawn จากกำแพง
- Wave System (ยากขึ้นเรื่อยๆ)
- Knockback Effect
- Health System

### 💪 ระบบผู้เล่น
- Health Bar UI
- Fall Damage (ดาเมจตกจากที่สูง)
- Knockback System
- Slide Mechanic (สไลด์)
- Crouch System (ย่อ)
- Death Screen

### 🎨 UI & Menu
- Main Menu
- Options Menu (Audio, Graphics)
- Death Panel
- Health Display

---

## 🎯 ระบบในเกม

### Gameplay Loop
1. ผู้เล่นเริ่มที่ชั้นล่างของหอคอย
2. ศัตรูโผล่มาจากกำแพงรอบๆ บันได
3. ยิงศัตรูพร้อมขึ้นบันได
4. ทุก Wave ศัตรูเพิ่มขึ้นและแรงขึ้น
5. เป้าหมาย: ขึ้นถึงยอดหอคอยให้ได้

### การควบคุม

| ปุ่ม | การทำงาน |
|------|----------|
| **WASD** | เคลื่อนที่ |
| **Mouse** | มองรอบตัว |
| **คลิกซ้าย** | ยิง |
| **คลิกขวา (ค้าง)** | เล็ง (ADS) |
| **R** | รีโหลด |
| **C** | สไลด์/ย่อ |
| **F** | เปิด/ปิดไฟฉาย |
| **Shift** | วิ่ง |
| **Space** | กระโดด |
| **ESC** | กลับ Menu |

---

## 🚀 การติดตั้ง

### ความต้องการระบบ
- Unity 2021.3 หรือสูงกว่า
- TextMeshPro Package
- OS: Windows / macOS / Linux

### ขั้นตอนการติดตั้ง

1. **Clone โปรเจกต์**
```bash
git clone https://github.com/konteeterrak/FlickForce
cd horror-fps-tower
```

2. **เปิดด้วย Unity Hub**
Unity Hub > Add > เลือกโฟลเดอร์โปรเจกต์

3. **Import TextMeshPro**
Window > TextMeshPro > Import TMP Essential Resources

4. **เปิด Scene**
Assets/Scenes/MenuScene

5. **กด Play!** 🎮

---

## 📁 โครงสร้างโปรเจกต์
Assets/
├── Scenes/
│   ├── MenuScene.unity          # หน้า Menu
│   └── GameScene.unity          # เกมหลัก
├── Scripts/
│   ├── FPSGunSystem.cs          # ระบบปืน
│   ├── BulletBehavior.cs        # พฤติกรรมกระสุน
│   ├── PlayerHealth.cs          # เลือดผู้เล่น
│   ├── EnemyAI.cs               # AI ศัตรู
│   ├── EnemySpawner.cs          # Spawn ศัตรู
│   ├── SlideController.cs       # ระบบสไลด์
│   ├── HorrorCameraEffects.cs   # เอฟเฟกต์กล้อง
│   ├── HorrorAtmosphere.cs      # บรรยากาศ
│   └── MenuManager.cs           # จัดการ Menu
├── Prefabs/
│   ├── Bullet.prefab            # กระสุน
│   └── Enemy.prefab             # ศัตรู
├── Materials/
│   ├── EnemyMaterial.mat        # วัสดุศัตรู
│   └── BulletMaterial.mat       # วัสดุกระสุน
└── Audio/
├── GunshotSound.wav         # เสียงยิง
├── ReloadSound.wav          # เสียงรีโหลด
├── HeartbeatSound.wav       # เสียงหัวใจ
└── AmbientHorror.wav        # เสียงพื้นหลัง

---

## 📜 Scripts หลัก

### FPSGunSystem.cs
ระบบปืนหลัก - ยิง, รีโหลด, ADS, Recoil
```csharp
public float bulletSpeed = 50f;
public int maxAmmo = 30;
public float reloadTime = 2f;
```

### PlayerHealth.cs
ระบบเลือด - รับดาเมจ, Fall Damage, Knockback
```csharp
public float maxHealth = 100f;
public float fallDamageThreshold = 5f;
```

### EnemyAI.cs
AI ศัตรู - ตามไล่, โจมตี, โผล่จากกำแพง
```csharp
public float moveSpeed = 3f;
public float detectionRange = 10f;
public float attackRange = 2f;
```

### EnemySpawner.cs
Spawn ศัตรู - Wave System
```csharp
public float spawnInterval = 3f;
public int enemiesPerWave = 3;
```

### HorrorCameraEffects.cs
เอฟเฟกต์กล้องสยอง
```csharp
public float fearLevel = 0f; // 0-1
public bool enableHeadBob = true;
```

---

## ⚙️ การปรับแต่ง

### ปรับความยาก

**ง่าย:**
```csharp
// EnemySpawner.cs
enemiesPerWave = 2;
spawnInterval = 5f;

// EnemyAI.cs
damage = 5f;
moveSpeed = 2f;
```

**ยาก:**
```csharp
// EnemySpawner.cs
enemiesPerWave = 5;
spawnInterval = 2f;

// EnemyAI.cs
damage = 20f;
moveSpeed = 5f;
```

### ปรับความน่ากลัว

**น้อย:**
```csharp
// HorrorCameraEffects.cs
fearLevel = 0.2f;
enableHeadBob = false;

// HorrorAtmosphere.cs
fogDensity = 0.02f;
```

**มาก:**
```csharp
// HorrorCameraEffects.cs
fearLevel = 0.9f;
enableBreathing = true;

// HorrorAtmosphere.cs
fogDensity = 0.1f;
flickeringFlashlight = true;
```

---

## 🐛 ปัญหาที่พบบ่อย

### กระสุนไม่โดนศัตรู
**วิธีแก้:**
- ตรวจสอบว่า Bullet Prefab มี Box Collider และ Rigidbody
- เช็ค Tag ของ Enemy = "Enemy"
- เปลี่ยน Collision Detection เป็น Continuous

### ศัตรูกลิ้ง/ล้ม
**วิธีแก้:**
- ใช้ Character Controller แทน Rigidbody
- ตั้งค่า Freeze Rotation ใน Rigidbody

### ขึ้นบันไดติด
**วิธีแก้:**
- เพิ่ม Step Offset ใน Character Controller (0.5)
- ลด Skin Width (0.05)

### FOV กระตุก
**วิธีแก้:**
- ย้าย Lerp ไปใน LateUpdate()
- ลด Aim Speed

---

## 🎨 Asset Credits

### เสียง
- [Freesound.org](https://freesound.org)
- [Zapsplat.com](https://www.zapsplat.com)

### 3D Models
- Built-in Unity Primitives

### Scripts
- สร้างโดย GETNOZ

---

## 📄 License

MIT License - ใช้งานได้อย่างอิสระ

---

## 🤝 Contributing

ยินดีรับ Pull Requests!

1. Fork โปรเจกต์
2. สร้าง Branch (`git checkout -b feature/AmazingFeature`)
3. Commit (`git commit -m 'Add some AmazingFeature'`)
4. Push (`git push origin feature/AmazingFeature`)
5. เปิด Pull Request


## 🔮 Roadmap

- [ ] เพิ่มอาวุธใหม่
- [ ] Boss Fight
- [ ] Multiplayer Mode
- [ ] Save System
- [ ] Achievement System
- [ ] VR Support

---

**Made with ❤️ and Unity**

📂 ไฟล์เพิ่มเติมที่ควรมี
.gitignore
gitignore# Unity
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/

# Visual Studio cache
.vs/

# Rider
.idea/

# OS
.DS_Store
Thumbs.db

# Asset Store Tools
AssetStoreTools*

CONTRIBUTING.md
markdown# Contributing Guidelines

## การส่ง Pull Request

1. ทดสอบให้แน่ใจว่าไม่มี Error
2. เขียน Commit Message ให้ชัดเจน
3. อธิบายการเปลี่ยนแปลงใน PR

## Code Style

- ใช้ camelCase สำหรับตัวแปร
- ใช้ PascalCase สำหรับ Class/Method
- เพิ่ม Comment ในส่วนที่ซับซ้อน

LICENSE
MIT License

Copyright (c) 2024 GETNOZ

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
