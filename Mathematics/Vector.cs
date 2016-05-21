using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Exceptions;
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Mathematics
{
    public class Vector
    {
        protected double[] values;
        public double[] Values {
            get { return values; }
            set { values = value; }
        }
        public int Length
        {
            get { return this.Values != null? this.Values.Length : 0; }
        }

        public double this[int index]
        {
            get
            {
                return this.Values[index];
            }
            set
            {
                this.Values[index] = value;
            }
        }

        #region Constructor and Destructor
        public Vector(params  double[] xx)
        {
            this.Values = xx;
        }

        public Vector(int length)
        {
            this.Values = new double[length];
        }

        public Vector(int length, double fillwith)
        {
            this.Values = new double[length];
            for (int i = 0; i < length; i++)
            {
                this.Values[i] = fillwith;
            }
        }
        #endregion

        #region Operation
        public static Vector operator -(Vector v1, Vector v2)
        {
            if (v1.Length != v2.Length)
                throw new ArrayDimensionMismatchException("Vectot size not equal");

            Vector nv = new Vector(v1.Length);
            for (int i = 0; i < v1.Length; i++)
                nv[i] = v1[i] - v2[i];

            return nv;
        }
        public static Vector operator +(Vector v1, Vector v2)
        {
            if (v1.Length != v2.Length)
                throw new ArrayDimensionMismatchException("Vectot size not equal");

            Vector nv = new Vector(v1.Length);
            for (int i = 0; i < v1.Length; i++)
                nv[i] = v1[i] + v2[i];

            return nv;
        }
        public static Vector operator *(double scale, Vector v1)
        {
            Vector nv = new Vector(v1.Length);

            for (int i = 0; i < v1.Length; i++)
            {
                nv[i] = v1[i] * scale;
            }

            return nv;
        }
        public static Vector operator *(Vector v1, double scale)
        {
            Vector nv = new Vector(v1.Length);

            for (int i = 0; i < v1.Length; i++)
            {
                nv[i] = v1[i] * scale;
            }

            return nv;
        }
        public static Vector operator /(Vector v1, double denominator)
        {
            Vector nv = new Vector(v1.Length);

            for (int i = 0; i < v1.Length; i++)
            {
                nv[i] = v1[i] / denominator;
            }

            return nv;
        }
        public static implicit operator double[](Vector p1)
        {
            return p1.Values;
        }
        public static implicit operator Vector(double[] p1)
        {
            return new Vector(p1);
        }
        #endregion

        public virtual object Clone()
        {
            Vector newVector = new Vector(this.values.Length);
            for (int i = 0; i < Length; i++)
            {
                newVector[i] = this[i];
            }
            return newVector;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("{ ");
            for (int i = 0; i < Length; i++)
            {
                str.Append(Values[i]);
                if (i != Length - 1)
                {
                    str.Append(", ");
                }
            }
            str.Append(" }");
            return str.ToString();
        }
    }
}
