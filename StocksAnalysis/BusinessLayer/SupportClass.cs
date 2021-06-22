using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserLibrary;

namespace StocksAnalysis.BusinessLayer
{
    public class SupportClass
    {


        public void InsertIntoStockHeader()
        {
            string lstrUrl = @"D:\Visual Studio Project\StocksAnalysis\Excel\MCAP31032021_0.xls";
            string lstrSheetName = "CompanyName";
            DataTable ldtbTable = FetchExcelData(lstrUrl, ".xls", "Yes", lstrSheetName);
            string Logfilename = "Shares_Error_log_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt";

            string path = @"D:\Project Logs\Shares Log\" + Logfilename;
            foreach (DataRow dr in ldtbTable.Rows)
            {
                string Query = "Select * from STOCK_NAME Where STOCK_SYMBOL = '" + GeneralFunction.GetStringValueFromDataRow(dr, "Symbol") + "'";
                DataTable ldtbRow = DBFunction.FetchDataFromDatabase(Constant.Common.DATABASE_NAME, Query);
                if (ldtbRow.IsNotNull() && ldtbRow.Rows.Count == 1)
                {
                    Query = "UPDATE STOCK_NAME SET STOCK_NAME = '" + WebUtility.HtmlEncode(GeneralFunction.GetStringValueFromDataRow(dr, "Company Name")) + "'" +
                        " , STOCK_TYPE = 'Stock' WHERE STOCK_ID = " + GeneralFunction.GetIntegerValueFromDataRow(ldtbRow.Rows[0], "STOCK_ID");
                    DBFunction.UpdateTable(Constant.Common.DATABASE_NAME, Query);
                }
                else
                {
                    LogFunction.WriteOnTextLogFile(GeneralFunction.GetStringValueFromDataRow(dr, "Company Name"), Logfilename);
                }
            }
        }

        public static DataTable FetchExcelData(string FilePath, string Extension, string isHDR, string astrSheetName = "")
        {
            DataTable ldtbExcel = new DataTable();
            string conStr = string.Empty, SheetName = string.Empty, ErrorStr = string.Empty;
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;

                    break;
                case ".xlsx": //Excel 07
                    conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                    break;

            }
            conStr = String.Format(conStr, FilePath, isHDR);
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();

            cmdExcel.Connection = connExcel;
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (astrSheetName == string.Empty)
            {
                //Get the name of First Sheet
                SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            }
            else
            {
                astrSheetName = astrSheetName + "$";
                DataRow dr = dtExcelSchema.Select("TABLE_NAME = '" + astrSheetName + "'").FirstOrDefault();
                if (dr == null)
                    ErrorStr = "Sheet Not Available";
                SheetName = astrSheetName;
            }
            connExcel.Close();
            if (ErrorStr == String.Empty)
            {
                //Read Data from First Sheet
                connExcel.Open();
                cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
                oda.SelectCommand = cmdExcel;
                oda.Fill(ldtbExcel);
                connExcel.Close();
            }
            return ldtbExcel;
        }

        //public static DataTable FetchDataFromDatabase(string astrDatabase, string astrFetchQuery)
        //{
        //    DataTable ldtbDB = new DataTable();
        //    try
        //    {
        //        string lstrConnectionString = @"Data Source=LAPTOP-86PJE7FL\SQLEXPRESS; Initial Catalog = {0}; Integrated Security = true";
        //        lstrConnectionString = String.Format(lstrConnectionString, astrDatabase);
        //        SqlConnection lSqlConnection = new SqlConnection(lstrConnectionString);
        //        SqlDataAdapter lSqlDataAdapter = new SqlDataAdapter(astrFetchQuery, lSqlConnection);
        //        lSqlDataAdapter.SelectCommand.CommandTimeout = 300;
        //        lSqlDataAdapter.Fill(ldtbDB);
        //        lSqlConnection.Close();
        //    }
        //    catch(Exception ex)
        //    {

        //        }
        //    return ldtbDB;
        //}

        //public static DataTable FilterDatatable(DataTable adtbTable, string astrWhereClause, string astrOrderClause)
        //{
        //    string _sqlWhere = astrWhereClause;
        //    string _sqlOrder = astrOrderClause;

        //    DataTable _newDataTable = new DataTable();

        //    var table = adtbTable.Select(_sqlWhere, _sqlOrder);
        //    if (table.Any())
        //        _newDataTable = table.CopyToDataTable();
        //    return _newDataTable;
        //}


        public void InsertIntoSheet(DataTable ldtbTable)
        {
            string lstrUrl = @"D:\Share Testing\GeneratedData\MACD_Generated_Data.xls";
            string lstrSheetName = "Sheet1";
            InsertExcelData(lstrUrl, ".xls", "Yes", ldtbTable, lstrSheetName);
        }

        public static void InsertExcelData(string FilePath, string Extension, string isHDR, DataTable adtbTable, string astrSheetName = "")
        {
            DataTable ldtbExcel = new DataTable();
            string conStr = string.Empty, SheetName = string.Empty, ErrorStr = string.Empty;
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;

                    break;
                case ".xlsx": //Excel 07
                    conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                    break;

            }
            conStr = String.Format(conStr, FilePath, isHDR);
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            connExcel.Open();

            foreach (DataRow dr in adtbTable.Rows)
            {
                string Query = "Insert Into [" + astrSheetName + "$] ([Date], [Close_Price], [12DayEMA], [26DayEMA], [MACD], [Signal], [Histogram] ) Values('" + dr["Date"] + "', '"
                                    + dr["Close"] + "', '" + GeneralFunction.GetStringValueFromDataRow(dr, "Exponential_12") + "' , '" + GeneralFunction.GetStringValueFromDataRow(dr, "Exponential_26") +
                                    "' , '" + GeneralFunction.GetStringValueFromDataRow(dr, "MACD") + "' , '" + GeneralFunction.GetStringValueFromDataRow(dr, "Signal") + "' , '" +
                                    GeneralFunction.GetStringValueFromDataRow(dr, "Histogram") + "')";

                Query = "Insert Into [" + astrSheetName + "$] ([BDate]) Values ('5126')";

                cmdExcel.Connection = connExcel;
                cmdExcel.CommandText = Query;


                int result = cmdExcel.ExecuteNonQuery();
            }
            connExcel.Close();
        }
    }
}
