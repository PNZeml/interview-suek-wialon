using System.Buffers;

namespace Suek.Interview.Wialon.Utils;

internal static class ByteReader {
    public static ReadOnlyMemory<byte> GetBuffer(ref ReadOnlySequence<byte> buffer) {
        // When sequence has a single segment there is no need to make additional work
        if (buffer.IsSingleSegment) {
            return buffer.First;
        }

        // Otherwise it is required to concat sequence's chunks by copying
        // them into single array.
        byte[]? pkgBufferCopy = null;

        try {
            var packageBufferLength = Convert.ToInt32(buffer.Length);

            pkgBufferCopy = ArrayPool<byte>.Shared.Rent(packageBufferLength);

            buffer.CopyTo(pkgBufferCopy);

            return pkgBufferCopy;
        } finally {
            if (pkgBufferCopy != null) {
                ArrayPool<byte>.Shared.Return(pkgBufferCopy);
            }
        }
    }
}
