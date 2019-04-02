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
            var token = ImmutableClass.Create<ImmutableTest>(
                JsonConvert.SerializeObject(
                    new { FirstName = "John", LastName = "Petersen" }))
                .ToString().Substring(2,32);

            Regex.Match(@"^[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$", token);
        }



        [Test]
        public void VerifyIsEqual()
        {


            var json = 
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Schools\":{\"MBA\":{\"Institution\":\"St. Joseph\'s University\",\"Year\":\"1993\",\"Degree\":\"MBA\"},\"JD\":{\"Institution\":\"Rutgers University School of Law\",\"Year\":\"2004\",\"Degree\":\"JD\"},\"BS\":{\"Institution\":\"Mansfield University\",\"Year\":\"1988\",\"Degree\":\"BS\"}}}";

            var obj1 = ImmutableClass.Create<ImmutableTest>(json);

            var x = ImmutableClass.Convert<ImmutableTest>(obj1);

           x["FirstName"] = "JOHN";

           var obj2 = ImmutableClass.Create<ImmutableTest>(x.ToString());


           Assert.IsFalse(obj1.IsEqual(obj2));

           x["FirstName"] = "John";

           obj2 = ImmutableClass.Create<ImmutableTest>(x.ToString());

           Assert.IsTrue(obj1.IsEqual(obj2));


        }


        [Test]
        public void VerifyFromAnonymousObject()
        {
            var obj = new {FirstName = "John", LastName = "Petersen"};

            var json = JsonConvert.SerializeObject(obj);

            var sut = ImmutableClass.Create<ImmutableTest>(json);

        }

      

        [Test]
        public void VerifyRoundTripWithNoException()
        {
            var json =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Schools\":{\"MBA\":{\"Institution\":\"St. Joseph\'s University\",\"Year\":\"1993\",\"Degree\":\"MBA\"},\"JD\":{\"Institution\":\"Rutgers University School of Law\",\"Year\":\"2004\",\"Degree\":\"JD\"},\"BS\":{\"Institution\":\"Mansfield University\",\"Year\":\"1988\",\"Degree\":\"BS\"}}}";

            var sut = ImmutableClass.Create<ImmutableTest>(json);




            json = sut.ToString();
            var token = json.Substring(2, 36);

            var jObject = JObject.Parse(json);

            var jx = jObject.ToString();

            jObject[token]["FirstName"] = "JOHN";

            jObject[token]["Schools"]["MBA"]["Degree"] = "M.B.A.";

            json = jObject.ToString();


            json = json.Substring(45, (json.Length - 46));


            sut = ImmutableClass.Create<ImmutableTest>(json);

            token = sut.ToString().Substring(2, 36);
            Assert.AreEqual("M.B.A.", sut.Schools["MBA"].Degree);
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
            var sut = ImmutableClass.Create<ImmutableTest>("{\"FirstName\":\"John\",\"LastName\":\"Petersen\"}");

        }


        [Test]
        public void AttemptDictionaryImmutablePropertyWithExceptionTrueThrowsException()
        {
            var expected = "An immutable object cannot be changed after it has been created.";

            var exception = Assert.Throws<ImmutableObjectEditException>(() =>
            {
                var obj = new
                {
                    FirstName = "John",
                    LastName = "Petersen"
                };

                var sut = ImmutableClass.Create<ImmutableTest>(JsonConvert.SerializeObject(obj));
                sut.FirstName = "FOO";
            });

            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void AttemptToDefineInvalidPropertyTypeThrowsException()
        {
            var expected =
                "Properties of an instance of ImmutableClass may only contain the following types: Boolean, Byte, SByte, Char, Decimal, Double, Single, Int32, UInt32, Int64, UInt64, Int16, UInt16, String, ImmutableArray, ImmutableDictionary, ImmutableList, ImmutableQueue, ImmutableSortedSet, ImmutableStack or ImmutableClass. Invalid property types: List";


            var exception = Assert.Throws<TargetInvocationException>(
                () =>
                {
                    ImmutableClass.Create<InvalidImmutableTestDefintion>();
                }
            );

            Assert.AreEqual(expected, exception.InnerException.Message);
            Console.WriteLine();
        }

        public class InvalidImmutableTestDefintion : ImmutableClass
        {

            public string FirstName { get; set; }
            public List<string> InvalidProperty { get; set; }
        }

        [Test]
        public void TestCreatePerson()
        {
            var json =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Address\":{},\"Schools\":[\"Mansfield\",\"St. Joseph's\",\"Rutgers\"]}";

            var person = ImmutableClass.Create<Person>(json);

            Assert.AreEqual("John",person.FirstName);
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

            private ImmutableArray<string> _schools;

            public ImmutableArray<string> Schools
            {
                get => _schools;
                set => Setter(MethodBase.GetCurrentMethod().Name.Substring(4), value, ref _schools);
            }
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

    }
}