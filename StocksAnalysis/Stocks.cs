using StocksAnalysis.BusinessLayer;
using StocksAnalysis.Interface;
using StocksAnalysis.StockEngine;
using StocksAnalysis.StockEngine.Indicators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserLibrary;

namespace StocksAnalysis
{
    public partial class Stocks : Form
    {
        public Stocks()
        {
            InitializeComponent();
        }

        private void InsertDataIntoDatabase()
        {
            string[] lstrFiles = Directory.GetFiles(@"C:\Getbhavcopy\data\NSE-EOD");
            bool IsFirstRow = false, IsSuccessFully = false;
            string lstrFileName = string.Empty;
            Dictionary<string, string> ldict = new Dictionary<string, string>();
            string Logfilename = "Shares_Error_log_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt";
            foreach (string Filenamewithpath in lstrFiles)
            {
                IsFirstRow = true;
                IsSuccessFully = false;
                try
                {
                    DateTime ldtBhavCopyDate = DateTime.MinValue;
                    lstrFileName = Path.GetFileName(Filenamewithpath);
                    //Filepath.Replace(@"C:\Getbhavcopy\data\NSE-EOD\", "");
                    string lstrDateString = lstrFileName.Replace("-NSE-EQ.txt", "");
                    if (DateTime.TryParse(lstrDateString, out ldtBhavCopyDate))
                    {
                        string strdate = ldtBhavCopyDate.ToString("MM/dd/yyyy");
                        string[] larrlines = File.ReadAllLines(Filenamewithpath);
                        foreach (string lines in larrlines)
                        {
                            string[] laarCellValue = lines.Split(',');
                            if (laarCellValue.Count() >= 6)
                            {
                                if (!IsFirstRow)
                                {
                                    ldict = new Dictionary<string, string>();
                                    ldict.Add("STOCK_ID", GetStockId(laarCellValue[0]));
                                    ldict.Add("STOCK_DATE", strdate);
                                    if (laarCellValue[3] != "-")
                                        ldict.Add("HIGH_PRICE", laarCellValue[3]);
                                    if (laarCellValue[4] != "-")
                                        ldict.Add("LOW_PRICE", laarCellValue[4]);
                                    if (laarCellValue[2] != "-")
                                        ldict.Add("OPEN_PRICE", laarCellValue[2]);
                                    if (laarCellValue[5] != "-")
                                        ldict.Add("CLOSE_PRICE", laarCellValue[5]);
                                    if (laarCellValue[6] != "-")
                                        ldict.Add("VOLUME", laarCellValue[6]);

                                    //if (!DBFunction.CheckRecordExists("STOCKS", "STOCK_DAILY_DATA", ldict))
                                    DBFunction.InsertIntoTable("STOCKS", "STOCK_DAILY_DATA", ldict);
                                }
                                else
                                {
                                    IsFirstRow = false;
                                }
                            }
                            else
                            {
                                WriteOnLogFile(lstrFileName + " This file doesn't have more than 6 Values", string.Empty, Logfilename);
                            }
                        }
                    }
                    IsSuccessFully = true;
                }
                catch (Exception ex)
                {
                    IsSuccessFully = false;
                    StringBuilder lStringBuilder = new StringBuilder();
                    lStringBuilder.AppendLine("Error : ");
                    foreach (KeyValuePair<string, string> keyvlu in ldict)
                    {
                        lStringBuilder.AppendLine(keyvlu.Key + " : " + keyvlu.Value);
                    }
                    WriteOnLogFile(ex.Message, ex.StackTrace, Logfilename, lStringBuilder.ToString());
                }
                finally
                {
                    string lstrPath = string.Empty;
                    if (IsSuccessFully)
                    {
                        lstrPath = Path.Combine(@"C:\Getbhavcopy\processed\NSE-EOD", lstrFileName);
                        if (System.IO.Directory.Exists(@"C:\Getbhavcopy\error\NSE-EOD"))
                            File.Move(Filenamewithpath, lstrPath);
                    }
                    else
                    {
                        lstrPath = Path.Combine(@"C:\Getbhavcopy\error\NSE-EOD", lstrFileName);
                        if (System.IO.Directory.Exists(@"C:\Getbhavcopy\error\NSE-EOD"))
                            File.Move(Filenamewithpath, lstrPath);
                    }
                }
            }
        }

        private void WriteOnLogFile(string Message, string stackTrace, string filename, string OtherMessage = "")
        {
            StringBuilder lStringBuilder = new StringBuilder();
            lStringBuilder.AppendLine("Error Message: " + Message);
            lStringBuilder.AppendLine("Stack Trace: " + stackTrace);
            if (OtherMessage != string.Empty)
            {
                lStringBuilder.Append(OtherMessage);
            }
            lStringBuilder.AppendLine("---------------------------------------------------");
            string path = @"D:\Project Logs\Shares Log\" + filename;
            if (File.Exists(path))
            {
                File.AppendAllText(path, lStringBuilder.ToString());
            }
            else
            {
                File.Create(path).Dispose();
                File.WriteAllText(path, lStringBuilder.ToString());
            }
        }

        private string GetStockId(string astrTickerName)
        {
            string lstrQuery = "Select STOCK_ID from STOCK_NAME with(nolock) Where STOCK_SYMBOL = '" + astrTickerName + "'";

            string lstrStockId = DBFunction.FetchScalarFromDatabase("STOCKS", lstrQuery);

            if (lstrStockId != String.Empty)
            {
                return lstrStockId;
            }
            else
            {
                Dictionary<string, string> ldict = new Dictionary<string, string>();
                ldict.Add("STOCK_SYMBOL", astrTickerName);
                int Id = 0;
                if (!DBFunction.CheckRecordExists("STOCKS", "STOCK_NAME", ldict))
                    Id = DBFunction.InsertIntoTable("STOCKS", "STOCK_NAME", ldict);

                if (Id > 0)
                    return Id.ToString();
            }
            return "-1";
        }

        private void InsertIntoDB_Click(object sender, EventArgs e)
        {
            InsertDataIntoDatabase();
        }

        private void btnStockAnalysis_Click(object sender, EventArgs e)
        {
            DataTable AnalyseDataTable = new DataTable();
            PivotShareAnalysis pivotShareAnalysis = new PivotShareAnalysis();
            AnalyseDataTable.Columns.Add("Stocks");
            AnalyseDataTable.Columns.Add("Symbol");
            AnalyseDataTable.Columns.Add("CURRENT PRICE");
            AnalyseDataTable.Columns.Add("MACD");
            AnalyseDataTable.Columns.Add("RSI");
            AnalyseDataTable.Columns.Add("PIVOT POINT");

            string lstrStockResult = string.Empty;

            DataTable ldtbDialyData = new DataTable();
            string Query = @"Select * from [dbo].[STOCK_DAILY_DATA] with (nolock) where 
                                Cast(Stock_date as date) >= Cast(DateAdd(Year, -1, Getdate()) as date)";
            ldtbDialyData = DBFunction.FetchDataFromDatabase(Constant.Common.DATABASE_NAME, Query);

            Query = "Select * from STOCK_NAME where STOCK_TYPE = 'Stock'";
            List<decimal> ldecClosingValue = new List<decimal>();
            IStockAnalysis shareAnalysis;
            DataTable ldtbStockTable = DBFunction.FetchDataFromDatabase(Constant.Common.DATABASE_NAME, Query);
            foreach (DataRow dr in ldtbStockTable.Rows)
            {
                ldecClosingValue = new List<decimal>();
                DataRow dt = AnalyseDataTable.NewRow();
                dt["Stocks"] = GeneralFunction.GetStringValueFromDataRow(dr, "STOCK_NAME");
                dt["Symbol"] = GeneralFunction.GetStringValueFromDataRow(dr, "STOCK_SYMBOL");

                string whereClause = "Stock_id = " + GeneralFunction.GetIntegerValueFromDataRow(dr, "STOCK_ID");

                DataTable ldtbStockIndividualData = ldtbDialyData.FilterDatatable(whereClause, "STOCK_DATE desc");
                if (ldtbStockIndividualData.IsNotNull() && ldtbStockIndividualData.Rows.Count >= 200)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        ldecClosingValue.Add(GeneralFunction.GetDecimalValueFromDataRow(ldtbStockIndividualData.Rows[i], "CLOSE_PRICE"));
                    }
                    dt["CURRENT PRICE"] = ldecClosingValue.FirstOrDefault();
                    ldecClosingValue.Reverse();
                    shareAnalysis = new MACDShareAnalysis();
                    bool lblnMACDAnalysis = shareAnalysis.ShareAnalysis(ldecClosingValue, out lstrStockResult);
                    if (lstrStockResult.IsNotNullOrEmpty())
                        dt["MACD"] = lstrStockResult;
                    
                    shareAnalysis = new RSIShareAnalysis();
                    bool lblnRSIAnalysis = shareAnalysis.ShareAnalysis(ldecClosingValue, out lstrStockResult);
                    dt["PIVOT POINT"] = pivotShareAnalysis.GetPivotPoint(ldtbStockIndividualData.Rows[0]);

                    if (lblnMACDAnalysis && Convert.ToString(dt["MACD"]).IsNullOrEmpty())
                    {
                        dt["MACD"] = "Yes";
                    }

                    if (lblnRSIAnalysis)
                    {
                        dt["RSI"] = "Yes";
                    }
                }

                if (Convert.ToString(dt["MACD"]).IsNotNullOrEmpty() || Convert.ToString(dt["RSI"]) == "Yes")
                    AnalyseDataTable.Rows.Add(dt);
            }
            dataGridView1.DataSource = AnalyseDataTable;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                PivotShareAnalysis pivotShareAnalysis = new PivotShareAnalysis();
                string symbol = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
                decimal PivotNumber = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                string CurrentNumber = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                string query = @"Select Top 1 * from [dbo].[STOCK_DAILY_DATA] A with (nolock)
                                inner join[dbo].[STOCK_NAME] B on A.[STOCK_ID] = B.STOCK_ID Where[STOCK_SYMBOL] = '{0}' order by[STOCK_DATE] desc";

                query = string.Format(query, symbol);

                DataTable dataTable = DBFunction.FetchDataFromDatabase(Constant.Common.DATABASE_NAME, query);
                if (dataTable.IsNotNull() && dataTable.Rows.Count > 0)
                {
                    DataRow dr = dataTable.Rows[0];
                    Form form = new Form();
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.Text = "Pivot Levels";
                    form.Height = 400;
                    form.Width = 300;

                    Label CurrentLabel = new Label();
                    CurrentLabel.Text = "Current Price : ";
                    CurrentLabel.Location = new Point(25, 35);

                    Label CurrentLabelValue = new Label();
                    CurrentLabelValue.Text = CurrentNumber;
                    CurrentLabelValue.Location = new Point(170, 35);

                    Label PivotLabel = new Label();
                    PivotLabel.Text = "Pivot Level : ";
                    PivotLabel.Location = new Point(25, 70);

                    Label PivotLabelValue = new Label();
                    PivotLabelValue.Text = PivotNumber.ToString();
                    PivotLabelValue.Location = new Point(170, 70);

                    Label ResistanceLabel3 = new Label();
                    ResistanceLabel3.Text = "Resistance 3 Level";
                    ResistanceLabel3.Location = new Point(25, 105);

                    Label ResistanceLabel3Value = new Label();
                    ResistanceLabel3Value.Text = pivotShareAnalysis.GetSupportLevel("R3", PivotNumber, dr);
                    ResistanceLabel3Value.Location = new Point(170, 105);

                    Label ResistanceLabel2 = new Label();
                    ResistanceLabel2.Text = "Resistance 2 Level";
                    ResistanceLabel2.Location = new Point(25, 140);

                    Label ResistanceLabel2Value = new Label();
                    ResistanceLabel2Value.Text = pivotShareAnalysis.GetSupportLevel("R2", PivotNumber, dr);
                    ResistanceLabel2Value.Location = new Point(170, 140);

                    Label ResistanceLabel1 = new Label();
                    ResistanceLabel1.Text = "Resistance 1 Level";
                    ResistanceLabel1.Location = new Point(25, 175);

                    Label ResistanceLabel1Value = new Label();
                    ResistanceLabel1Value.Text = pivotShareAnalysis.GetSupportLevel("R1", PivotNumber, dr);
                    ResistanceLabel1Value.Location = new Point(170, 175);

                    Label SupportLabel1 = new Label();
                    SupportLabel1.Text = "Support 1 Level";
                    SupportLabel1.Location = new Point(25, 210);

                    Label SupportLabel1Value = new Label();
                    SupportLabel1Value.Text = pivotShareAnalysis.GetSupportLevel("S1", PivotNumber, dr);
                    SupportLabel1Value.Location = new Point(170, 210);

                    Label SupportLabel2 = new Label();
                    SupportLabel2.Text = "Support 2 Level";
                    SupportLabel2.Location = new Point(25, 245);

                    Label SupportLabel2Value = new Label();
                    SupportLabel2Value.Text = pivotShareAnalysis.GetSupportLevel("S2", PivotNumber, dr);
                    SupportLabel2Value.Location = new Point(170, 245);

                    Label SupportLabel3 = new Label();
                    SupportLabel3.Text = "Support 3 Level";
                    SupportLabel3.Location = new Point(25, 280);

                    Label SupportLabel3Value = new Label();
                    SupportLabel3Value.Text = pivotShareAnalysis.GetSupportLevel("S3", PivotNumber, dr);
                    SupportLabel3Value.Location = new Point(170, 280);

                    form.Controls.Add(ResistanceLabel1);
                    form.Controls.Add(ResistanceLabel1Value);
                    form.Controls.Add(ResistanceLabel2);
                    form.Controls.Add(ResistanceLabel2Value);
                    form.Controls.Add(ResistanceLabel3);
                    form.Controls.Add(ResistanceLabel3Value);

                    form.Controls.Add(SupportLabel1);
                    form.Controls.Add(SupportLabel1Value);
                    form.Controls.Add(SupportLabel2);
                    form.Controls.Add(SupportLabel2Value);
                    form.Controls.Add(SupportLabel3);
                    form.Controls.Add(SupportLabel3Value);

                    form.Controls.Add(CurrentLabel);
                    form.Controls.Add(CurrentLabelValue);
                    form.Controls.Add(PivotLabel);
                    form.Controls.Add(PivotLabelValue);

                    form.ShowDialog();
                }
            }
            //else if(e.ColumnIndex == 3)
            //{
            //    List<decimal> ldecClosingValue = new List<decimal>();
            //    string symbol = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
            //    string query = @"Select Top 1 * from [dbo].[STOCK_DAILY_DATA] A with (nolock)
            //                    inner join[dbo].[STOCK_NAME] B on A.[STOCK_ID] = B.STOCK_ID Where[STOCK_SYMBOL] = '{0}' and Cast(Stock_date as date) >= Cast(DateAdd(Year, -1, Getdate()) as date) order by[STOCK_DATE] desc";

            //    query = string.Format(query, symbol);
            //    string lstrStockResult = string.Empty;
            //    DataTable ldtbStockIndividualData = DBFunction.FetchDataFromDatabase(Constant.Common.DATABASE_NAME, query);
            //    if (ldtbStockIndividualData.IsNotNull() && ldtbStockIndividualData.Rows.Count > 0)
            //    {
            //        if (ldtbStockIndividualData.IsNotNull() && ldtbStockIndividualData.Rows.Count >= 200)
            //        {
            //            for (int i = 0; i < 200; i++)
            //            {
            //                ldecClosingValue.Add(GeneralFunction.GetDecimalValueFromDataRow(ldtbStockIndividualData.Rows[i], "CLOSE_PRICE"));
            //            }
            //            dt["CURRENT PRICE"] = ldecClosingValue.FirstOrDefault();
            //            ldecClosingValue.Reverse();
            //            IStockAnalysis shareAnalysis = new MACDShareAnalysis();
            //            bool lblnMACDAnalysis = shareAnalysis.ShareAnalysis(ldecClosingValue, out lstrStockResult);
            //            if (lstrStockResult.IsNotNullOrEmpty())
            //                dt["MACD"] = lstrStockResult;

            //            if (lblnMACDAnalysis && Convert.ToString(dt["MACD"]).IsNullOrEmpty())
            //            {
            //                dt["MACD"] = "Yes";
            //            }

            //            if (lblnRSIAnalysis)
            //            {
            //                dt["RSI"] = "Yes";
            //            }
            //        }
            //    }
            //}
        }
    }
}
