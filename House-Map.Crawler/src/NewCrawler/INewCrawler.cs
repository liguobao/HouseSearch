using System;
using HouseMap.Dao.DBEntity;

namespace HouseMap.Crawler
{
    public interface INewCrawler
    {
        void Run();

        void SyncHouses();

        void AnalyzeHouse(DateTime fromDate, DateTime toDate);


        SourceEnum GetSource();
    }
}