﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Mineswipper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;
        public static  int levelToRestart=0;
        Field matrixMine;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            dt.Tick += new EventHandler(dt_Tick);
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EasyLevel(sender, e);
        }
        void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                if (currentTime == "59:59:59")
                    sw.Stop();
                TimeBlock.Text = currentTime;
            }
        }
        private void startbtn_Click(object sender, RoutedEventArgs e)
        {
            sw.Start();
            dt.Start();
        }
        private void EasyLevel(object sender, RoutedEventArgs e)
        {
            sw.Reset();
            this.Height = 435;
            this.Width = 450;
            CountBlock.Text = "50";
            int col = 18, row = 14;
            switch (levelToRestart)
            {
                case 1: Normal.IsEnabled = true; Easy.IsEnabled = false; break;
                case 2: Hard.IsEnabled = true; Easy.IsEnabled = false; break;
            }
            levelToRestart = 0;
            Easy.IsEnabled = false;
            TimeBlock.Text = "00:00:00";
            if (GameBlock.Children.Count != 0)
                CleanGrid();
            CreateGrid(col, row);
        }
        private void NormalLevel(object sender, RoutedEventArgs e)
        {
            sw.Reset();
            TimeBlock.Text = "00:00:00";
            CountBlock.Text = "100";
            switch (levelToRestart)
            {
                case 0: Normal.IsEnabled = false; Easy.IsEnabled = true; break;
                case 2: Hard.IsEnabled = true; Normal.IsEnabled = false; break;
            }
            levelToRestart = 1;
            this.Height = 535;
            this.Width = 650;
            int col = 26, row = 18;
            CleanGrid();
            CreateGrid(col, row);
        }
        private void HardLevel(object sender, RoutedEventArgs e)
        {
            sw.Reset();
            TimeBlock.Text = "00:00:00";
            CountBlock.Text = "150";
            this.Height = 680;
            switch (levelToRestart)
            {
                case 0: Hard.IsEnabled = false; Easy.IsEnabled = true; break;
                case 1: Hard.IsEnabled = false; Normal.IsEnabled = true; break;
            }
            levelToRestart = 2;
            this.Width = 850;
            int col = 34, row = 26;
            CleanGrid();
            CreateGrid(col, row);
        }
        private void CreateGrid(int col, int row)
        {
            GameBlock.Height = row * 25;
            for (int i = 0; i < col; ++i)
                GameBlock.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < row; ++i)
                GameBlock.RowDefinitions.Add(new RowDefinition());
            CreateButtons(col, row, GameBlock);
        }
        private void CreateButtons(int col, int row, Grid name)
        {
            for (int i = 0; i < col; ++i)
                for (int k = 0; k < row; ++k)
                {
                    Button btn = new Button();
                    btn.Style = (Style)Resources["GameBtn"];
                    name.Children.Add(btn);
                    btn.MouseRightButtonDown += ClickRightButton;
                    btn.Click += ClickLeftButton;
                    btn.Click += startbtn_Click;
                    
                    Grid.SetColumn(btn, i);
                    Grid.SetRow(btn, k);
                }
        }
        private void CleanGrid()
        {
            while (GameBlock.Children.Count != 0)
                GameBlock.Children.RemoveAt(0);
            while (GameBlock.ColumnDefinitions.Count != 0)
                GameBlock.ColumnDefinitions.RemoveAt(0);
            while (GameBlock.RowDefinitions.Count != 0)
                GameBlock.RowDefinitions.RemoveAt(0);
        }
        private void ClickLeftButton(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            (int, int) posButton = (Grid.GetColumn(button), Grid.GetRow(button));
            if (ConvertStr(TimeBlock.Text) == 0)
            {
                matrixMine = MineCreate(posButton);
                SearchCounter(ref matrixMine);
                Podskazochka.IsEnabled = true;
            }
            OpenBtn(posButton, sender, e);
            CheckYouWin();
        }
        private void ClickRightButton(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.Content != null)
            {
                button.Content = null;
                CountBlock.Text = Convert.ToString(Convert.ToInt32(CountBlock.Text) + 1);
            }
            else
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("https://ilkrbk.github.io/flag.png"));
                button.Content = img;
                CountBlock.Text = Convert.ToString(Convert.ToInt32(CountBlock.Text) - 1);
            }
            CheckYouWin();
        }
        private bool checkMatrixMine()
        {
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; i++)
                for (int j = 0; j < GameBlock.RowDefinitions.Count; j++)
                    if (matrixMine.cells[i, j].IsOpen == false && matrixMine.cells[i, j].HasMine == false)
                        return false;
            return true;
        }
        private int ConvertStr(string str)
        {
            List<char> list = new List<char>();
            foreach (var item in str)
                list.Add(item);
            list.RemoveAt(list.Count - 3);
            list.RemoveAt(list.Count - 5);
            string result = "";
            foreach (var item in list)
                result += item;
            return Convert.ToInt32(result);
        }
        private Field MineCreate((int, int) posButton)
        {
            Field field= new Field(GameBlock.ColumnDefinitions.Count, GameBlock.RowDefinitions.Count);
            int itemsNum = GameBlock.ColumnDefinitions.Count * GameBlock.RowDefinitions.Count;
            Random rnd = new Random();
            List<int> listWithMines = new List<int>();
            while (listWithMines.Count != Convert.ToInt32(CountBlock.Text))
            {
                int temp = rnd.Next(1, itemsNum);
                int rowPos = ((temp / GameBlock.ColumnDefinitions.Count));
                int colPos = ((temp % GameBlock.ColumnDefinitions.Count));
                if (SearchInList(listWithMines, temp))
                    if (rowPos != posButton.Item2 && colPos != posButton.Item1
                        | rowPos != posButton.Item2 - 1 && colPos != posButton.Item1
                        | rowPos != posButton.Item2 + 1 && colPos != posButton.Item1
                        | rowPos != posButton.Item2 && colPos != posButton.Item1 - 1
                        | rowPos != posButton.Item2 && colPos != posButton.Item1 + 1
                        | rowPos != posButton.Item2 - 1 && colPos != posButton.Item1 - 1
                        | rowPos != posButton.Item2 + 1 && colPos != posButton.Item1 + 1
                        | rowPos != posButton.Item2 + 1 && colPos != posButton.Item1 - 1
                        | rowPos != posButton.Item2 - 1 && colPos != posButton.Item1 + 1)
                    {
                        listWithMines.Add(temp);
                        field.cells[colPos, rowPos].HasMine = true;
                    }
            }
            return field;
        }
        private bool SearchInList(List<int> list, int a)
        {
            foreach (var item in list)
                if (a == item) return false;
            return true;
        }
        private void SearchCounter(ref Field matrix)
        {
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; ++i)
                for (int j = 0; j < GameBlock.RowDefinitions.Count; ++j)
                {
                    int count = 0;
                    if (matrix.cells[i, j].HasMine != true)
                    {
                        int colC = GameBlock.ColumnDefinitions.Count;
                        int rowC = GameBlock.RowDefinitions.Count;
                        if (i - 1 >=0 && j - 1 >= 0 && matrix.cells[i - 1, j - 1].HasMine == true) count++;
                        if (j - 1 >= 0 && matrix.cells[i, j - 1].HasMine == true) count++;
                        if (i - 1 >= 0 && matrix.cells[i - 1, j].HasMine == true) count++;
                        if (j + 1 < rowC && matrix.cells[i, j + 1].HasMine == true) count++;
                        if (i + 1 < colC && matrix.cells[i + 1, j].HasMine == true) count++;
                        if (i + 1 < colC && j + 1 < rowC && matrix.cells[i + 1, j + 1].HasMine == true) count++;
                        if (i - 1 >= 0 && j + 1 < rowC && matrix.cells[i - 1, j + 1].HasMine == true) count++;
                        if (i + 1 < colC && j - 1 >= 0 && matrix.cells[i + 1, j - 1].HasMine == true) count++;
                        matrix.cells[i, j].MinesAround = count;
                    }
                }
        }
        private void Podskazka(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < GameBlock.RowDefinitions.Count; j++)
                {
                    if (matrixMine.cells[i,j].HasMine == false && matrixMine.cells[i, j].IsOpen== true)
                    {
                        if (i-1>=0 && j-1>=0 &&matrixMine.cells[i - 1, j - 1].IsOpen == false && matrixMine.cells[i - 1, j - 1].HasMine == true)
                        {
                            Podskaz(i - 1, j - 1);
                            return;
                        }
                        if (i-1>=0 && j>=0 && matrixMine.cells[i - 1, j].IsOpen == false && matrixMine.cells[i - 1, j].HasMine == true)
                        {
                            Podskaz(i - 1, j );
                            return;
                        }
                        if (i-1>=0 && j+1<=(GameBlock.RowDefinitions.Count-1) &&matrixMine.cells[i - 1, j + 1].IsOpen == false && matrixMine.cells[i - 1, j + 1].HasMine == true)
                        {
                            Podskaz(i - 1, j + 1);
                            return;
                        }
                        if (i>=0 && j+1<=(GameBlock.RowDefinitions.Count-1) &&matrixMine.cells[i , j + 1].IsOpen == false && matrixMine.cells[i , j + 1].HasMine == true)
                        {
                            Podskaz(i , j + 1);
                            return;
                        }
                        if(i+1<=(GameBlock.ColumnDefinitions.Count-1) && j+1<=(GameBlock.RowDefinitions.Count-1) &&matrixMine.cells[i+1 , j + 1].IsOpen == false && matrixMine.cells[i+1 , j + 1].HasMine == true)
                        {
                            Podskaz(i+1 , j + 1);
                            return;
                        }
                        if(i+1<=(GameBlock.ColumnDefinitions.Count-1) && j<=(GameBlock.RowDefinitions.Count-1)&&matrixMine.cells[i+1 , j ].IsOpen == false && matrixMine.cells[i+1 , j ].HasMine == true)
                        {
                            Podskaz(i +1, j );
                            return;
                        }
                        if(i+1<=(GameBlock.ColumnDefinitions.Count-1) && j-1>=0&&matrixMine.cells[i+1 , j -1].IsOpen == false && matrixMine.cells[i+1 , j -1 ].HasMine == true)
                        {
                            Podskaz(i+1 , j -1 );
                            return;
                        }
                        if(i<=(GameBlock.ColumnDefinitions.Count-1) && j-1>=0&&matrixMine.cells[i , j -1].IsOpen == false && matrixMine.cells[i , j -1 ].HasMine == true)
                        {
                            Podskaz(i, j -1 );
                            return;
                        }
                    }
                }
            }
        }
        private void CheckYouWin()
        {
            if (Convert.ToInt32(CountBlock.Text) == 0 && checkMatrixMine())
            {
                Looser win2 = new Looser(true);
                win2.Show();
                this.Close();
            }
        }
        private void Podskaz(int i, int j)
        {
            int count = CheckClickCount((i,j));
            GameBlock.Children.RemoveAt(((i * GameBlock.RowDefinitions.Count) + j) - count);
            matrixMine.cells[i, j].IsOpen = true;
            Image img = new Image();
            CountBlock.Text = Convert.ToString(Convert.ToInt32(CountBlock.Text) - 1);
            img.Source = new BitmapImage(new Uri("https://ilkrbk.github.io/flag.png"));
            GameBlock.Children.Add(img);
            Grid.SetColumn(img, i);
            Grid.SetRow(img, j);
        }
        private void Restart(object sender, RoutedEventArgs e)
        {
            switch (levelToRestart)
            {
                case 0 : EasyLevel(sender,  e); break;
                case 1: NormalLevel( sender,  e); break;
                case 2: HardLevel( sender,  e); break;
            }
        }
        private void OpenBtn((int, int) pos, object sender, RoutedEventArgs e)
        {
            if (matrixMine.cells[pos.Item1, pos.Item2].IsOpen == true)
                return;
            Label text = new Label();
            int count = CheckClickCount(pos);
            GameBlock.Children.RemoveAt(((pos.Item1 * GameBlock.RowDefinitions.Count) + pos.Item2) - count);
            if (matrixMine.cells[pos.Item1, pos.Item2].MinesAround != 0 && matrixMine.cells[pos.Item1, pos.Item2].HasMine != true)
            {
                text.Content = matrixMine.cells[pos.Item1, pos.Item2].MinesAround;
                matrixMine.cells[pos.Item1, pos.Item2].IsOpen = true;
                switch (text.Content)
                {
                    case 1: text.Foreground = new SolidColorBrush(Colors.Lime); break;
                    case 2: text.Foreground = new SolidColorBrush(Colors.Yellow); break;
                    case 3: text.Foreground = new SolidColorBrush(Colors.Ivory); break;
                    case 4: text.Foreground = new SolidColorBrush(Colors.LightSkyBlue); break;
                    case 5: text.Foreground = new SolidColorBrush(Colors.Aquamarine); break;
                    case 6: text.Foreground = new SolidColorBrush(Colors.PeachPuff); break;
                    case 7: text.Foreground = new SolidColorBrush(Colors.Pink); break;
                    case 8: text.Foreground = new SolidColorBrush(Colors.Silver); break;
                    default:GameBlock.Children.Add(text); break;
                }
                text.Style = (Style)Resources["UnderNumber"];
                GameBlock.Children.Add(text);
                Grid.SetColumn(text, pos.Item1);
                Grid.SetRow(text, pos.Item2);
            }
            else if (matrixMine.cells[pos.Item1, pos.Item2].HasMine==true)
                OpenAllMine(sender, e);
            else
            {
                matrixMine.cells[pos.Item1, pos.Item2].IsOpen = true;
                if (pos.Item1 == 0 && pos.Item2 == 0)
                    for (int i = 0; i <= 1; i++)
                        for (int j = 0; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item1 == GameBlock.ColumnDefinitions.Count - 1 && pos.Item2 == GameBlock.RowDefinitions.Count - 1)
                    for (int i = -1; i <= 0; i++)
                        for (int j = -1; j <= 0; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item2 == 0 && pos.Item1 == GameBlock.ColumnDefinitions.Count - 1)
                    for (int i = -1; i <= 0; i++)
                        for (int j = 0; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item1 == 0 && pos.Item2 == GameBlock.RowDefinitions.Count - 1)
                    for (int i = 0; i <= 1; i++)
                        for (int j = -1; j <= 0; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item1 == 0)
                    for (int i = 0; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item2 == 0)
                    for (int i = -1; i <= 1; i++)
                        for (int j = 0; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item1 == GameBlock.ColumnDefinitions.Count - 1)
                    for (int i = -1; i <= 0; i++)
                        for (int j = -1; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item2 == GameBlock.RowDefinitions.Count - 1)
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 0; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
            }
        }
        private void OpenAllMine(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; i++)
                for (int j = 0; j < GameBlock.RowDefinitions.Count; j++)
                {
                    if (matrixMine.cells[i,j].HasMine == true)
                    {
                        (int, int) pos = (i, j);
                        Label text = new Label();
                        text.Background = new ImageBrush(new BitmapImage(new Uri("https://ilkrbk.github.io/mine.png")));
                        GameBlock.Children.Add(text);
                        Grid.SetColumn(text, pos.Item1);
                        Grid.SetRow(text, pos.Item2);
                    }
                }
            Looser win2 = new Looser(false);
            win2.Show();
            this.Close();
        }
        private int CheckClickCount((int, int) pos)
        {
            int count = 0;
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; ++i)
                for (int k = 0; k < GameBlock.RowDefinitions.Count; ++k)
                {
                    if (matrixMine.cells[i, k].IsOpen == true)
                        count++;
                    if (i == pos.Item1 && k == pos.Item2)
                        return count;
                }
            return count;
        }
    }
}
