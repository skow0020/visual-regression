using OpenQA.Selenium;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace VisualRegression
{
    public class VisualRegression
    {
        public IWebDriver driver;
        private string VRDirectory;
        private string ReferencesDirectory;
        private string ScreenshotsDirectory;
        private string DiffDirectory;

        public VisualRegression(IWebDriver driver)
        {
            this.driver = driver;
            VRDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + Path.DirectorySeparatorChar + "VR";
            ReferencesDirectory = $"{VRDirectory}\\references";
            ScreenshotsDirectory = $"{VRDirectory}\\screenshots";
            DiffDirectory = $"{VRDirectory}\\diff";
        }

        /// <summary>
        /// Check the web viewport against a reference screenshot for regressions
        /// </summary>
        /// <param name="screenshotName"></param>
        /// <param name="mismatchTolerance"></param>
        /// <returns></returns>
        public VRResults CheckViewport(string screenshotName, double mismatchTolerance)
        {
            CreateDirectories();

            if (!ReferenceScreenshotExists(screenshotName))
            {
                SaveReferenceScreenshot(((ITakesScreenshot)driver).GetScreenshot(), screenshotName);
                throw new Exception($"Reference screenshot did not exist. New one saved: {screenshotName}");
            }

            SaveCurrentScreenshot(((ITakesScreenshot)driver).GetScreenshot(), screenshotName);

            Bitmap currentScreenshot = LoadSavedScreenshot(screenshotName);
            Bitmap referenceScreenShot = LoadReferenceImage(screenshotName);

            Tuple<Bitmap, double> comparisonResults = CompareImages(referenceScreenShot, currentScreenshot);
            Bitmap imageDiff = comparisonResults.Item1;

            VRResults results = new VRResults(mismatchTolerance, comparisonResults.Item2);
            if (results.MismatchPercentage > 0) imageDiff.Save($"{DiffDirectory}/{screenshotName}.jpg");

            return results;
        }

        /// <summary>
        /// Check the web viewport for a specific element or image for regressions
        /// </summary>
        /// <param name="referenceElementName"></param>
        /// <param name="mismatchTolerance"></param>
        /// <returns></returns>
        public VRResults CheckViewportContains(string referenceElementName, double mismatchTolerance)
        {
            CreateDirectories();

            if (!ReferenceScreenshotExists(referenceElementName))
            {
                throw new Exception($"Reference screenshot did not exist: {referenceElementName} Pleasae add it.");
            }

            string screenshotName = $"{driver.Title}_{DateTime.Now.ToString("MMddyyyy_HHmm")}";
            SaveCurrentScreenshot(((ITakesScreenshot)driver).GetScreenshot(), screenshotName);

            Bitmap currentScreenshot = LoadSavedScreenshot(screenshotName);
            Bitmap referenceElementScreenShot = LoadReferenceImage(referenceElementName);

            double mismatchPercentage;

            using (Bitmap reference = new Bitmap(referenceElementScreenShot))
            using (Bitmap formattedReference = reference.Clone(new Rectangle(0, 0, reference.Width, reference.Height), PixelFormat.Format24bppRgb))
            {
                mismatchPercentage = BitmapExtensions.EmbeddedImageConfidenceValue(currentScreenshot, formattedReference);
            }

            return new VRResults(mismatchTolerance, mismatchPercentage);
        }

        /// <summary>
        /// Setup VR directories
        /// </summary>
        private void CreateDirectories()
        {
            Directory.CreateDirectory(VRDirectory);
            Directory.CreateDirectory(ReferencesDirectory);
            Directory.CreateDirectory(ScreenshotsDirectory);
            Directory.CreateDirectory(DiffDirectory);
        }

        /// <summary>
        /// Save screenshot in screenshots directory with specified file name
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="filename"></param>
        private void SaveCurrentScreenshot(Screenshot ss, string filename)
        {
            ss.SaveAsFile($"{ScreenshotsDirectory}/{filename}.jpg", ScreenshotImageFormat.Jpeg);
        }

        /// <summary>
        /// Save screenshot in screenshots directory with specified file name
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="filename"></param>
        private void SaveReferenceScreenshot(Screenshot reference, string filename)
        {
            reference.SaveAsFile($"{ReferencesDirectory}/{filename}.jpg", ScreenshotImageFormat.Jpeg);
        }

        /// <summary>
        /// Compare 2 provided bitmap images and return a tuple of a diff bitmap and the mismatch value
        /// </summary>
        /// <param name="referenceImage"></param>
        /// <param name="screenshotImage"></param>
        /// <returns></returns>
        private Tuple<Bitmap, double> CompareImages(Bitmap referenceImage, Bitmap screenshotImage)
        {
            if (referenceImage.Size != screenshotImage.Size)
            {
                throw new Exception("Unable to compare images, they are different sizes");
            }

            float diff = 0;
            Bitmap diffImage = new Bitmap(screenshotImage.Width, screenshotImage.Height);

            for (int y = 0; y < referenceImage.Height; y++)
            {
                for (int x = 0; x < referenceImage.Width; x++)
                {
                    Color pixel1 = referenceImage.GetPixel(x, y);
                    Color pixel2 = screenshotImage.GetPixel(x, y);

                    if (pixel1 != pixel2)
                    {
                        diff++;
                        diffImage.SetPixel(x, y, Color.Pink);
                    }
                    else diffImage.SetPixel(x, y, pixel2);
                }
            }

            double mismatchPercentage = diff / (referenceImage.Width * referenceImage.Height);

            return Tuple.Create(diffImage, mismatchPercentage);
        }

        /// <summary>
        /// Load image file from reference directory with specified file name
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        private Bitmap LoadReferenceImage(string imageFile)
        {
            return new Bitmap($"{ReferencesDirectory}/{imageFile}.jpg");
        }

        /// <summary>
        /// Load screenshot from the screenshot directory with specified file name
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        private Bitmap LoadSavedScreenshot(string imageFile)
        {
            return new Bitmap($"{ScreenshotsDirectory}/{imageFile}.jpg");
        }

        /// <summary>
        /// Check if a specified reference screenshot exists
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        private bool ReferenceScreenshotExists(string imageFile)
        {
            if (File.Exists($"{ReferencesDirectory}/{imageFile}.jpg")) return true;
            else return false;
        }
    }
}
