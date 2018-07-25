using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CleanDesk
{
    public partial class MainWindow : Form
    {
        const string S_About = "CleanDesk\r\n\r\nDeveloped by: Whilend\r\nCopyright ©️ 2018. All rights reserved";

        UserDesktop userDesktop;
        Settings settings;
        Debug debug;
        Task cleaner;

        string appDataPath;
        string settingsPath;
        int waitDotCount = 1;

        public MainWindow()
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Programs\\CleanDesk\\";
            settingsPath = appDataPath + "app.dat";

            settings = Settings.Load(settingsPath);
            userDesktop = new UserDesktop(settings.Categories);
            debug = new Debug(appDataPath + "app.log");
            debug.CreateInstance();

            InitializeComponent();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(S_About, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowStatistics(string prefix = "")
        {
            ChangeStats(UserDesktop.GetDesktopIcons().Length, prefix);
        }

        private void ChangeStats(int count, string prefix = "")
        {
            label2.Text = count >= 10 ? "DIRTY" : "CLEAN";
            label2.ForeColor = count >= settings.DirtyLimit ? Color.DarkRed : Color.Black;
            label3.Text = count.ToString() + prefix;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            ShowStatistics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            cleaner = new Task(delegate { userDesktop.Clean(); });
            cleaner.Start();
        }

        private void uiTimer_Tick(object sender, EventArgs e)
        {
            TaskStatus[] runStates = { TaskStatus.Running, TaskStatus.WaitingForChildrenToComplete };

            bool running = cleaner != null && runStates.InArray(cleaner.Status);

            ShowStatistics(running ? ".".Multiply(waitDotCount) : "");
            button1.Enabled = !running;

            waitDotCount++;
            if (waitDotCount > 3) waitDotCount = 1;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SettingsWindow.Execute(settings);
            settings.Save(settingsPath);
        }
    }
}
