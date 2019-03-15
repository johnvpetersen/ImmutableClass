using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ImmutableClassLibrary.Exceptions;
using Newtonsoft.Json;

namespace ImmutableClassLibrary.Classes
{
    public abstract class ImmutableClass
    {
        private readonly Dictionary<string, bool> _setProperties;
        private bool _lock;
        private bool _requireCreateMethod;

        protected ImmutableClass()
        {

          if( _requireCreateMethod &&  new StackTrace().GetFrames().ToList().Select(x => x.GetMethod()).Where(x => x.Name == "Create" && x.DeclaringType.Name == "ImmutableClass").Count() == 0)
                 throw new ImmutableObjectInvalidCreationException();

          var properties =
                GetType().GetProperties().Where(x => x.PropertyType.BaseType.Name != "ImmutableClass")
                    .Select(x => x.PropertyType.Name.Substring(0,
                        (x.PropertyType.Name.Contains("`")
                            ? x.PropertyType.Name.IndexOf("`", StringComparison.Ordinal)
                            : x.PropertyType.Name.Length))).ToImmutableHashSet();

            var invalidProperties = properties.Except(InvalidDataTypeException.ValidImmutableClassTypes);


            if (invalidProperties.Count > 0)

                throw new InvalidDataTypeException(invalidProperties);

         _setProperties =  GetType().GetProperties().ToDictionary(x => x.Name, x => false);

        }

        private readonly string _token = Guid.NewGuid().ToString();


        protected T Getter<T>(string name, ref T variable)
        {
            return variable;
        }

        protected void Setter<T>(string name, T value, ref T variable)
        {

            if (_lock)
                throw new ImmutableObjectEditException();

            if (_setProperties[name])
                throw new InvalidPropertySettingAttempt<T>(name, value);

            _setProperties[name] = true;
            variable = value;
        }

        public string ToString(bool withToken) =>  $"{{\"{_token}\":{ToString()}}}";

        public override string ToString() =>  JsonConvert.SerializeObject(this);
        
        public bool IsEqual<T>(T objToCompare) where T : ImmutableClass =>  !(GetType() != objToCompare.GetType() || ToString() != objToCompare.ToString());


        public static T  Create<T>(string json) where T : ImmutableClass
        {
            ImmutableClass retVal = JsonConvert.DeserializeObject<T>(json);

            retVal._lock = true;

            return (T) retVal;
        }

        public static T Lock<T>(T immutableInstance) where T : ImmutableClass
        {
            immutableInstance._lock = true;
            return immutableInstance;
        }
    }
}
