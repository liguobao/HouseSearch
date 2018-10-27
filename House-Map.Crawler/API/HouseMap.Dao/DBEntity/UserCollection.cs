using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseMap.Dao.DBEntity
{

    [Serializable]
    [Table("UserCollections")]
    public class UserCollection
    {
        public long ID { get; set; }

        public long UserID { get; set; }

        public long HouseID { get; set; }

        public string Source { get; set; }

        public string HouseCity { get; set; }

        public DateTime DataCreateTime { get; set; }

        public DateTime DataChange_LastTime { get; set; }

    }
}