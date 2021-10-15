using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StocksAnalysis.BusinessLayer;
using StocksAnalysis.Interface;
using UserLibrary;

namespace StocksAnalysis.StockEngine.Chart
{
    public class InvertedHeadAndShoulder : IStockChartPatternAnalysis
    {
        public bool ShareAnalysis(DataTable adtTable, out string astrResults)
        {
            astrResults = string.Empty;
            int firstCandleLimitHead = 4, firstCandleLimitCurrentHead = 0, SecondCandleLimitHead = 3, SecondCandleLimitCurrentHead = 0;
            int firstCandleLimitShoulder = 5, firstCandleLimitCurrentShoulder = 0, SecondCandleLimitShoulder = 4, SecondCandleLimitCurrentShoulder = 0;
            decimal firstpoint = 0, Secondpoint = 0, thirdpoint = 0, fourthpoint = 0, fifthpoint = 0;
            bool IsFirstHeadAvailable = false, IsShoulderAvailable = false, IsSecondHeadAvailable = false;
            bool IsInvertedHeadAndShoulder = false;
            int number = 0;
            if (int.TryParse(GlobalFunction.GetSystemSettingFromCache("SPEV"), out number))
            {
                adtTable = adtTable.ReverseDataTable(number);
                List<decimal> ldecLowValue = new List<decimal>();
                foreach (DataRow dr in adtTable.Rows)
                {
                    ldecLowValue.Add(GeneralFunction.GetDecimalValueFromDataRow(dr, "Low"));
                }

                for (int i = 0; i < ldecLowValue.Count - 1; i++)
                {
                    decimal firstnum = ldecLowValue[i];
                    decimal secondnum = ldecLowValue[i + 1];
                    if (!IsFirstHeadAvailable)
                    {
                        if (firstnum > secondnum)
                        {
                            firstCandleLimitCurrentHead++;
                            SecondCandleLimitCurrentHead = 0;
                            if (firstCandleLimitCurrentHead >= firstCandleLimitHead)
                            {
                                firstpoint = secondnum;
                            }
                        }
                        else if (secondnum > firstnum)
                        {
                            SecondCandleLimitCurrentHead++;
                            firstCandleLimitCurrentHead = 0;
                            if (SecondCandleLimitCurrentHead >= SecondCandleLimitHead)
                            {
                                Secondpoint = secondnum;
                                if (secondnum > ldecLowValue[i + 2])
                                    IsFirstHeadAvailable = true;
                            }
                        }
                    }
                    else if (!IsShoulderAvailable && IsFirstHeadAvailable)
                    {
                        if (firstnum > secondnum)
                        {
                            firstCandleLimitCurrentShoulder++;
                            SecondCandleLimitCurrentShoulder = 0;
                            if (firstCandleLimitCurrentShoulder >= firstCandleLimitShoulder)
                            {
                                thirdpoint = secondnum;
                            }
                        }
                        else if (secondnum > firstnum)
                        {
                            SecondCandleLimitCurrentShoulder++;
                            firstCandleLimitCurrentShoulder = 0;
                            if (SecondCandleLimitCurrentShoulder >= SecondCandleLimitShoulder)
                            {
                                fourthpoint = secondnum;
                                if (secondnum > ldecLowValue[i + 2])
                                    IsShoulderAvailable = true;
                            }
                        }
                    }
                    if(!IsSecondHeadAvailable && IsShoulderAvailable && IsFirstHeadAvailable)
                    {
                        if (firstnum > secondnum)
                        {
                            firstCandleLimitCurrentHead++;
                            SecondCandleLimitCurrentHead = 0;
                            if (firstCandleLimitCurrentHead >= firstCandleLimitHead)
                            {
                                fifthpoint = secondnum;
                            }
                        }
                        else if (secondnum > firstnum)
                        {
                            SecondCandleLimitCurrentHead++;
                            firstCandleLimitCurrentHead = 0;
                            if (SecondCandleLimitCurrentHead >= SecondCandleLimitHead)
                            {
                                Secondpoint = secondnum;
                                if (secondnum > ldecLowValue[i + 2])
                                    IsFirstHeadAvailable = true;
                            }
                        }
                    }
                }
            }
            return IsInvertedHeadAndShoulder;
        }
    }
}
