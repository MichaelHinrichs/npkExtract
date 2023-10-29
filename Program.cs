using System;
using System.IO;

namespace npkExtract
{
    class Program
    {

        public static BinaryReader br;
        public static BinaryWriter bw;

        struct Subfile
        {
            public string Name;
            public uint Offset;
            public uint Size;
        }

        static void Main(string[] args)
        {
            using FileStream source = File.OpenRead(args[0]);
            BinaryReader br = new(source);

            string magic = new(br.ReadChars(4));
            if (magic == "npk." || magic == ".npk")
            {
                if (magic == ".npk")
                {
                    br.ReadBytes(4);
                }
                uint metaOffset = br.ReadUInt32();
                uint metaSize = br.ReadUInt32();
                Subfile[] subfile = new Subfile[metaSize / 0X100];
                br.BaseStream.Position = metaOffset;
                for (int i = 0; i < subfile.Length; i++)
                {
                    long nameOffset = br.BaseStream.Position;
                    char[] fileName = Array.Empty<char>();
                    char readChar = (char)1;
                    while (readChar > 0)
                    {
                        readChar = br.ReadChar();
                        Array.Resize(ref fileName, fileName.Length + 1);
                        fileName[^1] = readChar;
                    }
                    Array.Resize(ref fileName, fileName.Length - 1);
                    subfile[i].Name = new(fileName);
                    br.BaseStream.Position = nameOffset + 0XF8;
                    subfile[i].Offset = br.ReadUInt32();
                    subfile[i].Size = br.ReadUInt32();
                }

                for (int i = 0; i < subfile.Length; i++)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(source.Name) + "\\" + Path.GetFileNameWithoutExtension(source.Name));
                    BinaryWriter bw = new(File.OpenWrite(Path.GetDirectoryName(source.Name) + "\\" + Path.GetFileNameWithoutExtension(source.Name) + "\\" + subfile[i].Name));
                    br.BaseStream.Position = subfile[i].Offset;
                    bw.Write(br.ReadBytes((int)subfile[i].Size));

                    bw.Close();
                }
            }
            else
            {
                Console.WriteLine("This file is not a NERD engine NPK file.");
            }
        }
    }
}
