using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace StarshipTheory.PatchInjector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PatchInjector injector;
        private bool injectionError = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            injector = new PatchInjector();
            injector.Error += Injector_Error;

            file_executable.Filename = Properties.Settings.Default.last_path;
            file_executable_TextChanged(null, null);
        }

        private void Injector_Error(object sender, PatchInjector.StringEventArgs e)
        {
            MessageBox.Show("Failed to patch game:\n" + e.Text, "Patching Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            injectionError = true;
        }

        private void file_executable_TextChanged(object sender, Controls.FileTextboxChangedEventArgs e)
        {
            btnPatch.IsEnabled = false;
            String AppDir = null;

            if (String.IsNullOrEmpty(file_executable.Filename))
                return;

            if (File.Exists(file_executable.Filename))
            {
                if (Directory.Exists(Path.Combine(new FileInfo(file_executable.Filename).Directory.FullName, "StarshipTheory_Data")))
                    AppDir = new FileInfo(file_executable.Filename).Directory.FullName;
            }
            else if (Directory.Exists(file_executable.Filename) && Directory.Exists(Path.Combine(file_executable.Filename, "StarshipTheory_Data")))
                AppDir = file_executable.Filename;

            if (String.IsNullOrEmpty(AppDir))
            {
                MessageBox.Show("Unable to find StarshipTheory_Data directory in the path specified", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                file_executable.Focus();
            }
            else
            {
                btnPatch.IsEnabled = true;
                btnRestoreGame.IsEnabled = Directory.Exists(PathZ.Combine(AppDir, "StarshipTheory_Data", "Managed", "@BACKUP"));
                injector.ApplicationPath = AppDir;
            }
        }

        private void btnPatch_Click(object sender, RoutedEventArgs e)
        {
            String AppDir = null;
            if (File.Exists(file_executable.Filename))
            {
                if (Directory.Exists(Path.Combine(new FileInfo(file_executable.Filename).Directory.FullName, "StarshipTheory_Data")))
                    AppDir = new FileInfo(file_executable.Filename).Directory.FullName;
            }
            else if (Directory.Exists(file_executable.Filename) && Directory.Exists(Path.Combine(file_executable.Filename, "StarshipTheory_Data")))
                AppDir = file_executable.Filename;

            if (!String.IsNullOrEmpty(AppDir))
            {
                injectionError = false;
                btnPatch.IsEnabled = false;
                btnPatch.Content = "Patching...";

                System.Threading.Thread T = new System.Threading.Thread(new System.Threading.ThreadStart(Patch));
                T.Name = "PatchInjector - Worker Thread";
                T.Start();
            }
        }

        private void Patch()
        {
            injector.RestoreFiles();
            injector.BackupFiles();

            int loaded = injector.LoadPatches(System.Reflection.Assembly.GetExecutingAssembly());
            Console.WriteLine("Loaded " + loaded + " patches");
            injector.InjectPatches();

            btnPatch.Dispatcher.Invoke(new Action(() => {
                Properties.Settings.Default.last_path = injector.ApplicationPath;
                Properties.Settings.Default.Save();

                if (!Directory.Exists(Path.Combine(injector.ApplicationPath, "Mods")))
                    Directory.CreateDirectory(Path.Combine(injector.ApplicationPath, "Mods"));

                if(!injectionError)
                    MessageBox.Show("All patches has been applied!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    if (MessageBox.Show("Patch injection failed.\nWould you like to restore the game to it's original state?", "Patching Failed", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        btnRestoreGame_Click(null, null);
                }

                btnPatch.IsEnabled = true;
                btnPatch.Content = "Patch";
                btnRestoreGame.IsEnabled = Directory.Exists(PathZ.Combine(injector.ApplicationPath, "StarshipTheory_Data", "Managed", "@BACKUP"));
            }));
        }

        private void btnRestoreGame_Click(object sender, RoutedEventArgs e)
        {
            injector.RestoreFiles();
            btnRestoreGame.Dispatcher.Invoke(new Action(() => {
                btnRestoreGame.IsEnabled = Directory.Exists(PathZ.Combine(injector.ApplicationPath, "StarshipTheory_Data", "Managed", "@BACKUP"));
                MessageBox.Show("All files has been restored to their original state", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }));
        }
    }
}
