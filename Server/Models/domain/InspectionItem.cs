using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("inspectionitem", Schema = "public")]
    public class InspectionItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string searchkeywords { get; set; }
        public string remark { get; set; }
    }
}