using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("inspectionabilitycategory", Schema = "public")]
    public class InspectionAbilityCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? parentid { get; set; }
        public int? index { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string samplenumber { get; set; }
        public int? departmentid { get; set; }
        public string remark { get; set; }
    }
}