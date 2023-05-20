using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Andy010.Seria;

public static class Seria
{
    private readonly static Encoding encoding = Encoding.Unicode;
    private readonly static Dictionary<Type, ISeriaTypeHandler> handlerLookup = new()
    {
        { typeof(int), new IntTypeHandler() }
    };

    public static void AddTypeHandler(ISeriaTypeHandler typeHandler)
    {
        Type type = typeHandler.GetType();
        if(handlerLookup.ContainsKey(type))
            throw new ArgumentException($"A Handler of the type {type} already has been added", nameof(typeHandler));

        handlerLookup[type] = typeHandler;
    }

    public static void RemoveTypeHandler(Type type)
    {
        if (!handlerLookup.ContainsKey(type))
            throw new ArgumentException($"A Handler of the type {type} has not yet been added", nameof(type));

        handlerLookup.Remove(type);
    }

    public static void Serialize<T>(T obj, Stream stream)
    {
        if(!stream.CanWrite)
            throw new ArgumentException("Stream must be writable", nameof(stream));

        Type type = typeof(T);

        // Get all fields with the SeriaValue Attribute
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            .Where((field) => field.GetCustomAttribute<SeriaValueAttribute>() != null).ToArray();

        using MemoryStream outputBuffer = new();
        for(int i = 0; i < fields.Length; i++)
        {
            // Find the field and get values
            FieldInfo field = fields[i];

            Type fieldType = field.FieldType;
            object fieldValue = field.GetValue(obj);
            string fieldName = field.Name;

            // Find the handler, or throw
            if (!handlerLookup.TryGetValue(fieldType, out ISeriaTypeHandler typeHandler))
                throw new ArgumentException($"The field {fieldName} of type {fieldType} does not have a handler", nameof(T));

            // Setup handler-buffer for the type handler
            using MemoryStream fieldBuffer = new();
            using BinaryWriter writer = new(fieldBuffer, encoding, true);

            typeHandler.Serialize(fieldValue, writer);

            // Write name of the field into the output-buffer
            using BinaryWriter fieldNameWriter = new(outputBuffer, encoding, true);
            fieldNameWriter.Write(fieldName);
            fieldNameWriter.Close();

            // Write length of the field-buffer into the output-buffer
            int bufferLength = (int)fieldBuffer.Length;
            outputBuffer.Write(BitConverter.GetBytes(bufferLength));

            // Copy contents of the field-buffer into the output-buffer
            fieldBuffer.Position = 0;
            fieldBuffer.CopyTo(outputBuffer);

            writer.Close();
            fieldBuffer.Close();
        }

        // Copy everything into the given output buffer
        outputBuffer.Position = 0;
        outputBuffer.CopyTo(stream);
    }

    public static T Deserialize<T>(Stream stream)
    {
        if (!stream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(stream));

        Type type = typeof(T);

        // Get all fields with the SeriaValue Attribute
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            .Where((field) => field.GetCustomAttribute<SeriaValueAttribute>() != null).ToArray();

        Dictionary<string, FieldInfo> fieldLookup = new();
        for(int i = 0; i < fields.Length; i++)
            fieldLookup.Add(fields[i].Name, fields[i]);

        T obj = Activator.CreateInstance<T>();

        // Create a MemoryBuffer of the supplied stream
        using MemoryStream inputBuffer = new();
        stream.Position = 0;
        stream.CopyTo(inputBuffer);
        inputBuffer.Position = 0;

        while(inputBuffer.Position < inputBuffer.Length)
        {
            // Read the field name
            using BinaryReader fieldReader = new(inputBuffer, encoding, true);
            string fieldName = fieldReader.ReadString();

            // Find the field in T or throw
            if (!fieldLookup.TryGetValue(fieldName, out FieldInfo field))
                throw new ArgumentException($"No SeriaValue Field with the name of {fieldName} has been found", nameof(T));
            Type fieldType = field.FieldType;

            // Find the handler for the field or throw
            if(!handlerLookup.TryGetValue(fieldType, out ISeriaTypeHandler fieldHandler))
                throw new ArgumentException($"The type {fieldType} of field {fieldName} does not have a handler", nameof(T));

            // Find out how big the field is and allocate space
            int fieldLength = fieldReader.ReadInt32();

            byte[] fieldByteBuffer = new byte[fieldLength];
            inputBuffer.ReadExactly(fieldByteBuffer, 0, fieldLength);

            // Use the handler to read the value
            using MemoryStream fieldBuffer = new(fieldByteBuffer);
            using BinaryReader reader = new(fieldBuffer, encoding, true);

            object fieldValue = fieldHandler.Deserialize(reader);

            reader.Close();
            fieldBuffer.Close();

            field.SetValue(obj, fieldValue);
        }

        return obj;
    }
}