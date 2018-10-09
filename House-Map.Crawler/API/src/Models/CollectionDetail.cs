using System;
using HouseMap.Dao.DBEntity;
using Newtonsoft.Json;

namespace HouseMapAPI.Models
{

    public class CollectionDetail : DBUserCollection
    {
        public DBHouse House { get; set; }

        public string DisplaySource
        {
            get
            {
                return SourceTool.GetDescriptionDic()[this.Source];
            }
        }
    }
}