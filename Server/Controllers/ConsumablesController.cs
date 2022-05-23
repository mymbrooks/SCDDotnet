using Aspose.Cells;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ruibu.Core.Library.Model;
using Server.Models;
using Server.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class ConsumablesController : Controller
    {
        private readonly ILogger<ConsumablesController> logger;
        private IConfiguration configuration;
        private QIContext context;
        private IWebHostEnvironment webHostEnvironment;

        public ConsumablesController(ILogger<ConsumablesController> logger, IConfiguration configuration, QIContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public string PrintConsumablesInReport(long masterid)
        {
            ResultModel<string> resultModel = new ResultModel<string>();

            try
            {
                DateTime dateTime = DateTime.Now;
                string relativePath, absolutePath;
                string excelPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/报表.xlsx");

                using (Workbook workbook = new Workbook(excelPath))
                {
                    List<Worksheet> listSheet = new List<Worksheet>();
                    foreach (Worksheet sheet in workbook.Worksheets)
                    {
                        if (sheet.Name != "耗材入库")
                        {
                            listSheet.Add(sheet);
                        }
                    }

                    foreach (Worksheet sheet in listSheet)
                    {
                        workbook.Worksheets.RemoveAt(sheet.Name);
                    }

                    ConsumablesInMasterModel master = context.ConsumablesInMasterModels.FromSqlRaw(
                        @"
                            select distinct m.id,
	                               m.number,
                                   w.companyid,
                                   co.chinesename as companyname,
	                               m.warehouseid,
	                               w.name as warehousename,
	                               to_char(m.indate, 'YYYY-MM-DD') as indate,
	                               m.createuserid,
	                               u.realname as createusername,
	                               to_char(m.createtime, 'YYYY-MM-DD HH24:MI:SS') as createtime,
	                               sum(trim_scale(COALESCE(d.inamount, 0))) over (PARTITION by m.id) as inamount,
	                               sum(trim_scale(COALESCE(d.totalamount, 0))) over (PARTITION by m.id) as totalamount,
	                               sum(trim_scale(COALESCE(c.buyprice, 0))) over (PARTITION by m.id) as buyprice,
	                               sum(trim_scale(COALESCE(c.sellprice, 0))) over (PARTITION by m.id) as sellprice,
	                               trim_scale(sum(COALESCE(d.totalamount, 0) * COALESCE(c.buyprice, 0)) over (PARTITION by m.id)) as money,
	                               m.remark
                            from consumablesinmaster as m
                            LEFT join consumablesindetail as d on m.id = d.masterid
                            LEFT join consumables as c on d.consumablesid = c.id
                            left join warehouse as w on m.warehouseid = w.id
                            left join company as co on w.companyid = co.id
                            left join ""user"" as u on m.createuserid = u.id
                            where m.id = {0} ", masterid).FirstOrDefault();

                    Worksheet worksheet = workbook.Worksheets["耗材入库"];

                    IList<ConsumablesInDetailModel> listDetail = context.ConsumablesInDetailModels.FromSqlRaw(
                        @"
                            select d.id,
                                   cs.warehouseid,
                                   d.masterid,
                                   d.consumablesid,
                                   c.categoryid,
                                   cc.name as categoryname,
                                   c.name,
                                   c.number as consumablesnumber,
                                   d.number as innumber,
                                   d.batchnumber,
                                   c.specification,
                                   c.baseunitid,
                                   baseunit.name as baseunitname,
                                   c.inunitid,
                                   inunit.name as inunitname,
                                   trim_scale(COALESCE(c.baseamount, 0)) as baseamount,
                                   trim_scale(COALESCE(d.inamount, 0)) as inamount,
                                   trim_scale(COALESCE(d.totalamount, 0)) as totalamount,
                                   to_char(d.expiredate, 'YYYY-MM-DD') as expiredate,
                                   c.supplierid,
                                   s.chinesename as suppliername,
                                   c.manufacturerid,
                                   ma.chinesename as manufacturername,
                                   trim_scale(COALESCE(c.buyprice, 0)) as buyprice,
                                   trim_scale(COALESCE(c.sellprice, 0)) as sellprice,
                                   trim_scale(COALESCE(d.inamount, 0) * COALESCE(c.buyprice, 0)) as money,
                                   d.invoicenumber,
                                   d.remark
                            from consumablesindetail as d
                            inner join consumables as c on d.consumablesid = c.id
                            inner join consumablesstock as cs on d.consumablesid = cs.consumablesid
                            left join consumablescategory as cc on c.categoryid = cc.id
                            left join unit as baseunit on c.baseunitid = baseunit.id
                            left join unit as inunit on c.inunitid = inunit.id
                            left join supplier as s on c.supplierid = s.id
                            left join manufacturer as ma on c.manufacturerid = ma.id
                            where d.masterid = {0}
                            order by c.number ", masterid).ToList();

                    ConsumablesInDetailModel detail;
                    IList<Name> listName = workbook.Worksheets.Names.Where(n => n.SheetIndex == 1).ToList();

                    string saveDir = Path.Combine(configuration["FileServerAbsolutePath"], "Report", dateTime.Year.ToString(), master.id.ToString());

                    if (Directory.Exists(saveDir))
                    {
                        Directory.Delete(saveDir, true);
                    }

                    Directory.CreateDirectory(saveDir);

                    int pageCount;
                    Aspose.Cells.Range rangeConsumables = workbook.Worksheets.GetRangeByName(worksheet.Name + "!耗材");
                    int rowCount = rangeConsumables.RowCount;
                    int lastRowCount, pageCountIndex;
                    if (listDetail.Count % rowCount == 0)
                    {
                        pageCount = listDetail.Count / rowCount;
                        lastRowCount = rowCount;
                    }
                    else
                    {
                        pageCount = listDetail.Count / rowCount + 1;
                        lastRowCount = listDetail.Count % rowCount;
                    }

                    for (int page = 0; page < pageCount; page++)
                    {
                        foreach (Name name in listName)
                        {
                            if (name.Text == "标题")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.companyname + "入库单";
                                continue;
                            }

                            if (name.Text == "入库单号")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.number;
                                continue;
                            }

                            if (name.Text == "仓库")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.warehousename;
                                continue;
                            }

                            if (name.Text == "入库日期")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.indate;
                                continue;
                            }

                            if (name.Text == "操作人")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.createusername;
                                continue;
                            }

                            if (name.Text == "操作时间")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.createtime;
                                continue;
                            }
                        }

                        // 耗材列表
                        Aspose.Cells.Range rangeConsumablesName = workbook.Worksheets.GetRangeByName(worksheet.Name + "!耗材名称");
                        Aspose.Cells.Range rangeSpecification = workbook.Worksheets.GetRangeByName(worksheet.Name + "!规格型号");
                        Aspose.Cells.Range rangeInNumber = workbook.Worksheets.GetRangeByName(worksheet.Name + "!编号");
                        Aspose.Cells.Range rangeUnit = workbook.Worksheets.GetRangeByName(worksheet.Name + "!单位");
                        Aspose.Cells.Range rangeAmount = workbook.Worksheets.GetRangeByName(worksheet.Name + "!数量");
                        Aspose.Cells.Range rangePrice = workbook.Worksheets.GetRangeByName(worksheet.Name + "!单价");
                        Aspose.Cells.Range rangeMoney = workbook.Worksheets.GetRangeByName(worksheet.Name + "!金额");

                        // 最后一页
                        if (page == pageCount - 1)
                        {
                            pageCountIndex = page * rowCount + lastRowCount;
                        }
                        else
                        {
                            pageCountIndex = (page + 1) * rowCount;
                        }

                        // 清空耗材列表
                        rangeConsumables.Value = "";

                        for (int i = page * rowCount; i < pageCountIndex; i++)
                        {
                            detail = listDetail[i];

                            rangeConsumablesName.GetOffset(i % rowCount + 1, 0).Value = detail.name;
                            rangeSpecification.GetOffset(i % rowCount + 1, 0).Value = detail.specification;
                            rangeInNumber.GetOffset(i % rowCount + 1, 0).Value = detail.innumber;
                            rangeUnit.GetOffset(i % rowCount + 1, 0).Value = detail.inunitname;
                            rangeAmount.GetOffset(i % rowCount + 1, 0).Value = detail.inamount;
                            rangePrice.GetOffset(i % rowCount + 1, 0).Value = detail.buyprice;
                            rangeMoney.GetOffset(i % rowCount + 1, 0).Value = detail.money;
                        }

                        // 单页合计
                        rangeAmount.GetOffset(13, 0).GetCellOrNull(0, 0).Formula = "=SUM(H5:H16)";
                        rangePrice.GetOffset(13, 0).GetCellOrNull(0, 0).Formula = "=SUM(I5:I16)";
                        rangeMoney.GetOffset(13, 0).GetCellOrNull(0, 0).Formula = "=SUM(J5:J16)";

                        // 总合计
                        rangeAmount.GetOffset(14, 0).Value = listDetail.Sum(d => d.inamount);
                        rangePrice.GetOffset(14, 0).Value = listDetail.Sum(d => d.buyprice);
                        rangeMoney.GetOffset(14, 0).Value = listDetail.Sum(d => d.money);

                        workbook.CalculateFormula();

                        worksheet.PageSetup.CustomPaperSize(24.1 * 0.393700787402, 14 * 0.393700787402);

                        workbook.Save(Path.Combine(saveDir, (page + 1) + ".pdf"), SaveFormat.Pdf);
                    }

                    List<string> inputDocs = new List<string>();
                    DirectoryInfo directoryInfo = new DirectoryInfo(saveDir);
                    foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.pdf", SearchOption.AllDirectories))
                    {
                        inputDocs.Add(fileInfo.FullName);
                    }

                    Aspose.Pdf.Document inputDoc;
                    Aspose.Pdf.Document targetDoc = new Aspose.Pdf.Document();

                    if (inputDocs.Count == 0)
                    {
                        resultModel.success = false;
                        resultModel.info = "没有耗材，无法打印！";
                        return JsonConvert.SerializeObject(resultModel);
                    }

                    for (int i = 0; i < inputDocs.Count; i++)
                    {
                        inputDoc = new Aspose.Pdf.Document(inputDocs[i]);
                        targetDoc.Pages.Add(inputDoc.Pages);
                    }

                    relativePath = configuration["FileServerPath"] + "Report/" + dateTime.Year.ToString() + "/" + master.id.ToString() + "/" + master.number + ".pdf";
                    absolutePath = Path.Combine(saveDir, master.number + ".pdf");
                    targetDoc.Save(absolutePath);
                }

                resultModel.success = true;
                resultModel.model = relativePath;
                return JsonConvert.SerializeObject(resultModel);
            }
            catch (Exception e)
            {
                resultModel.success = false;
                resultModel.info = e.Message;
            }

            return JsonConvert.SerializeObject(resultModel);
        }

        public string PrintConsumablesOutReport(long masterid)
        {
            ResultModel<string> resultModel = new ResultModel<string>();

            try
            {
                DateTime dateTime = DateTime.Now;
                string relativePath, absolutePath;
                string excelPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/报表.xlsx");

                using (Workbook workbook = new Workbook(excelPath))
                {
                    List<Worksheet> listSheet = new List<Worksheet>();
                    foreach (Worksheet sheet in workbook.Worksheets)
                    {
                        if (sheet.Name != "耗材出库")
                        {
                            listSheet.Add(sheet);
                        }
                    }

                    foreach (Worksheet sheet in listSheet)
                    {
                        workbook.Worksheets.RemoveAt(sheet.Name);
                    }

                    ConsumablesOutMasterModel master = context.ConsumablesOutMasterModels.FromSqlRaw(
                        @"
                            select distinct m.id,
                                            m.number,
                                            w.companyid,
                                            co.chinesename as companyname,
                                            m.warehouseid,
                                            w.name as warehousename,
                                            m.departmentid,
                                            d.name as departmentname,
                                            to_char(m.outdate, 'YYYY-MM-DD') as outdate,
                                            m.createuserid,
                                            u.realname as createusername,
                                            to_char(m.createtime, 'YYYY-MM-DD HH24:MI:SS') as createtime,
                                            sum(trim_scale(COALESCE(cd.outamount, 0))) over (PARTITION by m.id) as outamount,
                                            sum(trim_scale(COALESCE(cd.totalamount, 0))) over (PARTITION by m.id) as totalamount,
                                            sum(trim_scale(COALESCE(c.buyprice, 0))) over (PARTITION by m.id) as buyprice,
                                            sum(trim_scale(COALESCE(c.sellprice, 0))) over (PARTITION by m.id) as sellprice,
                                            trim_scale(sum(COALESCE(cd.totalamount, 0) * COALESCE(c.buyprice, 0)) over (PARTITION by m.id)) as money,
                                            m.remark
                            from consumablesoutmaster as m
                            LEFT join consumablesoutdetail as cd on m.id = cd.masterid
                            left join consumablesstock as cs on cd.stockid = cs.id
                            LEFT join consumables as c on cs.consumablesid = c.id
                            left join warehouse as w on m.warehouseid = w.id
                            left join company as co on w.companyid = co.id
                            left join department as d on m.departmentid = d.id
                            left join ""user"" as u on m.createuserid = u.id
                            where m.id = {0} ", masterid).FirstOrDefault();

                    Worksheet worksheet = workbook.Worksheets["耗材出库"];

                    IList<ConsumablesOutDetailModel> listDetail = context.ConsumablesOutDetailModels.FromSqlRaw(
                        @"
                            select  d.id,
		                            d.masterid,
                                    d.stockid,
                                    c.categoryid,
                                    cc.name as categoryname,
                                    c.name,
		                            c.number,
		                            cs.batchnumber,
		                            c.specification,
		                            c.baseunitid,
		                            baseunit.name as baseunitname,
		                            c.inunitid,
		                            inunit.name as inunitname,
		                            trim_scale(c.baseamount) as baseamount,
		                            to_char(cs.expiredate, 'YYYY-MM-DD') as expiredate,
                                    c.supplierid,
                                    s.chinesename as suppliername,
		                            c.manufacturerid,
		                            ma.chinesename as manufacturername,
		                            trim_scale(c.buyprice) as buyprice,
		                            trim_scale(c.sellprice) as sellprice,
		                            trim_scale(COALESCE(d.outamount, 0) * COALESCE(c.buyprice, 0)) as money,
		                            trim_scale(cs.stockamount) as stockamount,
		                            trim_scale(cs.totalamount) as stocktotalamount,
		                            trim_scale(d.outamount) as outamount,
		                            trim_scale(d.totalamount) as totalamount,
		                            d.remark
                            from consumablesoutdetail as d
                            inner join consumablesstock as cs on d.stockid = cs.id
                            inner join consumables as c on cs.consumablesid = c.id
                            left join consumablescategory as cc on c.categoryid = cc.id 
                            left join unit as baseunit on c.baseunitid = baseunit.id
                            left join unit as inunit on c.inunitid = inunit.id
                            left join supplier as s on c.supplierid = s.id
                            left join manufacturer as ma on c.manufacturerid = ma.id
                            where d.masterid = {0}
                            order by c.number ", masterid).ToList();

                    ConsumablesOutDetailModel detail;
                    IList<Name> listName = workbook.Worksheets.Names.Where(n => n.SheetIndex == 1).ToList();

                    string saveDir = Path.Combine(configuration["FileServerAbsolutePath"], "Report", dateTime.Year.ToString(), master.id.ToString());

                    if (Directory.Exists(saveDir))
                    {
                        Directory.Delete(saveDir, true);
                    }

                    Directory.CreateDirectory(saveDir);

                    int pageCount;
                    Aspose.Cells.Range rangeConsumables = workbook.Worksheets.GetRangeByName(worksheet.Name + "!耗材");
                    int rowCount = rangeConsumables.RowCount;
                    int lastRowCount, pageCountIndex;
                    if (listDetail.Count % rowCount == 0)
                    {
                        pageCount = listDetail.Count / rowCount;
                        lastRowCount = rowCount;
                    }
                    else
                    {
                        pageCount = listDetail.Count / rowCount + 1;
                        lastRowCount = listDetail.Count % rowCount;
                    }

                    for (int page = 0; page < pageCount; page++)
                    {
                        foreach (Name name in listName)
                        {
                            if (name.Text == "标题")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.companyname + "出库单";
                                continue;
                            }

                            if (name.Text == "出库单号")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.number;
                                continue;
                            }

                            if (name.Text == "仓库")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.warehousename;
                                continue;
                            }

                            if (name.Text == "出库部门")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.departmentname;
                                continue;
                            }

                            if (name.Text == "操作人")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.createusername;
                                continue;
                            }

                            if (name.Text == "操作时间")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.createtime;
                                continue;
                            }

                            if (name.Text == "出库日期")
                            {
                                workbook.Worksheets.GetRangeByName(worksheet.Name + "!" + name.Text).Value = master.outdate;
                                continue;
                            }
                        }

                        // 耗材列表
                        Aspose.Cells.Range rangeConsumablesName = workbook.Worksheets.GetRangeByName(worksheet.Name + "!耗材名称");
                        Aspose.Cells.Range rangeSpecification = workbook.Worksheets.GetRangeByName(worksheet.Name + "!规格型号");
                        Aspose.Cells.Range rangeNumber = workbook.Worksheets.GetRangeByName(worksheet.Name + "!编号");
                        Aspose.Cells.Range rangeUnit = workbook.Worksheets.GetRangeByName(worksheet.Name + "!单位");
                        Aspose.Cells.Range rangeAmount = workbook.Worksheets.GetRangeByName(worksheet.Name + "!数量");
                        Aspose.Cells.Range rangePrice = workbook.Worksheets.GetRangeByName(worksheet.Name + "!单价");
                        Aspose.Cells.Range rangeMoney = workbook.Worksheets.GetRangeByName(worksheet.Name + "!金额");

                        // 最后一页
                        if (page == pageCount - 1)
                        {
                            pageCountIndex = page * rowCount + lastRowCount;
                        }
                        else
                        {
                            pageCountIndex = (page + 1) * rowCount;
                        }

                        // 清空耗材列表
                        rangeConsumables.Value = "";

                        for (int i = page * rowCount; i < pageCountIndex; i++)
                        {
                            detail = listDetail[i];

                            rangeConsumablesName.GetOffset(i % rowCount + 1, 0).Value = detail.name;
                            rangeSpecification.GetOffset(i % rowCount + 1, 0).Value = detail.specification;
                            rangeNumber.GetOffset(i % rowCount + 1, 0).Value = detail.number;
                            rangeUnit.GetOffset(i % rowCount + 1, 0).Value = detail.inunitname;
                            rangeAmount.GetOffset(i % rowCount + 1, 0).Value = detail.outamount;
                            rangePrice.GetOffset(i % rowCount + 1, 0).Value = detail.buyprice;
                            rangeMoney.GetOffset(i % rowCount + 1, 0).Value = detail.money;
                        }

                        // 单页合计
                        rangeAmount.GetOffset(13, 0).GetCellOrNull(0, 0).Formula = "=SUM(H5:H16)";
                        rangePrice.GetOffset(13, 0).GetCellOrNull(0, 0).Formula = "=SUM(I5:I16)";
                        rangeMoney.GetOffset(13, 0).GetCellOrNull(0, 0).Formula = "=SUM(J5:J16)";

                        workbook.CalculateFormula();

                        worksheet.PageSetup.CustomPaperSize(24.1 * 0.393700787402, 14 * 0.393700787402);

                        workbook.Save(Path.Combine(saveDir, (page + 1) + ".pdf"), SaveFormat.Pdf);
                    }

                    List<string> inputDocs = new List<string>();

                    DirectoryInfo directoryInfo = new DirectoryInfo(saveDir);
                    foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.pdf", SearchOption.AllDirectories))
                    {
                        inputDocs.Add(fileInfo.FullName);
                    }

                    Aspose.Pdf.Document inputDoc;
                    Aspose.Pdf.Document targetDoc = new Aspose.Pdf.Document();

                    if (inputDocs.Count == 0)
                    {
                        resultModel.success = false;
                        resultModel.info = "没有耗材，无法打印！";
                        return JsonConvert.SerializeObject(resultModel);
                    }

                    for (int i = 0; i < inputDocs.Count; i++)
                    {
                        inputDoc = new Aspose.Pdf.Document(inputDocs[i]);
                        targetDoc.Pages.Add(inputDoc.Pages);
                    }

                    relativePath = configuration["FileServerPath"] + "Report/" + dateTime.Year.ToString() + "/" + master.id.ToString() + "/" + master.number + ".pdf";
                    absolutePath = Path.Combine(saveDir, master.number + ".pdf");
                    targetDoc.Save(absolutePath);
                }

                resultModel.success = true;
                resultModel.model = relativePath;
                return JsonConvert.SerializeObject(resultModel);
            }
            catch (Exception e)
            {
                resultModel.success = false;
                resultModel.info = e.Message;
            }

            return JsonConvert.SerializeObject(resultModel);
        }

        public async Task<string> ExportConsumablesInventoryReport()
        {
            ResultModel<string> resultModel = new ResultModel<string>();

            try
            {
                DateTime dateTime = DateTime.Now;

                string saveDir = Path.Combine(configuration["FileServerAbsolutePath"], "Report", dateTime.Year.ToString());

                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }

                string relativePath, absolutePath, reportName;
                StreamReader streamReader = new StreamReader(Request.Body);
                string json = await streamReader.ReadToEndAsync();
                List<ConsumablesStockModel> listStock = JsonConvert.DeserializeObject<List<ConsumablesStockModel>>(json);

                if (listStock == null || listStock.Count == 0)
                {
                    resultModel.success = false;
                    resultModel.info = "没有数据，无法导出";

                    return JsonConvert.SerializeObject(resultModel);
                }

                string excelPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/报表.xlsx");

                using (Workbook workbook = new Workbook(excelPath))
                {
                    List<Worksheet> listSheet = new List<Worksheet>();
                    foreach (Worksheet sheet in workbook.Worksheets)
                    {
                        if (sheet.Name != "耗材库存")
                        {
                            listSheet.Add(sheet);
                        }
                    }

                    foreach (Worksheet sheet in listSheet)
                    {
                        workbook.Worksheets.RemoveAt(sheet.Name);
                    }

                    Worksheet worksheet = workbook.Worksheets[0];

                    Row row;
                    int index = 0;
                    foreach (ConsumablesStockModel stockModel in listStock)
                    {
                        index = 3 + listStock.IndexOf(stockModel);
                        worksheet.Cells.InsertRow(index);

                        row = worksheet.Cells.Rows[index];

                        row.GetCellOrNull(0).Value = index - 2;
                        row.GetCellOrNull(1).Value = stockModel.warehousename;
                        row.GetCellOrNull(2).Value = stockModel.name;
                        row.GetCellOrNull(3).Value = stockModel.categoryname;
                        row.GetCellOrNull(4).Value = stockModel.specification;
                        row.GetCellOrNull(5).Value = stockModel.number;
                        row.GetCellOrNull(6).Value = stockModel.batchnumber;
                        row.GetCellOrNull(7).Value = stockModel.baseunitname;
                        row.GetCellOrNull(8).Value = stockModel.inunitname;
                        row.GetCellOrNull(9).Value = stockModel.baseamount;
                        row.GetCellOrNull(10).Value = stockModel.stockamount;
                        row.GetCellOrNull(11).Value = stockModel.totalamount;
                        row.GetCellOrNull(12).Value = stockModel.buyprice;
                        row.GetCellOrNull(13).Value = stockModel.sellprice;
                        row.GetCellOrNull(14).Value = stockModel.money;
                        row.GetCellOrNull(15).Value = stockModel.suppliername;
                        row.GetCellOrNull(16).Value = stockModel.remark;
                    }

                    row = worksheet.Cells.Rows[index + 1];

                    row.GetCellOrNull(9).Formula = "=SUM(J4:J" + (index + 1) + ")";
                    row.GetCellOrNull(10).Formula = "=SUM(K4:K" + (index + 1) + ")";
                    row.GetCellOrNull(11).Formula = "=SUM(L4:L" + (index + 1) + ")";
                    row.GetCellOrNull(12).Formula = "=SUM(M4:M" + (index + 1) + ")";
                    row.GetCellOrNull(13).Formula = "=SUM(N4:N" + (index + 1) + ")";
                    row.GetCellOrNull(14).Formula = "=SUM(O4:O" + (index + 1) + ")";

                    reportName = "耗材库存" + dateTime.ToString("yyyyMMddHHmmssfff");
                    relativePath = configuration["FileServerPath"] + "Report/" + dateTime.Year.ToString() + "/" + reportName + ".xlsx";
                    absolutePath = Path.Combine(saveDir, reportName + ".xlsx");
                    workbook.Save(absolutePath);

                    resultModel.success = true;
                    resultModel.model = relativePath;
                    return JsonConvert.SerializeObject(resultModel);
                }
            }
            catch (Exception e)
            {
                resultModel.success = false;
                resultModel.info = e.Message;
            }

            return JsonConvert.SerializeObject(resultModel);
        }
    }
}