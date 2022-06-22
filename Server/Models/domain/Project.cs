using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Domain
{
    [Table("project", Schema = "public")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public int? projecttypeid { get; set; }
        public int? entrusttypeid { get; set; }
        public int? companyid { get; set; }
        public int? entrustcustomerid { get; set; }
        public int? clientcustomerid { get; set; }
        public decimal? totalamount { get; set; }
        public decimal? prepayamount { get; set; }
        public decimal? remainingamount { get; set; }
        public int? reportprovidetypeid { get; set; }
        public int? reportcount { get; set; }
        public int? reportday { get; set; }
        public DateTime? reportprovidedate { get; set; }
        public DateTime? begindate { get; set; }
        public DateTime? enddate { get; set; }
        public bool? isinvoice { get; set; }
        public int? invoicetypeid { get; set; }
        public bool? isallowsubcontract { get; set; }
        public bool? isurgent { get; set; }
        public bool? iscriticize { get; set; }
        public string positionmapurl { get; set; }
        public int? createuserid { get; set; }
        public DateTime? createtime { get; set; }
        public int? stateid { get; set; }
        public string remark { get; set; }
    }
}