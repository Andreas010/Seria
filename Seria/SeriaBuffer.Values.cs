using System.Runtime.InteropServices;
using System.Text;

namespace Seria;

public sealed partial class SeriaBuffer
{
    public void SerializeValue<T>(ref T value) where T : unmanaged
    {
        if (writeMode)
            stream.Write(GetBytes(value));
        else
        {
            Span<byte> buffer = stackalloc byte[TypeSize<T>.Size];
            stream.ReadExactly(buffer);
            value = GetValue<T>(buffer);
        }
    }
    public void SerializeValue<T>(ref T[] value) where T : unmanaged
    {
        if (writeMode)
        {
            int length = value.Length;
            stream.Write(GetBytes(length));
            for (int i = 0; i < length; i++)
            {
                stream.Write(GetBytes(value[i]));
            }
        }
        else
        {
            Span<byte> lengthBuffer = stackalloc byte[sizeof(int)];
            stream.ReadExactly(lengthBuffer);
            int length = GetValue<int>(lengthBuffer);

            T[] results = new T[length];
            Span<byte> buffer = stackalloc byte[TypeSize<T>.Size];
            for (int i = 0; i < length; i++)
            {
                stream.ReadExactly(buffer);
                results[i] = GetValue<T>(buffer);
            }

            value = results;
        }
    }

    public void SerializeValue(ref string value)
    {
        if (writeMode)
        {
            int stringLength = value.Length;
            int bufferSize = sizeof(int) + sizeof(char) * stringLength;

            Span<byte> lengthBuffer = GetBytes(stringLength);
            ReadOnlySpan<byte> stringBuffer = MemoryMarshal.AsBytes(value.AsSpan());

            Span<byte> buffer = new byte[bufferSize];
            lengthBuffer.CopyTo(buffer);

            stringBuffer.CopyTo(buffer.Slice(sizeof(int)));

            stream.Write(buffer);
        }
        else
        {
            Span<byte> lengthBuffer = stackalloc byte[sizeof(int)];
            stream.ReadExactly(lengthBuffer);
            int length = GetValue<int>(lengthBuffer);

            Span<byte> stringBuffer = new byte[length * sizeof(char)];
            stream.ReadExactly(stringBuffer);

            value = Encoding.Unicode.GetString(stringBuffer);
        }
    }

    public void SerializeValue(ref string[] value)
    {
        if (writeMode)
        {
            int arrayLength = value.Length;

            stream.Write(GetBytes(arrayLength));
            for (int i = 0; i < arrayLength; i++)
            {
                string currentString = value[i];

                int stringLength = currentString.Length;
                int bufferSize = sizeof(int) + sizeof(char) * stringLength;

                Span<byte> lengthBuffer = GetBytes(stringLength);
                ReadOnlySpan<byte> stringBuffer = MemoryMarshal.AsBytes(currentString.AsSpan());

                Span<byte> buffer = new byte[bufferSize];
                lengthBuffer.CopyTo(buffer);

                stringBuffer.CopyTo(buffer.Slice(sizeof(int)));

                stream.Write(buffer);
            }
        }
        else
        {
            Span<byte> lengthBuffer = stackalloc byte[sizeof(int)];
            stream.ReadExactly(lengthBuffer);
            int resultsLength = GetValue<int>(lengthBuffer);

            string[] results = new string[resultsLength];

            for (int i = 0; i < resultsLength; i++)
            {
                stream.ReadExactly(lengthBuffer);
                int length = GetValue<int>(lengthBuffer);

                Span<byte> stringBuffer = new byte[length * sizeof(char)];
                stream.ReadExactly(stringBuffer);

                results[i] = Encoding.Unicode.GetString(stringBuffer);
            }

            value = results;
        }
    }
}