using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("instrumentcertificate", Schema = "public")]
    public class InstrumentCertificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int instrumentid { get; set; }
        public int certificatetypeid { get; set; }
        public string certificatenumber { get; set; }
        public DateTime inspectdate { get; set; }
        public DateTime expiredate { get; set; }
        public string inspectplace { get; set; }
        public string certificateurl { get; set; }
        public string remark { get; set; }
    }
}