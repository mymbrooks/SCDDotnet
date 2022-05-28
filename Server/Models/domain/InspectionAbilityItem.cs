using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("inspectionabilityitem", Schema = "public")]
    public class InspectionAbilityItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? index { get; set; }
        public int? categoryid { get; set; }
        public int? itemid { get; set; }
        public string remark { get; set; }
    }
}