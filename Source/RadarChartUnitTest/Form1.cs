namespace RadarChartUnitTest
{
    using System;
    using System.Windows.Forms;

    using OxyPlot;
    using OxyPlot.Series;
    using System.Collections.Generic;


    public partial class Form1 : Form
    {
        PlotModel plotModel;

        public Form1()
        {
            InitializeComponent();
            plotModel = new PlotModel { Title = "Example 1" };


            RadarSeries v = new RadarSeries{
                GridlineStrokeColor = OxyColors.Gray,
                GridlineStrokeThickness = 0.3,
                AxisStepCount = 10,
            };
            var i = new RadarItem {
                Value = new List<double>(new[] { 4, 3.2, 8, 9, 9, 5, 0, 10 }),
                ChartStrokeColor = OxyColors.Black,
                ChartStrokeThickness = 1,
            };

            v.Items = new List<RadarItem>();
            v.Items.Add(i);

            plotModel.Series.Add(v);


            this.plot1.Model = plotModel;



        }


    }
}
