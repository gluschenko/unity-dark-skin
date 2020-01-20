using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace UnityDarkSkin.Core
{
    public class IOHelper
    {
        public static void OpenFolderDialog(string initialDirectory, Action<string> onDone = null, Action onCancel = null)
        {
            using (var dialog = new CommonOpenFileDialog { InitialDirectory = initialDirectory, IsFolderPicker = true }) 
            {
                OpenDialog(dialog, onDone, onCancel);
            }
        }

        public static void OpenFileDialog(string initialDirectory, Action<string> onDone = null, Action onCancel = null)
        {
            using (var dialog = new CommonOpenFileDialog { InitialDirectory = initialDirectory }) 
            {
                OpenDialog(dialog, onDone, onCancel);
            }
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

        //

        public static string[] SearchFile(string directory, string file_name, bool recursive = true, bool contains_name = false)
        {
            var files = new List<string>();
            InternalSearchFile(ref files, directory, file_name, recursive, contains_name);
            return files.ToArray();
        }

        private static void InternalSearchFile(ref List<string> files, string directory, string file_name, bool recursive, bool contains_name)
        {
            if (Directory.Exists(directory))
            {
                string[] sub_dirs = new string[0];
                string[] matchingFiles = new string[0];

                try
                {
                    sub_dirs = Directory.GetDirectories(directory);
                    //
                    Func<string, bool> search;
                    if (contains_name)
                        search = (p) => Path.GetFileName(p).Contains(file_name);
                    else
                        search = (p) => Path.GetFileName(p).Equals(file_name);

                    matchingFiles = Directory.GetFiles(directory).Where(search).ToArray();
                    files.AddRange(matchingFiles);
                }
                catch {
                    // Don't care
                }
                //

                if (recursive) 
                {
                    foreach (string dir in sub_dirs)
                    {
                        InternalSearchFile(ref files, dir, file_name, recursive, contains_name);
                    }
                }
            }
        }
    }
}
