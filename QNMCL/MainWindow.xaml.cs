namespace QNMCL
{
    using KMCCC.Authentication;
    using KMCCC.Launcher;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using QNMCL.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();
            string last = Config.LastVersion;
            KMCCC.Launcher.Version[] source = App.Core.GetVersions().ToArray<KMCCC.Launcher.Version>();
            this.ListVersions.ItemsSource = source;
            if (source.Count<KMCCC.Launcher.Version>(ver => (ver.Id == last)) > 0)
            {
                this.ListVersions.SelectedItem = source.First<KMCCC.Launcher.Version>(ver => ver.Id == last);
            }
            else if (source.Any<KMCCC.Launcher.Version>())
            {
                this.ListVersions.SelectedItem = source[0];
            }
            App.Core.GameExit += new Action<LaunchHandle, int>(this.OnExit);
            App.Core.GameLog += new Action<LaunchHandle, string>(MainWindow.OnLog);
            try
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\QNMCL.cfg"))
                {
                    MessageBox.Show("检测到你第一次打开启动器，请设置游戏！");
                    new ConfigWindow().ShowDialog();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\bg.png";
            if (File.Exists(path))
            {
                this.bg.Source = new BitmapImage(new Uri(path));
            }
            if ((this.ListVersions.Text == "") || (this.ListVersions.Text == null))
            {
                MessageBox.Show("没有找到Minecraft版本文件，启动器不能启动！");
                base.Close();
                base.Close();
            }
            else
            {
                try
                {
                    string str3 = AppDomain.CurrentDomain.BaseDirectory + @"\music.wav";
                    if (File.Exists(str3))
                    {
                        new SoundPlayer(str3).Play();
                    }
                    else
                    {
                        SoundPlayer soundPlayer2 = new SoundPlayer(QNMCL.Properties.Resources.music);
                        soundPlayer2.Play();
                    }
                }
                catch (Exception exception2)
                {
                    MessageBox.Show(exception2.Message);
                }
            }
        }

        private void Btn_config_Click(object sender, RoutedEventArgs e)
        {
            new ConfigWindow { Owner = this }.ShowDialog();
        }

        private void Btn_launch_Click(object sender, RoutedEventArgs e)
        {
            KMCCC.Launcher.Version selectedItem = (KMCCC.Launcher.Version) this.ListVersions.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("检测到没有游戏文件！");
            }
            else
            {
                Config.LastVersion = selectedItem.Id;
                LaunchOptions options = new LaunchOptions {
                    Version = selectedItem,
                    MaxMemory = Config.MaxMemory,
                    Authenticator = (Config.Authenticator == "Yggdrasil") ? ((IAuthenticator) new YggdrasilLogin(Config.UserName, Config.Password, true, null, null)) : ((IAuthenticator) new OfflineAuthenticator(Config.UserName)),
                    Server = (Config.Server != null) ? new ServerInfo() : null,
                    Mode = (Config.LaunchMode == "BMCL") ? ((LaunchMode) LaunchMode.BmclMode) : ((Config.LaunchMode == "MCLauncher") ? ((LaunchMode) LaunchMode.MCLauncher) : null)
                };
                Action<MinecraftLaunchArguments>[] argumentsOperators = new Action<MinecraftLaunchArguments>[] { args => args.AdvencedArguments.Add(Config.AdvancedArguments) };
                LaunchResult result = App.Core.Launch(options, argumentsOperators);
                if (!result.Success)
                {
                    MessageBox.Show(result.ErrorMessage, result.ErrorType.ToString(), MessageBoxButton.OK, MessageBoxImage.Hand);
                    switch (result.ErrorType)
                    {
                        case ErrorType.NoJAVA:
                        case ErrorType.AuthenticationFailed:
                            new ConfigWindow { Owner = this }.ShowDialog();
                            break;
                    }
                }
                else
                {
                    base.Hide();
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            if ((this.textBox.Text == "") || (this.textBox.Text == null))
            {
                MessageBox.Show("你还没有填写版本名称");
            }
            else if (this.textBox.Text.Contains(" "))
            {
                MessageBox.Show("请不要填写空格");
            }
            else if (this.textBox.Text.StartsWith(".") || this.textBox.Text.EndsWith("."))
            {
                MessageBox.Show("开头或结尾不能为\".\"");
            }
            else
            {
                string sourceDirName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @".minecraft\versions\" + this.ListVersions.Text;
                string destDirName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @".minecraft\versions\" + this.textBox.Text;
                try
                {
                    Directory.Move(sourceDirName, destDirName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                try
                {
                    File.Move(destDirName + @"\" + this.ListVersions.Text + ".jar", destDirName + @"\" + this.textBox.Text + ".jar");
                    File.Move(destDirName + @"\" + this.ListVersions.Text + ".json", destDirName + @"\" + this.textBox.Text + ".json");
                    string str6 = destDirName + @"\" + this.ListVersions.Text + "-natives";
                    string str7 = destDirName + @"\" + this.textBox.Text + "-natives";
                    if (Directory.Exists(str6))
                    {
                        Directory.Move(str6, str7);
                    }
                }
                catch (Exception exception2)
                {
                    MessageBox.Show(exception2.Message);
                }
                string path = destDirName + @"\" + this.textBox.Text + ".json";
                JObject obj2 = (JObject) JsonConvert.DeserializeObject(File.ReadAllText(path));
                obj2["id"] = this.textBox.Text;
                string s = JsonConvert.SerializeObject(obj2);
                File.Delete(path);
                FileStream stream = new FileStream(path, FileMode.Create);
                byte[] bytes = new UTF8Encoding().GetBytes(s);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
                Process.Start(Assembly.GetExecutingAssembly().Location);
                base.Close();
            }
        }

        private void delgame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MessageBox.Show("你确认删除吗！", "QNMCL", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + @".minecraft\versions\" + this.ListVersions.Text + @"\", true);
                    Process.Start(Assembly.GetExecutingAssembly().Location);
                    base.Close();
                }
                else
                {
                    Process.Start(Assembly.GetExecutingAssembly().Location);
                    base.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void from_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("作者:MCBBS浅念\n本启动器基于KMCCC\n特别鸣谢\ngamerteam -KMCCC教程\nzhouyiran2 -KMCCC作者\nMineStudio -神奇的小组");
        }

        private void javalink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "http://www.java.com");
        }

        private void label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string arguments = AppDomain.CurrentDomain.BaseDirectory + @".minecraft\versions\" + this.ListVersions.Text + @"\";
            Process.Start("explorer.exe", arguments);
        }

        private void link_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", "http://tball.org.cn");
        }

        private void link_MouseEnter(object sender, EventArgs e)
        {
            Color color = Color.FromRgb(0xff, 0, 0);
            this.link.Foreground = new SolidColorBrush(color);
        }

        private void link_MouseLeave(object sender, EventArgs e)
        {
            Color color = Color.FromRgb(0, 0, 0);
            this.link.Foreground = new SolidColorBrush(color);
        }

        private void OnExit(LaunchHandle handle, int code)
        {
            object[] args = new object[] { code };
            base.Dispatcher.Invoke(new Action<int>(this.OnGameExit), args);
        }

        private void OnGameExit(int code)
        {
            if (code == 0)
            {
                base.Close();
            }
            else
            {
                MessageBox.Show("Minecraft崩溃了，请查看error.log");
                base.Close();
            }
        }

        private static void OnLog(LaunchHandle handle, string line)
        {
            Logger.Log(line);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

    }
}

