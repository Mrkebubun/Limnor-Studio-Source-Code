using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Limnor.Drawing2D
{
	/*********************/
	/* 2d geometry types */
	/*********************/

	public class Point2
	{   /* 2d point */
		public double x;
		public double y;
		public Point2(double x0, double y0)
		{
			x = x0;
			y = y0;
		}
	}

	public class IntPoint2
	{        /* 2d integer point */
		public int x;
		public int y;
	}

	public class Matrix3
	{  /* 3-by-3 matrix */
		public double[][] element;
		public Matrix3()
		{
			element = new double[3][];
			for (int i = 0; i < 3; i++)
			{
				element[i] = new double[3];
			}
		}
	}

	public class Box2d
	{            /* 2d box */
		public Point2 min;
		public Point2 max;
	}


	/*********************/
	/* 3d geometry types */
	/*********************/

	public class Point3
	{   /* 3d point */
		public double x;
		public double y;
		public double z;
	}

	public class IntPoint3
	{        /* 3d integer point */
		public int x;
		public int y;
		public int z;
	}


	public class Matrix4
	{  /* 4-by-4 matrix */
		public double[][] element;
		public Matrix4()
		{
			element = new double[4][];
			for (int i = 0; i < 4; i++)
			{
				element[i] = new double[4];
			}
		}
	}

	public class Box3d
	{            /* 3d box */
		public Point3 min;
		public Point3 max;
	}



	/***********************/
	/* one-argument macros */
	/***********************/

	/* absolute value of a */
	//#define ABS(a)          (((a)<0) ? -(a) : (a))

	/* round a to nearest int */
	//#define ROUND(a)        ((a)>0 ? (int)((a)+0.5) : -(int)(0.5-(a)))

	/* take sign of a, either -1, 0, or 1 */
	//#define ZSGN(a)         (((a)<0) ? -1 : (a)>0 ? 1 : 0)  

	/* take binary sign of a, either -1, or 1 if >= 0 */
	//#define SGN(a)          (((a)<0) ? -1 : 1)

	/* square a */
	//#define SQR(a)          ((a)*(a))       


	/***********************/
	/* two-argument macros */
	/***********************/

	/* find minimum of a and b */
	//#define MIN(a,b)        (((a)<(b))?(a):(b))     

	/* find maximum of a and b */
	//#define MAX(a,b)        (((a)>(b))?(a):(b))     

	/* swap a and b (see Gem by Wyvill) */
	//#define SWAP(a,b)       { a^=b; b^=a; a^=b; }

	/* linear interpolation from l (when a=0) to h (when a=1)*/
	/* (equal to (a*h)+((1-a)*l) */
	//#define LERP(a,l,h)     ((l)+(((h)-(l))*(a)))

	/* clamp the input to the specified range */
	//#define CLAMP(v,l,h)    ((v)<(l) ? (l) : (v) > (h) ? (h) : v)


	/****************************/
	/* memory allocation macros */
	/****************************/

	/* create a new instance of a structure (see Gem by Hultquist) */
	//#define NEWSTRUCT(x)    (struct x *)(malloc((unsigned)sizeof(struct x)))

	/* create a new instance of a type */
	//#define NEWTYPE(x)      (x *)(malloc((unsigned)sizeof(x)))





	/************/
	/* booleans */
	/************/

	//#define TRUE            1
	//#define FALSE           0
	//#define ON              1
	//#define OFF             0
	//typedef int boolean;                  /* boolean data type */
	//typedef boolean flag;                   /* flag data type */
	[StructLayoutAttribute(LayoutKind.Sequential)]
	public class Bezier
	{
		public double x0;
		public double y0;
		public double x1;
		public double y1;
		public double x2;
		public double y2;
		public double x3;
		public double y3;
	}

	public sealed class VectorUtil
	{
		public const int MAXDEPTH = 64;	/*  Maximum depth for recursion */
		public static double EPSILON = (ldexp(1.0, -MAXDEPTH - 1));
		/********************/
		/* useful constants */
		/********************/

		//#define PI              3.141592        /* the venerable pi */
		//#define PITIMES2        6.283185        /* 2 * pi */
		//#define PIOVER2         1.570796        /* pi / 2 */
		//#define E               2.718282        /* the venerable e */
		public const double SQRT2 = 1.414214;        /* sqrt(2) */
		public const double SQRT3 = 1.732051;        /* sqrt(3) */
		public const double GOLDEN = 1.618034;        /* the golden ratio */
		public const double DTOR = 0.017453;        /* convert degrees to radians */
		public const double RTOD = 57.29578;        /* convert radians to degrees */

		const int DEGREE = 3;			/*  Cubic Bezier curve		*/
		const int W_DEGREE = 5;			/*  Degree of eqn to find roots of */
		static double ldexp(double x, int exp)
		{
			return x * Math.Pow(2.0, exp);
		}
		public static double LERP(double a, double l, double h)
		{
			return ((l) + (((h) - (l)) * (a)));
		}
		public static void BU_NearestPointOnCurve(ref Bezier bz, ref double px, ref double py)
		{
			Point2[] bezCurve = new Point2[] {
	            new Point2( bz.x0, bz.y0 ),
	            new Point2( bz.x1, bz.y1 ),
	            new Point2( bz.x2, bz.y2),
	            new Point2( bz.x3, bz.y3 )
            };
			Point2 arbPoint = new Point2(px, py);
			Point2 pointOnCurve;		 /*  Nearest point on the curve */

			/*  Find the closest point */
			pointOnCurve = NearestPointOnCurve(arbPoint, bezCurve);
			px = pointOnCurve.x;
			py = pointOnCurve.y;

		}
		static int SGN(double a)
		{
			return (((a) < 0) ? -1 : 1);
		}

		/*
 *  Bezier : 
 *	Evaluate a Bezier curve at a particular parameter value
 *      Fill in control points for resulting sub-curves if "Left" and
 *	"Right" are non-null.
 * 
 */
		static Point2 BezierEval(Point2[] V, int degree, double t, Point2[] Left, Point2[] Right)
		//int 	degree;		/* Degree of bezier curve	*/
		//Point2 	*V;			/* Control pts			*/
		//double 	t;			/* Parameter value		*/
		//Point2 	*Left;		/* RETURN left half ctl pts	*/
		//Point2 	*Right;		/* RETURN right half ctl pts	*/
		{
			int i, j;		/* Index variables	*/
			Point2[][] Vtemp = new Point2[W_DEGREE + 1][];//[W_DEGREE+1][W_DEGREE+1];
			for (i = 0; i <= W_DEGREE; i++)
			{
				Vtemp[i] = new Point2[W_DEGREE + 1];
			}

			/* Copy control points	*/
			for (j = 0; j <= degree; j++)
			{
				Vtemp[0][j] = V[j];
			}

			/* Triangle computation	*/
			for (i = 1; i <= degree; i++)
			{
				for (j = 0; j <= degree - i; j++)
				{
					Vtemp[i][j] = new Point2(
						(1.0 - t) * Vtemp[i - 1][j].x + t * Vtemp[i - 1][j + 1].x,

						(1.0 - t) * Vtemp[i - 1][j].y + t * Vtemp[i - 1][j + 1].y);
				}
			}

			if (Left != null)
			{
				for (j = 0; j <= degree; j++)
				{
					Left[j] = Vtemp[j][0];
				}
			}
			if (Right != null)
			{
				for (j = 0; j <= degree; j++)
				{
					Right[j] = Vtemp[degree - j][j];
				}
			}

			return (Vtemp[degree][0]);
		}

		/*
	 * CrossingCount :
	 *	Count the number of times a Bezier control polygon 
	 *	crosses the 0-axis. This number is &gt;= the number of roots.
	 *
	 */
		static int CrossingCount(Point2[] V, int degree)
		//Point2	*V;			/*  Control pts of Bezier curve	*/
		//int		degree;			/*  Degreee of Bezier curve 	*/
		{
			int i;
			int n_crossings = 0;	/*  Number of zero-crossings	*/
			int sign, old_sign;		/*  Sign of coefficients	*/

			sign = old_sign = SGN(V[0].y);
			for (i = 1; i <= degree; i++)
			{
				sign = SGN(V[i].y);
				if (sign != old_sign) n_crossings++;
				old_sign = sign;
			}
			return n_crossings;
		}
		/*
		 *  ControlPolygonFlatEnough :
		 *	Check if the control polygon of a Bezier curve is flat enough
		 *	for recursive subdivision to bottom out.
		 *
		 */
		static int ControlPolygonFlatEnough(Point2[] V, int degree)
		//Point2	*V;		/* Control points	*/
		//int 	degree;		/* Degree of polynomial	*/
		{
			int i;			/* Index variable		*/
			double[] distance;		/* Distances from pts to line	*/
			double max_distance_above;	/* maximum of these		*/
			double max_distance_below;
			double error;			/* Precision of root		*/
			//Vector2 	t;			/* Vector from V[0] to V[degree]*/
			double intercept_1,
					intercept_2,
					left_intercept,
					right_intercept;
			double a, b, c;		/* Coefficients of implicit	*/
			/* eqn for line from V[0]-V[deg]*/

			/* Find the  perpendicular distance		*/
			/* from each interior control point to 	*/
			/* line connecting V[0] and V[degree]	*/
			distance = new double[degree + 1]; // (double *)malloc((unsigned)(degree + 1) * 					sizeof(double));
			{
				double abSquared;

				/* Derive the implicit equation for line connecting first *'
				/*  and last control points */
				a = V[0].y - V[degree].y;
				b = V[degree].x - V[0].x;
				c = V[0].x * V[degree].y - V[degree].x * V[0].y;

				abSquared = (a * a) + (b * b);

				for (i = 1; i < degree; i++)
				{
					/* Compute distance from each of the points to that line	*/
					distance[i] = a * V[i].x + b * V[i].y + c;
					if (distance[i] > 0.0)
					{
						distance[i] = (distance[i] * distance[i]) / abSquared;
					}
					if (distance[i] < 0.0)
					{
						distance[i] = -((distance[i] * distance[i]) / abSquared);
					}
				}
			}


			/* Find the largest distance	*/
			max_distance_above = 0.0;
			max_distance_below = 0.0;
			for (i = 1; i < degree; i++)
			{
				if (distance[i] < 0.0)
				{
					max_distance_below = Math.Min(max_distance_below, distance[i]);
				}
				if (distance[i] > 0.0)
				{
					max_distance_above = Math.Max(max_distance_above, distance[i]);
				}
			}
			//free((char *)distance);

			{
				double det, dInv;
				double a1, b1, c1, a2, b2, c2;

				/*  Implicit equation for zero line */
				a1 = 0.0;
				b1 = 1.0;
				c1 = 0.0;

				/*  Implicit equation for "above" line */
				a2 = a;
				b2 = b;
				c2 = c + max_distance_above;

				det = a1 * b2 - a2 * b1;
				dInv = 1.0 / det;

				intercept_1 = (b1 * c2 - b2 * c1) * dInv;

				/*  Implicit equation for "below" line */
				a2 = a;
				b2 = b;
				c2 = c + max_distance_below;

				det = a1 * b2 - a2 * b1;
				dInv = 1.0 / det;

				intercept_2 = (b1 * c2 - b2 * c1) * dInv;
			}

			/* Compute intercepts of bounding box	*/
			left_intercept = Math.Min(intercept_1, intercept_2);
			right_intercept = Math.Max(intercept_1, intercept_2);

			error = 0.5 * (right_intercept - left_intercept);
			if (error < EPSILON)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}


		/*
		 *  ComputeXIntercept :
		 *	Compute intersection of chord from first control point to last
		 *  	with 0-axis.
		 * 
		 */
		static double ComputeXIntercept(Point2[] V, int degree)
		//Point2 	*V;			/*  Control points	*/
		//int		degree; 		/*  Degree of curve	*/
		{
			double XLK, YLK, XNM, YNM, XMK, YMK;
			double det, detInv;
			double S;//, T;
			double X;//, Y;

			XLK = 1.0 - 0.0;
			YLK = 0.0 - 0.0;
			XNM = V[degree].x - V[0].x;
			YNM = V[degree].y - V[0].y;
			XMK = V[0].x - 0.0;
			YMK = V[0].y - 0.0;

			det = XNM * YLK - YNM * XLK;
			detInv = 1.0 / det;

			S = (XNM * YMK - YNM * XMK) * detInv;
			//T = (XLK * YMK - YLK * XMK) * detInv;

			X = 0.0 + XLK * S;
			//Y = 0.0 + YLK * S;

			return X;
		}

		/*
		 *  FindRoots :
		 *	Given a 5th-degree equation in Bernstein-Bezier form, find
		 *	all of the roots in the interval [0, 1].  Return the number
		 *	of roots found.
		 */
		static int FindRoots(Point2[] w, int degree, double[] t, int depth)
		//Point2 	*w;			/* The control points		*/
		//int 	degree;		/* The degree of the polynomial	*/
		//double 	*t;			/* RETURN candidate t-values	*/
		//int 	depth;		/* The depth of the recursion	*/
		{
			int i;
			Point2[] Left = new Point2[W_DEGREE + 1];	/* New left and right 		*/
			Point2[] Right = new Point2[W_DEGREE + 1];	/* control polygons		*/
			int left_count,		/* Solution count from		*/
					right_count;		/* children			*/
			double[] left_t = new double[W_DEGREE + 1];	/* Solutions from kids		*/
			double[] right_t = new double[W_DEGREE + 1];
			for (i = 0; i < Left.Length; i++)
			{
				Left[i] = new Point2(0, 0);
				Right[i] = new Point2(0, 0);
			}
			switch (CrossingCount(w, degree))
			{
				case 0:
					// {	/* No solutions here	*/
					return 0;
				//break;
				//}
				case 1:
					//{	/* Unique solution	*/
					/* Stop recursion when the tree is deep enough	*/
					/* if deep enough, return 1 solution at midpoint 	*/
					if (depth >= MAXDEPTH)
					{
						t[0] = (w[0].x + w[W_DEGREE].x) / 2.0;
						return 1;
					}
					if (ControlPolygonFlatEnough(w, degree) != 0)
					{
						t[0] = ComputeXIntercept(w, degree);
						return 1;
					}
					break;
				//}
			}

			/* Otherwise, solve recursively after	*/
			/* subdividing control polygon		*/
			BezierEval(w, degree, 0.5, Left, Right);
			left_count = FindRoots(Left, degree, left_t, depth + 1);
			right_count = FindRoots(Right, degree, right_t, depth + 1);


			/* Gather solutions together	*/
			for (i = 0; i < left_count; i++)
			{
				t[i] = left_t[i];
			}
			for (i = 0; i < right_count; i++)
			{
				t[i + left_count] = right_t[i];
			}

			/* Send back total number of solutions	*/
			return (left_count + right_count);
		}
		/* returns squared length of input vector */
		static double V2SquaredLength(Point2 a)
		//Vector2 *a;
		{
			return ((a.x * a.x) + (a.y * a.y));
		}
		/*
	 *  NearestPointOnCurve :
	 *  	Compute the parameter value of the point on a Bezier
	 *		curve segment closest to some arbtitrary, user-input point.
	 *		Return the point on the curve at that parameter value.
	 *
	 */
		static Point2 NearestPointOnCurve(Point2 P, Point2[] V)
		//Point2 	P;			/* The user-supplied point	  */
		//Point2 	*V;			/* Control points of cubic Bezier */
		{
			Point2[] w;			/* Ctl pts for 5th-degree eqn	*/
			double[] t_candidate = new double[W_DEGREE];	/* Possible roots		*/
			int n_solutions;		/* Number of roots found	*/
			double t;			/* Parameter value of closest pt*/

			/*  Convert problem to 5th-degree Bezier form	*/
			w = ConvertToBezierForm(P, V);

			/* Find all possible roots of 5th-degree equation */
			n_solutions = FindRoots(w, W_DEGREE, t_candidate, 0);
			//free((char *)w);

			/* Compare distances of P to all candidates, and to t=0, and t=1 */
			{
				double dist, new_dist;
				Point2 p;
				Point2 v = new Point2(0, 0);
				int i;


				/* Check distance to beginning of curve, where t = 0	*/
				dist = V2SquaredLength(V2Sub(P, V[0], v));
				t = 0.0;

				/* Find distances for candidate points	*/
				for (i = 0; i < n_solutions; i++)
				{
					p = BezierEval(V, DEGREE, t_candidate[i], null, null);
					new_dist = V2SquaredLength(V2Sub(P, p, v));
					if (new_dist < dist)
					{
						dist = new_dist;
						t = t_candidate[i];
					}
				}

				/* Finally, look at distance to end point, where t = 1.0 */
				new_dist = V2SquaredLength(V2Sub(P, V[DEGREE], v));
				if (new_dist < dist)
				{
					dist = new_dist;
					t = 1.0;
				}
			}

			/*  Return the point on the curve at parameter value t */
			//printf("t : %4.12f\n", t);
			return (BezierEval(V, DEGREE, t, null, null));
		}
		/* return vector difference c = a-b */
		static Point2 V2Sub(Point2 a, Point2 b, Point2 c)
		//Vector2 *a, *b, *c;
		{
			c.x = a.x - b.x;
			c.y = a.y - b.y;
			return (c);
		}
		static Point2 V2ScaleII(Point2 v, double s)
		//Vector2	*v;
		//double	s;
		{
			Point2 result = new Point2(v.x * s, v.y * s);

			//result.x = v.x * s; result.y = v.y * s;
			return (result);
		}
		/* return the dot product of vectors a and b */
		static double V2Dot(Point2 a, Point2 b)
		//Vector2 *a, *b; 
		{
			return ((a.x * b.x) + (a.y * b.y));
		}

		static double[][] z = new double[][] {	/* [3][4] Precomputed "z" for cubics	*/
	new double[]{1.0, 0.6, 0.3, 0.1},
	new double[]{0.4, 0.6, 0.6, 0.4},
	new double[]{0.1, 0.3, 0.6, 1.0},
    };
		/*
 *  ConvertToBezierForm :
 *		Given a point and a Bezier curve, generate a 5th-degree
 *		Bezier-format equation whose solution finds the point on the
 *      curve nearest the user-defined point.
 */
		static Point2[] ConvertToBezierForm(Point2 P, Point2[] V)
		//Point2 	P;			/* The point to find t for	*/
		//Point2 	*V;			/* The control points		*/
		{
			int i, j, k, m, n, ub, lb;
			//double 	t;			/* Value of t for point P	*/
			int row, column;		/* Table indices		*/
			Point2[] c = new Point2[DEGREE + 1];		/* V(i)'s - P			*/
			Point2[] d = new Point2[DEGREE];		/* V(i+1) - V(i)		*/
			Point2[] w;			/* Ctl pts of 5th-degree curve  */
			double[][] cdTable = new double[3][];// [3][4];		/* Dot product of c, d		*/
			for (i = 0; i < 3; i++)
			{
				cdTable[i] = new double[4];
			}
			for (i = 0; i < c.Length; i++)
			{
				c[i] = new Point2(0, 0);
			}
			for (i = 0; i < d.Length; i++)
			{
				d[i] = new Point2(0, 0);
			}
			/*Determine the c's -- these are vectors created by subtracting*/
			/* point P from each of the control points				*/
			for (i = 0; i <= DEGREE; i++)
			{
				V2Sub(V[i], P, c[i]);
			}
			/* Determine the d's -- these are vectors created by subtracting*/
			/* each control point from the next					*/
			for (i = 0; i <= DEGREE - 1; i++)
			{
				d[i] = V2ScaleII(V2Sub(V[i + 1], V[i], d[i]), 3.0);
			}

			/* Create the c,d table -- this is a table of dot products of the */
			/* c's and d's							*/
			for (row = 0; row <= DEGREE - 1; row++)
			{
				for (column = 0; column <= DEGREE; column++)
				{
					cdTable[row][column] = V2Dot(d[row], c[column]);
				}
			}

			/* Now, apply the z's to the dot products, on the skew diagonal*/
			/* Also, set up the x-values, making these "points"		*/
			//w = (Point2 *)malloc((unsigned)(W_DEGREE+1) * sizeof(Point2));
			w = new Point2[W_DEGREE + 1];
			for (i = 0; i <= W_DEGREE; i++)
			{
				w[i] = new Point2((double)(i) / W_DEGREE, 0.0);
			}

			n = DEGREE;
			m = DEGREE - 1;
			for (k = 0; k <= n + m; k++)
			{
				lb = Math.Max(0, k - m);
				ub = Math.Min(k, n);
				for (i = lb; i <= ub; i++)
				{
					j = k - i;
					w[i + j].y += cdTable[j][i] * z[j][i];
				}
			}

			return (w);
		}

	}
}