using System;
using Dreamy.Settings;
using Dreamy.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamy.Feature.Settings.Integration
{
    public sealed class SettingsPanel : UIPanel, ISettingsView
    {
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Button gdprButton;
        [SerializeField] private Button restorePurchasesButton;
        [SerializeField] private Button openStoreButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text statusText;

        public override bool CanBack => true;

        public event Action<float> MusicVolumeChanged;
        public event Action<float> SfxVolumeChanged;
        public event Action GdprRequested;
        public event Action RestorePurchasesRequested;
        public event Action OpenStoreRequested;
        public event Action CloseRequested;

        private void OnEnable()
        {
            musicVolumeSlider.onValueChanged.AddListener(RequestMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(RequestSfxVolume);
            gdprButton.onClick.AddListener(RequestGdpr);
            restorePurchasesButton.onClick.AddListener(RequestRestorePurchases);
            openStoreButton.onClick.AddListener(RequestOpenStore);
            closeButton.onClick.AddListener(RequestClose);
        }

        protected override void OnDisable()
        {
            musicVolumeSlider.onValueChanged.RemoveListener(RequestMusicVolume);
            sfxVolumeSlider.onValueChanged.RemoveListener(RequestSfxVolume);
            gdprButton.onClick.RemoveListener(RequestGdpr);
            restorePurchasesButton.onClick.RemoveListener(RequestRestorePurchases);
            openStoreButton.onClick.RemoveListener(RequestOpenStore);
            closeButton.onClick.RemoveListener(RequestClose);
            base.OnDisable();
        }

        public void Render(SettingsViewState state)
        {
            musicVolumeSlider.SetValueWithoutNotify(state.MusicVolume);
            sfxVolumeSlider.SetValueWithoutNotify(state.SfxVolume);
            gdprButton.gameObject.SetActive(state.CanShowGdprConsent);
            restorePurchasesButton.gameObject.SetActive(state.CanRestorePurchases);
            openStoreButton.gameObject.SetActive(state.CanOpenStore);
        }

        public void SetPlatformActionsInteractable(bool interactable)
        {
            gdprButton.interactable = interactable;
            restorePurchasesButton.interactable = interactable;
            openStoreButton.interactable = interactable;
        }

        public void ShowOperationResult(SettingsOperationResult result) =>
            statusText.text = string.IsNullOrWhiteSpace(result.Message) ? result.Status.ToString() : result.Message;

        public void Close() => Hide();

        private void RequestMusicVolume(float volume) => MusicVolumeChanged?.Invoke(volume);
        private void RequestSfxVolume(float volume) => SfxVolumeChanged?.Invoke(volume);
        private void RequestGdpr() => GdprRequested?.Invoke();
        private void RequestRestorePurchases() => RestorePurchasesRequested?.Invoke();
        private void RequestOpenStore() => OpenStoreRequested?.Invoke();
        private void RequestClose() => CloseRequested?.Invoke();
    }
}
