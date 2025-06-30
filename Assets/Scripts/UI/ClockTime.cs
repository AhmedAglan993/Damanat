[System.Serializable]
public struct ClockTime
{
    public int hour;
    public int minute;

    public ClockTime(int h, int m)
    {
        hour = h;
        minute = m;
        Normalize();
    }

    public int TotalMinutes => hour * 60 + minute;

    public static ClockTime FromTotalMinutes(int total)
    {
        return new ClockTime(total / 60, total % 60);
    }

    public override string ToString() => $"{hour:00}:{minute:00}";

    public static ClockTime operator +(ClockTime a, int minutes) => FromTotalMinutes(a.TotalMinutes + minutes);
    public static int operator -(ClockTime a, ClockTime b) => a.TotalMinutes - b.TotalMinutes;

    private void Normalize()
    {
        if (minute >= 60)
        {
            hour += minute / 60;
            minute = minute % 60;
        }
    }
}
