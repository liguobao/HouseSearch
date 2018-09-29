using System;
using HouseMap.Dao.DBEntity;

namespace HouseMap.Crawler
{
    public interface INewCrawler
    {
        void Run();


        SourceEnum GetSource();
    }
}