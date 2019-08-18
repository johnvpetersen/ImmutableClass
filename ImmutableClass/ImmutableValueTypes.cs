using System;

namespace ImmutableClassLibrary
{

    public static class ImmutableExtension
    {

        public static ImmutableBoolean ToImmutable(this Boolean value)
        {
            return new ImmutableBoolean(value);
        }

        public static ImmutableByte ToImmutable(this Byte value)
        {
            return new ImmutableByte(value);
        }

        public static ImmutableSByte ToImmutable(this SByte value)
        {
            return new ImmutableSByte(value);
        }

        public static ImmutableChar ToImmutable(this Char value)
        {
            return new ImmutableChar(value);
        }

        public static ImmutableDecimal ToImmutable(this Decimal value)
        {
            return new ImmutableDecimal(value);
        }

        public static ImmutableDouble ToImmutable(this Double value)
        {
            return new ImmutableDouble(value);
        }

        public static ImmutableSingle ToImmutable(this Single value)
        {
            return new ImmutableSingle(value);
        }

        public static ImmutableInt32 ToImmutable(this Int32 value)
        {
            return new ImmutableInt32(value);
        }

        public static ImmutableInt16 ToImmutable(this Int16 value)
        {
            return new ImmutableInt16(value);
        }

        public static ImmutableInt64 ToImmutable(this Int64 value)
        {
            return new ImmutableInt64(value);
        }

        public static ImmutableUInt32 ToImmutable(this UInt32 value)
        {
            return new ImmutableUInt32(value);
        }

        public static ImmutableUInt16 ToImmutable(this UInt16 value)
        {
            return new ImmutableUInt16(value);
        }

        public static ImmutableUInt64 ToImmutable(this UInt64 value)
        {
            return new ImmutableUInt64(value);
        }

        public static ImmutableString ToImmutable(this String value)
        {
            return new ImmutableString(value);
        }
    }



    public class ImmutableString : ImmutableType<String>
    {
        public ImmutableString(String value) : base(value)
        {

        }

        public static ImmutableString Create(string value) => new ImmutableString(value);


    }


    public class ImmutableBoolean : ImmutableType<Boolean>
    {
        public ImmutableBoolean(Boolean value = false) : base(value)
        {

        }

        public static ImmutableBoolean Create(Boolean value) => new ImmutableBoolean(value);


    }

    public class ImmutableByte : ImmutableType<Byte>
    {
        public ImmutableByte(Byte value) : base(value)
        {

        }

        public static ImmutableByte Create(Byte value) => new ImmutableByte(value);


    }

    public class ImmutableSByte : ImmutableType<SByte>
    {
        public ImmutableSByte(SByte value) : base(value)
        {

        }

        public static ImmutableSByte Create(SByte value) => new ImmutableSByte(value);

    }

    public class ImmutableChar : ImmutableType<Char>
    {
        public ImmutableChar(Char value) : base(value)
        {

        }

        public static ImmutableChar Create(Char value) => new ImmutableChar(value);

    }


    public class ImmutableDecimal : ImmutableType<Decimal>
    {
        public ImmutableDecimal(Decimal value) : base(value)
        {

        }

        public static ImmutableDecimal Create(Decimal value) => new ImmutableDecimal(value);

    }

    public class ImmutableDouble : ImmutableType<Double>
    {
        public ImmutableDouble(Double value) : base(value)
        {

        }

        public static ImmutableDouble Create(Double value) => new ImmutableDouble(value);

    }

    public class ImmutableSingle : ImmutableType<Single>
    {
        public ImmutableSingle(Single value) : base(value)
        {

        }

        public static ImmutableSingle Create(Single value) => new ImmutableSingle(value);

    }

    public class ImmutableInt32 : ImmutableType<Int32>
    {
        public ImmutableInt32(Int32 value) : base(value)
        {

        }

        public static ImmutableInt32 Create(Int32 value) => new ImmutableInt32(value);

    }

    public class ImmutableInt16 : ImmutableType<Int16>
    {
        public ImmutableInt16(Int16 value) : base(value)
        {

        }

        public static ImmutableInt16 Create(Int16 value) => new ImmutableInt16(value);

    }


    public class ImmutableInt64 : ImmutableType<Int64>
    {
        public ImmutableInt64(Int64 value) : base(value)
        {

        }

        public static ImmutableInt64 Create(Int64 value) => new ImmutableInt64(value);

    }



    public class ImmutableUInt32 : ImmutableType<UInt32>
    {
        public ImmutableUInt32(UInt32 value) : base(value)
        {

        }

        public static ImmutableUInt32 Create(UInt32 value) => new ImmutableUInt32(value);

    }

    public class ImmutableUInt16 : ImmutableType<UInt16>
    {
        public ImmutableUInt16(UInt16 value) : base(value)
        {

        }

        public static ImmutableUInt16 Create(UInt16 value) => new ImmutableUInt16(value);

    }


    public class ImmutableUInt64 : ImmutableType<UInt64>
    {
        public ImmutableUInt64(UInt64 value) : base(value)
        {

        }

        public static ImmutableUInt64 Create(UInt64 value) => new ImmutableUInt64(value);

    }

    public abstract class ImmutableType<T>
    {
        private readonly T _value;


        public ImmutableType(T value)
        {

            _value = value;
        }

        public T Value => _value;


        public static implicit operator T(ImmutableType<T> immutableValue) => immutableValue.Value;

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }

}
