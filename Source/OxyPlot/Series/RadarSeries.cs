// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadarSeries.cs" company="OxyPlot">
//   Copyright (c) 2017 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a series for radar chart.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
//*
namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot.Axes;

    /// <summary>
    /// Represents a series for radar chart.
    /// </summary>
    /// <remarks>The arc length/central angle/area of each slice is proportional to the quantity it represents.
    public class RadarSeries : ItemsSeries
    {
        /// <summary>
        /// Gets or sets axis min value for all dimension
        /// </summary>
        public IList<double> AxisMinValues { get; set; }

        /// <summary>
        /// Gets or sets axis min value
        /// </summary>
        public double AxisMinValue { get; set; }

        /// <summary>
        /// Gets or sets axis max value for all dimension
        /// </summary>
        public IList<double> AxisMaxValues { get; set; }

        /// <summary>
        /// Gets or sets axis min value
        /// </summary>
        public double AxisMaxValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// all axis have same axis value or not.
        /// <para> If this flag is set to <c>true</c> diffent axis are enable.</para>
        /// </summary>
        public bool IsDifferentAxisValue { get; set; }

        /// <summary>
        /// Gets or sets axis step count
        /// </summary>
        public int AxisStepCount { get; set; }

        /// <summary>
        /// Get count of dimension
        /// </summary>
        public int DimensionCount { get; private set; }


        /// <summary>
        /// Gets or sets gridline stroke color
        /// </summary>
        public OxyColor GridlineStrokeColor { get; set; }

        /// <summary>
        /// Gets of set gridline stroke thickness
        /// </summary>
        public double GridlineStrokeThickness { get; set; }


        /// <summary>
        /// Array of items
        /// </summary>
        public IList<RadarItem> Items { get; set; }





        private double radius;


        /// <summary>
        /// Render gridline of radar chart
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        protected void RenderGridline(IRenderContext rc, ScreenPoint midPoint )
        {
            double currentRadius;
            double radiusStep;
            currentRadius = radiusStep = radius / AxisStepCount;

            double currentAngle = 0;
            double angleStep = 2 * Math.PI / DimensionCount;

            ScreenPoint[] points = new ScreenPoint[DimensionCount];

            // calculate vertex of Gridlines polygon
            for (int step = 0; step < AxisStepCount; step++, currentRadius += radiusStep)
            {
                currentAngle = 0;

                for (int dimension = 0; dimension < DimensionCount; dimension++, currentAngle += angleStep)
                {
                    points[dimension] = new ScreenPoint(
                        midPoint.X - currentRadius * Math.Sin(currentAngle),
                        midPoint.Y - currentRadius * Math.Cos(currentAngle)
                        );
                }
                //TODO: add gridline fill color
                rc.DrawPolygon(points, OxyColors.Automatic, GridlineStrokeColor, GridlineStrokeThickness);
            }
        }

        public override void Render(IRenderContext rc)
        {
            DimensionCount = Items[0].Value.Count;


            if (DimensionCount < 3)
                throw new ArgumentException(string.Format("Dimension count must be greater than 2, but you have {0} dimentions", DimensionCount));


            AxisMinValue = 0;
            AxisMaxValue = 10;


            // set size of graph, area is square
            double heightMargin = 0;
            double heightGraph = PlotModel.PlotArea.Height;
            double radiusBigCircle, radiusSmallCircle;
            double angle = (2 * Math.PI / DimensionCount);
            // set radiuses
            if (DimensionCount % 2 == 0)
            {
                radius =
                radiusBigCircle =
                radiusSmallCircle = heightGraph / 2;
            }
            else
            {
                radius = radiusBigCircle = heightGraph / (1 + (float)Math.Cos(angle / 2));
                radiusSmallCircle = heightGraph * (float)Math.Cos(angle / 2) / (1 + (float)Math.Cos(angle / 2));
            }





            var midPoint = new ScreenPoint(
                (this.PlotModel.PlotArea.Left + this.PlotModel.PlotArea.Right) / 2,
                (this.PlotModel.PlotArea.Top + this.PlotModel.PlotArea.Bottom) / 2);


            RenderGridline(rc, midPoint);
            foreach (var item in Items)
                if (IsDifferentAxisValue)
                    item.Render(rc, midPoint, radius, 0, 0);
                else
                    item.Render(rc, midPoint, radius, AxisMinValue, AxisMaxValue);

        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
        }

        protected internal override bool AreAxesRequired()
        {
            return false;
        }

        protected internal override void EnsureAxes()
        {
            //throw new NotImplementedException();
        }

        protected internal override bool IsUsing(Axis axis)
        {
            return false;
        }

        protected internal override void SetDefaultValues()
        {
            //throw new NotImplementedException();
        }

        protected internal override void UpdateAxisMaxMin()
        {

           // throw new NotImplementedException();
        }

        protected internal override void UpdateData()
        {
            //throw new NotImplementedException();
        }

        protected internal override void UpdateMaxMin()
        {
            //throw new NotImplementedException();
        }
    }
}
//*/