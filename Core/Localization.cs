using System;

namespace DiscordBypass.Core
{
    public enum AppLanguage
    {
        TR,
        EN
    }

    public static class Strings
    {
        public static AppLanguage CurrentLanguage { get; set; } = AppLanguage.TR;

        public static bool IsTurkish => CurrentLanguage == AppLanguage.TR;

        public static void ToggleLanguage()
        {
            CurrentLanguage = CurrentLanguage == AppLanguage.TR ? AppLanguage.EN : AppLanguage.TR;
            PresetManager.Config.Language = CurrentLanguage.ToString();
            PresetManager.SaveConfig();
        }

        public static void SetLanguage(string lang)
        {
            if (Enum.TryParse<AppLanguage>(lang, true, out var parsed))
            {
                CurrentLanguage = parsed;
            }
        }

        // Dashboard & UI
        public static string AppTitle => "DC Bypass Pro v1.0 - Zero Lag DPI Unblocker";
        public static string AppTagline => "-- ZERO LAG DPI UNBLOCKER v1.0 --";
        public static string StatusPanelTitle => IsTurkish ? "DURUM PANELİ" : "STATUS DASHBOARD";
        public static string BypassStatusLabel => IsTurkish ? "Bypass Durumu     : " : "Bypass Status     : ";
        public static string StatusActive => IsTurkish ? "● AKTİF (Discord Açık & 0ms Ek Gecikme)" : "● ACTIVE (Discord Unblocked & 0ms Added Lag)";
        public static string StatusPassive => IsTurkish ? "○ PASİF (Durduruldu)" : "○ INACTIVE (Stopped)";
        public static string ActivePresetLabel => IsTurkish ? "Aktif Mod / İSS   : " : "Active Preset/ISP : ";
        public static string AutoStartLabel => IsTurkish ? "Windows Başlangıcı: " : "Windows Auto-Start: ";
        public static string AutoStartOn => IsTurkish ? "AÇIK (Bilgisayar açılınca otomatik başlar)" : "ENABLED (Starts automatically on boot)";
        public static string AutoStartOff => IsTurkish ? "KAPALI" : "DISABLED";
        public static string UptimeLabel => IsTurkish ? "Çalışma Süresi    : " : "Uptime            : ";
        public static string SelectActionPrompt => IsTurkish ? "LÜTFEN BİR İŞLEM SEÇİN:" : "PLEASE SELECT AN ACTION:";
        public static string YourChoice => IsTurkish ? "Seçiminiz" : "Your choice";

        // Menu Items
        public static string MenuStartBypass => IsTurkish ? "▶️  Bypass'ı BAŞLAT" : "▶️  START Bypass";
        public static string MenuStopBypass => IsTurkish ? "⏹️  Bypass'ı DURDUR" : "⏹️  STOP Bypass";
        public static string MenuChangePreset => IsTurkish ? "🌐 Mod & İSS Değiştir (Superonline, TT, Kablonet...)" : "🌐 Change Mode & ISP (Superonline, TT, Direct...)";
        public static string MenuPingTest => IsTurkish ? "⚡ Discord Bağlantı ve Ping Testi Yap (Canlı Ölçüm)" : "⚡ Test Discord Ping & Connectivity (Live Check)";
        public static string MenuMinimizeTray => IsTurkish ? "📥 Sağ Alt Sistem Tepsisine (Tray) Küçült" : "📥 Minimize to System Tray";
        public static string MenuLaunchDiscord => IsTurkish ? "🚀 Discord'u Aç (Normal Başlat)" : "🚀 Launch Discord (Normal Start)";
        public static string MenuFixGreyScreen => IsTurkish ? "🧹 GRİLEŞME & TAKILMA ÇÖZÜCÜ (Önbelleği Temizle & Yeniden Başlat)" : "🧹 FIX GREY SCREEN & STUCK LOOP (Clear Cache & Restart)";
        public static string MenuAutoStartToggle(bool current) => current
            ? (IsTurkish ? "⚙️  Windows ile Başlatmayı KAPAT" : "⚙️  DISABLE Windows Auto-Start")
            : (IsTurkish ? "⚙️  Windows ile Başlatmayı AÇ" : "⚙️  ENABLE Windows Auto-Start");
        public static string MenuServiceManager => IsTurkish ? "🛠️  Windows Arka Plan Servis Yönetimi (Service Mode)" : "🛠️  Windows Background Service Manager";
        public static string MenuToggleLanguage => IsTurkish ? "🌍 Dil Değiştir / Switch to ENGLISH [EN]" : "🌍 Switch Language / TÜRKÇE'ye Geç [TR]";
        public static string MenuExit => IsTurkish ? "❌ Çıkış" : "❌ Exit";

        // Presets Menu
        public static string PresetsHeader => IsTurkish ? "═══ MOD VE İNTERNET SERVİS SAĞLAYICI (İSS) SEÇİMİ ═══" : "═══ PRESET & INTERNET SERVICE PROVIDER (ISP) SELECTION ═══";
        public static string RecommendedBadge => IsTurkish ? "★ [TAVSİYE EDİLEN]" : "★ [RECOMMENDED]";
        public static string DescriptionLabel => IsTurkish ? "Açıklama " : "Desc     ";
        public static string ParameterLabel => IsTurkish ? "Parametre" : "Params   ";
        public static string CancelPrompt => IsTurkish ? "[0] İptal ve Ana Menüye Dön" : "[0] Cancel and Return to Main Menu";
        public static string EnterPresetNumber => IsTurkish ? "Kullanmak istediğiniz mod numarasını girin: " : "Enter the preset number you want to use: ";
        public static string PresetSetSuccess => IsTurkish ? "Mod başarıyla ayarlandı!" : "Preset configured successfully!";

        // Ping Test
        public static string PingTestHeader => IsTurkish ? "═══ DİSCORD BAĞLANTI & PİNG TESTİ ═══" : "═══ DISCORD CONNECTIVITY & PING TEST ═══";
        public static string PingTestingWait => IsTurkish ? "Discord sunucuları test ediliyor, lütfen bekleyin...\n" : "Testing Discord servers, please wait...\n";
        public static string ColServerName => IsTurkish ? "Hedef Sunucu Adı" : "Target Server Name";
        public static string ColStatus => IsTurkish ? "Durum" : "Status";
        public static string ColLatency => IsTurkish ? "Gecikme" : "Latency";
        public static string ColHttpResponse => IsTurkish ? "HTTP Yanıtı" : "HTTP Response";
        public static string StatusReachable => IsTurkish ? "ERİŞİLEBİLİR" : "REACHABLE";
        public static string StatusBlocked => IsTurkish ? "ENGELLİ/HATA" : "BLOCKED/ERR";
        public static string PingSuccessMsg => IsTurkish ? "🎉 HARİKA! Discord sunucularına doğrudan 0 ms ek gecikmeyle erişim sağlanabiliyor." : "🎉 GREAT! Discord servers are reachable directly with 0 ms added latency.";
        public static string PingFailMsg => IsTurkish ? "⚠️ Discord'a ulaşılamadı. 'Mod Değiştir' menüsünden başka bir İSS modu (özellikle Superonline modları) seçmeyi deneyin." : "⚠️ Could not reach Discord. Try selecting another ISP preset (especially Superonline modes).";
        public static string PressAnyKey => IsTurkish ? "Ana menüye dönmek için herhangi bir tuşa basın..." : "Press any key to return to main menu...";

        // Service Menu
        public static string ServiceHeader => IsTurkish ? "═══ WINDOWS ARKA PLAN SERVİS YÖNETİMİ ═══" : "═══ WINDOWS BACKGROUND SERVICE MANAGER ═══";
        public static string ServiceInstalledLabel => IsTurkish ? "Servis Kurulu mu?  : " : "Service Installed? : ";
        public static string ServiceRunningLabel => IsTurkish ? "Servis Durumu      : " : "Service Status     : ";
        public static string YesText => IsTurkish ? "EVET (Sistemde Kayıtlı)" : "YES (Registered)";
        public static string NoText => IsTurkish ? "HAYIR" : "NO";
        public static string RunningText => IsTurkish ? "ÇALIŞIYOR (Arka Planda Aktif)" : "RUNNING (Active in Background)";
        public static string StoppedText => IsTurkish ? "DURDURULDU" : "STOPPED";
        public static string ServiceInstallOption => IsTurkish ? "[1] Arka Plan Servisi Olarak Kur ve Başlat (Konsolsuz çalışır)" : "[1] Install & Start as Windows Background Service (Headless)";
        public static string ServiceUninstallOption => IsTurkish ? "[2] Arka Plan Servisini Kaldır" : "[2] Uninstall Background Service";
        public static string ServiceStartOption => IsTurkish ? "[3] Servisi Başlat" : "[3] Start Service";
        public static string ServiceStopOption => IsTurkish ? "[4] Servisi Durdur" : "[4] Stop Service";

        // Notifications & Tray
        public static string TrayTitle => "DC Bypass Pro";
        public static string TrayStatusActive => IsTurkish ? "Durum: 🟢 AKTİF (Discord Açık)" : "Status: 🟢 ACTIVE (Discord Unblocked)";
        public static string TrayStatusPassive => IsTurkish ? "Durum: 🔴 PASİF (Durduruldu)" : "Status: 🔴 INACTIVE (Stopped)";
        public static string TrayShowConsole => IsTurkish ? "📊 Konsol Penceresini Göster" : "📊 Show Console Window";
        public static string TrayHideConsole => IsTurkish ? "📊 Konsol Penceresini Gizle" : "📊 Hide Console Window";
        public static string TrayPresetsMenu => IsTurkish ? "🌐 Mod / İSS Seçimi" : "🌐 Preset / ISP Selection";
        public static string TrayPingTest => IsTurkish ? "⚡ Discord Ping Testi Yap" : "⚡ Test Discord Ping";
        public static string TrayLaunchDiscord => IsTurkish ? "🚀 Discord'u Aç" : "🚀 Launch Discord";
        public static string TrayAutoStart => IsTurkish ? "⚙️ Windows ile Otomatik Başlat" : "⚙️ Start with Windows";
        public static string TrayLanguageMenu => IsTurkish ? "🌍 Dil: Türkçe (Switch to English)" : "🌍 Language: English (Türkçe'ye Geç)";
        public static string TrayExit => IsTurkish ? "❌ Tamamen Kapat ve Çık" : "❌ Exit DC Bypass";
    }
}
