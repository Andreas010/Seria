namespace Seria.Tests;

public partial class Tests
{
    class IntContainer : ISeriaObject, IEquatable<IntContainer>
    {
        private int number;

        public IntContainer(int number)
        {
            this.number = number;
        }

        public bool Equals(IntContainer? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return number == other.number;
        }

        public void Serialize(SeriaBuffer buffer)
        {
            buffer.SerializeValue(ref number);
        }
    }

    class ArrayContainer : ISeriaObject, IEquatable<ArrayContainer>
    {
        private double[] numbers;
        private string[] names;

        public ArrayContainer()
        {
            numbers = new double[16];
            names = new string[16];
        }

        public void Fill(int seed)
        {
            Random random = new(seed);
            for(int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = random.NextDouble() * 100;
                names[i] = "Name No. " + random.Next(1_000_000, 9_999_999);
            }
        }

        public bool Equals(ArrayContainer? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            for (int i = 0; i < numbers.Length; i++)
                if (numbers[i] != other.numbers[i])
                    return false;

            for (int i = 0; i < names.Length; i++)
                if (names[i] != other.names[i])
                    return false;

            return true;
        }

        public void Serialize(SeriaBuffer buffer)
        {
            buffer.SerializeValue(ref numbers);
            buffer.SerializeValue(ref names);
        }
    }

    class StringContainer : ISeriaObject, IEquatable<StringContainer>
    {
        private string text;

        public StringContainer(string text)
        {
            this.text = text;
        }

        public bool Equals(StringContainer? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return text.Equals(other.text);
        }

        public void Serialize(SeriaBuffer buffer)
        {
            buffer.SerializeValue(ref text);
        }
    }

    class NestedContainer : ISeriaObject, IEquatable<NestedContainer>
    {
        private NestedContainer? inside;
        private string text;

        public NestedContainer(string text)
        {
            this.text = text;
            inside = new(this);
        }

        internal NestedContainer(NestedContainer parent)
        {
            text = parent.text + " (CHILD)";
        }

        public bool Equals(NestedContainer? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return text.Equals(other.text) && inside!.text.Equals(other.inside!.text);
        }

        public void Serialize(SeriaBuffer buffer)
        {
            buffer.SerializeValue(ref text);
            inside?.Serialize(buffer);
        }
    }

    [ObjectName("NamedContainer")]
    class NamedContainer : ISeriaObject, IEquatable<NamedContainer>
    {
        private int number;

        public NamedContainer(int number)
        {
            this.number = number;
        }

        public bool Equals(NamedContainer? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return number == other.number;
        }

        public void Serialize(SeriaBuffer buffer)
        {
            buffer.SerializeValue(ref number);
        }
    }

    [ObjectName("OtherNamedContainer")]
    class OtherNamedContainer : ISeriaObject, IEquatable<OtherNamedContainer>
    {
        private int number;

        public OtherNamedContainer(int number)
        {
            this.number = number;
        }

        public bool Equals(OtherNamedContainer? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return number == other.number;
        }

        public void Serialize(SeriaBuffer buffer)
        {
            buffer.SerializeValue(ref number);
        }
    }
}
