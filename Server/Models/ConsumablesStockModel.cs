namespace Server.Models
{
    public class ConsumablesStockModel
    {
        public int? id { get; set; }
        public int? warehouseid { get; set; }
        public string warehousename { get; set; }
        public int? consumablesid { get; set; }
        public int? categoryid { get; set; }
        public string categoryname { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string batchnumber { get; set; }
        public string specification { get; set; }
        public string expiredate { get; set; }
        public int? baseunitid { get; set; }
        public string baseunitname { get; set; }
        public decimal? baseamount { get; set; }
        public int? inunitid { get; set; }
        public string inunitname { get; set; }
        public decimal? stockamount { get; set; }
        public decimal? totalamount { get; set; }
        public int? supplierid { get; set; }
        public string suppliername { get; set; }
        public int? manufacturerid { get; set; }
        public string manufacturername { get; set; }
        public decimal? buyprice { get; set; }
        public decimal? sellprice { get; set; }
        public decimal? money { get; set; }
        public string remark { get; set; }
    }
}