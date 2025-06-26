using System;
using System.Windows.Forms;

namespace PCPlaytimeTracker
{
    public partial class TimeDisplayForm : Form
    {
        private int totalSeconds;

        public TimeDisplayForm(int initialSeconds)
        {
            totalSeconds = initialSeconds;
            InitializeComponent();
            UpdateDisplay();
        }

        public void UpdateTime(int newTotalSeconds)
        {
            totalSeconds = newTotalSeconds;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            hourLabel.Text = $"Hour: {time.Hours:D2}";
            minuteLabel.Text = $"Minutes: {time.Minutes:D2}";
            secondLabel.Text = $"Seconds: {time.Seconds:D2}";
        }
    }
}