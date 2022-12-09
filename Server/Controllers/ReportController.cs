using Aspose.Pdf;
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
                //Report report = (from r in context.Reports
                //                 where r.projectid == projectid
                //                 select r).FirstOrDefault();

                //if (report == null || string.IsNullOrEmpty(report.fileurl))
                //{
                //    resultModel.success = false;
                //    resultModel.info = "请先上传报告";
                //    return JsonConvert.SerializeObject(resultModel);
                //}

                //string compileSignPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/zhangzeliang.png");
                //string verifySignPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/suzhiwei.png");
                //string signSignPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/suntao.png");

                //DateTime dateTime = DateTime.Now;
                //string docPath = Path.Combine(configuration["FileServerAbsolutePath"], report.fileurl);
                //Aspose.Words.Document document = new Aspose.Words.Document(docPath);
                //Aspose.Words.Drawing.Shape shape;
                //foreach (Node node in document.GetChildNodes(NodeType.Shape, true))
                //{
                //    shape = (Aspose.Words.Drawing.Shape)node;

                //    if (shape.AlternativeText == "编制")
                //    {
                //        shape.ImageData.SetImage(compileSignPath);
                //        report.compiletime = dateTime;
                //    }

                //    if (shape.AlternativeText == "审核")
                //    {
                //        shape.ImageData.SetImage(verifySignPath);
                //        report.verifytime = dateTime;
                //    }

                //    if (shape.AlternativeText == "签发")
                //    {
                //        shape.ImageData.SetImage(signSignPath);
                //        report.signtime = dateTime;
                //    }
                //}

                //document.Save(docPath);

                //context.SaveChanges();

                resultModel.success = true;
                resultModel.info = "批量签名成功";
                return JsonConvert.SerializeObject(resultModel);
            }
            catch (Exception e)
            {
                logger.LogError(e.StackTrace);

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

                string sealLargePath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/检测专用章.png");
                if (!System.IO.File.Exists(sealLargePath))
                {
                    resultModel.success = false;
                    resultModel.info = "检验专用章图片不存在，请联系管理员";
                    return JsonConvert.SerializeObject(resultModel);
                }

                DateTime dateTime = DateTime.Now;
                //string pdfPath;
                //Aspose.Words.Document document;
                //Aspose.Words.Drawing.Shape shapeSeal;

                //pdfPath = Path.Combine(configuration["FileServerAbsolutePath"], report.fileurl);
                //document = new Aspose.Words.Document(pdfPath);

                //foreach (Node node in document.GetChildNodes(NodeType.Shape, true))
                //{
                //    shapeSeal = (Aspose.Words.Drawing.Shape)node;

                //    if (shapeSeal.AlternativeText == "签章")
                //    {
                //        // 添加普通章
                //        shapeSeal.ImageData.SetImage(sealLargePath);
                //    }
                //}


                //document.Save(pdfPath);

                // 添加骑缝章
                string pdfPath = Path.Combine(configuration["FileServerAbsolutePath"], report.fileurl);

                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(pdfPath);
                int pageCount = pdfDocument.Pages.Count;
                string sealSmallPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources/检测专用章small.png");
                string crossSealPath;
                List<System.Drawing.Image> listImage = SystemUtil.SplitImage(sealSmallPath, pageCount);
                System.Drawing.Image image;
                string saveDir = Path.Combine(configuration["FileServerAbsolutePath"], "Report", report.id.ToString());

                if (Directory.Exists(saveDir))
                {
                    Directory.Delete(saveDir, true);
                }

                Directory.CreateDirectory(saveDir);

                int lowerLeftX;
                int lowerLeftY;
                int upperRightX;
                int upperRightY;

                Aspose.Pdf.Rectangle rectangle;
                Aspose.Pdf.Matrix matrix;
                XImage ximage;
                FileStream imageStream;
                int pageIndex;
                foreach (Page page in pdfDocument.Pages)
                {
                    pageIndex = pdfDocument.Pages.IndexOf(page);
                    crossSealPath = saveDir + "/" + pageIndex + ".png";
                    image = listImage[pageIndex - 1];
                    image.Save(crossSealPath);
                    imageStream = new FileStream(crossSealPath, FileMode.Open);
                    page.Resources.Images.Add(imageStream);
                    page.Contents.Add(new Aspose.Pdf.Operators.GSave());

                    lowerLeftX = (int)(page.PageInfo.Width) - image.Width;
                    lowerLeftY = (int)(page.PageInfo.Height / 2) - image.Height / 2;
                    upperRightX = (int)page.PageInfo.Width;
                    upperRightY = (int)(page.PageInfo.Height / 2) + image.Height / 2;

                    rectangle = new Aspose.Pdf.Rectangle(lowerLeftX, lowerLeftY, upperRightX, upperRightY);
                    matrix = new Aspose.Pdf.Matrix(new double[] { rectangle.URX - rectangle.LLX, 0, 0, rectangle.URY - rectangle.LLY, rectangle.LLX, rectangle.LLY });
                    // Using ConcatenateMatrix (concatenate matrix) operator: defines how image must be placed
                    page.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix));

                    ximage = page.Resources.Images[page.Resources.Images.Count];
                    // Using Do operator: this operator draws image
                    page.Contents.Add(new Aspose.Pdf.Operators.Do(ximage.Name));
                    // Using GRestore operator: this operator restores graphics state
                    page.Contents.Add(new Aspose.Pdf.Operators.GRestore());
                }

                pdfDocument.Save(pdfPath);

                report.sealtime = dateTime;

                context.SaveChanges();

                resultModel.success = true;
                resultModel.info = "批量签章成功";
                return JsonConvert.SerializeObject(resultModel);
            }
            catch (Exception e)
            {
                logger.LogError(e.StackTrace);

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

                resultModel.success = true;
                resultModel.model = configuration["FileServerPath"] + report.fileurl;
                return JsonConvert.SerializeObject(resultModel);
            }
            catch (Exception e)
            {
                logger.LogError(e.StackTrace);

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
                logger.LogError(e.StackTrace);

                resultModel.success = false;
                resultModel.info = e.Message;
                return JsonConvert.SerializeObject(resultModel);
            }
        }
    }
}