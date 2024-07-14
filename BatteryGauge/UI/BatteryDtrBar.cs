using System;
using System.Threading;
using System.Threading.Tasks;
using BatteryGauge.Base;
using BatteryGauge.Battery;
using BatteryGauge.Util;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Plugin.Services;

namespace BatteryGauge.UI;

public class BatteryDtrBar : IDisposable {
    private const int BatteryPollMillis = 5_000;
    private const string DtrBarTitle = "BatteryGauge";

    private readonly IDtrBarEntry? _barEntry;
    private readonly CancellationTokenSource _ts = new();

    private readonly PluginConfig _pluginConfig;

    public BatteryDtrBar(PluginConfig config, IDtrBar dtrBar) {
        this._pluginConfig = config;

        this._barEntry = dtrBar.Get(DtrBarTitle);

        if (this._barEntry != null) {
            this._barEntry.Text = "Measuring...";

            // disable the bar if there's no battery - nothing to display.
            if (!SystemPower.HasBattery) {
                this._barEntry.Shown = false;
            }
        }

        Task.Run(async () => {
            while (!this._ts.Token.IsCancellationRequested) {
                this.UpdateBarEntry();

                await Task.Delay(BatteryPollMillis, this._ts.Token);
            }
        }, this._ts.Token);
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
            this._barEntry.Tooltip = "A battery was not detected, or it has failed.";
            return;
        }

        if (SystemPower.IsCharging) {
            if (SystemPower.ChargePercentage == 100 && this._pluginConfig.HideWhenFull) {
                this._barEntry.Text = "";
                this._barEntry.Tooltip = "Battery fully charged.";
                return;
            }

            this._barEntry.Text = this._pluginConfig.ChargingDisplayMode switch {
                ChargingDisplayMode.Hide => "",
                ChargingDisplayMode.PercentageOnly => $"{SystemPower.ChargePercentage}%",
                ChargingDisplayMode.TextOnly => "Charging",
                ChargingDisplayMode.TextPercentage => $"Charging ({SystemPower.ChargePercentage}%)",
                _ => throw new ArgumentOutOfRangeException()
            };

            this._barEntry.Tooltip = $"Battery is charging.\nCurrent percentage: {SystemPower.ChargePercentage}%";
        } else {
            var lifetime = TimeUtil.GetPrettyTimeFormat(SystemPower.LifetimeSeconds);

            this._barEntry.Text = this._pluginConfig.DischargingDisplayMode switch {
                DischargingDisplayMode.Hide => "",
                DischargingDisplayMode.PercentageOnly => $"{SystemPower.ChargePercentage}%",
                DischargingDisplayMode.RuntimeOnly => lifetime,
                DischargingDisplayMode.PercentageRuntime => $"{SystemPower.ChargePercentage}% ({lifetime})",
                _ => throw new ArgumentOutOfRangeException()
            };

            this._barEntry.Tooltip = $"Battery is discharging.\n" +
                                     $"Current percentage: {SystemPower.ChargePercentage}%\n" +
                                     $"Remaining life: {lifetime}";
        }
    }

    public void Dispose() {
        this._ts.Cancel();
        this._barEntry?.Remove();

        GC.SuppressFinalize(this);
    }
}