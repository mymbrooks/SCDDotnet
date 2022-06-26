using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("consumablesindetail", Schema = "public")]
    public class ConsumablesInDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public long masterid { get; set; }
        public int? consumablesid { get; set; }
        public string number { get; set; }
        public string batchnumber { get; set; }
        public decimal? inamount { get; set; }
        public decimal? totalamount { get; set; }
        public DateTime? expiredate { get; set; }
        public string invoicenumber { get; set; }
        public DateTime? createtime { get; set; }
        public string remark { get; set; }
    }
}