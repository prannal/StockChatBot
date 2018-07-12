using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Newtonsoft.Json.Linq;
namespace LuisBot
{
    public class BankData
    {
        public static object GetBankData(string symbol)
        {


            try
            {

                using (var client = new WebClient()) //WebClient  
                {
                    client.Headers.Add("Content-Type:application/json"); //Content-Type  
                    client.Headers.Add("Accept:application/json");

                    var json = client.DownloadString("https://api.iextrading.com/1.0/stock/"+symbol+"/previous");
                    var item = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    return item;

                }



            }
            catch
            {

                return null;
            }

        }
    }
}