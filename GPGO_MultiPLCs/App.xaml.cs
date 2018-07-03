using System;
using System.ServiceProcess;
using System.Windows;
using GPGO_MultiPLCs.Helpers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GPGO_MultiPLCs
{
    /// <summary>
    ///     App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //!以下方式在win10時，須保證在系統管理員權限下執行才可成功，可在app.config中加入<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />，
            //!但一旦指定為系統管理員權限，就無法再start menu中自動啟動，須改用工作管理員設定為登入後啟動

            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
            var mongo_service = new ServiceController("MongoDB");
            if (mongo_service.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    mongo_service.Start();
                }
                catch (Exception ex)
                {
                    ErrorRecoder.RecordError(ex, "Mongo嘗試啟動失敗");
                }

                mongo_service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(9));
            }
        }
    }
}