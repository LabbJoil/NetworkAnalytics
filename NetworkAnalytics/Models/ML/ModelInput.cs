
using Microsoft.ML.Data;

namespace NetworkAnalytics.Models.ML;

public class ModelInput
{
    [LoadColumn(0)]
    [ColumnName(@"col0")]
    public float Col0 { get; set; }

    [LoadColumn(1)]
    [ColumnName(@"col1")]
    public string Col1 { get; set; } = "";

}
