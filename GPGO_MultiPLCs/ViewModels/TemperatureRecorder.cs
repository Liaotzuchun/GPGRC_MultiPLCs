using System;
using GPGRC_MultiPLCs.Models;

namespace GPGRC_MultiPLCs.ViewModels;

public class TemperatureRecorder
{
    private readonly BaseInfoWithChart _ovenInfo;

    public TemperatureRecorder(BaseInfoWithChart ovenInfo)
    {
        _ovenInfo = ovenInfo;
    }

    public void AddTemperatures(bool keypoint, DateTime addtime, double t0, double t1)
    {
        var record = new RecordTemperatures
        {
            KeyPoint = keypoint,
            AddedTime = addtime,
            OvenTemperatures_1 = t0,
            OvenTemperatures_2 = t1,
        };

        _ovenInfo.ChartModel.AddData(record, 0);
    }
    public void AddTemperatures(bool keypoint, DateTime addtime, double t0, double t1, double t2, double t3, double t4, double t5, double t6, double t7)
    {
        var record = new RecordTemperatures
        {
            KeyPoint = keypoint,
            AddedTime = addtime,
            OvenTemperatures_1 = t0,
            OvenTemperatures_2 = t1,
            OvenTemperatures_3 = t2,
            OvenTemperatures_4 = t3,
            OvenTemperatures_5 = t4,
            OvenTemperatures_6 = t5,
            OvenTemperatures_7 = t6,
            OvenTemperatures_8 = t7,
        };

        _ovenInfo.ChartModel.AddData(record, 1);
    }
}