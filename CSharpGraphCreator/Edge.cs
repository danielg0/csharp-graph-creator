using System.Drawing;

namespace CSharpGraphCreator
{
    class Edge
    {
        private Vertex start;
        private Vertex end;

        private float weight;
        private Pen p;

        // Constructor
        public Edge(Vertex start, Vertex end, float weight)
        {
            this.start = start;
            this.end = end;
            this.weight = weight;

            p = new Pen(Color.Black);
        }

        // Equality operators
        // Check for the equality of two Edges
        // Note: returns true if a is reverse of b
        public static bool operator== (Edge a, Edge b) {
            return ((a.start == b.start && a.end == b.end) ||
                    (a.start == b.end && a.end == b.start));
        }

        public static bool operator!= (Edge a, Edge b) {
            return !(a == b);
        }

        // Return a bool indicating whether a Vertex is part of this edge
        public bool Contains(Vertex x) {
            return (x == start || x == end);
        }

        // Return the Vertex at the other end to a vertex provided
        public Vertex Destination(Vertex x) {
            // Check this edge contains the passed vertex
            if(this.Contains(x)) {
                if(x == end) {
                    return start;
                }
                else {
                    return end;
                }
            }
            // Else, throw exception
            else {
                throw new System.ArgumentException("Vertex not in Edge", "x");
            }
        }

        // Draw this edge on a provided graphics object
        public void Draw(Graphics e)
        {
            // Draw a line with a given pen between the two vertices
            e.DrawLine(p, start.X(), start.Y(), end.X(), end.Y());
        }

        public float Weight()
        {
            return weight;
        }
    }
}
