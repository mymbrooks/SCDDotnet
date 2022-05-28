using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("standard", Schema = "public")]
    public class Standard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public DateTime? publishdate { get; set; }
        public DateTime? effectdate { get; set; }
        public DateTime? expiredate { get; set; }
        public string fileurl { get; set; }
        public int? industrytypeid { get; set; }
        public string searchkeywords { get; set; }
        public int? stateid { get; set; }
        public string remark { get; set; }
    }
}