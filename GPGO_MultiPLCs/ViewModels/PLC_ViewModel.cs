
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GPGRC_MultiPLCs.Models;
using GPMVVM.Core.Helpers;
using GPMVVM.Helpers;
using GPMVVM.Models;
using GPMVVM.PooledCollections;
using MongoDB.Driver;
using PLCService;
using Serilog;
#pragma warning disable VSTHRD110

namespace GPGRC_MultiPLCs.ViewModels;

public sealed class PLC_ViewModel : GOL_DataModel, IDisposable
{
    public event Action?                                                                         ExecutingStarted;
    public event Action?                                                                         RecipeKeyInError;
    public event Func<bool>?                                                                     CheckRbReady;
    public event Action<(double,string)>?                                                        WriteToRB;
    public event Action?                                                                         WantDetail;
    public event Action<(EventType type, DateTime time, string note, string tag, object value)>? EventHappened;
    public event Action<(int,string)>? PanelMoveHappened;
    public event Action<(string opid, string rackid)>?                                           CheckIn;
    public event Action<string, bool>?                                                           InvokeSECSAlarm;
    public event Action<string, object>?                                                         SV_Changed;
    public event Action<string>?                                                                 AssetNumberChanged;
    public event Action<string>?                                                                 CancelCheckIn;
    public event Action<string>?                                                                 CheckOut;
    public event Action<string>?                                                                 InvokeSECSEvent;
    public event Action<string>?                                                                 LotAdded;
    public event Action<string>?                                                                 LotRemoved;
    public event Action<string>?                                                                 MachineCodeChanged;
    public event Func<BaseInfo, Task>?                                                           ExecutingFinished;
    public event Func<string, bool>?                                                             CheckUser;
    public event Func<string, PLC_Recipe?>?                                                      GetRecipe;
    public event Func<bool>                                                                      bChangeStatusevent;

    private readonly CountDownTimer          countDownTimer = new();
    private readonly IDialogService          Dialog;
    private readonly TaskFactory             OneScheduler = new(new StaTaskScheduler(1));
    private          bool                    isCheckin;
    private          bool                    ManualRecord;
    private          CancellationTokenSource CheckRecipeCTS = new();
    private          CancellationTokenSource ppCTS          = new();
    private          DateTime                OfflineTime    = DateTime.MaxValue;
    private          TextBox?                inputFocusTB;
    public ObservableCollection<Item> RecipeItem { get; set; }
    public ObservableCollection<TemperatureItem> TemperatureItems { get; set; }

    #region webservice 功能
    public RelayCommand TopCheckButton { get; }
    public RelayCommand TopIngredients { get; }
    public RelayCommand TopLocalCleanWO { get; }

    public event Action<string> TopDataUploadevent;
    public event Action<int> TopDataUploadTimeevent;
    public event Func<Task> TopIngredientsevent;
    public event Func<string> TopTaskControlevent;

    public bool TopIngredientsButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }
    public string TopBarcode
    {
        get => Get<string>();
        set => Set(value);
    }
    public bool TopBarcodeEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool TopCheckButtonEnabled
    {
        get => Get<bool>();
        set => Set(value);
    }

    public string TopMESMessage
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    #endregion
    public int InputQuantityMin => 0;
    public int InputQuantityMax => 99999;
    public int InputLayerMin => 1;
    public int InputLayerMax => 8;
    //public event Action<PLC_Recipe> RecipeChangedbyPLC;
    public int RecordDelay { get; set; } = 1;
    public int ClearInputDelay { get; set; } = 60;
    public RelayCommand InputFocusCommand { get; }
    //public AsyncCommand StartCommand { get; }
    //public AsyncCommand StopCommand { get; }
    //public RelayCommand SilinceCommand { get; }
    /// <summary>取消投產</summary>
    public RelayCommand CancelCheckInCommand { get; }
    /// <summary>投產</summary>
    public RelayCommand CheckInCommand { get; }
    public CommandWithResult<bool> CheckInDialogCommand { get; }
    public RelayCommand CheckRecipeCommand_KeyIn { get; }
    public RelayCommand CheckRecipeCommand_KeyLeave { get; }
    public RelayCommand AddLotCommand { get; }
    public RelayCommand DeleteLotCommand { get; }
    public RelayCommand GoDetailCommand { get; }
    public RelayCommand ClearOPTextCommand { get; }
    public RelayCommand ClearPartTextCommand { get; }
    public RelayCommand ClearLotTextCommand { get; }
    public RelayCommand ClearRecipeTextCommand { get; }
    public RelayCommand ClearQuantityCommand { get; }
    public RelayCommand CheckIsExecutingCommand { get; }

    /// <summary>機台資訊</summary>
    public BaseInfoWithChart OvenInfo { get; }

    /// <summary>取得是否正在紀錄溫度</summary>
    public bool IsExecuting => ExecutingTask?.Status is TaskStatus.Running or TaskStatus.WaitingForActivation or TaskStatus.WaitingToRun;

    /// <summary>OP輸入的配方名稱</summary>
    public string InputRecipeName
    {
        get => Get<string>();
        set => Set(value);
    }

    public ICollection<string>? Recipe_Names
    {
        get => Get<ICollection<string>>();
        set => Set(value);
    }

    /// <summary>用來紀錄的任務，可追蹤狀態</summary>
    public Task? ExecutingTask
    {
        get => Get<Task>();
        private set => Set(value);
    }

    /// <summary>輸入人員ID</summary>
    public string InputOperatorID
    {
        get => Get<string>();
        set
        {
            value = value.Trim().ToUpper();

            if (value.Length > 12)
            {
                value = value.Substring(0, 12);
            }

            if (CheckUser != null && !CheckUser.Invoke(value))
            {
                Set(string.Empty);
                Dialog.Show(new Dictionary<Language, string>
                            {
                                { Language.TW, "OP權限不符" },
                                { Language.CHS, "OP权限不符" },
                                { Language.EN, "OP permissions error." }
                            },
                            DialogMsgType.Alert);

                InputReFocus();
            }
            else
            {
                Set(value);
                DelayClean();
            }
        }
    }

    /// <summary>輸入料號</summary>
    public string InputPartID
    {
        get => Get<string>();
        set
        {
            value = value.Trim().ToUpper();
            Set(value.Length > 16 ? value.Substring(0, 16) : value);
            DelayClean();
        }
    }

    /// <summary>輸入批號</summary>
    public string InputLotID
    {
        get => Get<string>();
        set
        {
            value = value.Trim().ToUpper();
            //if (value.Length < 10)
            //{
            //    Set(string.Empty);
            //    Dialog.Show(new Dictionary<Language, string>
            //                {
            //                    { Language.TW, "需至少10個字元" },
            //                    { Language.CHS, "需至少10个字符" },
            //                    { Language.EN, "At least 10 chars" }
            //                },
            //                DialogMsgType.Alert);

            //    InputReFocus();
            //}
            //else
            //{
            //    //! 欣興要求批號只取14個字
            //    Set(value.Length > 14 ? value.Substring(0, 14) : value);
            //    DelayClean();
            //}
            Set(value);
        }
    }

    /// <summary>輸入數量</summary>
    public int InputQuantity
    {
        get => Get<int>();
        set
        {
            if (value < InputQuantityMin)
            {
                value = InputQuantityMin;
            }
            else if (value > InputQuantityMax)
            {
                value = InputQuantityMax;
            }

            Set(value);
            DelayClean();
        }
    }

    public int InputLayer
    {
        get => Get<int>();
        set
        {
            if (value < InputLayerMin)
            {
                value = InputLayerMin;
            }
            else if (value > InputLayerMax)
            {
                value = InputLayerMax;
            }

            Set(value);
            DelayClean();
        }
    }

    public bool SecsIsOnline
    {
        get => Get<bool>();
        set => Set(value);
    }

    public bool SecsIsRemoteOnline
    {
        get => Get<bool>();
        set => Set(value);
    }

    public LogEvent SelectedLogEvent
    {
        set => OvenInfo.ChartModel.SetAnnotation(value);
    }
    public Func<object, object> START_Command { get; internal set; }

    public PLC_ViewModel(IDialogService dialog,
                         IGate gate,
                         int plcindex,
                         string plctag,
                         (Dictionary<BitType, int> bits_shift, Dictionary<DataType, int> datas_shift) shift = new()) : base(gate, plcindex, plctag, shift)
    {
        #region WebService功能
        #region

        TopCheckButtonEnabled = false;
        TopIngredientsButtonEnabled = false;


        TopCheckButton = new RelayCommand(_ =>
        {
            TopCheckButtonEnabled = false;
            TopDataUploadevent?.Invoke("Normal");
        });

        #endregion
        #endregion
        InputLayer = InputLayerMin;
        Dialog = dialog;
        ConnectionStatus.ValueChanged += status =>
                                         {
                                             SV_Changed?.Invoke("OnlineStatus", status);
                                             OfflineTime = status ? DateTime.MaxValue : DateTime.Now;
                                         };

        OvenInfo = new BaseInfoWithChart();

        OvenInfo.PropertyChanged += (s, e) =>
                                    {
                                        //! 在機台編號或財產編號變更時需通知儲存
                                        if (s is BaseInfo bi)
                                        {
                                            if (e.PropertyName == nameof(BaseInfo.MachineCode))
                                            {
                                                MachineCodeChanged?.Invoke(bi.MachineCode);
                                            }
                                            else if (e.PropertyName == nameof(BaseInfo.AssetNumber))
                                            {
                                                AssetNumberChanged?.Invoke(bi.AssetNumber);
                                            }
                                        }
                                    };

        InputFocusCommand = new RelayCommand(e => inputFocusTB = e as TextBox);

        CheckRecipeCommand_KeyLeave = new RelayCommand(_ =>
                                                       {
                                                       });

        CheckInDialogCommand = new CommandWithResult<bool>(_ => false);

        CheckInCommand = new RelayCommand(_ =>
                                          {
                                              isCheckin = true;

                                              OvenInfo.OperatorID = InputOperatorID;
                                              DoorLock = true;
                                          });

        CheckIsExecutingCommand = new RelayCommand(e =>
                                                   {
                                                       if (e is MouseButtonEventArgs { Source: ToggleButton tb } args)
                                                       {
                                                           //! 避免烘烤中意外中止
                                                           if (IsExecuting && tb.IsChecked == true)
                                                           {
                                                               args.Handled = true;
                                                           }

                                                           //! 避免CheckIn的OP權限不符
                                                           if (tb.IsChecked == false && CheckUser != null && !CheckUser.Invoke(InputOperatorID))
                                                           {
                                                               dialog.Show(new Dictionary<Language, string>
                                                                           {
                                                                               { Language.TW, "OP權限不符" },
                                                                               { Language.CHS, "OP权限不符" },
                                                                               { Language.EN, "OP permissions error." }
                                                                           },
                                                                           DialogMsgType.Alert);

                                                               args.Handled = true;
                                                           }
                                                       }
                                                   });

        GoDetailCommand = new RelayCommand(_ => WantDetail?.Invoke());

        #region 註冊PLC事件
        //! 只有有註冊PLC點位的值變事件
        ValueChanged += (LogType, data) =>
                        {
                            var (name, value, oldvalue, type, Subscriptions, SubPosition) = data;

                            var nowtime = DateTime.Now;

                            if (LogType == LogType.StatusVariables)
                            {
                                var eventval = (EventType.StatusChanged, nowtime, name, $"{type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);

                                SV_Changed?.Invoke(name, value!);

                                if (value is bool val)
                                {
                                    EventHappened?.Invoke(eventval!);
                                }
                                else if (value is short sv)
                                {

                                }
                            }
                            else if (LogType == LogType.Alert)
                            {
                                var eventval = (EventType.Alert, nowtime, name, $"{(BitType)type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);
                                EventHappened?.Invoke(eventval!);

                                if (value is bool boolval)
                                {
                                }
                            }
                            else if (LogType == LogType.Alarm)
                            {
                                var eventval = (EventType.Alarm, nowtime, name, $"{(BitType)type!}{Subscriptions!.First()}{(SubPosition > -1 ? $"-{SubPosition:X}" : string.Empty)}", value);
                                EventHappened?.Invoke(eventval!);

                                if (value is bool boolval)
                                {
                                }
                            }
                            else if (LogType == LogType.RecipeSet) //PLC配方"設定值"改變時
                            {
                                RecipeItem = InitalItems(plcindex);
                                SV_Changed?.Invoke(name, value!);
                            }
                            else if (LogType == LogType.Trigger)
                            {
                            }
                            else if (LogType == LogType.CustomData)
                            {
                                if (value is bool val)
                                {
                                    if (val)
                                    {
                                        PanelMoveHappened?.Invoke((plcindex, name));
                                    }
                                }
                            }
                        };

        OvenInfo.Products.CollectionChanged += (_, _) =>
                                               {
                                                   using var lots   = OvenInfo.Products.Select(x => x.LotID).Distinct().ToPooledList();
                                                   using var parts  = OvenInfo.Products.Select(x => x.PartID).Distinct().ToPooledList();
                                                   var       panels = OvenInfo.Products.Sum(x => x.Quantity);

                                                   SV_Changed?.Invoke("LotIDs", lots.Count > 0 ? string.Join(",", lots) : string.Empty);
                                                   SV_Changed?.Invoke("PartIDs", parts.Count > 0 ? string.Join(",", parts) : string.Empty);
                                                   SV_Changed?.Invoke("PanelIDs", panels);
                                               };

        #endregion 註冊PLC事件
        RecipeItem = InitalItems(plcindex);
        TemperatureItems = InitalTemperatureItem(plcindex);
    }

    private async void InputReFocus()
    {
        if (inputFocusTB != null)
        {
            await Task.Delay(60);
            Keyboard.Focus(inputFocusTB);
        }
    }

    private async void DelayClean()
    {
        if (await countDownTimer.WaitAsync(TimeSpan.FromSeconds(ClearInputDelay)))
        {
            inputFocusTB = null;
            Set(string.Empty, nameof(InputOperatorID));
            Set(string.Empty, nameof(InputPartID));
            Set(string.Empty, nameof(InputLotID));
            Set(string.Empty, nameof(InputRecipeName));
            Set(InputQuantityMin, nameof(InputQuantity));
            Set(InputLayerMin, nameof(InputLayer));
        }
    }

    public async Task<SetRecipeResult> WriteRecipeToPlcAsync(PLC_Recipe recipe)
    {
        if (AutoMode)
        {
            AutoMode = false;
            await Task.Delay(900).ConfigureAwait(false);
        }
        var a = recipe.ToDictionary(PLCIndex);
        var errs = await ManualSetByPropertiesWithCheck(recipe.ToDictionary(PLCIndex)).ConfigureAwait(false);
        var result = errs.Count == 0 ? SetRecipeResult.成功 : SetRecipeResult.比對不相符;
        if (result == SetRecipeResult.成功)
        {
            AutoMode = true;
        }
        return result;
    }

    /// <summary>重設CancellationTokenSource狀態</summary>
    /// <param name="act">取消動作時執行的委派</param>
    private void ResetStopTokenSource(Action? act = null)
    {
        ppCTS.Dispose();

        ppCTS = new CancellationTokenSource();

        if (act != null)
        {
            ppCTS.Token.Register(act);
        }
    }

    public bool WebRecipetoPLC(string RecipeName, string part)
    {
        try
        {
            var recipe = GetRecipe(RecipeName);
            if (recipe is null)
            {
                Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, "找不到配方，請確認是否創建該配方" },
                            { Language.CHS, "找不到配方，请确认是否创建该配方" }
                        });
                return false;
            }
            if (SetRecipeAsync(recipe).Result != SetRecipeResult.成功)
            {
                return false;
            }
            //Set(lot, nameof(TopLotID));
            //Set(panelcount, nameof(TopQuantity));
            return true;
        }
        catch (Exception ex)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, ex.Message },
                            { Language.CHS, ex.Message }
                        });
            Log.Debug(ex.Message);
            return false;
        }
    }

    public PLC_Recipe GetRecipeCoater() => new PLC_Recipe
    {
        RecipeName = RecipeName,
    };

    public async Task<SetRecipeResult> SetRecipeAsync(PLC_Recipe? recipe)
    {
        if (await WriteRecipeToPlcAsync(recipe).ConfigureAwait(false) == SetRecipeResult.比對不相符)
        {
            Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, $"Coater{PLCIndex+1} 配方切換失敗" },
                            { Language.CHS, $"Coater{PLCIndex+1} 配方切换失敗" }
                        });
            return SetRecipeResult.比對不相符;
        }
        Dialog.Show(new Dictionary<Language, string>
                        {
                            { Language.TW, $"Coater{PLCIndex+1} 配方切換完成" },
                            { Language.CHS, $"Coater{PLCIndex+1} 配方切換完成"}
                        });
        return SetRecipeResult.成功;
    }

    public class Item : ObservableObject
    {
        public string RecipeDESC
        {
            get => Get<string>();
            set => Set(value);
        }

        public string RecipeValue
        {
            get => Get<string>();
            set => Set(value);
        }
    }
    public class TemperatureItem : ObservableObject
    {
        public string TemperatureDESC
        {
            get => Get<string>();
            set => Set(value);
        }

        public string TemperatureValue
        {
            get => Get<string>();
            set => Set(value);
        }
    }

    /// <summary>
    /// 資料蒐集 顯示UI
    /// </summary>
    /// <param name="plcindex"></param>
    /// <returns></returns>
    private ObservableCollection<Item> InitalItems(int plcindex)
    {
        var items = new ObservableCollection<Item>();

        //三台PLC配方點位都是一樣，但客戶某幾段沒有要全看
        var recipeData = new Dictionary<string, string>
        {
            { "塗佈使用", UseCoating == 0 ? "OK" : "NG"  },
            { "塞孔使用", UsePlug == 0 ? "OK" : "NG"  },
            { "塗佈次數", Coatingoftimes.ToString() },
            { "塞孔次數設定", Plugoftimes.ToString() },
            { "塗佈速度設定", CoatingSpeedSetting.ToString() },
            { "塞孔刮刀壓力設定", Blade_Pressure.ToString() },
            { "塗佈壓力設定", CoatingPressureSetting.ToString() },
            { "基板厚度設定", PanelThicknessSetting.ToString() },
            { "入料下降位置設定", LocationOfDrop.ToString() },
            { "板面夾持距離設定", BoardClampingDistance.ToString() },
            { "左前D.BAR壓力設定", D_BarPressureSetting1.ToString() },
            { "右前D.BAR壓力設定", D_BarPressureSetting2.ToString() },
            { "左後D.BAR壓力設定", D_BarPressureSetting3.ToString() },
            { "右後D.BAR壓力設定", D_BarPressureSetting4.ToString() },
            { "標準墨重", StandardInk.ToString() },
            { "墨重誤差值", DifferenceOfInk.ToString() },
            { "烘烤時間設定", BakingTimeSetting.ToString() },
            { "第1段溫度設定值", TemperatureSV1.ToString() },
            { "第2段溫度設定值", TemperatureSV2.ToString() },
        };
        if (plcindex == 0)
        {
            foreach (var kvp in recipeData)
            {
                items.Add(new Item { RecipeDESC = kvp.Key, RecipeValue = kvp.Value.ToString() });
            }
        }
        else if (plcindex == 1)
        {
            foreach (var kvp in recipeData)
            {
                items.Add(new Item { RecipeDESC = kvp.Key, RecipeValue = kvp.Value.ToString() });
            }
        }
        else if (plcindex == 2)
        {
            foreach (var kvp in recipeData)
            {
                items.Add(new Item { RecipeDESC = kvp.Key, RecipeValue = kvp.Value.ToString() });
            }
            items.Add(new Item { RecipeDESC = "第3段溫度設定值", RecipeValue = TemperatureSV3.ToString() });
            items.Add(new Item { RecipeDESC = "第4段溫度設定值", RecipeValue = TemperatureSV4.ToString() });
            items.Add(new Item { RecipeDESC = "第5段溫度設定值", RecipeValue = TemperatureSV5.ToString() });

        }
        NotifyPropertyChanged(nameof(RecipeItem));
        return items;
    }
    private ObservableCollection<TemperatureItem> InitalTemperatureItem(int plcindex)
    {
        var items = new ObservableCollection<TemperatureItem>();
        var TemperatureData = new Dictionary<string, double>
        {
            { "第1段溫度實際值", TemperaturePV1 },
            { "第2段溫度實際值", TemperaturePV2 },
        };
        if (plcindex == 0)
        {
            foreach (var kvp in TemperatureData)
            {
                items.Add(new TemperatureItem { TemperatureDESC = kvp.Key, TemperatureValue = kvp.Value.ToString() });
            }
        }
        else if (plcindex == 1)
        {
            foreach (var kvp in TemperatureData)
            {
                items.Add(new TemperatureItem { TemperatureDESC = kvp.Key, TemperatureValue = kvp.Value.ToString() });
            }
        }
        else if (plcindex == 2)
        {
            foreach (var kvp in TemperatureData)
            {
                items.Add(new TemperatureItem { TemperatureDESC = kvp.Key, TemperatureValue = kvp.Value.ToString() });
            }
            items.Add(new TemperatureItem { TemperatureDESC = "第3段溫度實際值", TemperatureValue = TemperaturePV3.ToString() });
            items.Add(new TemperatureItem { TemperatureDESC = "第4段溫度實際值", TemperatureValue = TemperaturePV4.ToString() });
            items.Add(new TemperatureItem { TemperatureDESC = "第5段溫度實際值", TemperatureValue = TemperaturePV5.ToString() });
        }
        NotifyPropertyChanged(nameof(TemperatureItems));
        return items;
    }
    #region Interface Implementations
    public void Dispose() => ppCTS.Dispose();
    #endregion

}