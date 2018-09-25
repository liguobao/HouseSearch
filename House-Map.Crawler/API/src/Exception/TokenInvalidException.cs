

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HouseMapAPI.CommonException
{
    public class TokenInvalidException : Exception
    {
        public TokenInvalidException(string message) : base(message)
        {

        }
    }
}