namespace Server.Models
{
    public class ConsumablesStockQueryModel
    {
        public int? warehouseid { get; set; }
        public int? categoryid { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string batchnumber { get; set; }
        public int? supplierid { get; set; }
        public int? manufacturerid { get; set; }
    }
}