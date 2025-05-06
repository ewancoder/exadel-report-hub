namespace ExportPro.Export.Job.ServiceHost.Helpers;

public static class CronToTextHelper
{
    public static string ToReadableText(string cron)
    {
        // Quartz Cron format: "0 mm HH dd MM ?"
        var parts = cron.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 6)
            return "Invalid cron expression";

        var minute = int.Parse(parts[1]);
        var hour = int.Parse(parts[2]);
        var dayOfMonth = parts[3];
        var month = parts[4];
        var dayOfWeek = parts[5];

        var time = new TimeOnly(hour, minute);
        var formattedTime = time.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

        if (dayOfMonth == "*" && dayOfWeek == "?")
        {
            return $"Every day at {formattedTime}";
        }

        if (dayOfWeek != "?" && dayOfWeek != "*")
        {
            return $"Every {Capitalize(dayOfWeek)} at {formattedTime}";
        }

        if (int.TryParse(dayOfMonth, out int dom))
        {
            return $"Every month on the {Ordinal(dom)} at {formattedTime}";
        }

        return "Custom schedule";
    }

    private static string Capitalize(string input)
    {
        return input.Length > 1
            ? char.ToUpper(input[0]) + input.Substring(1).ToLower()
            : input.ToUpper();
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
            _ => $"{number}th"
        };
    }
}