﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Hardware;
using Meadow.Units;
using System;

namespace TemperatureMonitor
{
    // public class MeadowApp : App<F7Micro, MeadowApp> <- If you have a Meadow F7v1.*
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        Color[] colors = new Color[4] 
        { 
            Color.FromHex("#008500"), 
            Color.FromHex("#269926"), 
            Color.FromHex("#00CC00"), 
            Color.FromHex("#67E667") 
        };

        MicroGraphics graphics;
        AnalogTemperature analogTemperature;               

        public MeadowApp()
        {
            Initialize();

            LoadScreen();
            analogTemperature.StartUpdating(TimeSpan.FromSeconds(5));
        }

        void Initialize() 
        {
            var onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            analogTemperature = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );
            analogTemperature.TemperatureUpdated += AnalogTemperatureTemperatureUpdated; //+= AnalogTemperatureUpdated;

            var config = new SpiClockConfiguration(
                speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
                clock: Device.Pins.SCK,
                copi: Device.Pins.MOSI,
                cipo: Device.Pins.MISO,
                config: config);
            var st7789 = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.D02,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240, height: 240);

            graphics = new MicroGraphics(st7789);
            graphics.Rotation = RotationType._270Degrees;

            onboardLed.SetColor(Color.Green);
        }

        void AnalogTemperatureTemperatureUpdated(object sender, IChangeResult<Meadow.Units.Temperature> e)
        {
            graphics.DrawRectangle(
                x: 48,
                y: 160,
                width: 144,
                height: 40,
                color: colors[colors.Length - 1],
                filled: true);

            graphics.DrawText(
                x: 48, y: 160,
                text: $"{e.New.Celsius:00.0}°C",
                color: Color.White,
                scaleFactor: ScaleFactor.X2);

            graphics.Show();
        }

        void LoadScreen()
        {
            Console.WriteLine("LoadScreen...");

            graphics.Clear();

            int radius = 225;
            int originX = graphics.Width / 2;
            int originY = graphics.Height / 2 + 130;

            graphics.Stroke = 3;
            for (int i = 1; i < 5; i++)
            {
                graphics.DrawCircle
                (
                    centerX: originX,
                    centerY: originY,
                    radius: radius,
                    color: colors[i - 1],
                    filled: true
                );

                graphics.Show();
                radius -= 20;
            }

            graphics.DrawLine(0, 220, 240, 220, Color.White);
            graphics.DrawLine(0, 230, 240, 230, Color.White);

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText(54, 130, "TEMPERATURE", Color.White);

            graphics.Show();
        }
    }
}