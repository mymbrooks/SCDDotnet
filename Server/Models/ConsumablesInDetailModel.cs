using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class ConsumablesInDetailModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public int? warehouseid { get; set; }
        public int? masterid { get; set; }
        public int? consumablesid { get; set; }
        public int? categoryid { get; set; }
        public string categoryname { get; set; }
        public string consumablesname { get; set; }
        public string consumablesnumber { get; set; }
        public string detailnumber { get; set; }
        public string batchnumber { get; set; }
        public string specification { get; set; }
        public int? baseunitid { get; set; }
        public string baseunitname { get; set; }
        public int? inunitid { get; set; }
        public string inunitname { get; set; }
        public decimal? baseamount { get; set; }
        public decimal? inamount { get; set; }
        public decimal? totalamount { get; set; }
        public string expiredate { get; set; }
        public int? supplierid { get; set; }
        public string suppliername { get; set; }
        public int? manufacturerid { get; set; }
        public string manufacturername { get; set; }
        public decimal? buyprice { get; set; }
        public decimal? money { get; set; }
        public string invoicenumber { get; set; }
        public string createtime { get; set; }
        public string remark { get; set; }
    }
}