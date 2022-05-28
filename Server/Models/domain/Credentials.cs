using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("credentials", Schema = "public")]
    public class Credentials
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string chinesename { get; set; }
        public string englishname { get; set; }
        public string content { get; set; }
        public string remark { get; set; }
        public string number { get; set; }
        public int companyid { get; set; }
        public DateTime senddate { get; set; }
        public DateTime expiredate { get; set; }
        public string approveuser { get; set; }
        public string certificateurl { get; set; }
        public string sendorganization { get; set; }
    }
}