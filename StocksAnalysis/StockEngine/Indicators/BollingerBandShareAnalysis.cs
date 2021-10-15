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
    class BollingerBandShareAnalysis : IStockAnalysis
    {
        public bool ShareAnalysis(DataTable adtTable, out string astrResults)
        {
            bool lblnIsShareBuy = false;
            int number = 0;
            astrResults = " ";
            if (int.TryParse(GlobalFunction.GetSystemSettingFromCache("SPEV"), out number))
            {
                DataTable ldecClosingValue = new DataTable();


                ldecClosingValue = adtTable.AsEnumerable().Take(200).CopyToDataTable();

                ldecClosingValue = ldecClosingValue.AsEnumerable().Reverse().CopyToDataTable();

                List<decimal> last10decimal = new List<decimal>();
                BollingerBandShareAnalysis bollingerShareAnalysis = new BollingerBandShareAnalysis();
                DataTable dataTable = bollingerShareAnalysis.StockIndicator(ldecClosingValue);

                DataTable ldtbDatatableAnalysis = dataTable.AsEnumerable().Reverse().Take(10).CopyToDataTable();
                ldtbDatatableAnalysis = ldtbDatatableAnalysis.AsEnumerable().Reverse().CopyToDataTable();
                foreach (DataRow dr in ldtbDatatableAnalysis.Rows)
                {
                    if (!lblnIsShareBuy)
                    {
                        if (Convert.ToDecimal(dr["Low"]) < Convert.ToDecimal(dr["SMA"]))
                        {
                            if (Convert.ToDecimal(dr["Low"]) < Convert.ToDecimal(dr["Lower_Deviation"]))
                            {
                                lblnIsShareBuy = true;
                            }
                        }
                    }
                    else
                    {
                        if(Convert.ToDecimal(dr["High"]) > Convert.ToDecimal(dr["Upper_Deviation"]))
                        {
                            lblnIsShareBuy = false;
                        }
                    }
                }
            }
            return lblnIsShareBuy;
        }

        public DataTable StockIndicator(List<decimal> numbers)
        {
            List<decimal> TwentyDayAverageNumbers = new List<decimal>();
            DataTable dataTable = new DataTable();

            TwentyDayAverageNumbers = SimpleMovingAverageIndicator(numbers, 20);

            dataTable.Columns.Add("Close");
            dataTable.Columns.Add("SMA");
            dataTable.Columns.Add("Deviation");
            dataTable.Columns.Add("Upper_Deviation");
            dataTable.Columns.Add("Lower_Deviation");

            for (int i = 1; i <= numbers.Count; i++)
            {
                DataRow dr = dataTable.NewRow();
                dr["Close"] = numbers[i - 1];

                if (i >= 20)
                {
                    dr["SMA"] = TwentyDayAverageNumbers[i - 20];
                    decimal ldecDeviation = Calculation.GetStandardDeviation(numbers.Skip(i - 1).Take(20));
                    dr["Deviation"] = ldecDeviation;
                    dr["Upper_Deviation"] = ((decimal)dr["SMA"] + (ldecDeviation * 2));
                    dr["Lower_Deviation"] = ((decimal)dr["SMA"] - (ldecDeviation * 2));
                    dataTable.Rows.Add(dr);
                }
            }

            return dataTable;
        }

        public DataTable StockIndicator(DataTable table)
        {
            List<decimal> TwentyDayAverageNumbers = new List<decimal>();
            DataTable dataTable = new DataTable();

            List<decimal> ldecClosingValue = new List<decimal>();
            foreach (DataRow dr in table.Rows)
            {
                ldecClosingValue.Add(GeneralFunction.GetDecimalValueFromDataRow(dr, "CLOSE_PRICE"));
            }

            TwentyDayAverageNumbers = SimpleMovingAverageIndicator(ldecClosingValue, 20);

            dataTable.Columns.Add("Close");
            dataTable.Columns.Add("High");
            dataTable.Columns.Add("Low");
            dataTable.Columns.Add("Open");
            dataTable.Columns.Add("SMA");
            dataTable.Columns.Add("Deviation");
            dataTable.Columns.Add("Upper_Deviation");
            dataTable.Columns.Add("Lower_Deviation");

            for (int i = 1; i <= table.Rows.Count; i++)
            {
                DataRow dr = dataTable.NewRow();
                dr["Close"] = table.Rows[i - 1]["CLOSE_PRICE"];
                dr["High"] = table.Rows[i - 1]["HIGH_PRICE"];
                dr["Low"] = table.Rows[i - 1]["LOW_PRICE"];
                dr["Open"] = table.Rows[i - 1]["OPEN_PRICE"];

                if (i > 20)
                {
                    dr["SMA"] = TwentyDayAverageNumbers[i - 21];
                    decimal ldecDeviation = Calculation.GetStandardDeviation(ldecClosingValue.Skip(i - 21).Take(20));
                    dr["Deviation"] = ldecDeviation;
                    dr["Upper_Deviation"] = (Convert.ToDecimal(dr["SMA"]) + (ldecDeviation * 2));
                    dr["Lower_Deviation"] = (Convert.ToDecimal(dr["SMA"]) - (ldecDeviation * 2));
                }
                dataTable.Rows.Add(dr);
            }
            return dataTable;
        }

        private List<decimal> SimpleMovingAverageIndicator(List<decimal> numbers, int NoofDays)
        {
            List<decimal> Averagenumbers = new List<decimal>();
            Calculation calculation = new Calculation();
            if (numbers.Count > NoofDays)
            {
                for (int j = 0; j < numbers.Skip(NoofDays).Count(); j++)
                {
                    decimal FirstExponential = calculation.SimpleAverageCalculation(numbers.Skip(j).Take(NoofDays).ToList());
                    Averagenumbers.Add(FirstExponential);
                }
            }
            return Averagenumbers;
        }
    }
}
