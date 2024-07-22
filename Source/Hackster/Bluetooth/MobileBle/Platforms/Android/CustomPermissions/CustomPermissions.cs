using Android;

namespace MobileBle.Platforms.Android.CustomPermissions;

public class BluetoothScanPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            (Manifest.Permission.BluetoothScan, true)
        }.ToArray();
}

public class BluetoothConnectPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            (Manifest.Permission.BluetoothConnect, true)
        }.ToArray();
}