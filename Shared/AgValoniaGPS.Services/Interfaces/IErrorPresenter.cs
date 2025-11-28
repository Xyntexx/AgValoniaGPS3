using System;

namespace AgValoniaGPS.Services.Interfaces
{
    public interface IErrorPresenter
    {
        void PresentTimedMessage(
            TimeSpan timeSpan,
            string titleString,
            string messageString);
    }
}
