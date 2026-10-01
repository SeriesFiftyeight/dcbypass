using System;
using System.Diagnostics;
using System.IO;

namespace DiscordBypass.Core
{
    public static class ServiceInstaller
    {
        private const string ServiceName = "GoodbyeDPI";

        public static bool IsServiceInstalled()
        {
            var res = RunScCommand($"query \"{ServiceName}\"");
            return res.Success && !res.Output.Contains("1060"); // 1060: Service does not exist
        }

        public static bool IsServiceRunning()
        {
            var res = RunScCommand($"query \"{ServiceName}\"");
            return res.Success && res.Output.Contains("RUNNING");
        }

        public static (bool Success, string Output) InstallService(BypassPreset preset)
        {
            string engineExe = DpiEngine.Instance.GetEngineExecutablePath();
            if (!File.Exists(engineExe))
            {
                return (false, "DPI Motoru bulunamadı!");
            }

            // Remove existing first
            UninstallService();

            string binPath = $"\\\"{engineExe}\\\" {preset.Arguments}";
            string cmd = $"create \"{ServiceName}\" binPath= \"{binPath}\" start= auto";

            var res = RunScCommand(cmd);
            if (res.Success)
            {
                RunScCommand($"description \"{ServiceName}\" \"Discord Zero-Lag DPI Bypass Service\"");
                RunScCommand($"start \"{ServiceName}\"");
            }

            return res;
        }

        public static (bool Success, string Output) UninstallService()
        {
            RunScCommand($"stop \"{ServiceName}\"");
            return RunScCommand($"delete \"{ServiceName}\"");
        }

        public static (bool Success, string Output) StartService()
        {
            return RunScCommand($"start \"{ServiceName}\"");
        }

        public static (bool Success, string Output) StopService()
        {
            return RunScCommand($"stop \"{ServiceName}\"");
        }

        private static (bool Success, string Output) RunScCommand(string args)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var p = Process.Start(psi);
                if (p == null) return (false, "sc.exe başlatılamadı.");
                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit(5000);

                bool success = p.ExitCode == 0 || output.Contains("SUCCESS") || output.Contains("BAŞARILI") || output.Contains("RUNNING");
                return (success, string.IsNullOrWhiteSpace(output) ? error : output);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
