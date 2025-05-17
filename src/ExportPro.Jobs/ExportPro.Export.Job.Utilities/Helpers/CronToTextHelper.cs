namespace ExportPro.Export.Job.Utilities.Helpers;

public static class CronToTextHelper
{
    private static readonly Dictionary<string, string> DayOfWeekNames = new()
    {
        ["1"] = "Sunday",
        ["2"] = "Monday",
        ["3"] = "Tuesday",
        ["4"] = "Wednesday",
        ["5"] = "Thursday",
        ["6"] = "Friday",
        ["7"] = "Saturday",
    };

    public static string ToReadableText(string cron)
    {
        var parts = cron.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 6)
            return "Invalid cron expression";

        var seconds = parts[0];
        var minute = parts[1];
        var hour = parts[2];
        var dayOfMonth = parts[3];
        var month = parts[4];
        var dayOfWeek = parts[5];

        // Handle minute-based intervals like "0/5"
        if (minute.Contains("/"))
        {
            var step = GetStepValue(minute);
            return $"Every {step} minutes";
        }

        // Handle hour-based intervals like "*/2"
        if (hour.Contains("/"))
        {
            var step = GetStepValue(hour);
            return $"Every {step} hours";
        }

        // Fixed time formatting
        if (int.TryParse(minute, out var min) && int.TryParse(hour, out var hr))
        {
            var time = new TimeOnly(hr, min);
            var formattedTime = time.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

            if (dayOfMonth == "*" && (dayOfWeek == "?" || dayOfWeek == "*"))
                return $"Every day at {formattedTime}";

            if (dayOfWeek != "?" && dayOfWeek != "*")
            {
                var readableDay = DayOfWeekNames.TryGetValue(dayOfWeek, out var name) ? name : Capitalize(dayOfWeek);

                return $"Every {readableDay} at {formattedTime}";
            }

            if (int.TryParse(dayOfMonth, out int dom))
            {
                return $"Every month on the {Ordinal(dom)} at {formattedTime}";
            }

            return $"At {formattedTime}";
        }

        return "Custom schedule";
    }

    private static int GetStepValue(string part)
    {
        var stepParts = part.Split('/');
        return stepParts.Length == 2 && int.TryParse(stepParts[1], out var value) ? value : 1;
    }

    private static string Capitalize(string input)
    {
        return input.Length > 1 ? char.ToUpper(input[0]) + input.Substring(1).ToLower() : input.ToUpper();
    }

    private static string Ordinal(int number)
    {
        if (number % 100 is 11 or 12 or 13)
            return $"{number}th";

        return (number % 10) switch
        {
            1 => $"{number}st",
            2 => $"{number}nd",
            3 => $"{number}rd",
            _ => $"{number}th",
        };
    }
}
