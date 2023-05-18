using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace mc_mods_sorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if (!Directory.Exists(".\\mc mods"))
            {
                Directory.CreateDirectory(".\\mc mods");
                Directory.CreateDirectory(".\\mc mods\\forge");
                Directory.CreateDirectory(".\\mc mods\\forge\\1.19.2");
            }
        }

        private string[] GetModsList(string version, string loader)
        {
            List<string> result = new List<string>();
            string modsPath = ".\\mc mods\\" + loader + "\\" + version;

            string[] directories = Directory.GetDirectories(modsPath);
            for (int i = 0; i < directories.Length; i++)
            {
                string directory = directories[i];
                string modName = directory.Substring(directory.LastIndexOf("\\") + 1);
                result.Add(modName);
            }

            return result.ToArray();
        }

        private bool ModIsEnabled(string modName)
        {
            string path = ".\\data.txt";
            if (!File.Exists(path)) File.Create(path);
            string[] data = File.ReadAllLines(path);

            /* DATA.TXT FILE FORMAT
             * line 0: last_used_version=[VERSION];last_uesd_loader=[LOADER]
             * every other line: mod name;enabled/disabled
             */

            for (int i = 1; i < data.Length; i++)
            {
                string dataItem = data[i];
                string[] splitData = dataItem.Split(";");
                string dataModName = splitData[0];
                string dataModEnabled = splitData[1];

                if (dataModName == modName)
                {
                    bool enabled = false;
                    if (dataModEnabled == "enabled") enabled = true;
                    return enabled;
                }
            }
            return false;
        }

        // used to set data for data.txt
        private void SetDataValue(string name, string value)
        {
            string path = "./data.txt";
            bool requiresNewData = false;
            if (!File.Exists(path))
            {
                File.Create(path);
                requiresNewData = true;
            }
            List<string> data = File.ReadAllLines(path).ToList<string>();
            if (requiresNewData) data.Add("last_used_version=;last_used_loader=");
            
            for (int i = 1; i < data.Count; i++)
            {
                string modName = data[i].Split(";")[0];
                if (modName == name)
                {
                    data[i] = name + ";" + value;
                    File.WriteAllLines(path, data);
                    return;
                }
            }

            data.Add(name + ";" + value);
            File.WriteAllLines(path, data);
            return;
        }

        public void UpdateModsList()
        {
            string[] mods = new string[] { };

            try
            {
                mods = GetModsList(gameVersionOption.SelectedItem as String, gameLoaderOption.SelectedItem as String);
            } catch
            {
                return;
            }

            if (gameVersionOption.Text == null || gameVersionOption.Text.Length == 0) return;
            if (gameLoaderOption.Text == null || gameLoaderOption.Text.Length == 0) return;

            ModListPanel.Children.Clear();
            List<Button> buttons = new List<Button> { };
            for (int i = 0; i < mods.Length; i++)
            {
                string mod = mods[i];
                Border border = new Border();
                Grid container = new Grid();
                Grid rightSideContainer = new Grid();
                TextBlock block = new TextBlock();
                Border btContainer = new Border();
                Button bt = new Button();
                Button delete = new Button();
                Button edit = new Button();
                
                border.Child = container;
                border.Margin = new Thickness(0, 5, 0, 0);
                border.MouseEnter += Border_MouseEnter;
                border.MouseLeave += Border_MouseLeave;
                border.BorderBrush = new BrushConverter().ConvertFrom("#222") as SolidColorBrush;
                border.BorderThickness = new Thickness(1);
                border.Padding = new Thickness(5);

                rightSideContainer.Children.Add(delete);
                rightSideContainer.Children.Add(edit);
                rightSideContainer.MouseEnter += RightSideContainer_MouseEnter;
                rightSideContainer.MouseLeave += RightSideContainer_MouseLeave;
                rightSideContainer.Width = 300;
                rightSideContainer.Height = 30;
                rightSideContainer.Background = new BrushConverter().ConvertFrom("#222") as SolidColorBrush;
                rightSideContainer.Margin = new Thickness(410, 0, 0, 0);

                delete.Content = "X";
                delete.Width = 30;
                delete.Height = 30;
                delete.FontSize = 20;
                delete.FontWeight = FontWeights.Bold;
                delete.Background = new BrushConverter().ConvertFrom("#222") as SolidColorBrush;
                delete.BorderThickness = new Thickness(0);
                delete.Foreground = new SolidColorBrush(Colors.Red);
                delete.Visibility = Visibility.Hidden;
                delete.HorizontalAlignment = HorizontalAlignment.Left;
                delete.Margin = new Thickness(50, 0, 0, 0);
                delete.Click += Delete_Click;

                edit.Content = "✎";
                edit.Width = 50;
                edit.Height = 30;
                edit.Background = new BrushConverter().ConvertFrom("#222") as SolidColorBrush;
                edit.Foreground = new SolidColorBrush(Colors.White);
                edit.BorderThickness = new Thickness(0);
                edit.Visibility = Visibility.Hidden;
                edit.FontWeight = FontWeights.Bold;
                edit.HorizontalAlignment = HorizontalAlignment.Left;
                edit.FontSize = 20;
                edit.Margin = new Thickness(0, 0, 100, 0);
                edit.Click += Edit_Click;

                container.Children.Add(block);
                container.Children.Add(btContainer);
                btContainer.Child = bt;
                container.Children.Add(rightSideContainer);

                container.Height = 30;

                block.FontSize = 20;
                block.FontWeight = FontWeights.Bold;
                block.Foreground = new SolidColorBrush(Colors.White);
                block.Text = mod;
                block.Text = mod;

                btContainer.Background = new BrushConverter().ConvertFrom("#333") as SolidColorBrush;
                btContainer.Width = 100;
                btContainer.Height = 30;

                bt.Height = 20;
                bt.Width = 20;
                bt.Margin = new Thickness(5);
                bt.BorderThickness = new Thickness(0);
                bt.PreviewMouseDown += SliderEvent;
                bt.Foreground = new SolidColorBrush(Colors.Red);
                bt.Foreground = new SolidColorBrush(Colors.Red);

                if (ModIsEnabled(mod))
                {
                    bt.Background = new SolidColorBrush(Colors.Green);
                    bt.HorizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    bt.Background = new SolidColorBrush(Colors.Red);
                    bt.HorizontalAlignment = HorizontalAlignment.Left;
                }

                ModListPanel.Children.Add(border);

                void SliderEvent(object sender, MouseButtonEventArgs e)
                {
                    Button button = bt;

                    if (ModIsEnabled(mod))
                    {
                        SetDataValue(mod, "disabled");
                        ModsFolder.Remove(mod, gameVersionOption.SelectedItem as String, gameLoaderOption.SelectedItem as String);

                        button.Background = new SolidColorBrush(Colors.Red);
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                    }
                    else
                    {
                        SetDataValue(mod, "enabled");
                        ModsFolder.Add(mod, gameVersionOption.SelectedItem as String, gameLoaderOption.SelectedItem as String);

                        button.Background = new SolidColorBrush(Colors.Green);
                        button.HorizontalAlignment = HorizontalAlignment.Right;
                    }
                }

                void RightSideContainer_MouseEnter(object sender, MouseEventArgs e)
                {
                    Button delBt = delete;
                    delBt.Visibility = Visibility.Visible;
                    edit.Visibility = Visibility.Visible;
                }

                void RightSideContainer_MouseLeave(object sender, MouseEventArgs e)
                {
                    Button delBt = delete;
                    delBt.Visibility = Visibility.Hidden;
                    edit.Visibility = Visibility.Hidden;
                }

                void Delete_Click(object sender, RoutedEventArgs e)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure?", "", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        Directory.Delete(".\\mc mods\\" + gameLoaderOption.SelectedItem as String + "\\" + gameVersionOption.SelectedItem as String + "\\" + mod,true);
                        UpdateModsList();
                    }
                    else return;
                }

                void Border_MouseEnter(object sender, MouseEventArgs e)
                {
                    border.BorderBrush = new SolidColorBrush(Colors.White);
                }

                void Border_MouseLeave(object sender, MouseEventArgs e)
                {
                    border.BorderBrush = new BrushConverter().ConvertFrom("#222") as SolidColorBrush;
                }

                void Edit_Click(object sender, RoutedEventArgs e)
                {
                    string modDir = @".\mc mods\" + gameLoaderOption.SelectedItem as String + "\\" + gameVersionOption.SelectedItem as String + "\\" + mod;
                    string[] modFiles = Directory.GetFiles(modDir);
                    editMod editModWindow = new editMod(modFiles,modDir);
                    editModWindow.Owner = this;
                    editModWindow.Show();
                }
            }
        }

        string? gameVersion = null;
        string? gameLoader = null;
        bool changingSelection = false;

        public void SetVersionDropdown(string version)
        {
            bool matched = false;
            for(int i = 0; i < gameVersionOption.Items.Count; i++)
            {
                string? item = gameVersionOption.Items[i] as string;
                
                if (item == version)
                {
                    gameVersionOption.SelectedIndex = i;
                    matched = true;
                    break;
                }
            }
            if(!matched && gameVersionOption.Items.Count > 0)
            {
                gameVersionOption.SelectedIndex = 0;
            }
        }

        private void SetLoaderDropdown(string loader)
        {
            for (int i = 0; i < gameLoaderOption.Items.Count; i++)
            {
                string? item = gameLoaderOption.Items[i] as string;

                if (item == loader)
                {
                    gameLoaderOption.SelectedIndex = i;
                    break;
                }
            }
        }

        private string GetLastUsedVersion()
        {
            List<string> data = File.ReadAllLines(".\\data.txt").ToList();
            string lastUsedVersion = data[0].Split(";")[0].Split("=")[1];
            return lastUsedVersion;
        }

        private string GetLastUsedLoader()
        {
            List<string> data = File.ReadAllLines(".\\data.txt").ToList();
            string lastUsedLoader = data[0].Split(";")[1].Split("=")[1];
            return lastUsedLoader;
        }

        private void InitializeDropdowns()
        {
            string lastUsedVersion = GetLastUsedVersion();
            string lastUsedLoader = GetLastUsedLoader();

            if (String.IsNullOrEmpty(lastUsedLoader))
            {
                lastUsedLoader = "forge";
            }

            string loaderPath = ".\\mc mods";
            string versionPath = ".\\mc mods\\" + lastUsedLoader;

            if(!Directory.Exists(versionPath)) Directory.CreateDirectory(versionPath); 

            string[] loaders = Directory.GetDirectories(loaderPath);
            string[] versions = Directory.GetDirectories(versionPath);

            foreach (string loader in loaders)
            {
                gameLoaderOption.Items.Add(loader.Substring(loader.LastIndexOf("\\")+1));
            }

            foreach (string version in versions)
            {
                gameVersionOption.Items.Add(version.Substring(version.LastIndexOf("\\")+1));
            }

            SetLoaderDropdown(lastUsedLoader);
            SetVersionDropdown(lastUsedVersion);
        }
        public void UpdateLoaderDropdown()
        {
            changingSelection = true;
            gameVersion = gameVersionOption.SelectedItem as string;
            gameLoader = gameLoaderOption.SelectedItem as string;

            gameLoaderOption.Items.Clear();

            string[] versions = Directory.GetDirectories(".\\mc mods");

            foreach (string version in versions)
            {
                gameLoaderOption.Items.Add(version.Substring(version.LastIndexOf("\\") + 1));
            }

            SetLoaderDropdown(gameLoader);

            changingSelection = false;
        }

        public void UpdateVersionDropdown()
        {
            changingSelection = true;
            gameVersion = gameVersionOption.SelectedItem as string;
            gameLoader = gameLoaderOption.SelectedItem as string;

            gameVersionOption.Items.Clear();

            string[] versions = Directory.GetDirectories(".\\mc mods\\" + gameLoader);

            foreach (string version in versions)
            {
                gameVersionOption.Items.Add(version.Substring(version.LastIndexOf("\\") + 1));
            }

            SetVersionDropdown(gameVersion);

            changingSelection = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowHeight = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualHeight;
            ModList.Height = windowHeight - 50;
        }

        private bool windowLoaded = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeDropdowns();
            UpdateModsList();
            windowLoaded = true;
        }


        private addMod? window1 = null;
        private void addNewMod_button_Click(object sender, RoutedEventArgs e)
        {
            if (window1 != null)
            {
                window1.Close();
                window1 = null;
            }

            window1 = new addMod();
            window1.Owner = this;
            window1.Show();
        }
        private void SetLastUsedVersion(string version)
        {
            string[] data = File.ReadAllLines(".\\data.txt");
            data[0] = "last_used_version=" + version + ";" + data[0].Split(";")[1];
            File.WriteAllLines(".\\data.txt",data);
        }

        private void SetLastUsedLoader(string loader)
        {
            string[] data = File.ReadAllLines(".\\data.txt");
            data[0] = data[0].Split(';')[0] + ";last_used_loader="+loader;
            File.WriteAllLines(".\\data.txt", data);
        }

        private void gameVersionOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!windowLoaded || changingSelection) { return; }
            SetLastUsedVersion(gameVersionOption.SelectedItem as String);
            UpdateModsList();
            
        }

        private void gameLoaderOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!windowLoaded || changingSelection) { return; }
            SetLastUsedLoader(gameLoaderOption.SelectedItem as String);
            SetLastUsedVersion(gameVersionOption.SelectedItem as String);
            UpdateVersionDropdown();
            UpdateModsList();
        }

        private void disableAllMods_Click(object sender, RoutedEventArgs e)
        {
            string[] modStatusList = File.ReadAllLines(".\\data.txt");

            for (int i = 1; i < modStatusList.Length; i++)
            {

                string modData = modStatusList[i];
                string status = "disabled";
                string modName = modData.Split(";")[0];
                string newData = modName + ";" + status;

                modStatusList[i] = newData;
            }

            File.WriteAllLines(".\\data.txt", modStatusList);
            UpdateModsList();
            ModsFolder.Clear();
        }

        private static class ModsFolder
        {
            static private string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+@"\.minecraft\mods";
            static public void Clear()
            {
                foreach(string mod in Directory.GetFiles(dir))
                {
                    File.Delete(mod);
                }
            }

            static private string[] GetModFiles(string modName, string gameVersion, string gameLoader)
            {
                string[] files = Directory.GetFiles(@".\mc mods\" + gameLoader + "\\" + gameVersion + "\\" + modName);
                return files;
            }

            static public void Remove(string modName, string gameVersion, string gameLoader)
            {
                string[] files = GetModFiles(modName, gameVersion, gameLoader);

                foreach(string file in Directory.GetFiles(dir))
                {
                    for(int i = 0; i < files.Length; i++)
                    {
                        // refers to the copied files in directory ".\mc mods"
                        string fileCopy = files[i].Substring(files[i].LastIndexOf("\\") + 1);

                        // refers to files inside .minecraft\mods
                        string activeFile = file.Substring(file.LastIndexOf("\\") + 1);

                        if(activeFile == fileCopy)
                        {
                            File.Delete(file);
                            break;
                        }
                    }
                }
            }

            static public void Add(string modName, string gameVersion, string gameLoader)
            {
                string[] files = GetModFiles(modName, gameVersion, gameLoader);

                foreach (string file in files)
                {
                    File.Copy(file, dir + "\\" + file.Substring(file.LastIndexOf("\\")+1), true);
                }
            }
        }
    }
}