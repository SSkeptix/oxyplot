﻿// --------------------------------------------------------------------------------------------------------------------
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
    public class RadarSeries : Series
    {
        /// <summary>
        /// Gets or sets axis min value for all dimension
        /// </summary>
        public IList<double> AxisMinValues { get; set; }

        /// <summary>
        /// Gets or sets axis min value
        /// </summary>
        public double? AxisMinValue { get; set; }

        /// <summary>
        /// Gets or sets axis max value for all dimension
        /// </summary>
        public IList<double> AxisMaxValues { get; set; }

        /// <summary>
        /// Gets or sets axis min value
        /// </summary>
        public double? AxisMaxValue { get; set; }

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
        /// Default constructor
        /// </summary>
        public RadarSeries()
        {
            Items = new List<RadarItem>();
            IsDifferentAxisValue = false;
            AxisStepCount = 0;
        }

        /// <summary>
        /// Render gridline of radar chart
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        protected void RenderGridline(IRenderContext rc, ScreenPoint midPoint)
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
                        midPoint.X + currentRadius * Math.Sin(currentAngle),
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


            // set size of graph, area is square
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


            // rendering
            RenderGridline(rc, midPoint);
            foreach (var item in Items)
                if (IsDifferentAxisValue)
                    item.Render(rc, midPoint, radius, 0, 0);
                else
                    item.Render(rc, midPoint, radius, AxisMinValue.Value, AxisMaxValue.Value);

        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            //TODO: write code
        }

        /// <summary>
        /// Sets the default values.
        /// </summary>
        protected internal override void SetDefaultValues()
        {
            if (Items != null && Items.Count > 0 && Items[0].Value != null)
                DimensionCount = Items[0].Value.Count;

            if (DimensionCount < 3)
                DimensionCount = 3;


            //TODO: write code
        }

        /// <summary>
        /// Updates the maximum and minimum values of the axes used by this series.
        /// </summary>
        protected internal override void UpdateAxisMaxMin()
        {
            if (Items != null && Items.Count > 0)
            {
                // if axis value are different
                if (IsDifferentAxisValue)
                {
                    if (AxisMinValues == null || AxisMaxValues == null)
                    {
                        List<double> min = new List<double>(DimensionCount);
                        List<double> max = new List<double>(DimensionCount);

                        for (int dimension = 0; dimension < DimensionCount; dimension++)
                        {
                            min[dimension] = max[dimension] = Items[0].Value[dimension];

                            foreach (var item in Items)
                                if (item.Value != null)
                                {
                                    if (item.Value[dimension] < min[dimension])
                                        min[dimension] = item.Value[dimension];
                                    else if (item.Value[dimension] > max[dimension])
                                        max[dimension] = item.Value[dimension];
                                }
                        }
                        if (AxisMinValues == null)
                            AxisMinValues = min;

                        if (AxisMaxValues == null)
                            AxisMaxValues = max;

                        // if all data are similar to each other
                        for (int dimension = 0; dimension < DimensionCount; dimension++)
                            if (AxisMaxValues[dimension] == AxisMinValues[dimension])
                                AxisMaxValues[dimension] += 1;
                    }
                }

                // if axis value are similar
                else
                {
                    if (AxisMinValue == null || AxisMaxValue == null)
                    {
                        double min, max;
                        min = max = Items[0].Value[0];

                        foreach (var item in Items)
                            foreach (var value in item.Value)
                                if (value < min)
                                    min = value;
                                else if (value > max)
                                    max = value;

                        if (AxisMinValue == null)
                            AxisMinValue = min;

                        if (AxisMaxValue == null)
                            AxisMaxValue = max;
                    }

                    // if all data are similar to each other
                    if (AxisMaxValue == AxisMinValue)
                        AxisMaxValue += 1;
                }
            }
        }


        /// <summary>
        /// Updates the data.
        /// </summary>
        protected internal override void UpdateData()
        {
            // set dimension count
            if (Items != null && Items[0].Value != null)
                DimensionCount = Items[0].Value.Count;

            //TODO: write code
        }

        /// <summary>
        /// Updates the maximum and minimum values of the series.
        /// </summary>
        protected internal override void UpdateMaxMin()
        {
            //TODO: write code
        }

        /// <summary>
        /// Updates the valid items
        /// </summary>
        protected internal override void UpdateValidData()
        {}

        /// <summary>
        /// Checks if this data series requires X/Y axes. (e.g. RadarSeries does not require axes)
        /// </summary>
        /// <returns>True if no axes are required.</returns>
        protected internal override bool AreAxesRequired()
        {
            return false;
        }

        /// <summary>
        /// Ensures that the axes of the series is defined.
        /// </summary>
        protected internal override void EnsureAxes()
        { }

        /// <summary>
        /// Check if the data series is using the specified axis.
        /// </summary>
        /// <param name="axis">An axis.</param>
        /// <returns>True if the axis is in use.</returns>
        protected internal override bool IsUsing(Axis axis)
        {
            //TODO: maybe change this
            return false;
        }
    }
}
//*/