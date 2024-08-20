using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IPOS
{
    public partial class showmap : Window
    {
        private int mapXYsize = 6000; 
        private int mapCanvasSize = 600; 

        Brush titleBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a41b11"));

        //We get the WorldSize that's comes from the parsed fastman .ini file
        public showmap(List<Cartesian> mapCoords, int WorldSize)
        {
            InitializeComponent();
            calcImgSize(WorldSize);
            showOnMap(mapCoords);
        }

        private void calcImgSize(int AlteredSize)
        {
            //We just calculate the map size.
            double mapRatio = (double)mapXYsize / AlteredSize;
            double mapsize = mapCanvasSize * mapRatio;
            int ImgMapSize = (int)mapsize;
            mapIMG.Height = ImgMapSize;
            mapIMG.Width = ImgMapSize;
            Canvas.SetLeft(mapIMG, (mapCanvasSize - ImgMapSize) / 2);
            Canvas.SetTop(mapIMG, (mapCanvasSize - ImgMapSize) / 2);
            mapXYsize = AlteredSize;
        }

        private void showOnMap(List<Cartesian> mapCoords)
        {
            //We put the marks and names by the xy coordinates. 
            if (mapCoords.Count > 0)
            {
                foreach (var coord in mapCoords)
                {
                    Image marker = new Image
                    {
                        Source = new BitmapImage(new Uri("/marker.png", UriKind.Relative)),
                        Width = 20,
                        Height = 20,
                    };

                    TextBlock nameLabel = new TextBlock
                    {
                        Text = coord.name,
                        Foreground = Brushes.Yellow,
                        FontWeight = FontWeights.Bold,
                        FontSize = 12,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 2, 0, 0)
                    };


                    double scale = mapCanvasSize / (double)mapXYsize;

                    double xPos = (coord.x + mapXYsize / 2) * scale;
                    double yPos = (mapCanvasSize / 2 - coord.y * scale);

                    Canvas.SetLeft(marker, xPos - marker.Width / 2);
                    Canvas.SetTop(marker, yPos - marker.Height / 2);

                    nameLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double textWidth = nameLabel.DesiredSize.Width;
                    double textHeight = nameLabel.DesiredSize.Height;

                    Canvas.SetLeft(nameLabel, xPos - textWidth / 2);
                    Canvas.SetTop(nameLabel, yPos + marker.Height / 2);

                    mapCanvas.Children.Add(marker);
                    mapCanvas.Children.Add(nameLabel);
                }
            }
        }

        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {
            SaveMapAsImage();
        }

        private void SaveMapAsImage()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG image|*.png",
                Title = "Save map as image"
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveFileDialog.FileName;

                double width = mapCanvas.ActualWidth;
                double height = mapCanvas.ActualHeight;

                int extraSpace = 50;
                width += extraSpace;
                height += extraSpace;

                RenderTargetBitmap renderTargetMap = new RenderTargetBitmap(
                    (int)width,
                    (int)height,
                    96, 
                    96, 
                    PixelFormats.Pbgra32
                );

                renderTargetMap.Render(mapCanvas);

                PngBitmapEncoder encodepng = new PngBitmapEncoder();
                encodepng.Frames.Add(BitmapFrame.Create(renderTargetMap));

                using (FileStream pngFS = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    encodepng.Save(pngFS);
                }
            }
        }


        private void dragOn(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                // Handle exception
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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
    }
}
