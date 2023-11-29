using System;
using System.IO;
using System.Reflection;

namespace Seria;

public static class SeriaSerializer
{
    public static void Serialize<TObject>(TObject obj, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanWrite)
            throw new ArgumentException("Stream isn't writable", nameof(stream));

        ObjectNameAttribute? objectName = typeof(TObject).GetCustomAttribute<ObjectNameAttribute>();
        if (objectName is not null)
        {
            stream.WriteByte(1);
            stream.Write(BitConverter.GetBytes(HashName(objectName.Name)));
        }
        else
        {
            stream.WriteByte(0);
        }

        SeriaBuffer buffer = new(stream, true);
        if (obj is ISeriaObject seriaObject)
        {
            seriaObject.Serialize(buffer);
        }
        else
        {
            throw new ArgumentException("The type " + nameof(TObject) + " isn't serializable", nameof(TObject));
        }
    }

    public static TObject Deserialize<TObject>(Stream stream) where TObject : new()
    {
        TObject obj = new();
        Deserialize(obj, stream);
        return obj;
    }

    public static void Deserialize<TObject>(TObject obj, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
            throw new ArgumentException("Stream isn't readable", nameof(stream));

        int named = stream.ReadByte();

        if (named == 1)
        {
            ObjectNameAttribute? objectName = typeof(TObject).GetCustomAttribute<ObjectNameAttribute>();
            if (objectName is not null)
            {
                Span<byte> hashBuffer = stackalloc byte[sizeof(ulong)];
                stream.ReadExactly(hashBuffer);

                ulong streamHash = BitConverter.ToUInt64(hashBuffer);
                ulong objectHash = HashName(objectName.Name);

                if (streamHash != objectHash)
                    throw new ArgumentException(typeof(TObject) + " does not have the correct name", nameof(TObject));
            }
            else
            {
                throw new ArgumentException(typeof(TObject) + " does not have an ObjectName attribute", nameof(TObject));
            }
        }

        SeriaBuffer buffer = new(stream, false);

        if(obj is ISeriaObject seriaObject)
        {
            seriaObject.Serialize(buffer);
        }
        else
        {
            throw new ArgumentException("The type " + nameof(TObject) + " isn't serializable", nameof(TObject));
        }
    }

    private static ulong HashName(in string name)
    {
        ulong hash = 0;
        byte section = 0;

        ReadOnlySpan<char> characters = name.AsSpan();
        for(int i = 0; i < characters.Length; i++)
        {
            ulong value = characters[i];
            value <<= section * 16;

            section++;
            section %= 4;

            hash ^= value;
        }

        for (int i = 0; i < 64; i++)
            hash ^= (ulong)name.Length << i;

        return hash;
    }
}
