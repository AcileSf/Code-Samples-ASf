namespace PO_FileRetrieverUI.Classes
{
    public class PackageType(string name, string table, string column, string retrieveSQL)
    {
        public string Name { get; set; } = name;
        public string Table { get; set; } = table;
        public string Column { get; set; } = column;
        public string SQLRetrieve { get; set; } = retrieveSQL;
        public string SQLExist => $"SELECT * FROM nita.dbo.{Table} where {Column}=";



        //private string retrieveSQL;
        //public string SQLRetrieve
        //{
        //    get
        //    {
        //        return retrieveSQL;
        //    }
        //    set
        //    {
        //        retrieveSQL = value +$" {Column}=";
        //    }
        //}

        //// "KArim"
        //public PackageType(string name, string table, string column, string retrieveSQL)
        //{
        //    Name = name;
        //    Table = table;
        //    Column = column;
        //    SQLRetrieve = retrieveSQL;
        //}

    }
}
