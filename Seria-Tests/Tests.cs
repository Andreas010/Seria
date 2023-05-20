using System;
using System.IO;
using Xunit.Abstractions;

namespace Seria_Tests;

public class Tests
{
    private readonly ITestOutputHelper output;

    public Tests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TestOneField()
    {
        ClassWithOneField input = new()
        {
            numberOne = 4,
        };

        using MemoryStream s = new();
        Seria.Serialize(input, s);

        ClassWithOneField output = Seria.Deserialize<ClassWithOneField>(s);

        Assert.Equal(input, output);
    }

    [Fact]
    public void TestTwoFields()
    {
        ClassWithTwoFields input = new()
        {
            numberOne = 5,
            numberTwo = 80,
        };

        using MemoryStream s = new();
        Seria.Serialize(input, s);

        ClassWithTwoFields output = Seria.Deserialize<ClassWithTwoFields>(s);

        Assert.Equal(input, output);
    }

    [Fact]
    public void TestClassWithExtraField()
    {
        ClassWithOneField input = new()
        {
            numberOne = 988,
        };

        using MemoryStream s = new();
        Seria.Serialize(input, s);

        Seria.Deserialize<ClassWithTwoFields>(s);
    }

    [Fact]
    public void TestClassWithMissingField()
    {
        ClassWithTwoFields input = new()
        {
            numberOne = 789,
            numberTwo = 13775,
        };

        using MemoryStream s = new();
        Seria.Serialize(input, s);

        Assert.Throws<ArgumentException>(() => Seria.Deserialize<ClassWithOneField>(s));
    }
}