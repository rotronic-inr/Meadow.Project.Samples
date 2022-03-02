﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Hid;
using Meadow.Hardware;
using Meadow.Units;

namespace TouchKeypad
{
    // public class MeadowApp : App<F7Micro, MeadowApp> <- If you have a Meadow F7v1.*
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        Mpr121 sensor;
        MicroGraphics graphics;

        public MeadowApp()
        {
            Initialize();
        }

        void Initialize() 
        {
            var onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            var config = new SpiClockConfiguration(
                speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
                clock: Device.Pins.SCK,
                copi: Device.Pins.MOSI,
                cipo: Device.Pins.MISO,
                config: config);
            var display = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.D02,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240, height: 240);

            graphics = new MicroGraphics(display);
            graphics.Rotation = RotationType._180Degrees;
            graphics.CurrentFont = new Font12x16();

            sensor = new Mpr121(Device.CreateI2cBus(I2cBusSpeed.Standard), 90, 100);
            sensor.ChannelStatusesChanged += SensorChannelStatusesChanged;

            onboardLed.SetColor(Color.Green);
        }

        void SensorChannelStatusesChanged(object sender, ChannelStatusChangedEventArgs e)
        {
            graphics.Clear();
            graphics.Stroke = 1;

            for (int i = 0; i < e.ChannelStatus.Count; i++)
            {
                int numpadIndex = 0;
                for (int columns = 0; columns < 3; columns++)
                {
                    for (int rows = 3; rows >= 0; rows--)
                    {
                        if (numpadIndex == i)
                        {
                            if (e.ChannelStatus[(Mpr121.Channels)i])
                                graphics.DrawRectangle(columns * 57 + 38, rows * 57 + 10, 51, 51, Meadow.Foundation.Color.Cyan, true);
                            else
                                graphics.DrawRectangle(columns * 57 + 38, rows * 57 + 10, 51, 51, true);
                        }
                        numpadIndex++;
                    }
                }
            }

            graphics.Show();
        }
    }
}