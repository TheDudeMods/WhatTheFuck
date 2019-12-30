using Microsoft.Win32;
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

using System.IO;
using WhatTheBlam;


namespace WhatTheFuck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Test code, disregard
            OpenFileDialog fileDialog = new OpenFileDialog();
            if(fileDialog.ShowDialog() == true)
            {
                Stream str = File.Open(fileDialog.FileName, FileMode.Open);
                MapHeader mh = HeaderReader.ReadHeader(str);

                ConsoleOutput.AppendText("Read in the map file!\n");  
            }
        }
    }
}
