using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Community.JsonRpc.ServiceClient.UnitTests
{
    [TestClass]
    public sealed class JsonRpcMethodAttributeTests
    {
        [TestMethod]
        public void ConstructorWhenMethodNameIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute(null));
        }

        [TestMethod]
        public void ConstructorWithParametersByPositionWhenMethodNameIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute(null, new[] { 0 }));
        }

        [TestMethod]
        public void ConstructorWithParametersByNameWhenMethodNameIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute(null, new[] { "a" }));
        }

        [TestMethod]
        public void ConstructorWithErrorDataTypeWhenMethodNameIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute(null, typeof(long)));
        }

        [TestMethod]
        public void ConstructorWithErrorDataTypeAndParametersByPositionWhenMethodNameIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute(null, typeof(long), new[] { 0 }));
        }

        [TestMethod]
        public void ConstructorWithErrorDataTypeAndParametersByNameWhenMethodNameIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute(null, typeof(long), new[] { "a" }));
        }

        [TestMethod]
        public void ConstructorWithParametersByPositionWhenParametersIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute("m", (int[])null));
        }

        [TestMethod]
        public void ConstructorWithParametersByNameWhenParametersIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute("m", (string[])null));
        }

        [TestMethod]
        public void ConstructorWithErrorDataTypeAndParametersByPositionWhenParametersIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute("m", typeof(long), (int[])null));
        }

        [TestMethod]
        public void ConstructorWithErrorDataTypeAndParametersByNameWhenParametersIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute("m", typeof(long), (string[])null));
        }

        [TestMethod]
        public void ConstructorWithErrorDataTypeWhenErrorDataTypeIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute("m", (Type)null));
        }

        [TestMethod]
        public void ConstructorWithErrorDataTypeAndParametersByPositionWhenErrorDataTypeIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute("m", (Type)null, new[] { 0 }));
        }

        [TestMethod]
        public void ConstructorWithErrorDataTypeAndParametersByNameWhenErrorDataTypeIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcMethodAttribute("m", (Type)null, new[] { "a" }));
        }
    }
}