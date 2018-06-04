﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using GPGO_MultiPLCs.GP_PLCs;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GPGO_MultiPLCs.ViewModels
{
    public class TotalView_ViewModel : ViewModelBase, IGPServiceCallback
    {
        void IGPServiceCallback.Status_Changed(int index, bool val)
        {
            if (index < PLC_Count)
            {
                PLC_All[index].OnlineStatus = val;
            }
        }

        void IGPServiceCallback.M_Changed(int index, Dictionary<int, bool> val)
        {
            if (index < PLC_Count)
            {
                foreach (var v in val)
                {
                    PLC_All[index].M_Values[v.Key] = v.Value;
                }
            }
        }

        void IGPServiceCallback.D_Changed(int index, Dictionary<int, short> val)
        {
            if (index < PLC_Count)
            {
                foreach (var v in val)
                {
                    PLC_All[index].D_Values[v.Key] = v.Value;
                }
            }
        }

        public delegate void WantRecipeHandler(int index, string recipe);

        private const int PLC_Count = 20;
        private const int Check_Dev = 21; //心跳信號位置
        private readonly Timer Checker;
        private readonly InstanceContext site;
        private bool _Gate_Status;
        private int _Index; //Tab頁面的index
        private int _ViewIndex = -1; //選取PLC的index
        private GPServiceClient PLC_Client;

        public PLC_Data[] PLC_All { get; }
        public RelayCommand BackCommand { get; }
        public PlotModel HistogramView { get; set; }

        public int Index
        {
            get => _Index;
            set
            {
                _Index = value;
                NotifyPropertyChanged();
                if (value == 0)
                {
                    ViewIndex = -1;
                }
            }
        }

        public int ViewIndex
        {
            get => _ViewIndex;
            set
            {
                _ViewIndex = value;
                NotifyPropertyChanged();
                if (value > -1)
                {
                    NotifyPropertyChanged(nameof(PLC_In_Focused));
                    Index = 1;
                }
            }
        }

        public bool Gate_Status
        {
            get => _Gate_Status;
            set
            {
                _Gate_Status = value;
                NotifyPropertyChanged();
            }
        }

        public PLC_Data PLC_In_Focused => _ViewIndex > -1 ? PLC_All[_ViewIndex] : null;

        public TotalView_ViewModel(IDialogService<string> dialog)
        {
            IniPlotView();

            site = new InstanceContext(this);

            BackCommand = new RelayCommand(o =>
                                           {
                                               Index = o is int i ? i : 0;
                                           });

            PLC_All = new PLC_Data[PLC_Count];

            var M_List = new Dictionary<SignalNames, int>
                         {
                             { SignalNames.PC_ByPass, 20 },
                             { SignalNames.自動模式, 50 },
                             { SignalNames.自動啟動, 51 },
                             { SignalNames.自動停止, 52 },
                             { SignalNames.手動模式, 60 },
                             { SignalNames.降溫中, 208 },
                             { SignalNames.程式結束, 209 },
                             { SignalNames.加熱門未關, 250 },
                             { SignalNames.緊急停止, 700 },
                             { SignalNames.溫控器低溫異常, 701 },
                             { SignalNames.電源反相, 702 },
                             { SignalNames.OTP超溫異常, 703 },
                             { SignalNames.循環風車過載, 704 },
                             { SignalNames.冷卻進氣風車異常, 705 },
                             { SignalNames.超溫警報, 710 },
                             { SignalNames.停止後未開門, 715 },
                             { SignalNames.循環風車INV異常, 718 },
                             { SignalNames.充氮氣逾時, 721 },
                             { SignalNames.門未關定位異常, 722 },
                             { SignalNames.升恆溫逾時, 723 }
                         };

            var D_List = new Dictionary<DataNames, int>
                         {
                             { DataNames.溫控器溫度, 130 },
                             { DataNames.片段剩餘時間, 132 },
                             { DataNames.總剩餘時間, 134 },
                             { DataNames.目前段數, 140 },
                             { DataNames.爐內溫度_1, 380 },
                             { DataNames.爐內溫度_2, 381 },
                             { DataNames.爐內溫度_3, 382 },
                             { DataNames.爐內溫度_4, 383 },
                             { DataNames.爐內溫度_5, 384 },
                             { DataNames.爐內溫度_6, 385 },
                             { DataNames.爐內溫度_7, 386 },
                             { DataNames.爐內溫度_8, 387 }
                         };

            var Recipe_List = new Dictionary<DataNames, int>
                              {
                                  { DataNames.目標溫度_1, 712 },
                                  { DataNames.升溫時間_1, 713 },
                                  { DataNames.恆溫溫度_1, 714 },
                                  { DataNames.恆溫時間_1, 715 },
                                  { DataNames.目標溫度_2, 716 },
                                  { DataNames.升溫時間_2, 717 },
                                  { DataNames.恆溫溫度_2, 718 },
                                  { DataNames.恆溫時間_2, 719 },
                                  { DataNames.目標溫度_3, 720 },
                                  { DataNames.升溫時間_3, 721 },
                                  { DataNames.恆溫溫度_3, 722 },
                                  { DataNames.恆溫時間_3, 723 },
                                  { DataNames.目標溫度_4, 724 },
                                  { DataNames.升溫時間_4, 725 },
                                  { DataNames.恆溫溫度_4, 726 },
                                  { DataNames.恆溫時間_4, 727 },
                                  { DataNames.目標溫度_5, 728 },
                                  { DataNames.升溫時間_5, 729 },
                                  { DataNames.恆溫溫度_5, 730 },
                                  { DataNames.恆溫時間_5, 731 },
                                  { DataNames.目標溫度_6, 732 },
                                  { DataNames.升溫時間_6, 733 },
                                  { DataNames.恆溫溫度_6, 734 },
                                  { DataNames.恆溫時間_6, 735 },
                                  { DataNames.目標溫度_7, 736 },
                                  { DataNames.升溫時間_7, 737 },
                                  { DataNames.恆溫溫度_7, 738 },
                                  { DataNames.恆溫時間_7, 739 },
                                  { DataNames.目標溫度_8, 740 },
                                  { DataNames.升溫時間_8, 741 },
                                  { DataNames.恆溫溫度_8, 742 },
                                  { DataNames.恆溫時間_8, 743 },
                                  { DataNames.降溫溫度, 745 },
                                  { DataNames.充氣時間, 747 },
                                  { DataNames.使用段數, 749 },
                                  { DataNames.配方名稱_01, 750 },
                                  { DataNames.配方名稱_02, 751 },
                                  { DataNames.配方名稱_03, 752 },
                                  { DataNames.配方名稱_04, 753 },
                                  { DataNames.配方名稱_05, 754 },
                                  { DataNames.配方名稱_06, 755 },
                                  { DataNames.配方名稱_07, 756 },
                                  { DataNames.配方名稱_08, 757 },
                                  { DataNames.配方名稱_09, 758 },
                                  { DataNames.配方名稱_10, 759 },
                                  { DataNames.配方名稱_11, 760 },
                                  { DataNames.配方名稱_12, 761 },
                                  { DataNames.配方名稱_13, 762 }
                              };

            for (var i = 0; i < PLC_Count; i++)
            {
                PLC_All[i] = new PLC_Data(M_List, D_List, Recipe_List);
                var j = i;
                PLC_All[i].SwitchRecipeEvent += recipe =>
                                                {
                                                    WantRecipe?.Invoke(j, recipe);
                                                };
            }

            var namelists = M_List.Values.OrderBy(x => x).Select(x => "M" + x.ToString()).Concat(D_List.Values.OrderBy(x => x).Select(x => "D" + x.ToString())).ToList();
            var namearray = new[]
                            {
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray(),
                                namelists.ToArray()
                            };

            Checker = new Timer(o =>
                                {
                                    if (!Gate_Status)
                                    {
                                        if (Connect() && Initial() && SetReadLists(namearray))
                                        {
                                            Gate_Status = true;
                                        }
                                    }
                                    else if (!Check())
                                    {
                                        Gate_Status = false;

                                        foreach (var plc in PLC_All)
                                        {
                                            plc.OnlineStatus = false;
                                            plc.IsRecording = false;
                                        }
                                    }

                                    Checker.Change(150, Timeout.Infinite);
                                },
                                null,
                                0,
                                Timeout.Infinite);
        }

        public event WantRecipeHandler WantRecipe;

        public bool SetReadLists(string[][] list)
        {
            try
            {
                if (PLC_Client.State != CommunicationState.Opened)
                {
                    return false;
                }

                PLC_Client.SetReadLists(list);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetRecipe(int index, PLC_Recipe recipe)
        {
            PLC_All[index].RecipeName = recipe.RecipeName;
            PLC_All[index].TargetTemperature_1 = recipe.TargetTemperature_1;
            PLC_All[index].TargetTemperature_2 = recipe.TargetTemperature_2;
            PLC_All[index].TargetTemperature_3 = recipe.TargetTemperature_3;
            PLC_All[index].TargetTemperature_4 = recipe.TargetTemperature_4;
            PLC_All[index].TargetTemperature_5 = recipe.TargetTemperature_5;
            PLC_All[index].TargetTemperature_6 = recipe.TargetTemperature_6;
            PLC_All[index].TargetTemperature_7 = recipe.TargetTemperature_7;
            PLC_All[index].TargetTemperature_8 = recipe.TargetTemperature_8;
            PLC_All[index].HeatingTime_1 = recipe.HeatingTime_1;
            PLC_All[index].HeatingTime_2 = recipe.HeatingTime_2;
            PLC_All[index].HeatingTime_3 = recipe.HeatingTime_3;
            PLC_All[index].HeatingTime_4 = recipe.HeatingTime_4;
            PLC_All[index].HeatingTime_5 = recipe.HeatingTime_5;
            PLC_All[index].HeatingTime_6 = recipe.HeatingTime_6;
            PLC_All[index].HeatingTime_7 = recipe.HeatingTime_7;
            PLC_All[index].HeatingTime_8 = recipe.HeatingTime_8;
            PLC_All[index].ConstantTemperature_1 = recipe.ConstantTemperature_1;
            PLC_All[index].ConstantTemperature_2 = recipe.ConstantTemperature_2;
            PLC_All[index].ConstantTemperature_3 = recipe.ConstantTemperature_3;
            PLC_All[index].ConstantTemperature_4 = recipe.ConstantTemperature_4;
            PLC_All[index].ConstantTemperature_5 = recipe.ConstantTemperature_5;
            PLC_All[index].ConstantTemperature_6 = recipe.ConstantTemperature_6;
            PLC_All[index].ConstantTemperature_7 = recipe.ConstantTemperature_7;
            PLC_All[index].ConstantTemperature_8 = recipe.ConstantTemperature_8;
            PLC_All[index].ConstantTime_1 = recipe.ConstantTime_1;
            PLC_All[index].ConstantTime_2 = recipe.ConstantTime_2;
            PLC_All[index].ConstantTime_3 = recipe.ConstantTime_3;
            PLC_All[index].ConstantTime_4 = recipe.ConstantTime_4;
            PLC_All[index].ConstantTime_5 = recipe.ConstantTime_5;
            PLC_All[index].ConstantTime_6 = recipe.ConstantTime_6;
            PLC_All[index].ConstantTime_7 = recipe.ConstantTime_7;
            PLC_All[index].ConstantTime_8 = recipe.ConstantTime_8;
            PLC_All[index].CoolingTemperature = recipe.CoolingTemperature;
            PLC_All[index].InflatingTime = recipe.InflatingTime;
            PLC_All[index].UsedSegmentCounts = recipe.UsedSegmentCounts;

            if (PLC_Client.State == CommunicationState.Opened)
            {
                PLC_Client.Set_D(index, PLC_All[index].Recipe_Values.GetKeyValuePairsOfKey2().ToDictionary(x => x.Key, x => x.Value));
            }
        }

        public void SetRecipeNames(ICollection<string> names)
        {
            foreach (var plc in PLC_All)
            {
                plc.Recipe_Names = names;
            }
        }

        public void IniPlotView()
        {
            var color = OxyColor.FromRgb(50, 70, 60);

            HistogramView = new PlotModel
                            {
                                PlotAreaBackground = OxyColor.FromArgb(0, 0, 0, 0),
                                DefaultFont = "Microsoft JhengHei",
                                PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
                                PlotMargins = new OxyThickness(20, 10, 10, 20)
                            };

            var categoryAxis1 = new CategoryAxis
                                {
                                    MajorGridlineColor = color,
                                    MinorGridlineColor = color,
                                    TicklineColor = color,
                                    ExtraGridlineColor = color,
                                    TextColor = color,
                                    TickStyle = TickStyle.Inside,
                                    AxislineStyle = LineStyle.Solid,
                                    AxislineColor = color,
                                    GapWidth = 0,
                                    MinorStep = 1,
                                    Position = AxisPosition.Left
                                };

            categoryAxis1.ActualLabels.Add("1");
            categoryAxis1.ActualLabels.Add("2");
            categoryAxis1.ActualLabels.Add("3");
            categoryAxis1.ActualLabels.Add("4");
            categoryAxis1.ActualLabels.Add("5");
            categoryAxis1.ActualLabels.Add("6");
            categoryAxis1.ActualLabels.Add("7");
            categoryAxis1.ActualLabels.Add("8");
            categoryAxis1.ActualLabels.Add("9");
            categoryAxis1.ActualLabels.Add("10");
            categoryAxis1.ActualLabels.Add("11");
            categoryAxis1.ActualLabels.Add("12");
            categoryAxis1.ActualLabels.Add("13");
            categoryAxis1.ActualLabels.Add("14");
            categoryAxis1.ActualLabels.Add("15");
            categoryAxis1.ActualLabels.Add("16");
            categoryAxis1.ActualLabels.Add("17");
            categoryAxis1.ActualLabels.Add("18");
            categoryAxis1.ActualLabels.Add("19");
            categoryAxis1.ActualLabels.Add("20");
            categoryAxis1.ActualLabels.Reverse();

            var XAxis = new LinearAxis
                        {
                            MinimumPadding = 0,
                            MaximumPadding = 0,
                            TickStyle = TickStyle.Inside,
                            MajorGridlineStyle = LineStyle.None,
                            MajorStep = 100,
                            MinorGridlineStyle = LineStyle.None,
                            MinorTickSize = 0,
                            MinorStep = 100,
                            Position = AxisPosition.Bottom,
                            AxislineStyle = LineStyle.Solid,
                            AxislineColor = color,
                            MajorGridlineColor = color,
                            MinorGridlineColor = color,
                            TicklineColor = color,
                            ExtraGridlineColor = color,
                            TextColor = color,
                            Minimum = 0
                        };

            var barSeries1 = new BarSeries { LabelFormatString = "{0}", ValueField = "Value" };

            HistogramView.Axes.Add(categoryAxis1);
            HistogramView.Axes.Add(XAxis);
            HistogramView.Series.Add(barSeries1);
        }

        private bool Connect()
        {
            try
            {
                PLC_Client = new GPServiceClient(site);
                PLC_Client.Open();

                return PLC_Client.State == CommunicationState.Opened;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool Initial()
        {
            try
            {
                if (PLC_Client.State != CommunicationState.Opened)
                {
                    return false;
                }

                PLC_Client.Initial();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool Check()
        {
            try
            {
                if (PLC_Client.State != CommunicationState.Opened)
                {
                    return false;
                }

                PLC_Client.CheckSignal(Check_Dev);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}