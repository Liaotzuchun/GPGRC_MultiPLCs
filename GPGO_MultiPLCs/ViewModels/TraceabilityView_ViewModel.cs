using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GPGO_MultiPLCs.ViewModels
{
    public class TraceabilityView_ViewModel : ViewModelBase
    {
        private readonly MongoClient Mongo_Client;

        private ICollection<ProcessInfo> _Results;
        private DateTime _Date1;
        private DateTime _Date2;
        private int _Index1;
        private int _Index2;
        private int _TotalCount;
        private bool _Standby;

        public ICollection<ProcessInfo> Results
        {
            get => _Results;
            set
            {
                _Results = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime Date1
        {
            get => _Date1;
            set
            {
                _Date1 = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime Date2
        {
            get => _Date2;
            set
            {
                _Date2 = value;
                NotifyPropertyChanged();
            }
        }

        public int Index1
        {
            get => _Index1;
            set
            {
                _Index1 = value;
                NotifyPropertyChanged();
            }
        }

        public int Index2
        {
            get => _Index2;
            set
            {
                _Index2 = value;
                NotifyPropertyChanged();
            }
        }

        public int TotalCount
        {
            get => _TotalCount;
            set
            {
                _TotalCount = value;
                NotifyPropertyChanged();
            }
        }

        public bool Standby
        {
            get => _Standby;
            set
            {
                _Standby = value;
                NotifyPropertyChanged();
            }
        }

        public TraceabilityView_ViewModel(MongoClient mongo)
        {
            Mongo_Client = mongo;
        }

        public async void AddToDB(int index, ProcessInfo info)
        {
            info.StationNumber = index;
            info.AddedTime = DateTime.Now;

            try
            {
                var db = Mongo_Client.GetDatabase("GP");
                var Sets = db.GetCollection<ProcessInfo>("Product_Infos");

                await Sets.InsertOneAsync(info);

            }
            catch (Exception ex)
            {
                ErrorRecoder.RecordError(ex, "生產紀錄寫入資料庫失敗");
            }
        }
    }
}