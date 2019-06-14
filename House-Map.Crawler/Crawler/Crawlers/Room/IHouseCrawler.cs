using System;
using HouseMap.Dao.DBEntity;

namespace HouseMap.Crawler
{
    public interface IHouseCrawler
    {
        void Run();
        
        SourceEnum GetSource();

        void AnalyzeData();
    }
}