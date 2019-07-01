using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Drawing;

namespace VisualRegression
{
    static class BitmapExtensions
    {
        /// <summary>
        /// Check if reference image is embedded in the provided screenshot and return (1 - confidence value) as mismatch percentage
        /// </summary>
        /// <param name="screenshot">The Bitmap that might contain.</param>
        /// <param name="referenceImage">The Bitmap that might be contained in.</param>        
        /// <returns>You guess!</returns>
        public static double EmbeddedImageConfidenceValue(this Bitmap screenshot, Bitmap referenceImage)
        {
            const int divisor = 4;

            // This is set to 0.9f because processing returns a list of rectangular coordinates that pass the threshold provided - limiting the list to above 0.9
            ExhaustiveTemplateMatching etm = new ExhaustiveTemplateMatching(0.9f);

            TemplateMatch[] tm = etm.ProcessImage(
                new ResizeNearestNeighbor(screenshot.Width / divisor, screenshot.Height / divisor).Apply(screenshot),
                new ResizeNearestNeighbor(referenceImage.Width / divisor, referenceImage.Height / divisor).Apply(referenceImage)
                );

            if (tm.Length == 0) throw new Exception("Image provided was not found on the screenshot. No rectangular pixel set was above 90% confidence value");

            return 1 - tm[0].Similarity;
        }
    }
}
