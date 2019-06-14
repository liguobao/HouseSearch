
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HouseMap.Crawler
{

    public static class SouFunSec
    {
        [DllImport("Resources/libSouFunSec.so", EntryPoint = "getSec")]
        public static extern string getSec(string text);

    }
}
