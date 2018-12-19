using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;

namespace GPGO_MultiPLCs
{
    /// <summary>App.xaml 的互動邏輯</summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (e.ApplicationExitCode != 23555277)
            {
                Log.Error("程序未經授權中止");
            }
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            Log.Error("程序未正常中止");
        }

        public App()
        {
            var temp = Process.GetProcesses();
            var mp = Process.GetCurrentProcess();
            temp.Where(p => p.ProcessName.ToLower().Contains(mp.ProcessName.ToLower()) && p.Id != mp.Id).ToList().ForEach(p => p.Kill());

            Log.Logger = new LoggerConfiguration().WriteTo.File("C:\\GP\\Errors\\log.txt", rollingInterval: RollingInterval.Day, shared: true, encoding: Encoding.UTF8).CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                                                          {
                                                              Log.Error((Exception)e.ExceptionObject, "");

                                                              if (e.IsTerminating)
                                                              {
                                                                  //Process.Start(ResourceAssembly.Location);
                                                                  //Current.Shutdown();
                                                              }
                                                          };

            TaskScheduler.UnobservedTaskException += (s, e) =>
                                                     {
                                                         Log.Error(e.Exception, "");
                                                         e.SetObserved();

                                                         //Process.Start(ResourceAssembly.Location);
                                                         //Current.Shutdown();
                                                     };

            DispatcherUnhandledException += (s, e) =>
                                            {
                                                e.Handled = true;
                                                Log.Error(e.Exception, "");

                                                //Process.Start(ResourceAssembly.Location);
                                                //Current.Shutdown();
                                            };

            //!以下方式在win10時，須保證在系統管理員權限下執行才可成功，可在app.config中加入<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />，
            //!但一旦指定為系統管理員權限，就無法再start menu中自動啟動，須改用工作管理員設定為登入後啟動

            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
            var mongo_service = new ServiceController("MongoDB");
            if (mongo_service.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    mongo_service.Start();
                    mongo_service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(9));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Mongo嘗試啟動失敗");
                }
            }
        }
    }
}