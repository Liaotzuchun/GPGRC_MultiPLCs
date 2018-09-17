using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GPGO_MultiPLCs
{
    /// <summary>App.xaml 的互動邏輯</summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if(e.ApplicationExitCode != 23555277)
            {
                ErrorRecoder.RecordError(null, "程序未經授權中止");
            }
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            ErrorRecoder.RecordError(null, "程序未正常中止");
        }

        public App()
        {
            var temp = Process.GetProcesses();
            var mp = Process.GetCurrentProcess();
            temp.Where(p => p.ProcessName.ToLower().Contains(mp.ProcessName.ToLower()) && p.Id != mp.Id).ToList().ForEach(p => p.Kill());

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                                                          {
                                                              ((Exception)e.ExceptionObject).RecordError();

                                                              if (e.IsTerminating)
                                                              {
                                                                  //Process.Start(ResourceAssembly.Location);
                                                                  //Current.Shutdown();
                                                              }
                                                          };

            TaskScheduler.UnobservedTaskException += (s, e) =>
                                                     {
                                                         e.Exception.RecordError();
                                                         e.SetObserved();

                                                         //Process.Start(ResourceAssembly.Location);
                                                         //Current.Shutdown();
                                                     };

            DispatcherUnhandledException += (s, e) =>
                                            {
                                                e.Handled = true;
                                                e.Exception.RecordError();

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
                    ex.RecordError("Mongo嘗試啟動失敗");
                }
            }
        }
    }
}