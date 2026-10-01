using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace DiscordBypass.Core
{
    public static class StartupManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "DCBypassPro";

        public static bool IsAutoStartEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
                return key?.GetValue(AppName) != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool SetAutoStart(bool enable)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
                if (key == null) return false;

                if (enable)
                {
                    string exePath = Process.GetCurrentProcess().MainModule?.FileName ??
                                     Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dcbypass.exe");
                    key.SetValue(AppName, $"\"{exePath}\" --tray");
                }
                else
                {
                    key.DeleteValue(AppName, false);
                }

                PresetManager.Config.AutoStartWithWindows = enable;
                PresetManager.SaveConfig();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void FlushDns()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "ipconfig",
                    Arguments = "/flushdns",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                p?.WaitForExit(3000);
            }
            catch { }
        }

        public static void KillDiscord()
        {
            try
            {
                var processes = Process.GetProcessesByName("Discord");
                foreach (var p in processes)
                {
                    try
                    {
                        p.Kill(true);
                        p.WaitForExit(1000);
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static bool ClearDiscordCache()
        {
            try
            {
                KillDiscord();
                Thread.Sleep(1000);

                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string discordDir = Path.Combine(appData, "discord");

                if (Directory.Exists(discordDir))
                {
                    string[] cacheDirs = new[]
                    {
                        Path.Combine(discordDir, "Cache"),
                        Path.Combine(discordDir, "Code Cache"),
                        Path.Combine(discordDir, "GPUCache"),
                        Path.Combine(discordDir, "DawnCache")
                    };

                    foreach (var dir in cacheDirs)
                    {
                        if (Directory.Exists(dir))
                        {
                            try { Directory.Delete(dir, true); } catch { }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool FixGreyScreenAndRestart()
        {
            try
            {
                // 1. Kill Discord
                KillDiscord();

                // 2. Clear Discord network & web cache
                ClearDiscordCache();

                // 3. Flush DNS
                FlushDns();

                // 4. Ensure DPI Bypass is running
                if (!DpiEngine.Instance.IsRunning)
                {
                    DpiEngine.Instance.Start();
                }

                Thread.Sleep(1500);

                // 5. Launch Discord fresh
                return LaunchDiscord();
            }
            catch
            {
                return false;
            }
        }

        public static bool LaunchDiscord()
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string updateExe = Path.Combine(localAppData, "Discord", "Update.exe");

                if (File.Exists(updateExe))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = updateExe,
                        Arguments = "--processStart Discord.exe",
                        UseShellExecute = true
                    });
                    return true;
                }

                // Check default discord shortcut in start menu
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string startMenuLink = Path.Combine(appData, @"Microsoft\Windows\Start Menu\Programs\Discord Inc\Discord.lnk");
                if (File.Exists(startMenuLink))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = startMenuLink,
                        UseShellExecute = true
                    });
                    return true;
                }

                // Try opening discord protocol
                Process.Start(new ProcessStartInfo
                {
                    FileName = "discord://",
                    UseShellExecute = true
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
