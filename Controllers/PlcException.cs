using System;

namespace PlcVariableReader
{
    public class PlcException : Exception
    {
        public PlcException(string message, Exception innerException) : base(message, innerException) { }
        public PlcException(string message) : base(message) { } // Dodatkowy konstruktor
    }
}