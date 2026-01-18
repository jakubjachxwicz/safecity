using System.Globalization;

namespace SafeCityMobile.ViewModels.Map;

public class MapHelpers
{
    public string StringifyCoordinate(double coord)
    {
        return coord.ToString(CultureInfo.InvariantCulture);
    }
}
