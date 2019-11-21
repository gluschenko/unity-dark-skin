using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using UnityDarkSkin.App.Core;
using UnityDarkSkin.Lib;
using Version = UnityDarkSkin.Lib.Version;

/*using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;*/

namespace UnityDarkSkin.App
{
    public partial class MainWindow : Window
    {
        // Internal preferances management
        public ApplicationData Data { get; private set; } // Contains current application state (e.g. window size)
        public DataManager<ApplicationData> Manager { get; private set; } // Сontrols data loading
        private readonly string PrefsFile = "prefs.json"; // Relative path to preferances file

        private string StartPath = @"C:\Program Files\Unity";
        private const string EditorFileName = "Unity.exe";

        private Patcher Patcher;

        public enum Section
        {
            StartScreen,
            PatchScreen
        }

        public MainWindow()
        {
            InitializeComponent();

            // Application data management
            Manager = new DataManager<ApplicationData>(PrefsFile);
            LoadData();
            Closed += (sender, args) => SaveData();

            //
            Navigate(Section.StartScreen);
            //

            DirectoryTextBox.PreviewMouseDoubleClick += (sender, args) => ChooseDirectoryButton_Click(sender, null);

            // Versions combo box
            foreach (var version in Versions.Get())
            {
                VersionsCombo.Items.Add(version);
            }
            VersionsCombo.SelectionChanged += VersionsCombo_SelectionChanged;

            // Resets thumbs by default
            SetThemeThumbs(ThemeType.None);
        }

        // Load & Save methods

        public void LoadData()
        {
            Data = Manager.Load(MessageHelper.ThrowException);
            //
            if (Data.WindowWidth > 0)
                Width = Data.WindowWidth;
            if (Data.WindowHeight > 0)
                Height = Data.WindowHeight;
        }

        public void SaveData()
        {
            Data.WindowWidth = Width;
            Data.WindowHeight = Height;
            //
            Manager.Save(Data, MessageHelper.ThrowException);
        }

        // Event handlers

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            IOHelper.OpenFolderDialog(StartPath, OnFolderChosen);
        }

        private void ChooseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            IOHelper.OpenFolderDialog(DirectoryTextBox.Text, OnFolderChosen);
        }

        private void VersionsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Patcher != null)
            {
                var version = (Version)VersionsCombo.SelectedItem;
                if (Patcher.CurrentVersion != version)
                {
                    Patcher.CurrentVersion = version;
                    Patcher.Reset();
                    OnVersionDetected();
                }
            }
        }

        private void SwitchThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Patcher != null)
            {
                Freeze("Switching theme...");
                ThreadHelper.Invoke(() => {
                    ThemeType newTheme = Patcher.CurrentTheme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
                    ThemeType theme = Patcher.SetTheme(newTheme);

                    if (newTheme == theme)
                    {
                        try
                        {
                            Patcher.Save();
                        }
                        catch (Exception ex)
                        {
                            MessageHelper.Error($"Could not to save {EditorFileName}! Check access permissions.\n\nDetails:\n\n{ex}");
                        }
                    }
                    else
                    {
                        MessageHelper.Error("Could not to switch theme");
                    }

                    Dispatcher.Invoke(() => {
                        Freeze(false);
                        SetThemeThumbs(theme);
                    });
                });
            }
        }

        private void MakeBackupButton_Click(object sender, RoutedEventArgs e)
        {
            Freeze("Making a new backup...");
            ThreadHelper.Invoke(() => {
                try
                {
                    Patcher?.MakeBackup();
                }
                catch (Exception ex)
                {
                    MessageHelper.ThrowException(ex);
                }

                Dispatcher.Invoke(() => {
                    Freeze(false);
                });
            });
        }

        private void RestoreBackupButton_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(Patcher.FilePath);

            Freeze("Searching backups...");
            ThreadHelper.Invoke(() => {
                var files = IOHelper.SearchFile(path, "Backup_", true, true);

                Dispatcher.Invoke(() => {
                    Freeze(false);
                    OnBackupFilesFound(files);
                });
            });
        }

        // Async callbacks
        private void OnFolderChosen(string path)
        {
            Navigate(Section.PatchScreen);

            DirectoryTextBox.Text = path;

            Freeze("Searching files...");
            ThreadHelper.Invoke(() => {
                var files = IOHelper.SearchFile(path, EditorFileName);
                Dispatcher.Invoke(() => {
                    Freeze(false);
                    OnFilesFound(files);
                });
            });
        }

        private void OnFilesFound(string[] files)
        {
            //Alert(string.Join("\n", files));

            if (files.Length > 0)
            {
                Freeze("Choose a file in another window");
                FilesListWindow win = new FilesListWindow(this, files, OnSelectFile) { Owner = this };
                win.Show();
                win.Focus();
                win.Closed += (s, e) => Freeze(false);
            }
            else
            {
                MessageHelper.Error($"There is no {EditorFileName}. Try choose another folder");
            }
        }

        private void OnSelectFile(string file)
        {
            Patcher = null;
            GC.Collect();
            //
            Patcher = new Patcher(file);

            Freeze("Loading...");
            ThreadHelper.Invoke(() => {
                Patcher.Load();
                Dispatcher.Invoke(() => {
                    Freeze(false);
                    OnFileLoaded();
                });
            });
        }

        private void OnFileLoaded()
        {
            Freeze("Detecting version...");
            ThreadHelper.Invoke(() => {
                Version version = Patcher.DetectVersion();

                Dispatcher.Invoke(() => {
                    Freeze(false);

                    if (version != null)
                    {
                        VersionsCombo.SelectedItem = version;
                        OnVersionDetected();
                    }
                    else
                    {
                        MessageHelper.Error("This version is not supported. Try another version of Unity");
                    }
                });
            });
        }

        private void OnVersionDetected()
        {
            Freeze("Detecting theme...");
            ThreadHelper.Invoke(() => {
                ThemeType theme = ThemeType.None;

                if (Patcher?.CurrentVersion != null)
                    theme = Patcher.DetectTheme(Patcher.CurrentVersion);

                Dispatcher.Invoke(() => {
                    Freeze(false);
                    SetThemeThumbs(theme);
                    if (theme == ThemeType.None)
                    {
                        MessageHelper.Error("Could not find signature");
                    }
                });
            });
        }

        private void OnBackupFilesFound(string[] files)
        {
            if (files.Length > 0)
            {
                Freeze("Choose a file in another window");
                FilesListWindow win = new FilesListWindow(this, files, RestoreBackup) { Owner = this };
                win.Show();
                win.Focus();
                win.Closed += (s, e) => Freeze(false);
            }
            else
            {
                MessageHelper.Error($"There is no backup files. Make a first backup!");
            }

            void RestoreBackup(string path)
            {
                try
                {
                    Patcher.RestoreBackup(path);
                    OnSelectFile(Patcher.FilePath);
                }
                catch (Exception ex)
                {
                    MessageHelper.ThrowException(ex);
                }
            }
        }

        // Thumbs
        public void ToggleThumb(Label thumb, bool state)
        {
            thumb.Style = (Style)Application.Current.FindResource(state ? "ThemeThumbSelected" : "ThemeThumb");
        }

        public void SetThemeThumbs(ThemeType skin)
        {
            ToggleThumb(LightTheme, skin == ThemeType.Light);
            ToggleThumb(DarkTheme, skin == ThemeType.Dark);
        }

        // Sections behaviour
        public void Navigate(Section section)
        {
            StartScreen.Visibility = section == Section.StartScreen ? Visibility.Visible : Visibility.Hidden;
            PatchScreen.Visibility = section == Section.PatchScreen ? Visibility.Visible : Visibility.Hidden;
        }

        public void Freeze(bool state)
        {
            ProcessState.Content = "Processing...";
            ProcessingScreen.Visibility = state ? Visibility.Visible : Visibility.Hidden;
            PatchScreen.IsEnabled = !state;
            PatchScreen.Effect = state ? new BlurEffect() { Radius = 10, KernelType = KernelType.Gaussian } : null;

            /*if (state)
            {
                ProcessingScreen.Visibility = Visibility.Visible;
                PatchScreen.IsEnabled = false;
                PatchScreen.Effect = new BlurEffect() { Radius = 10, KernelType = KernelType.Gaussian };
            }
            else
            {
                ProcessingScreen.Visibility = Visibility.Hidden;
                PatchScreen.IsEnabled = true;
                PatchScreen.Effect = null;
            }*/
        }

        public void Freeze(string text)
        {
            Freeze(true);
            ProcessState.Content = text;
        }
    }
}
