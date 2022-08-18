using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("report", Schema = "public")]
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? projectid { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string fileurl { get; set; }
        public int? compileuserid { get; set; }
        public DateTime? compiletime { get; set; }
        public int? verifyuserid { get; set; }
        public DateTime? verifytime { get; set; }
        public int? signuserid { get; set; }
        public DateTime? signtime { get; set; }
        public int? sealuserid { get; set; }
        public DateTime? sealtime { get; set; }
        public int? provideuserid { get; set; }
        public DateTime? providetime { get; set; }
        public int? stateid { get; set; }
        public string remark { get; set; }
    }
}