using System;
using System.IO;

namespace Andy010.Seria;

public interface ISeriaTypeHandler
{
    public Type GetFormatterType();

    public void Serialize(object value, BinaryWriter writer);
    public object Deserialize(BinaryReader reader);
}

public abstract class SeriaTypeHandler<T> : ISeriaTypeHandler
{
    public abstract void Serialize(T value, BinaryWriter writer);
    public abstract T Deserialize(BinaryReader reader);

    public Type GetFormatterType() => typeof(T);

    public void Serialize(object value, BinaryWriter writer) => Serialize((T)value, writer);
    object ISeriaTypeHandler.Deserialize(BinaryReader reader) => Deserialize(reader);
}
