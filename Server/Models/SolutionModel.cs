using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class SolutionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public string taskname { get; set; }
        public string categoryname { get; set; }
        public string itemname { get; set; }
        public string rates { get; set; }
        public string determinationstandardlimit { get; set; }
        public string determinationstandardnumber { get; set; }
        public string samplingstandardnumber { get; set; }
    }
}