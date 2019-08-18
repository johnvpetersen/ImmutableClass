using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ImmutableClassLibrary;
using ImmutableClassLibrary.Classes;
using ImmutableClassLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;


namespace ImmutableClassLibraryTests
{
    [ExcludeFromCodeCoverage]
    public class SimpleImmutableClass
    {
        public SimpleImmutableClass(string firstName, string lastName, List<string> items)
        {
            FirstName = firstName;
            LastName = lastName;
            Items = items;
        }

        //Private setters are implicit
        public string LastName { get; }
        public string FirstName { get; }
        public List<string> Items { get; }
    }


    [ExcludeFromCodeCoverage]
    public struct PersonX
    {
        public string FirstName;
        public string LastName;
        public List<string> Items;
    }


    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class Tests
    {
        [Test]
        public void TestCreatePerson()
        {
            var immutablePerson =
                ImmutableClass
                    .Create<Person>
                    (
                        "{\"FirstName\":\"John\"," +
                        "\"LastName\":\"Petersen\"}"
                    );
            //Or

            immutablePerson =
                ImmutableClass.Create
                (
                    new Person()
                    {
                        FirstName = "John",
                        LastName = "Petersen"
                    }
                );
        }

        [Test]
        public void CanCreatePersonStruct()
        {
            var sut = new PersonX() {FirstName = "John", LastName = "Petersen"};
            sut.Items = new List<string>();
            sut.FirstName = "john";
            sut.Items.Add("Structs are not immutable!");
        }

        [Test]
        public void CanCreateSimpleImmutableClass()
        {
            var sut = new SimpleImmutableClass("John", "Petesen", new List<string>());
            sut.Items.Add("BS");
            sut.Items.Add("MBA");
            sut.Items.Add("JD");
        }


        [Test]
        public void CanGetToken()

        {
            var token = ImmutableClass.Create<ImmutableTest>(
                    JsonConvert.SerializeObject(
                        new {FirstName = "John", LastName = "Petersen"}))
                .ToString().Substring(2, 32);

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
        public void VerifyStrictCreate()

        {
            var expected = "An immutable object can only be created via the static Create<T> method.";

            var exception = Assert.Throws<ImmutableObjectInvalidCreationException>(() =>
            {
                var sut = new Person(true);
            });

            Assert.AreEqual(expected, exception.Message);
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
                "Properties of an instance of ImmutableClass may only contain the following types: Boolean, Byte, SByte, Char, Decimal, Double, Single, Int32, UInt32, Int64, UInt64, Int16, UInt16, String, ImmutableArray, ImmutableDictionary, ImmutableList, ImmutableQueue, ImmutableSortedSet, ImmutableStack or ImmutableClass. Invalid property types:    List";


            var exception = Assert.Throws<InvalidDataTypeException>(
                () => { ImmutableClass.Create(new InvalidImmutableTestDefintion()); }
            );

            Assert.AreEqual(expected, exception.Message);
            Console.WriteLine();
        }

        public class InvalidImmutableTestDefintion : ImmutableClass
        {
            public string FirstName { get; set; }
            public List<string> InvalidProperty { get; set; }
        }


        [Test]
        public void TestCreateCustomer()
        {

            var sut = new ImmutableCustomer() {FirstName = "John"};

            Assert.IsTrue(sut.FirstName == "John");

            sut.FirstName = ImmutableString.Create("JP");

            Assert.AreEqual("John", sut.FirstName.Value);



        }

        [Test]
        public void TestCreateImmutablePerson()
        {
            var expected =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Schools\":[{\"Institution\":\"Mansfield\",\"Year\":\"1988\",\"Degree\":\"BS\"},{\"Institution\":\"St. Joseph's\",\"Year\":\"1993\",\"Degree\":\"MBA\"},{\"Institution\":\"Rutgers\",\"Year\":\"2004\",\"Degree\":\"JD\"}]}";


            var schools = new List<ImmutableSchool>()
            {
                new ImmutableSchool() { Degree = "BS", Institution = "Mansfield", Year = "1988" },
                new ImmutableSchool() { Degree = "MBA", Institution = "St. Joseph's", Year = "1993" },
                new ImmutableSchool() { Degree = "JD", Institution = "Rutgers", Year = "2004" }
            };

            var person = new ImmutablePerson() {FirstName = "John", LastName = "Petersen",Schools = schools.ToImmutableArray()};

            var actual = JsonConvert.SerializeObject(person);


            Assert.AreEqual(expected,actual);

            person.Schools = new ImmutableArray<ImmutableSchool>();

            Assert.AreEqual(3, person.Schools.Value.Length);

            person.Schools.Value[0].Degree = "XX";

            Assert.AreEqual("BS", person.Schools.Value[0].Degree);


            var x = new ImmutableArray<ImmutableSchool>();



        }




        [Test]
        public void TestCreatePerson2()
        {
            var json =
                "{\"FirstName\":\"John\",\"LastName\":\"Petersen\",\"Address\":{},\"Schools\":[\"Mansfield\",\"St. Joseph's\",\"Rutgers\"]}";

            var person = ImmutableClass.Create<Person>(json);

            Assert.AreEqual("John", person.FirstName);

        }

        public class ImmutableCustomer
        {


            private ImmutableString _firstName;

            public ImmutableString FirstName
            {
                get => _firstName;
                set => _firstName = _firstName ?? value;
            }


            private ImmutableString _middleInitial;

            public ImmutableString MiddleInitial
            {
                get => _middleInitial;
                set => _middleInitial = _middleInitial ?? value;
            }   


            private ImmutableString _lastName;

            public ImmutableString LastName
            {
                get => _lastName;
                set => _lastName = _lastName ?? value;
            }

            private ImmutableString _street;

            public ImmutableString Street
            {
                get => _street;
                set => _street = _street ?? value;
            }

            private ImmutableString _city;

            public ImmutableString City
            {
                get => _city;
                set => _city = _city ?? value;
            }

            private ImmutableString _state;

            public ImmutableString State
            {
                get => _state;
                set => _state = _state ?? value;
            }

            private ImmutableString _zip;

            public ImmutableString Zip
            {
                get => _zip;
                set => _zip = _zip ?? value;
            }

            private ImmutableBoolean _active;

            public ImmutableBoolean Active
            {
                get => _active;
                set => _active = _active ?? value;
            }

            private ImmutableDateTime _customerSince;

            public ImmutableDateTime CustomerSince
            {
                get => _customerSince;
                set => _customerSince = _customerSince ?? value;
            }

            private ImmutableInt32 _pointBalance;

            public ImmutableInt32 PointBalance
            {
                get => _pointBalance;
                set => _pointBalance = _pointBalance ?? value;
            }

            private ImmutableString _tin;

            public ImmutableString TIN
            {
                get => _tin;
                set => _tin = _tin ?? value;
            }



        }

        public class ImmutablePerson
        {
            private String _firstName;

            public String FirstName
            {
                get => _firstName;
                set => _firstName = _firstName ?? value;
            }

            private String _lastName;

            public String LastName
            {
                get => _lastName;
                set => _lastName = _lastName ?? value;
            }

            private ImmutableArray<ImmutableSchool>? _schools;

            public ImmutableArray<ImmutableSchool>? Schools
            {
                get => _schools;
                set => _schools = _schools ?? value;
            }
        }

        public class ImmutableSchool
        {
            private String _institution;

            public String Institution
            {
                get => _institution;
                set => _institution = _institution ?? value;
            }

            private String _year;

            public String Year
            {
                get => _year;
                set => _year = _year ?? value;
            }

            private String _degree;

            public String Degree
            {
                get => _degree;
                set => _degree = _degree ?? value;
            }

        }



        public class Person : ImmutableClass
        {
            public Person() : base(false)
            {
            }

            public Person(bool strictCreate) : base(strictCreate)
            {
            }


            private string _firstName;

            public string FirstName
            {
                get => _firstName;
                set => Setter(
                    MethodBase
                        .GetCurrentMethod()
                        .Name
                        .Substring(4),
                    value,
                    ref _firstName);
            }

            private string _lastName;

            public string LastName
            {
                get => _lastName;
                set => Setter(
                    MethodBase
                        .GetCurrentMethod()
                        .Name
                        .Substring(4),
                    value,
                    ref _lastName);
            }

            private ImmutableArray<string> _schools;

            public ImmutableArray<string> Schools
            {
                get => _schools;
                set => Setter(
                    MethodBase
                        .GetCurrentMethod()
                        .Name.Substring(4),
                    value,
                    ref _schools);
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