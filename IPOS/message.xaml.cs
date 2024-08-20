using System;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IPOS
{
    /// <summary>
    /// Interaction logic for errorMessage.xaml
    /// </summary>
    public partial class message : Window
    {
        Brush errorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a41b11"));

        public message(string message, bool isError)
        {
            InitializeComponent();
            setErrorMsg(message, isError);
        }

        private void setErrorMsg(string msg, bool isError)
        {
            msgTitle.Text = msg;
            if (isError)
            {
                msgTitle.Foreground = errorBrush;
            }
            else
            {
                msgTitle.Foreground = Brushes.White;
            }
            SystemSounds.Hand.Play();
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
            DialogResult = true;
            this.Close();
        }
    }
}
