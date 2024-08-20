using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for EditData.xaml
    /// </summary>
    public partial class EditData : Window
    {
        private rawData data;

        internal rawData UpdatedData { get; private set; }

        private int _drawDis = 300;
        private int MaxDrawDis = 300;
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

        internal EditData(rawData dataIndex, int DrawDis)
        {
            InitializeComponent();
            drawDis.Text = _drawDis.ToString();
            LoadFlagNums();
            data = dataIndex;
            MaxDrawDis = DrawDis;
            InitializeFields();
        }

        private void InitializeFields()
        {
            drawDis.Text = data.drawDis.ToString();
            flags.SelectedValue = data.flags;
            intrchk.IsChecked = data.interior;
            changeTitle.Text = $"Changing: {data.modelName}";
        }

        private void LoadFlagNums()
        {
            int startValue = 1;

            int maxValue = 4194304;

            List<int> values = new List<int>
            {
                0
            };

            while (startValue <= maxValue)
            {
                values.Add(startValue);
                startValue *= 2;
            }

            flags.ItemsSource = values;
            flags.SelectedValue = values[0];
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
            checkIfDrawDisIsOutOfRange(MaxDrawDis);
        }

        private void down_Click(object sender, RoutedEventArgs e)
        {
            DrawDis--;
            checkIfDrawDisIsOutOfRange(MaxDrawDis);
        }

        private void InfFlag_click(object sender, RoutedEventArgs e)
        {
            FlagInf flagInfs = new FlagInf();
            flagInfs.ShowDialog();
        }

        private void drawDis_MouseLeave(object sender, MouseEventArgs e)
        {
            if (drawDis == null)
            {
                return;
            }
            else
            {
                checkIfDrawDisIsOutOfRange(MaxDrawDis);
            }
        }

        private bool checkIfDrawDisIsOutOfRange(int max)
        {
            if (int.TryParse(drawDis.Text, out _drawDis))
            {
                if (_drawDis >= 0 && _drawDis <= max)
                {
                    drawDis.Text = _drawDis.ToString();
                    return true;
                }
                else
                {
                    drawDis.Text = $"{MaxDrawDis}";
                    return false;
                }
            }
            return false;
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkIfDrawDisIsOutOfRange(MaxDrawDis))
            {
                UpdatedData = new rawData
                {
                    modelName = data.modelName,
                    interior = intrchk.IsChecked == true ? true : false,
                    drawDis = int.Parse(drawDis.Text),
                    flags = (int)flags.SelectedValue
                };
                DialogResult = true;
                ApplToAll = appltoall.IsChecked == true ? true : false;
                Close();
            }
        }
    }
}
