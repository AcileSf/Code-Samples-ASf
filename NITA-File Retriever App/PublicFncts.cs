using Microsoft.Data.SqlClient;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using PO_FileRetrieverUI.Classes;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Resources;
using System.Windows.Shapes;
using SWAPP = System.Windows.Application;


namespace PO_FileRetrieverUI
{
    public class GenFnct
    {
        public static void SetLanguage(string language)
        {
            SWAPP.Current.Resources.MergedDictionaries.Clear();
            ResourceDictionary dict = [];
            try { dict.Source = new Uri($"Dictionary-{language}.xaml", UriKind.Relative); }
            catch { dict.Source = new Uri($"Dictionary-EN.xaml", UriKind.Relative); }
            SWAPP.Current.Resources.MergedDictionaries.Add(dict);
        }

        /// <summary>
        /// Get from Dictionnary to support translation + help debug
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetFromDictionary(string key)
        {
            try { return (string)SWAPP.Current.FindResource(key); }
            catch (Exception) { return "Key " + key + " not found in the Language Dictionnary."; }
        }
    }

    public static class PrextraDB
    {
        // Connection String (SqlConnection? type object)
        private static readonly SqlConnection? Cnx = new(@"TrustServerCertificate=True;Data Source=NIT-PREXTRA2K22;Initial Catalog=nita;User ID=bomtoprextra;Password=pass!456;");

        /// <summary>
        /// Function that tries to reconnect to the SQL Database if first attempt is unsuccessful or if connection is lost at some point.
        /// </summary>
        /// <param name="maxRetry">int representing the number of reconnection attempts</param>
        public static bool Connect(int maxRetry)
        {
            string? msg1 = GenFnct.GetFromDictionary("UnableToConnect");
            string? msgTitle1 = GenFnct.GetFromDictionary("CnxError");
            while (maxRetry > 0)
            {
                if (SQLCnx()) return true;
                else
                {
                    if (MessageBox.Show(msg1, msgTitle1, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes) maxRetry--;
                    else maxRetry = 0;
                    
                }
            }
            return false;
        }

        /// <summary>
        /// Initiating the SQL connection using the defined Connection String "Cnx"
        /// </summary>
        private static bool SQLCnx()
        {
            try
            {
                Cnx?.Open();
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Closes the SQL connection if it was already open
        /// </summary>
        public static void Close()
        {
            if (Cnx != null && Cnx.State != System.Data.ConnectionState.Open) Cnx?.Close();
        }


        /// <summary>
        /// Verifies the state of the connection to SQL
        /// </summary>
        /// <param name="cnx">SqlConnection? type item to check</param>
        /// <returns>A boolean "true" if the connection was established and "false" if it was not</returns>
        private static bool VerifyConnection(SqlConnection? cnx)
        {
            string? msg2 = GenFnct.GetFromDictionary("LostCnx");
            string? msgTitle2 = GenFnct.GetFromDictionary("CnxError");
            if (cnx != null && cnx.State != System.Data.ConnectionState.Open)
            {
                if (MessageBox.Show(msg2, msgTitle2, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    return Connect(3);
                }
                else return false;
            }
            else return true;
        }


        /// <summary>
        /// Checks if the entered package number is valid (ie. exists in the SQL tables)
        /// </summary>
        /// <param name="pkgInputNo">String for the targeted package number</param>
        /// <param name="sqlCmdStart">String defining the SQL Command to open the connection</param>
        /// <returns>flagReader: A boolean "true" if the number was found in the SQL tables and "false" if it was not</returns>
        public static bool? PkgNbrChecker(string pkgInputNo, string sqlCmdStart)
        {
            if (!VerifyConnection(Cnx)) return null;
            SqlCommand InputNbrCommand = new(sqlCmdStart + pkgInputNo, Cnx);
            SqlDataReader NbrReader = InputNbrCommand.ExecuteReader();

            // Check if package number exists: true if in DB
            bool flagReader = NbrReader.Read();

            // Release SQL resources
            NbrReader.Close();
            InputNbrCommand.Dispose();

            return flagReader;
        }


        private static readonly PackageType chld = new("BOMChildren", "BOMDetail", "BomId", "SELECT tit.itemcode, tir.revision, tit.BOMID FROM nita.dbo.BOMDetail tbom INNER JOIN nita.dbo.items tit ON tit.itemid = tbom.itemid LEFT JOIN nita.dbo.itemrevision tir ON tir.itemrevisionid = tbom.itemrevisionid WHERE tbom.BOMID=");
        /// <summary>
        /// Recursive function that calls itself to retrieve the children of each assembly if they exist and if the "Include Children" option is checked by user
        /// </summary>
        /// <param name="sqlCmd"> SQL query that fetches the 3 columns of the SQL table pertaining to the PO or Quote number entered by the user, or to the BOM ID Number if the recursive loop is entered </param>
        /// <param name="AccessChildren"> a boolean flagging if the "Include Children" option is checked by user or not, to enter the recursive loop or not </param>
        /// <returns> A list of ITEM objects containing the contents of the 3 retrieved columns from the SQL table </returns>
        public static List<ItemEx>? RetrievePackageDetails(string sqlCmd, bool AccessChildren)
        {
            List<ItemEx>? retVal = RetrievePackageDetails_initial(sqlCmd);
            List<ItemEx>? temp = new();
            if (AccessChildren && retVal != null)
            {
                // for each BOM ID number in previous iteration that is not 0
                foreach (ItemEx returnItem in retVal)
                {
                    if (returnItem.BomID != "0" && !returnItem.BomID.IsNullOrEmpty())
                    {
                        temp.AddRange(RetrievePackageDetails(chld.SQLRetrieve + returnItem.BomID, AccessChildren));
                    }
                }
                retVal.AddRange(temp);
            }

            // Sorting function to be called in main program (to use MissingMessages variable directly)

            return retVal;
        }


        /// <summary>
        ///  Retrieves the contents of 3 columns (items reference numbers,revision letter, if any and BOMID, if the assembly has children) from the SQL table pertaining to the PO or Quote number entered by the user
        /// </summary>
        /// <param name="sqlCmd"> SQL query that fetches the 3 columns of the SQL table pertaining to the PO or Quote number entered by the user </param>
        /// <returns> A list of ITEM objects containing the contents of the 3 retrieved columns from the SQL table </returns>
        public static List<ItemEx>? RetrievePackageDetails_initial(string sqlCmd)
        {
            if (!VerifyConnection(Cnx)) return null;
            
            // Initialize variables and build connection string
            List<ItemEx> itemsList = [];

            SqlCommand command = new(sqlCmd, Cnx);
            SqlDataReader dataReader = command.ExecuteReader();

            // Retrieve data from SQL table returned by the query
            while (dataReader.Read() && dataReader != null)
            {
                string readNumber = dataReader.GetValue(0).ToString() ?? string.Empty;
                int dashIndex = readNumber.IndexOf("-T");
                string readRevision = dataReader.GetValue(1).ToString() ?? string.Empty;
                string readBomID = dataReader.GetValue(2).ToString() ?? string.Empty;

                if (dashIndex >= 0) itemsList.Add(new(readNumber[..dashIndex], readRevision, readBomID));
                else itemsList.Add(new(readNumber, readRevision, readBomID));
            }

            // Release SQL resources
            dataReader?.Close();
            command.Dispose();
            
            return itemsList;
        }

    }
}
