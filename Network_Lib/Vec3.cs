using System;

namespace Net
{
    public class Vec3
    {
        #region Variables
        public float x;
        public float y;
        public float z;

        public float sqrMagnitude { get { return (MathF.Pow(x, 2) + MathF.Pow(y, 2) + MathF.Pow(z, 2)); } }
        public Vec3 normalized { get { return new Vec3(x / magnitude, y / magnitude, z / magnitude); } }
        public float magnitude { get { return Magnitude(new Vec3(x, y, z)); } }
        #endregion

        #region constants
        public const float epsilon = 1e-05f;
        #endregion

        #region Default Values
        public static Vec3 Zero { get { return new Vec3(0.0f, 0.0f, 0.0f); } }
        public static Vec3 One { get { return new Vec3(1.0f, 1.0f, 1.0f); } }
        public static Vec3 Forward { get { return new Vec3(0.0f, 0.0f, 1.0f); } }
        public static Vec3 Back { get { return new Vec3(0.0f, 0.0f, -1.0f); } }
        public static Vec3 Right { get { return new Vec3(1.0f, 0.0f, 0.0f); } }
        public static Vec3 Left { get { return new Vec3(-1.0f, 0.0f, 0.0f); } }
        public static Vec3 Up { get { return new Vec3(0.0f, 1.0f, 0.0f); } }
        public static Vec3 Down { get { return new Vec3(0.0f, -1.0f, 0.0f); } }
        public static Vec3 PositiveInfinity { get { return new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); } }
        public static Vec3 NegativeInfinity { get { return new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); } }
        #endregion

        #region Constructors
        public Vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(Vec3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        //public Vec3(Vector3 v3)
        //{
        //    this.x = v3.x;
        //    this.y = v3.y;
        //    this.z = v3.z;
        //}
        //
        //public Vec3(Vector2 v2)
        //{
        //    this.x = v2.x;
        //    this.y = v2.y;
        //    this.z = 0.0f;
        //}
        #endregion

        #region Operators
        // Summary:
        // This operator method is done this way, because comparing floating-point numbers for exact equality can be problematic due to precision limitations.
        // By comparing the squared magnitude difference, it allows for a small tolerance and helps avoid issues with floating-point precision.
        // (Ax - Bx) * (Ay - By) * (Az - Bz) < the value most close to 0
        public static bool operator ==(Vec3 left, Vec3 right)
        {
            float diff_x = left.x - right.x;
            float diff_y = left.y - right.y;
            float diff_z = left.z - right.z;
            float sqrmag = diff_x * diff_x + diff_y * diff_y + diff_z * diff_z;

            return sqrmag < epsilon * epsilon;
        }

        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !(left == right);
        }

        public static Vec3 operator +(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x + rightV3.x, leftV3.y + rightV3.y, leftV3.z + rightV3.z);
        }

        public static Vec3 operator -(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x - rightV3.x, leftV3.y - rightV3.y, leftV3.z - rightV3.z);
        }

        public static Vec3 operator -(Vec3 v3)
        {
            return new Vec3(-1 * v3.x, -1 * v3.y, -1 * v3.z);
        }

        public static Vec3 operator *(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }
        public static Vec3 operator *(float scalar, Vec3 v3)
        {
            return new Vec3(scalar * v3.x, scalar * v3.y, scalar * v3.z);
        }
        public static Vec3 operator /(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x / scalar, v3.y / scalar, v3.z / scalar);
        }

        //    public static implicit operator Vector3(Vec3 v3)
        //    {
        //        return new Vector3(v3.x, v3.y, v3.z);
        //    }
        //
        //    public static implicit operator Vec3(Vector3 v3)
        //    {
        //        return new Vec3(v3.x, v3.y, v3.z);
        //    }
        //
        //    public static implicit operator Vector2(Vec3 v2)
        //    {
        //        return new Vector2(v2.x, v2.y);
        //    }
        #endregion

        #region Functions
        public override string ToString()
        {
            return "X = " + x.ToString() + "   Y = " + y.ToString() + "   Z = " + z.ToString();
        }

        // Summary:
        // The dot product gives the cosine of the angle between the vectors.
        // Dividing the dot product by the product of the magnitudes of the vectors gives the cosine of the angle.
        // Taking the inverse cosine (cos-1) of this value gives the angle in radians.
        // Angle between two vectors using Dot Product.
        // It can be used to calculate:
        // @Trigonometric Calculations: The angle between vectors is essential for trigonometric calculations, such as finding the sine, cosine, and tangent of an angle.
        // @Rotation and Transformation: In computer graphics and robotics, the angle between vectors is crucial for determining how much one vector needs to be rotated to align with another vector. 
        // @Dot Product and Projection: The angle between vectors affects the dot product, which measures how much two vectors point in the same direction.
        // θ = cos^-1 [ (a · b) / (|a| |b|) ]
        // public static float Angle(Vec3 from, Vec3 to)
        // {
        //     float num1 = Dot(from, to);
        //     float num2 = Magnitude(from);
        //     float num3 = Magnitude(to);
        //     float division = num1 / (num2 * num3);
        //
        //     float angleRad = MathF.Acos(division);
        //     float angleDeg = angleRad * MathF.Rad2Deg;
        //
        //     return angleDeg;
        // }

        // Summary:
        // Lets restrict the magnitude (length) of a vector to a specific range.
        public static Vec3 ClampMagnitude(Vec3 vector, float maxLength)
        {
            if (Magnitude(vector) <= maxLength && Magnitude(vector) >= 0)
            {
                return vector;
            }
            else
            {
                return (vector / Magnitude(vector)) * maxLength;
            }
        }

        // Summary:
        // The magnitude of a vector is essentially its length or size in space.
        // It can be used to calculate:
        // @Distance: The magnitude of a vector can represent the distance between two points in space.
        // @Normalization: Normalizing a vector involves dividing it by its magnitude to create a unit vector (a vector with a magnitude of 1) that points in the same direction.
        // @Comparisons: Comparing magnitudes can help you determine which vector is longer or shorter.
        // @Clamping and Scaling: As in your previous question, clamping the magnitude of a vector can help you limit its maximum value while maintaining its direction.
        // @Vector Operations: Magnitude plays a role in various vector operations, such as dot product, cross product, and calculating angles between vectors.
        // sqrt(x^2 + y^2 + z^2)
        public static float Magnitude(Vec3 vector)
        {
            float vectorMagnitudeSum = (vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z);

            return MathF.Sqrt(vectorMagnitudeSum);
        }

        // Summary:
        // Calculates the cross product between two vectors.
        // The cross product of two vectors provides you with a new vector that is perpendicular to the plane defined by the original vectors.
        // It can be used to calculate:
        // @Rotation and Angular Velocity: The cross product is used to find the axis of rotation and angular velocity when dealing with rotational motion.
        // @Determining Area and Volume: In geometry, the magnitude of the cross product of two vectors can be used to calculate the area of a parallelogram or the volume of a parallelepiped defined by those vectors.
        // @3D Transformations: The cross product can be used to generate new coordinate systems and perform rotations.
        // c = a × b = (Ay * Bz - Az * By, Az * Bx - Ax * Bz, Ax * By - Ay * Bx)
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            float xCrossProduct = (a.y * b.z) - (a.z * b.y);
            float yCrossProduct = -((a.x * b.z) - (a.z * b.x));
            float zCrossProduct = (a.x * b.y) - (a.y * b.x);

            Vec3 crossProduct = new Vec3(xCrossProduct, yCrossProduct, zCrossProduct);

            return crossProduct;
        }

        // Summary:
        // The distance between two vectors gives you a measure of how far apart they are in space, and the shortest distance between them.
        // It can be used to calculate:
        // @Optimization: Distance metrics are often used in optimization problems to find the solution that minimizes or maximizes the distance between vectors.
        // @Geometry Transformation: When working with transformations, such as translation, rotation, and scaling, understanding the distance between vectors helps ensure accurate transformations.
        // @Computer Graphics: Calculating distances between points or vertices is crucial for rendering, collision detection, and creating realistic simulations.
        // @Geometry and Spatial Relationships: The distance between vectors provides insights into the spatial arrangement of objects.
        // It helps determine how far apart points or objects are in a multi-dimensional space, which is crucial for understanding their relative positions.
        // sqrt((Ax - Bx)^2 + (Ay - By)^2 + (Az - Bz)^2)
        //    public static float Distance(Vec3 a, Vec3 b)
        //    {
        //        Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        //
        //        return MathF.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        //    }

        // Summary:
        // The Dot product (scalar prodcut) operation produces a scalar value than represents the degree of similarity or correlation between two vectors.
        // It provides information about alignment or orientation of the vectos with respect to each other.
        // It can be used to calculate:
        // @Angle between Vectors: The dot product is used to calculate the cosine of the angle between two vectors.
        // @Projection: The dot product can be used to find the projection of one vector onto another. 
        // @Orthogonality: The dot product helps determine whether two vectors are orthogonal (perpendicular) to each other.
        // @Geometry and Trigonometry: The dot product provides information about the relative lengths of vectors and the cosine of the angle between them.
        // Ax * Bx + Ay * By + Az * Bz
        public static float Dot(Vec3 a, Vec3 b)
        {
            float dotProduct = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);

            return dotProduct;
        }

        // Summary:
        // Interpolates between the points a and b by the interpolant t.
        // Interpolation refers to the process of calculating intermediate values of vectors based on the characteristics or properties of given vectors.
        // The parameter t is clamped to the range[0, 1].
        // It can be used to calculate:
        // @Animation and Motion: In computer graphics and animation, Lerp is used to smoothly interpolate between two keyframes, creating fluid animations for objects, characters, cameras, and more.
        // @Transitions: Lerp is used to create smooth transitions between different states or positions. 
        // a + (b - a) * t
        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            float x = a.x;
            float y = a.y;
            float z = a.z;

            if (t < 1.0f)
            {
                x = a.x + (b.x - a.x) * t;
                y = a.y + (b.y - a.y) * t;
                z = a.z + (b.z - a.z) * t;
            }

            return new Vec3(x, y, z);
        }

        // Summary:
        // Linearly interpolates between two vectors.
        // Interpolation refers to the process of calculating intermediate values of vectors based on the characteristics or properties of given vectors.
        // Interpolates between the vectors a and b by the interpolant t.
        // This is most commonly used to find a point some fraction of the way along a line between two endpoints(e.g.to move an object gradually between those points).
        // When t = 0 returns a.
        // When t = 1 returns b.
        // When t = 0.5 returns the point midway between a and b.
        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            float x = a.x;
            float y = a.y;
            float z = a.z;

            x = a.x + (b.x - a.x) * t;
            y = a.y + (b.y - a.y) * t;
            z = a.z + (b.z - a.z) * t;

            return new Vec3(x, y, z);
        }

        // Summary:
        // Returns a new vector that contains the maximum values from each component.
        public static Vec3 Max(Vec3 a, Vec3 b)
        {
            Vec3 vecMax = new Vec3(0, 0, 0);

            if (a.x > b.x)
            {
                vecMax.x = a.x;
            }
            else
            {
                vecMax.x = b.x;
            }

            if (a.y > b.y)
            {
                vecMax.y = a.y;
            }
            else
            {
                vecMax.y = b.y;
            }

            if (a.z > b.z)
            {
                vecMax.z = a.z;
            }
            else
            {
                vecMax.z = b.z;
            }

            return vecMax;
        }

        // Summary:
        // Returns a new vector that contains the minimum values from each component.
        public static Vec3 Min(Vec3 a, Vec3 b)
        {
            Vec3 vecMin = new Vec3(0, 0, 0);

            if (a.x < b.x)
            {
                vecMin.x = a.x;
            }
            else
            {
                vecMin.x = b.x;
            }

            if (a.y < b.y)
            {
                vecMin.y = a.y;
            }
            else
            {
                vecMin.y = b.y;
            }

            if (a.z < b.z)
            {
                vecMin.z = a.z;
            }
            else
            {
                vecMin.z = b.z;
            }

            return vecMin;
        }

        // Summary:
        // The square magintude is the squared length of the vector and is a more efficient alternative to calculating the actual magnitude or length.
        // It can be used to calculate:
        // @Efficiency: Calculating the square magnitude is computationally cheaper than calculating the actual magnitude (length) of a vector.
        // @Distance Comparison: When comparing distances between vectors, using the square magnitude instead of the magnitude itself is sufficient.
        // @Ray Tracing and Collision Detection: In computer graphics and simulations, the square magnitude is used in ray tracing and collision detection algorithms to determine intersections and distances.
        // x^2 + y^2 + z^2
        public static float SqrMagnitude(Vec3 vector)
        {
            return (MathF.Pow(vector.x, 2) + MathF.Pow(vector.y, 2) + MathF.Pow(vector.z, 2));
        }

        // Summary:
        // Used to calculate the projection of one vector onto another.
        // It calculates a new vector that represents the component of a given vector that lies in the direction of another vector.
        // It can be used to calculate:
        // @Computer Graphics: The vector projection is used in lighting calculations, shading, and determining how light interacts with surfaces.
        // @Optimization: The vector projection is used in optimization problems, where you might want to maximize or minimize a quantity along a specific direction.
        // @Geometry and Trigonometry: The vector projection is essential for calculating distances, angles, and components in geometry and trigonometry. 
        // ( vector . onNormal )            Represents the dot product of vectors vector and onNormal.
        // --------------------- * onNormal
        // ( onNormal . onNormal )          This basically is the same as the square magnitude of vector onNormal
        public static Vec3 Project(Vec3 vector, Vec3 onNormal)
        {
            float division = Dot(vector, onNormal) / Dot(onNormal, onNormal);
            Vec3 vecProjection = division * onNormal;

            return vecProjection;
        }

        // Summary:
        // Calculates the reflection of a vector off a surface with a given normal vector.
        // Dot gives us the angle, and - 2 gives us the double of the angle facing the other way around.
        // By doing this to the normal we can multiply it to get the reflection.
        // It can be used to calculate:
        // @Geometry and Spatial Relationships: Vector reflection is used to determine how a vector changes direction when it encounters a surface.
        // @Ray Tracing and Optics: In computer graphics and optics, vector reflection is used to simulate how light rays interact with surfaces. 
        // @Game Development: In game development, vector reflection is used to model physical interactions, such as the reflection of projectiles or the behavior of billiard balls. 
        // inDirection - 2 * Dot(inDirection, inNormal) * inNormal
        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal)
        {
            Vec3 vec1 = inNormal * -2;
            Vec3 vec2 = vec1 * Dot(inDirection, inNormal);
            Vec3 vec3 = vec2 + inDirection;

            return vec3;
        }

        public void Set(float newX, float newY, float newZ)
        {
            this.x = newX;
            this.y = newY;
            this.z = newZ;
        }

        public void Scale(Vec3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        public void Normalize()
        {
            Set(normalized.x, normalized.y, normalized.z);
        }

        public static Vec3 Normalize(Vec3 vector)
        {
            return new Vec3(vector.x / Magnitude(vector), vector.y / Magnitude(vector), vector.z / Magnitude(vector));
        }
        #endregion

        #region Internals
        public override bool Equals(object other)
        {
            if (!(other is Vec3)) return false;
            return Equals((Vec3)other);
        }

        public bool Equals(Vec3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        #endregion
    }
}