# Dreamy Settings

Package thuộc Dreamy Game Studio. Hướng dẫn dưới đây mô tả cấu trúc, cách cài vào project và tích hợp ở root/scene.

## Cài package

Dùng Unity 6000.0 trở lên. Sandbox đã tham chiếu package bằng `file:../LocalPackages/com.dreamy.feature.settings`. Project khác dùng Package Manager > + > Install package from disk và chọn package.json, hoặc Git URL của repository nội bộ. Cài cả dependency Dreamy/Git vào manifest của game; version dependency không tự cấu hình registry riêng.

Dependency trực tiếp theo package.json:

- `com.dreamy.core` (1.1.2)
- `com.dreamy.audio` (0.1.0)
- `com.dreamy.feature` (0.1.0)
- `com.dreamy.ui` (0.2.0)
- `com.cysharp.unitask` (2.5.10)

## Cấu trúc và asmdef

| Assembly | Reference | Phạm vi |
| --- | --- | --- |
| `Dreamy.Settings.Runtime` | Dreamy.Core.Runtime, Dreamy.Audio.Runtime, UniTask | Runtime |

Trong asmdef của game, thêm assembly chứa API trực tiếp sử dụng. Code bootstrap reference thêm Core/DataConfig/Datasave/Economy theo nhu cầu; code async reference UniTask. Code gọi type sample reference assembly sample. Giữ Editor reference trong asmdef Editor-only.

## Cấu trúc và cài ở root

Runtime/Contracts chứa service/view/platform gateway; Domain chứa SettingsModel; Presentation chứa presenter; Installation chứa SettingsInstaller. Sample chứa SettingsPanel/RateUsPanel và launcher. Runtime dùng audio service, không gọi trực tiếp SDK store/consent.

```csharp
using Dreamy.Audio;
using Dreamy.Core;
using Dreamy.Settings;

// Trong GameInstaller, audioProfile và platformGateway do game cung cấp.
DreamyAudio.Initialize(audioProfile);
ServiceLocator.Register<IAudioService>(DreamyAudio.Service);
ServiceLocator.Register<ISettingsPlatformGateway>(platformGateway);
SettingsInstaller.Install();
```

Cài audio trước Settings, cài gateway thật trước khi tạo panel. Gateway nối consent, restore purchase và mở store với SDK của game. Root unregister ISettingsService và gateway khi teardown theo lifecycle sở hữu; không tạo lại audio service trong panel.

## Dùng sample và panel

Import Settings Feature; gán toggle Music/SFX, button GDPR/Restore/Open Store/Rate Us/Close. SettingsPanelLauncher reference RateUsPanel prefab và tự initialize/show ở Awake. Launcher dùng gateway mô phỏng khi chưa có settings service; vì vậy bootstrap phải cài service thật trước.

Chọn luồng instantiate prefab với launcher, hoặc chỉnh integration để PanelManager sở hữu init/show. Không cho cả hai luồng cùng chạy. Khi đổi sang Addressables, khai báo Panel/SettingsPanel.prefab và Panel/RateUsPanel.prefab; launcher hiện instantiate RateUs từ field, không tự tra address chỉ vì prefab được thêm vào group.

Sample asmdef có reference Economy; cài package này khi dùng sample. Presenter thuộc integration của game, dispose khi panel bị hủy và bind lại khi mở panel cache.
## Import sample

Mở Window > Package Manager, chọn Dreamy Settings > Samples > Import. Unity chép vào Assets/Samples/Dreamy Settings/0.2.0/. Chuyển cả folder nếu tùy biến, giữ .meta và reference prefab; không giữ bản script/asmdef hoặc Resources document trùng.

- **Settings Feature**: nguồn `Samples~/Settings Feature`.
  Assembly `Dreamy.Feature.Settings.Integration.Runtime` reference Dreamy.Settings.Runtime, Dreamy.Core.Runtime, Dreamy.Economy.Runtime, Dreamy.UI.Runtime, Unity.TextMeshPro, UnityEngine.UI, UniTask.

## Addressables Group và class address

1. Lưu prefab/variant của game tại Assets/_Project/Prefabs/Panel/SettingsPanel.prefab. Với UIPanel, root phải có subclass tương ứng.
2. Mở Window > Asset Management > Addressables > Groups; tạo settings nếu chưa có.
3. Tạo group UI Panels và kéo prefab vào group.
4. Đặt cột Address thành Panel/SettingsPanel.prefab.
5. Tạo class dùng chung trong game:

```csharp
public static class PanelAddress
{
    public const string Home = "Panel/HomePanel.prefab";
    public const string Current = "Panel/SettingsPanel.prefab";
}
```

Đường dẫn asset trên disk và address là hai giá trị riêng. Address do bạn đặt, constant phải khớp chính xác cột Address. Tên group không phải key tải. HomePanel là ví dụ subclass do game tự tạo.

Launcher sample tự init/show trong Awake. Nếu giữ launcher, load prefab bằng AssetLoader rồi instantiate dưới Canvas; nếu chuyển sang PanelManager.Create, chỉnh launcher để manager quản lý lifecycle. Cài service thật trước cả hai luồng. RateUs launcher reference prefab trực tiếp; muốn tra address phải sửa integration của game.

Build Addressables content cho target trước khi thử player. AssetLoader cache prefab; đóng panel không tự unload cache. Chỉ unload sau khi mọi instance/consumer đã kết thúc.
