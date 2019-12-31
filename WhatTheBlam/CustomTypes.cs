using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhatTheBlam
{
    
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

        public int val
        {
            get { return this.ToInt(); }
        }
        public uint uval
        {
            get { return this.ToUInt(); }
        }

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

        public uint ToUInt(bool bigEndian = false)
        {
            byte[] tmp = this.bytes;
            if (bigEndian) Array.Reverse(tmp);
            return BitConverter.ToUInt32(tmp, 0);
        }

        public int ToInt(bool bigEndian = false)
        {
            byte[] tmp = this.bytes;
            if (bigEndian) Array.Reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
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
