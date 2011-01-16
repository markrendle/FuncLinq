using FuncLinq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FuncLinq.Tests
{
    
    
    /// <summary>
    ///This is a test class for FObservableTest and is intended
    ///to contain all FObservableTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FObservableTest
    {
        [TestMethod()]
        public void EmptyTest()
        {
            FObservable.Empty<int>()(Assert.IsNull);
        }

        [TestMethod]
        public void ReturnTest()
        {
            int i = 0;
            FObservable.Return(1)(f =>
                                      {
                                          if (i++ == 0) // First call
                                              Assert.AreEqual(1, f());
                                          else // Second call
                                              Assert.IsNull(f);
                                      });
        }

        [TestMethod]
        public void AnaTest()
        {
            int j = 0;
            FObservable.Ana(0, i => i < 3, i => i + 1)(f =>
            {
                if (j < 3) // "OnNext" calls
                    Assert.AreEqual(j++, f());
                else // "OnCompleted" calls
                    Assert.IsNull(f);
            });
        }

        [TestMethod]
        public void CataTest()
        {
            var observable = FObservable.Ana(0, i => i < 4, i => i + 1);
            var sum = observable.Cata(0, (total, next) => total + next);
            Assert.AreEqual(6, sum);
        }
    }
}
