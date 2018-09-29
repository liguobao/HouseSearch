using System;
using HouseMap.Dao.DBEntity;

namespace HouseMap.Crawler
{
    public interface ICrawler
    {
        void Run();

        string GetSource();

    }
}