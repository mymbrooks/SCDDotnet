using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("instrument", Schema = "public")]
    public class Instrument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public string instrumentnumber { get; set; }
        public string factorynumber { get; set; }
        public string specification { get; set; }
        public string measuringrange { get; set; }
        public string uncertainty { get; set; }
        public string accuracyclass { get; set; }
        public decimal? price { get; set; }
        public int? tracetypeid { get; set; }
        public int? inspectioncycle { get; set; }
        public int? supplierid { get; set; }
        public int? manufacturerid { get; set; }
        public int? departmentid { get; set; }
        public string searchkeywords { get; set; }
        public int? stateid { get; set; }
        public string remark { get; set; }
    }
}