using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DiscordBypass.Core
{
    public class BypassPreset
    {
        public int Id { get; set; }
        public string NameTr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionTr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        public bool IsRecommended { get; set; }

        public string Name => Strings.IsTurkish ? NameTr : NameEn;
        public string Description => Strings.IsTurkish ? DescriptionTr : DescriptionEn;
    }

    public class AppConfig
    {
        public int SelectedPresetId { get; set; } = 1;
        public bool AutoStartBypassOnLaunch { get; set; } = true;
        public bool StartMinimizedToTray { get; set; } = false;
        public bool AutoStartWithWindows { get; set; } = false;
        public string Language { get; set; } = "TR";
    }

    public static class PresetManager
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bypass_config.json");

        public static List<BypassPreset> Presets { get; } = new List<BypassPreset>
        {
            new BypassPreset
            {
                Id = 1,
                NameTr = "Standart TR (Türk Telekom, TurkNet, Kablonet, Vodafone)",
                NameEn = "Standard TR (Turk Telekom, TurkNet, Kablonet, Vodafone)",
                DescriptionTr = "TTL=5 ve Güvenli Yandex DNS. Çoğu kullanıcı için idealdir.",
                DescriptionEn = "TTL=5 and Secure Yandex DNS redirection. Ideal for most users.",
                Arguments = "-5 --set-ttl 5 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253",
                IsRecommended = true
            },
            new BypassPreset
            {
                Id = 2,
                NameTr = "Superonline Mod 4 (Önerilen SOL)",
                NameEn = "Superonline Mode 4 (Recommended for SOL)",
                DescriptionTr = "Superonline fiber kullanıcıları için en başarılı mod.",
                DescriptionEn = "Most effective mode for Superonline fiber internet users.",
                Arguments = "-5 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253",
                IsRecommended = true
            },
            new BypassPreset
            {
                Id = 3,
                NameTr = "Superonline Mod 1 (TTL 3)",
                NameEn = "Superonline Mode 1 (TTL 3)",
                DescriptionTr = "Superonline için saf TTL=3 manipülasyonu.",
                DescriptionEn = "Pure TTL=3 packet manipulation for Superonline.",
                Arguments = "--set-ttl 3",
                IsRecommended = false
            },
            new BypassPreset
            {
                Id = 4,
                NameTr = "Superonline Mod 2 (Standart -5)",
                NameEn = "Superonline Mode 2 (Standard -5)",
                DescriptionTr = "Saf -5 DPI bypass modu.",
                DescriptionEn = "Pure -5 DPI fragmentation bypass mode.",
                Arguments = "-5",
                IsRecommended = false
            },
            new BypassPreset
            {
                Id = 5,
                NameTr = "Superonline Mod 3 (TTL 3 + Güvenli DNS)",
                NameEn = "Superonline Mode 3 (TTL 3 + Secure DNS)",
                DescriptionTr = "Superonline için hem TTL=3 hem güvenli DNS.",
                DescriptionEn = "Combines TTL=3 and secure DNS redirection for Superonline.",
                Arguments = "--set-ttl 3 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253",
                IsRecommended = false
            },
            new BypassPreset
            {
                Id = 6,
                NameTr = "Agresif / Maksimum Bypass (-9 Modu)",
                NameEn = "Aggressive / Maximum Bypass (-9 Mode)",
                DescriptionTr = "Zorlu DPI filtreleri için derin paket parçalama ve fake HTTP request.",
                DescriptionEn = "Deep packet fragmentation and fake HTTP request injection.",
                Arguments = "-9 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253",
                IsRecommended = false
            },
            new BypassPreset
            {
                Id = 7,
                NameTr = "Cloudflare Güvenli DNS (1.1.1.1 + TTL 5)",
                NameEn = "Cloudflare Secure DNS (1.1.1.1 + TTL 5)",
                DescriptionTr = "Cloudflare DNS ile entegre standart DPI bypass.",
                DescriptionEn = "Standard DPI bypass integrated with Cloudflare DNS.",
                Arguments = "-5 --set-ttl 5 --dns-addr 1.1.1.1 --dns-port 53",
                IsRecommended = false
            },
            new BypassPreset
            {
                Id = 8,
                NameTr = "Saf Paket Parçalama (DNS Yönlendirmesiz Hızlı Mod)",
                NameEn = "Direct Packet Splitting (Fast / No DNS Redirection)",
                DescriptionTr = "DNS yönlendirmesi yapmaz, sadece DPI paketlerini parçalar.",
                DescriptionEn = "Splits DPI packets without redirecting DNS queries.",
                Arguments = "-5 -e 1 -q --reverse-frag --set-ttl 5",
                IsRecommended = false
            }
        };

        public static AppConfig Config { get; private set; } = new AppConfig();

        static PresetManager()
        {
            LoadConfig();
        }

        public static BypassPreset GetCurrentPreset()
        {
            var preset = Presets.Find(p => p.Id == Config.SelectedPresetId);
            return preset ?? Presets[0];
        }

        public static void SetPreset(int id)
        {
            if (Presets.Exists(p => p.Id == id))
            {
                Config.SelectedPresetId = id;
                SaveConfig();
            }
        }

        public static void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    var loaded = JsonSerializer.Deserialize<AppConfig>(json);
                    if (loaded != null)
                    {
                        Config = loaded;
                        Strings.SetLanguage(Config.Language);
                    }
                }
            }
            catch
            {
                Config = new AppConfig();
            }
        }

        public static void SaveConfig()
        {
            try
            {
                string json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
            }
            catch
            {
                // Ignore config write failures
            }
        }
    }
}
