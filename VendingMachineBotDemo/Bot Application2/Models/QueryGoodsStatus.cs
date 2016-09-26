using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application2.Models
{
        public class QueryGoodsStatus
        {
            public Goodsdata[] goodsdata { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public int code { get; set; }
        }

        public class Goodsdata
        {
            public int goodsno { get; set; }
            public int goodsstock { get; set; }
            public long svmtime { get; set; }
            public string goodsname { get; set; }
            public int goodsstatus { get; set; }
        }
}