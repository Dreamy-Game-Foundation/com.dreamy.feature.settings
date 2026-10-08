using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreamy.Feature.Settings.Integration;
using Dreamy.Settings;

internal static class PlatformGatewayHarness
{
    private static void Check(bool value, string description)
    { if (!value) throw new Exception(description); Console.WriteLine("PASS " + description); }
    public static void Run()
    {
        int calls = 0;
        using var cancellation = new CancellationTokenSource();
        var gateway = new SettingsPlatformGateway("https://example.com/store",
            consent: token => {
                Check(token == cancellation.Token, "consent adapter forwards host cancellation token");
                calls++;
                return UniTask.FromResult(SettingsOperationResult.Succeeded("host-consent"));
            });
        Check(gateway.CanOpenStore && gateway.CanShowGdprConsent && !gateway.CanRestorePurchases,
            "platform capabilities reflect supplied URL and SDK callbacks");
        Check(gateway.ShowGdprConsentAsync(cancellation.Token).GetAwaiter().GetResult().Message == "host-consent",
            "consent adapter preserves host result");
        cancellation.Cancel();
        gateway.ShowGdprConsentAsync(cancellation.Token).GetAwaiter().GetResult();
        Check(calls == 1, "cancelled consent never invokes SDK callback");
        Check(!gateway.RestorePurchasesAsync().GetAwaiter().GetResult().IsSuccess,
            "missing restore provider never reports success");
        bool rejected = false;
        try { new SettingsPlatformGateway("javascript:invalid"); }
        catch (ArgumentException) { rejected = true; }
        Check(rejected, "invalid store URL is rejected before launch");
    }
}
