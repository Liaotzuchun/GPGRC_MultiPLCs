using System;
using GPMVVM.Helpers;
using GPMVVM.Models;
using System.Collections.Generic;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.Helpers;

public static class Extensions
{
    public static string ReaderName;
    public static bool IsReaderInput;

    public static void UsersToCSVFILE(this IEnumerable<User> users, string path)
    {
        FastCSV.WriteFile(path, new[] { "名稱", "密碼", "層級" }, ',', users, o => new[] { o.Name, o.Password, o.Level.ToString() });
    }

    public static List<User> UsersFromCSVFILE(string path)
    {
        return FastCSV.ReadFile<User>(path,
                                      ',',
                                      (o, c) =>
                                      {
                                          o.Name     = c[0];
                                          o.Password = c[1];
                                          if (Enum.TryParse<UserLevel>(c[2], out var lv))
                                          {
                                              o.Level = lv;
                                          }

                                          return true;
                                      });
    }

    public static void RecipesToCSVFILE(this IEnumerable<PLC_Recipe> users, string path)
    {
        FastCSV.WriteFile(path,
                          new[]
                          {
                              "配方名稱",
                              "氮氣模式",
                              "氧含量",
                              "充氣時間",
                              "使用段數",
                              "目標溫度 1",
                              "目標溫度 2",
                              "目標溫度 3",
                              "目標溫度 4",
                              "目標溫度 5",
                              "目標溫度 6",
                              "升溫時間 1",
                              "升溫時間 2",
                              "升溫時間 3",
                              "升溫時間 4",
                              "升溫時間 5",
                              "升溫時間 6",
                              "升溫警報 1",
                              "升溫警報 2",
                              "升溫警報 3",
                              "升溫警報 4",
                              "升溫警報 5",
                              "升溫警報 6",
                              "恆溫時間 1",
                              "恆溫時間 2",
                              "恆溫時間 3",
                              "恆溫時間 4",
                              "恆溫時間 5",
                              "恆溫時間 6",
                              "恆溫警報 1",
                              "恆溫警報 2",
                              "恆溫警報 3",
                              "恆溫警報 4",
                              "恆溫警報 5",
                              "恆溫警報 6",
                              "降溫時間",
                              "降溫溫度"
                          },
                          ',',
                          users,
                          o => new[]
                               {
                                   o.RecipeName,
                                   o.NitrogenMode.ToString(),
                                   o.OxygenContentSet.ToString("0.0"),
                                   o.InflatingTime.ToString("0"),
                                   o.SegmentCounts.ToString(),
                                   o.TemperatureSetpoint_1.ToString("0.0"),
                                   o.TemperatureSetpoint_2.ToString("0.0"),
                                   o.TemperatureSetpoint_3.ToString("0.0"),
                                   o.TemperatureSetpoint_4.ToString("0.0"),
                                   o.TemperatureSetpoint_5.ToString("0.0"),
                                   o.TemperatureSetpoint_6.ToString("0.0"),
                                   o.RampTime_1.ToString("0.0"),
                                   o.RampTime_2.ToString("0.0"),
                                   o.RampTime_3.ToString("0.0"),
                                   o.RampTime_4.ToString("0.0"),
                                   o.RampTime_5.ToString("0.0"),
                                   o.RampTime_6.ToString("0.0"),
                                   o.RampAlarm_1.ToString("0.0"),
                                   o.RampAlarm_2.ToString("0.0"),
                                   o.RampAlarm_3.ToString("0.0"),
                                   o.RampAlarm_4.ToString("0.0"),
                                   o.RampAlarm_5.ToString("0.0"),
                                   o.RampAlarm_6.ToString("0.0"),
                                   o.DwellTime_1.ToString("0.0"),
                                   o.DwellTime_2.ToString("0.0"),
                                   o.DwellTime_3.ToString("0.0"),
                                   o.DwellTime_4.ToString("0.0"),
                                   o.DwellTime_5.ToString("0.0"),
                                   o.DwellTime_6.ToString("0.0"),
                                   o.DwellAlarm_1.ToString("0.0"),
                                   o.DwellAlarm_2.ToString("0.0"),
                                   o.DwellAlarm_3.ToString("0.0"),
                                   o.DwellAlarm_4.ToString("0.0"),
                                   o.DwellAlarm_5.ToString("0.0"),
                                   o.DwellAlarm_6.ToString("0.0"),
                                   o.CoolingTime.ToString("0.0"),
                                   o.CoolingTemperature.ToString("0.0")
                               });
    }

    public static List<PLC_Recipe> RecipesFromCSVFILE(string path)
    {
        return FastCSV.ReadFile<PLC_Recipe>(path,
                                            ',',
                                            (o, c) =>
                                            {
                                                o.RecipeName            = c[0];
                                                o.NitrogenMode          = bool.TryParse(c[1], out var i1) && i1;
                                                o.OxygenContentSet      = double.TryParse(c[2], out var i2) ? i2 : 0;
                                                o.InflatingTime         = double.TryParse(c[3], out var i3) ? i3 : 0;
                                                o.SegmentCounts         = short.TryParse(c[4], out var i4) ? i4 : (short)1;
                                                o.TemperatureSetpoint_1 = double.TryParse(c[5],  out var i5) ? i5 : 0;
                                                o.TemperatureSetpoint_2 = double.TryParse(c[6],  out var i6) ? i6 : 0;
                                                o.TemperatureSetpoint_3 = double.TryParse(c[7],  out var i7) ? i7 : 0;
                                                o.TemperatureSetpoint_4 = double.TryParse(c[8],  out var i8) ? i8 : 0;
                                                o.TemperatureSetpoint_5 = double.TryParse(c[9],  out var i9) ? i9 : 0;
                                                o.TemperatureSetpoint_6 = double.TryParse(c[10], out var i10) ? i10 : 0;
                                                o.RampTime_1            = double.TryParse(c[11], out var i11) ? i11 : 0;
                                                o.RampTime_2            = double.TryParse(c[12], out var i12) ? i12 : 0;
                                                o.RampTime_3            = double.TryParse(c[13], out var i13) ? i13 : 0;
                                                o.RampTime_4            = double.TryParse(c[14], out var i14) ? i14 : 0;
                                                o.RampTime_5            = double.TryParse(c[15], out var i15) ? i15 : 0;
                                                o.RampTime_6            = double.TryParse(c[16], out var i16) ? i16 : 0;
                                                o.RampAlarm_1           = double.TryParse(c[17], out var i17) ? i17 : 0;
                                                o.RampAlarm_2           = double.TryParse(c[18], out var i18) ? i18 : 0;
                                                o.RampAlarm_3           = double.TryParse(c[19], out var i19) ? i19 : 0;
                                                o.RampAlarm_4           = double.TryParse(c[20], out var i20) ? i20 : 0;
                                                o.RampAlarm_5           = double.TryParse(c[21], out var i21) ? i21 : 0;
                                                o.RampAlarm_6           = double.TryParse(c[22], out var i22) ? i22 : 0;
                                                o.DwellTime_1           = double.TryParse(c[23], out var i23) ? i23 : 0;
                                                o.DwellTime_2           = double.TryParse(c[24], out var i24) ? i24 : 0;
                                                o.DwellTime_3           = double.TryParse(c[25], out var i25) ? i25 : 0;
                                                o.DwellTime_4           = double.TryParse(c[26], out var i26) ? i26 : 0;
                                                o.DwellTime_5           = double.TryParse(c[27], out var i27) ? i27 : 0;
                                                o.DwellTime_6           = double.TryParse(c[28], out var i28) ? i28 : 0;
                                                o.DwellAlarm_1          = double.TryParse(c[29], out var i29) ? i29 : 0;
                                                o.DwellAlarm_2          = double.TryParse(c[30], out var i30) ? i30 : 0;
                                                o.DwellAlarm_3          = double.TryParse(c[31], out var i31) ? i31 : 0;
                                                o.DwellAlarm_4          = double.TryParse(c[32], out var i32) ? i32 : 0;
                                                o.DwellAlarm_5          = double.TryParse(c[33], out var i33) ? i33 : 0;
                                                o.DwellAlarm_6          = double.TryParse(c[34], out var i34) ? i34 : 0;
                                                o.CoolingTime           = double.TryParse(c[35], out var i35) ? i35 : 0;
                                                o.CoolingTemperature    = double.TryParse(c[36], out var i36) ? i36 : 0;

                                                return true;
                                            });
    }
}