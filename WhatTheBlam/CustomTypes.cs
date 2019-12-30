using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhatTheBlam
{
    public static class HeaderReader
    {
        /// <summary>
        /// Extracts a subsection from a byte array.
        /// </summary>
        /// <param name="array">The array to extract the bytes from.</param>
        /// <param name="offset">The offset to start at in the array.</param>
        /// <param name="count">The number of bytes to extract.</param>
        /// <returns></returns>
        public static byte[] ExtractBytes(byte[] array, int offset, int count)
        {
            if(offset > array.Length || offset+count > array.Length)
            {
                return null;
            }
            byte[] tmp = new byte[count];
            for(int i = 0; i < count; ++i)
            {
                tmp[i] = array[offset + i];
            }

            return tmp;
        }
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
                if(head_tag != "head")
                {
                    throw new Exception("Don't recognize the map file!");
                }
            }


            //forgot to do the endianess checks... fuuuuuuuuuuuuuuuuhhhhhhhhhhhhkkkkkkkkk
            mh.head_signiture = ExtractBytes(raw, 0x0000, 0x0004);
            mh.mapfile_length = ExtractBytes(raw, 0x0004, 0x0004);
            mh.mapfile_compressed_length = ExtractBytes(raw, 0x008, 0x0004);

            mh.tags_header_address = ExtractBytes(raw, 0x0010, 0x0008);

            mh.memory_buffer_offset = ExtractBytes(raw, 0x0018, 0x0004);
            mh.memory_buffer_size = ExtractBytes(raw, 0x001C, 0x0004);

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

            mh.unknown4 = ExtractBytes(raw, 0x0148, 0x004);
            mh.unknown5 = ExtractBytes(raw, 0x014C, 0x004);
            mh.unknown6 = ExtractBytes(raw, 0x0150, 0x004);
            mh.unknown7 = ExtractBytes(raw, 0x0154, 0x004);
            mh.unknown8 = ExtractBytes(raw, 0x0158, 0x004);

            mh.string_id_count = ExtractBytes(raw, 0x015C, 0x004);
            mh.string_ids_buffer_size = ExtractBytes(raw, 0x0160, 0x004);
            mh.string_id_indices_offset = ExtractBytes(raw, 0x0164, 0x004);
            mh.string_ids_buffer_offset = ExtractBytes(raw, 0x0168, 0x004);

            mh.unknown9 = ExtractBytes(raw, 0x016C, 0x004);

            mh.timestamp = ExtractBytes(raw, 0x0170, 0x008);
            mh.mainmenu_timestamp = ExtractBytes(raw, 0x0178, 0x008);
            mh.shared_timestamp = ExtractBytes(raw, 0x0180, 0x008);
            mh.campaign_timestamp = ExtractBytes(raw, 0x0188, 0x008);
            mh.multiplayer_timestamp = ExtractBytes(raw, 0x0190, 0x008);

            mh.name = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x0198, 0x0020));

            mh.unknown10 = ExtractBytes(raw, 0x01B8, 0x004);

            mh.scenario_path = Encoding.ASCII.GetChars(ExtractBytes(raw, 0x01BC, 0x0100));

            mh.minor_version = ExtractBytes(raw, 0x02BC, 0x004);

            mh.tag_name_count = ExtractBytes(raw, 0x02C0, 0x004);
            mh.tag_names_buffer_offset = ExtractBytes(raw, 0x02C4, 0x004);
            mh.tag_names_buffer_size = ExtractBytes(raw, 0x02C8, 0x004);
            mh.tag_name_indices_offset = ExtractBytes(raw, 0x02CC, 0x004);

            mh.checksum = ExtractBytes(raw, 0x02D0, 0x004);

            mh.unknown11 = ExtractBytes(raw, 0x02D4, 0x004);
            mh.unknown12 = ExtractBytes(raw, 0x02D8, 0x004);
            mh.unknown13 = ExtractBytes(raw, 0x02DC, 0x004);
            mh.unknown14 = ExtractBytes(raw, 0x02E0, 0x004);
            mh.unknown15 = ExtractBytes(raw, 0x02E4, 0x004);
            mh.unknown16 = ExtractBytes(raw, 0x02E8, 0x004);
            mh.unknown17 = ExtractBytes(raw, 0x02EC, 0x004);
            mh.unknown18 = ExtractBytes(raw, 0x02F0, 0x004);
            mh.unknown19 = ExtractBytes(raw, 0x02F4, 0x004);

            mh.virtual_base_address = ExtractBytes(raw, 0x02F8, 0x008);

            mh.xdx_version = ExtractBytes(raw, 0x0300, 0x004);

            mh.unknown20 = ExtractBytes(raw, 0x0304, 0x004);

            mh.tag_post_link_buffer = BitConverter.ToUInt16(raw, 0x0308);
            mh.tag_language_dependent_read_only_buffer = BitConverter.ToUInt16(raw, 0x0318);
            mh.tag_language_dependent_read_write_buffer = BitConverter.ToUInt16(raw, 0x0328);

            mh.tag_language_neutral_read_write_buffer = BitConverter.ToUInt16(raw, 0x0338);
            mh.tag_language_neutral_write_combined_buffer = BitConverter.ToUInt16(raw, 0x0348);
            mh.tag_language_neutral_read_only_buffer = BitConverter.ToUInt16(raw, 0x0358);

            mh.unknown21 = ExtractBytes(raw, 0x0368, 0x008);
            mh.unknown22 = ExtractBytes(raw, 0x0370, 0x008);

            
            mh.sha1_a = uint_4.BuildArray(ExtractBytes(raw, 0x0378, 0x0014));
            mh.sha1_b = uint_4.BuildArray(ExtractBytes(raw, 0x038C, 0x0014));
            mh.sha1_c = uint_4.BuildArray(ExtractBytes(raw, 0x03A0, 0x0014));

            mh.rsa = uint_4.BuildArray(ExtractBytes(raw, 0x03B4, 0x0100));

            
            mh.section_offsets = uint_4.BuildArray(ExtractBytes(raw, 0x04B4, 0x0010));

            mh.section_bounds = file_bounds.BuildArray(ExtractBytes(raw, 0x04C4, 0x0020));

            mh.guid = uint_4.BuildArray(ExtractBytes(raw, 0x04E4, 0x0010));

            mh.unknown23 = uint_4.BuildArray(ExtractBytes(raw, 0x04F4, 0x26C2 * 4));

            mh.foot_signiture = ExtractBytes(raw, 0x9FFC, 0x004);


            return mh;
        }
    }
    //These are some helper types because C# types are different than C/C++ types and fucks up the map loading

    /// <summary>
    /// 2 byte unsigned int
    /// </summary>
    public class uint_2
    {
        public byte[] bytes = new byte[2];
        public static int size = 2;

        public static implicit operator UInt64(uint_2 op)
        {
            return op.ToUInt();
        }

        public static implicit operator uint_2(byte[] op)
        {
            if (op.Length != 2) throw new Exception("Invalid conversion from byte array of length " + op.Length + " to uint_2.");
            uint_2 tmp = new uint_2();
            tmp.bytes = op;
            return tmp;
        }

        public UInt64 ToUInt(bool bigEndian = false)
        {
            byte[] tmp = this.bytes;
            if (bigEndian) Array.Reverse(tmp);
            return BitConverter.ToUInt64(tmp, 0);
        }

        public Int64 ToInt(bool bigEndian = false)
        {
            byte[] tmp = this.bytes;
            if (bigEndian) Array.Reverse(tmp);
            return BitConverter.ToInt64(tmp, 0);
        }
    }

    /// <summary>
    /// 4 byte unsigned int
    /// </summary>
    public class uint_4
    {
        public byte[] bytes = new byte[4];
        public static int size = 4;

        public static implicit operator UInt64(uint_4 op)
        {
            return op.ToUInt();
        }

        public static implicit operator uint_4(byte[] op)
        {
            if (op.Length != 4)
            {
                throw new Exception("Invalid conversion from byte array of length " + op.Length + " to uint_4.");
            }
            uint_4 tmp = new uint_4();
            tmp.bytes = op;
            return tmp;
        }

        public static uint_4[] BuildArray(byte[] op)
        {
            if(op.Length % 4 != 0)
            {
                throw new Exception("Invalid conversion from byte array of length " + op.Length + " to uint_4 array.");
            }
            int len = op.Length / 4;
            uint_4[] tmp = new uint_4[len];
            for(int i =0; i < len; ++i)
            {
                tmp[i] = new uint_4();
                tmp[i].bytes[0] = op[i * 4];
                tmp[i].bytes[1] = op[(i * 4) + 1];
                tmp[i].bytes[2] = op[(i * 4) + 2];
                tmp[i].bytes[3] = op[(i * 4) + 3];
            }
            return tmp;
        }

        public UInt64 ToUInt(bool bigEndian = false)
        {
            byte[] tmp = this.bytes;
            if (bigEndian) Array.Reverse(tmp);
            return BitConverter.ToUInt64(tmp, 0);
        }

        public Int64 ToInt(bool bigEndian = false)
        {
            byte[] tmp = this.bytes;
            if (bigEndian) Array.Reverse(tmp);
            return BitConverter.ToInt64(tmp, 0);
        }
    }


    /// <summary>
    /// 8 byte unsigned int
    /// </summary>
    public class uint_8
    {
        public byte[] bytes = new byte[8];
        public static int size = 8;

        public static implicit operator uint(uint_8 op)
        {
            return op.ToUInt();
        }

        public static implicit operator uint_8(byte[] op)
        {
            if (op.Length != 8) throw new Exception("Invalid conversion from byte array of length " + op.Length + " to uint_8.");
            uint_8 tmp = new uint_8();
            tmp.bytes = op;
            return tmp;
        }

        public uint ToUInt(bool bigEndian = false)
        {
            byte[] tmp = this.bytes;
            if (bigEndian) Array.Reverse(tmp);
            return BitConverter.ToUInt32(tmp, 0);
        }

        public Int64 ToInt(bool bigEndian = false)
        {
            byte[] tmp = this.bytes;
            if (bigEndian) Array.Reverse(tmp);
            return BitConverter.ToInt64(tmp, 0);
        }
    }


    /// <summary>
    /// Cache file section bounds.
    /// Contains uint_4 offset, and uint_4 size
    /// </summary>
    public class file_bounds
    {
        public uint_4 offset = new uint_4();
        public uint_4 size = new uint_4();

        public static implicit operator file_bounds(byte[] op)
        {
            if (op.Length != 8) throw new Exception("Invalid conversion from byte array of length " + op.Length + " to file_bounds.");
            file_bounds tmp = new file_bounds();
            Array.Copy(op, 0, tmp.offset.bytes, 0, 4);
            Array.Copy(op, 4, tmp.size.bytes, 0, 4);
            return tmp;
        }

        public static file_bounds[] BuildArray(byte[] op)
        {
            if (op.Length % 8 != 0)
            {
                throw new Exception("Invalid conversion from byte array of length " + op.Length + " to file_bounds array.");
            }
            int len = op.Length / 8;
            file_bounds[] tmp = new file_bounds[len];
            for (int i = 0; i < len; ++i)
            {
                tmp[i] = new file_bounds();
                byte[] buff = new byte[8];
                Array.Copy(op, i * 8, buff, 0, 8);
                tmp[i] = buff;
            }
            return tmp;
        }
    }

    /// <summary>
    /// Helper class to read types from a stream.
    /// </summary>
    static class HaloTypeReader
    {
        public static uint_2 ReadUInt2(Stream s)
        {
            uint_2 tmp = new uint_2();
            ReadType(out tmp, s);
            return tmp;
        }

        public static uint_4 ReadUInt4(Stream s)
        {
            uint_4 tmp = new uint_4();
            ReadType(out tmp, s);
            return tmp;
        }

        public static uint_8 ReadUInt8(Stream s)
        {
            uint_8 tmp = new uint_8();
            ReadType(out tmp, s);
            return tmp;
        }

        public static file_bounds ReadFileBounds(Stream s)
        {
            file_bounds tmp = new file_bounds();
            ReadType(out tmp, s);
            return tmp;
        }

        public static char[] ReadCharArray(Stream s, int size)
        {
            byte[] tmp = new byte[size];
            s.Read(tmp, 0, size);
            return System.Text.Encoding.ASCII.GetString(tmp).ToCharArray(); //oof
        }

        public static char ReadChar(Stream s)
        {
            byte[] tmp = new byte[1];
            s.Read(tmp, 0, 1);
            return (char)tmp[0];
        }

        public static byte ReadByte(Stream s)
        {
            byte[] tmp = new byte[1];
            s.Read(tmp, 0, 1);
            return tmp[0];
        }

        public static byte[] ReadByteArray(Stream s, int size)
        {
            byte[] tmp = new byte[size];
            s.Read(tmp, 0, size);
            return tmp;
        }




        //Type readers
        public static void ReadType(out uint_2 out_int, Stream s)
        {
            uint_2 tmp = new uint_2();
            s.Read(tmp.bytes, 0, uint_2.size);
            out_int = tmp;
        }

        public static void ReadType(out uint_4 out_int, Stream s)
        {
            uint_4 tmp = new uint_4();
            s.Read(tmp.bytes, 0, uint_4.size);
            out_int = tmp;
        }

        public static void ReadType(out uint_8 out_int, Stream s)
        {
            uint_8 tmp = new uint_8();
            s.Read(tmp.bytes, 0, uint_8.size);
            out_int = tmp;
        }

        public static void ReadType(out file_bounds out_fb, Stream s)
        {
            file_bounds tmp = new file_bounds();
            ReadType(out tmp.offset, s);
            ReadType(out tmp.size, s);
            out_fb = tmp;
        }
    }
}
