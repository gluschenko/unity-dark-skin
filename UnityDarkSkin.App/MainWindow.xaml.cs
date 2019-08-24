using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnityDarkSkin.App.Core;
using UnityDarkSkin.Lib;
using Version = UnityDarkSkin.Lib.Version;

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

        private Version CurrentVersion;

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

            foreach (var version in Versions.Get())
            {
                VersionsCombo.Items.Add(version);
            }

            VersionsCombo.SelectionChanged += (sender, args) => {
                CurrentVersion = (Version)VersionsCombo.SelectedItem;
                Alert(CurrentVersion.ToString());
            };

            // Resets thumbs by default
            ToggleThumb(LightTheme, false);
            ToggleThumb(DarkTheme, false);
        }

        // Load & Save methods

        public void LoadData()
        {
            Data = Manager.Load(ThrowException);
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
            Manager.Save(Data, ThrowException);
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

        // Async callbacks
        private void OnFolderChosen(string path)
        {
            Navigate(Section.PatchScreen);

            DirectoryTextBox.Text = path;

            Freeze(true);
            ThreadHelper.Invoke(() => {
                var files = IOHelper.SearchFile(path, EditorFileName);
                Dispatcher.Invoke(() => {
                    OnFilesFound(files);
                    Freeze(false);
                });
            });
        }

        private void OnFilesFound(string[] files)
        {
            //Alert(string.Join("\n", files));

            FilesListWindow win = new FilesListWindow(this, files) { Owner = this };
            win.Show();
            win.Focus();
        }

        // Thumbs
        public void ToggleThumb(Label thumb, bool state)
        {
            thumb.Style = Application.Current.FindResource(state ? "ThemeThumbSelected" : "ThemeThumb") as Style;
        }

        // Sections behaviour
        public void Navigate(Section section)
        {
            StartScreen.Visibility = section == Section.StartScreen ? Visibility.Visible : Visibility.Hidden;
            PatchScreen.Visibility = section == Section.PatchScreen ? Visibility.Visible : Visibility.Hidden;
        }

        public void Freeze(bool state)
        {
            if (state)
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
            }
        }

        // Alert windows
        public void Alert(string text, string title = "Alert") => MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Information);
        public void Warning(string text, string title = "Warning") => MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        public void Error(string text, string title = "Error") => MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        // Safer exception throwing (useful for live debugging)
        public void ThrowException(Exception exception) => Error(exception.ToString(), exception.GetType().Name);
    }
}
