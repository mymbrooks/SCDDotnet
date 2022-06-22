using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("company", Schema = "public")]
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string chinesename { get; set; }
        public string englishname { get; set; }
        public int? parentid { get; set; }
        public string description { get; set; }
        public int? regionid { get; set; }
        public string address { get; set; }
        public string contact { get; set; }
        public string telephone { get; set; }
        public string website { get; set; }
        public string postcode { get; set; }
        public string email { get; set; }
        public string logo { get; set; }
        public string creditnumber { get; set; }
        public decimal? longitude { get; set; }
        public decimal? latitude { get; set; }
        public int? bankid { get; set; }
        public string bankfullname { get; set; }
        public string bankaccount { get; set; }
        public string remark { get; set; }
    }
}