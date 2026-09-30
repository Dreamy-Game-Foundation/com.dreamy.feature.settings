using System;
using Cysharp.Threading.Tasks;

namespace Dreamy.Settings
{
    public sealed class SettingsPresenter : IDisposable
    {
        private readonly ISettingsService service;
        private readonly ISettingsView view;
        private bool isBound;

        public SettingsPresenter(ISettingsService service, ISettingsView view)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public void Show()
        {
            Bind();
            Refresh();
        }

        public void Dispose()
        {
            if (!isBound)
            {
                return;
            }

            view.MusicVolumeChanged -= SetMusicVolume;
            view.SfxVolumeChanged -= SetSfxVolume;
            view.GdprRequested -= ShowGdprConsent;
            view.RestorePurchasesRequested -= RestorePurchases;
            view.OpenStoreRequested -= OpenStore;
            view.CloseRequested -= Close;
            isBound = false;
        }

        private void Bind()
        {
            if (isBound)
            {
                return;
            }

            view.MusicVolumeChanged += SetMusicVolume;
            view.SfxVolumeChanged += SetSfxVolume;
            view.GdprRequested += ShowGdprConsent;
            view.RestorePurchasesRequested += RestorePurchases;
            view.OpenStoreRequested += OpenStore;
            view.CloseRequested += Close;
            isBound = true;
        }

        private void Refresh() => view.Render(service.GetState());

        private void SetMusicVolume(float volume)
        {
            service.SetMusicVolume(volume);
            Refresh();
        }

        private void SetSfxVolume(float volume)
        {
            service.SetSfxVolume(volume);
            Refresh();
        }

        private void ShowGdprConsent() => RunOperation(service.ShowGdprConsentAsync()).Forget();
        private void RestorePurchases() => RunOperation(service.RestorePurchasesAsync()).Forget();
        private void OpenStore() => RunOperation(service.OpenStoreAsync()).Forget();

        private async UniTaskVoid RunOperation(UniTask<SettingsOperationResult> operation)
        {
            view.SetPlatformActionsInteractable(false);
            try
            {
                SettingsOperationResult result = await operation;
                view.ShowOperationResult(result);
                Refresh();
            }
            finally
            {
                view.SetPlatformActionsInteractable(true);
            }
        }

        private void Close()
        {
            Dispose();
            view.Close();
        }
    }
}
