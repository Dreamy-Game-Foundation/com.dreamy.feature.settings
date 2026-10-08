# Dreamy Settings

Runtime services and presenters for Music/SFX volume, haptic preferences, and host-owned consent, purchase restore, and Rate Us actions. Runtime has no UnityEngine/UnityEditor references. The optional Settings Feature sample provides MVP views, presenter factories, and simulated host adapters. It contains no per-feature demo, launcher, or controller classes.

## Installation

Implement your game's platform, haptic, and review gateways, then install at the composition root after audio and save initialization:

```csharp
ServiceLocator.Register<ISettingsPlatformGateway>(platformGateway);
ServiceLocator.Register<ISettingsHapticsGateway>(hapticsGateway);
ServiceLocator.Register<ISettingsReviewGateway>(reviewGateway);
ISettingsService settings = SettingsInstaller.Install();
```

Explicit dependencies are also supported:

```csharp
ISettingsService settings = SettingsInstaller.Install(
    audioService, platformGateway, hapticsGateway, reviewGateway);
```

A platform adapter may implement the optional haptic/review interfaces itself. Without a supported haptic gateway, `CanSetHaptics` is false and requests are ignored. Without a review gateway, opening the store remains available but no rating state or reward is persisted. Production hosts should supply the review gateway when persistence is required.

## Custom UI

Implement `ISettingsView` and `IRateUsView` in the game's UI and pass them to `SettingsPresenter` and `RateUsPresenter`. Call `Show()` when opening the view and `Dispose()` when closing or destroying its owner. The host owns UI creation, layout, transitions, and navigation. Reopening a disposed presenter rebinds it.

Settings views render `SettingsViewState`, including `HapticsEnabled`, `CanSetHaptics`, `CanRequestReview`, `HasPositiveRating`, and `HasClaimedReviewReward`. Emit `HapticsEnabledChanged` from the toggle. The haptic gateway owns persistence and applying the preference to the actual haptic provider; Settings does not produce vibration itself.

The UI needs only one Rate Us action. Custom rating views use `SubmitRatingAsync(int rating, CancellationToken)` for the store/persistence/reward flow. The legacy `RequestReviewAsync` service method opens the listing directly and bypasses rating persistence/rewards. The host `OpenStoreAsync` reports success only when it has successfully launched the listing.

## Review state and optional reward

The Rate Us presenter closes on ratings 1–3. Ratings 4–5 open the store listing and then call `ISettingsReviewGateway.RecordPositiveRating`. Save this as a local positive rating/request, not a verified published review: Google Play [does not report whether a user submitted a review](https://developer.android.com/guide/playcore/in-app-review/unity).

Persist `HasPositiveRating` and `HasClaimedReward` in the game's account-scoped Datasave/backend owner. Once a positive request is saved, the flow avoids reopening the store. `CanRequestReview` suppresses repeat requests while still allowing a pending optional reward to retry. Runtime rejects concurrent submissions, freezes the submitted rating, and passes lifetime cancellation to the host.

Rewards are opt-in through `ISettingsReviewGateway.CanGrantReward` and `GrantRewardOnceAsync`. Keep `CanGrantReward` false by default. The host decides platform eligibility (for an Android-only reward, return false in Editor, iOS, and other players). Persist a fixed transaction ID together with the wallet mutation, and return success for an already processed transaction. Only set the claimed flag after that durable grant; failed grants remain retryable without reopening the store. `PositiveRatingSubmitted` is a notification and must not grant an additional reward.

Google Play [prohibits incentivized reviews/ratings](https://support.google.com/googleplay/android-developer/answer/9898684?hl=en). Keep review-linked rewards disabled in Play builds; any feedback reward must be independent of publishing or selecting a positive store review.

## Settings Feature integration

Production integration for the supplied passive SettingsPanel and RateUsPanel views. At the composition root, after audio/save readiness:

```csharp
var factory = new PanelPresenterFactory();
SettingsFeatureInstaller.Install(factory, audioService, OpenRateUs, platformGateway, hapticsGateway, reviewGateway);
PanelManager.Instance.PresenterFactory = factory;
```

One Install call installs the runtime service and registers both presenters. If the host already owns an ISettingsService, use `SettingsFeatureInstaller.Install(factory, settingsService, OpenRateUs)`. Runtime SettingsInstaller remains available for custom UI; callers using these panels only need SettingsFeatureInstaller.

Open from any caller through `PanelManager.Show<SettingsPanel>(address)` or `Transition<SettingsPanel>(address)`. In the sandbox, GameInstaller owns the combined Shop/Settings factory, attaches it to each scene's manager, and opens Rate Us using `Panel/RateUsPanel.prefab`. FoundationDemoRoot only opens views. Every opening owns one presenter; close/disable/destroy/failure releases it, and cached reopen creates a fresh presenter. Add Dreamy.UI.Presentation to asmdefs using the factory or IPanelPresenter.

Gateway choices are explicit; the installer never creates mock providers:

- SettingsPlatformGateway opens a real HTTP(S) store listing with Application.OpenURL. Pass the store URL (required for iOS; Android can derive it from Application.identifier). Optional consent/restore callbacks call the host SDK. Missing callbacks disable those actions. Launch success is a request to open the URL, not confirmation of a published review.
- PlayerPrefsSettingsHapticsGateway persists the device-local preference and calls an injected real haptic provider. Without an installed haptic gateway, the toggle is unavailable. The constructor applies the saved preference to the provider.
- PlayerPrefsSettingsReviewGateway stores device-local positive rating/request and optional reward claim. Account-scoped games should supply their Datasave/backend implementation. Existing Dreamy.Settings.Sample preference keys are preserved for compatibility.

Review-linked reward remains off by default. Its opt-in example requires Android and an idempotent wallet; keep it disabled for Google Play ([policy](https://support.google.com/googleplay/android-developer/answer/9898684?hl=en)). No gateway claims to verify a published store review.

## Validation

From a Unity project with generated script assemblies and the .NET 10 SDK:

```sh
python3 LocalPackages/com.dreamy.feature.settings/Tests~/validate-settings.py
```

The harness checks that views have no presenter/service/navigation construction, validates prefab references, compiles runtime/sample code against the project's assemblies, and verifies factory/host show/hide/reopen/disposal plus haptic/review behavior. Native store launch and host save/reward adapters require separate Unity/device validation.
