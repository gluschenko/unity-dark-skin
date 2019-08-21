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

        private string StartPath = @"C:\Program Files\Unity";
        private const string UnityName = "Unity.exe";

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
            Manager = new DataManager<ApplicationData>("prefs.json");
            LoadData();
            Closed += (sender, args) => SaveData();

            //

            Navigate(Section.StartScreen);

            //

            DirectoryTextBox.MouseDoubleClick += (sender, args) => ChooseDirectoryButton_Click(sender, null);

            foreach (var version in Versions.Get())
            {
                VersionsCombo.Items.Add(version);
            }

            VersionsCombo.SelectionChanged += (s, a) => {
                CurrentVersion = (Version)VersionsCombo.SelectedItem;
                Alert(CurrentVersion.ToString());
            };
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
            IOHelper.OpenFolderDialog(StartPath, (path) => {
                Navigate(Section.PatchScreen);
                DirectoryTextBox.Text = path;
            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Navigate(Section.StartScreen);
        }

        private void ChooseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            IOHelper.OpenFolderDialog(DirectoryTextBox.Text, (path) => {
                DirectoryTextBox.Text = path;
            });
        }


        // Sections behaviour
        public void Navigate(Section section)
        {
            StartScreen.Visibility = section == Section.StartScreen ? Visibility.Visible : Visibility.Hidden;
            PatchScreen.Visibility = section == Section.PatchScreen ? Visibility.Visible : Visibility.Hidden;
        }

        // Alert windows
        public static void Alert(string text, string title = "Alert") => MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Information);
        public static void Warning(string text, string title = "Warning") => MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        public static void Error(string text, string title = "Error") => MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        // Safer exception throwing (useful for live debugging)
        public static void ThrowException(Exception exception) => Error(exception.ToString(), exception.GetType().Name);
    }
}
