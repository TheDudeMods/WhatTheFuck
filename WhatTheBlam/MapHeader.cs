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
        public uint_4 head_version;
        public uint_4 mapfile_length;
        public uint_4 mapfile_compressed_length;

        public uint_8 tags_header_address;

        public uint_4 memory_buffer_offset;
        public uint_4 memory_buffer_size;

        public char[] source_file = new char[256];

        public char[] build = new char[32];

        public byte scenario_type;
        public byte scenario_short;

        public byte scenario_load_type;
        public byte scenario_load_type_short;

        public char unknown1;
        public byte tracked_build;
        public char unknown2;
        public char unknown3;

        public uint_4 unknown4;
        public uint_4 unknown5;
        public uint_4 unknown6;
        public uint_4 unknown7;
        public uint_4 unknown8;

        public uint_4 string_id_count;
        public uint_4 string_ids_buffer_size;
        public uint_4 string_id_indices_offset;
        public uint_4 string_ids_buffer_offset;

        public uint_4 unknown9;
        
        public uint_8 timestamp;
        public uint_8 mainmenu_timestamp;
        public uint_8 shared_timestamp;
        public uint_8 campaign_timestamp;
        public uint_8 multiplayer_timestamp;

        public char[] name = new char[32];

        public uint_4 unknown10;

        public char[] scenario_path = new char[256];

        public uint_4 minor_version;

        public uint_4 tag_name_count;
        public uint_4 tag_names_buffer_offset;
        public uint_4 tag_names_buffer_size;
        public uint_4 tag_name_indices_offset;

        public uint_4 checksum;


        public uint_4 unknown11;
        public uint_4 unknown12;
        public uint_4 unknown13;
        public uint_4 unknown14;
        public uint_4 unknown15;
        public uint_4 unknown16;
        public uint_4 unknown17;
        public uint_4 unknown18;
        public uint_4 unknown19;

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


        public void Load(Stream s)
        {
            this.head_version = HaloTypeReader.ReadUInt4(s);
            if(this.head_version == 1684104552)
            {

            }
        }

    }
}
