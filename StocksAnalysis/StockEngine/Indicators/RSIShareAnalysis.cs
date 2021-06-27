using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StocksAnalysis.BusinessLayer;
using StocksAnalysis.Interface;
using UserLibrary;

namespace StocksAnalysis.StockEngine.Indicators
{
    public class RSIShareAnalysis : IStockAnalysis
    {
        bool IStockAnalysis.ShareAnalysis(DataTable adtTable, out string astrResults)
        {
            astrResults = string.Empty;

            int number = 0;
            if (int.TryParse(GlobalFunction.GetSystemSettingFromCache("SPEV"),out number))
            {
                List<decimal> ldecClosingValue = new List<decimal>();
                for (int i = 0; i < number; i++)
                {
                    ldecClosingValue.Add(GeneralFunction.GetDecimalValueFromDataRow(adtTable.Rows[i], "CLOSE_PRICE"));
                }
                ldecClosingValue.Reverse();

                IStockAnalysis stockAnalysis = new RSIShareAnalysis();
                DataTable dataTable = stockAnalysis.StockIndicator(ldecClosingValue);
                decimal ldecLastRSI = Convert.ToDecimal(dataTable.Rows[dataTable.Rows.Count - 1]["RSI"]);

                if (ldecLastRSI < 20)
                {
                    astrResults = "Extra Large Selling";
                    return true;
                }
                if (ldecLastRSI < 30 && ldecLastRSI >= 20)
                {
                    astrResults = "Large Selling";
                    return true;
                }
                if (ldecLastRSI > 70 && ldecLastRSI <= 80)
                {
                    astrResults = "Large Buying";
                    return true;
                }
                if (ldecLastRSI > 80)
                {
                    astrResults = "Extra Large Buying";
                    return true;
                }
            }
            return false;
        }

        DataTable IStockAnalysis.StockIndicator(List<decimal> numbers)
        {
            Queue<decimal> ldec14deciGain = new Queue<decimal>();
            Queue<decimal> ldec14deciLoss = new Queue<decimal>();
            decimal ldecOldAvgGain = 0.0m, ldecOldAverageLoss = 0.0m;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Close");
            dataTable.Columns.Add("Gain");
            dataTable.Columns.Add("Loss");
            dataTable.Columns.Add("Average_Gain");
            dataTable.Columns.Add("Average_Loss");
            dataTable.Columns.Add("RS");
            dataTable.Columns.Add("RSI");

            if (numbers.IsNotNull() && numbers.Count >= 14)
            {
                for (int i = 0; i < numbers.Count; i++)
                {
                    DataRow dr = dataTable.NewRow();
                    dr["Close"] = numbers[i];
                    if (i == 0)
                    {
                        dr["Gain"] = 0.0m;
                        ldec14deciGain.Enqueue(0.0m);
                        dr["Loss"] = 0.0m;
                        ldec14deciLoss.Enqueue(0.0m);
                    }
                    else
                    {
                        decimal dec = numbers[i] - numbers[i - 1];
                        if (i == 13)
                        {
                            dr["Average_Gain"] = ldecOldAvgGain = Math.Round(ldec14deciGain.Average(), 3);
                            dr["Average_Loss"] = ldecOldAverageLoss = Math.Round(ldec14deciLoss.Average(), 3);
                            if (Convert.ToDecimal(dr["Average_Loss"]) != 0.00m)
                            {
                                dr["RS"] = Math.Round(Decimal.Divide(Convert.ToDecimal(dr["Average_Gain"]), Convert.ToDecimal(dr["Average_Loss"])), 3);
                                decimal Temp = 1 + Convert.ToDecimal(dr["RS"]);
                                dr["RSI"] = Math.Round(100 - (100 / Temp), 2);
                            }
                            ldec14deciGain.Dequeue();
                            ldec14deciLoss.Dequeue();
                        }
                        if (dec > 0)
                        {
                            dr["Gain"] = dec;
                            ldec14deciGain.Enqueue(dec);
                            dr["Loss"] = 0.0m;
                            ldec14deciLoss.Enqueue(0.0m);
                        }
                        else
                        {
                            dr["Gain"] = 0.0m;
                            ldec14deciGain.Enqueue(0.0m);
                            dr["Loss"] = Math.Abs(dec);
                            ldec14deciLoss.Enqueue(Math.Abs(dec));
                        }
                        if (i > 13)
                        {
                            dr["Average_Gain"] = ldecOldAvgGain = Math.Round((((ldecOldAvgGain * 13) + Convert.ToDecimal(dr["Gain"])) / 14), 3);
                            dr["Average_Loss"] = ldecOldAverageLoss = Math.Round((((ldecOldAverageLoss * 13) + Convert.ToDecimal(dr["Loss"])) / 14), 3);
                            if (Convert.ToDecimal(dr["Average_Loss"]) != 0.00m)
                            {
                                dr["RS"] = Math.Round(Decimal.Divide(Convert.ToDecimal(dr["Average_Gain"]), Convert.ToDecimal(dr["Average_Loss"])), 3);
                                decimal Temp = 1 + Convert.ToDecimal(dr["RS"]);
                                dr["RSI"] = Math.Round(100 - (100 / Temp), 2);
                            }
                            ldec14deciGain.Dequeue();
                            ldec14deciLoss.Dequeue();
                        }
                    }
                    dataTable.Rows.Add(dr);
                }
            }
            return dataTable;
        }
    }
}
