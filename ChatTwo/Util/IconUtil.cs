using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ChatTwo.Util;

// From Kizer: https://github.com/Soreepeong/Dalamud/blob/feature/log-wordwrap/Dalamud/Interface/Spannables/Internal/GfdFileView.cs
public readonly unsafe ref struct GfdFileView
{
    private readonly ReadOnlySpan<byte> Span;
    private readonly bool DirectLookup;

    /// <summary>Initializes a new instance of the <see cref="GfdFileView"/> struct.</summary>
    /// <param name="span">The data.</param>
    public GfdFileView(ReadOnlySpan<byte> span)
    {
        Span = span;
        if (span.Length < sizeof(GfdHeader))
            throw new InvalidDataException($"Not enough space for a {nameof(GfdHeader)}");
        if (span.Length < sizeof(GfdHeader) + (Header.Count * sizeof(GfdEntry)))
            throw new InvalidDataException($"Not enough space for all the {nameof(GfdEntry)}");

        var entries = Entries;
        DirectLookup = true;
        for (var i = 0; i < entries.Length && DirectLookup; i++)
            DirectLookup &= i + 1 == entries[i].Id;
    }

    /// <summary>Gets the header.</summary>
    private ref readonly GfdHeader Header => ref MemoryMarshal.AsRef<GfdHeader>(Span);

    /// <summary>Gets the entries.</summary>
    private ReadOnlySpan<GfdEntry> Entries => MemoryMarshal.Cast<byte, GfdEntry>(Span[sizeof(GfdHeader)..]);

    /// <summary>Attempts to get an entry.</summary>
    /// <param name="iconId">The icon ID.</param>
    /// <param name="entry">The entry.</param>
    /// <param name="followRedirect">Whether to follow redirects.</param>
    /// <returns><c>true</c> if found.</returns>
    public bool TryGetEntry(uint iconId, out GfdEntry entry, bool followRedirect = true)
    {
        if (iconId == 0)
        {
            entry = default;
            return false;
        }

        var entries = Entries;
        if (DirectLookup)
        {
            if (iconId <= entries.Length)
            {
                entry = entries[(int)(iconId - 1)];
                return !entry.IsEmpty;
            }

            entry = default;
            return false;
        }

        var lo = 0;
        var hi = entries.Length;
        while (lo <= hi)
        {
            var i = lo + ((hi - lo) >> 1);
            if (entries[i].Id == iconId)
            {
                if (followRedirect && entries[i].Redirect != 0)
                {
                    iconId = entries[i].Redirect;
                    lo = 0;
                    hi = entries.Length;
                    continue;
                }

                entry = entries[i];
                return !entry.IsEmpty;
            }

            if (entries[i].Id < iconId)
                lo = i + 1;
            else
                hi = i - 1;
        }

        entry = default;
        return false;
    }

    /// <summary>Header of a .gfd file.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GfdHeader
    {
        /// <summary>Signature: "gftd0100".</summary>
        public fixed byte Signature[8];

        /// <summary>Number of entries.</summary>
        public int Count;

        /// <summary>Unused/unknown.</summary>
        public fixed byte Padding[4];
    }

    /// <summary>An entry of a .gfd file.</summary>
    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public struct GfdEntry
    {
        /// <summary>ID of the entry.</summary>
        public ushort Id;

        /// <summary>The left offset of the entry.</summary>
        public ushort Left;

        /// <summary>The top offset of the entry.</summary>
        public ushort Top;

        /// <summary>The width of the entry.</summary>
        public ushort Width;

        /// <summary>The height of the entry.</summary>
        public ushort Height;

        /// <summary>Unknown/unused.</summary>
        public ushort Unk0A;

        /// <summary>The redirected entry, maybe.</summary>
        public ushort Redirect;

        /// <summary>Unknown/unused.</summary>
        public ushort Unk0E;

        /// <summary>Gets a value indicating whether this entry is effectively empty.</summary>
        public bool IsEmpty => Width == 0 || Height == 0;
    }
}



internal static class IconUtil
{
    private static byte[]? GfdFile;
    public static unsafe GfdFileView GfdFileView
    {
        get
        {
            GfdFile ??= Plugin.DataManager.GetFile("common/font/gfdata.gfd")!.Data;
            return new GfdFileView(new ReadOnlySpan<byte>(Unsafe.AsPointer(ref GfdFile[0]), GfdFile.Length));
        }
    }

    public static byte[] ImageToRaw(this Image<Rgba32> image)
    {
        var data = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(data);
        return data;
    }
}
