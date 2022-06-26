using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class ConsumablesOutDetailModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public long masterid { get; set; }
        public int? stockid { get; set; }
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
        public string expiredate { get; set; }
        public int? supplierid { get; set; }
        public string suppliername { get; set; }
        public int? manufacturerid { get; set; }
        public string manufacturername { get; set; }
        public decimal? buyprice { get; set; }
        public decimal? money { get; set; }
        public decimal? stockamount { get; set; }
        public decimal? stocktotalamount { get; set; }
        public decimal? outamount { get; set; }
        public decimal? totalamount { get; set; }
        public string createtime { get; set; }
        public string remark { get; set; }
    }
}