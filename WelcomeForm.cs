using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PCPlaytimeTracker
{
    public partial class WelcomeForm : Form
    {
        private CheckBox? startupCheckBox; // Made nullable to avoid CS8618

        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(400, 300);
            this.Text = "Welcome to PC Playtime Tracker";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(10)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var titleLabel = new Label
            {
                Text = "Welcome to PC Playtime Tracker!",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            var descriptionLabel = new Label
            {
                Text = "This application runs in the background and tracks your PC usage time.\n" +
                       "Right-click the system tray icon to view your total time or exit.\n" +
                       "Your playtime is saved automatically.",
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            var sourceLink = new LinkLabel
            {
                Text = "View Source Code",
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            sourceLink.Click += (s, e) =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/your-repo/pc-playtime-tracker",
                    UseShellExecute = true
                });
            };

            var settingsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            startupCheckBox = new CheckBox
            {
                Text = "Run on Windows startup",
                Checked = true,
                AutoSize = true
            };

            var okButton = new Button
            {
                Text = "OK",
                AutoSize = true,
                DialogResult = DialogResult.OK
            };

            settingsPanel.Controls.Add(startupCheckBox);
            layout.Controls.Add(titleLabel, 0, 0);
            layout.Controls.Add(descriptionLabel, 0, 1);
            layout.Controls.Add(sourceLink, 0, 2);
            layout.Controls.Add(settingsPanel, 0, 3);
            layout.Controls.Add(okButton, 0, 4);

            this.Controls.Add(layout);
            this.AcceptButton = okButton;
        }

        public bool RunOnStartup => startupCheckBox?.Checked ?? true; // Handle null case
    }
}