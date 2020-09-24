namespace QNMCL
{
    using KMCCC.Launcher;
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using System.Windows;

    public partial class App : Application
    {
        private MainWindow _mainWindow;
        public static LauncherCore Core = LauncherCore.Create((string) null);

        [STAThread, DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            new App().Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Thread.Sleep(0x3e8);
            Logger.End();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Reporter.SetClientName("QNMCL @" + Assembly.GetExecutingAssembly().GetName().Version);
            this._mainWindow = new MainWindow();
            this._mainWindow.Show();
        }
    }
}

