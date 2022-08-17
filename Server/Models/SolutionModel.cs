using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class SolutionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public string tasknumber { get; set; }
        public string taskname { get; set; }
        public string categoryname { get; set; }
        public string itemname { get; set; }
        public int? rate { get; set; }
        public int? day { get; set; }
        public string rates { get; set; }
        public string determinationstandardlimit { get; set; }
        public string determinationstandardnumber { get; set; }
        public string samplingstandardnumber { get; set; }
        public decimal? samplingstandardfee { get; set; }
        public decimal? inspectionstandardfee { get; set; }
        public decimal? singletotalfee { get; set; }
        public decimal? totalfee { get; set; }
    }
}