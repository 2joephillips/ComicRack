using System.Drawing;
using System.Drawing.Imaging;

namespace ComicRack.Desktop.Core;
public static class ColorAnalyzer
{
    public static List<Color> GetTopColors(Bitmap bitmap, int topCount)
    {
        var colorCounts = new Dictionary<int, int>(); // Use ARGB as key
        var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int offset = y * data.Stride + x * 4;
                        int alpha = ptr[offset + 3];
                        int red = ptr[offset + 2];
                        int green = ptr[offset + 1];
                        int blue = ptr[offset];

                        // Skip transparent and filtered-out colors
                        if (alpha < 255 || IsWhiteGrayBlack(red, green, blue))
                            continue;

                        // Pack the color into an int
                        int color = (alpha << 24) | (red << 16) | (green << 8) | blue;

                        if (colorCounts.TryGetValue(color, out int count))
                            colorCounts[color] = count + 1;
                        else
                            colorCounts[color] = 1;
                    }
                }
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        // Get top N colors
        return colorCounts
            .OrderByDescending(c => c.Value)
            .Take(topCount)
            .Select(c => System.Drawing.Color.FromArgb(c.Key))
            .ToList();
    }

    private static bool IsWhiteGrayBlack(int r, int g, int b)
    {
        // Threshold values for optimization
        const int brightnessThreshold = 220; // Near white
        const int darknessThreshold = 35;   // Near black
        const int grayTolerance = 15;       // Low saturation (gray)

        int max = Math.Max(r, Math.Max(g, b));
        int min = Math.Min(r, Math.Min(g, b));
        int diff = max - min;

        // If brightness or darkness is out of range, or if the color is too gray
        return max > brightnessThreshold || min < darknessThreshold || diff < grayTolerance;
    }
}
