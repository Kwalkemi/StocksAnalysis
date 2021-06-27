using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace StocksAnalysis.BusinessLayer
{
    public static class GlobalFunction
    {
        public static void SetSystemSettingInCache(DataTable adtbtable)
        {
            Dictionary<string, string> ldictSetting = new Dictionary<string, string>();
            foreach (DataRow dr in adtbtable.Rows)
            {
                if (!ldictSetting.ContainsKey(Convert.ToString(dr["STOCK_SETTING_UNIQUE_ID"])))
                    ldictSetting.Add(Convert.ToString(dr["STOCK_SETTING_UNIQUE_ID"]), Convert.ToString(dr["STOCK_SETTING_VALUE"]));
            }

            ObjectCache cache = MemoryCache.Default;
            string CacheKey = "SETTING";
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
            cache.Add(CacheKey, ldictSetting, cacheItemPolicy);
        }

        public static string GetSystemSettingFromCache(string astrUniqueId)
        {
            string lstrValue = string.Empty;
            ObjectCache cache = MemoryCache.Default;
            string CacheKey = "SETTING";
            Dictionary<string, string> ldictSetting = new Dictionary<string, string>();
            if (cache.Contains(CacheKey))
                ldictSetting = (Dictionary<string,string>)cache.Get(CacheKey);
            if (ldictSetting.ContainsKey(astrUniqueId))
                lstrValue = ldictSetting[astrUniqueId];
            return lstrValue;
        }
    }
}
