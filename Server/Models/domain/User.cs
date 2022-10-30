using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("user", Schema = "public")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? roleid { get; set; }
        public int? departmentid { get; set; }
        public string number { get; set; }
        public string username { get; set; }
        public string realname { get; set; }
        public string password { get; set; }
        public int? sexid { get; set; }
        public DateTime? birthday { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string idcardnumber { get; set; }
        public string remark { get; set; }
        public int? stateid { get; set; }
        public string wechatid { get; set; }
        public string qqid { get; set; }
        public int? typeid { get; set; }
        public int? qualificationid { get; set; }
        public int? titleid { get; set; }
        public string sign { get; set; }
    }
}