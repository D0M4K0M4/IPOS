using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IPOS
{
    /// <summary>
    /// Interaction logic for AddLOD.xaml
    /// </summary>
    public partial class AddLOD : Window
    {
        private string srcpth;
        private int _drawDis = 301;
        public string modelName = "";
        public bool lodAdded = false;

        public int DrawDis
        {
            get { return _drawDis; }
            set
            {
                _drawDis = value;
                drawDis.Text = value.ToString();
            }
        }

        public AddLOD(string srcPth)
        {
            InitializeComponent();
            drawDis.Text = _drawDis.ToString();
            srcpth = srcPth;
        }

        private void dragOn(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {

            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void OnHoverClo(object sender, RoutedEventArgs e)
        {
            close_img.Source = new BitmapImage(new Uri(@"/close_hover.png", UriKind.Relative));
        }

        private void LeaveHoverClo(object sender, RoutedEventArgs e)
        {
            close_img.Source = new BitmapImage(new Uri(@"/close.png", UriKind.Relative));
        }

        private void OnHoverSet(object sender, RoutedEventArgs e)
        {
            set_img.Source = new BitmapImage(new Uri(@"/set_onhover.png", UriKind.Relative));
        }

        private void LeaveHoverSet(object sender, RoutedEventArgs e)
        {
            set_img.Source = new BitmapImage(new Uri(@"/set.png", UriKind.Relative));
        }

        private void up_Click(object sender, RoutedEventArgs e)
        {
            DrawDis++;
            checkIfDrawDisIsOutOfRange(3000);
        }

        private void down_Click(object sender, RoutedEventArgs e)
        {
            DrawDis--;
            checkIfDrawDisIsOutOfRange(3000);
        }

        private void drawDis_MouseLeave(object sender, MouseEventArgs e)
        {
            if (drawDis == null)
            {
                return;
            }
            else
            {
                checkIfDrawDisIsOutOfRange(3000);
            }
        }

        private bool checkIfDrawDisIsOutOfRange(int max)
        {
            if (int.TryParse(drawDis.Text, out _drawDis))
            {
                if (_drawDis >= 301 && _drawDis <= max)
                {
                    drawDis.Text = _drawDis.ToString();
                    return true;
                }
                else
                {
                    drawDis.Text = "301";
                    return false;
                }
            }
            return false;
        }

        private void addlod_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dff_diag = new OpenFileDialog
            {
                InitialDirectory = srcpth,
                Filter = "Model file (*.dff)|*.dff",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (dff_diag.ShowDialog() == true)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(dff_diag.FileName);
                addlod.Content = name;
                modelName = name;
                lodAdded = true;
            }
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkIfDrawDisIsOutOfRange(3000)) 
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
