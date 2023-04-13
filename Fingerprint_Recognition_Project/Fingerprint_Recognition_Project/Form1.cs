using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.ROC;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;
using PatternRecognition.FingerprintRecognition.Matchers;
using PatternRecognition.FingerprintRecognition.ResourceProviders;
using ImageProcessingTools;


namespace Fingerprint_Recognition_Project
{
    public partial class Form1 : Form
    {
     
        public Form1()
        {
            InitializeComponent();
        }

        public string score;
        public string qry;
        public string temp;

        private Bitmap Change_Resolution(string file)
        {
            using (Bitmap bitmap = (Bitmap)Image.FromFile(file))
            {
                using (Bitmap newBitmap = new Bitmap(bitmap))
                {
                    newBitmap.SetResolution(500,500);
                    return newBitmap;
                        }
            }
        }



        private void match(string query, string template)
        {
            Change_Resolution(query);
            Change_Resolution(template);
            // Loading fingerprints
            var fingerprintImg1 = ImageLoader.LoadImage(query);
            var fingerprintImg2 = ImageLoader.LoadImage(template);
            //// Building feature extractor and extracting features
            var featExtractor = new PNFeatureExtractor() { MtiaExtractor = new Ratha1995MinutiaeExtractor() };
            var features1 = featExtractor.ExtractFeatures(fingerprintImg1);
            var features2 = featExtractor.ExtractFeatures(fingerprintImg2);

            // Building matcher and matching
            var matcher = new PN();
            double similarity = matcher.Match(features1, features2);
            score = similarity.ToString("0.000");
            MessageBox.Show(similarity.ToString("0.000"));

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName;
                fileName = dlg.FileName;
                qry = fileName;
                pictureBox1.ImageLocation = qry;

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName;
                fileName = dlg.FileName;
                temp= fileName;
                pictureBox2.ImageLocation = temp;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            match(qry, temp);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var myForm = new Form4();
            myForm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Получение текущего изображения из Picture box
            Bitmap currentImage = (Bitmap)pictureBox1.Image;

            // Создание объекта для повернутого изображения
            Bitmap rotatedImage = new Bitmap(currentImage.Width, currentImage.Height);

            // Создание объекта Graphics, который используется для поворота изображения
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Поворот изображения на 180 градусов
                g.TranslateTransform((float)currentImage.Width / 2, (float)currentImage.Height / 2);
                g.RotateTransform(180);
                g.TranslateTransform(-(float)currentImage.Width / 2, -(float)currentImage.Height / 2);
                g.DrawImage(currentImage, new Point(0, 0));
            }

            // Замена текущего изображения на повернутое
            pictureBox1.Image = rotatedImage;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Получение текущего изображения из Picture box
            Bitmap currentImage = (Bitmap)pictureBox2.Image;

            // Создание объекта для повернутого изображения
            Bitmap rotatedImage = new Bitmap(currentImage.Width, currentImage.Height);

            // Создание объекта Graphics, который используется для поворота изображения
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Поворот изображения на 180 градусов
                g.TranslateTransform((float)currentImage.Width / 2, (float)currentImage.Height / 2);
                g.RotateTransform(180);
                g.TranslateTransform(-(float)currentImage.Width / 2, -(float)currentImage.Height / 2);
                g.DrawImage(currentImage, new Point(0, 0));
            }

            // Замена текущего изображения на повернутое
            pictureBox2.Image = rotatedImage;

        }
    }
}
