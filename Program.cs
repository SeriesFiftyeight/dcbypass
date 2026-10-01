using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordBypass.Core;

namespace DiscordBypass
{
    internal static class Program
    {
        private static Mutex? _mutex;
        private const string MutexName = "Global\\DCBypassPro_SingleInstance_Mutex";

        [STAThread]
        private static async Task Main(string[] args)
        {
            // Single instance check
            _mutex = new Mutex(true, MutexName, out bool isOnlyInstance);
            if (!isOnlyInstance)
            {
                // Relaunch or bring window to front
                WindowHelper.ShowConsole();
                Console.WriteLine(Strings.IsTurkish
                    ? "DC Bypass zaten çalışıyor! Mevcut pencere ön plana getirildi."
                    : "DC Bypass is already running! Bringing current window to front.");
                Thread.Sleep(1000);
                return;
            }

            ConsoleUi.SetupConsole();

            // Handle graceful shutdown
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Shutdown();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Shutdown();
                Environment.Exit(0);
            };

            // Initialize Tray Icon Manager
            TrayIconManager.Instance.Initialize();

            // Auto-start bypass engine on launch
            if (PresetManager.Config.AutoStartBypassOnLaunch)
            {
                DpiEngine.Instance.Start(PresetManager.GetCurrentPreset());
            }

            bool startInTray = args.Contains("--tray") || args.Contains("-t") || PresetManager.Config.StartMinimizedToTray;
            if (startInTray)
            {
                WindowHelper.HideConsole();
                TrayIconManager.Instance.ShowNotification(
                    "DC Bypass",
                    $"{Strings.StatusActive}\n{DpiEngine.Instance.ActivePreset.Name}",
                    ToolTipIcon.Info
                );
            }

            // Main Interactive CLI Loop
            while (true)
            {
                if (WindowHelper.IsConsoleVisible())
                {
                    ConsoleUi.DrawDashboard();
                    var key = Console.ReadKey(true).KeyChar;

                    switch (char.ToLowerInvariant(key))
                    {
                        case '1':
                            if (DpiEngine.Instance.IsRunning)
                            {
                                DpiEngine.Instance.Stop();
                            }
                            else
                            {
                                DpiEngine.Instance.Start();
                            }
                            break;

                        case '2':
                            ConsoleUi.ShowPresetsMenu();
                            break;

                        case '3':
                            await ConsoleUi.RunPingTestAsync();
                            break;

                        case '4':
                            WindowHelper.HideConsole();
                            TrayIconManager.Instance.ShowNotification(
                                "DC Bypass",
                                Strings.IsTurkish
                                    ? "Sistem tepsisine küçültüldü. Sağ alttaki simgeye çift tıklayarak tekrar açabilirsiniz."
                                    : "Minimized to system tray. Double click the tray icon to reopen.",
                                ToolTipIcon.Info
                            );
                            break;

                        case '5':
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(Strings.IsTurkish
                                ? "\n  DNS önbelleği temizleniyor ve Discord başlatılıyor..."
                                : "\n  Flushing DNS cache and starting Discord...");
                            Console.ResetColor();
                            StartupManager.FlushDns();
                            StartupManager.LaunchDiscord();
                            Thread.Sleep(1500);
                            break;

                        case '6':
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            if (Strings.IsTurkish)
                            {
                                Console.WriteLine("\n  [1/4] Açık Discord süreçleri tamamen kapatılıyor...");
                                Console.WriteLine("  [2/4] Bozuk web ve GPU önbellek dosyaları temizleniyor...");
                                Console.WriteLine("  [3/4] DNS tablosu sıfırlanıyor (flushdns)...");
                                Console.WriteLine("  [4/4] Bypass motoru tazeleniyor ve Discord sıfırdan başlatılıyor...");
                            }
                            else
                            {
                                Console.WriteLine("\n  [1/4] Terminating all running Discord processes...");
                                Console.WriteLine("  [2/4] Clearing corrupted web and GPU caches...");
                                Console.WriteLine("  [3/4] Flushing DNS resolver cache...");
                                Console.WriteLine("  [4/4] Refreshing bypass engine and restarting Discord fresh...");
                            }
                            Console.ResetColor();
                            StartupManager.FixGreyScreenAndRestart();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Strings.IsTurkish
                                ? "  ✔ İşlem tamamlandı! Discord temiz şekilde açılıyor."
                                : "  ✔ Operation completed! Discord launched with clean state.");
                            Console.ResetColor();
                            Thread.Sleep(2000);
                            break;

                        case '7':
                            bool currentAuto = StartupManager.IsAutoStartEnabled();
                            StartupManager.SetAutoStart(!currentAuto);
                            break;

                        case '8':
                            ConsoleUi.ShowServiceMenu();
                            break;

                        case '9':
                        case 'l':
                        case 't':
                            Strings.ToggleLanguage();
                            TrayIconManager.Instance.RefreshLocalization();
                            ConsoleUi.SetupConsole();
                            break;

                        case '0':
                            Shutdown();
                            return;
                    }
                }
                else
                {
                    // While console is hidden, sleep to conserve CPU
                    Thread.Sleep(500);
                }
            }
        }

        private static void Shutdown()
        {
            try
            {
                DpiEngine.Instance.Stop();
                TrayIconManager.Instance.Dispose();
                _mutex?.ReleaseMutex();
                _mutex?.Dispose();
            }
            catch { }
        }
    }
}
