using System;
using System.Threading;
using System.Threading.Tasks;
using BatteryGauge.Base;
using BatteryGauge.Battery;
using BatteryGauge.Util;
using Dalamud.Game.Gui.Dtr;

namespace BatteryGauge.UI;

public class BatteryDtrBar : IDisposable {
    private const int BatteryPollMillis = 250;
    private const string DtrBarTitle = "BatteryGauge";
    
    private readonly DtrBarEntry? _barEntry;
    private readonly CancellationTokenSource _ts = new();
    private readonly CancellationToken _token;
    private readonly Task _watcherTask;

    private static PluginConfig _pluginConfig = BatteryGauge.Instance.Configuration;

    public BatteryDtrBar() {
        this._barEntry = Injections.DtrBar.Get(DtrBarTitle);

        if (this._barEntry != null) {
            this._barEntry.Text = "Measuring...";

            // disable the bar if there's no battery - nothing to display.
            if (!SystemPower.HasBattery) {
                this._barEntry.Shown = false;
            }
        }

        this._token = this._ts.Token;
        
        this._watcherTask = Task.Run(async () => {
            while (!this._token.IsCancellationRequested) {
                this.UpdateBarEntry();

                await Task.Delay(BatteryPollMillis, this._token);
            }
        }, this._token);
    }

    private void UpdateBarEntry() {
        if (this._barEntry == null) return;
        
        if (!this._barEntry.Shown) {
            // Handle an edge case where a battery is added *after* the game initializes.
            // I don't know why this would realistically happen, but it came up in UPS testing. 
            if (SystemPower.HasBattery) {
                this._barEntry.Shown = true;
            } else {
                return;
            }
        }
        
        // Handle an edge case where the battery *did* exist at initialization but no longer exists somehow.
        // Again, came up in UPS testing. Also probably if the battery catastrophically fails.
        if (!SystemPower.HasBattery) {
            this._barEntry.Text = "Battery Error";
            return;
        }

        if (SystemPower.IsCharging) {
            if (SystemPower.ChargePercentage == 100 && _pluginConfig.HideWhenFull) {
                this._barEntry.Text = "";
                return;
            }
            
            this._barEntry.Text = _pluginConfig.ChargingDisplayMode switch {
                ChargingDisplayMode.Hide => "",
                ChargingDisplayMode.PercentageOnly => $"{SystemPower.ChargePercentage}%",
                ChargingDisplayMode.TextOnly => "Charging",
                ChargingDisplayMode.TextPercentage => $"Charging ({SystemPower.ChargePercentage}%)",
                _ => throw new ArgumentOutOfRangeException()
            };
        } else {
            var lifetime = TimeUtil.GetPrettyTimeFormat(SystemPower.LifetimeSeconds);

            this._barEntry.Text = _pluginConfig.DischargingDisplayMode switch {
                DischargingDisplayMode.Hide => "",
                DischargingDisplayMode.PercentageOnly => $"{SystemPower.ChargePercentage}%",
                DischargingDisplayMode.RuntimeOnly => lifetime,
                DischargingDisplayMode.PercentageRuntime => $"{SystemPower.ChargePercentage}% ({lifetime})",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public void Dispose() {
        this._ts.Cancel();
        this._watcherTask.Dispose();
        this._barEntry?.Dispose();

        GC.SuppressFinalize(this);
    }
}