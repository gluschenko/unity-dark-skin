using System;
using System.Windows;
using System.Windows.Controls;

/*
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
*/

namespace UnityDarkSkin.App
{
    public partial class FilesListWindow : Window
    {
        public FilesListWindow(MainWindow Main, string[] files, Action<string> onSelect)
        {
            InitializeComponent();

            FilesList.Children.Clear();
            foreach (string file in files)
            {
                ComboBoxItem item = new ComboBoxItem() { Content = file, ToolTip = file };
                item.MouseDoubleClick += (sender, args) => {
                    onSelect?.Invoke(file);
                    //
                    Close();
                    Main.Focus();
                };

                FilesList.Children.Add(item);
            }
        }
    }
}
