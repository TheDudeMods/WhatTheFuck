using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhatTheBlam
{

    /// <summary>
    /// Information regarding how to deconstruct the map tags
    /// </summary>
    public class MapHeader
    {
        
        public uint_4 head_signiture;    //not really a long, but 4 bytes either being head or daeh (1684104552 or 1751474532) (big endian or little endian)
                                                            //0x0000    little endian .map file data offsets, useful for debugging and exploration (and masochists that like to edit in the raw)
        public uint_4 engine_version;                       //0x0004
        public uint_4 mapfile_length;                       //0x0008
        public uint_4 mapfile_compressed_length;            //0x000C

        public uint_8 tags_header_address;                  //0x0010

        public uint_4 memory_buffer_offset;                 //0x0018
        public uint_4 memory_buffer_size;                   //0x001C

        public char[] source_file = new char[256];          //0x0020

        public char[] build = new char[32];                 //0x0120

        public byte scenario_type;                          //0x0140    think these should actually be a Int16
        public byte scenario_short;                         //0x0141

        public byte scenario_load_type;                     //0x0142    maybe the same with these two as  well
        public byte scenario_load_type_short;               //0x0143

        public char unknown1;                               //0x0144
        public byte tracked_build;                          //0x0145
        public char unknown2;                               //0x0146
        public char unknown3;                               //0x0147

        public uint_4 unknown4;                             //0x0148
        public uint_4 unknown5;                             //0x014C
        public uint_4 unknown6;                             //0x0150
        public uint_4 unknown7;                             //0x0154
        public uint_4 unknown8;                             //0x0158

        public uint_4 string_id_count;                      //0x015C
        public uint_4 string_ids_buffer_size;               //0x0160
        public uint_4 string_id_indices_offset;             //0x0164
        public uint_4 string_ids_buffer_offset;             //0x0168

        public uint_4 unknown9;                             //0x016C
        
        public uint_8 timestamp;                            //0x0170
        public uint_8 mainmenu_timestamp;                   //0x0178
        public uint_8 shared_timestamp;                     //0x0180
        public uint_8 campaign_timestamp;                   //0x0188
        public uint_8 multiplayer_timestamp;                //0x0190

        public char[] name = new char[32];                  //0x0198

        public uint_4 unknown10;                            //0x01B8

        public char[] scenario_path = new char[256];        //0x01BC

        public uint_4 minor_version;                        //0x02BC

        public uint_4 tag_name_count;                       //0x02C0
        public uint_4 tag_names_buffer_offset;              //0x02C4
        public uint_4 tag_names_buffer_size;                //0x02C8
        public uint_4 tag_name_indices_offset;              //0x02CC

        public uint_4 checksum;                             //0x02D0


        public uint_4 unknown11;                            //0x02D4
        public uint_4 unknown12;                            //0x02D8
        public uint_4 unknown13;                            //0x02DC
        public uint_4 unknown14;                            //0x02E0
        public uint_4 unknown15;                            //0x02E4
        public uint_4 unknown16;                            //0x02E8
        public uint_4 unknown17;                            //0x02EC
        public uint_4 unknown18;                            //0x02F0
        public uint_4 unknown19;                            //0x02F4

        public uint_8 virtual_base_address;

        public uint_4 xdx_version;

        public uint_4 unknown20;

        //Not totally sure how these get used yet but will investigate further
        //Shamelessly stole these from zeta tool
        //Credit where credit is due https://github.com/camden-smallwood/zeta
        public UInt16 tag_post_link_buffer;
        public UInt16 tag_language_dependent_read_only_buffer;
        public UInt16 tag_language_dependent_read_write_buffer;
        public UInt16 tag_language_neutral_read_write_buffer;
        public UInt16 tag_language_neutral_write_combined_buffer;
        public UInt16 tag_language_neutral_read_only_buffer;

        public uint_8 unknown21;
        public uint_8 unknown22;

        public uint_4[] sha1_a = new uint_4[5];
        public uint_4[] sha1_b = new uint_4[5];
        public uint_4[] sha1_c = new uint_4[5];

        public uint_4[] rsa = new uint_4[64];

        public uint_4[] section_offsets = new uint_4[4];


        public file_bounds[] section_bounds = new file_bounds[4];

        public uint_4[] guid = new uint_4[4];

        public uint_4[] unknown23 = new uint_4[0x26C2];

        public uint_4 foot_signiture; //not really a long, but 4 bytes either being foot or toof (1953460070 or 1718579060) (big endian or little endian)

    }


    //A helper class to read in the header data from a .map file
    public static class HeaderReader
    {
        /// <summary>
        /// Extracts a subsection from a byte array.
        /// </summary>
        /// <param name="array">The array to extract the bytes from.</param>
        /// <param name="offset">The offset to start at in the array.</param>
        /// <param name="count">The number of bytes to extract.</param>
        /// <returns>An array containing the bytes extracted</returns>
        public static byte[] ExtractBytes(byte[] array, int offset, int count, bool reverse = false)
        {
            if (offset > array.Length || offset + count > array.Length)
            {
                return null;
            }
            byte[] tmp = new byte[count];
            for (int i = 0; i < count; ++i)
            {
                tmp[i] = array[offset + i];
            }

            if (reverse) Array.Reverse(tmp);
            return tmp;
        }

        /// <summary>
        /// Gets the header section of a halo reach map file
        /// </summary>
        /// <param name="s">The stream to read the header from</param>
        /// <returns>A fully constructed MapHeader</returns>
        public static MapHeader ReadHeader(Stream s)
        {
            MapHeader mh = new MapHeader();
            byte[] raw = new byte[0xA000];  //Halo reach header size is 0xA000
            s.Read(raw, 0, 0xA000);
            bool bigEndian = true;

            string head_tag = Encoding.ASCII.GetString(raw, 0, 4);
            if (head_tag == "daeh")
            {
                bigEndian = false;
            }
            else
            {
                if (head_tag != "head")
                {
                    throw new Exception("Don't recognize the map file!");
                }
            }


            //forgot to do the endianess checks... fuuuuuuuuuuuuuuuuhhhhhhhhhhhhkkkkkkkkk
            mh.head_signiture = ExtractBytes(raw, 0x0000, 0x0004);

            mh.engine_version = ExtractBytes(raw, 0x0004, 0x004, bigEndian);    //quick edit of ExtractBytes allows for easy reversing for endianness!

            mh.mapfile_length = ExtractBytes(raw, 0x0008, 0x0004, bigEndian);
            mh.mapfile_compressed_length = ExtractBytes(raw, 0x00C, 0x0004, bigEndian);

            mh.tags_header_address = ExtractBytes(raw, 0x0010, 0x0008, bigEndian);

            mh.memory_buffer_offset = ExtractBytes(raw, 0x0018, 0x0004, bigEndian);
            mh.memory_buffer_size = ExtractBytes(raw, 0x001C, 0x0004, bigEndian);

            mh.source_file = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x0020, 0x0100));
            mh.build = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x0120, 0x0020));

            mh.scenario_load_type = ExtractBytes(raw, 0x0140, 0x001)[0];
            mh.scenario_load_type_short = ExtractBytes(raw, 0x0141, 0x001)[0];

            mh.scenario_load_type = ExtractBytes(raw, 0x0142, 0x001)[0];
            mh.scenario_load_type_short = ExtractBytes(raw, 0x0143, 0x001)[0];


            mh.unknown1 = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x0144, 0x0001))[0];
            mh.tracked_build = ExtractBytes(raw, 0x0145, 0x001)[0];
            mh.unknown2 = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x0146, 0x001))[0];
            mh.unknown3 = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x0147, 0x001))[0];

            mh.unknown4 = ExtractBytes(raw, 0x0148, 0x004, bigEndian);
            mh.unknown5 = ExtractBytes(raw, 0x014C, 0x004, bigEndian);
            mh.unknown6 = ExtractBytes(raw, 0x0150, 0x004, bigEndian);
            mh.unknown7 = ExtractBytes(raw, 0x0154, 0x004, bigEndian);
            mh.unknown8 = ExtractBytes(raw, 0x0158, 0x004, bigEndian);

            mh.string_id_count = ExtractBytes(raw, 0x015C, 0x004, bigEndian);
            mh.string_ids_buffer_size = ExtractBytes(raw, 0x0160, 0x004, bigEndian);
            mh.string_id_indices_offset = ExtractBytes(raw, 0x0164, 0x004, bigEndian);
            mh.string_ids_buffer_offset = ExtractBytes(raw, 0x0168, 0x004, bigEndian);

            mh.unknown9 = ExtractBytes(raw, 0x016C, 0x004, bigEndian);

            mh.timestamp = ExtractBytes(raw, 0x0170, 0x008, bigEndian);
            mh.mainmenu_timestamp = ExtractBytes(raw, 0x0178, 0x008, bigEndian);
            mh.shared_timestamp = ExtractBytes(raw, 0x0180, 0x008, bigEndian);
            mh.campaign_timestamp = ExtractBytes(raw, 0x0188, 0x008, bigEndian);
            mh.multiplayer_timestamp = ExtractBytes(raw, 0x0190, 0x008, bigEndian);

            mh.name = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x0198, 0x0020));

            mh.unknown10 = ExtractBytes(raw, 0x01B8, 0x004, bigEndian);

            mh.scenario_path = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x01BC, 0x0100));

            mh.minor_version = ExtractBytes(raw, 0x02BC, 0x004, bigEndian);

            mh.tag_name_count = ExtractBytes(raw, 0x02C0, 0x004, bigEndian);
            mh.tag_names_buffer_offset = ExtractBytes(raw, 0x02C4, 0x004, bigEndian);
            mh.tag_names_buffer_size = ExtractBytes(raw, 0x02C8, 0x004, bigEndian);
            mh.tag_name_indices_offset = ExtractBytes(raw, 0x02CC, 0x004, bigEndian);

            mh.checksum = ExtractBytes(raw, 0x02D0, 0x004, bigEndian);

            mh.unknown11 = ExtractBytes(raw, 0x02D4, 0x004, bigEndian);
            mh.unknown12 = ExtractBytes(raw, 0x02D8, 0x004, bigEndian);
            mh.unknown13 = ExtractBytes(raw, 0x02DC, 0x004, bigEndian);
            mh.unknown14 = ExtractBytes(raw, 0x02E0, 0x004, bigEndian);
            mh.unknown15 = ExtractBytes(raw, 0x02E4, 0x004, bigEndian);
            mh.unknown16 = ExtractBytes(raw, 0x02E8, 0x004, bigEndian);
            mh.unknown17 = ExtractBytes(raw, 0x02EC, 0x004, bigEndian);
            mh.unknown18 = ExtractBytes(raw, 0x02F0, 0x004, bigEndian);
            mh.unknown19 = ExtractBytes(raw, 0x02F4, 0x004, bigEndian);

            mh.virtual_base_address = ExtractBytes(raw, 0x02F8, 0x008, bigEndian);

            mh.xdx_version = ExtractBytes(raw, 0x0300, 0x004, bigEndian);

            mh.unknown20 = ExtractBytes(raw, 0x0304, 0x004, bigEndian);

            mh.tag_post_link_buffer = BitConverter.ToUInt16(raw, 0x0308);                       //need extra stuff for endian swap
            mh.tag_language_dependent_read_only_buffer = BitConverter.ToUInt16(raw, 0x0318);
            mh.tag_language_dependent_read_write_buffer = BitConverter.ToUInt16(raw, 0x0328);

            mh.tag_language_neutral_read_write_buffer = BitConverter.ToUInt16(raw, 0x0338);
            mh.tag_language_neutral_write_combined_buffer = BitConverter.ToUInt16(raw, 0x0348);
            mh.tag_language_neutral_read_only_buffer = BitConverter.ToUInt16(raw, 0x0358);

            mh.unknown21 = ExtractBytes(raw, 0x0368, 0x008, bigEndian);
            mh.unknown22 = ExtractBytes(raw, 0x0370, 0x008, bigEndian);


            mh.sha1_a = uint_4.BuildArray(ExtractBytes(raw, 0x0378, 0x0014), bigEndian);
            mh.sha1_b = uint_4.BuildArray(ExtractBytes(raw, 0x038C, 0x0014), bigEndian);
            mh.sha1_c = uint_4.BuildArray(ExtractBytes(raw, 0x03A0, 0x0014), bigEndian);

            mh.rsa = uint_4.BuildArray(ExtractBytes(raw, 0x03B4, 0x0100), bigEndian);


            //for reach 
            //0 = debug section
            //1 = resource section
            //2 = tags section
            //3 = localizations
            mh.section_offsets = uint_4.BuildArray(ExtractBytes(raw, 0x04B4, 0x0010), bigEndian);
            //same layout as above but contains section boundary information
            mh.section_bounds = file_bounds.BuildArray(ExtractBytes(raw, 0x04C4, 0x0020), bigEndian);

            mh.guid = uint_4.BuildArray(ExtractBytes(raw, 0x04E4, 0x0010), bigEndian);

            mh.unknown23 = uint_4.BuildArray(ExtractBytes(raw, 0x04F4, 0x26C2 * 4), bigEndian);

            mh.foot_signiture = ExtractBytes(raw, 0x9FFC, 0x004);


            return mh;
        }
    }
}
