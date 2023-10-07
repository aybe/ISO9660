using System;

namespace CDROMTools.Utils
{
    /// <summary>
    /// Progress reporting helper, reports progress in increments of one percent, as a range from zero to one.
    /// </summary>
    public sealed class ProgressHelper
    {
        private readonly IProgress<double> _progress;
        private readonly int _iterations;
        private int _percent;

        public ProgressHelper(IProgress<double> progress, int iterations)
        {
            if (progress == null) throw new ArgumentNullException(nameof(progress));
            if (iterations <= 0) throw new ArgumentOutOfRangeException(nameof(iterations));
            _progress = progress;
            _iterations = iterations;
        }

        public void Update(int iteration)
        {
            var percent = (int)Math.Ceiling((double)(iteration + 1) / _iterations * 100.0d);
            if (percent <= _percent) return;
            _progress.Report(percent / 100.0d);
            _percent = percent;
        }
    }
}