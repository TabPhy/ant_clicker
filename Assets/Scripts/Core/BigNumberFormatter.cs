using System;

/// <summary>
/// Converts large doubles into human-readable strings.
/// e.g.  1 234 567  →  "1.23 Million"
///       4.56e18    →  "4.56 Quintillion"
/// </summary>
public static class BigNumberFormatter
{
    private static readonly (double threshold, string suffix)[] _tiers =
    {
        (1e63,  "Vigintillion"),
        (1e60,  "Novemdecillion"),
        (1e57,  "Octodecillion"),
        (1e54,  "Septendecillion"),
        (1e51,  "Sexdecillion"),
        (1e48,  "Quindecillion"),
        (1e45,  "Quattuordecillion"),
        (1e42,  "Tredecillion"),
        (1e39,  "Duodecillion"),
        (1e36,  "Undecillion"),
        (1e33,  "Decillion"),
        (1e30,  "Nonillion"),
        (1e27,  "Octillion"),
        (1e24,  "Septillion"),
        (1e21,  "Sextillion"),
        (1e18,  "Quintillion"),
        (1e15,  "Quadrillion"),
        (1e12,  "Trillion"),
        (1e9,   "Billion"),
        (1e6,   "Million"),
        (1e3,   "Thousand"),
    };

    public static string Format(double value, int decimals = 2)
    {
        if (double.IsInfinity(value) || double.IsNaN(value)) return "∞";
        if (value < 0) return "-" + Format(-value, decimals);
        if (value < 1000) return Math.Floor(value).ToString();

        foreach (var (threshold, suffix) in _tiers)
        {
            if (value >= threshold)
            {
                double scaled = value / threshold;
                return scaled.ToString($"F{decimals}") + " " + suffix;
            }
        }

        return value.ToString("E2");
    }

    /// <summary>Short suffix format: 1.23M, 4.56B, etc.</summary>
    public static string FormatShort(double value)
    {
        if (value < 1_000)       return ((int)value).ToString();
        if (value < 1_000_000)   return (value / 1e3).ToString("F1")  + "K";
        if (value < 1_000_000_000) return (value / 1e6).ToString("F1") + "M";
        if (value < 1e12)        return (value / 1e9).ToString("F1")  + "B";
        if (value < 1e15)        return (value / 1e12).ToString("F1") + "T";
        return Format(value);
    }
}
