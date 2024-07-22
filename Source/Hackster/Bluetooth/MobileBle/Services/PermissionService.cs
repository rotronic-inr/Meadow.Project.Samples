using MobileBle.Platforms.Android.CustomPermissions;

namespace MobileBle.Services;

public class PermissionService
{
    public async Task<bool> RequestLocationAlwaysAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationAlways>();
        }

        if (status != PermissionStatus.Granted)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> RequestMissingPermissionsAsync()
    {
#if ANDROID
        var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (locationStatus != PermissionStatus.Granted)
        {
            locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (locationStatus != PermissionStatus.Granted)
        {
            // Notify user permission was denied
            return false;
        }

        if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major >= 12)
        {
            var bluetoothScanStatus = await Permissions.CheckStatusAsync<BluetoothScanPermission>();
            var bluetoothConnectStatus = await Permissions.CheckStatusAsync<BluetoothConnectPermission>();

            if (bluetoothScanStatus != PermissionStatus.Granted)
            {
                bluetoothScanStatus = await Permissions.RequestAsync<BluetoothScanPermission>();
            }

            if (bluetoothConnectStatus != PermissionStatus.Granted)
            {
                bluetoothConnectStatus = await Permissions.RequestAsync<BluetoothConnectPermission>();
            }

            if (bluetoothScanStatus != PermissionStatus.Granted || bluetoothConnectStatus != PermissionStatus.Granted)
            {
                // Notify user that Bluetooth permissions were denied
                return false;
            }
        }
#endif
        return true;
    }

}