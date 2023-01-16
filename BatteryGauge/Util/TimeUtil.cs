using System;

namespace BatteryGauge.Util; 

public static class TimeUtil {
    public static string GetPrettyTimeFormat(int seconds) {
        var time = TimeSpan.FromSeconds(seconds);
        return $"{time.Hours}h{time.Minutes:00}m";
    }
}