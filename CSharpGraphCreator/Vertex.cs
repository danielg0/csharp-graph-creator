using System.Drawing;

namespace CSharpGraphCreator
{
    class Vertex
    {
        const float WIDTH = 10;

        public PointF pos;
        private RectangleF collisionBox;
        private Pen p;

        public Vertex(float x, float y)
        {
            // Initialise the vertex's properties
            pos = new PointF(x, y);

            // Setup pen
            p = new Pen(Brushes.Black);
            p.Width = 1;

            // Get box position
            collisionBox = new RectangleF(X() - (WIDTH / 2), Y() - (WIDTH / 2), WIDTH, WIDTH);
        }

        public float X()
        {
            return pos.X;
        }

        public float Y()
        {
            return pos.Y;
        }

        public void Select()
        {
            p.Width = 3;
        }

        public void Deselect()
        {
            p.Width = 1;
        }

        public void Draw(Graphics e)
        {
            // Draw the collision rectange with the passed pen
            e.DrawRectangle(p, collisionBox.X, collisionBox.Y, collisionBox.Width, collisionBox.Height);
        }

        public bool Contains(float x, float y)
        {
            return collisionBox.Contains(x, y);
        }

        public bool Intersects(Vertex a)
        {
            return collisionBox.IntersectsWith(a.collisionBox);
        }
    }
}
