using System;

namespace Figure_7_Sikorski
{
    public class Vec2
    {
        // Properties for X, Y components of the vector.
        public double X { get; set; }
        public double Y { get; set; }

        // Constructor to initialize the vector with X, Y components.
        public Vec2(double x, double y)
        {
            X = x;
            Y = y;
        }

        // Add two vectors.
        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }

        // Subtract two vectors.
        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }

        // Negate a vector.
        public static Vec2 operator -(Vec2 a)
        {
            return new Vec2(-a.X, -a.Y);
        }

        // Multiply vector by a scalar.
        public static Vec2 operator *(Vec2 a, double scalar)
        {
            return new Vec2(a.X * scalar, a.Y * scalar);
        }

        // Multiply scalar by vector (commutative).
        public static Vec2 operator *(double scalar, Vec2 a)
        {
            return a * scalar;
        }

        // Divide vector by a scalar.
        public static Vec2 operator /(Vec2 a, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");
            return new Vec2(a.X / scalar, a.Y / scalar);
        }

        // Calculate dot product of two vectors.
        public static double operator *(Vec2 a, Vec2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        // Check if two vectors are equal.
        public static bool operator ==(Vec2 a, Vec2 b)
        {
            // Handle the case where both are null.
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            // Handle the case where one is null but not both.
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            // Use the instance method for equality.
            return a.Equals(b);
        }

        // Check if two vectors are not equal.
        public static bool operator !=(Vec2 a, Vec2 b)
        {
            // The inequality operator is the negation of the equality operator.
            return !(a == b);
        }

        // Override Equals() method for value comparison.
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Vec2 vec = (Vec2)obj;
            return X == vec.X && Y == vec.Y;
        }

        // Override GetHashCode() method.
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        // Calculate magnitude (length) of the vector.
        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double LengthSquared()
        {
            return X * X + Y * Y;
        }

        // Normalize the vector (make it unit length).
        public Vec2 Normalize()
        {
            double magnitude = Magnitude();
            if (magnitude == 0)
                throw new InvalidOperationException("Cannot normalize a zero vector.");
            return this / magnitude;
        }

        // Calculate distance between two vectors.
        public static double Distance(Vec2 a, Vec2 b)
        {
            return (a - b).Magnitude();
        }

        // Calculate the angle between two vectors in radians.
        public static double Angle(Vec2 a, Vec2 b)
        {
            double dot = a * b;
            double magA = a.Magnitude();
            double magB = b.Magnitude();
            if (magA == 0 || magB == 0)
                throw new InvalidOperationException("Cannot calculate the angle with a zero vector.");
            double cosTheta = dot / (magA * magB);
            cosTheta = Math.Max(-1, Math.Min(1, cosTheta));
            return Math.Acos(cosTheta);
        }

        // Override ToString() for easy debugging and display.
        public override string ToString()
        {
            return String.Format("({0}, {1})", X, Y);
        }
    }
}