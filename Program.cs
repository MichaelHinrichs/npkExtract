//Written for games in the NERD engine, by minorkeygames. https://minorkeygames.com/
//You Have to Win the Game https://store.steampowered.com/app/286100/You_Have_to_Win_the_Game/
//Super Win The Game https://store.steampowered.com/app/310700/Super_Win_the_Game/
//Gunmetal Arcadia https://store.steampowered.com/app/332270/Gunmetal_Arcadia/
//Gunmetal Arcadia Zero https://store.steampowered.com/app/555610/Gunmetal_Arcadia_Zero/

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
            if (magic != "npk." && magic != ".npk")
            {
                throw new ArgumentException("This file is not a NERD engine NPK file.");
            }
            
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
                subfile[i].Name = new string(br.ReadChars(0XF8)).TrimEnd('\0');
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
    }
}
