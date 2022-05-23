using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Aspose.Words.License licenseWords = new Aspose.Words.License();
            licenseWords.SetLicense("./Libs/License.txt");

            Aspose.Cells.License licenseCells = new Aspose.Cells.License();
            licenseCells.SetLicense("./Libs/License.txt");

            Aspose.Pdf.License licensePdf = new Aspose.Pdf.License();
            licensePdf.SetLicense("./Libs/License.txt");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}