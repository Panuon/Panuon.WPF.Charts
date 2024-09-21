# Panuon.WPF.Charts

`This project is still under development.`

## How to use

### 1. Add Chart Control

Add a CartesianChart control (for line charts, column charts, etc.) or a RadialChart control (for pie charts, donut charts, etc.).  

```xml
xmlns:pc="https://opensource.panuon.com/wpf-charts"
...

<pc:CartesianChart x:Name="chart" TitleMemberPath="Title" AnimationDuration="0:0:3" AnimationEasing="CubicOut" GridLinesBrush="#EFEFEF" GridLinesVisibility="Horizontal">
</pc:CartesianChart>

or

<pc:RadialChart x:Name="chart" TitleMemberPath="Title" AnimationDuration="0:0:3" AnimationEasing="CubicOut">
</pc:RadialChart>

```

Add a data source, supporting data binding.

```CSharp

chart.ItemsSource = new object[]
{
    { Title = "Data 1", Value1 = 5, Value2 = 15, Value3 = 10, Value4 = 20, Value5 = 15 },
    { Title = "Data 2", Value1 = 15, Value2 = 10, Value3 = 20, Value4 = 30, Value5 = 20 },
    ...
}
```

### 2. Set axises (optional)

Add X and Y axes to the chart to customize the axis details (Note: RadialChart does not support XY axes), or hide the axes by using `XAxis="{x:Null}"` or `YAxis="{x:Null}"`.  

```xml

...
<pc:CartesianChart.XAxis>
    <pc:XAxis Spacing="15" Stroke="Red" Foreground="Red" />
</pc:CartesianChart.XAxis>
<pc:CartesianChart.YAxis>
    <!--If MinValue and MaxValue are not set, the chart control will automatically calculate the maximum and minimum values.-->
    <pc:YAxis Spacing="15" Stroke="Red" Foreground="Red" MinValue="0" MaxValue="50"/>
</pc:CartesianChart.YAxis>
...

```

### 3. Add layers (optional)

By default, the chart control does not respond to mouse interactions. You can enhance mouse interaction effects (such as crosshairs and hover points) by adding layers. Additionally, you can customize additional layers by deriving from the LayerBase class.
`Panuon.UI.Charts` currently provides two layers, and we will continue to optimize them.

```xml
<pc:CartesianChart.Layers>
    <pc:CrossLineLayer />
    <pc:ToolTipLayer />
</pc:CartesianChart.Layers>
```
### 4. Add series to chart

You can add chart series to the `Series` property of the chart control.

```xml
<pc:CartesianChart.Series>
    ...
</pc:CartesianChart.Series>
```

Available series are listed here.

#### CartesianChart

#### LineSeries

```xml
<pc:LineSeries Stroke="#FCBA03" StrokeThickness="2" ValueMemberPath="Value1" />
<pc:LineSeries Stroke="#36A5EB" StrokeThickness="2" ValueMemberPath="Value2" />
<pc:LineSeries Stroke="#F55195" StrokeThickness="2" ValueMemberPath="Value3" />
```

#### ColumnSeries

Note: The ColumnWidth property is of type GridLength, which supports using percentages (e.g., ColumnWidth="0.2*"), indicating that the column width will be 20% of the available width for that coordinate column. If you do not explicitly set the ColumnWidth property, the chart control will use the default automatic value.

```xml
<pc:ColumnSeries Fill="#FCBA03" ColumnWidth="15" ValueMemberPath="Value1" />
<pc:ColumnSeries Fill="#36A5EB" ColumnWidth="15" ValueMemberPath="Value2" />
<pc:ColumnSeries Fill="#F55195" ColumnWidth="15" ValueMemberPath="Value3" />
```

#### ClusteredColumnSeries

```xml
<pc:ClusteredColumnSeries>
    <pc:ClusteredColumnSeriesSegment Title="Data 2-1" BackgroundFill="#1A36A5EB" Fill="#36A5EB" ValueMemberPath="Value1" />
    <pc:ClusteredColumnSeriesSegment Title="Data 2-2" BackgroundFill="#1AF55195" Fill="#F55195" ValueMemberPath="Value2" />
    <pc:ClusteredColumnSeriesSegment Title="Data 2-3" BackgroundFill="#1AFCBA03" Fill="#FCBA03" ValueMemberPath="Value3" />
</pc:ClusteredColumnSeries>
```