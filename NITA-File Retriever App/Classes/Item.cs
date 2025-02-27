namespace PO_FileRetrieverUI.Classes
{
    // // LINQ SYNTAX
    //public class Item (string number, string revision, string bomID)
    //{
    //    public string Number { get; set; } = number;
    //    public string Revision { get; set; } = revision;
    //    public string BomID { get; set; } = bomID;

    //    public string ExpectedFolderLoc(string pkg)
    //    {
    //        return $@"I:\{pkg}\{Number[..^4]}0000\";
    //    }

    //    //public string ExpectedFolember
    //    //{
    //    //    get
    //    //    {
    //    //        return $@"I:\{Number[..^4]}0000\";
    //    //    }
    //    //}
    //}

    // EXPANDED OG SYNTAX
    public class Item
    {

        public string Number   // property
        {
            get; set;  // get/set methods
        }


        public string Revision   // property
        {
            get; set;   // get/set methods
        }


        public string ExpectedFolderLoc(string pkg)   // method
        {
            return $@"I:\{pkg}\{Number[..^4]}0000\";
        }

        public Item()   // empty initialization
        {
            Number=string.Empty;
            Revision=string.Empty;
        }

        public Item(string number, string revision)   // constructor
        {
            Number = number;
            Revision = revision;
        }
    }

    public class ItemEx : Item
    {
        public string BomID { get; set; }   // new field specific to ItemEx + get/set methods
        public ItemEx(string number, string revision, string bomID)   // constructor
        {
            Number = number;
            Revision = revision;
            BomID = bomID;
        }

        public ItemEx()   // empty initialization
        {
            Number = string.Empty;
            Revision = string.Empty;
            BomID= string.Empty;
        }  
    }
}