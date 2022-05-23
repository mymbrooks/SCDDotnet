using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("consumablesoutdetail", Schema = "public")]
    public class ConsumablesOutDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public long masterid { get; set; }
        public int? stockid { get; set; }
        public decimal? outamount { get; set; }
        public decimal? totalamount { get; set; }
        public string remark { get; set; }
    }
}