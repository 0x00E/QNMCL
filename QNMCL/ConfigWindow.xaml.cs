namespace QNMCL
{
    using Microsoft.CSharp.RuntimeBinder;
    using Microsoft.VisualBasic.Devices;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    public partial class ConfigWindow : Window
    {

        public ConfigWindow()
        {
            this.InitializeComponent();
            switch (Config.Authenticator)
            {
                case "Yggdrasil":
                    this.Authenticator.SelectedIndex = 0;
                    break;

                case "Offline":
                    this.Authenticator.SelectedIndex = 1;
                    break;
            }
            string launchMode = Config.LaunchMode;
            if (launchMode == "Default")
            {
                this.LaunchMode.SelectedIndex = 0;
            }
            else if (launchMode == "BMCL")
            {
                this.LaunchMode.SelectedIndex = 1;
            }
            else if (launchMode == "MCLauncher")
            {
                this.LaunchMode.SelectedIndex = 2;
            }
            this.UserName.Text = Config.UserName;
            this.Password.Password = Config.Password;
            this.MaxMem.Text = Config.MaxMemory.ToString(CultureInfo.InvariantCulture);
            this.AdvArg.Text = Config.AdvancedArguments;
            ComputerInfo info = new ComputerInfo();
            this.maxMeoLabel.Content = "系统物理内存:" + ((info.TotalPhysicalMemory / ((ulong) 0x400L)) / ((ulong) 0x400L));
            this.canMeoLabel.Content = "可用物理内存:" + ((info.AvailablePhysicalMemory / ((ulong) 0x400L)) / ((ulong) 0x400L));
            this.MaxMem.Text = "1024";
            if ((Config.Server != null) && (Config.Server != ""))
            {
                if ((Config.Port != null) && (Config.Port != ""))
                {
                    this.server.Text = Config.Server + ":" + Config.Port;
                }
                else
                {
                    this.server.Text = Config.Server;
                }
            }
        }

        private void Authenticator_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            switch ((String)((dynamic)Authenticator.SelectedItem).Tag) {
                case "Yggdrasil":
                    Password.IsEnabled = true;
                    break;
                case "Offline":
                    Password.IsEnabled = false;
                    break;
            }
        }

        private void bg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\bg.png";
                System.IO.File.Delete(path);
                if (this.bgPath.Text.StartsWith("http"))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("User-Agent", "Chrome");
                        client.DownloadFile(this.bgPath.Text, path);
                    }
                }
                else
                {
                    System.IO.File.Move(this.bgPath.Text, path);
                }
                MessageBox.Show("背景加载成功，重启启动器生效！");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void memory_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MaxMem.Text = "1024";
        }

        private void music_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\music.wav";
                System.IO.File.Delete(path);
                if (this.musictext.Text.StartsWith("http"))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("User-Agent", "Chrome");
                        client.DownloadFile(this.musictext.Text, path);
                    }
                }
                else
                {
                    System.IO.File.Move(this.musictext.Text, path);
                }
                MessageBox.Show("音乐加载成功，重启启动器生效！");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void resetlabel_Click(object sender, RoutedEventArgs e)
        {
            ComputerInfo info = new ComputerInfo();
            this.maxMeoLabel.Content = "系统物理内存:" + ((info.TotalPhysicalMemory / ((ulong) 0x400L)) / ((ulong) 0x400L));
            this.canMeoLabel.Content = "可用物理内存:" + ((info.AvailablePhysicalMemory / ((ulong) 0x400L)) / ((ulong) 0x400L));
        }

        private void up_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "http://pan.baidu.com/s/1gfsMQun");
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            if (Authenticator.SelectedItem == null) {
                MessageBox.Show("请选择验证方式");
                e.Cancel = true;
                return;
            }
            switch ((String)((dynamic)Authenticator.SelectedItem).Tag) {
                case "Yggdrasil":
                    if (String.IsNullOrWhiteSpace(UserName.Text)) {
                        MessageBox.Show("请输入Email");
                        e.Cancel = true;
                        return;
                    }
                    if (String.IsNullOrWhiteSpace(Password.Password)) {
                        MessageBox.Show("请输入密码");
                        e.Cancel = true;
                        return;
                    }
                    Config.Authenticator = "Yggdrasil";
                    Config.UserName = UserName.Text;
                    Config.Password = Password.Password;
                    break;
                case "Offline":
                    if (String.IsNullOrWhiteSpace(UserName.Text)) {
                        MessageBox.Show("请输入用户名");
                        e.Cancel = true;
                        return;
                    }
                    Config.Authenticator = "Offline";
                    Config.UserName = UserName.Text;
                    break;
                default:
                    e.Cancel = true;
                    return;
            }
            if (LaunchMode.SelectedIndex == -1) {
                MessageBox.Show("请选择启动模式");
                e.Cancel = true;
                return;
            }
            Config.LaunchMode = (String)((dynamic)LaunchMode.SelectedItem).Tag;
            int maxMem;
            if (!int.TryParse(MaxMem.Text, out maxMem)) {
                MessageBox.Show("请输入正确的最大内存");
                e.Cancel = true;
                return;
            }
            Config.MaxMemory = maxMem;
            Config.AdvancedArguments = AdvArg.Text;
        }
    }
}

