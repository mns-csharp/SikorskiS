using ZedGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Figure_7_Sikorski
{
    public class DataPlotter
    {        
        private GraphPane myPane_;
        private int curveCounter_ = 0;

        #region [Single line properties]
        public ZedGraphControl ZedGraphCtrl { get; set; }
        public SymbolType DefaultSymbolType { get; set; } = SymbolType.Circle;
        public float DefaultSymbolSize { get; set; } = 7;
        public Color DefaultLineColor { get; set; } = Color.Blue;

        public DashStyle DefaultLineDashStyle { get; set; } = DashStyle.Solid;
        public float DefaultLineWidth { get; set; } = 1;

        public Color? CurveColor { get; set; }
        public SymbolType? CurveSymbolType { get; set; }
        public float? CurveSymbolSize { get; set; }
        public DashStyle? CurveDashStyle { get; set; }
        #endregion

        #region [Multi-line properties]
        public string ImageTitle
        {
            get => myPane_.Title.Text;
            set => myPane_.Title.Text = value;
        }

        public string XAxisTitle
        {
            get => myPane_.XAxis.Title.Text;
            set => myPane_.XAxis.Title.Text = value;
        }

        public string YAxisTitle
        {
            get => myPane_.YAxis.Title.Text;
            set => myPane_.YAxis.Title.Text = value;
        }

        public int Width
        {
            get => ZedGraphCtrl.Width;
            set => ZedGraphCtrl.Width = value;
        }

        public int Height
        {
            get => ZedGraphCtrl.Height;
            set => ZedGraphCtrl.Height = value;
        }

        public bool IsLogX
        {
            get => myPane_.XAxis.Type == AxisType.Log;
            set => myPane_.XAxis.Type = value ? AxisType.Log : AxisType.Linear;
        }

        public bool IsLogY
        {
            get => myPane_.YAxis.Type == AxisType.Log;
            set => myPane_.YAxis.Type = value ? AxisType.Log : AxisType.Linear;
        }

        private bool _isSymbolVisible = true;

        public bool IsSymbolVisible
        {
            get => _isSymbolVisible;
            set
            {
                _isSymbolVisible = value;
                foreach (CurveItem curve in myPane_.CurveList)
                {
                    if (curve is LineItem lineItem)
                    {
                        lineItem.Symbol.IsVisible = value;
                    }
                }
                ZedGraphCtrl.Invalidate();
            }
        }

        public bool IsLegendVisible
        {
            get => myPane_.Legend.IsVisible;
            set
            {
                myPane_.Legend.IsVisible = value;
                ZedGraphCtrl.Invalidate();
            }
        }
        #endregion

        #region [ctor]
        public DataPlotter()
        {
            ZedGraphCtrl = new ZedGraphControl();
            ZedGraphCtrl.Size = new Size(1200, 800); //GetDisplayScreenResolution();
            myPane_ = ZedGraphCtrl.GraphPane;            

            ImageTitle = "Image Title";
            XAxisTitle = "X Axis";
            YAxisTitle = "Y Axis";
            IsLogX = false;
            IsLogY = false;
            IsLegendVisible = true;
            IsSymbolVisible = false;
            ZedGraphCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
        }
        #endregion

        #region [AddCurve]
        public void AddCurve(string curveName, List<double> xValues, List<double> yValues, Color color, bool IsSymbolVisible=false, FillType SymbolFillType=FillType.Solid)
        {
            PointPairList list = new PointPairList();
            for (int tau = 0; tau < yValues.Count; tau++)
            {
                list.Add(xValues[tau], yValues[tau]);
            }

            Color curveColor = color; //ColorFromIndex(curveCounter);

            LineItem myCurve = myPane_.AddCurve(curveName, list,
                CurveColor ?? curveColor,
                CurveSymbolType ?? DefaultSymbolType);

            myCurve.Line.IsVisible = true;
            myCurve.Line.Width = DefaultLineWidth;
            myCurve.Line.Style = CurveDashStyle ?? DefaultLineDashStyle;
            myCurve.Symbol.Fill.Type = SymbolFillType;
            myCurve.Symbol.Size = CurveSymbolSize ?? DefaultSymbolSize;
            myCurve.Symbol.IsVisible = IsSymbolVisible;
            myCurve.Line.IsAntiAlias = true;

            BringCurveToFront(myCurve);

            curveCounter_++;

            CurveColor = null;
            CurveSymbolType = null;
            CurveSymbolSize = null;
            CurveDashStyle = null;

            // Call this method to update the scales based on the new settings
            ZedGraphCtrl.AxisChange();

            // Redraw the graph to show the changes
            ZedGraphCtrl.Invalidate();
        }

        public void AddCurve(string curveName, List<double> xValues, List<double> yValues, bool IsSymbolVisible)
        {
            AddCurve(curveName, xValues, yValues, Color.Blue, IsSymbolVisible);
        }

        public void AddCurve(string curveName, List<double> xValues, List<double> yValues, Color color)
        {
            AddCurve(curveName, xValues, yValues, color, true);
        }

        public void AddCurve(string curveName, List<double> xValues, List<double> yValues)
        {
            AddCurve(curveName, xValues, yValues, Color.Blue, true);
        }
        #endregion

        #region [AddErrorBars]
        public void AddErrorBars(string curveName, List<double> xValues, List<double> yValues, List<double> yErrors, Color color)
        {
            if (xValues.Count != yValues.Count || yValues.Count != yErrors.Count)
            {
                throw new ArgumentException("All lists must be of equal length.");
            }

            // Create a point pair list for the error bars
            PointPairList listErrorBars = new PointPairList();
            for (int i = 0; i < xValues.Count; i++)
            {
                double x = xValues[i];
                double y = yValues[i];
                double yError = yErrors[i];
                listErrorBars.Add(x, y + yError, y - yError);
            }

            ErrorBarItem errorBars = myPane_.AddErrorBar("ErrorBars", listErrorBars, Color.Black);

            // Call this method to update the scales based on the new settings
            ZedGraphCtrl.AxisChange();

            // Redraw the graph to show the changes
            ZedGraphCtrl.Invalidate();
        }
        #endregion

        #region [AddPoint]
        public void AddPoint(string curveName, double x, double y, Color color)
        {
            List<double> xValues = new List<double>(new double[] { x });
            List<double> yValues = new List<double>(new double[] { y });
            AddCurve(curveName, xValues, yValues, color, true);
        }
        #endregion

        public void AddVerticalLine(string curveName, double xValue, Color? curveColor = null)
        {
            Color actualCurveColor = curveColor ?? DefaultLineColor;

            if (IsLogX)
            {
                xValue = Math.Pow(10, xValue);
            }
            // IsLogX is true, so xValue becomes 10^2.2 (~158.49)
            // But your XAxis is being displayed from 0.0 to 1.0
            // So the line is plotted correctly, it's just out of scale
            // To fix we just change the scale of the graph
            myPane_.XAxis.Scale.Min = xValue - 10.0d;
            myPane_.XAxis.Scale.Max = xValue + 10.0d;
            ////////////////////////////////////////////////////


            PointPairList list = new PointPairList();
            double minY = myPane_.YAxis.Scale.Min;
            double maxY = myPane_.YAxis.Scale.Max;

            if (myPane_.YAxis.Type == AxisType.Log)
            {
                minY = Math.Max(minY, 1.0);
                maxY = Math.Max(maxY, minY * 100);
            }


            list.Add(xValue, minY);
            list.Add(xValue, maxY);

            LineItem myCurve = myPane_.AddCurve(curveName, list, actualCurveColor, SymbolType.None);

            myCurve.Line.IsVisible = true;
            myCurve.Line.Style = DashStyle.Solid;
            myCurve.Line.Width = 2.0F;

            myCurve.Symbol.IsVisible = false;

            ZedGraphCtrl.Invalidate();
        }

        #region [Zoom Functions for x-axis]

        // This function will zoom to a specific part of x-axis in linear scale
        public void ZoomXaxis(double minX, double maxX)
        {
            if (minX >= maxX)
            {
                throw new ArgumentException("minX must be less than maxX for a proper zoom.");
            }

            // Set the x-axis to linear type
            //myPane_.XAxis.Type = AxisType.Linear;

            // Set the scale for x-axis
            if (IsLogX)
            {
                myPane_.XAxis.Scale.Min = Math.Log10(minX);
                myPane_.XAxis.Scale.Max = Math.Log10(maxX);
            }
            else
            {
                myPane_.XAxis.Scale.Min = minX;
                myPane_.XAxis.Scale.Max = maxX;
            }

            // Refresh the graph to show the changes
            ZedGraphCtrl.AxisChange();
            ZedGraphCtrl.Invalidate();
        }

        #endregion

        #region [Zoom Functions for Y-axis]
        // This function will zoom to a specific part of y-axis in linear scale
        public void ZoomYaxis(double minY, double maxY)
        {
            if (minY >= maxY)
            {
                throw new ArgumentException("minY must be less than maxY for a proper zoom.");
            }
        
            // Set the scale for y-axis
            if (IsLogY)
            {
                myPane_.YAxis.Scale.Min = Math.Log10(minY);
                myPane_.YAxis.Scale.Max = Math.Log10(maxY);
            }
            else
            {
                myPane_.YAxis.Scale.Min = minY;
                myPane_.YAxis.Scale.Max = maxY;
            }

            // Refresh the graph to show the changes
            ZedGraphCtrl.AxisChange();
            ZedGraphCtrl.Invalidate();
        }
        #endregion

        #region [utility functions]
        private Color ColorFromIndex(int index)
        {
            KnownColor[] knownColors = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            List<KnownColor> colorList = new List<KnownColor>();

            foreach (KnownColor knownColor in knownColors)
            {
                Color color = Color.FromKnownColor(knownColor);

                if (!color.IsSystemColor)
                {
                    double luminance = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;

                    const double lightColorThreshold = 160;

                    if (luminance < lightColorThreshold)
                    {
                        colorList.Add(knownColor);
                    }
                }
            }

            if (colorList.Count == 0) return Color.Black;
            return Color.FromKnownColor(colorList[index % colorList.Count]);
        }

        // Method to bring the specified curve to the front
        private void BringCurveToFront(LineItem curve)
        {
            myPane_.CurveList.Remove(curve);
            myPane_.CurveList.Insert(0, curve);
            ZedGraphCtrl.Invalidate();
        }

        public void SavePlot(string outputPath, string outputFileName)
        {
            try
            {
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                string fullPath = Path.Combine(outputPath, outputFileName);

                ZedGraphCtrl.AxisChange();
                ZedGraphCtrl.Invalidate();

                using (Bitmap bmp = new Bitmap(ZedGraphCtrl.Width, ZedGraphCtrl.Height))
                {
                    ZedGraphCtrl.DrawToBitmap(bmp, new Rectangle(0, 0, ZedGraphCtrl.Width, ZedGraphCtrl.Height));
                    bmp.Save(fullPath, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while saving the plot: " + ex.Message);
            }
        }

        //private Size GetDisplayScreenResolution()
        //{
        //    // PrimaryScreen represents the primary display monitor
        //    Rectangle resolution = Screen.PrimaryScreen.Bounds;

        //    // Create a Size structure to hold the width and height of the primary screen
        //    Size screenSize = new Size(resolution.Width, resolution.Height);

        //    return screenSize;
        //}

        public void Clear()
        {
            myPane_.CurveList.Clear();
            ZedGraphCtrl.Invalidate();
        }

        public void ShowDialog()
        {
            DataPlotterForm f = new DataPlotterForm();
            ZedGraphCtrl.Dock = DockStyle.Fill;
            f.Controls.Add(ZedGraphCtrl);
            f.WindowState = FormWindowState.Maximized;
            f.ShowDialog();
        }
        #endregion

        #region [AddText]
        public void AddText(string text, double xValue, double yValue, Color color, float fontSize = 14, bool isBold = false, string fontFamily = "Arial")
        {
            if (IsLogX)
            {
                xValue = Math.Pow(10, xValue);
            }
            // IsLogX is true, so xValue becomes 10^2.2 (~158.49)
            // But your XAxis is being displayed from 0.0 to 1.0
            // So the line is plotted correctly, it's just out of scale
            // To fix we just change the scale of the graph
            myPane_.XAxis.Scale.Min = xValue - 10.0d;
            myPane_.XAxis.Scale.Max = xValue + 10.0d;
            ////////////////////////////////////////////////////

            // Create a text object with the provided parameters
            TextObj textObj = new TextObj(text, xValue, yValue)
            {
                FontSpec = new FontSpec(fontFamily, fontSize, color, isBold, false, false)
                {
                    Border = { IsVisible = false },
                    Fill = new Fill(Color.Empty), // No background
                    StringAlignment = StringAlignment.Center // Centered text
                }
            };

            // AlignH and AlignV are properties of the TextObj, Align the text to the location point
            textObj.Location.AlignH = AlignH.Center;
            textObj.Location.AlignV = AlignV.Center;

            // Rotate the text if needed
            // textObj.FontSpec.Angle = 90;

            // Add the text object to the graph pane
            myPane_.GraphObjList.Add(textObj);

            // Redraw the graph to show the changes
            ZedGraphCtrl.Invalidate();
        }
        #endregion
    }
}
