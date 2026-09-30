using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreamy.Audio;

namespace Dreamy.Settings
{
    public sealed class SettingsModel : ISettingsService
    {
        private readonly IAudioService audioService;
        private readonly ISettingsPlatformGateway platformGateway;

        public SettingsModel(IAudioService audioService, ISettingsPlatformGateway platformGateway = null)
        {
            this.audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            this.platformGateway = platformGateway;
        }

        public SettingsViewState GetState() => new(
            audioService.GetVolume(AudioBusId.Music),
            audioService.GetVolume(AudioBusId.Sfx),
            platformGateway?.CanShowGdprConsent ?? false,
            platformGateway?.CanRestorePurchases ?? false,
            platformGateway?.CanOpenStore ?? false);

        public void SetMusicVolume(float volume) =>
            audioService.SetVolume(AudioBusId.Music, Clamp01(volume));

        public void SetSfxVolume(float volume) =>
            audioService.SetVolume(AudioBusId.Sfx, Clamp01(volume));

        public UniTask<SettingsOperationResult> ShowGdprConsentAsync(CancellationToken cancellationToken = default) =>
            platformGateway?.CanShowGdprConsent == true
                ? platformGateway.ShowGdprConsentAsync(cancellationToken)
                : UniTask.FromResult(SettingsOperationResult.Unavailable());

        public UniTask<SettingsOperationResult> RestorePurchasesAsync(CancellationToken cancellationToken = default) =>
            platformGateway?.CanRestorePurchases == true
                ? platformGateway.RestorePurchasesAsync(cancellationToken)
                : UniTask.FromResult(SettingsOperationResult.Unavailable());

        public UniTask<SettingsOperationResult> OpenStoreAsync(CancellationToken cancellationToken = default) =>
            platformGateway?.CanOpenStore == true
                ? platformGateway.OpenStoreAsync(cancellationToken)
                : UniTask.FromResult(SettingsOperationResult.Unavailable());

        public UniTask<SettingsOperationResult> RequestReviewAsync(CancellationToken cancellationToken = default) =>
            platformGateway?.CanRequestReview == true
                ? platformGateway.RequestReviewAsync(cancellationToken)
                : UniTask.FromResult(SettingsOperationResult.Unavailable());

        private static float Clamp01(float value) => value < 0f ? 0f : value > 1f ? 1f : value;
    }
}
