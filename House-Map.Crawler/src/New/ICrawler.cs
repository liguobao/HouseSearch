using System;

namespace HouseMap.Crawler
{
    public interface ICrawler
    {
        void Run();

        string GetSource();
    }
}