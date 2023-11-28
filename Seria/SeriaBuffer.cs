using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Seria;

public sealed partial class SeriaBuffer
{
    private readonly Stream stream;
    private readonly bool writeMode;

    internal SeriaBuffer(Stream stream, bool writeMode)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if ((!stream.CanRead && !writeMode) || (!stream.CanWrite && writeMode))
            throw new ArgumentException(nameof(writeMode) + " is set to " + writeMode + ", but the stream can't do that", nameof(stream));

        this.writeMode = writeMode;
        this.stream = stream;
    }

    private static class TypeSize<T>
    {
        public readonly static int Size;

        static TypeSize()
        {
            var dm = new DynamicMethod("SizeOfType", typeof(int), []);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Sizeof, typeof(T));
            il.Emit(OpCodes.Ret);
            Size = (int)dm.Invoke(null, null)!;
        }
    }

    private byte[] GetBytes<TObject>(TObject value) where TObject : unmanaged
    {
        byte[] buffer = new byte[TypeSize<TObject>.Size];
        Unsafe.As<byte, TObject>(ref buffer[0]) = value;
        return buffer;
    }

    private TObject GetValue<TObject>(Span<byte> buffer) where TObject : unmanaged
    {
        return Unsafe.ReadUnaligned<TObject>(ref MemoryMarshal.GetReference(buffer));
    }
}
