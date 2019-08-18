using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ImmutableClassLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImmutableClassLibrary.Classes
{
    public abstract class ImmutableClass
    {
        private bool _lock;


        protected ImmutableClass() : this(false)
        {
        }


        protected ImmutableClass(bool strictCreate)
        {
            if (strictCreate && new StackTrace()
                    .GetFrames()
                    .ToList()
                    .Select(x => x.GetMethod())
                    .Count(x => x.Name == "Create" &&
                                x.DeclaringType.Name == "ImmutableClass") == 0)
                throw new ImmutableObjectInvalidCreationException();

            var properties =
                GetType()
                    .GetProperties()
                    .Where(x => x.PropertyType.BaseType.Name
                                != "ImmutableClass")
                    .Select(x =>
                        x.PropertyType.Name.Substring(0,
                            (x.PropertyType.Name.Contains("`")
                                ? x.PropertyType.Name.IndexOf("`",
                                    StringComparison.Ordinal)
                                : x.PropertyType.Name.Length)))
                    .ToImmutableHashSet();

            var invalidProperties =
                properties
                    .Except(InvalidDataTypeException
                        .ValidImmutableClassTypes);

            if (invalidProperties.Count > 0)

                throw new InvalidDataTypeException(
                    invalidProperties
                );
        }

        private readonly string _token = Guid.NewGuid().ToString();

        protected void Setter<T>(string name, T value, ref T variable)
        {
            if (_lock)
                throw new ImmutableObjectEditException();
            variable = value;
        }

        public override string ToString() => $"{{\"{_token}\":{JsonConvert.SerializeObject(this)}}}";

        public bool IsEqual<T>(T objToCompare) where T : ImmutableClass =>
            !(GetType() != objToCompare.GetType() ||
              JsonConvert.SerializeObject(this) != JsonConvert.SerializeObject(objToCompare));

        public static T Create<T>(T instance) where T : ImmutableClass
        {
            instance._lock = true;
            return instance;
        }


      


        public static T Create<T>(string json, JsonConverter[] jsonConverters = null) where T : ImmutableClass
        {
            ImmutableClass retVal = JsonConvert.DeserializeObject<T>(json, jsonConverters);
            retVal._lock = true;
            return (T) retVal;
        }

        public static JObject Convert<T>(T immutableClass) =>
            JsonConvert
                .DeserializeObject<JObject>
                (
                    JsonConvert
                        .SerializeObject(
                            immutableClass
                        )
                );
    }



}