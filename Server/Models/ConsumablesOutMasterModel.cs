using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class ConsumablesOutMasterModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string number { get; set; }
        public int? companyid { get; set; }
        public string companyname { get; set; }
        public int? warehouseid { get; set; }
        public string warehousename { get; set; }
        public int? departmentid { get; set; }
        public string departmentname { get; set; }
        public string outdate { get; set; }
        public int? createuserid { get; set; }
        public string createusername { get; set; }
        public string createtime { get; set; }
        public decimal? outamount { get; set; }
        public decimal? totalamount { get; set; }
        public decimal? buyprice { get; set; }
        public decimal? sellprice { get; set; }
        public decimal? money { get; set; }
        public string remark { get; set; }
    }
}