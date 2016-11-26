using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Decompile
{
    public class Exe
    {
        private byte[] _content;

        private Exe() { }

        private void load(string filename)
        {
            var stream = new FileStream(filename, FileMode.Open);
            using (var reader = new BinaryReader(stream))
            {
                _content = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            int newHeader = 0;
            Image.DosHeader.Access(_content, 0, delegate(ref Image.DosHeader header)
            {
                newHeader = (int)header.PNewHeader;
            });
            UInt32 magic = 0;
            AccessDWord(_content, newHeader, delegate(ref UInt32 val)
            {
                magic = val;
            });
            Image.OptionalHeader.Access(_content, newHeader + 24, delegate(ref Image.OptionalHeader header)
            {
                int i = 0;
            });
        }

        public static Exe Load(string filename)
        {
            Exe exe = new Exe();
            exe.load(filename);
            return exe;
        }

        private delegate void WordAccessor(ref UInt16 val);
        private unsafe static void AccessWord(byte[] content, int offset, WordAccessor accessor)
        {
            fixed (byte* pContent = &content[offset])
            {
                UInt16* pWord = (UInt16*)pContent;
                accessor(ref *pWord);
            }
        }

        private unsafe static UInt16 ReadWord(byte[] content, int offset)
        {
            UInt16 val = 0;
            AccessWord(content, offset, delegate(ref UInt16 v)
            {
                val = v;
            });
            return val;
        }

        private delegate void DWordAccessor(ref UInt32 val);
        private unsafe static void AccessDWord(byte[] content, int offset, DWordAccessor accessor)
        {
            fixed (byte* pContent = &content[offset])
            {
                UInt32* pDWord = (UInt32*)pContent;
                accessor(ref *pDWord);
            }
        }

        private unsafe static UInt32 ReadDWord(byte[] content, int offset)
        {
            UInt32 val = 0;
            AccessDWord(content, offset, delegate(ref UInt32 v)
            {
                val = v;
            });
            return val;
        }

        private struct Image
        {
            public unsafe struct DosHeader
            {
                public UInt16 Signature;
                public UInt16 BytesInLastBlock;
                public UInt16 BlocksInFile;
                public UInt16 NumRelocs;
                public UInt16 HeaderParagraphs;
                public UInt16 MinExtraParagraphs;
                public UInt16 MaxExtraParagraphs;
                public UInt16 Ss;
                public UInt16 Sp;
                public UInt16 Checksum;
                public UInt16 Ip;
                public UInt16 Cs;
                public UInt16 RelocTableOffset;
                public UInt16 OverlayNumber;
                public fixed UInt16 Reserved[4];
                public UInt16 OemId;
                public UInt16 OemInfo;
                public fixed UInt16 Reserved2[10];
                public UInt32 PNewHeader;

                public delegate void Accessor(ref DosHeader header);
                public static void Access(byte[] content, int offset, Accessor accessor)
                {
                    fixed (byte* pContent = &content[offset])
                    {
                        DosHeader* pHeader = (DosHeader*)pContent;
                        accessor(ref *pHeader);
                    }
                }

                public static DosHeader Read(byte[] content, int offset)
                {
                    DosHeader header = new DosHeader();
                    Access(content, offset, delegate(ref DosHeader h)
                    {
                        header = h;
                    });
                    return header;
                }
            }

            public unsafe struct FileHeader
            {
                public UInt16 Machine;
                public UInt16 NumberOfSections;
                public UInt32 TimeDateStamp;
                public UInt32 PSymbolTable;
                public UInt32 NumberOfSymbols;
                public UInt16 SizeOfOptionalHeader;
                public UInt16 Characteristics;

                public delegate void Accessor(ref FileHeader header);
                public static void Access(byte[] content, int offset, Accessor accessor)
                {
                    fixed (byte* pContent = &content[offset])
                    {
                        FileHeader* pHeader = (FileHeader*)pContent;
                        accessor(ref *pHeader);
                    }
                }

                public static FileHeader Read(byte[] content, int offset)
                {
                    FileHeader header = new FileHeader();
                    Access(content, offset, delegate(ref FileHeader h)
                    {
                        header = h;
                    });
                    return header;
                }
            }

            public struct DataDirectory
            {
                public UInt32 VirtualAddress;
                public UInt32 Size;
            }

            public const int NumberOfDirectoryEntries = 16;
            public enum DirectoryEntry
            {
                Export = 0,
                Import = 1,
                Resource = 2,
                Exception = 3,
                Security = 4,
                BaseReloc = 5,
                Debug = 6,
                Copyright = 7,
                GlobalPtr = 8,
                TLS = 9,
                LoadConfig = 10
            }

            public unsafe struct OptionalHeader
            {
                public UInt16 Signature;
                public Byte MajorLinkerVersion;
                public Byte MinorLinkerVersion;
                public UInt32 SizeOfCode;
                public UInt32 SizeOfInitializedData;
                public UInt32 SizeOfUninitializedData;
                public UInt32 PEntryPoint;
                public UInt32 BaseOfCode;
                public UInt32 BaseOfData;

                public UInt32 ImageBase;
                public UInt32 SectionAlignment;
                public UInt32 FileAlignment;
                public UInt16 MajorOperatingSystemVersion;
                public UInt16 MinorOperatingSystemVersion;
                public UInt16 MajorImageVersion;
                public UInt16 MinorImageVersion;
                public UInt16 MajorSubsystemVersion;
                public UInt16 MinorSubsystemVersion;
                public UInt32 Reserved1;
                public UInt32 SizeOfImage;
                public UInt32 SizeOfHeaders;
                public UInt32 CheckSum;
                public UInt16 Subsystem;
                public UInt16 DllCharacteristics;
                public UInt32 SizeOfStackReserve;
                public UInt32 SizeOfStackCommit;
                public UInt32 SizeOfHeapReserve;
                public UInt32 SizeOfHeapCommit;
                public UInt32 LoaderFlags;
                public UInt32 NumberOfRvaAndSizes;
                public DataDirectory DataDirectory00;
                public DataDirectory DataDirectory01;
                public DataDirectory DataDirectory02;
                public DataDirectory DataDirectory03;
                public DataDirectory DataDirectory04;
                public DataDirectory DataDirectory05;
                public DataDirectory DataDirectory06;
                public DataDirectory DataDirectory07;
                public DataDirectory DataDirectory08;
                public DataDirectory DataDirectory09;
                public DataDirectory DataDirectory10;
                public DataDirectory DataDirectory11;
                public DataDirectory DataDirectory12;
                public DataDirectory DataDirectory13;
                public DataDirectory DataDirectory14;
                public DataDirectory DataDirectory15;
                public DataDirectory DataDirectory(int i)
                {
                    switch (i)
                    {
                        case 0: return DataDirectory00;
                        case 1: return DataDirectory01;
                        case 2: return DataDirectory02;
                        case 3: return DataDirectory03;
                        case 4: return DataDirectory04;
                        case 5: return DataDirectory05;
                        case 6: return DataDirectory06;
                        case 7: return DataDirectory07;
                        case 8: return DataDirectory08;
                        case 9: return DataDirectory09;
                        case 10: return DataDirectory10;
                        case 11: return DataDirectory11;
                        case 12: return DataDirectory12;
                        case 13: return DataDirectory13;
                        case 14: return DataDirectory14;
                        case 15: return DataDirectory15;
                        default: throw new Exception();
                    }
                }

                public delegate void Accessor(ref OptionalHeader header);
                public static void Access(byte[] content, int offset, Accessor accessor)
                {
                    fixed (byte* pContent = &content[offset])
                    {
                        OptionalHeader* pHeader = (OptionalHeader*)pContent;
                        accessor(ref *pHeader);
                    }
                }

                public static OptionalHeader Read(byte[] content, int offset)
                {
                    OptionalHeader header = new OptionalHeader();
                    Access(content, offset, delegate(ref OptionalHeader h)
                    {
                        header = h;
                    });
                    return header;
                }
            }

            public const int SizeofShortName = 8;
            public unsafe struct SectionHeader
            {
                public fixed Byte Name[SizeofShortName];
                public UInt32 Misc;
                public UInt32 PhysicalAddress { get { return Misc; } set { Misc = value; } }
                public UInt32 VirtualSize { get { return Misc; } set { Misc = value; } }
                public UInt32 VirtualAddress;
                public UInt32 SizeOfRawData;
                public UInt32 PRawData;
                public UInt32 PRelocations;
                public UInt32 PLinenumbers;
                public UInt16 NumberOfRelocations;
                public UInt16 NumberOfLinenumbers;
                public UInt32 Characteristics;
            }

            [Flags]
            public enum SectionCharacteristics : uint
            {
                Code = 0x20,
                InitializedData = 0x40,
                UninitializedData = 0x80,
                CannotBeCached = 0x04000000,
                NotPageable = 0x08000000,
                Shared = 0x10000000,
                Executable = 0x20000000,
                Readable = 0x40000000,
                Writable = 0x80000000
            }

            public struct Resource
            {
                public struct Directory
                {
                    public UInt32 Characteristics;
                    public UInt32 TimeDateStamp;
                    public UInt16 MajorVersion;
                    public UInt16 MinorVersion;
                    public UInt16 NumberOfNamedEntries;
                    public UInt16 NumberOfIdEntries;

                    public struct Entry
                    {
                        public UInt32 Name;
                        public UInt32 OffsetToData;
                    }

                    public unsafe struct String
                    {
                        public UInt16 Length;
                        public UInt16 NameString;

                        public override string ToString()
                        {
                            fixed (String* pThis = &this)
                            {
                                byte[] buffer = new byte[Length * 2];
                                IntPtr pBuffer = new IntPtr(&(pThis->NameString));
                                Marshal.Copy(pBuffer, buffer, 0, buffer.Length);
                                return System.Text.UnicodeEncoding.Default.GetString(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }

                public struct DataEntry
                {
                    public UInt32 OffsetToData;
                    public UInt32 Size;
                    public UInt32 CodePage;
                    public UInt32 Reserved;
                }
            }
        }

        public struct Reloc
        {
            public UInt16 Offset;
            public UInt16 Segment;
        }

        //public static byte[] ReadData(Header header, byte[] content)
        //{
        //    int exeDataStart = header.HeaderParagraphs * 16;
        //    int extraDataStart = header.BlocksInFile * 512;
        //    if (header.BytesInLastBlock > 0)
        //    {
        //        extraDataStart -= (512 - header.BytesInLastBlock);
        //    }
        //    int exeDataLength = extraDataStart - exeDataStart;

        //    byte[] result = new byte[exeDataLength];
        //    Array.Copy(content, exeDataStart, result, 0, exeDataLength);
        //    return result;
        //}
    }
}
