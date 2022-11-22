using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Listener.Models
{
    public class lastValue
    {
        public int id { get; set; }
        public byte[] value { get; set; }

    }
    public class StaticClass
    {




        /*
         her sayfadan erişilmesi gereken bilgiler burada tutulur.
         */
        public static ValueModel Values = new ValueModel();
        public static ValueModel list=new ValueModel();
        public static List<lastValue> lastvalues = new List<lastValue>();
        public static List<historyModel> historyModels = new List<historyModel>();


    }


}
