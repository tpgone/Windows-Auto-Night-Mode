﻿using AutoDarkModeSvc.Config;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AutoDarkModeSvc;
using System.Diagnostics;
using AutoDarkModeApp.Handlers;

namespace AutoDarkModeApp
{
    /// <summary>
    /// Interaction logic for PageApps.xaml
    /// </summary>
    public partial class PageApps : Page
    {
        private AdmConfigBuilder builder = AdmConfigBuilder.Instance();
        bool is1903 = false;

        public PageApps()
        {
            builder.Load();
            InitializeComponent();
            UiHandler();

            //follow windows theme
            ThemeChange(this, null);
            SourceChord.FluentWPF.SystemTheme.ThemeChanged += ThemeChange;
        }

        //react to windows theme change
        private void ThemeChange(object sender, EventArgs e)
        {
            if (SourceChord.FluentWPF.SystemTheme.AppTheme.Equals(SourceChord.FluentWPF.ApplicationTheme.Dark))
            {
                EdgyIcon.Source = new BitmapImage(new Uri(@"/Resources/Microsoft_Edge_Logo_White.png", UriKind.Relative));
            }
            else
            {
                EdgyIcon.Source = new BitmapImage(new Uri(@"/Resources/Microsoft_Edge_Logo.png", UriKind.Relative));
            }
        }

        private void UiHandler()
        {
            try
            {
                builder.Load();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
            //if automatic theme switch isn't enabled
            if (!builder.Config.AutoThemeSwitchingEnabled)
            {
                AccentColorCheckBox.IsEnabled = false;
                SystemComboBox.IsEnabled = false;
                AppComboBox.IsEnabled = false;
                EdgeComboBox.IsEnabled = false;
                OfficeComboBox.IsEnabled = false;
                CheckBoxOfficeWhiteTheme.IsEnabled = false;
            }

            //if a windows theme file was picked
            if (!builder.Config.ClassicMode)
            {
                AccentColorCheckBox.IsEnabled = false;
                AccentColorCheckBox.ToolTip = Properties.Resources.ToolTipDisabledDueTheme;
                SystemComboBox.IsEnabled = false;
                SystemComboBox.ToolTip = Properties.Resources.ToolTipDisabledDueTheme;
                AppComboBox.IsEnabled = false;
                AppComboBox.ToolTip = Properties.Resources.ToolTipDisabledDueTheme;
            }

            //if the OS version is older than 1903
            if (int.Parse(RegistryHandler.GetOSversion()).CompareTo(1900) > 0) is1903 = true;
            if (!is1903)
            {
                SystemComboBox.IsEnabled = false;
                SystemComboBox.ToolTip = Properties.Resources.cmb1903;
                AccentColorCheckBox.IsEnabled = false;
                AccentColorCheckBox.ToolTip = Properties.Resources.cmb1903;
            }
            else
            //os version 1903+
            {
                //inform user about settings
                if(builder.Config.ClassicMode) AccentColorCheckBox.ToolTip = Properties.Resources.cbAccentColor;

                //is accent color switch enabled?
                AccentColorCheckBox.IsChecked = builder.Config.AccentColorTaskbarEnabled;
            }

            //combobox
            AppComboBox.SelectedIndex = (int)builder.Config.AppsTheme;
            SystemComboBox.SelectedIndex = (int)builder.Config.SystemTheme;
            EdgeComboBox.SelectedIndex = (int)builder.Config.EdgeTheme;
            if (builder.Config.Office.Enabled)
            {
                OfficeComboBox.SelectedIndex = (int)builder.Config.Office.Mode;
            }
            else
            {
                OfficeComboBox.SelectedIndex = 3;
            }


            //checkbox
            if (builder.Config.Office.LightTheme == 5)
            {
                CheckBoxOfficeWhiteTheme.IsChecked = true;
            }
        }

        private void AppComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (AppComboBox.SelectedIndex.Equals(0))
            {
                builder.Config.AppsTheme = Mode.Switch;
            }

            if (AppComboBox.SelectedIndex.Equals(1))
            {
                builder.Config.AppsTheme = Mode.LightOnly;
            }

            if (AppComboBox.SelectedIndex.Equals(2))
            {
                builder.Config.AppsTheme = Mode.DarkOnly;
            }
            try
            {
                builder.Save();
            } 
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        private void SystemComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (SystemComboBox.SelectedIndex.Equals(0))
            {
                builder.Config.SystemTheme = Mode.Switch;
                AccentColorCheckBox.IsEnabled = true;
            }

            if (SystemComboBox.SelectedIndex.Equals(1))
            {
                builder.Config.SystemTheme = Mode.LightOnly;
                AccentColorCheckBox.IsEnabled = false;
                AccentColorCheckBox.IsChecked = false;
            }

            if (SystemComboBox.SelectedIndex.Equals(2))
            {
                builder.Config.SystemTheme = Mode.DarkOnly;
                Properties.Settings.Default.SystemThemeChange = 2;
                AccentColorCheckBox.IsEnabled = true;
            }
            try
            {
                builder.Save();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        private void EdgeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (EdgeComboBox.SelectedIndex.Equals(0))
            {
                builder.Config.EdgeTheme = Mode.Switch;
            }

            if (EdgeComboBox.SelectedIndex.Equals(1))
            {
                builder.Config.EdgeTheme = Mode.LightOnly;
            }

            if (EdgeComboBox.SelectedIndex.Equals(2))
            {
                builder.Config.EdgeTheme = Mode.DarkOnly;
            }

            if (EdgeComboBox.SelectedIndex.Equals(3))
            {
                builder.Config.EdgeTheme = Mode.Switch;
            }
            try
            {
                builder.Save();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }
        private void DisableEdgeSwitch()
        {
            //does nothing for now
            Properties.Settings.Default.EdgeThemeChange = 3;
            EdgeComboBox.SelectedIndex = 3;
        }

        private void AccentColorCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked ?? false)
            {
                builder.Config.AccentColorTaskbarEnabled = true;
            }
            try
            {
                builder.Save();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        private void OfficeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            builder.Config.Office.Enabled = true;
            if (OfficeComboBox.SelectedIndex.Equals(0))
            {
                builder.Config.Office.Mode = Mode.Switch;
            }

            if (OfficeComboBox.SelectedIndex.Equals(1))
            {
                builder.Config.Office.Mode = Mode.LightOnly;
            }

            if (OfficeComboBox.SelectedIndex.Equals(2))
            {
                builder.Config.Office.Mode = Mode.DarkOnly;
            }

            if (OfficeComboBox.SelectedIndex.Equals(3))
            {
                builder.Config.Office.Enabled = false;
            }
            try
            {
                builder.Save();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }
        private void DisableOfficeSwitch()
        {
            //does nothing for now
            Properties.Settings.Default.OfficeThemeChange = 3;
            OfficeComboBox.SelectedIndex = 3;
        }

        private void ButtonWikiBrowserExtension_Click(object sender, RoutedEventArgs e)
        {
            StartProcessByProcessInfo("https://github.com/Armin2208/Windows-Auto-Night-Mode/wiki/Dark-Mode-for-Webbrowser");
        }

        private void CheckBoxOfficeWhiteTheme_Click(object sender, RoutedEventArgs e)
        {
            if(CheckBoxOfficeWhiteTheme.IsChecked ?? true){
                builder.Config.Office.LightTheme = 5;
                OfficeComboBox_DropDownClosed(this, null);
            }
            else
            {
                builder.Config.Office.LightTheme = 0;
                OfficeComboBox_DropDownClosed(this, null);
            }
        }

        private void ShowErrorMessage(Exception ex)
        {
            string error = Properties.Resources.errorThemeApply + "\n\nError ocurred in: " + ex.Source + "\n\n" + ex.Message;
            MsgBox msg = new MsgBox(error, Properties.Resources.errorOcurredTitle, "error", "yesno")
            {
                Owner = Window.GetWindow(this)
            };
            msg.ShowDialog();
            var result = msg.DialogResult;
            if (result == true)
            {
                string issueUri = @"https://github.com/Armin2208/Windows-Auto-Night-Mode/issues";
                Process.Start(new ProcessStartInfo(issueUri)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            return;
        }

        private void StartProcessByProcessInfo(string message)
        {
            Process.Start(new ProcessStartInfo(message)
            {
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}