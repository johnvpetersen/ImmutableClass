using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImmutableClassLibrary.Exceptions
{
    public class ImmutableObjectInvalidCreationException : Exception
    {
        public ImmutableObjectInvalidCreationException() : base(
            "An immutable object can only be created via the static Create<T> method.")
        {
        }
    }

    public class ImmutableObjectEditException : Exception
    {
        public ImmutableObjectEditException() : base("An immutable object cannot be changed after it has been created.")
        {
        }
    }


    public class InvalidDataTypeException : Exception
    {
        public static ImmutableHashSet<string> ValidImmutableClassTypes =
            ImmutableHashSet.Create<string>(
                "Boolean",
                "Byte",
                "SByte",
                "Char",
                "Decimal",
                "DateTime",
                "Double",
                "Single",
                "Int32",
                "UInt32",
                "Int64",
                "UInt64",
                "Int16",
                "UInt16",
                "String",
                "ImmutableBoolean",
                "ImmutableByte",
                "ImmutableSByte",
                "ImmutableChar",
                "ImmutableDecimal",
                "ImmutableDateTime",
                "ImmutableDouble",
                "ImmutableSingle",
                "ImmutableInt32",
                "ImmutableUInt32",
                "ImmutableInt64",
                "ImmutableUInt64",
                "ImmutableInt16",
                "ImmutableUInt16",
                "ImmutableString",
                "ImmutableArray",
                "ImmutableDictionary",
                "ImmutableList",
                "ImmutableHashSet",
                "ImmutableSortedDictionary",
                "ImmutableSortedSet",
                "ImmutableStack",
                "ImmutableQueue"
            );

        public InvalidDataTypeException(
            ImmutableHashSet<string> invalidProperties) : base(
            $"Properties of an instance of " +
            "ImmutableClass may only " +
            "contain the following types: Boolean, Byte, " +
            "SByte, Char, Decimal, Double, Single, " +
            "Int32, UInt32, Int64, " +
            "UInt64, Int16, UInt16, String, ImmutableArray, " +
            "ImmutableDictionary, ImmutableList, ImmutableQueue, " +
            "ImmutableSortedSet, ImmutableStack or ImmutableClass. " +
            $"Invalid property types: " +
            $"   {string.Join(",", invalidProperties.ToArray())}")
        {
            Data.Add("InvalidPropertyTypes",
                invalidProperties.ToArray());
        }
    }
}