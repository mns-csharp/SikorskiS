using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Figure_7_Sikorski
{
    public class Vector3
    {
        // Properties for X, Y, Z components of the vector.
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        // A static read-only property representing a zero vector.
        public static Vector3 Zero { get; } = new Vector3(0, 0, 0);

        // Constructor to initialize the vector with X, Y, Z components.
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // Add two vectors.
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            // Null checks for both vectors a and b.
            if (a is null || b is null)
            {
                throw new ArgumentNullException(a is null ? nameof(a) : nameof(b), "Operand cannot be null.");
            }

            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        // Subtract two vectors.
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            // Null checks for both vectors a and b.
            if (a is null || b is null)
            {
                throw new ArgumentNullException(a is null ? nameof(a) : nameof(b), "Operand cannot be null.");
            }

            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        // Negate a vector.
        public static Vector3 operator -(Vector3 a)
        {
            // Null check for vector a.
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a), "Operand cannot be null.");
            }

            return new Vector3(-a.X, -a.Y, -a.Z);
        }

        // Multiply vector by a scalar.
        public static Vector3 operator *(Vector3 a, double scalar)
        {
            // Null check for vector a.
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a), "Operand cannot be null.");
            }

            return new Vector3(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }

        // Multiply scalar by vector (commutative).
        public static Vector3 operator *(double scalar, Vector3 a)
        {
            // Null check for vector a.
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a), "Operand cannot be null.");
            }

            return a * scalar;
        }

        // Divide vector by a scalar.
        public static Vector3 operator /(Vector3 a, double scalar)
        {
            // Null check for vector a.
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a), "Operand cannot be null.");
            }

            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            return new Vector3(a.X / scalar, a.Y / scalar, a.Z / scalar);
        }

        // Calculate dot product of two vectors.
        public static double operator *(Vector3 a, Vector3 b)
        {
            // Null checks for both vectors a and b.
            if (a is null || b is null)
            {
                throw new ArgumentNullException(a is null ? nameof(a) : nameof(b), "Operand cannot be null.");
            }

            return Vector3.Dot(a, b);
        }

        // Calculate Dot product of two Vec3 objects
        public double Dot(Vector3 other) => X * other.X + Y * other.Y + Z * other.Z;

        public static double Dot(Vector3 a, Vector3 b)
        {
            // Null checks for both vectors a and b.
            if (a is null || b is null)
            {
                throw new ArgumentNullException(a is null ? nameof(a) : nameof(b), "Operand cannot be null.");
            }

            // Calculate the dot product.
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        // Calculate cross product of two vectors.
        public static Vector3 operator %(Vector3 a, Vector3 b)
        {
            // Null checks for both vectors a and b.
            if (a is null || b is null)
            {
                throw new ArgumentNullException(a is null ? nameof(a) : nameof(b), "Operand cannot be null.");
            }

            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
        }

        // Check if two vectors are equal.
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            // If both are null, return true.
            if (a is null && b is null)
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (a is null || b is null)
            {
                return false;
            }

            // Return true if the fields match:
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        // Check if two vectors are not equal.
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        // Override Equals() method for value comparison.
        public override bool Equals(object obj)
        {
            // Instead of direct type check, use 'as' to allow nulls
            Vector3 vec = obj as Vector3;
            return vec != null && X == vec.X && Y == vec.Y && Z == vec.Z;
        }

        // Override GetHashCode() method.
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }

        // Calculate magnitude (length) of the vector.
        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        // Normalize the vector (make it unit length).
        public Vector3 Normalize()
        {
            double magnitude = Magnitude();
            if (magnitude == 0)
                throw new InvalidOperationException("Cannot normalize a zero vector.");
            return this / magnitude;
        }

        // Calculate distance between two vectors.
        public static double Distance(Vector3 a, Vector3 b)
        {
            if (a is null || b is null)
            {
                throw new ArgumentNullException(a is null ? nameof(a) : nameof(b), "Operand cannot be null.");
            }
            return (a - b).Magnitude();
        }

        // Calculate the angle between two vectors in radians.
        public static double Angle(Vector3 a, Vector3 b)
        {
            if (a is null || b is null)
            {
                throw new ArgumentNullException(a is null ? nameof(a) : nameof(b), "Operand cannot be null.");
            }
            double dot = a * b;
            double magA = a.Magnitude();
            double magB = b.Magnitude();
            // Ensure no division by zero
            if (magA == 0 || magB == 0)
                throw new InvalidOperationException("Cannot calculate the angle with a zero vector.");
            double cosTheta = dot / (magA * magB);
            // Ensure the value is within -1 and 1 to account for any doubleing point errors
            cosTheta = Math.Max(-1, Math.Min(1, cosTheta));
            return Math.Acos(cosTheta);
        }

        // Squared norm of the vector
        public double NormSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public static double DistanceSquared(Vector3 v1, Vector3 v2)
        {
            double dx = v1.X - v2.X;
            double dy = v1.Y - v2.Y;
            double dz = v1.Z - v2.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        // Override ToString() for easy debugging and display.
        public override string ToString()
        {
            // Using String.Format instead of string interpolation for compatibility.
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }

        public static (List<double> lags, List<double> autocorrelationValues) AutoCorrelationVec3(List<Vector3> data, double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        {
            int n = data.Count;
            Vector3 mean = new Vector3(0, 0, 0);
            double variance = 0;

            // Calculate the mean vector
            foreach (Vector3 vec in data)
            {
                mean.X += vec.X;
                mean.Y += vec.Y;
                mean.Z += vec.Z;
            }

            mean.X /= n;
            mean.Y /= n;
            mean.Z /= n;

            // Pre-calculate squared differences from the mean for each data point
            Vector3[] diff = new Vector3[n];
            for (int i = 0; i < n; i++)
            {
                diff[i] = data[i] - mean;
                variance += diff[i].Dot(diff[i]);//square
            }
            variance /= n;

            // Use a concurrent collection to hold the results
            ConcurrentBag<(double lag, double autocorrelation)> results = new ConcurrentBag<(double lag, double autocorrelation)>();

            // Use Parallel.For for parallel computation
            Parallel.For(0, Math.Min(numLag, n), t =>
            {
                double autocorrelation = 0;
                for (int i = 0; i < n - t; i++)
                {
                    autocorrelation += diff[i].Dot(diff[i + t]);
                }
                autocorrelation /= ((n - t) * variance);

                if (autocorrelation >= thresholdMin && autocorrelation <= thresholdMax)
                {
                    results.Add((t, autocorrelation));
                }
            });

            // Sort the results by lag
            var sortedResults = results.OrderBy(r => r.lag).ToList();

            // Extract lags and autocorrelationValues from the sorted results
            List<double> lags = sortedResults.Select(r => r.lag).ToList();
            List<double> autocorrelationValues = sortedResults.Select(r => r.autocorrelation).ToList();

            return (lags, autocorrelationValues);
        }

        //// No change needed for CalculateMeanVector as it's a simple accumulation
        //private static Vec3 CalculateMeanVector(List<Vec3> data)
        //{
        //    Vec3 sum = Vec3.Zero;
        //    foreach (Vec3 vec in data)
        //    {
        //        sum += vec;
        //    }
        //    return sum / data.Count;
        //}

        //// Optimized variance calculation to use Parallel.For loop
        //private static double CalculateVariance(List<Vec3> data, Vec3 meanVector)
        //{
        //    double sumOfSquaredDistances = 0.0;
        //    object lockObject = new object();

        //    Parallel.For(0, data.Count, () => 0.0, (i, loopState, subtotal) =>
        //    {
        //        subtotal += Vec3.DistanceSquared(data[i], meanVector);
        //        return subtotal;
        //    },
        //    (subtotal) =>
        //    {
        //        lock (lockObject)
        //        {
        //            sumOfSquaredDistances += subtotal;
        //        }
        //    });

        //    double meanOfSquaredDistances = sumOfSquaredDistances / data.Count;
        //    double meanMagnitudeSquared = Vec3.DistanceSquared(meanVector, Vec3.Zero);
        //    return meanOfSquaredDistances - meanMagnitudeSquared;
        //}

        //public static (List<double> lags, List<double> autocorrelationValues) AutoCorrelationVec3(List<Vec3> data, double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        //{
        //    Vec3 meanVector = CalculateMeanVector(data);
        //    double variance = CalculateVariance(data, meanVector);
        //    List<double> lags = new List<double>();
        //    List<double> autocorrelation = new List<double>();

        //    // Pre-calculate adjusted vectors to avoid redundant computation
        //    Vec3[] adjustedData = data.Select(vec => vec - meanVector).ToArray();

        //    // Use ConcurrentBag to handle parallel addition of elements
        //    ConcurrentBag<(double lag, double autocorr)> results = new ConcurrentBag<(double lag, double autocorr)>();

        //    Parallel.For(0, numLag + 1, t =>
        //    {
        //        double sum = 0f;
        //        for (int i = 0; i < data.Count - t; i++)
        //        {
        //            sum += Vec3.Dot(adjustedData[i], adjustedData[i + t]);
        //        }
        //        double autocorrelationValue = sum / ((data.Count - t) * variance);
        //        if (autocorrelationValue >= thresholdMin && autocorrelationValue <= thresholdMax)
        //        {
        //            results.Add((t, autocorrelationValue));
        //        }
        //    });

        //    // Order results and populate lags and autocorrelation lists
        //    var orderedResults = results.OrderBy(result => result.lag).ToList();
        //    foreach (var result in orderedResults)
        //    {
        //        lags.Add(result.lag);
        //        autocorrelation.Add(result.autocorr);
        //    }

        //    return (lags, autocorrelation);
        //}

        //// Optimized autocorrelation calculation to use Parallel.For loop
        //public static (List<double> lags, List<double> autocorrelationValues) AutoCorrelationVec3(List<Vec3> data, double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        //{
        //    Vec3 meanVector = CalculateMeanVector(data);
        //    double variance = CalculateVariance(data, meanVector);
        //    List<double> lags = new List<double>(new double[numLag + 1]);
        //    List<double> autocorrelation = new List<double>(new double[numLag + 1]);

        //    Parallel.For(0, numLag + 1, t =>
        //    {
        //        double sum = 0f;
        //        for (int i = 0; i < data.Count - t; i++)
        //        {
        //            Vec3 adjustedVecI = data[i] - meanVector;
        //            Vec3 adjustedVecIT = data[i + t] - meanVector;
        //            sum += Vec3.Dot(adjustedVecI, adjustedVecIT);
        //        }
        //        double autocorrelationValue = sum / ((data.Count - t) * variance);
        //        if (autocorrelationValue >= thresholdMin && autocorrelationValue <= thresholdMax)
        //        {
        //            lags[t] = t;
        //            autocorrelation[t] = autocorrelationValue;
        //        }
        //    });

        //    return (lags, autocorrelation);
        //}

        //// Function to compute the mean of a dataset of Vec3
        //private static Vec3 ComputeMean(Vec3[] dataset)
        //{
        //    Vec3 sum = Vec3.Zero;
        //    foreach (Vec3 v in dataset)
        //    {
        //        sum += v;
        //    }
        //    return sum / dataset.Length;
        //}

        //// Function to compute the variance of the magnitude squared of end-to-end distances
        //private static double ComputeVariance(Vec3[] dataset, Vec3 mean)
        //{
        //    double variance = 0f;
        //    foreach (Vec3 v in dataset)
        //    {
        //        double distanceSquared = (v - mean).NormSquared();
        //        variance += distanceSquared;
        //    }
        //    variance /= dataset.Length;
        //    return variance;
        //}

        //// Autocorrelation function for a 3D vector dataset, optimized for large datasets
        //public static (double[] lags, double[] autocorrelationValues) AutoCorrelationVec3(List<Vec3> dataset, double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        //{
        //    if (dataset == null || dataset.Count == 0)
        //        throw new ArgumentException("Dataset cannot be null or empty.");

        //    Vec3 mean = ComputeMean(dataset.ToArray());
        //    double variance = ComputeVariance(dataset.ToArray(), mean);
        //    int n = dataset.Count;

        //    // Adjust the maximum number of lags to the number of data points if it's larger than the dataset
        //    numLag = Math.Min(numLag, n);

        //    double[] lags = new double[numLag];
        //    double[] autocorr = new double[numLag];

        //    // Pre-compute mean-adjusted vectors to avoid redundant computations
        //    Vec3[] meanAdjusted = new Vec3[n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        meanAdjusted[i] = dataset[i] - mean;
        //    }

        //    Parallel.For(0, numLag, t =>
        //    {
        //        double sum = 0f;
        //        for (int i = 0; i < n - t; i++)
        //        {
        //            sum += Vec3.Dot(meanAdjusted[i], meanAdjusted[i + t]);
        //        }
        //        double autocorrValue = sum / ((n - t) * variance);
        //        if (autocorrValue >= thresholdMin && autocorrValue <= thresholdMax)
        //        {
        //            lags[t] = t;
        //            autocorr[t] = autocorrValue;
        //        }
        //    });

        //    // Filter out the zero-initialized elements due to thresholding
        //    var validIndices = Enumerable.Range(0, numLag).Where(i => autocorr[i] != 0).ToArray();
        //    double[] filteredLags = validIndices.Select(i => lags[i]).ToArray();
        //    double[] filteredAutocorr = validIndices.Select(i => autocorr[i]).ToArray();

        //    return (filteredLags, filteredAutocorr);
        //}

        // Autocorrelation function for a 3D vector dataset
        //public static (List<double> lags, List<double> autocorrelationValues) AutoCorrelationVec3(List<Vec3> dataset, double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        //{
        //    if (dataset == null || dataset.Count == 0)
        //        throw new ArgumentException("Dataset cannot be null or empty.");

        //    Vec3 mean = ComputeMean(dataset.ToArray());
        //    double variance = ComputeVariance(dataset.ToArray(), mean);
        //    int n = dataset.Count;
        //    double[] lags = new double[n];
        //    double[] autocorr = new double[n];

        //    // Pre-compute mean-adjusted vectors to avoid redundant computations
        //    Vec3[] meanAdjusted = new Vec3[n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        meanAdjusted[i] = dataset[i] - mean;
        //    }

        //    for (int t = 0; t < numLag && t < n; t++) // t is the lag
        //    {
        //        double sum = 0f;
        //        for (int i = 0; i < n - t; i++)
        //        {                    
        //            sum += Vec3.Dot(meanAdjusted[i], meanAdjusted[i + t]);
        //        }
        //        double autocorrValue = sum / ((n - t) * variance);
        //        if (autocorrValue >= thresholdMin && autocorrValue <= thresholdMax)
        //        {
        //            lags[t] = t;
        //            autocorr[t] = autocorrValue;
        //        }
        //    }

        //    return (new List<double>(lags), new List<double>(autocorr));
        //}


        // Calculate the mean (average) vector from a list of vectors.
        //public static Vec3 Mean(List<Vec3> list)
        //{
        //    if (list == null)
        //    {
        //        throw new ArgumentNullException(nameof(list), "List cannot be null.");
        //    }

        //    if (list.Count == 0)
        //    {
        //        throw new ArgumentException("List cannot be empty.", nameof(list));
        //    }

        //    double sumX = 0, sumY = 0, sumZ = 0;
        //    foreach (Vec3 vec in list)
        //    {
        //        if (vec == null)
        //        {
        //            throw new ArgumentException("List cannot contain null elements.", nameof(list));
        //        }
        //        sumX += vec.X;
        //        sumY += vec.Y;
        //        sumZ += vec.Z;
        //    }

        //    return new Vec3(sumX / list.Count, sumY / list.Count, sumZ / list.Count);
        //}

        //public static (List<double> lags, List<double> autocorrelationValues) AutoCorrelationVec3(List<Vec3> vec3List, double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        //{
        //    var autocorrelationValues = new List<double>();
        //    var lags = new List<double>();
        //    int n = vec3List.Count;

        //    // Subtract mean and prepare data for autocorrelation calculation
        //    var centeredData = new List<Vec3>();
        //    Vec3 vec_mean = Mean(vec3List);
        //    for (int i = 0; i < n; i++)
        //    {
        //        centeredData.Add(vec3List[i] - vec_mean);
        //    }
        //    // Calculate variance
        //    double varData = 0;
        //    for (int i = 0; i < n; i++)
        //    {
        //        varData += Vec3.Dot(centeredData[i], centeredData[i]);
        //    }
        //    varData = (n - 1) > 0 ? varData / (n - 1) : 1.0;


        //    // Calculate autocorrelation for each lag
        //    for (int t = 0; t < numLag && t < n; t++)
        //    {
        //        double sumProduct = 0.0;
        //        for (int i = 0; i < n - t; i++)
        //        {
        //            sumProduct += Vec3.Dot(centeredData[i], centeredData[i+t]);
        //        }

        //        double autocorrValue = sumProduct / (n - t);
        //        //Normalize by variance and check thresholds
        //        autocorrValue /= varData;
        //        if (autocorrValue >= thresholdMin && autocorrValue <= thresholdMax)
        //        {
        //            autocorrelationValues.Add(autocorrValue);
        //            lags.Add(t);
        //        }
        //    }
        //    return (lags, autocorrelationValues);

        //        public static (List<double> tau, List<double> autoCorrelation) Autocorrelation(List<Vec3> vec3List, int maxLag, int lag, bool normAutocorrFlag)
        //        {
        //            int n = vec3List.Count;
        //            List<double> tau = new List<double>();
        //            List<double> autocorrelationValues = new List<double>();
        //
        //            for (int k = 0; k < maxLag; k++) // Compute autocorrelation up to maxLag
        //            {
        //                tau.Add(k);
        //
        //                double dotProductSum = 0;
        //                double normSquaredSum1 = 0;
        //                double normSquaredSum2 = 0;
        //
        //                for (int t = 0; t < n - k; t++) // Iterate over all elements that have a pair k steps ahead
        //                {
        //                    Vec3 v1 = vec3List[t];
        //                    Vec3 v2 = vec3List[t + k];
        //
        //                    dotProductSum += Vec3.Dot(v1, v2);
        //                    if (normAutocorrFlag) {
        //                        normSquaredSum1 += v1.NormSquared();
        //                        normSquaredSum2 += v2.NormSquared();
        //                    }
        //                    
        //                }
        //
        //                //double normalizedAutocorrelation = (n - k) > 0 ? dotProductSum / Math.Sqrt(normSquaredSum1 * normSquaredSum2) : 0;
        //                double normalizedAutocorrelation;
        //                if (normAutocorrFlag) {
        //                    normalizedAutocorrelation = (n - k) > 0 ? dotProductSum / (Math.Sqrt(normSquaredSum1 * normSquaredSum2) * (n - k)) : 0;
        //                } else normalizedAutocorrelation = (n - k) > 0 ? dotProductSum / (n - k) : 0;
        //                //double normalizedAutocorrelation = (n - k) > 0 ? dotProductSum / (Math.Sqrt(normSquaredSum1 * normSquaredSum2)*(n-k)) : 0;
        //
        //                autocorrelationValues.Add(normalizedAutocorrelation);
        //            }
        //
        //            return (tau, autocorrelationValues);
        //}

        
    }
}