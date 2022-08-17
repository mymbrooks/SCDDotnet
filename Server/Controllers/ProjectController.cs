using Aspose.Words;
using Aspose.Words.Properties;
using Aspose.Words.Tables;
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

        private static void HorizontallyMergeCells(Cell c1, Cell c2)
        {
            c1.CellFormat.HorizontalMerge = CellMerge.First;
            c1.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;

            //Move all content from next cell to previous
            foreach (Node child in c2.ChildNodes)
            {
                c1.AppendChild(child);
            }

            c2.CellFormat.HorizontalMerge = CellMerge.Previous;
            c2.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
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

                if (dic.ContainsKey("项目编号"))
                {
                    document.Range.Replace(dic["项目编号"], projectModel.number ?? "");
                }

                if (dic.ContainsKey("项目名称"))
                {
                    document.Range.Replace(dic["项目名称"], projectModel.name ?? "");
                }

                if (dic.ContainsKey("项目类型"))
                {
                    document.Range.Replace(dic["项目类型"], projectModel.projecttypename ?? "");
                }

                if (dic.ContainsKey("委托单位名称"))
                {
                    document.Range.Replace(dic["委托单位名称"], projectModel.entrustcustomername ?? "");
                }

                if (dic.ContainsKey("受检单位名称"))
                {
                    document.Range.Replace(dic["受检单位名称"], projectModel.clientcustomername ?? "");
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

                if (dic.ContainsKey("委托单位地址"))
                {
                    document.Range.Replace(dic["委托单位地址"], projectModel.entrustcustomeraddress ?? "");
                }

                if (dic.ContainsKey("受检单位地址"))
                {
                    document.Range.Replace(dic["受检单位地址"], projectModel.clientcustomeraddress ?? "");
                }

                if (dic.ContainsKey("委托单位电话"))
                {
                    document.Range.Replace(dic["委托单位电话"], projectModel.entrustcustomertelephone ?? "");
                }

                if (dic.ContainsKey("受检单位电话"))
                {
                    document.Range.Replace(dic["受检单位电话"], projectModel.clientcustomertelephone ?? "");
                }

                if (dic.ContainsKey("公司地址"))
                {
                    document.Range.Replace(dic["公司地址"], company.address ?? "");
                }

                if (dic.ContainsKey("公司电话"))
                {
                    document.Range.Replace(dic["公司电话"], company.telephone ?? "");
                }

                if (dic.ContainsKey("委托单位联系人"))
                {
                    document.Range.Replace(dic["委托单位联系人"], projectModel.entrustcustomercontact ?? "");
                }

                if (dic.ContainsKey("创建人"))
                {
                    document.Range.Replace(dic["创建人"], projectModel.createusername ?? "");
                }

                // 检测方案
                List<SolutionModel> listSolutionModel = context.SolutionModels.FromSqlRaw(
                    @"select   ti.id,
                               t.number as tasknumber,
			                   t.name as taskname,
                               ic.name as categoryname,
                               CASE WHEN COALESCE(ti.issubcontract, false)
                                  THEN ti.subcontractitem
                                  ELSE i.name
                               END AS itemname,
                               t.rate,
			                   t.day,
			                   CASE WHEN ti.cycletypeid is null
                                  THEN t.rate || '批/次，1次'
                                  ELSE t.rate || '批/次，1次/' || cycletype.value
                               END AS rates,
                               CASE WHEN ids.min is null and ids.max is not null
                                      THEN cast(trim_scale(ids.max) as VARCHAR)
                                    WHEN ids.min is not null and ids.max is null
                                      THEN cast(trim_scale(ids.min) as VARCHAR)
                                    WHEN ids.min is not null and ids.max is not null
                                      THEN trim_scale(ids.min) || ' - ' || trim_scale(ids.max)
                                    else ''
                               END as determinationstandardlimit,
			                   determinationstandard.number as determinationstandardnumber,
			                   samplingstandard.number as samplingstandardnumber,
                               trim_scale(iss.fee) as samplingstandardfee,
                               trim_scale(iis.fee) as inspectionstandardfee,
			                   trim_scale((iss.fee + iis.fee) * t.rate * t.day) as singletotalfee,
			                   trim_scale(sum((iss.fee + iis.fee) * t.rate * t.day) over (PARTITION BY t.projectid)) as totalfee
                        from taskitem as ti
                        inner join task as t on ti.taskid = t.id
                        inner join inspectionabilityitem as iai on ti.abilityitemid = iai.id
                        inner join inspectionabilitycategory as ic on iai.categoryid = ic.id
                        left join inspectionitem as i on iai.itemid = i.id
                        left join inspectionitemsamplingstandard as iss on ti.samplingstandardid = iss.id
                        left join standard as samplingstandard on iss.standardid = samplingstandard.id
                        left join inspectioniteminspectionstandard as iis on ti.inspectionstandardid = iis.id
                        left join inspectionitemdeterminationstandard as ids on ti.determinationstandardid = ids.id
                        left join standard as determinationstandard on ids.standardid = determinationstandard.id
                        left join (select dd.id, dd.value from datadictionary as dd where dd.key = '周期类型' order by dd.index) as cycletype on ti.cycletypeid = cycletype.id
                        where t.projectid = {0}
                        order by t.index, ti.index", new object[] { projectid }).ToList();

                Table table;
                Row row;
                Cell cell;
                Paragraph paragraph;
                Run run;
                List<Cell> listCell;
                foreach (Node node in document.GetChildNodes(NodeType.Table, true))
                {
                    table = (Table)node;

                    if (table.Title == "检测方案")
                    {
                        foreach (SolutionModel solutionModel in listSolutionModel)
                        {
                            row = new Row(document);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.tasknumber ?? "/");
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.taskname ?? "");
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document);
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            documentBuilder.MoveTo(run);
                            documentBuilder.InsertHtml(solutionModel.itemname ?? "");

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.rates ?? "");
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.determinationstandardlimit ?? "");
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.determinationstandardnumber ?? "");
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.samplingstandardnumber ?? "");
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            table.InsertAfter(row, table.Rows[listSolutionModel.IndexOf(solutionModel)]);
                        }
                    }

                    if (table.Title == "报价单")
                    {
                        foreach (SolutionModel solutionModel in listSolutionModel)
                        {
                            row = new Row(document);
                            listCell = new List<Cell>();

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.categoryname ?? "");
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document);
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            documentBuilder.MoveTo(run);
                            documentBuilder.InsertHtml(solutionModel.itemname ?? "");

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.samplingstandardfee == null ? "" : solutionModel.samplingstandardfee.ToString());
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.inspectionstandardfee == null ? "" : solutionModel.inspectionstandardfee.ToString());
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.rate == null ? "" : solutionModel.rate.ToString());
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.day == null ? "" : solutionModel.day.ToString());
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);
                            run = new Run(document, solutionModel.singletotalfee == null ? "" : solutionModel.singletotalfee.ToString());
                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            cell = new Cell(document);
                            paragraph = new Paragraph(document);

                            if (listSolutionModel.IndexOf(solutionModel) == listSolutionModel.Count - 1)
                            {
                                run = new Run(document, solutionModel.totalfee == null ? "" : solutionModel.totalfee.ToString());
                            }
                            else
                            {
                                run = new Run(document);
                            }

                            paragraph.Runs.Add(run);
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            cell.Paragraphs.Add(paragraph);
                            cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                            row.Cells.Add(cell);

                            table.InsertAfter(row, table.Rows[listSolutionModel.IndexOf(solutionModel) + 10]);
                        }
                    }
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