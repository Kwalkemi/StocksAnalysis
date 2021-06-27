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
    public class PivotShareAnalysis 
    {
        public decimal GetPivotPoint(DataRow adtRow)
        {
            decimal high = GeneralFunction.GetDecimalValueFromDataRow(adtRow, "HIGH_PRICE");
            decimal close = GeneralFunction.GetDecimalValueFromDataRow(adtRow, "LOW_PRICE");
            decimal low = GeneralFunction.GetDecimalValueFromDataRow(adtRow, "CLOSE_PRICE");

            decimal pivotPoint = Math.Round(Decimal.Divide(high + low + close, 3), 2);
            return pivotPoint;
        }

        public string GetSupportLevel(string astrLevel, decimal PivotPoint, DataRow dataRow)
        {
            switch (astrLevel)
            {
                case "S1":
                    {
                        return Convert.ToString(Math.Round((2 * PivotPoint) - GeneralFunction.GetDecimalValueFromDataRow(dataRow, "HIGH_PRICE"),2));
                    }
                case "S2":
                    {
                        return Convert.ToString(Math.Round((PivotPoint) - (GeneralFunction.GetDecimalValueFromDataRow(dataRow, "HIGH_PRICE") - GeneralFunction.GetDecimalValueFromDataRow(dataRow, "LOW_PRICE"))));
                    }
                case "S3":
                    {
                        return Convert.ToString(Math.Round(GeneralFunction.GetDecimalValueFromDataRow(dataRow, "LOW_PRICE") - 2 * (GeneralFunction.GetDecimalValueFromDataRow(dataRow, "HIGH_PRICE") - PivotPoint)));
                    }
                case "R1":
                    {
                        return Convert.ToString(Math.Round((2 * PivotPoint) - GeneralFunction.GetDecimalValueFromDataRow(dataRow, "LOW_PRICE")));
                    }
                case "R2":
                    {
                        return Convert.ToString(Math.Round((PivotPoint) + (GeneralFunction.GetDecimalValueFromDataRow(dataRow, "HIGH_PRICE") - GeneralFunction.GetDecimalValueFromDataRow(dataRow, "LOW_PRICE"))));
                    }
                case "R3":
                    {
                        return Convert.ToString(Math.Round(GeneralFunction.GetDecimalValueFromDataRow(dataRow, "HIGH_PRICE") + 2 * (PivotPoint - GeneralFunction.GetDecimalValueFromDataRow(dataRow, "LOW_PRICE"))));
                    }
                default:
                    return string.Empty;
            }
        }
    }
}
