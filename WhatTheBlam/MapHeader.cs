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
        
        public uint head_signiture;    //not really a long, but 4 bytes either being head or daeh (1684104552 or 1751474532) (big endian or little endian)
                                                          //0x0000    little endian .map file data offsets, useful for debugging and exploration (and masochists that like to edit in the raw)
        public uint engine_version;                       //0x0004
        public uint mapfile_length;                       //0x0008
        public uint mapfile_compressed_length;            //0x000C

        public ulong tags_header_address;                  //0x0010

        public uint memory_buffer_offset;                 //0x0018
        public uint memory_buffer_size;                   //0x001C

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

        public uint unknown4;                             //0x0148
        public uint unknown5;                             //0x014C
        public uint unknown6;                             //0x0150
        public uint unknown7;                             //0x0154
        public uint unknown8;                             //0x0158

        public uint string_id_count;                      //0x015C
        public uint string_ids_buffer_size;               //0x0160
        public uint string_id_indices_offset;             //0x0164
        public uint string_ids_buffer_offset;             //0x0168

        public uint unknown9;                             //0x016C
        
        public ulong timestamp;                            //0x0170
        public ulong mainmenu_timestamp;                   //0x0178
        public ulong shared_timestamp;                     //0x0180
        public ulong campaign_timestamp;                   //0x0188
        public ulong multiplayer_timestamp;                //0x0190

        public char[] name = new char[32];                  //0x0198

        public uint unknown10;                            //0x01B8

        public char[] scenario_path = new char[256];        //0x01BC

        public uint minor_version;                        //0x02BC

        public uint tag_name_count;                       //0x02C0
        public uint tag_names_buffer_offset;              //0x02C4
        public uint tag_names_buffer_size;                //0x02C8
        public uint tag_name_indices_offset;              //0x02CC

        public uint checksum;                             //0x02D0


        public uint unknown11;                            //0x02D4
        public uint unknown12;                            //0x02D8
        public uint unknown13;                            //0x02DC
        public uint unknown14;                            //0x02E0
        public uint unknown15;                            //0x02E4
        public uint unknown16;                            //0x02E8
        public uint unknown17;                            //0x02EC
        public uint unknown18;                            //0x02F0
        public uint unknown19;                            //0x02F4

        public ulong virtual_base_address;                //0x02F8

        public uint xdx_version;                          //0x0300
        
        public uint unknown20;                            //0x0308

        //Not totally sure how these get used yet but will investigate further
        //Shamelessly stole these from zeta tool
        //Credit where credit is due https://github.com/camden-smallwood/zeta
        //Looking at how Assembly uses this section, looks like they are offsets
        //and lengths. Will investigate further
        public UInt16 tag_post_link_buffer;
        public UInt16 tag_language_dependent_read_only_buffer;
        public UInt16 tag_language_dependent_read_write_buffer;
        public UInt16 tag_language_neutral_read_write_buffer;
        public UInt16 tag_language_neutral_write_combined_buffer;
        public UInt16 tag_language_neutral_read_only_buffer;

        public ulong unknown21;
        public ulong unknown22;

        public uint[] sha1_a = new uint[5];
        public uint[] sha1_b = new uint[5];
        public uint[] sha1_c = new uint[5];

        public uint[] rsa = new uint[64];

        public uint[] section_offsets = new uint[4];


        public file_bounds[] section_bounds = new file_bounds[4];

        public uint[] guid = new uint[4];

        public uint[] unknown23 = new uint[0x26C2];

        public uint foot_signiture; //not really a long, but 4 bytes either being foot or toof (1953460070 or 1718579060) (big endian or little endian)

    }


    //A helper class to read in the header data from a .map file
    public static class HeaderReader
    {


 

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

            
            

            mh.head_signiture = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0000, 0x0004), 0);

            mh.engine_version = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0004, 0x004, bigEndian), 0);

            mh.mapfile_length = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0008, 0x0004, bigEndian), 0);
            mh.mapfile_compressed_length = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x00C, 0x0004, bigEndian), 0);

            mh.tags_header_address = BitConverter.ToUInt64(Util.ExtractBytes(raw, 0x0010, 0x0008, bigEndian), 0);

            mh.memory_buffer_offset = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0018, 0x0004, bigEndian), 0);
            mh.memory_buffer_size = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x001C, 0x0004, bigEndian), 0);

            mh.source_file = Encoding.ASCII.GetChars(Util.ExtractBytes(raw, 0x0020, 0x0100));
            mh.build = Encoding.ASCII.GetChars(Util.ExtractBytes(raw, 0x0120, 0x0020));

            mh.scenario_load_type = Util.ExtractBytes(raw, 0x0140, 0x001)[0];
            mh.scenario_load_type_short = Util.ExtractBytes(raw, 0x0141, 0x001)[0];

            mh.scenario_load_type = Util.ExtractBytes(raw, 0x0142, 0x001)[0];
            mh.scenario_load_type_short = Util.ExtractBytes(raw, 0x0143, 0x001)[0];


            mh.unknown1 = Encoding.ASCII.GetChars(Util.ExtractBytes(raw, 0x0144, 0x0001))[0];
            mh.tracked_build = Util.ExtractBytes(raw, 0x0145, 0x001)[0];
            mh.unknown2 = Encoding.ASCII.GetChars(Util.ExtractBytes(raw, 0x0146, 0x001))[0];
            mh.unknown3 = Encoding.ASCII.GetChars(Util.ExtractBytes(raw, 0x0147, 0x001))[0];

            mh.unknown4 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0148, 0x004, bigEndian), 0);
            mh.unknown5 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x014C, 0x004, bigEndian), 0);
            mh.unknown6 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0150, 0x004, bigEndian), 0);
            mh.unknown7 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0154, 0x004, bigEndian), 0);
            mh.unknown8 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0158, 0x004, bigEndian), 0);

            mh.string_id_count = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x015C, 0x004, bigEndian), 0);
            mh.string_ids_buffer_size = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0160, 0x004, bigEndian), 0);
            mh.string_id_indices_offset = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0164, 0x004, bigEndian), 0);
            mh.string_ids_buffer_offset = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0168, 0x004, bigEndian), 0);

            mh.unknown9 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x016C, 0x004, bigEndian), 0);

            mh.timestamp =BitConverter.ToUInt64( Util.ExtractBytes(raw, 0x0170, 0x008, bigEndian), 0); 
            mh.mainmenu_timestamp = BitConverter.ToUInt64(Util.ExtractBytes(raw, 0x0178, 0x008, bigEndian), 0);
            mh.shared_timestamp = BitConverter.ToUInt64(Util.ExtractBytes(raw, 0x0180, 0x008, bigEndian), 0);
            mh.campaign_timestamp = BitConverter.ToUInt64(Util.ExtractBytes(raw, 0x0188, 0x008, bigEndian), 0);
            mh.multiplayer_timestamp = BitConverter.ToUInt64(Util.ExtractBytes(raw, 0x0190, 0x008, bigEndian), 0);

            mh.name = Encoding.ASCII.GetChars(Util.ExtractBytes(raw, 0x0198, 0x0020));

            mh.unknown10 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x01B8, 0x004, bigEndian), 0);

            mh.scenario_path = Encoding.ASCII.GetChars(Util.ExtractBytes(raw, 0x01BC, 0x0100));

            mh.minor_version = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02BC, 0x004, bigEndian), 0);

            mh.tag_name_count = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02C0, 0x004, bigEndian), 0);
            mh.tag_names_buffer_offset = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02C4, 0x004, bigEndian), 0);
            mh.tag_names_buffer_size = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02C8, 0x004, bigEndian), 0);
            mh.tag_name_indices_offset = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02CC, 0x004, bigEndian), 0);

            mh.checksum = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02D0, 0x004, bigEndian), 0);

            mh.unknown11 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02D4, 0x004, bigEndian), 0);
            mh.unknown12 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02D8, 0x004, bigEndian), 0);
            mh.unknown13 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02DC, 0x004, bigEndian), 0);
            mh.unknown14 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02E0, 0x004, bigEndian), 0);
            mh.unknown15 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02E4, 0x004, bigEndian), 0);
            mh.unknown16 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02E8, 0x004, bigEndian), 0);
            mh.unknown17 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02EC, 0x004, bigEndian), 0);
            mh.unknown18 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02F0, 0x004, bigEndian), 0);
            mh.unknown19 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x02F4, 0x004, bigEndian), 0);

            mh.virtual_base_address = BitConverter.ToUInt64(Util.ExtractBytes(raw, 0x02F8, 0x008, bigEndian), 0);

            mh.xdx_version = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0300, 0x004, bigEndian), 0);

            mh.unknown20 = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x0304, 0x004, bigEndian), 0);

            mh.tag_post_link_buffer = BitConverter.ToUInt16(Util.ExtractBytes(raw, 0x0308, 0x0010, bigEndian), 0);                       //using Util.ExtractBytes allows for a quick and easy endian swap
            mh.tag_language_dependent_read_only_buffer = BitConverter.ToUInt16(Util.ExtractBytes(raw, 0x0318, 0x0010, bigEndian), 0);
            mh.tag_language_dependent_read_write_buffer = BitConverter.ToUInt16(Util.ExtractBytes(raw, 0x0328, 0x0010, bigEndian), 0);

            mh.tag_language_neutral_read_write_buffer = BitConverter.ToUInt16(raw, 0x0338);
            mh.tag_language_neutral_write_combined_buffer = BitConverter.ToUInt16(raw, 0x0348);
            mh.tag_language_neutral_read_only_buffer = BitConverter.ToUInt16(raw, 0x0358);

            mh.unknown21 = BitConverter.ToUInt64(Util.ExtractBytes(raw, 0x0368, 0x008, bigEndian), 0);
            mh.unknown22 = BitConverter.ToUInt64(Util.ExtractBytes(raw, 0x0370, 0x008, bigEndian), 0);


            mh.sha1_a = Util.UIntArray(Util.ExtractBytes(raw, 0x0378, 0x0014), bigEndian);
            mh.sha1_b = Util.UIntArray(Util.ExtractBytes(raw, 0x038C, 0x0014), bigEndian);
            mh.sha1_c = Util.UIntArray(Util.ExtractBytes(raw, 0x03A0, 0x0014), bigEndian);

            mh.rsa = Util.UIntArray(Util.ExtractBytes(raw, 0x03B4, 0x0100), bigEndian);


            //for reach 
            //0 = debug section
            //1 = resource section
            //2 = tags section
            //3 = localizations
            mh.section_offsets = Util.UIntArray(Util.ExtractBytes(raw, 0x04B4, 0x0010), bigEndian);
            //same layout as above but contains section boundary information
            mh.section_bounds = file_bounds.BuildArray(Util.ExtractBytes(raw, 0x04C4, 0x0020), bigEndian);

            mh.guid = Util.UIntArray(Util.ExtractBytes(raw, 0x04E4, 0x0010), bigEndian);

            mh.unknown23 = Util.UIntArray(Util.ExtractBytes(raw, 0x04F4, 0x26C2 * 4), bigEndian);

            mh.foot_signiture = BitConverter.ToUInt32(Util.ExtractBytes(raw, 0x9FFC, 0x004), 0);


            return mh;
        }
    }
}
