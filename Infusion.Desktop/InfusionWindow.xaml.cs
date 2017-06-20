﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Infusion.Desktop.Launcher;
using Infusion.Desktop.Profiles;
using Infusion.Proxy;
using Infusion.Proxy.LegacyApi;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Infusion.Desktop
{
    public partial class InfusionWindow
    {
        private NotifyIcon notifyIcon;
        private string scriptFileName;

        public InfusionWindow()
        {
            InitializeComponent();

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(new Bitmap(Properties.Resources.infusion).GetHicon());
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
            };

            Legacy.CommandHandler.RegisterCommand(new Command("reload", () => Dispatcher.Invoke(() => Reload()), "Reloads an initial script file."));
            Legacy.CommandHandler.RegisterCommand(new Command("edit", () => Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() => Edit())), "Opens the script editor."));
            Legacy.CommandHandler.RegisterCommand(new Command("load", path => Dispatcher.Invoke(() => Load(path)), "Loads a script file."));
            Legacy.CommandHandler.RegisterCommand(new Command("cls", () => Dispatcher.Invoke(Cls), "Clears console content."));
        }

        private void Cls()
        {
            _console.Clear();
        }

        private void Load(string scriptFileName)
        {
            this.scriptFileName = scriptFileName;
            Reload();
        }

        internal void Initialize(Profile profile)
        {
            Title = $"{profile.Name}";

            if (!string.IsNullOrEmpty(profile.LauncherOptions.InitialScriptFileName))
                Load(profile.LauncherOptions.InitialScriptFileName);
        }

        public void Edit()
        {
            if (!string.IsNullOrEmpty(scriptFileName) && File.Exists(scriptFileName))
            {
                string scriptPath = System.IO.Path.GetDirectoryName(scriptFileName);
                _console.ScriptEngine.ScriptRootPath = scriptPath;

                var roslynPadWindow = new RoslynPad.MainWindow(_console.ScriptEngine, scriptPath);
                roslynPadWindow.Show();
            }
            else
                Program.Console.Error("Initial script is not set. You can set the initial script by restarting Infusion and setting an absolute path to a script in 'Initial script' edit box at Infusion launcher dialog, or by invoking ,load <absolute path to script>");
        }

        private void Reload()
        {
#pragma warning disable 4014
            Reload(scriptFileName);
#pragma warning restore 4014
        }

        private async Task Reload(string scriptFileName)
        {
            if (!string.IsNullOrEmpty(scriptFileName) && File.Exists(scriptFileName))
            {
                Legacy.CommandHandler.Terminate();
                _console.ScriptEngine.Reset();
                await _console.ScriptEngine.ExecuteScript(scriptFileName);
            }
            else
                Program.Console.Error(
                    "Initial script is not set. You can set the initial script by restarting Infusion and setting an absolute path to a script in 'Initial script' edit box at Infusion launcher dialog, or by invoking ,load <absolute path to script>");
        }

        protected override void OnClosed(EventArgs e)
        {
            if (ProfileRepositiory.SelectedProfile != null)
                ProfileRepositiory.SaveProfile(ProfileRepositiory.SelectedProfile);

            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized && Program.Configuration.HideWhenMinimized)
                Hide();

            base.OnStateChanged(e);
        }

        private void InfusionWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, (Action)(() =>
            {
                _console.Initialize();

                var launcherWindow = new LauncherWindow(Initialize);
                launcherWindow.Show();
                launcherWindow.Activate();
            }));
        }
    }
}