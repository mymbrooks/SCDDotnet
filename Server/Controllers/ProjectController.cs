using Aspose.Words;
using Aspose.Words.Properties;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ruibu.Core.Library;
using Ruibu.Core.Library.Model;
using Server.Models;
using Server.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> logger;
        private IConfiguration configuration;
        private QIContext context;
        private IWebHostEnvironment webHostEnvironment;

        public ProjectController(ILogger<ProjectController> logger, IConfiguration configuration, QIContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public string PrintSolution(long projectid)
        {
            ResultModel<string> resultModel = new ResultModel<string>();

            try
            {
                DateTime datetime = DateTime.Now;
                string year = datetime.Year.ToString();
                string templatePath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/合同书.docx");
                string dirPath = Path.Combine(configuration["FileServerAbsolutePath"], "EntrustOrder", year);
                string relativePath, absolutePath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                Document document = new Document(templatePath);
                DocumentBuilder documentBuilder = new DocumentBuilder(document);
                Regex regex = new Regex(@"\$\[(\w*\d*)\]");
                MatchCollection matches;

                // 项目
                ProjectModel projectModel = context.ProjectModels.FromSqlRaw(
                    @"select    p.id,
                                p.number,
                                p.name,
                                p.projecttypeid,
                                pt.value as projecttypename,
                                p.entrusttypeid,
                                et.value as entrusttypename,
                                p.companyid,
                                co.chinesename as companyname,
                                p.entrustcustomerid,
                                ec.chinesename as entrustcustomername,
                                ec.address as entrustcustomeraddress,
                                ec.contact as entrustcustomercontact,
                                ec.telephone as entrustcustomertelephone,
                                ec.email as entrustcustomeremail,
                                p.clientcustomerid,
                                cc.chinesename as clientcustomername,
                                cc.address as clientcustomeraddress,
                                cc.contact as clientcustomercontact,
                                cc.telephone as clientcustomertelephone,
                                cc.email as clientcustomeremail,
                                trim_scale(p.totalamount) as totalamount,
                                trim_scale(p.prepayamount) as prepayamount,
                                trim_scale(p.remainingamount) as remainingamount,
                                p.reportprovidetypeid,
                                rpt.value as reportprovidetypename,
                                p.reportcount,
                                p.reportday,
                                to_char(p.reportprovidedate, 'YYYY-MM-DD') as reportprovidedate,
                                to_char(p.begindate, 'YYYY-MM-DD') as begindate,
                                to_char(p.enddate, 'YYYY-MM-DD') as enddate,
                                p.isinvoice,
                                p.invoicetypeid,
                                it.value as invoicetypename,
                                p.isallowsubcontract,
                                p.isurgent,
                                p.iscriticize,
                                p.positionmapurl,
                                p.createuserid,
                                createuser.realname as createusername,
                                to_char(p.createtime, 'YYYY-MM-DD HH24:MI:SS') as createtime,
                                p.stateid,
                                ps.value as statename,
                                p.remark
                        from project as p
                        left join (select dd.id, dd.value from datadictionary as dd where dd.key = '项目类型') as pt on p.projecttypeid = pt.id
                        left join (select dd.id, dd.value from datadictionary as dd where dd.key = '委托类型') as et on p.entrusttypeid = et.id
                        left join customer as ec on p.entrustcustomerid = ec.id
                        left join customer as cc on p.clientcustomerid = cc.id
                        left join ""user"" as createuser on p.createuserid = createuser.id
                        left join company as co on p.companyid = co.id
                        left join (select dd.id, dd.value from datadictionary as dd where dd.key = '发票类型') as it on p.invoicetypeid = it.id
                        left join (select dd.id, dd.value from datadictionary as dd where dd.key = '报告交付方式') as rpt on p.reportprovidetypeid = rpt.id
                        left join (select dd.id, dd.value from datadictionary as dd where dd.key = '项目状态') as ps on p.stateid = ps.id
                        where p.id = {0} ", new object[] { projectid }).FirstOrDefault();

                if (projectModel == null)
                {
                    resultModel.success = false;
                    resultModel.info = "项目不存在！";
                    return JsonConvert.SerializeObject(resultModel);
                }

                Company company = (from c in context.Companys
                                   where c.id == projectModel.companyid
                                   select c).FirstOrDefault();

                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (Node node in document.GetChildNodes(NodeType.Paragraph, true))
                {
                    matches = regex.Matches(document.Range.Text);

                    if (matches != null && matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Groups.Count == 2)
                            {
                                if (!dic.ContainsKey(match.Groups[1].Value))
                                {
                                    dic.Add(match.Groups[1].Value, match.Groups[0].Value);
                                }
                            }
                        }
                    }
                }

                if (dic.ContainsKey("项目名称"))
                {
                    document.Range.Replace(dic["项目名称"], projectModel.name ?? "");
                }

                if (dic.ContainsKey("受检客户名称"))
                {
                    document.Range.Replace(dic["受检客户名称"], projectModel.clientcustomername ?? "");
                }

                if (dic.ContainsKey("公司名称"))
                {
                    document.Range.Replace(dic["公司名称"], projectModel.companyname ?? "");
                }

                if (dic.ContainsKey("总金额小写"))
                {
                    document.Range.Replace(dic["总金额小写"], projectModel.totalamount == null ? "0" : projectModel.totalamount.ToString());
                }

                if (dic.ContainsKey("总金额大写"))
                {
                    document.Range.Replace(dic["总金额大写"], projectModel.totalamount == null ? "" : SystemUtil.MoneyToChinese((decimal)projectModel.totalamount));
                }

                if (dic.ContainsKey("预付金额小写"))
                {
                    document.Range.Replace(dic["预付金额小写"], projectModel.prepayamount == null ? "0" : projectModel.prepayamount.ToString());
                }

                if (dic.ContainsKey("预付金额大写"))
                {
                    document.Range.Replace(dic["预付金额大写"], projectModel.prepayamount == null ? "" : SystemUtil.MoneyToChinese((decimal)projectModel.prepayamount));
                }

                if (dic.ContainsKey("剩余金额小写"))
                {
                    document.Range.Replace(dic["剩余金额小写"], projectModel.remainingamount == null ? "0" : projectModel.remainingamount.ToString());
                }

                if (dic.ContainsKey("剩余金额大写"))
                {
                    document.Range.Replace(dic["剩余金额大写"], projectModel.remainingamount == null ? "" : SystemUtil.MoneyToChinese((decimal)projectModel.remainingamount));
                }

                if (dic.ContainsKey("服务开始日期"))
                {
                    document.Range.Replace(dic["服务开始日期"], projectModel.begindate ?? "");
                }

                if (dic.ContainsKey("服务结束日期"))
                {
                    document.Range.Replace(dic["服务结束日期"], projectModel.enddate ?? "");
                }

                if (dic.ContainsKey("统一社会信用代码"))
                {
                    document.Range.Replace(dic["统一社会信用代码"], company.creditnumber ?? "");
                }

                if (dic.ContainsKey("开户银行全称"))
                {
                    document.Range.Replace(dic["开户银行全称"], company.bankfullname ?? "");
                }

                if (dic.ContainsKey("开户银行账号"))
                {
                    document.Range.Replace(dic["开户银行账号"], company.bankaccount ?? "");
                }

                if (dic.ContainsKey("发票类型"))
                {
                    document.Range.Replace(dic["发票类型"], projectModel.invoicetypename ?? "");
                }

                if (dic.ContainsKey("报告天数"))
                {
                    document.Range.Replace(dic["报告天数"], projectModel.reportday == null ? "0" : projectModel.reportday.ToString());
                }

                if (dic.ContainsKey("报告份数"))
                {
                    document.Range.Replace(dic["报告份数"], projectModel.reportcount == null ? "0" : projectModel.reportcount.ToString());
                }

                if (dic.ContainsKey("受检客户地址"))
                {
                    document.Range.Replace(dic["受检客户地址"], projectModel.clientcustomeraddress ?? "");
                }

                if (dic.ContainsKey("受检客户电话"))
                {
                    document.Range.Replace(dic["受检客户电话"], projectModel.clientcustomertelephone ?? "");
                }

                if (dic.ContainsKey("公司地址"))
                {
                    document.Range.Replace(dic["公司地址"], company.address ?? "");
                }

                if (dic.ContainsKey("公司电话"))
                {
                    document.Range.Replace(dic["公司电话"], company.telephone ?? "");
                }

                if (dic.ContainsKey("创建人"))
                {
                    document.Range.Replace(dic["创建人"], projectModel.createusername ?? "");
                }

                CustomDocumentProperties props = document.CustomDocumentProperties;
                if (props["Type"] == null)
                {
                    props.Add("Type", "EntrustOrder");
                    props.Add("id", projectid);
                }

                string saveName = "合同书-" + projectModel.number;
                relativePath = "EntrustOrder/" + year + "/" + saveName + ".docx";
                absolutePath = Path.Combine(dirPath, saveName + ".docx");
                document.Save(absolutePath);

                resultModel.success = true;
                resultModel.model = configuration["FileServerPath"] + relativePath;
                return JsonConvert.SerializeObject(resultModel);
            }
            catch (Exception e)
            {
                resultModel.success = false;
                resultModel.info = e.Message;
                return JsonConvert.SerializeObject(resultModel);
            }
        }
    }
}