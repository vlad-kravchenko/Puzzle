using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Puzzle
{
    public partial class MainWindow : Window
    {
        Random rand = new Random();
        bool taken = false;
        bool cheat = false;
        int prevRow = -1, prevCol = -1;
        int cols = 5, rows = 5;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetupBlocks(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dlg.ShowDialog() != true)
                return;
            string filename = dlg.FileName;

            BitmapImage img = new BitmapImage(new Uri(filename));
            int width = img.PixelWidth - 1;
            int height = img.PixelHeight - 1;

            if (width > height)
            {
                this.Height = 500;
                this.Width = ((double)width / (double)height) * this.Height;
            }
            else
            {
                this.Width = 500;
                this.Height = ((double)height / (double)width) * this.Width;
            }
            this.Height += 100;

            int rowH = height / rows, colW = width / cols;

            GeneralImage.Source = img;

            taken = false;
            cheat = false;
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < cols; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < rows; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            List<Source> sources = new List<Source>();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var rect = new Int32Rect(colW * col, rowH * row, colW, rowH);
                    CroppedBitmap crop = new CroppedBitmap(img, rect);
                    sources.Add(new Source { OriginalCol = col, OriginalRow = row, Img = crop });
                }
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int left = sources.Count(s => s.Taken == false);
                    Source[] temp = new Source[left];
                    sources.Where(s => s.Taken == false).ToList().CopyTo(temp);
                    int id = rand.Next(0, left);
                    Source pic = temp[id];
                    pic.Taken = true;
                    Image image = new Image
                    {
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Stretch = Stretch.Fill,
                        Source = pic.Img,
                        Tag = pic
                    };
                    MainGrid.Children.Add(image);
                    Grid.SetRow(image, row);
                    Grid.SetColumn(image, col);
                }
            }
            Setup.IsEnabled = false;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = 0, col = 0;
            GetClickCoordinates(out row, out col);
            if (row > rows || row < 0 || col > cols || col < 0) return;
            if (!taken)
            {
                prevCol = col;
                prevRow = row;
                taken = true;
            }
            else
            {
                taken = false;
                Image prevImage = MainGrid.Children.Cast<Image>().FirstOrDefault(i => Grid.GetColumn(i) == prevCol && Grid.GetRow(i) == prevRow);
                Image currImage = MainGrid.Children.Cast<Image>().FirstOrDefault(i => Grid.GetColumn(i) == col && Grid.GetRow(i) == row);

                Grid.SetColumn(prevImage, col);
                Grid.SetRow(prevImage, row);

                Grid.SetColumn(currImage, prevCol);
                Grid.SetRow(currImage, prevRow);

                CheckIfEndGame();
            }
        }

        private void CheckIfEndGame()
        {
            bool end = true;
            foreach(var child in MainGrid.Children.Cast<Image>())
            {
                var source = child.Tag as Source;
                if (Grid.GetRow(child) == source.OriginalRow && Grid.GetColumn(child) == source.OriginalCol) { }
                else
                {
                    end = false;
                }
            }
            if (end)
            {
                Setup.IsEnabled = true;
                if (MessageBox.Show("Do you want to start a new game?", "End of game", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    StartGame();
            }
        }

        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            new InputWindow(this).ShowDialog();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                cheat = !cheat;
                if (cheat)
                {
                    foreach (var child in MainGrid.Children.Cast<Image>())
                    {
                        var source = child.Tag as Source;
                        if (Grid.GetRow(child) == source.OriginalRow && Grid.GetColumn(child) == source.OriginalCol) { }
                        else
                        {
                            child.Opacity = 0.3;
                        }
                    }
                }
                else
                {
                    foreach (var child in MainGrid.Children.Cast<Image>())
                    {
                        child.Opacity = 1;
                    }
                }
            }
        }

        private void GetClickCoordinates(out int row, out int col)
        {
            col = row = 0;
            var point = Mouse.GetPosition(MainGrid);
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;
            foreach (var rowDefinition in MainGrid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }
            foreach (var columnDefinition in MainGrid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }
        }
    }
}