using System;
using ImmutableClassLibrary;
using NUnit.Framework;

namespace ImmutableTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void VerifyImmutableString()
        {
            var sut = "value".ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, "NewValue"));

        }

        [Test]
        public void VerifyImmutableBoolean()
        {
            var sut = true.ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, false));

        }

        [Test]
        public void VerifyImmutableInt16()
        {
            var sut = short.MaxValue.ToImmutable(); 

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, 2));
        }

        [Test]
        public void VerifyImmutableInt32()
        {
            var sut = int.MaxValue.ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, 2));
        }

        [Test]
        public void VerifyImmutableInt64()
        {
            var sut = long.MaxValue.ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, 2));
        }



        [Test]
        public void VerifyImmutableUInt16()
        {
            var sut = UInt16.MaxValue.ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, 2));
        }

        [Test]
        public void VerifyImmutableUInt32()
        {
            var sut = UInt32.MaxValue.ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, 2));
        }

        [Test]
        public void VerifyImmutableUInt64()
        {
            var sut = UInt64.MaxValue.ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, 2));
        }

        [Test]
        public void VerifyImmutableByte()
        {
            var sut = new byte().ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, new byte()));
        }

        [Test]
        public void VerifyImmutableSByte()
        {
            var sut = new sbyte().ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, new sbyte()));
        }

        [Test]
        public void VerifyImmutableDecimal()
        {
            var sut = new decimal(1).ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, new decimal(2)));
        }

        [Test]
        public void VerifyImmutableDouble()
        {
            var sut = new double().ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, new double()));
        }

        [Test]
        public void VerifyImmutableSingle()
        {

            Single x = 1;

            var sut = x.ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, (Single)2    ));
        }

        [Test]
        public void VerifyImmutableChar()
        {
            var sut = char.Parse("X").ToImmutable();

            var ex = Assert.Throws<ArgumentException>(() => sut.GetType().GetProperty("Value").SetValue(sut, char.Parse("Y")));
        }

        [Test]
        public void VerifyImmutableValue()
        {
            var sut =1.ToImmutable();
            Assert.AreEqual(1,sut.Value);

        }

        [Test]
        public void VerifyImmutableToString()
        {
            var sut = false.ToImmutable();
            Assert.AreEqual("False", sut.ToString());

        }

        [Test]
        public void VerifyImmutableEquals()
        {

          var sut = 1.ToImmutable();

          Assert.IsTrue(sut.Equals(1));  

        }

        [Test]
        public void VerifyGetHashCode()
        {
            var sut = "Foo".ToImmutable();

            Assert.AreEqual("Foo".GetHashCode(),sut.GetHashCode());


        }


        [Test]
        public void VerifyEdit()
        {

            var sut = "foo".ToImmutable().Value.ToUpper().ToImmutable();


            Assert.AreEqual("FOO", sut.Value);


        }




    }
}
