### Visual Regression Testing

The VR directory contains references, screenshots, and diff folders

- All images are in jpeg format and handled as Bitmaps internally
- For visual regression tests to run effectively, reference images must be placed in the references folder for comparisons
- Viewport tests will automatically save a reference image based on the test method name if one does not exist
- Element based tests will require references to be manually added and are also accessed by the file name

Usage:
1. `CheckViewport(string screenshotName, double mismatchTolerance)`
   - screenshotName: Name of the screenshot to save/compare against in the references directory
  - mismatchTolerance: The percent tolerance as a decimal in which to allow for a passed result (0.00 - 1). Set to 0 to require an exact match of the web view.
  - Ex: CheckViewport("WebsitePageName", 0.01)
2. `CheckViewportContains(string referenceElementName, double mismatchTolerance)`
  - referenceElementName: Name of the reference image to compare against in the references directory
  - mismatchTolerance: The percent tolerance as a decimal in which to allow for a passed result (0.00 - 1). Set to 0 to require an exact match is found within the web view screenshot.
  - Ex: CheckViewportContains("ElementName", 0.01)

Test Examples using [NUnit](https://github.com/nunit/docs/wiki):

```
[TestCase()]
public void Visual_Regression_Viewport_Test()
{
	string testName = TestContext.CurrentContext.Test.MethodName;

	VRResults results = VR.CheckViewport(testName, 0.05);
	Assert.IsTrue(results.IsWithinTolerance, $"Visual Regression viewport check failed. Mismatch percentage: {results.MismatchPercentage}");
}
```

```
[TestCase()]
public void Visual_Regression_logo_Test()
{
	VRResults results = VR.CheckViewportContains("logo", 0.01);
	Assert.True(results.IsWithinTolerance, $"Visual Regression viewport logo check failed. Mismatch percentage: {results.MismatchPercentage}");
}
```

### Dependencies
- [Selenium](https://www.seleniumhq.org/docs/)
- [AForge.Imaging] (http://www.aforgenet.com/framework/docs/html/d087503e-77da-dc47-0e33-788275035a90.htm)
