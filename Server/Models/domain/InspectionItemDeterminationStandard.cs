using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("inspectionitemdeterminationstandard", Schema = "public")]
    public class InspectionItemDeterminationStandard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? categoryid { get; set; }
        public int? itemid { get; set; }
        public int? index { get; set; }
        public int? minoperatorid { get; set; }
        public decimal? min { get; set; }
        public int? maxoperatorid { get; set; }
        public decimal? max { get; set; }
        public string determination { get; set; }
        public string remark { get; set; }
        public int? standardid { get; set; }
        public int? unitid { get; set; }
        public string method { get; set; }
        public string range { get; set; }
        public bool? ispercentage { get; set; }
    }
}