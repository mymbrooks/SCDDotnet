using Aspose.Words;
using Aspose.Words.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ruibu.Core.Library;
using Server.Models;
using Server.Models.Domain;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        private QIContext context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(QIContext context, ILogger<HomeController> logger)
        {
            this.context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string FormatCell(string cellValue)
        {
            return cellValue.Replace("\r", "").Replace("\a", "").Replace("\t", "").TrimStart().TrimEnd();
        }

        public string InspectionItem()
        {
            string docPath = @"D:\安徽顺诚达批准能力表.docx";
            Document document = new Document(docPath);

            Regex regexStandard = new Regex(@"(.*)(GB|HJ|SL)(/T)?(\s)?(-?\d+.\d+-\d+)*(\(.*\))?");
            int rowIndex = 0, cellIndex = 0;
            string cellValue = "";
            GroupCollection groups;
            StringBuilder sb;

            string categorynumber, categoryname, itemnumber, itemname, standardname, standardnumber, remark;
            categorynumber = categoryname = itemnumber = itemname = standardname = standardnumber = remark = "";

            InspectionAbilityCategory category = null;
            InspectionItem item = null;
            InspectionAbilityItem abilityItem = null;
            InspectionItemInspectionStandard itemStandard = null;
            InspectionStandardCredentials credentials = null;
            Standard standard = null;
            foreach (Table table in document.GetChildNodes(NodeType.Table, true))
            {
                foreach (Aspose.Words.Tables.Row row in table.Rows)
                {
                    rowIndex = table.Rows.IndexOf(row);
                    if (rowIndex == 0)
                    {
                        continue;
                    }

                    sb = new StringBuilder();
                    foreach (Aspose.Words.Tables.Cell cell in row.Cells)
                    {
                        cellIndex = row.Cells.IndexOf(cell);
                        cellValue = FormatCell(cell.GetText());

                        if (cellIndex == 0)
                        {
                            categorynumber = cellValue;
                            continue;
                        }

                        if (cellIndex == 1)
                        {
                            categoryname = cellValue;
                            continue;
                        }

                        if (cellIndex == 2)
                        {
                            itemnumber = cellValue;
                            continue;
                        }

                        if (cellIndex == 3)
                        {
                            itemname = cellValue;
                            continue;
                        }

                        if (cellIndex == 4)
                        {
                            if (regexStandard.IsMatch(cellValue))
                            {
                                foreach (Match match in regexStandard.Matches(cellValue))
                                {
                                    groups = match.Groups;
                                    if (groups.Count >= 6)
                                    {
                                        sb.Append(groups[2].Value).Append(groups[3].Value).Append(groups[4].Value).Append(groups[5].Value);

                                        standardname = groups[1].Value.TrimStart().TrimEnd();
                                        standardnumber = sb.ToString();
                                        continue;
                                    }
                                }
                            }
                        }

                        if (cellIndex == 5)
                        {
                            remark = cellValue;
                        }

                        if (!string.IsNullOrEmpty(categoryname))
                        {
                            // 检测能力类别
                            category = (from c in context.InspectionAbilityCategorys
                                        where c.name == categoryname
                                        select c).FirstOrDefault();

                            if (category == null)
                            {
                                category = new InspectionAbilityCategory();
                                category.number = categorynumber;
                                category.name = categoryname;

                                context.InspectionAbilityCategorys.Add(category);
                                context.SaveChanges();

                                category = (from c in context.InspectionAbilityCategorys
                                            where c.name == categoryname
                                            select c).FirstOrDefault();
                            }
                        }

                        if (!string.IsNullOrEmpty(itemname))
                        {
                            // 检测项目
                            item = (from i in context.InspectionItems
                                    where i.name == itemname
                                    select i).FirstOrDefault();

                            if (item == null)
                            {
                                item = new InspectionItem();
                                item.number = itemnumber;
                                item.name = itemname;
                                item.searchkeywords = SystemUtil.GetSearchKeywords(itemname);

                                context.InspectionItems.Add(item);
                                context.SaveChanges();

                                item = (from i in context.InspectionItems
                                        where i.name == itemname
                                        select i).FirstOrDefault();
                            }
                        }

                        if (category != null && item != null)
                        {
                            // 检测能力项目
                            abilityItem = (from ai in context.InspectionAbilityItems
                                           where ai.categoryid == category.id && ai.itemid == item.id
                                           select ai).FirstOrDefault();

                            if (abilityItem == null)
                            {
                                abilityItem = new InspectionAbilityItem();
                                abilityItem.categoryid = category.id;
                                abilityItem.itemid = item.id;

                                context.InspectionAbilityItems.Add(abilityItem);
                                context.SaveChanges();

                                abilityItem = (from ai in context.InspectionAbilityItems
                                               where ai.categoryid == category.id && ai.itemid == item.id
                                               select ai).FirstOrDefault();
                            }
                        }

                        if (!string.IsNullOrEmpty(standardname))
                        {
                            // 标准
                            standard = (from s in context.Standards
                                        where s.name == standardname
                                        select s).FirstOrDefault();

                            if (standard == null)
                            {
                                standard = new Standard();
                                standard.name = standardname;
                                standard.number = standardnumber;
                                standard.searchkeywords = SystemUtil.GetSearchKeywords(standardname);
                                standard.industrytypeid = 4;
                                standard.stateid = 7;

                                context.Standards.Add(standard);
                                context.SaveChanges();

                                standard = (from s in context.Standards
                                            where s.name == standardname
                                            select s).FirstOrDefault();
                            }
                        }

                        if (item != null && standard != null)
                        {
                            // 检测项目检测标准
                            itemStandard = (from items in context.InspectionItemInspectionStandards
                                            where items.itemid == item.id && items.standardid == standard.id
                                            select items).FirstOrDefault();

                            if (itemStandard == null)
                            {
                                itemStandard = new InspectionItemInspectionStandard();
                                itemStandard.itemid = item.id;
                                itemStandard.standardid = standard.id;

                                if (!string.IsNullOrEmpty(remark))
                                {
                                    itemStandard.remark = remark;
                                }

                                context.InspectionItemInspectionStandards.Add(itemStandard);
                                context.SaveChanges();

                                itemStandard = (from items in context.InspectionItemInspectionStandards
                                                where items.itemid == item.id && items.standardid == standard.id
                                                select items).FirstOrDefault();
                            }
                        }

                        if (item != null && standard != null)
                        {
                            //检测标准资质
                            credentials = (from c in context.InspectionStandardCredentialss
                                           where c.itemstandardid == itemStandard.id && c.credentialsid == 2
                                           select c).FirstOrDefault();

                            if (credentials == null)
                            {
                                credentials = new InspectionStandardCredentials();
                                credentials.itemstandardid = itemStandard.id;
                                credentials.credentialsid = 2;

                                context.InspectionStandardCredentialss.Add(credentials);
                                context.SaveChanges();
                            }
                        }
                    }

                    Console.WriteLine(rowIndex + ": " + categorynumber + " --- " + categoryname + " --- " + itemnumber + " --- " + itemname + " --- " + standardname + " --- " + standardnumber + " --- " + remark);
                    Console.WriteLine();
                }
            }

            return "执行完毕";
        }

        public string Instrument()
        {
            string docPath = @"D:\2022年仪器检定、校准计划表 - 2022.docx";
            Document document = new Document(docPath);
            int rowIndex = 0, cellIndex = 0;
            string cellValue = "";

            Instrument instrument;
            InstrumentCertificate instrumentCertificate;
            string instrumentnumber, instrumentname, specification, factorynumber, inspectplace, inspectdate, expiredate, departmentname;
            foreach (Table table in document.GetChildNodes(NodeType.Table, true))
            {
                foreach (Aspose.Words.Tables.Row row in table.Rows)
                {
                    rowIndex = table.Rows.IndexOf(row);
                    if (rowIndex <= 1)
                    {
                        continue;
                    }

                    instrumentnumber = instrumentname = specification = factorynumber = inspectplace = inspectdate = expiredate = departmentname = "";
                    foreach (Aspose.Words.Tables.Cell cell in row.Cells)
                    {
                        cellIndex = row.Cells.IndexOf(cell);
                        cellValue = FormatCell(cell.GetText());

                        if (cellIndex == 0)
                        {
                            instrumentnumber = cellValue;
                            continue;
                        }

                        if (cellIndex == 1)
                        {
                            instrumentname = cellValue;
                            continue;
                        }

                        if (cellIndex == 2)
                        {
                            specification = cellValue;
                            continue;
                        }

                        if (cellIndex == 3)
                        {
                            factorynumber = cellValue;
                            continue;
                        }

                        if (cellIndex == 4)
                        {
                            continue;
                        }

                        if (cellIndex == 5)
                        {
                            inspectplace = cellValue;
                            continue;
                        }

                        if (cellIndex == 6)
                        {
                            inspectdate = cellValue;
                            continue;
                        }

                        if (cellIndex == 7)
                        {
                            expiredate = cellValue;
                            continue;
                        }

                        if (cellIndex == 8)
                        {
                            departmentname = cellValue;
                        }

                        if (!string.IsNullOrEmpty(instrumentname))
                        {
                            instrument = (from i in context.Instruments
                                          where i.name == instrumentname && i.instrumentnumber == instrumentnumber
                                          select i).FirstOrDefault();

                            if (instrument == null)
                            {
                                instrument = new Instrument();
                                instrument.name = instrumentname;
                                instrument.instrumentnumber = instrumentnumber;
                                instrument.factorynumber = factorynumber;
                                instrument.specification = specification;
                                instrument.tracetypeid = 13;
                                instrument.tracecycle = 12;
                                instrument.searchkeywords = SystemUtil.GetSearchKeywords(instrumentname);
                                instrument.stateid = 17;

                                context.Instruments.Add(instrument);
                                context.SaveChanges();

                                instrument = (from i in context.Instruments
                                              where i.name == instrumentname && i.instrumentnumber == instrumentnumber
                                              select i).FirstOrDefault();

                                instrumentCertificate = new InstrumentCertificate();
                                instrumentCertificate.instrumentid = instrument.id;
                                instrumentCertificate.certificatetypeid = 2;

                                if (!string.IsNullOrEmpty(inspectdate))
                                {
                                    instrumentCertificate.inspectdate = DateTime.Parse(inspectdate);
                                }

                                if (!string.IsNullOrEmpty(expiredate))
                                {
                                    instrumentCertificate.expiredate = DateTime.Parse(expiredate);
                                }

                                context.InstrumentCertificates.Add(instrumentCertificate);
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }

            return "执行完毕";
        }
    }
}