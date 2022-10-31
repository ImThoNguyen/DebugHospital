using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using DebugHospital.Interface;
using DebugHospital.BLL;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Display;
using System.Globalization;
using LibGit2Sharp.Handlers;

namespace DebugHospital
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";
            Serilog.Formatting.Display.MessageTemplateTextFormatter tf = new Serilog.Formatting.Display.MessageTemplateTextFormatter(outputTemplate, CultureInfo.InvariantCulture);

            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Information()
           .WriteTo.CustomSink(tf, LogHandler)
           .CreateLogger();

            // Log.Logger = new LoggerConfiguration()
            //.MinimumLevel.Debug()
            //.WriteTo.File(new MessageTemplateTextFormatter(outputTemplate, null), "Logs/DebugHospitalLog_{Date}.txt","" , rollingInterval: RollingInterval.Day)
            //.CreateLogger();

            Log.Information("Start App");

            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;

            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }
        public static IServiceProvider ServiceProvider { get; private set; }
        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Trace).AddDebug())
                .ConfigureServices((context, services) => {
                    services.AddSingleton<IColumnDataBLL>(provider  => new ColumnDataBLL(provider.GetService<ILogger<ColumnDataBLL>>()));
                    services.AddTransient<IResultBLL, ResultBLL>();
                    services.AddTransient<MainForm>();
                })
                ;
        }

    }
}
