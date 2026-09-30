using System;
using Cysharp.Threading.Tasks;
using Dreamy.Core;
using Dreamy.Settings;
using UnityEngine;

namespace Dreamy.Feature.Settings.Integration
{
    [RequireComponent(typeof(RateUsPanel))]
    public sealed class RateUsPanelLauncher : MonoBehaviour
    {
        private RateUsPanel panel;
        private RateUsPresenter presenter;

        private void Awake()
        {
            panel = GetComponent<RateUsPanel>();
            ShowAsync().Forget();
        }

        private async UniTaskVoid ShowAsync()
        {
            try
            {
                await UniTask.WaitUntil(
                    () => ServiceLocator.IsRegistered<ISettingsService>(),
                    cancellationToken: this.GetCancellationTokenOnDestroy());
                presenter = new RateUsPresenter(ServiceLocator.Get<ISettingsService>(), panel);
                await panel.Init();
                await panel.PostInit();
                presenter.Show();
                await panel.Show();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void OnEnable() => presenter?.Show();

        private void OnDestroy()
        {
            presenter?.Dispose();
        }
    }
}
