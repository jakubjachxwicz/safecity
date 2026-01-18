using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Color = Android.Graphics.Color;

namespace SafeCityMobile.Platforms.Android;

public class CustomPickerHandler : PickerHandler
{
    protected override MauiPicker CreatePlatformView()
    {
        var picker = base.CreatePlatformView();

        picker.SetHintTextColor(Color.White);

        return picker;
    }
}