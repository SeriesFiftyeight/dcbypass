# 🛡️ DC Bypass Pro - Zero-Lag DPI Unblocker

<p align="center">
  <a href="https://github.com/SeriesFiftyeight/dcbypass/releases/latest">
    <img src="https://img.shields.io/badge/DOWNLOAD-DC_Bypass_v1.0_(ZIP)-blue?style=for-the-badge&logo=windows&logoColor=white" alt="Download DC Bypass" />
  </a>
  <img src="https://img.shields.io/badge/Status-Active-brightgreen?style=for-the-badge" alt="Status Active" />
  <img src="https://img.shields.io/badge/Platform-Windows_10_%2F_11-informational?style=for-the-badge&logo=windows" alt="Windows" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="License MIT" />
</p>

---

### 📥 İndirme & Hızlı Başlangıç (Download & Quick Start)

> **Normal Kullanıcılar İçin (For End Users):**  
> Kodları derlemenize veya kaynak dosyalarla uğraşmanıza gerek yoktur!  
> 1. Sağdaki **[Releases (Sürümler)](https://github.com/SeriesFiftyeight/dcbypass/releases/latest)** bölümünden veya yukarıdaki **DOWNLOAD** butonundan **`dcbypass-v1.0.zip`** dosyasını indirin.
> 2. ZIP'i bir klasöre çıkartın.
> 3. **`START.bat`** (Konsol ile açar) veya **`START_TRAY.vbs`** (Doğrudan sağ alttaki simgeyle açar) dosyasına çift tıklayın!

---

## 🌟 Key Features / Öne Çıkan Özellikler

- 🚀 **0 Added Ping / Sıfır Gecikme:** Connects directly to Discord's original servers without routing traffic through foreign VPN proxies.
- 🌍 **Bilingual Support (English & Türkçe):** Press **`[9]`** or **`[L]`** in the console or right-click the tray icon to switch between English and Turkish instantly.
- 🎛️ **Dual Mode (Console CMD & System Tray):** Interactive Cyberpunk CLI with live dashboard + sleek background System Tray shield icon.
- 🌐 **All ISP Presets Supported (8 Specialized Modes):**
  - **Mode 1:** Standard TR (Turk Telekom, TurkNet, Kablonet, Vodafone)
  - **Mode 2 - 5:** Superonline (Specific TTL and DNS presets for SOL Fiber)
  - **Mode 6:** Aggressive / Maximum Bypass (-9 Mode)
  - **Mode 7:** Cloudflare Secure DNS (1.1.1.1 + TTL 5)
  - **Mode 8:** Direct Packet Splitting (Fast / No DNS redirection)
- 🧹 **Grey Screen & Stuck Loop Resolver:** Press **`[6]`** in the console to auto-terminate stuck Discord processes, wipe corrupted caches, flush DNS, and restart fresh.
- ⚡ **Live Latency & Health Check:** Real-time ping and HTTP status tester for Discord Web, Gateway, and CDN endpoints.
- ⚙️ **Windows Auto-Start:** Enable/disable starting silently on boot via Registry.
- 🛠️ **Windows Background Service:** Install/manage as a headless official Windows service.

---

## 📁 Package Contents / Paket İçeriği (İndirilen Dosya)

Kullanıcıların indirdiği ZIP dosyasında sadece çalıştırmaya hazır dosyalar yer alır:

```text
dcbypass/
├── dcbypass.exe        # Ana derlenmiş program (Compiled executable)
├── START.bat           # 1-Tıkla Konsol Başlatıcı (Console Launcher)
├── START_TRAY.vbs      # 1-Tıkla Tepside Başlatıcı (Silent Tray Launcher)
├── STOP_AND_CLEAR.bat  # Durdurucu ve Temizleyici (Stop & Clean)
├── README.txt          # Kullanım Kılavuzu (User Manual)
└── core_engine/        # 64-bit WinDivert sürücüleri (Drivers)
```

---

## 🚀 How to Run / Nasıl Kullanılır?

| Dosya | Açıklama |
| :--- | :--- |
| **`START.bat`** | Konsol ekranını açar. Menüden modu değiştirebilir, test yapabilir veya **`[4]`** ile sağ alta küçültebilirsiniz. |
| **`START_TRAY.vbs`** | Hiç pencere açmadan doğrudan sağ alttaki kalkan simgesiyle arka planda başlatır. |
| **`STOP_AND_CLEAR.bat`** | Bypass'ı durdurur, servisleri kapatır ve DNS önbelleğini temizler. |

---

## ⚖️ Credits & Licenses
- **GoodbyeDPI** by ValdikSS (Apache License 2.0)
- **WinDivert** (LGPLv3 License)
- **DC Bypass Pro** (MIT License - Copyright © 2026 SeriesFiftyeight)
