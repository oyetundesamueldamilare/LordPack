using System.Globalization;

namespace LordPack.Mobile.Converters;

public class PlayPauseTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isPlaying = value is bool b && b;
        if (isPlaying) return "Pause";
        if (parameter is string paramStr && !string.IsNullOrEmpty(paramStr))
            return $"▶ {paramStr}";
        return "Play";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}