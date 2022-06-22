using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class ProjectModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public int? projecttypeid { get; set; }
        public string projecttypename { get; set; }
        public int? entrusttypeid { get; set; }
        public string entrusttypename { get; set; }
        public int? companyid { get; set; }
        public string companyname { get; set; }
        public int? entrustcustomerid { get; set; }
        public string entrustcustomername { get; set; }
        public string entrustcustomeraddress { get; set; }
        public string entrustcustomercontact { get; set; }
        public string entrustcustomertelephone { get; set; }
        public string entrustcustomeremail { get; set; }
        public int? clientcustomerid { get; set; }
        public string clientcustomername { get; set; }
        public string clientcustomeraddress { get; set; }
        public string clientcustomercontact { get; set; }
        public string clientcustomertelephone { get; set; }
        public string clientcustomeremail { get; set; }
        public decimal? totalamount { get; set; }
        public decimal? prepayamount { get; set; }
        public decimal? remainingamount { get; set; }
        public int? reportprovidetypeid { get; set; }
        public string reportprovidetypename { get; set; }
        public int? reportcount { get; set; }
        public int? reportday { get; set; }
        public string reportprovidedate { get; set; }
        public string begindate { get; set; }
        public string enddate { get; set; }
        public bool? isinvoice { get; set; }
        public int? invoicetypeid { get; set; }
        public string invoicetypename { get; set; }
        public bool? isallowsubcontract { get; set; }
        public bool? isurgent { get; set; }
        public bool? iscriticize { get; set; }
        public string positionmapurl { get; set; }
        public int? createuserid { get; set; }
        public string createusername { get; set; }
        public string createtime { get; set; }
        public int? stateid { get; set; }
        public string statename { get; set; }
        public string remark { get; set; }
    }
}