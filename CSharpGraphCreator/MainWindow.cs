using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CSharpGraphCreator
{
    public partial class MainWindow : Form
    {
        // Create lists to hold vertices and edges and a vertex to hold the last
        // vertex clicked on
        List<Vertex> vertices = new List<Vertex>();
        List<Edge> edges = new List<Edge>();
        Vertex lastClicked;

        // A dictionary to hold the adjacency list of the graph
        Dictionary<Vertex, List<Vertex>> adjacencyList =
            new Dictionary<Vertex, List<Vertex>>();

        // Create window to output the adjacency list
        AdjacencyListOutput output = new AdjacencyListOutput();

        public MainWindow()
        {
            InitializeComponent();
            lastClicked = null;

            // Show adjacency list window
            output.Show();
        }

        private void RegenerateAdjacencyList()
        {
            // Recalculate the adjacency list
            adjacencyList.Clear();

            // Add all vertices and connections to dictionary
            foreach(Vertex vertex in vertices)
            {
                adjacencyList.Add(vertex, new List<Vertex>());

                // Get a list of all the vertices this vertex connects to
                foreach(Edge edge in edges)
                {
                    // See if this edge connects to this vertex
                    if(edge.Contains(vertex))
                    {
                        // If so, add destination to the adjacency list
                        adjacencyList[vertex].Add(edge.Destination(vertex));
                    }
                }
            }

            // Output this to the output window
            // Clear data view
            output.dataGridView.Rows.Clear();

            // Loop through adjency list
            foreach (KeyValuePair<Vertex, List<Vertex>> entry in adjacencyList)
            {
                string[] row = new string[2];

                // Find the position of the initial key and write to file
                for (int i = 0; i < vertices.Count; ++i)
                {
                    if (vertices[i] == entry.Key)
                    {
                        row[0] = Convert.ToString(i);
                        break;
                    }
                }

                // Add all connections to an array for ordering
                List<int> indexes = new List<int>();
                foreach (Vertex vertex in entry.Value)
                {
                    for (int i = 0; i < vertices.Count; ++i)
                    {
                        if (vertices[i] == vertex)
                        {
                            indexes.Add(i);
                            break;
                        }
                    }
                }

                // Sort indexes and output
                indexes.Sort();

                row[1] = "";
                foreach(int index in indexes)
                {
                    row[1] += Convert.ToString(index) + ", ";
                }

                // Add to data grid view
                output.dataGridView.Rows.Add(row);
            }

            /*
            // DEBUG: Output dictionary to text file
            StreamWriter file = new StreamWriter("debug.txt");
            foreach(KeyValuePair<Vertex, List<Vertex>> entry in adjacencyList)
            {
                // Find the position of the initial key and write to file
                for(int i = 0; i < vertices.Count; ++i)
                {
                    if(vertices[i] == entry.Key)
                    {
                        file.Write(i + ": ");
                        break;
                    }
                }

                // Add all connections to an array for ordering
                List<int> indexes = new List<int>();
                foreach(Vertex vertex in entry.Value)
                {
                    for(int i = 0; i < vertices.Count; ++i)
                    {
                        if(vertices[i] == vertex)
                        {
                            indexes.Add(i);
                            break;
                        }
                    }
                }

                // Sort indexes and output
                indexes.Sort();
                foreach (int index in indexes) {
                    file.Write(index + ", ");
                }

                // Write endline character
                file.WriteLine();
            }

            // Close and save file
            file.Close();
            */
        }

        private void DrawBox_MouseDown(object sender, MouseEventArgs e)
        {
            bool regenerate = false;

            if (e.Button == MouseButtons.Left)
            {
                // Create a new vertex at the position of the mouse click
                Vertex newVertex = new Vertex(e.X, e.Y);

                // Check if it is overlapping another vertex
                bool overlapping = false;
                foreach(Vertex vertex in vertices)
                {
                    if(newVertex.Intersects(vertex))
                    {
                        overlapping = true;
                    }
                }

                // Add to vertex list
                if(!overlapping) {
                    vertices.Add(newVertex);
                    regenerate = true;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Variable to hold vertex that collides with mouse click
                Vertex collided = null;

                // Check for a collision
                foreach (Vertex vertex in vertices)
                {
                    // Loop through until a vertex is found under the mouse click
                    if (vertex.Contains(e.X, e.Y))
                    {
                        // Make the collided vertex equal to this
                        collided = vertex;

                        // Stop looping
                        break;
                    }
                }

                // If a rectangle was found under the mouse click
                if (collided != null)
                {
                    // If no other shape has been clicked
                    if (lastClicked == null)
                    {
                        // Make this shape the start of an edge
                        lastClicked = collided;
                        lastClicked.Select();
                    }
                    // lastClicked is different from the collided object
                    else if (lastClicked != collided)
                    {
                        // Create new edge
                        Edge newEdge = new Edge(lastClicked, collided, 5);

                        // Loop through the current edges to see if this edge
                        // already exists on the graph
                        bool alreadyExists = false;
                        foreach(Edge edge in edges)
                        {
                            if(newEdge == edge)
                            {
                                alreadyExists = true;
                                break;
                            }
                        }

                        if(!alreadyExists)
                        {
                            edges.Add(newEdge);
                            regenerate = true;
                        }

                        lastClicked.Deselect();
                        lastClicked = null;
                    }
                    // Same object clicked again
                    else
                    {
                        // Remove lastClicked from
                        lastClicked.Deselect();
                        vertices.Remove(lastClicked);

                        // Remove all edges containing this vector
                        for(int i = 0; i < edges.Count; ) {
                            // See if edge matches
                            if(edges[i].Contains(lastClicked)) {
                                // Remove the edge
                                edges.Remove(edges[i]);
                            }
                            else {
                                // Increment i
                                ++i;
                            }
                        }

                        // Set lastClicked to null
                        lastClicked = null;

                        // Regereate
                        regenerate = true;
                    }
                }
            }

            // Invalidate the display
            DrawBox.Invalidate();

            // Regenerate adjacency list
            if(regenerate) {
                RegenerateAdjacencyList();
            }
        }

        private void DrawBox_Paint(object sender, PaintEventArgs e)
        {
            // Draw all vertices
            foreach (Vertex vertex in vertices)
            {
                vertex.Draw(e.Graphics);
            }

            // Draw all edges
            foreach (Edge edge in edges)
            {
                edge.Draw(e.Graphics);
            }

            // Create font object
            Font drawFont = new System.Drawing.Font("Arial", 11);
            SolidBrush drawBrush  = new SolidBrush(Color.Black);
            StringFormat drawFormat = new StringFormat();

            // Draw labels with the vertex number on for each item in the vertex
            int vertexId = 0;
            foreach(Vertex vertex in vertices)
            {
                // Set text to id
                string text = Convert.ToString(vertexId);

                // Get position of vertex
                int x = (int)vertex.pos.X + 3;
                int y = (int)vertex.pos.Y + 3;

                // Place on form at position of vertex
                e.Graphics.DrawString(text, drawFont, drawBrush, x, y, drawFormat);

                // Increment vertex id
                ++vertexId;
            }
        }
    }
}
