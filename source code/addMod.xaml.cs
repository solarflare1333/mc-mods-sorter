using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.IO;

namespace mc_mods_sorter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class addMod : Window
    {
        private MainWindow win;
        public addMod()
        {
            win = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private string[] mods = new string[] { };

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Files *.jar | *.jar";
            open.Multiselect = true;
            open.Title = "Open Text Files";

            open.ShowDialog();

            mods = open.FileNames;
            
            for(int i = 0; i < mods.Length; i++)
            {
                string fileName = mods[i].Substring(mods[i].LastIndexOf("\\"));
                TextBlock block = new TextBlock();
                SolidColorBrush white = new SolidColorBrush(Colors.White);
                block.Foreground = white;
                block.Text = fileName;
                importedModsList.Children.Add(block);
            }

        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            string modName = modNameInput.Text;
            string gameVersion = gameVersionInput.Text;
            string gameLoader = gameLoaderInput.Text;

            string messageBoxText = "";
            string caption = "";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            if (modName == null || modName == "")
            {
                messageBoxText = "Please type the mod's name";
                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                return;
            }

            if(gameVersion == null || gameVersion == "")
            {
                messageBoxText = "Please type the mod's game version";
                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                return;
            }

            if(gameLoader == null || gameLoader == "")
            {
                messageBoxText = "Please type the mod's game loader";
                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                return;
            }

            if(!Directory.Exists("./mc mods"))
            {
                Directory.CreateDirectory("./mc mods");
            }

            if(!Directory.Exists("./mc mods/"+gameLoader))
            {
                Directory.CreateDirectory("./mc mods/" + gameLoader);
            }

            if(!Directory.Exists("./mc mods/"+gameLoader+"/"+gameVersion))
            {
                Directory.CreateDirectory("./mc mods/" + gameLoader + "/" + gameVersion);
            }

            string fullDirectory = "./mc mods/" + gameLoader + "/" + gameVersion + "/" + modName;
            if (Directory.Exists(fullDirectory))
            {
                messageBoxText = "Mod already exists";
                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                return;
            } else
            {
                Directory.CreateDirectory(fullDirectory);

                for(int i = 0; i < mods.Length; i++) 
                {
                    string fileEndName = mods[i].Substring(mods[i].LastIndexOf("\\"));
                    File.Copy(mods[i], fullDirectory + fileEndName);
                }

                ((MainWindow)this.Owner).UpdateModsList();
                ((MainWindow)this.Owner).UpdateVersionDropdown();
                ((MainWindow)this.Owner).UpdateLoaderDropdown();
                this.Close();
            }
        }
    }
}
