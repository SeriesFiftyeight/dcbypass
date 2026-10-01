using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBypass.Core
{
    public class DpiEngine
    {
        private static DpiEngine? _instance;
        public static DpiEngine Instance => _instance ??= new DpiEngine();

        private Process? _process;
        private readonly object _lock = new();

        public bool IsRunning => _process != null && !_process.HasExited;
        public BypassPreset ActivePreset { get; private set; }
        public DateTime? StartedAt { get; private set; }
        public string LastError { get; private set; } = string.Empty;

        public event Action<bool>? StatusChanged;
        public event Action<string>? LogReceived;

        private DpiEngine()
        {
            ActivePreset = PresetManager.GetCurrentPreset();
        }

        public string GetEngineExecutablePath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string arch = Environment.Is64BitOperatingSystem ? "x86_64" : "x86";

            // Check possible paths
            string[] possiblePaths = new[]
            {
                Path.Combine(baseDir, "core_engine", arch, "goodbyedpi.exe"),
                Path.Combine(baseDir, arch, "goodbyedpi.exe"),
                Path.Combine(baseDir, "goodbyedpi.exe")
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return possiblePaths[0];
        }

        public bool Start(BypassPreset? preset = null)
        {
            lock (_lock)
            {
                if (IsRunning)
                {
                    Stop();
                }

                if (preset != null)
                {
                    ActivePreset = preset;
                    PresetManager.SetPreset(preset.Id);
                }
                else
                {
                    ActivePreset = PresetManager.GetCurrentPreset();
                }

                string exePath = GetEngineExecutablePath();
                if (!File.Exists(exePath))
                {
                    LastError = $"DPI Motoru bulunamadı: {exePath}";
                    LogReceived?.Invoke($"[HATA] {LastError}");
                    return false;
                }

                string workingDir = Path.GetDirectoryName(exePath)!;

                // Cleanup any stale process or service
                KillExistingGoodbyeDpiProcesses();

                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = ActivePreset.Arguments,
                        WorkingDirectory = workingDir,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    _process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

                    _process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            LogReceived?.Invoke($"[DPI] {e.Data}");
                        }
                    };

                    _process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            LogReceived?.Invoke($"[DPI-ERR] {e.Data}");
                        }
                    };

                    _process.Exited += (sender, e) =>
                    {
                        StartedAt = null;
                        StatusChanged?.Invoke(false);
                        LogReceived?.Invoke("[BİLGİ] DPI Bypass servisi durduruldu.");
                    };

                    bool started = _process.Start();
                    if (started)
                    {
                        _process.BeginOutputReadLine();
                        _process.BeginErrorReadLine();
                        StartedAt = DateTime.Now;
                        StatusChanged?.Invoke(true);
                        LogReceived?.Invoke($"[BAŞARILI] DPI Bypass başlatıldı! (Mod: {ActivePreset.Name})");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    LogReceived?.Invoke($"[HATA] DPI Bypass başlatılamadı: {ex.Message}");
                    _process = null;
                    return false;
                }

                return false;
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                try
                {
                    if (_process != null && !_process.HasExited)
                    {
                        _process.Kill(true);
                        _process.WaitForExit(2000);
                        _process.Dispose();
                    }
                }
                catch
                {
                    // Ignore errors during kill
                }
                finally
                {
                    _process = null;
                    StartedAt = null;
                }

                KillExistingGoodbyeDpiProcesses();
                StatusChanged?.Invoke(false);
            }
        }

        public static void KillExistingGoodbyeDpiProcesses()
        {
            try
            {
                var processes = Process.GetProcessesByName("goodbyedpi");
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
    }
}
