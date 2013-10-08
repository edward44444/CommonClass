using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonClass.Utilities
{
    public class ImageFormatInspector
    {
        public static bool IsJPEG(Stream stream)
        {
            long oriPosition = stream.Position;
            byte[] formatByte = new byte[4];
            stream.Position = 6;
            stream.Read(formatByte, 0, formatByte.Length);
            stream.Position = oriPosition;
            string format=Encoding.ASCII.GetString(formatByte);
            if (format == "Exif" || format == "JFIF")
            {
                return true;
            }
            return false;
        }

        public static bool IsPNG(Stream stream)
        {
            long oriPosition = stream.Position;
            byte[] formatByte = new byte[3];
            stream.Position = 1;
            stream.Read(formatByte, 0, formatByte.Length);
            stream.Position = oriPosition;
            if (Encoding.ASCII.GetString(formatByte) == "PNG")
            {
                return true;
            }
            return false;
        }

        public static bool IsBMP(Stream stream)
        {
            long oriPosition = stream.Position;
            byte[] formatByte = new byte[2];
            stream.Position = 0;
            stream.Read(formatByte, 0, formatByte.Length);
            stream.Position = oriPosition;
            if (Encoding.ASCII.GetString(formatByte) == "BM")
            {
                return true;
            }
            return false;
        }

        public static bool IsGIF(Stream stream)
        {
            long oriPosition = stream.Position;
            byte[] formatByte = new byte[3];
            stream.Position = 0;
            stream.Read(formatByte, 0, formatByte.Length);
            stream.Position = oriPosition;
            if (Encoding.ASCII.GetString(formatByte) == "GIF")
            {
                return true;
            }
            return false;
        }
    }
}
