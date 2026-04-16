using System.Collections.Generic;

namespace MyWorksheet.Webpage.Helper.FileTypeToMediaType;

public class FileMagicNumberService
{
    public FileMagicNumberService()
    {
        FileSignatures = [];
        CreateKnownList();
    }

    private void CreateKnownList()
    {
        FileSignatures.Add(new FileSignature("pcap", "application/octet-stream", new byte[] { 0xA1, 0xB2, 0xC3, 0xD4 }));
        FileSignatures.Add(new FileSignature("pcap", "application/octet-stream", new byte[] { 0xD4, 0xC3, 0xB2, 0xA1 }));
        FileSignatures.Add(new FileSignature("pcapng", "application/octet-stream", new byte[] { 0x0A, 0x0D, 0x0D, 0x0A }));
        FileSignatures.Add(new FileSignature("rpm", "application/octet-stream", new byte[] { 0x0A, 0x0D, 0x0D, 0x0A }));
        FileSignatures.Add(new FileSignature("sqlite", "application/octet-stream", new byte[] { 0x53, 0x51, 0x4c, 0x69, 0x74, 0x65, 0x20, 0x66, 0x6f, 0x72, 0x6d, 0x61, 0x74, 0x20, 0x33, 0x00 }));
        FileSignatures.Add(new FileSignature("bin", "application/octet-stream", new byte[] { 0x53, 0x50, 0x30, 0x31 }));
    }

    public ICollection<FileSignature> FileSignatures { get; private set; }
}

public struct FileSignature
{
    public FileSignature(string extension, string mediaType, params byte[] magicBytes)
    {
        MagicBytes = magicBytes;
        Extension = extension;
        MediaType = mediaType;
    }

    public string Extension { get; private set; }
    public string MediaType { get; private set; }
    public byte[] MagicBytes { get; private set; }
}