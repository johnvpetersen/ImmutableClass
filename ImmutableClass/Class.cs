using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImmutableClassLibrary.Exceptions;
using Newtonsoft.Json;

namespace ImmutableClassLibrary.Classes
{
    public abstract class ImmutableClass
    {
        private readonly Dictionary<string, bool> _setProperties;

  

        protected ImmutableClass()
        {
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

        public string GetToken() => _token;


        public ImmutableDictionary<string, bool> Lock()
        {
            var toUpdate = _setProperties.Where(x => x.Value == false).ToList();
            foreach (var item in toUpdate)
                _setProperties[item.Key] = true;

            return GetPropertyStatus();

        }

        public bool IsLocked()
        {
           var retVal = (_setProperties.Count(x => x.Value == false) == 0);

           return retVal;

        }

        public ImmutableDictionary<string, bool> GetPropertyStatus()
        {
            return _setProperties.ToImmutableDictionary();
        }


        protected void Setter<T>(string name, T value, ref T variable)
        {
            if (_setProperties[name])
                throw new InvalidPropertySettingAttempt<T>(name, value);

            _setProperties[name] = true;
            variable = value;
        }

        public string ToString(bool withToken) =>  $"{{\"{_token}\":{ToString()}}}";


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        public ( bool IsEqual, Dictionary<string,string> Details) IsEqual<T>(T objToCompare) where T : ImmutableClass
        {

            var message = new Dictionary<string, string>();


            if (GetType() != objToCompare.GetType())
            {
                message.Add("TYPE", "Compared object type is not consistent.");
                return (IsEqual: false, Details: message);
            }

            if (IsLocked() != objToCompare.IsLocked())
                message.Add("LOCKED", "Compared object locked status is not consistent.");
            
            if (ToString() != objToCompare.ToString())
                message.Add("PROPERTIES","Compared object property values are not consistent.");

            if (message.Count > 0)

                return (IsEqual: false, Details: message);

            return (IsEqual: true, Details: null);
        }

        public static T  Create<T>(string json) where T : ImmutableClass
        {

            ImmutableClass retVal = JsonConvert.DeserializeObject<T>(json);

            retVal.Lock();

            return (T) retVal;
        }
    }
}
