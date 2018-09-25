using System.ComponentModel;

namespace HouseMap.Common
{

    public class SourceAttribute : DescriptionAttribute
    {
        private string _tableName;

        private string _name;
        public SourceAttribute(string name, string tableName, string description) : base(description)
        {
            _tableName = tableName;
            _name = name;
        }

        public string TableName
        {
            get
            {
                return _tableName;
            }
        }


        public string Name
        {
            get
            {
                return _name;
            }
        }

    }

}