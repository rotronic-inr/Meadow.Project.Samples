using Meadow.Foundation;
using Meadow.Foundation.Leds;
using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Peripherals.Leds;

namespace MeadowBleLed.Controller
{
    public class LedController
    {
        private static readonly Lazy<LedController> instance =
            new Lazy<LedController>(() => new LedController());
        public static LedController Instance => instance.Value;

        RgbPwmLed rgbPwmLed;

        Task animationTask = null;
        CancellationTokenSource cancellationTokenSource = null;

        private LedController()
        {
            Initialize();
        }

        private void Initialize()
        {
            rgbPwmLed = new RgbPwmLed(
                redPwmPin: MeadowApp.Device.Pins.OnboardLedRed,
                greenPwmPin: MeadowApp.Device.Pins.OnboardLedGreen,
                bluePwmPin: MeadowApp.Device.Pins.OnboardLedBlue,
                CommonType.CommonAnode);
        }

        void Stop()
        {
            rgbPwmLed.StopAnimation();
            cancellationTokenSource?.Cancel();
        }

        public void SetColor(Color color)
        {
            Stop();
            rgbPwmLed.SetColor(color);
        }

        public void TurnOn()
        {
            Stop();
            rgbPwmLed.SetColor(GetRandomColor());
            rgbPwmLed.IsOn = true;
        }

        public void TurnOff()
        {
            Stop();
            rgbPwmLed.IsOn = false;
        }

        public void StartBlink()
        {
            rgbPwmLed.StartBlink(GetRandomColor());
        }

        public void StartPulse()
        {
            rgbPwmLed.StartPulse(GetRandomColor());
        }

        public void StartRunningColors()
        {
            rgbPwmLed.StopAnimation();

            animationTask = new Task(async () =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                await StartRunningColors(cancellationTokenSource.Token);
            });
            animationTask.Start();
        }

        protected async Task StartRunningColors(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                rgbPwmLed.SetColor(GetRandomColor());
                await Task.Delay(1000);
            }
        }

        protected Color GetRandomColor()
        {
            var random = new Random();
            var randomBytes = new byte[3];
            random.NextBytes(randomBytes);
            var randomColor = Color.FromRgb(randomBytes[0], randomBytes[1], randomBytes[2]);
            return randomColor;
        }
    }
}