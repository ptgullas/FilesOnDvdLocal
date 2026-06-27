using System;
using System.Drawing;

public static class ColorHelper {
    /// <summary>
    /// Darkens a hex color by a specified percentage.
    /// </summary>
    /// <param name="hexColor">The input hex string (e.g., "#3B82F6" or "3B82F6").</param>
    /// <param name="darkenPercentage">Value between 0 and 100 (e.g., 20 for 20% darker).</param>
    /// <returns>A darkened hex color string.</returns>
    public static string DarkenHexColor(string hexColor, double darkenPercentage) {
        // 1. Clean the input and parse to a Color object
        hexColor = hexColor.TrimStart('#');
        Color color = ColorTranslator.FromHtml("#" + hexColor);

        // 2. Calculate the multiplier factor (e.g., 20% darker means keeping 80% brightness)
        double factor = 1.0 - (darkenPercentage / 100.0);
        factor = Math.Max(0.0, Math.Min(1.0, factor)); // Clamp between 0 and 1

        // 3. Multiply the RGB channels to lower brightness without shifting hue
        int r = (int)Math.Round(color.R * factor);
        int g = (int)Math.Round(color.G * factor);
        int b = (int)Math.Round(color.B * factor);

        // 4. Convert the new RGB values back to a standard hex string
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}