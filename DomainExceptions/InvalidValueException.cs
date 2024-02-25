using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinha.Backend.API.DomainExceptions
{
    public class InvalidValueException : Exception
    {
        public InvalidValueException()
        {
        }

        public InvalidValueException(string message) : base(message)
        {
        }
        public InvalidValueException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}