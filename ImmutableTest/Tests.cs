using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using ImmutableClassLibrary.Classes;
using ImmutableClassLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;


namespace ImmutableClassLibraryTests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class Tests
    {

        [Test]
        public void CanGetToken()

        
        {
            var obj = new { FirstName = "John", LastName = "Petersen" };

            var json = JsonConvert.SerializeObject(obj);

            var sut = ImmutableClass.Create<ImmutableTest>(json);

            var token = sut.GetToken();

            Regex.Match(@"^[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$", token);
        }






        [Test]
        public void VerifyFromAnonymousObject()
        {
            var obj = new {FirstName = "John", LastName = "Petersen"};

            var json = JsonConvert.SerializeObject(obj);

            var sut = ImmutableClass.Create<ImmutableTest>(json);

            Assert.IsTrue(sut.IsLocked());
        }

        [Test]
        public void VerifyEquals()
        {

            var json1 =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Schools\":{\"MBA\":{\"Institution\":\"St. Joseph\'s University\",\"Year\":\"1993\",\"Degree\":\"MBA\"},\"JD\":{\"Institution\":\"Rutgers University School of Law\",\"Year\":\"2004\",\"Degree\":\"JD\"},\"BS\":{\"Institution\":\"Mansfield University\",\"Year\":\"1988\",\"Degree\":\"BS\"}}}";

            var json2 =
                "{\"FirstName\":\"JOHN\",\"LastName\":\"Petersen\",\"Schools\":{\"MBA\":{\"Institution\":\"St. Joseph\'s University\",\"Year\":\"1993\",\"Degree\":\"MBA\"},\"JD\":{\"Institution\":\"Rutgers University School of Law\",\"Year\":\"2004\",\"Degree\":\"JD\"},\"BS\":{\"Institution\":\"Mansfield University\",\"Year\":\"1988\",\"Degree\":\"BS\"}}}";

            var obj1 = ImmutableClass.Create<ImmutableTest>(json1);
            var obj2 = ImmutableClass.Create<ImmutableTest>(json2);

            Assert.IsFalse(obj1.IsEqual(obj2).IsEqual);

            obj1 = new ImmutableTest() {FirstName = "John"};
            obj1.Lock();

            obj2 = new ImmutableTest() { FirstName = "John" };

            Assert.IsFalse(obj1.IsEqual(obj2).IsEqual);

            obj2.Lock();

            Assert.IsTrue(obj1.IsEqual(obj2).IsEqual);

            var address = new Address();

            var results = address.IsEqual(obj1);


            Assert.IsFalse(results.IsEqual);



        }


        [Test]
        public void CanLock()
        {
            var sut = new ImmutableTest() { FirstName = "John" };

            var propertyStatus = sut.GetPropertyStatus();

            Assert.IsTrue(propertyStatus["FirstName"]);
            Assert.IsFalse(propertyStatus["Schools"]);
            Assert.IsFalse(propertyStatus["LastName"]);

            Assert.IsFalse(sut.IsLocked());

            sut.Lock();

            propertyStatus = sut.GetPropertyStatus();

            Assert.IsTrue(propertyStatus["FirstName"]);
            Assert.IsTrue(propertyStatus["Schools"]);
            Assert.IsTrue(propertyStatus["LastName"]);

            Assert.IsTrue(sut.IsLocked());
        }

        [Test]
        public void VerifyPropertyStatus()
        {
            var sut = new ImmutableTest() {FirstName = "John"};

            var propertyStatus = sut.GetPropertyStatus();

            Assert.IsFalse(propertyStatus["Schools"]);
            Assert.IsFalse(propertyStatus["LastName"]);
        }




        [Test]
        public void VerifyRoundTripWithNoException()
        {
            var json =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Schools\":{\"MBA\":{\"Institution\":\"St. Joseph\'s University\",\"Year\":\"1993\",\"Degree\":\"MBA\"},\"JD\":{\"Institution\":\"Rutgers University School of Law\",\"Year\":\"2004\",\"Degree\":\"JD\"},\"BS\":{\"Institution\":\"Mansfield University\",\"Year\":\"1988\",\"Degree\":\"BS\"}}}";


            var sut = ImmutableClass.Create<ImmutableTest>(json);

            json = sut.ToString();

            var jObject = JObject.Parse(json);

            jObject["FirstName"] = "JOHN";

            jObject["Schools"]["MBA"]["Degree"] = "M.B.A.";

            sut = ImmutableClass.Create<ImmutableTest>(jObject.ToString());

            Assert.AreEqual("M.B.A.", sut.Schools["MBA"].Degree);
        }


        [Test]
        public void CanDeserializeNewInstanceWithUpdatedFirstNameWithException()
        {
            var expected = "Error setting FirstName. Immutable Class Instance Properties are readonly.";


            var json =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Schools\":{\"MBA\":{\"Institution\":\"St. Joseph\'s University\",\"Year\":\"1993\",\"Degree\":\"MBA\"},\"JD\":{\"Institution\":\"Rutgers University School of Law\",\"Year\":\"2004\",\"Degree\":\"JD\"},\"BS\":{\"Institution\":\"Mansfield University\",\"Year\":\"1988\",\"Degree\":\"BS\"}}}";

            var exception =
                Assert.Throws<InvalidPropertySettingAttempt<string>>(() =>
                {
                    var sut = ImmutableClass.Create<ImmutableTest>(json);

                    sut.FirstName = "JOHN";
                });

            Assert.AreEqual(expected, exception.Message);
        }


        [Test]
        public void CanDeserializeNewInstanceWithUpdatedFirstName()
        {
            var expected = "JOHN";

            var json =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Schools\":{\"MBA\":{\"Institution\":\"St. Joseph\'s University\",\"Year\":\"1993\",\"Degree\":\"MBA\"},\"JD\":{\"Institution\":\"Rutgers University School of Law\",\"Year\":\"2004\",\"Degree\":\"JD\"},\"BS\":{\"Institution\":\"Mansfield University\",\"Year\":\"1988\",\"Degree\":\"BS\"}}}";


            var sut = ImmutableClass.Create<ImmutableTest>(json.Replace("John", expected));


            Assert.AreEqual(expected, sut.FirstName);
        }


        [Test]
        public void VerifyJsonToString()
        {
            var sut = new ImmutableTest()
            {
                FirstName = "John",
                LastName = "Petersen",
                Schools = (new Dictionary<string, School>()
                {
                    {"BS", CreateSchool("Mansfield University", "1988", "BS")},
                    {"MBA", CreateSchool("St. Joseph's University", "1993", "MBA")},
                    {"JD", CreateSchool("Rutgers University School of Law", "2004", "JD")}
                }).ToImmutableDictionary()
            };


            var expected =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Schools\":{\"MBA\":{\"Institution\":\"St. Joseph\'s University\",\"Year\":\"1993\",\"Degree\":\"MBA\"},\"JD\":{\"Institution\":\"Rutgers University School of Law\",\"Year\":\"2004\",\"Degree\":\"JD\"},\"BS\":{\"Institution\":\"Mansfield University\",\"Year\":\"1988\",\"Degree\":\"BS\"}}}";


            var actual = sut.ToString();


            Assert.AreEqual(expected, sut.ToString());
        }


        [Test]
        public void AttemptDictionaryImmutablePropertyWithExceptionTrueThrowsException()
        {
            var expected = "Error setting Schools. Immutable Class Instance Properties are readonly.";


            var exception = Assert.Throws<InvalidPropertySettingAttempt<ImmutableDictionary<string, School>>>(() =>
            {
                var sut = new ImmutableTest()
                {
                    FirstName = "John",
                    LastName = "Petersen",
                    Schools = (new Dictionary<string, School>()
                    {
                        {"BS", CreateSchool("Mansfield University", "1988", "BS")},
                        {"MBA", CreateSchool("St. Joseph's University", "1993", "MBA")},
                        {"JD", CreateSchool("Rutgers University School of Law", "2004", "JD")}
                    }).ToImmutableDictionary()
                };

                sut.Schools = new Dictionary<string, School>().ToImmutableDictionary();
            });

            Assert.AreEqual(expected, exception.Message);
        }


        [Test]
        public void AttemptStringImmutablePropertyWithExceptionTrueThrowsException()
        {
            Assert.Throws<InvalidPropertySettingAttempt<string>>(() =>
            {
                var sut = new ImmutableTest() {FirstName = "John"};
                sut.FirstName = "FOO";
            });
        }

        [Test]
        public void AttemptToDefineInvalidPropertyTypeThrowsException()
        {
            var expectedMessage =
                "Properties of an instance of ImmutableClass may only contain the following types: Boolean, Byte, " +
                "SByte, Char, Decimal, Double, Single, Int32, UInt32, Int64, UInt64, Int16, UInt16, String, ImmutableArray, " +
                "ImmutableDictionary, ImmutableList, ImmutableQueue, ImmutableSortedSet, ImmutableStack or ImmutableClass. " +
                "Invalid property types: List";
            var exception = Assert.Throws<InvalidDataTypeException>(
                () =>
                {
                    var sut = new InvalidImmutableTestDefintion();
                }
            );

            Assert.AreEqual(expectedMessage, exception.Message);
        }



        public class InvalidImmutableTestDefintion : ImmutableClass
        {
            public List<string> InvalidProperty { get; set; }
        }

        [Test]
        public void TestCreatePerson()
        {
            var json =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Address\":{},\"Schools\":[\"Mansfield\",\"St. Joseph's\",\"Rutgers\"]}";

            var person = ImmutableClass.Create<Person>(json);


            var wrappedJSON = person.ToString(true);

            var person2 = ImmutableClass.Create<Person>(wrappedJSON);
        }




        public class Person : ImmutableClass
        {

            private string _firstName;

            public string FirstName
            {
                get => _firstName;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _firstName);
            }

            private string _lastName;

            public string LastName
            {
                get => _lastName;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _lastName);
            }


            private Address _address;

            public Address Address
            {
                get => _address;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _address);

            }

            private ImmutableArray<string> _schools;

            public ImmutableArray<string> Schools
            {
                get => _schools;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _schools);
            }
        }

        public class Address : ImmutableClass
        {
        }






        public class ImmutableTest : ImmutableClass
        {
            private string _firstName;

            public string FirstName
            {
                get => _firstName;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _firstName);
            }

            private string _lastName;

            public string LastName
            {
                get => _lastName;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _lastName);
            }

            private ImmutableDictionary<string, School> _schools;

            public ImmutableDictionary<string, School> Schools
            {
                get => _schools;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _schools);
            }
        }

        public class School : ImmutableClass
        {
            private string _institution;

            public string Institution
            {
                get => _institution;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _institution);
            }

            private string _year;

            public string Year
            {
                get => _year;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _year);
            }

            private string _degree;

            public string Degree
            {
                get => _degree;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _degree);
            }
        }

        public static School CreateSchool(string institution, string year, string degree)
        {
            return new School() {Institution = institution, Year = year, Degree = degree};
        }
    }
}