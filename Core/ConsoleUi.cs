using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBypass.Core
{
    public static class ConsoleUi
    {
        public static void SetupConsole()
        {
            try
            {
                Console.Title = Strings.AppTitle;
                Console.OutputEncoding = System.Text.Encoding.UTF8;
            }
            catch { }
        }

        public static void DrawBanner()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
 ╔═══════════════════════════════════════════════════════════════════════════╗
 ║  ██████╗  ██████╗    ██████╗ ██╗   ██╗██████╗  █████╗ ███████╗███████╗    ║
 ║  ██╔══██╗██╔════╝    ██╔══██╗╚██╗ ██╔╝██╔══██╗██╔══██╗██╔════╝██╔════╝    ║
 ║  ██║  ██║██║         ██████╔╝ ╚████╔╝ ██████╔╝███████║███████╗███████╗    ║
 ║  ██║  ██║██║         ██╔══██╗  ╚██╔╝  ██╔═══╝ ██╔══██║╚════██║╚════██║    ║
 ║  ██████╔╝╚██████╗    ██████╔╝   ██║   ██║     ██║  ██║███████║███████║    ║
 ║  ╚═════╝  ╚═════╝    ╚═════╝    ╚═╝   ╚═╝     ╚═╝  ╚═╝╚══════╝╚══════╝    ║
 ║                      -- ZERO LAG DPI UNBLOCKER v1.0 --                   ║
 ╚═══════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        public static void DrawDashboard()
        {
            DrawBanner();

            bool isRunning = DpiEngine.Instance.IsRunning;
            var preset = DpiEngine.Instance.ActivePreset;
            bool autoStart = StartupManager.IsAutoStartEnabled();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" ┌───────────────────────── {Strings.StatusPanelTitle} ──────────────────────────────────┐");
            Console.ResetColor();

            // Status Line
            Console.Write(" │ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Strings.BypassStatusLabel);
            if (isRunning)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Strings.StatusActive.PadRight(44));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Strings.StatusPassive.PadRight(44));
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("│");

            // Preset Line
            Console.Write(" │ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Strings.ActivePresetLabel);
            Console.ForegroundColor = ConsoleColor.Yellow;
            string presetText = $"[{preset.Id}] {preset.Name}";
            if (presetText.Length > 44) presetText = presetText.Substring(0, 41) + "...";
            Console.Write(presetText.PadRight(44));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("│");

            // Auto-start Line
            Console.Write(" │ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Strings.AutoStartLabel);
            if (autoStart)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Strings.AutoStartOn.PadRight(44));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(Strings.AutoStartOff.PadRight(44));
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("│");

            // Uptime Line
            if (isRunning && DpiEngine.Instance.StartedAt.HasValue)
            {
                var uptime = DateTime.Now - DpiEngine.Instance.StartedAt.Value;
                Console.Write(" │ ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(Strings.UptimeLabel);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{uptime.Hours:D2}:{uptime.Minutes:D2}:{uptime.Seconds:D2}".PadRight(44));
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("│");
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(" └──────────────────────────────────────────────────────────────────────────┘");
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  {Strings.SelectActionPrompt}");
            Console.ResetColor();

            PrintMenuItem("1", isRunning ? Strings.MenuStopBypass : Strings.MenuStartBypass, isRunning ? ConsoleColor.Red : ConsoleColor.Green);
            PrintMenuItem("2", Strings.MenuChangePreset, ConsoleColor.Yellow);
            PrintMenuItem("3", Strings.MenuPingTest, ConsoleColor.Cyan);
            PrintMenuItem("4", Strings.MenuMinimizeTray, ConsoleColor.Magenta);
            PrintMenuItem("5", Strings.MenuLaunchDiscord, ConsoleColor.Blue);
            PrintMenuItem("6", Strings.MenuFixGreyScreen, ConsoleColor.Green);
            PrintMenuItem("7", Strings.MenuAutoStartToggle(autoStart), ConsoleColor.White);
            PrintMenuItem("8", Strings.MenuServiceManager, ConsoleColor.DarkYellow);
            PrintMenuItem("9", Strings.MenuToggleLanguage, ConsoleColor.DarkCyan);
            PrintMenuItem("0", Strings.MenuExit, ConsoleColor.DarkRed);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  {Strings.YourChoice} [0-9]: ");
            Console.ResetColor();
        }

        private static void PrintMenuItem(string key, string text, ConsoleColor color)
        {
            Console.Write("   [");
            Console.ForegroundColor = color;
            Console.Write(key);
            Console.ResetColor();
            Console.Write($"] {text}\n");
        }

        public static void ShowPresetsMenu()
        {
            DrawBanner();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" {Strings.PresetsHeader}\n");
            Console.ResetColor();

            int currentId = PresetManager.Config.SelectedPresetId;

            foreach (var preset in PresetManager.Presets)
            {
                bool isSelected = preset.Id == currentId;
                Console.Write(isSelected ? "  👉 " : "     ");
                Console.ForegroundColor = isSelected ? ConsoleColor.Green : ConsoleColor.White;
                Console.Write($"[{preset.Id}] {preset.Name}");
                if (preset.IsRecommended)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($" {Strings.RecommendedBadge}");
                }
                Console.ResetColor();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"       {Strings.DescriptionLabel}: {preset.Description}");
                Console.WriteLine($"       {Strings.ParameterLabel}: {preset.Arguments}");
                Console.WriteLine();
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"   {Strings.CancelPrompt}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  {Strings.EnterPresetNumber}");
            Console.ResetColor();

            string? input = Console.ReadLine();
            if (int.TryParse(input, out int id) && id > 0 && id <= PresetManager.Presets.Count)
            {
                PresetManager.SetPreset(id);
                var newPreset = PresetManager.GetCurrentPreset();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  ✔ {Strings.PresetSetSuccess} ({newPreset.Name})");
                Console.ResetColor();

                if (DpiEngine.Instance.IsRunning)
                {
                    Console.WriteLine(Strings.IsTurkish ? "  Bypass yeni ayarlarla yeniden başlatılıyor..." : "  Restarting bypass with new settings...");
                    DpiEngine.Instance.Start(newPreset);
                }
                Thread.Sleep(1200);
            }
        }

        public static async Task RunPingTestAsync()
        {
            DrawBanner();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" {Strings.PingTestHeader}\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" {Strings.PingTestingWait}");
            Console.ResetColor();

            var results = await ConnectivityTester.TestAllEndpointsAsync();

            Console.WriteLine(" ┌──────────────────────────────────────┬─────────────┬──────────┬────────────────┐");
            Console.WriteLine($" │ {Strings.ColServerName.PadRight(36)} │ {Strings.ColStatus.PadRight(11)} │ {Strings.ColLatency.PadRight(8)} │ {Strings.ColHttpResponse.PadRight(14)} │");
            Console.WriteLine(" ├──────────────────────────────────────┼─────────────┼──────────┼────────────────┤");

            foreach (var r in results)
            {
                Console.Write(" │ ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(r.Name.PadRight(36));
                Console.ResetColor();
                Console.Write(" │ ");

                if (r.IsReachable)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Strings.StatusReachable.PadRight(11));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(Strings.StatusBlocked.PadRight(11));
                }
                Console.ResetColor();
                Console.Write(" │ ");

                Console.ForegroundColor = r.LatencyMs < 100 ? ConsoleColor.Green : (r.LatencyMs < 200 ? ConsoleColor.Yellow : ConsoleColor.Red);
                Console.Write($"{r.LatencyMs} ms".PadRight(8));
                Console.ResetColor();
                Console.Write(" │ ");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                string statusText = r.StatusCode > 0 ? $"HTTP {r.StatusCode}" : "Timeout";
                Console.Write(statusText.PadRight(14));
                Console.ResetColor();
                Console.WriteLine(" │");
            }

            Console.WriteLine(" └──────────────────────────────────────┴─────────────┴──────────┴────────────────┘");
            Console.WriteLine();

            bool anySuccess = results.Exists(r => r.IsReachable);
            if (anySuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {Strings.PingSuccessMsg}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  {Strings.PingFailMsg}");
            }
            Console.ResetColor();

            Console.WriteLine($"\n  {Strings.PressAnyKey}");
            Console.ReadKey(true);
        }

        public static void ShowServiceMenu()
        {
            DrawBanner();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($" {Strings.ServiceHeader}\n");
            Console.ResetColor();

            bool isInstalled = ServiceInstaller.IsServiceInstalled();
            bool isRunning = ServiceInstaller.IsServiceRunning();

            Console.WriteLine($"  {Strings.ServiceInstalledLabel}{(isInstalled ? Strings.YesText : Strings.NoText)}");
            Console.WriteLine($"  {Strings.ServiceRunningLabel}{(isRunning ? Strings.RunningText : Strings.StoppedText)}");
            Console.WriteLine();
            Console.WriteLine($"  {Strings.ServiceInstallOption}");
            Console.WriteLine($"  {Strings.ServiceUninstallOption}");
            Console.WriteLine($"  {Strings.ServiceStartOption}");
            Console.WriteLine($"  {Strings.ServiceStopOption}");
            Console.WriteLine($"  {Strings.CancelPrompt}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  {Strings.YourChoice}: ");
            Console.ResetColor();

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var res = ServiceInstaller.InstallService(PresetManager.GetCurrentPreset());
                    Console.WriteLine(res.Success ? (Strings.IsTurkish ? "\n  ✔ Servis başarıyla kuruldu ve başlatıldı!" : "\n  ✔ Service installed and started successfully!") : $"\n  ❌ Hata: {res.Output}");
                    Thread.Sleep(1500);
                    break;
                case "2":
                    var unRes = ServiceInstaller.UninstallService();
                    Console.WriteLine(unRes.Success ? (Strings.IsTurkish ? "\n  ✔ Servis sistemden kaldırıldı." : "\n  ✔ Service uninstalled.") : $"\n  ❌ Hata: {unRes.Output}");
                    Thread.Sleep(1500);
                    break;
                case "3":
                    var startRes = ServiceInstaller.StartService();
                    Console.WriteLine(startRes.Success ? (Strings.IsTurkish ? "\n  ✔ Servis başlatıldı." : "\n  ✔ Service started.") : $"\n  ❌ Hata: {startRes.Output}");
                    Thread.Sleep(1500);
                    break;
                case "4":
                    var stopRes = ServiceInstaller.StopService();
                    Console.WriteLine(stopRes.Success ? (Strings.IsTurkish ? "\n  ✔ Servis durduruldu." : "\n  ✔ Service stopped.") : $"\n  ❌ Hata: {stopRes.Output}");
                    Thread.Sleep(1500);
                    break;
            }
        }
    }
}
