using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("consumablesstock", Schema = "public")]
    public class ConsumablesStock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? warehouseid { get; set; }
        public int? consumablesid { get; set; }
        public string batchnumber { get; set; }
        public DateTime? expiredate { get; set; }
        public decimal? stockamount { get; set; }
        public decimal? totalamount { get; set; }
        public decimal? warningamount { get; set; }
        public string remark { get; set; }
    }
}