using System;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace UnityDarkSkin.App.Core
{
    public class IOHelper
    {
        public static void OpenFolderDialog(string path, Action<string> onDone = null, Action onCancel = null)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = path,
            };

            OpenDialog(dialog, onDone, onCancel);
        }

        public static void OpenFileDialog(string path, Action<string> onDone = null, Action onCancel = null)
        {
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = path,
            };

            OpenDialog(dialog, onDone, onCancel);
        }

        public static void OpenDialog(CommonOpenFileDialog dialog, Action<string> onDone, Action onCancel = null)
        {
            var result = dialog.ShowDialog();

            switch (result)
            {
                case CommonFileDialogResult.Ok:
                    onDone?.Invoke(dialog.FileName);
                    break;
                case CommonFileDialogResult.Cancel:
                    onCancel?.Invoke();
                    break;
            }
        }

        public static void SearchFile(string diretcory, string file, bool recursive = true)
        {
            // TODO
        }
    }
}
