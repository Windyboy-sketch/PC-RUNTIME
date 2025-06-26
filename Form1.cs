using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Timers;

namespace PCPlaytimeTracker
{
    public partial class Form1 : Form
    {
        private int totalSeconds = 0;
        private string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PCPlaytimeTracker", "playtime.json");
        private NotifyIcon trayIcon;
        private System.Timers.Timer timer;
        private TimeDisplayForm timeDisplayForm;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(savePath))
            {
                var welcomeForm = new WelcomeForm();
                if (welcomeForm.ShowDialog() == DialogResult.OK)
                {
                    if (welcomeForm.RunOnStartup)
                    {
                        AddToStartup();
                    }
                }
            }
            else
            {
                AddToStartup();
            }

            LoadPlaytime();

            trayIcon = new NotifyIcon();
            trayIcon.Icon = System.Drawing.SystemIcons.Application;
            trayIcon.Visible = true;
            trayIcon.Text = "PC Playtime Tracker";
            trayIcon.BalloonTipTitle = "Playtime Tracker Running";
            trayIcon.BalloonTipText = "Tracking your PC uptime.";
            trayIcon.ShowBalloonTip(3000);

            var menu = new ContextMenuStrip();
            menu.Items.Add("Show Time").Click += (s, e) =>
            {
                if (timeDisplayForm == null || timeDisplayForm.IsDisposed)
                {
                    timeDisplayForm = new TimeDisplayForm(totalSeconds);
                    timeDisplayForm.FormClosed += (s2, e2) => timeDisplayForm = null;
                    timeDisplayForm.Show();
                }
                else
                {
                    timeDisplayForm.BringToFront();
                }
            };
            menu.Items.Add("Guide").Click += (s, e) =>
            {
                string guideText = "PC Playtime Tracker Guide:\n\n" +
                    "1. The application runs in the system tray.\n" +
                    "2. Right-click the tray icon to open the menu.\n" +
                    "3. Select 'Show Time' to view your total PC usage time.\n" +
                    "4. Select 'Guide' to view this guide again.\n" +
                    "5. Select 'Exit' to close the application.\n" +
                    "6. Your playtime is automatically saved to %AppData%\\PCPlaytimeTracker\\playtime.json.\n" +
                    "7. The application can be set to run on Windows startup.";
                MessageBox.Show(guideText, "PC Playtime Tracker Guide");
            };
            menu.Items.Add("Exit").Click += (s, e) =>
            {
                SavePlaytime();
                trayIcon.Visible = false;
                Application.Exit();
            };
            trayIcon.ContextMenuStrip = menu;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                totalSeconds++;
                SavePlaytime();
                if (timeDisplayForm != null && !timeDisplayForm.IsDisposed)
                {
                    timeDisplayForm.UpdateTime(totalSeconds);
                }
            };
            timer.Start();
        }

        private void LoadPlaytime()
        {
            if (File.Exists(savePath))
            {
                var json = File.ReadAllText(savePath);
                var doc = JsonDocument.Parse(json);
                totalSeconds = doc.RootElement.GetProperty("totalSeconds").GetInt32();
            }
        }

        private void SavePlaytime()
        {
            try
            {
                string directory = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(savePath, JsonSerializer.Serialize(new { totalSeconds }));
            }
            catch (UnauthorizedAccessException ex)
            {
                // Fallback to a temp directory if AppData is inaccessible
                string fallbackPath = Path.Combine(Path.GetTempPath(), "PCPlaytimeTracker", "playtime.json");
                string fallbackDirectory = Path.GetDirectoryName(fallbackPath);
                if (!Directory.Exists(fallbackDirectory))
                {
                    Directory.CreateDirectory(fallbackDirectory);
                }
                try
                {
                    File.WriteAllText(fallbackPath, JsonSerializer.Serialize(new { totalSeconds }));
                    savePath = fallbackPath; // Update savePath to use fallback
                    MessageBox.Show($"Switched to temp location: {fallbackPath}", "Permission Issue");
                }
                catch (Exception fallbackEx)
                {
                    MessageBox.Show($"Failed to save even to temp: {fallbackEx.Message}", "Critical Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving playtime to {savePath}: {ex.Message}", "Error");
            }
        }

        private void AddToStartup()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                using (RegistryKey? rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (rk != null)
                    {
                        rk.SetValue("PCPlaytimeTracker", $"\"{exePath}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add to startup: " + ex.Message);
            }
        }
    }
}