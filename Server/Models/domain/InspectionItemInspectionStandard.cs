using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("inspectioniteminspectionstandard", Schema = "public")]
    public class InspectionItemInspectionStandard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? itemid { get; set; }
        public int? index { get; set; }
        public int? standardid { get; set; }
        public string method { get; set; }
        public string range { get; set; }
        public int? detectionminlimitoperatorid { get; set; }
        public decimal? detectionminlimit { get; set; }
        public int? detectionmaxlimitoperatorid { get; set; }
        public decimal? detectionmaxlimit { get; set; }
        public int? quantificationminlimitoperatorid { get; set; }
        public decimal? quantificationminlimit { get; set; }
        public int? quantificationmaxlimitoperatorid { get; set; }
        public decimal? quantificationmaxlimit { get; set; }
        public int? unitid { get; set; }
        public bool? ispercentage { get; set; }
        public decimal? pretreatmentfee { get; set; }
        public decimal? inspectionfee { get; set; }
        public decimal? relativedeviationlimit { get; set; }
        public decimal? recoveryrateminlimit { get; set; }
        public decimal? recoveryratemaxlimit { get; set; }
        public string remark { get; set; }
    }
}