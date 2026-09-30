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
            return Install(ServiceLocator.Get<IAudioService>(), platformGateway);
        }

        public static ISettingsService Install(
            IAudioService audioService,
            ISettingsPlatformGateway platformGateway = null)
        {
            if (audioService == null)
            {
                throw new ArgumentNullException(nameof(audioService));
            }

            SettingsModel service = new(audioService, platformGateway);
            ServiceLocator.Register<ISettingsService>(service);
            return service;
        }
    }
}
