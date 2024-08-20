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
    /// Interaction logic for EditLOD.xaml
    /// </summary>
    public partial class EditLOD : Window
    {
        private int _drawDis = 301;

        public bool ApplToAll { get; private set; }

        public int DrawDis
        {
            get { return _drawDis; }
            set
            {
                _drawDis = value;
                drawDis.Text = value.ToString();
            }
        }

        internal EditLOD(int RawDrawDis)
        {
            InitializeComponent();
            drawDis.Text = RawDrawDis.ToString();
            DrawDis = RawDrawDis;
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

        private void OnHoverChange(object sender, RoutedEventArgs e)
        {
            change_img.Source = new BitmapImage(new Uri(@"/change_onhover.png", UriKind.Relative));
        }

        private void LeaveHoverChange(object sender, RoutedEventArgs e)
        {
            change_img.Source = new BitmapImage(new Uri(@"/change.png", UriKind.Relative));
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

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkIfDrawDisIsOutOfRange(3000))
            {
                ApplToAll = appltoall.IsChecked == true ? true : false;
                DialogResult = true;
                Close();
            }
        }
    }
}
