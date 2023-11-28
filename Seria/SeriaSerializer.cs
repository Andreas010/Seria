using System;
using System.IO;

namespace Seria;

public static class SeriaSerializer
{
    public static void Serialize<TObject>(TObject obj, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanWrite)
            throw new ArgumentException("Stream isn't writable", nameof(stream));

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
}
