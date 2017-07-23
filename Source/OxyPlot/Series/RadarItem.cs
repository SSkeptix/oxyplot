// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadarItem.cs" company="OxyPlot">
//   Copyright (c) 2017 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a item for radar chart.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
//*
namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a item for radar chart.
    /// </summary>
    public class RadarItem
    {
        /// <summary>
        /// Gets or sets chart stroke color
        /// </summary>
        public OxyColor ChartStrokeColor { get; set; }

        /// <summary>
        /// Gets of set chart stroke thickness
        /// </summary>
        public double ChartStrokeThickness { get; set; }

        /// <summary>
        /// Gets of set chart dot radius
        /// </summary>
        public double ChartDotRadius { get; set; }

        /// <summary>
        /// Fill color of polygon
        /// </summary>
        public OxyColor FillColor { get; set; }

        /// <summary>
        /// Array of values
        /// </summary>
        public IList<double> Value { get; set; }
      


        /// <summary>
        /// Render chart line of item
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        /// <param name="midPoint">The chart center position</param>
        /// <param name="radius">The radius of chart</param>
        internal void Render(IRenderContext rc, ScreenPoint midPoint, double radius, double axisMinValue, double axisMaxValue)
        {
            double currentAngle = 0;
            double angleStep = 2 * Math.PI / Value.Count;

            ScreenPoint[] points = new ScreenPoint[Value.Count];

            // calculate vertex of chart polygon
            for (int dimension = 0; dimension < Value.Count; dimension++, currentAngle += angleStep)
            {
                points[dimension] = new ScreenPoint(
                    midPoint.X - radius * (Value[dimension] - axisMinValue) / (axisMaxValue - axisMinValue) * Math.Sin(currentAngle),
                    midPoint.Y - radius * (Value[dimension] - axisMinValue) / (axisMaxValue - axisMinValue) * Math.Cos(currentAngle)
                    );
            }
            rc.DrawPolygon(points, OxyColors.Automatic, ChartStrokeColor, ChartStrokeThickness);
        }
    }
}
//*/