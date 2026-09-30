# Settings Feature

Import this sample and move the entire folder into the game. `SettingsPanel.prefab` and `RateUsPanel.prefab` are variants of `BaseFeaturePanel`; keep `com.dreamy.feature` installed.

The prefabs contain the UI and launcher components. Register Audio and install Settings at bootstrap; the launcher waits for `ISettingsService` before showing the panel. Check their serialized TMP labels, music/SFX toggles, GDPR, Restore Purchases, Open Store, and Close buttons after import. The optional simulated gateway is for a standalone demo only and must be registered explicitly.

For production, register the real host platform gateway before installing Settings and creating the panel:

```csharp
ServiceLocator.Register<ISettingsPlatformGateway>(platformGateway);
SettingsInstaller.Install();
```

`SimulatedSettingsPlatformGateway` is only for the sample. A production gateway maps its methods to `DreamySDK.Consent_ShowForm`, `DreamySDK.IAP_RestorePurchases`, and the game store-opening API.
