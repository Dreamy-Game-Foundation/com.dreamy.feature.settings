using System;
using Cysharp.Threading.Tasks;

namespace Dreamy.Settings
{
    public sealed class RateUsPresenter : IDisposable
    {
        private const int PositiveRatingThreshold = 4;

        private readonly ISettingsService service;
        private readonly IRateUsView view;
        private bool isBound;
        private int rating;

        public RateUsPresenter(ISettingsService service, IRateUsView view)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public event Action<int> PositiveRatingSubmitted;

        public void Show()
        {
            Bind();
            view.SetRating(rating);
        }

        public void Dispose()
        {
            if (!isBound)
            {
                return;
            }

            view.RatingSelected -= SelectRating;
            view.RateRequested -= SubmitRating;
            view.CloseRequested -= Close;
            isBound = false;
        }

        private void Bind()
        {
            if (isBound)
            {
                return;
            }

            view.RatingSelected += SelectRating;
            view.RateRequested += SubmitRating;
            view.CloseRequested += Close;
            isBound = true;
        }

        private void SelectRating(int selectedRating)
        {
            rating = selectedRating < 1 ? 1 : selectedRating > 5 ? 5 : selectedRating;
            view.SetRating(rating);
        }

        private void SubmitRating()
        {
            if (rating <= 0)
            {
                return;
            }

            if (rating < PositiveRatingThreshold)
            {
                Close();
                return;
            }

            RequestReviewAsync().Forget();
        }

        private async UniTaskVoid RequestReviewAsync()
        {
            view.SetInteractable(false);
            try
            {
                SettingsOperationResult result = await service.RequestReviewAsync();
                if (result.IsSuccess)
                {
                    PositiveRatingSubmitted?.Invoke(rating);
                    Close();
                }
            }
            finally
            {
                view.SetInteractable(true);
            }
        }

        private void Close()
        {
            Dispose();
            view.Close();
        }
    }
}
