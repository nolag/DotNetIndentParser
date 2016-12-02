using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndentParser.TestHelper
{
    public static class ExceptionHelper
    {
        public static T AssertArgumentException<T>(Action run, string argName, string message = null)
            where T : ArgumentException
        {
            var e = AssertException<T>(run, message);
            Assert.AreEqual(argName, e.ParamName, "Wrong argument name");
            return e;
        }

        public static T AssertException<T>(Action run, string message = null)
            where T : Exception
        {
            try
            {
                run();
                Assert.Fail($"Expected exception of type {typeof(T)}");
                return null;
            }
            catch (T e)
            {
                if (message != null)
                {
                    Assert.AreEqual(message, e.Message, "Wrong error message on thrown exception");
                }

                return e;
            }
        }
    }
}
