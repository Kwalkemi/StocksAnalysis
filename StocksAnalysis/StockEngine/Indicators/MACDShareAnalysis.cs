using StocksAnalysis.BusinessLayer;
using StocksAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserLibrary;

namespace StocksAnalysis.StockEngine.Indicators
{
    public class MACDShareAnalysis : IStockAnalysis
    {
        bool IStockAnalysis.ShareAnalysis(DataTable adtTable, out string astrResults)
        {
            astrResults = string.Empty;
            bool IsBullishCrossover = false;

            int number = 0;
            if (int.TryParse(GlobalFunction.GetSystemSettingFromCache("SPEV"), out number))
            {
                List<decimal> ldecClosingValue = new List<decimal>();
                for (int i = 0; i < number; i++)
                {
                    ldecClosingValue.Add(GeneralFunction.GetDecimalValueFromDataRow(adtTable.Rows[i], "CLOSE_PRICE"));
                }
                ldecClosingValue.Reverse();

                //decimal MACD = 0.0m, Signal = 0.0m, Histogram = 0.0m;
                List<decimal> last10decimal = new List<decimal>();
                IStockAnalysis mACDShareAnalysis = new MACDShareAnalysis();
                DataTable dataTable = mACDShareAnalysis.StockIndicator(ldecClosingValue);
                if (dataTable.IsNotNull() && dataTable.Rows.Count > 0)
                {
                    //foreach (DataRow dr in dataTable.Rows)
                    //{
                    //    MACD = dr["MACD"] != System.DBNull.Value ? Convert.ToDecimal(dr["MACD"]) : 0.0m;
                    //    Signal = dr["Signal"] != System.DBNull.Value ? Convert.ToDecimal(dr["Signal"]) : 0.0m;
                    //    Histogram = dr["Histogram"] != System.DBNull.Value ? Convert.ToDecimal(dr["Histogram"]) : 0.0m;

                    //    if (MACD > 0 && MACD > Signal)
                    //    {
                    //        if (Histogram > 0)
                    //        {
                    //            IsBullishCrossover = true;
                    //        }
                    //        else
                    //            IsBullishCrossover = false;
                    //    }
                    //    else
                    //    {
                    //        IsBullishCrossover = false;
                    //    }
                    //}
                    if (dataTable.Rows.Count >= 20)
                    {
                        List<DataRow> ldtRow = dataTable.Rows.Cast<DataRow>().Skip(dataTable.Rows.Count - 20).ToList();
                        if (CheckBullishCrossOver(ldtRow))
                        {
                            astrResults = "Bullish Crossover";
                            IsBullishCrossover = true;
                        }
                        if (CheckMACDDivergence(ldtRow))
                        {
                            astrResults = astrResults + "MACD Divergence";
                            IsBullishCrossover = true;
                        }
                        if (CheckHistogramDivergence(ldtRow))
                        {
                            astrResults = astrResults + "Histogram Divergence";
                            IsBullishCrossover = true;
                        }
                    }
                }
            }
            return IsBullishCrossover;
        }

        DataTable IStockAnalysis.StockIndicator(List<decimal> numbers)
        {
            Calculation calculation = new Calculation();
            List<decimal> TwelveDayExponentialNumbers = new List<decimal>();
            List<decimal> TwentySixExponentialNumbers = new List<decimal>();
            List<decimal> MACDExponentialNumbers = new List<decimal>();
            decimal Exponential = 0.0m;
            bool IsMACDExponentialFirstNumber = true;
            TwelveDayExponentialNumbers = ExponentialMovingAverageIndicator(numbers, 12);
            TwentySixExponentialNumbers = ExponentialMovingAverageIndicator(numbers, 26);

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Close");
            dataTable.Columns.Add("Exponential_12");
            dataTable.Columns.Add("Exponential_26");
            dataTable.Columns.Add("MACD");
            dataTable.Columns.Add("Signal");
            dataTable.Columns.Add("Histogram");

            if (TwentySixExponentialNumbers.Count > 0)
            {
                for (int i = 1; i <= numbers.Count; i++)
                {
                    DataRow dr = dataTable.NewRow();
                    dr["Close"] = numbers[i - 1];
                    dr["Date"] = numbers[i - 1];
                    if (i >= 12)
                        dr["Exponential_12"] = TwelveDayExponentialNumbers[i - 12];
                    if (i >= 26)
                    {
                        dr["Exponential_26"] = TwentySixExponentialNumbers[i - 26];
                        dr["MACD"] = Convert.ToDecimal(dr["Exponential_12"]) - Convert.ToDecimal(dr["Exponential_26"]);
                        MACDExponentialNumbers.Add(Convert.ToDecimal(dr["MACD"]));
                        if (MACDExponentialNumbers.Count >= 9)
                        {
                            if (IsMACDExponentialFirstNumber)
                            {
                                Exponential = calculation.SimpleAverageCalculation(MACDExponentialNumbers.Take(9).ToList());
                                dr["Signal"] = Convert.ToDecimal(Exponential);
                                IsMACDExponentialFirstNumber = false;
                            }
                            else
                            {
                                dr["Signal"] = calculation.ExponentialAverageCalculation(Convert.ToDecimal(dr["MACD"]), Exponential, 9);
                                Exponential = Convert.ToDecimal(dr["Signal"]);
                            }
                            dr["Histogram"] = Convert.ToDecimal(dr["MACD"]) - Convert.ToDecimal(dr["Signal"]);
                        }
                    }
                    dataTable.Rows.Add(dr);
                }
            }
            return dataTable;
        }

        private List<decimal> ExponentialMovingAverageIndicator(List<decimal> numbers, int NoofDays)
        {
            List<decimal> Exponentialnumbers = new List<decimal>();
            Calculation calculation = new Calculation();
            if (numbers.Count > NoofDays)
            {
                decimal FirstExponential = calculation.SimpleAverageCalculation(numbers.Take(NoofDays).ToList());
                Exponentialnumbers.Add(FirstExponential);
                foreach (decimal deci in numbers.Skip(NoofDays))
                {
                    decimal expo = calculation.ExponentialAverageCalculation(deci, FirstExponential, NoofDays);
                    FirstExponential = expo;
                    Exponentialnumbers.Add(expo);
                }
            }
            return Exponentialnumbers;
        }

        private bool CheckBullishCrossOver(List<DataRow> dataRows)
        {
            List<DataRow> ldtr = new List<DataRow>();
            bool IsBullishCrossOver = false;
            if (dataRows.Count > 10)
            {
                ldtr = dataRows.Skip(dataRows.Count - 5).ToList();
                DataRow firstRow = ldtr.FirstOrDefault();
                decimal FirstMACD = firstRow["MACD"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["MACD"]) : 0.0m;
                decimal FirstSignal = firstRow["Signal"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["Signal"]) : 0.0m;

                DataRow lastRow = ldtr.LastOrDefault();
                decimal LastMACD = lastRow["MACD"] != System.DBNull.Value ? Convert.ToDecimal(lastRow["MACD"]) : 0.0m;
                decimal LastSignal = lastRow["Signal"] != System.DBNull.Value ? Convert.ToDecimal(lastRow["Signal"]) : 0.0m;

                if (FirstSignal >= FirstMACD)
                {
                    if (LastMACD > LastSignal)
                        IsBullishCrossOver = true;
                }
                else
                {
                    if (LastMACD > LastSignal)
                    {
                        foreach (DataRow dtr in ldtr)
                        {
                            decimal MACD = firstRow["MACD"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["MACD"]) : 0.0m;
                            decimal Signal = firstRow["Signal"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["Signal"]) : 0.0m;
                            if(Signal > MACD)
                                IsBullishCrossOver = true;
                        }
                    }
                }
            }
            return IsBullishCrossOver;
        }

        private bool CheckMACDDivergence(List<DataRow> dataRows)
        {
            List<DataRow> ldtr = new List<DataRow>();
            bool IsBullishDivergence = false;
            if (dataRows.Count > 10)
            {
                ldtr = dataRows.Skip(dataRows.Count - 13).ToList();
                DataRow firstRow = ldtr.FirstOrDefault();
                decimal FirstClose = firstRow["Close"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["Close"]) : 0.0m;
                decimal FirstMACD = firstRow["MACD"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["MACD"]) : 0.0m;

                DataRow lastRow = ldtr.LastOrDefault();
                decimal LastClose = lastRow["Close"] != System.DBNull.Value ? Convert.ToDecimal(lastRow["Close"]) : 0.0m;
                decimal LastMACD = lastRow["MACD"] != System.DBNull.Value ? Convert.ToDecimal(lastRow["MACD"]) : 0.0m;

                if (FirstClose >= LastClose && Calculation.Percentage(FirstClose,LastClose) > 2)
                {
                    if (LastMACD > FirstMACD)
                    {
                        IsBullishDivergence = true;
                        //foreach (DataRow dtr in ldtr)
                        //{
                        //    decimal MACD = firstRow["MACD"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["MACD"]) : 0.0m;
                        //    decimal Close = firstRow["Close"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["Close"]) : 0.0m;
                        //    if (Close > FirstClose && (Calculation.Percentage(FirstClose, Close) > 5))
                        //        IsBullishDivergence = false;
                        //}

                    }
                }
            }
            return IsBullishDivergence;
        }

        private bool CheckHistogramDivergence(List<DataRow> dataRows)
        {
            List<DataRow> ldtr = new List<DataRow>();
            bool IsBullishDivergence = false;
            if (dataRows.Count > 10)
            {
                ldtr = dataRows.Skip(dataRows.Count - 13).ToList();
                DataRow firstRow = ldtr.FirstOrDefault(); 
                decimal FirstClose = firstRow["Close"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["Close"]) : 0.0m;
                decimal FirstMACD = firstRow["MACD"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["MACD"]) : 0.0m;
                decimal FirstHistogram = firstRow["Histogram"] != System.DBNull.Value ? Convert.ToDecimal(firstRow["Histogram"]) : 0.0m;

                DataRow lastRow = ldtr.LastOrDefault();
                decimal LastClose = lastRow["Close"] != System.DBNull.Value ? Convert.ToDecimal(lastRow["Close"]) : 0.0m;
                decimal LastMACD = lastRow["MACD"] != System.DBNull.Value ? Convert.ToDecimal(lastRow["MACD"]) : 0.0m;
                decimal LastHistogram = lastRow["Histogram"] != System.DBNull.Value ? Convert.ToDecimal(lastRow["Histogram"]) : 0.0m;

                if (FirstClose >= LastClose && FirstMACD >= LastMACD && Calculation.Percentage(FirstClose, LastClose) > 2)
                {
                    if (LastHistogram > FirstHistogram)
                        IsBullishDivergence = true;
                }
            }
            return IsBullishDivergence;
        }
    }
}