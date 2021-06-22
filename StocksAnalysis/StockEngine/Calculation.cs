using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksAnalysis.StockEngine
{
    public class Calculation
    {
        public decimal SimpleAverageCalculation(List<decimal> numbers)
        {
            if (numbers != null && numbers.Count > 0)
                return Math.Round(Decimal.Divide(numbers.Sum(), numbers.Count), 5);
            else
                return 0.0m;
        }

        public decimal ExponentialAverageCalculation(decimal CurrentClose, decimal PreviousEMA, int NoOfDays)
        {
            decimal Multiplier = Math.Round(Decimal.Divide(2, NoOfDays + 1), 2);
            decimal Result = (CurrentClose - PreviousEMA) * Multiplier + PreviousEMA;
            return Math.Round(Result, 5);
        }

        public static decimal Divide(decimal firstnum, decimal secondnum)
        {
            return Decimal.Divide(firstnum, secondnum);
        }

        public static decimal Percentage(decimal firstnum, decimal secondnum)
        {
            return Math.Round(Decimal.Divide(Math.Abs(secondnum - firstnum) * 100, firstnum), 2);
        }
    }
}
