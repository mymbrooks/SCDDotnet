using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("consumablesoutmaster", Schema = "public")]
    public class ConsumablesOutMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string number { get; set; }
        public int? warehouseid { get; set; }
        public int? createuserid { get; set; }
        public int? receiveuserid { get; set; }
        public DateTime? createtime { get; set; }
        public string remark { get; set; }
    }
}