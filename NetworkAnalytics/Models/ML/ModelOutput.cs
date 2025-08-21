
using Microsoft.ML.Data;

namespace NetworkAnalytics.Models.ML;

public class ModelOutput
{
    [ColumnName(@"col0")]
    public uint Col0 { get; set; }

    [ColumnName(@"col1")]
    public float[] Col1 { get; set; } = [];

    [ColumnName(@"Features")]
    public float[] Features { get; set; } = [];

    [ColumnName(@"PredictedLabel")]
    public float PredictedLabel { get; set; }

    [ColumnName(@"Score")]
    public float[] Score { get; set; } = [];
}