using System;

namespace Dreamy.Settings
{
    public interface ISettingsView
    {
        event Action<float> MusicVolumeChanged;
        event Action<float> SfxVolumeChanged;
        event Action GdprRequested;
        event Action RestorePurchasesRequested;
        event Action OpenStoreRequested;
        event Action CloseRequested;

        void Render(SettingsViewState state);
        void SetPlatformActionsInteractable(bool interactable);
        void ShowOperationResult(SettingsOperationResult result);
        void Close();
    }
}
