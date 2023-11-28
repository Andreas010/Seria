﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Seria.Tests;

public partial class Tests
{
    private readonly ITestOutputHelper logger;

    public Tests(ITestOutputHelper logger)
    {
        this.logger = logger;
    }

    [Fact]
    void SerializeAndDeserializeIntContainer()
    {
        using MemoryStream stream = new(sizeof(int));

        // Write the Object to the Stream
        IntContainer startContainer = new(5);
        SeriaSerializer.Serialize(startContainer, stream);

        // Reset State
        stream.Position = 0;

        // Read the Object from the Stream
        IntContainer endContainer = new(0);
        SeriaSerializer.Deserialize(endContainer, stream);

        Assert.Equal(startContainer, endContainer);

        // Dump to Logger
        StringBuilder builder = new("0x");
        stream.Position = 0;
        int value;
        while((value = stream.ReadByte()) != -1) {
            builder.AppendFormat("{0:x2}", value);
        }

        logger.WriteLine("Stream Dump:\n" + builder.ToString());
    }

    [Fact]
    void SerializeAndDeserializeStringContainer()
    {
        using MemoryStream stream = new(sizeof(int) + sizeof(char) * 24);

        // Write the Object to the Stream
        StringContainer startContainer = new("This is a Test String!!!");
        SeriaSerializer.Serialize(startContainer, stream);

        // Reset State
        stream.Position = 0;

        // Read the Object from the Stream
        StringContainer endContainer = new("");
        SeriaSerializer.Deserialize(endContainer, stream);

        Assert.Equal(startContainer, endContainer);

        // Dump to Logger
        StringBuilder builder = new("0x");
        stream.Position = 0;
        int value;
        while ((value = stream.ReadByte()) != -1)
        {
            builder.AppendFormat("{0:x2}", value);
        }

        logger.WriteLine("Stream Dump:\n" + builder.ToString());
    }

    [Fact]
    void SerializeAndDeserializeArrayContainer()
    {
        using MemoryStream stream = new(sizeof(int) + sizeof(char) * 24);

        // Write the Object to the Stream
        ArrayContainer startContainer = new();
        startContainer.Fill(69420);
        SeriaSerializer.Serialize(startContainer, stream);

        // Reset State
        stream.Position = 0;

        // Read the Object from the Stream
        ArrayContainer endContainer = new();
        SeriaSerializer.Deserialize(endContainer, stream);

        Assert.Equal(startContainer, endContainer);

        // Dump to Logger
        StringBuilder builder = new("0x");
        stream.Position = 0;
        int value;
        while ((value = stream.ReadByte()) != -1)
        {
            builder.AppendFormat("{0:x2}", value);
        }

        logger.WriteLine("Stream Dump:\n" + builder.ToString());
    }

    [Fact]
    void SerializeAndDeserializeNestedContainer()
    {
        using MemoryStream stream = new(sizeof(int) + sizeof(char) * 24);

        // Write the Object to the Stream
        NestedContainer startContainer = new("Cool");
        SeriaSerializer.Serialize(startContainer, stream);

        // Reset State
        stream.Position = 0;

        // Read the Object from the Stream
        NestedContainer endContainer = new("Not so cool");
        SeriaSerializer.Deserialize(endContainer, stream);

        Assert.Equal(startContainer, endContainer);

        // Dump to Logger
        StringBuilder builder = new("0x");
        stream.Position = 0;
        int value;
        while ((value = stream.ReadByte()) != -1)
        {
            builder.AppendFormat("{0:x2}", value);
        }

        logger.WriteLine("Stream Dump:\n" + builder.ToString());
    }
}
