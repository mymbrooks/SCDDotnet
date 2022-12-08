using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using Server.Models.Domain;

namespace Server
{
    public class Startup
    {
        readonly string Cors = "Cors";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<QIContext>(options => options.UseNpgsql(Configuration.GetConnectionString("qi")));

            services.AddCors(options =>
            {
                options.AddPolicy(Cors,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:3000",
                                                          "http://47.96.41.13:1200")
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod();
                                  });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                PrivateKeyFile privateKeyFile = new PrivateKeyFile(@"D:\WeiYun\Cloud\Aliyun\ruibu.pem");
                using (SshClient client = new SshClient("47.96.41.13", 22, "root", privateKeyFile))
                {
                    client.Connect();
                    ForwardedPortLocal forwardedPortLocal = new ForwardedPortLocal(5433, "localhost", 5432);
                    client.AddForwardedPort(forwardedPortLocal);
                }
            }

            app.UseRouting();

            app.UseCors(Cors);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}