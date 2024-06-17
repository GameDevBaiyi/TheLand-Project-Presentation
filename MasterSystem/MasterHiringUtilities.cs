using System;
using System.Collections;

using UnityEngine;

public static class MasterHiringUtilities
{
    public const int RTicketID = 10001;
    public const int SrTicketID = 10002;
    public const int UrTicketID = 10003;
    public const int MrTicketID = 10004;
    public const int LrTicketID = 10005;

    public static string GetFormatedDurationText(TimeSpan duration)
    {
        string text;
        if (duration.TotalSeconds <= 0d)
        {
            text = $"0{StringUtil.GetUIString("Time_Hour")}:0{StringUtil.GetUIString("Time_Min")}";
            return text;
        }
        if (duration >= TimeSpan.FromDays(1d))
        {
            text = $"{duration.Days}{StringUtil.GetUIString("Time_Day")}:{duration.Hours}{StringUtil.GetUIString("Time_Hour")}";
            return text;
        }
        text = $"{duration.Hours}{StringUtil.GetUIString("Time_Hour")}:{duration.Minutes}{StringUtil.GetUIString("Time_Min")}";
        return text;
    }

    public static IEnumerator CountDownDurationAsync(SuPro.MasterInfo masterInfo,UILabel durationText,string prefix = null)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
        while (true)
        {
            TimeSpan duration = masterInfo.expireTimer - NetworkManager.Instance.Now;
            durationText.text = $"{prefix}{MasterHiringUtilities.GetFormatedDurationText(duration)}";
            Color color = duration.TotalDays <= 1d ? Color.red : Color.green;
            durationText.color = color;
            yield return waitForSeconds;
            if (duration.TotalSeconds <= 0d) break;
        }
    }
}