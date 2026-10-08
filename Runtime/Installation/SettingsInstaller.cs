using System;
using Dreamy.Audio;
using Dreamy.Core;

namespace Dreamy.Settings
{
    public static class SettingsInstaller
    {
        public static ISettingsService Install()
        {
            ISettingsPlatformGateway platformGateway = ServiceLocator.TryGet<ISettingsPlatformGateway>(
                out ISettingsPlatformGateway resolvedGateway)
                ? resolvedGateway
                : null;
            ISettingsHapticsGateway hapticsGateway = ServiceLocator.TryGet<ISettingsHapticsGateway>(
                out ISettingsHapticsGateway resolvedHapticsGateway)
                ? resolvedHapticsGateway
                : null;
            ServiceLocator.TryGet<ISettingsReviewGateway>(out ISettingsReviewGateway reviewGateway);
            return Install(ServiceLocator.Get<IAudioService>(), platformGateway, hapticsGateway, reviewGateway);
        }

        public static ISettingsService Install(
            IAudioService audioService,
            ISettingsPlatformGateway platformGateway = null,
            ISettingsHapticsGateway hapticsGateway = null,
            ISettingsReviewGateway reviewGateway = null)
        {
            if (audioService == null)
            {
                throw new ArgumentNullException(nameof(audioService));
            }

            SettingsModel service = new(audioService, platformGateway, hapticsGateway, reviewGateway);
            ServiceLocator.Register<ISettingsService>(service);
            return service;
        }
    }
}
