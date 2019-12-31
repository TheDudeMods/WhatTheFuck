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
            //Test code, please ignore
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                //open file dialog to open the map
                Stream str = File.Open(fileDialog.FileName, FileMode.Open);

                //Get the header
                MapHeader mh = HeaderReader.ReadHeader(str);

                ///Break out the sections of the file in order to avoid seeks and shit
                ///

                //First get the debug section. What the fuck is this for anyways?
                int debug_offset = mh.section_offsets[0].ToInt() + mh.section_bounds[0].offset.ToInt();
                int debug_size = mh.section_bounds[0].size.ToInt();
                byte[] debug_section = new byte[debug_size];

                if(str.Position != debug_offset)
                    str.Seek(debug_offset, SeekOrigin.Begin);

                str.Read(debug_section, 0, debug_size);


                //Then get the resources section. Images and models maybe?
                int resource_offset = mh.section_offsets[1].ToInt() + mh.section_bounds[1].offset.ToInt();
                int resource_size = mh.section_bounds[1].size.ToInt();
                byte[] resource_section = new byte[resource_size];

                if (str.Position != resource_offset)
                    str.Seek(resource_offset, SeekOrigin.Begin);

                str.Read(resource_section, 0, resource_size);

                //Get the actual tags
                int tag_offset = mh.section_offsets[2].ToInt() + mh.section_bounds[2].offset.ToInt();
                int tag_size = mh.section_bounds[2].size.ToInt();
                byte[] tag_section = new byte[tag_size];

                long pos = str.Position;

                if(str.Position != tag_offset)
                    str.Seek(tag_offset, SeekOrigin.Begin);
                str.Read(tag_section, 0, tag_size);

                //Lastly get the localization data
                //But I dont need it now so fuck it

                ConsoleOutput.AppendText("Read in the map file!\n");  
            }
        }
    }
}
