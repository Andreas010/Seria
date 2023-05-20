using System;

namespace Seria_Tests;

public class ClassWithOneField : IEquatable<ClassWithOneField>
{
    [SeriaValue]
    public int numberOne;

    public bool Equals(ClassWithOneField other)
    {
        return numberOne == other.numberOne;
    }
}

public class ClassWithTwoFields : IEquatable<ClassWithTwoFields>
{
    [SeriaValue]
    public int numberOne;

    [SeriaValue]
    public int numberTwo;

    public bool Equals(ClassWithTwoFields other)
    {
        return numberOne == other.numberOne && numberTwo == other.numberTwo;
    }
}