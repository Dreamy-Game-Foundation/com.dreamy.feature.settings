# Dreamy Settings

Reusable MVP settings runtime for Music/SFX volume and host-owned GDPR, restore purchase, and store actions.

Runtime has no `MonoBehaviour`, prefab, or DreamySDK dependency. It uses `IAudioService` for audio bus volumes and `ISettingsPlatformGateway` for platform operations.

```csharp
ServiceLocator.Register<ISettingsPlatformGateway>(platformGateway);
SettingsInstaller.Install();
```

Import **Settings Feature** to get the replaceable TMP/UI panel implementation.
