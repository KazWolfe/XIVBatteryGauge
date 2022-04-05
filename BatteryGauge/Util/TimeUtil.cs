using System;

namespace BatteryGauge.Util; 

public static class TimeUtil {
    public static string GetPrettyTimeFormat(int seconds) {
        var time = TimeSpan.FromSeconds(seconds);

        // just some pretty formatting shenanigans
        if (time.Hours == 0) {
            return $"{time.Hours}h{time.Minutes}m";
        }
        
        return $"{time.Hours}h{time.Minutes:00}m";
    }
}