namespace VisualRegression
{
    public class VRResults
    {
        public VRResults(double mismatchTolerance, double mismatchPercentage)
        {
            MismatchTolerance = mismatchTolerance;
            MismatchPercentage = mismatchPercentage;
            IsWithinTolerance = MismatchPercentage <= MismatchTolerance;
        }

        private double MismatchTolerance { get; }
        public double MismatchPercentage { get; set; }
        public bool IsWithinTolerance { get; }
    }
}