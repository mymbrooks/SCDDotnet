using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("consumablesoutdetail", Schema = "public")]
    public class ConsumablesOutDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public int? masterid { get; set; }
        public int? stockid { get; set; }
        public string number { get; set; }
        public decimal? outamount { get; set; }
        public decimal? totalamount { get; set; }
        public DateTime? createtime { get; set; }
        public string remark { get; set; }
    }
}