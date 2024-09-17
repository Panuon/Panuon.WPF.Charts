using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public interface IChartSegmentsSeries
    {
		IEnumerable<SegmentBase> GetSegments();
	}
}
