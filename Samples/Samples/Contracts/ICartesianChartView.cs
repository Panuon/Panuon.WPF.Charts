using Panuon.WPF;
using System;

namespace Samples
{
    public interface ICartesianChartView
    {
        void Generate();

        void SetAnimation(
            AnimationEasing animationEasing,
            TimeSpan? animationDuration
        );
    }
}
