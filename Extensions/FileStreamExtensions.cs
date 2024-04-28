using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GitWorker.Extensions
{
    public static class FileStreamExtensions
    {
        public static int ReadInt(this Stream stream)
        {
            var wordBuffer = new byte[4];
            stream.Read(wordBuffer, 0, 4);
            Array.Reverse(wordBuffer);
            return BitConverter.ToInt32(wordBuffer);
        }

        public static string ReadSha(this Stream stream)
        {
            var wordBuffer = new byte[20];
            stream.Read(wordBuffer, 0, 20);
            return string.Join("", wordBuffer.Select(b => b.ToString("x2")));
        }

        public static short ReadShort(this Stream stream)
        {
            var wordBuffer = new byte[2];
            stream.Read(wordBuffer, 0, 2);
            Array.Reverse(wordBuffer);
            return BitConverter.ToInt16(wordBuffer);
        }

        public static string ReadString(this Stream stream, int? size = null)
        {
            if(size != null)
            {
                var defBuffer = new byte[size.Value];
                stream.Read(defBuffer, 0, size.Value);
                return Encoding.UTF8.GetString(defBuffer.Where(v => v != 0x00).ToArray()).Trim();
            }

            List<byte> buffer = new List<byte>();
            while(buffer.Count == 0 || buffer[buffer.Count - 1] != 0x00)
            {
                var result = stream.ReadByte();
                if(result == -1) break;

                buffer.Add((byte)result);
            }

            if(buffer.Count == 0) return null;

            return Encoding.UTF8.GetString(buffer.Take(buffer[buffer.Count - 1] == 0x00 ? buffer.Count - 1 : buffer.Count).ToArray());
        }
    }
}