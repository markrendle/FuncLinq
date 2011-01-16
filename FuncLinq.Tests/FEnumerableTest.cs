using System.Linq;
using FuncLinq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FuncLinq.Tests
{
    
    
    /// <summary>
    ///This is a test class for FEnumerableTest and is intended
    ///to contain all FEnumerableTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FEnumerableTest
    {
        [TestMethod()]
        public void EmptyTest()
        {
            var enumerable = FEnumerable.Empty<int>();
            var enumerator = enumerable();
            var next = enumerator();
            Assert.IsNull(next);
        }

        [TestMethod]
        public void ReturnTest()
        {
            var enumerable = FEnumerable.Return(1);
            var enumerator = enumerable();
            var next = enumerator();
            Assert.AreEqual(1, next());
            next = enumerator();
            Assert.IsNull(next);
        }

        [TestMethod]
        public void AnaTest()
        {
            var enumerable = FEnumerable.Ana(0, i => i < 3, i => i+1);
            var moveNext = enumerable();
            Assert.AreEqual(0, moveNext()());
            Assert.AreEqual(1, moveNext()());
            Assert.AreEqual(2, moveNext()());
            Assert.IsNull(moveNext());
        }

        [TestMethod]
        public void AsEnumerableTest()
        {
            var array = FEnumerable.Ana(0, i => i < 3, i => i + 1).AsEnumerable().ToArray();
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(2, array[2]);
        }

        [TestMethod]
        public void AsFEnumerableTest()
        {
            var enumerable = new[] {0,1,2}.AsFEnumerable();
            var moveNext = enumerable();
            Assert.AreEqual(0, moveNext()());
            Assert.AreEqual(1, moveNext()());
            Assert.AreEqual(2, moveNext()());
            Assert.IsNull(moveNext());
        }

        [TestMethod]
        public void CataTest()
        {
            var enumerable = FEnumerable.Ana(0, i => i < 4, i => i + 1);
            var sum = enumerable.Cata(0, (total, next) => total + next);
            Assert.AreEqual(6, sum);
        }
    }
}
