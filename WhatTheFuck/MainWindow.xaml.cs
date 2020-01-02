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
                int debug_offset = (int)mh.section_offsets[0] + (int)mh.section_bounds[0].offset;
                int debug_size = (int)mh.section_bounds[0].size;
                byte[] debug_section = new byte[debug_size];

                if(str.Position != debug_offset)
                    str.Seek(debug_offset, SeekOrigin.Begin);

                str.Read(debug_section, 0, debug_size);


                //Then get the resources section. Images and models maybe? and strings probably!
                int resource_offset = (int)mh.section_offsets[1] + (int)mh.section_bounds[1].offset;
                int resource_size = (int)mh.section_bounds[1].size;
                byte[] resource_section = new byte[resource_size];

                if (str.Position != resource_offset)
                    str.Seek(resource_offset, SeekOrigin.Begin);

                str.Read(resource_section, 0, resource_size);

                //Get the actual tags
                int tag_offset = (int)mh.section_offsets[2] + (int)mh.section_bounds[2].offset;
                int tag_size = (int)mh.section_bounds[2].size;
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
