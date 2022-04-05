using System;
using BatteryGauge.Base;
using BatteryGauge.Battery;
using BatteryGauge.Util;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;

namespace BatteryGauge.UI; 

// This borrowed heavily from https://github.com/karashiiro/PingPlugin/blob/main/PingPlugin/PingUI.cs

public class BatteryDtrBar : IDisposable {
    private const string DtrBarTitle = "BatteryGauge";
    private readonly DtrBarEntry? _barEntry;
    
    public BatteryDtrBar() {
        try {
            this._barEntry = Injections.DtrBar.Get(DtrBarTitle);
        } catch (ArgumentException e) {
            // This usually only runs once after any given plugin reload
            for (var i = 0; i < 5; i++) {
                PluginLog.LogError(e, $"Failed to acquire DtrBarEntry {DtrBarTitle}, trying {DtrBarTitle}{i}");
                try {
                    this._barEntry = Injections.DtrBar.Get(DtrBarTitle + i);
                } catch (ArgumentException) {
                    continue;
                }

                break;
            }
        }

        if (this._barEntry != null) {
            this._barEntry.Text = "Measuring...";
            
            if (!SystemPower.HasBattery) {
                this._barEntry.Shown = false;
            }
        }
    }

    public void UpdateBarEntry() {
        if (this._barEntry is not { Shown: true }) return;

        if (!SystemPower.HasBattery) {
            // this should never realistically be possible
            this._barEntry.Text = "Battery Error";
            return;
        }

        if (SystemPower.IsCharging) {
            this._barEntry.Text = $"Charging ({SystemPower.ChargePercentage}%)";
            return;
        }

        this._barEntry.Text = $"{SystemPower.ChargePercentage}% ({TimeUtil.GetPrettyTimeFormat(SystemPower.LifetimeSeconds)})";
    }
    
    public void Dispose() {
        this._barEntry?.Dispose();
        
        GC.SuppressFinalize(this);
    }
}