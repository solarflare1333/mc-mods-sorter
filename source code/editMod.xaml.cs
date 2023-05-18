using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
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

namespace mc_mods_sorter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class editMod : Window
    {
        public string[] files { get; private set; }
        public string modDir { get; private set; }
        public editMod(string[] modFiles, string modDirectory)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            files = modFiles;
            modDir = modDirectory;
            UpdateFileList();
        }
        
        private void UpdateFileList()
        {
            PrimaryContainer.Children.Clear();
            foreach (string file in files)
            {
                string fileName = file.Substring(file.LastIndexOf("\\") + 1);

                Border border = new Border();
                Grid grid = new Grid();
                TextBlock text = new TextBlock();
                Button remove = new Button();

                border.BorderBrush = new ColorConverter().ConvertFrom("#222") as SolidColorBrush;
                border.BorderThickness = new Thickness(1);
                border.Padding = new Thickness(5);
                border.MouseEnter += Grid_MouseEnter;
                border.MouseLeave += Grid_MouseLeave;
                border.Margin = new Thickness(5, 0, 0, 0);

                grid.Height = 30;

                text.Text = fileName;
                text.Foreground = new SolidColorBrush(Colors.White);
                text.FontWeight = FontWeights.Bold;
                text.FontSize = 15;
                text.HorizontalAlignment = HorizontalAlignment.Left;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.Width = 200;

                remove.Background = new ColorConverter().ConvertFrom("#222") as SolidColorBrush;
                remove.Foreground = new SolidColorBrush(Colors.Red);
                remove.Width = 30;
                remove.Height = 30;
                remove.FontSize = 20;
                remove.Content = "X";
                remove.FontWeight = FontWeights.Bold;
                remove.BorderThickness = new Thickness(0);
                remove.Click += Remove_Click;
                remove.Margin = new Thickness(10, 0, 0, 0);
                remove.HorizontalAlignment = HorizontalAlignment.Right;

                border.Child = grid;
                grid.Children.Add(text);
                grid.Children.Add(remove);
                PrimaryContainer.Children.Add(border);

                void Remove_Click(object sender, RoutedEventArgs e)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure?", "", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        File.Delete(modDir+"\\"+fileName);
                        files = Directory.GetFiles(modDir);
                        UpdateFileList();
                    }
                }

                void Grid_MouseEnter(object sender, MouseEventArgs e)
                {
                    border.BorderBrush = new SolidColorBrush(Colors.White);
                }

                void Grid_MouseLeave(object sender, MouseEventArgs e)
                {
                    border.BorderBrush = new ColorConverter().ConvertFrom("#222") as SolidColorBrush;
                }
            }

            // places a "+" at the end of the list to allow adding new files
            Border border1 = new Border();
            Grid grid1 = new Grid();
            Button button1 = new Button();

            border1.MouseEnter += Grid1_MouseEnter;
            border1.MouseLeave += Grid1_MouseLeave;
            border1.BorderThickness = new Thickness(1);
            border1.BorderBrush = new ColorConverter().ConvertFrom("#222") as SolidColorBrush;
            border1.Padding = new Thickness(5);

            grid1.Height = 30;

            button1.Content = "+";
            button1.Foreground = new SolidColorBrush(Colors.Green);
            button1.HorizontalAlignment = HorizontalAlignment.Left;
            button1.BorderThickness = new Thickness(0);
            button1.FontWeight = FontWeights.Bold;
            button1.FontSize = 20;
            button1.Width = 30;
            button1.Height = 30;
            button1.Background = new ColorConverter().ConvertFrom("#222") as SolidColorBrush;
            button1.Click += Button1_Click;

            border1.Child = grid1;
            grid1.Children.Add(button1);
            PrimaryContainer.Children.Add(border1);


            void Grid1_MouseEnter(object sender, MouseEventArgs e)
            {
                border1.BorderBrush = new SolidColorBrush(Colors.White);
            }

            void Grid1_MouseLeave(object sender, MouseEventArgs e)
            {
                border1.BorderBrush = new ColorConverter().ConvertFrom("#222") as SolidColorBrush;
            }

            void Button1_Click(object sender, RoutedEventArgs e)
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "All Files *.jar | *.jar";
                open.Multiselect = true;
                open.Title = "Open Text Files";

                open.ShowDialog();
                string[] addedFiles = open.FileNames;
                List<string> newFileList = files.ToList<string>();

                foreach(string addedFile in addedFiles)
                {
                    string addedfile_Name = addedFile.Substring(addedFile.LastIndexOf("\\")+1);

                    // does not skip already existing files. Instead, seems to duplicate them.
                    bool fileExists = false;
                    foreach(string file in files)
                    {
                        string fileName = file.Substring(file.LastIndexOf("\\")+1);
                        if (addedfile_Name == fileName)
                        {
                            fileExists = true;
                            break;
                        }
                    }

                    if (!fileExists) newFileList.Add(addedFile);
                    File.Copy(addedFile, modDir + "\\" + addedfile_Name, true);
                }

                files = newFileList.ToArray();

                UpdateFileList();
            }
        }

        private void ClearAllBt_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                for(var i = 0; i < files.Length; i++)
                {
                    string file = files[i];

                    File.Delete(file);
                }
                files = new string[] { };
                UpdateFileList();
            }
        }
    }
}
