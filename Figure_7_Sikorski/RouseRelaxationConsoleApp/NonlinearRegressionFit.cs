using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;


namespace Figure_7_Sikorski
{
    public class NonlinearRegressionCurveFitting
    {
        public static (List<double> XFit, List<double> YFit, List<double> MinimizingPoint) Fit(List<double> xData, List<double> yData)
        {
            // example data
            var xDataDense = new DenseVector(xData.ToArray());
            var yDataDense = new DenseVector(yData.ToArray());

            Vector<double> Model(Vector<double> parameters, Vector<double> x)
            {
                var y = CreateVector.Dense<double>(x.Count);
                for (int i = 0; i < x.Count; i++)
                {
                    y[i] = parameters[0] * Math.Exp(parameters[1] * x[i]);
                }
                return y;
            }

            var start = new DenseVector(new double[] { 1.0, 0.1 });
            var objective = ObjectiveFunction.NonlinearModel(Model, xDataDense, yDataDense);
            var solver = new LevenbergMarquardtMinimizer(maximumIterations: 10000);
            var result = solver.FindMinimum(objective, start);

            Vector<double> points = result.MinimizedValues;

            Vector<double> minimizing = result.MinimizingPoint;

            Console.WriteLine($"Reason for exit: {result.ReasonForExit}");

            return (xData, new List<double>(points.ToArray()), new List<double>(minimizing.ToArray()));
        }
    }
}


