using System.Numerics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OBJ
{
    public partial class Form1 : Form
    {
        private List<Vector3> vertices;
        private List<Tuple<int, int, int>> faces;
        private float rotationX;
        private float rotationY;
        private float rotationZ;

        public Form1()
        {
            InitializeComponent();

            // Initialize the list of vertices and faces
            vertices = new List<Vector3>();
            faces = new List<Tuple<int, int, int>>();

            // Set up initial rotation values
            rotationX = 0.0f;
            rotationY = 0.0f;
            rotationZ = 0.0f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "OBJ Files|*.obj";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;

                // Read the OBJ file and extract the vertices and faces
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(' ');
                        if (parts[0] == "v")
                        {
                            float x = float.Parse(parts[1]);
                            float y = float.Parse(parts[2]);
                            float z = float.Parse(parts[3]);
                            Vector3 vertex = new Vector3(x, y, z);
                            vertices.Add(vertex);
                        }
                        else if (parts[0] == "f")
                        {
                            int v1 = int.Parse(parts[1].Split('/')[0]) - 1;
                            int v2 = int.Parse(parts[2].Split('/')[0]) - 1;
                            int v3 = int.Parse(parts[3].Split('/')[0]) - 1;
                            faces.Add(new Tuple<int, int, int>(v1, v2, v3));
                        }
                    }
                }
                listBox1.Items.Clear();
                foreach (Vector3 vertex in vertices)
                {
                    listBox1.Items.Add(vertex);
                }

                // Redraw the scene
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Clear the background
            e.Graphics.Clear(Color.Black);

            // Apply rotation to the vertices
            List<Vector3> transformedVertices = new List<Vector3>();
            foreach (Vector3 vertex in vertices)
            {
                // Rotate around the X-axis
                float y1 = (float)(vertex.Y * Math.Cos(rotationX) - vertex.Z * Math.Sin(rotationX));
                float z1 = (float)(vertex.Y * Math.Sin(rotationX) + vertex.Z * Math.Cos(rotationX));

                // Rotate around the Y-axis
                float x2 = (float)(vertex.X * Math.Cos(rotationY) - z1 * Math.Sin(rotationY));
                float z2 = (float)(vertex.X * Math.Sin(rotationY) + z1 * Math.Cos(rotationY));

                // Rotate around the Z-axis
                float x3 = (float)(x2 * Math.Cos(rotationZ) - y1 * Math.Sin(rotationZ));
                float y3 = (float)(x2 * Math.Sin(rotationZ) + y1 * Math.Cos(rotationZ));

                transformedVertices.Add(new Vector3(x3, y3, z2));
            }
            // Draw the wireframe of the model
            foreach (Tuple<int, int, int> face in faces)
            {
                Vector3 v1 = transformedVertices[face.Item1];
                Vector3 v2 = transformedVertices[face.Item2];
                Vector3 v3 = transformedVertices[face.Item3];

                Pen pen = new Pen(Color.White, 1.0f);

                e.Graphics.DrawLine(pen, v1.X, v1.Y, v2.X, v2.Y);
                e.Graphics.DrawLine(pen, v2.X, v2.Y, v3.X, v3.Y);
                e.Graphics.DrawLine(pen, v3.X, v3.Y, v1.X, v1.Y);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Update the rotation values based on the trackbar positions
            rotationX = (float)(trackBar1.Value * Math.PI / 180.0);
            rotationY = (float)(trackBar2.Value * Math.PI / 180.0);
            rotationZ = (float)(trackBar3.Value * Math.PI / 180.0);

            // Redraw the scene
            pictureBox1.Invalidate();
        }
    }
}