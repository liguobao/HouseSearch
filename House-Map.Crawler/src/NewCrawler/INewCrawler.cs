using System;
using HouseMap.Dao.DBEntity;

namespace HouseMap.Crawler
{
    public interface INewCrawler
    {
        void Run();

        void SyncHouses();


        SourceEnum GetSource();
    }
}