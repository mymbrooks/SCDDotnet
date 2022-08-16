using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class ConsumablesOutMasterModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public string number { get; set; }
        public int? warehouseid { get; set; }
        public string warehousename { get; set; }
        public decimal? outamount { get; set; }
        public decimal? totalamount { get; set; }
        public decimal? buyprice { get; set; }
        public decimal? money { get; set; }
        public int? createuserid { get; set; }
        public string createusername { get; set; }
        public int? receiveuserid { get; set; }
        public string receiveusername { get; set; }
        public string createtime { get; set; }
        public string remark { get; set; }
    }
}