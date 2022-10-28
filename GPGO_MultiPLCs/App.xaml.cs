using GPGO_MultiPLCs.Views;
using GPMVVM.Helpers;
using Microsoft.Win32;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GPGO_MultiPLCs;

/// <summary>App.xaml 的互動邏輯</summary>
public partial class App
{
    public string GetServiceInstallPath(string serviceName)
    {
        var imagePath = (string)Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\services\\{serviceName}")?.GetValue("ImagePath");
        if (string.IsNullOrEmpty(imagePath))
        {
            return imagePath;
        }

        if (imagePath[0] == '"')
        {
            return imagePath.Substring(1, imagePath.IndexOf('"', 1) - 1);
        }

        var indexOfParameters = imagePath.IndexOf(' ');

        return indexOfParameters >= 0 ? imagePath.Remove(indexOfParameters) : imagePath;
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        if (e.ApplicationExitCode != 23555277)
        {
            Log.Error("程序未經授權中止");
        }
    }

    private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e) { Log.Error("程序未正常中止"); }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var splash = new SplashWindow();
        splash.Show();

        var main = new MainWindow();
        main.Loaded += (_, _) =>
                       {
                           splash.Close();
                       };

        main.Show();
    }

    public App()
    {
        var temp = Process.GetProcesses();
        var mp   = Process.GetCurrentProcess();
        temp.ForEach(p =>
                     {
                         if (p.ProcessName.ToLower().Contains(mp.ProcessName.ToLower()) && p.Id != mp.Id)
                         {
                             p.Kill();
                         }
                     });

        Log.Logger = new LoggerConfiguration().WriteTo.File("C:\\GP\\Logs\\log.txt", 
                                                            rollingInterval: RollingInterval.Day, 
                                                            shared: true, 
                                                            encoding: Encoding.UTF8, 
                                                            retainedFileTimeLimit: TimeSpan.FromDays(180)).CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                                                      {
                                                          Log.Error((Exception)e.ExceptionObject, "");

                                                          if (e.IsTerminating)
                                                          {
                                                              //Process.Start(ResourceAssembly.Location);
                                                              //Current.Shutdown();
                                                          }
                                                      };

        TaskScheduler.UnobservedTaskException += (_, e) =>
                                                 {
                                                     Log.Error(e.Exception, "");
                                                     e.SetObserved();

                                                     //Process.Start(ResourceAssembly.Location);
                                                     //Current.Shutdown();
                                                 };

        DispatcherUnhandledException += (_, e) =>
                                        {
                                            e.Handled = true;
                                            Log.Error(e.Exception, "");

                                            //Process.Start(ResourceAssembly.Location);
                                            //Current.Shutdown();
                                        };

        //! 當啟MongoDB服務或進程不存在時動MongoDB
        if (Process.GetProcessesByName("mongod").Length == 0)
        {
            var path = GetServiceInstallPath("MongoDB");

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var process = new Process
                              {
                                  StartInfo = new ProcessStartInfo
                                              {
                                                  FileName    = path,
                                                  Arguments   = "--config \"C:\\Program Files\\MongoDB\\Server\\4.4\\bin\\mongod.cfg\"",
                                                  WindowStyle = ProcessWindowStyle.Hidden
                                              }
                              };
                process.Start();
            }
        }

        //! 以下方式在win10時，須保證在系統管理員權限下執行才可成功，可在app.config中加入<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />，
        //! 但一旦指定為系統管理員權限，就無法再start menu中自動啟動，須改用工作管理員設定為登入後啟動
        //var mongo_service = new ServiceController("MongoDB");
        //if (mongo_service.Status == ServiceControllerStatus.Stopped)
        //{
        //try
        //{
        //    mongo_service.Start();
        //    mongo_service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(9));
        //}
        //catch (Exception ex)
        //{
        //    Log.Error(ex, "Mongo嘗試啟動失敗");
        //}
        //}

        BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
        BsonSerializer.RegisterSerializationProvider(new ValueTupleSerializerProvider());
    }
}