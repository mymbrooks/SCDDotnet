using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("inspectionabilityiteminspectionstandard", Schema = "public")]
    public class InspectionAbilityItemInspectionStandard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? index { get; set; }
        public int? abilityitemid { get; set; }
        public int? inspectionstandardid { get; set; }
        public string remark { get; set; }
    }
}