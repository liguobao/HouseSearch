using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HouseMapAPI.CommonException
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}