namespace Dreamy.Settings
{
    public readonly struct SettingsViewState
    {
        public SettingsViewState(
            float musicVolume,
            float sfxVolume,
            bool canShowGdprConsent,
            bool canRestorePurchases,
            bool canOpenStore)
        {
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
            CanShowGdprConsent = canShowGdprConsent;
            CanRestorePurchases = canRestorePurchases;
            CanOpenStore = canOpenStore;
        }

        public float MusicVolume { get; }
        public float SfxVolume { get; }
        public bool CanShowGdprConsent { get; }
        public bool CanRestorePurchases { get; }
        public bool CanOpenStore { get; }
    }
}
