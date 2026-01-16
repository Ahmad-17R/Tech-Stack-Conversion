namespace VetSuccess.Shared.Utilities;

public static class USHolidays
{
    public static bool IsHoliday(DateTime date)
    {
        var year = date.Year;
        var holidays = GetHolidays(year);
        return holidays.Contains(date.Date);
    }
    
    public static bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
    
    public static DateTime GetNextWorkday(DateTime date)
    {
        var nextDay = date.AddDays(1);
        while (IsWeekend(nextDay) || IsHoliday(nextDay))
        {
            nextDay = nextDay.AddDays(1);
        }
        return nextDay;
    }
    
    public static DateTime GetPreviousWorkday(DateTime date)
    {
        var previousDay = date.AddDays(-1);
        while (IsWeekend(previousDay) || IsHoliday(previousDay))
        {
            previousDay = previousDay.AddDays(-1);
        }
        return previousDay;
    }
    
    private static HashSet<DateTime> GetHolidays(int year)
    {
        var holidays = new HashSet<DateTime>
        {
            // New Year's Day
            new DateTime(year, 1, 1),
            
            // Martin Luther King Jr. Day (3rd Monday in January)
            GetNthWeekdayOfMonth(year, 1, DayOfWeek.Monday, 3),
            
            // Presidents' Day (3rd Monday in February)
            GetNthWeekdayOfMonth(year, 2, DayOfWeek.Monday, 3),
            
            // Memorial Day (Last Monday in May)
            GetLastWeekdayOfMonth(year, 5, DayOfWeek.Monday),
            
            // Independence Day
            new DateTime(year, 7, 4),
            
            // Labor Day (1st Monday in September)
            GetNthWeekdayOfMonth(year, 9, DayOfWeek.Monday, 1),
            
            // Columbus Day (2nd Monday in October)
            GetNthWeekdayOfMonth(year, 10, DayOfWeek.Monday, 2),
            
            // Veterans Day
            new DateTime(year, 11, 11),
            
            // Thanksgiving (4th Thursday in November)
            GetNthWeekdayOfMonth(year, 11, DayOfWeek.Thursday, 4),
            
            // Christmas
            new DateTime(year, 12, 25)
        };
        
        // Note: Black Friday is NOT included (as per Django USHolidaysWithoutBlackFriday)
        
        return holidays;
    }
    
    private static DateTime GetNthWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek, int occurrence)
    {
        var firstDay = new DateTime(year, month, 1);
        var firstOccurrence = firstDay.AddDays(((int)dayOfWeek - (int)firstDay.DayOfWeek + 7) % 7);
        return firstOccurrence.AddDays(7 * (occurrence - 1));
    }
    
    private static DateTime GetLastWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek)
    {
        var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        var daysToSubtract = ((int)lastDay.DayOfWeek - (int)dayOfWeek + 7) % 7;
        return lastDay.AddDays(-daysToSubtract);
    }
}
