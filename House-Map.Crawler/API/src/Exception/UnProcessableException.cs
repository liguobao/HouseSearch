

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HouseMapAPI.CommonException
{
    public class UnProcessableException : Exception
    {
        public UnProcessableException(string message) : base(message)
        {

        }
    }
}