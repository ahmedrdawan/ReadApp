using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Exceptions
{
    public class FoundException : Exception
    {
        public FoundException(string message) : base(message) { } 
    }
}
