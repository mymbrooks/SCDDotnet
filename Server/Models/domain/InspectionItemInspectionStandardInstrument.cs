using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("inspectioniteminspectionstandardinstrument", Schema = "public")]
    public class InspectionItemInspectionStandardInstrument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? index { get; set; }
        public int? standardid { get; set; }
        public int? instrumentid { get; set; }
        public string remark { get; set; }
    }
}