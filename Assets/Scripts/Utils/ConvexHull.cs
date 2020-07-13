using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class ConvexHull {
    private static double cross(Vector2Int O, Vector2Int A, Vector2Int B) {
        return (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x);
    }

    public static List<Vector2Int> GetConvexHull(List<Vector2Int> points) {
        if (points == null)
            return null;

        if (points.Count <= 1)
            return points;

        int n = points.Count, k = 0;
        List<Vector2Int> H = new List<Vector2Int>(new Vector2Int[2 * n]);

        points.Sort((a, b) =>
            a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x)
        );

        // Build lower hull
        for (int i = 0; i < n; ++i) {
            while (k >= 2 && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                k--;
            H[k++] = points[i];
        }

        // Build upper hull
        for (int i = n - 2, t = k + 1; i >= 0; i--) {
            while (k >= t && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                k--;
            H[k++] = points[i];
        }

        return H.Take(k - 1).ToList();
    }
}