using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("inspectionstandardcredentials", Schema = "public")]
    public class InspectionStandardCredentials
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? itemstandardid { get; set; }
        public int? credentialsid { get; set; }
        public string remark { get; set; }
    }
}