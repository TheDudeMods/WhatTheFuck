﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WhatTheBlam
{
    class Util
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


        public static int[] IntArray(byte[] array, bool reverse = false)
        {
            if (array.Length % 4 != 0)
            {
                throw new Exception("Invalid conversion from byte array of length " + array.Length + " to int array.");
            }
            int len = array.Length / 4;
            int[] tmp = new int[len];
            for (int i = 0; i < len; ++i)
            {
                byte[] thisInt = ExtractBytes(array, i * 4, 4, reverse);
                tmp[i] = BitConverter.ToInt32(thisInt, 0);
            }
            return tmp;
        }

        /// <summary>
        /// Extracts an array of 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public static uint[] UIntArray(byte[] array, bool reverse = false)
        {
            if (array.Length % 4 != 0)
            {
                throw new Exception("Invalid conversion from byte array of length " + array.Length + " to int array.");
            }
            int len = array.Length / 4;
            uint[] tmp = new uint[len];
            for (int i = 0; i < len; ++i)
            {
                byte[] thisInt = ExtractBytes(array, i * 4, 4, reverse);
                tmp[i] = BitConverter.ToUInt32(thisInt, 0);
            }
            return tmp;
        }
    }
}
