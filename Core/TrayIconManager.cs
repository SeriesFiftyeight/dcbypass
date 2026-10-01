using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBypass.Core
{
    public static class WindowHelper
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int SW_RESTORE = 9;

        public static void HideConsole()
        {
            var handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_HIDE);
            }
        }

        public static void ShowConsole()
        {
            var handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_RESTORE);
                SetForegroundWindow(handle);
            }
        }

        public static bool IsConsoleVisible()
        {
            var handle = GetConsoleWindow();
            return handle != IntPtr.Zero && IsWindowVisible(handle);
        }

        public static void ToggleConsole()
        {
            if (IsConsoleVisible())
            {
                HideConsole();
            }
            else
            {
                ShowConsole();
            }
        }
    }

    public class TrayIconManager : IDisposable
    {
        private static TrayIconManager? _instance;
        public static TrayIconManager Instance => _instance ??= new TrayIconManager();

        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenu;
        private ToolStripMenuItem? _titleItem;
        private ToolStripMenuItem? _statusItem;
        private ToolStripMenuItem? _toggleBypassItem;
        private ToolStripMenuItem? _showConsoleItem;
        private ToolStripMenuItem? _autoStartItem;
        private ToolStripMenuItem? _presetsMenu;
        private ToolStripMenuItem? _pingTestItem;
        private ToolStripMenuItem? _launchDcItem;
        private ToolStripMenuItem? _fixGreyScreenItem;
        private ToolStripMenuItem? _langItem;
        private ToolStripMenuItem? _exitItem;

        private Icon? _activeIcon;
        private Icon? _inactiveIcon;

        private Thread? _uiThread;
        private bool _isDisposed;

        public void Initialize()
        {
            _uiThread = new Thread(() =>
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                CreateIcons();
                SetupTray();

                DpiEngine.Instance.StatusChanged += OnEngineStatusChanged;

                Application.Run();
            });

            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.IsBackground = true;
            _uiThread.Start();
        }

        private void CreateIcons()
        {
            _activeIcon = GenerateShieldIcon(Color.FromArgb(88, 101, 242), Color.FromArgb(46, 204, 113)); // Discord Blue + Green Accent
            _inactiveIcon = GenerateShieldIcon(Color.FromArgb(80, 80, 80), Color.FromArgb(231, 76, 60)); // Grey + Red Accent
        }

        private Icon GenerateShieldIcon(Color baseColor, Color accentColor)
        {
            using var bmp = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            // Draw shield background
            using var brush = new LinearGradientBrush(new Point(0, 0), new Point(32, 32), baseColor, Color.FromArgb(20, 20, 30));
            using var path = new GraphicsPath();
            path.AddPolygon(new PointF[]
            {
                new PointF(16, 2),
                new PointF(29, 6),
                new PointF(29, 18),
                new PointF(16, 30),
                new PointF(3, 18),
                new PointF(3, 6)
            });
            g.FillPath(brush, path);

            // Draw border
            using var pen = new Pen(accentColor, 2f);
            g.DrawPath(pen, path);

            // Draw lightning bolt
            using var boltBrush = new SolidBrush(Color.White);
            g.FillPolygon(boltBrush, new PointF[]
            {
                new PointF(17, 7),
                new PointF(10, 17),
                new PointF(15, 17),
                new PointF(13, 25),
                new PointF(21, 14),
                new PointF(16, 14)
            });

            return Icon.FromHandle(bmp.GetHicon());
        }

        private void SetupTray()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.RenderMode = ToolStripRenderMode.System;

            // Title
            _titleItem = new ToolStripMenuItem($"🛡️ {Strings.TrayTitle}") { Enabled = false };
            _titleItem.Font = new Font(_titleItem.Font, FontStyle.Bold);

            // Status display
            _statusItem = new ToolStripMenuItem(Strings.TrayStatusPassive) { Enabled = false };

            // Toggle bypass
            _toggleBypassItem = new ToolStripMenuItem(Strings.MenuStartBypass, null, (s, e) =>
            {
                if (DpiEngine.Instance.IsRunning)
                {
                    DpiEngine.Instance.Stop();
                }
                else
                {
                    DpiEngine.Instance.Start();
                }
            });

            // Presets Submenu
            _presetsMenu = new ToolStripMenuItem(Strings.TrayPresetsMenu);
            RefreshPresetsMenu();

            // Toggle Console
            _showConsoleItem = new ToolStripMenuItem(Strings.TrayShowConsole, null, (s, e) =>
            {
                WindowHelper.ToggleConsole();
                UpdateConsoleMenuItemText();
            });

            // Ping Test
            _pingTestItem = new ToolStripMenuItem(Strings.TrayPingTest, null, async (s, e) =>
            {
                ShowNotification("DCBaypass", Strings.PingTestingWait, ToolTipIcon.Info);
                var res = await ConnectivityTester.TestEndpointAsync("Discord", "https://discord.com");
                if (res.IsReachable)
                {
                    ShowNotification(Strings.IsTurkish ? "Test Başarılı! 🟢" : "Test Passed! 🟢", $"Discord {Strings.StatusReachable}! {res.LatencyMs} ms\n{res.Details}", ToolTipIcon.Info);
                }
                else
                {
                    ShowNotification(Strings.IsTurkish ? "Test Başarısız 🔴" : "Test Failed 🔴", $"Discord {Strings.StatusBlocked}!\n{res.Details}", ToolTipIcon.Warning);
                }
            });

            // Launch Discord
            _launchDcItem = new ToolStripMenuItem(Strings.TrayLaunchDiscord, null, (s, e) =>
            {
                StartupManager.LaunchDiscord();
            });

            // Fix Grey Screen
            _fixGreyScreenItem = new ToolStripMenuItem(Strings.MenuFixGreyScreen, null, (s, e) =>
            {
                StartupManager.FixGreyScreenAndRestart();
                ShowNotification("DCBaypass", Strings.IsTurkish ? "Önbellek temizlendi ve Discord başlatıldı!" : "Cache cleared & Discord restarted!", ToolTipIcon.Info);
            });

            // Auto-start with Windows
            _autoStartItem = new ToolStripMenuItem(Strings.TrayAutoStart, null, (s, e) =>
            {
                bool current = StartupManager.IsAutoStartEnabled();
                StartupManager.SetAutoStart(!current);
                if (_autoStartItem != null) _autoStartItem.Checked = !current;
                ShowNotification(Strings.AutoStartLabel, !current ? Strings.AutoStartOn : Strings.AutoStartOff, ToolTipIcon.Info);
            })
            {
                Checked = StartupManager.IsAutoStartEnabled()
            };

            // Language Switcher in Tray
            _langItem = new ToolStripMenuItem(Strings.TrayLanguageMenu, null, (s, e) =>
            {
                Strings.ToggleLanguage();
                RefreshLocalization();
            });

            // Exit
            _exitItem = new ToolStripMenuItem(Strings.TrayExit, null, (s, e) =>
            {
                DpiEngine.Instance.Stop();
                ExitApp();
            });

            _contextMenu.Items.Add(_titleItem);
            _contextMenu.Items.Add(_statusItem);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add(_toggleBypassItem);
            _contextMenu.Items.Add(_presetsMenu);
            _contextMenu.Items.Add(_launchDcItem);
            _contextMenu.Items.Add(_fixGreyScreenItem);
            _contextMenu.Items.Add(_pingTestItem);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add(_showConsoleItem);
            _contextMenu.Items.Add(_autoStartItem);
            _contextMenu.Items.Add(_langItem);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add(_exitItem);

            _notifyIcon = new NotifyIcon
            {
                Icon = _inactiveIcon,
                Text = "DCBaypass Pro",
                ContextMenuStrip = _contextMenu,
                Visible = true
            };

            _notifyIcon.DoubleClick += (s, e) =>
            {
                WindowHelper.ToggleConsole();
                UpdateConsoleMenuItemText();
            };

            UpdateTrayState(DpiEngine.Instance.IsRunning);
        }

        public void RefreshLocalization()
        {
            if (_contextMenu == null) return;

            try
            {
                _contextMenu.Invoke(new Action(() =>
                {
                    if (_titleItem != null) _titleItem.Text = $"🛡️ {Strings.TrayTitle}";
                    if (_presetsMenu != null) _presetsMenu.Text = Strings.TrayPresetsMenu;
                    if (_launchDcItem != null) _launchDcItem.Text = Strings.TrayLaunchDiscord;
                    if (_fixGreyScreenItem != null) _fixGreyScreenItem.Text = Strings.MenuFixGreyScreen;
                    if (_pingTestItem != null) _pingTestItem.Text = Strings.TrayPingTest;
                    if (_autoStartItem != null) _autoStartItem.Text = Strings.TrayAutoStart;
                    if (_langItem != null) _langItem.Text = Strings.TrayLanguageMenu;
                    if (_exitItem != null) _exitItem.Text = Strings.TrayExit;

                    RefreshPresetsMenu();
                    UpdateConsoleMenuItemText();
                    UpdateTrayState(DpiEngine.Instance.IsRunning);
                }));
            }
            catch { }
        }

        private void RefreshPresetsMenu()
        {
            if (_presetsMenu == null) return;
            _presetsMenu.DropDownItems.Clear();

            int currentId = PresetManager.Config.SelectedPresetId;

            foreach (var preset in PresetManager.Presets)
            {
                var item = new ToolStripMenuItem($"[{preset.Id}] {preset.Name}")
                {
                    Checked = (preset.Id == currentId)
                };

                int pId = preset.Id;
                item.Click += (s, e) =>
                {
                    PresetManager.SetPreset(pId);
                    RefreshPresetsMenu();
                    if (DpiEngine.Instance.IsRunning)
                    {
                        DpiEngine.Instance.Start(PresetManager.GetCurrentPreset());
                    }
                    ShowNotification(Strings.IsTurkish ? "Mod Değiştirildi" : "Preset Changed", $"{preset.Name}", ToolTipIcon.Info);
                };

                _presetsMenu.DropDownItems.Add(item);
            }
        }

        private void OnEngineStatusChanged(bool isRunning)
        {
            if (_notifyIcon == null || _contextMenu == null) return;

            try
            {
                _contextMenu.Invoke(new Action(() =>
                {
                    UpdateTrayState(isRunning);
                }));
            }
            catch { }
        }

        private void UpdateTrayState(bool isRunning)
        {
            if (_notifyIcon == null) return;

            if (isRunning)
            {
                _notifyIcon.Icon = _activeIcon;
                _notifyIcon.Text = $"DCBaypass: {Strings.StatusActive}\n{DpiEngine.Instance.ActivePreset.Name}";
                if (_statusItem != null) _statusItem.Text = Strings.TrayStatusActive;
                if (_toggleBypassItem != null) _toggleBypassItem.Text = Strings.MenuStopBypass;
            }
            else
            {
                _notifyIcon.Icon = _inactiveIcon;
                _notifyIcon.Text = $"DCBaypass: {Strings.StatusPassive}";
                if (_statusItem != null) _statusItem.Text = Strings.TrayStatusPassive;
                if (_toggleBypassItem != null) _toggleBypassItem.Text = Strings.MenuStartBypass;
            }
        }

        private void UpdateConsoleMenuItemText()
        {
            if (_showConsoleItem != null)
            {
                _showConsoleItem.Text = WindowHelper.IsConsoleVisible()
                    ? Strings.TrayHideConsole
                    : Strings.TrayShowConsole;
            }
        }

        public void ShowNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            if (_notifyIcon != null && _notifyIcon.Visible)
            {
                _notifyIcon.ShowBalloonTip(3000, title, message, icon);
            }
        }

        public void ExitApp()
        {
            DpiEngine.Instance.Stop();
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            Environment.Exit(0);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _notifyIcon?.Dispose();
                _activeIcon?.Dispose();
                _inactiveIcon?.Dispose();
            }
        }
    }
}
