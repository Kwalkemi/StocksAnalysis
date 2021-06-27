using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksAnalysis.Interface
{
    public interface IStockAnalysis
    {
        DataTable StockIndicator(List<decimal> numbers);        

        bool ShareAnalysis(DataTable adtTable, out string astrResults);
    }
}
