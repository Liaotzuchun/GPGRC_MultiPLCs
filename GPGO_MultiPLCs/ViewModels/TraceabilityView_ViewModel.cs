using System;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using MongoDB.Driver;

namespace GPGO_MultiPLCs.ViewModels
{
    public class TraceabilityView_ViewModel : ViewModelBase
    {
        private readonly MongoClient Mongo_Client;
        private DateTime _Date1;
        private DateTime _Date2;
        private int _Index1;
        private int _Index2;

        private ProcessInfo[] _Results;
        private bool _Standby;
        private int _StationIndex;
        private ProcessInfo[] _ViewResults;

        public ProcessInfo[] Results
        {
            get => _Results;
            set
            {
                _Results = value;
                NotifyPropertyChanged();
            }
        }

        public ProcessInfo[] ViewResults
        {
            get => _ViewResults;
            set
            {
                _ViewResults = value;
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
                NotifyPropertyChanged(nameof(LowerDate));
            }
        }

        public int Index2
        {
            get => _Index2;
            set
            {
                _Index2 = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(UpperDate));
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

        public int StationIndex
        {
            get => _StationIndex;
            set
            {
                _StationIndex = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime? LowerDate => _Results?[_Index1]?.AddedTime;
        public DateTime? UpperDate => _Results?[_Index2]?.AddedTime;

        public RelayCommand LoadedCommand { get; }

        public TraceabilityView_ViewModel(MongoClient mongo)
        {
            Mongo_Client = mongo;

            LoadedCommand = new RelayCommand(o =>
                                             {
                                                 Date1 = DateTime.Today;
                                                 Date2 = DateTime.Today;
                                             });
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