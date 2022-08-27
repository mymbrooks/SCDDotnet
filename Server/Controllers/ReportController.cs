using Aspose.Words;
using Aspose.Words.Layout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ruibu.Core.Library;
using Ruibu.Core.Library.Model;
using Server.Models.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Server.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> logger;
        private IConfiguration configuration;
        private QIContext context;
        private IWebHostEnvironment webHostEnvironment;

        public ReportController(ILogger<ReportController> logger, IConfiguration configuration, QIContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public string Sign(int projectid)
        {
            ResultModel<string> resultModel = new ResultModel<string>();

            try
            {
                Report report = (from r in context.Reports
                                 where r.projectid == projectid
                                 select r).FirstOrDefault();

                if (report == null || string.IsNullOrEmpty(report.fileurl))
                {
                    resultModel.success = false;
                    resultModel.info = "请先上传报告";
                    return JsonConvert.SerializeObject(resultModel);
                }

                string compileSignPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/suzhiwei.png");
                string verifySignPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/zhangzeliang.png");
                string signSignPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/suntao.png");

                DateTime dateTime = DateTime.Now;
                string docPath = Path.Combine(configuration["FileServerAbsolutePath"], report.fileurl);
                Document document = new Document(docPath);
                Aspose.Words.Drawing.Shape shape;
                foreach (Node node in document.GetChildNodes(NodeType.Shape, true))
                {
                    shape = (Aspose.Words.Drawing.Shape)node;

                    if (shape.AlternativeText == "编制")
                    {
                        shape.ImageData.SetImage(compileSignPath);
                        report.compiletime = dateTime;
                    }

                    if (shape.AlternativeText == "审核")
                    {
                        shape.ImageData.SetImage(verifySignPath);
                        report.verifytime = dateTime;
                    }

                    if (shape.AlternativeText == "签发")
                    {
                        shape.ImageData.SetImage(signSignPath);
                        report.signtime = dateTime;
                    }
                }

                document.Save(docPath);

                context.SaveChanges();

                resultModel.success = true;
                resultModel.info = "批量签名成功";
                return JsonConvert.SerializeObject(resultModel);
            }
            catch (Exception e)
            {
                resultModel.success = false;
                resultModel.info = e.Message;
                return JsonConvert.SerializeObject(resultModel);
            }
        }

        public string Seal(int projectid)
        {
            ResultModel<string> resultModel = new ResultModel<string>();

            try
            {
                Report report = (from r in context.Reports
                                 where r.projectid == projectid
                                 select r).FirstOrDefault();

                if (report == null || string.IsNullOrEmpty(report.fileurl))
                {
                    resultModel.success = false;
                    resultModel.info = "请先上传报告";
                    return JsonConvert.SerializeObject(resultModel);
                }

                string sealPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/检测专用章.png");
                if (!System.IO.File.Exists(sealPath))
                {
                    resultModel.success = false;
                    resultModel.info = "检验专用章图片不存在，请联系管理员";
                    return JsonConvert.SerializeObject(resultModel);
                }

                DateTime dateTime = DateTime.Now;
                string docPath;
                Document document;
                NodeCollection paragraphs;
                LayoutCollector collector;
                Paragraph anchorPara;
                Aspose.Words.Drawing.Shape shapeSeal;

                docPath = Path.Combine(configuration["FileServerAbsolutePath"], report.fileurl);
                document = new Document(docPath);

                foreach (Node node in document.GetChildNodes(NodeType.Shape, true))
                {
                    shapeSeal = (Aspose.Words.Drawing.Shape)node;

                    if (shapeSeal.AlternativeText == "签章")
                    {
                        // 添加普通章
                        shapeSeal.ImageData.SetImage(sealPath);
                    }
                }

                // 添加骑缝章
                paragraphs = document.GetChildNodes(NodeType.Paragraph, true);
                collector = new LayoutCollector(document);
                int pageCount = document.PageCount;
                List<Image> listImage = SystemUtil.SplitImage(sealPath, pageCount);
                string saveDir = Path.Combine(configuration["FileServerAbsolutePath"], "Report", report.id.ToString());

                if (Directory.Exists(saveDir))
                {
                    Directory.Delete(saveDir, true);
                }

                Directory.CreateDirectory(saveDir);

                string crossSealPath;
                Aspose.Words.Drawing.Shape shapeCrossSeal;
                int pageIndex = 1;
                int width = 20, height = 120;
                Image image;
                foreach (Paragraph paragraph in paragraphs)
                {
                    if (collector.GetStartPageIndex(paragraph) == pageIndex && paragraph.GetAncestor(NodeType.GroupShape) == null)
                    {
                        crossSealPath = Path.Combine(saveDir, pageIndex + ".png");
                        image = SystemUtil.ResizeImage(listImage[pageIndex - 1], width, height);
                        image.Save(crossSealPath);

                        anchorPara = paragraph;

                        shapeCrossSeal = new Aspose.Words.Drawing.Shape(document, Aspose.Words.Drawing.ShapeType.Image);
                        shapeCrossSeal.Left = 500;
                        shapeCrossSeal.Top = 50;

                        shapeCrossSeal.ImageData.SetImage(crossSealPath);

                        shapeCrossSeal.WrapType = Aspose.Words.Drawing.WrapType.None;
                        shapeCrossSeal.AlternativeText = "骑缝章";
                        anchorPara.AppendChild(shapeCrossSeal);

                        pageIndex++;
                    }
                }

                report.sealtime = dateTime;

                document.Save(docPath);

                context.SaveChanges();

                resultModel.success = true;
                resultModel.info = "批量签章成功";
                return JsonConvert.SerializeObject(resultModel);
            }
            catch (Exception e)
            {
                resultModel.success = false;
                resultModel.info = e.Message;
                return JsonConvert.SerializeObject(resultModel);
            }
        }

        public string Preview(long projectid)
        {
            ResultModel<string> resultModel = new ResultModel<string>();

            try
            {
                Report report = (from r in context.Reports
                                 where r.projectid == projectid
                                 select r).FirstOrDefault();

                if (report == null || string.IsNullOrEmpty(report.fileurl))
                {
                    resultModel.success = false;
                    resultModel.info = "请先上传报告";
                    return JsonConvert.SerializeObject(resultModel);
                }

                string docPath = Path.Combine(configuration["FileServerAbsolutePath"], report.fileurl);
                DateTime dateTime = DateTime.Now;
                Document document = new Document(docPath);

                string tempPath = Path.Combine(configuration["FileServerAbsolutePath"], "Temp", dateTime.Year.ToString());
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                string saveName = report.number;
                string relativePath = "Temp/" + dateTime.Year.ToString() + "/" + saveName + ".pdf";
                string absolutePath = Path.Combine(tempPath, saveName + ".pdf");

                document.Save(absolutePath, SaveFormat.Pdf);

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

        public string Download(long projectid)
        {
            ResultModel<string> resultModel = new ResultModel<string>();

            try
            {
                Report report = (from r in context.Reports
                                 where r.projectid == projectid
                                 select r).FirstOrDefault();

                if (report == null || string.IsNullOrEmpty(report.fileurl))
                {
                    resultModel.success = false;
                    resultModel.info = "请先上传报告";
                    return JsonConvert.SerializeObject(resultModel);
                }

                resultModel.success = true;
                resultModel.model = configuration["FileServerPath"] + report.fileurl;
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