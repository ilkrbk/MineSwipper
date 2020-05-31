using System;
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
        //string[,] matrixMine;
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
                currentTime = String.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
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
            TimeBlock.Text = "00:00";
            if (GameBlock.Children.Count != 0)
            {
                CleanGrid();
            }
            CreateGrid(col, row);
        }
        private void NormalLevel(object sender, RoutedEventArgs e)
        {
            sw.Reset();
            TimeBlock.Text = "00:00";
            CountBlock.Text = "100";
            this.Height = 535;
            this.Width = 650;
            int col = 26, row = 18;
            CleanGrid();
            CreateGrid(col, row);
        }
        private void HardLevel(object sender, RoutedEventArgs e)
        {
            sw.Reset();
            TimeBlock.Text = "00:00";
            CountBlock.Text = "150";
            this.Height = 680;
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
                //matrixMine = MineCreate(posButton);
                matrixMine = MineCreate(posButton);
                SearchCounter(ref matrixMine);
                
            }
           

            
         OpenBtn(posButton, sender, e);
            
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
                img.Source = new BitmapImage(new Uri("https://img2.freepng.ru/20181116/jkb/kisspng-vector-graphics-clip-art-stock-illustration-monkey-png-5bee729ce16496.8767947215423535649232.jpg"));
                button.Content = img;
                CountBlock.Text = Convert.ToString(Convert.ToInt32(CountBlock.Text) - 1);
            }

            if (Convert.ToInt32(CountBlock.Text) == 0 && checkMatrixMine())
            {
                MessageBox.Show("Вы выйграли");
                this.Close();
            }
            (int, int) posButton = (Grid.GetColumn(button), Grid.GetRow(button));
        }
        private bool checkMatrixMine()
        {
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < GameBlock.RowDefinitions.Count; j++)
                {
                    if (matrixMine.cells[i, j].MinesAround != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int ConvertStr(string str)
        {
            List<char> list = new List<char>();
            foreach (var item in str)
                list.Add(item);
            list.RemoveAt(list.Count - 3);
            string result = "";
            foreach (var item in list)
            {
                result += item;
            }

            return Convert.ToInt32(result);
        }

        private Field MineCreate((int, int) posButton)
        {
            //string[,] field = new string[GameBlock.ColumnDefinitions.Count, GameBlock.RowDefinitions.Count];
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
            {
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
        }

        private void Podskazka(object sender, RoutedEventArgs e)
        {
            
            
            
        }
        private void Restart(object sender, RoutedEventArgs e)
        {
            
            
            
        }
        

        private void OpenBtn((int, int) pos, object sender, RoutedEventArgs e)
        {
           
            if (matrixMine.cells[pos.Item1, pos.Item2].IsOpen == true)
                return;
            
            Label text = new Label();
            int count = CheckClickCount(pos);
            //MessageBox.Show($"{pos.Item1} = {pos.Item2} = {count} ==== {((pos.Item1 * GameBlock.RowDefinitions.Count) + pos.Item2) - count}");
            GameBlock.Children.RemoveAt(((pos.Item1 * GameBlock.RowDefinitions.Count) + pos.Item2) - count);
            if (matrixMine.cells[pos.Item1, pos.Item2].MinesAround != 0 && matrixMine.cells[pos.Item1, pos.Item2].HasMine != true)
            {
                text.Content = matrixMine.cells[pos.Item1, pos.Item2].MinesAround;
                //matrixMine.cells[pos.Item1, pos.Item2].MinesAround = -1;
                matrixMine.cells[pos.Item1, pos.Item2].IsOpen = true;
                switch (text.Content)
                {
                    case "1": text.Foreground = new SolidColorBrush(Colors.Lime); break;
                    case "2": text.Foreground = new SolidColorBrush(Colors.Yellow); break;
                    case "3": text.Foreground = new SolidColorBrush(Colors.Ivory); break;
                    case "4": text.Foreground = new SolidColorBrush(Colors.LightSkyBlue); break;
                    case "5": text.Foreground = new SolidColorBrush(Colors.Aquamarine); break;
                    case "6": text.Foreground = new SolidColorBrush(Colors.PeachPuff); break;
                    case "7": text.Foreground = new SolidColorBrush(Colors.Pink); break;
                    case "8": text.Foreground = new SolidColorBrush(Colors.Silver); break;
                }
                text.Style = (Style)Resources["UnderNumber"];
                GameBlock.Children.Add(text);
                Grid.SetColumn(text, pos.Item1);
                Grid.SetRow(text, pos.Item2);
            }
            else if (matrixMine.cells[pos.Item1, pos.Item2].HasMine==true)
            {
                OpenAllMine();
            }
            else
            {
                matrixMine.cells[pos.Item1, pos.Item2].IsOpen = true;
                if (pos.Item1 == 0 && pos.Item2 == 0)
                    for (int i = 0; i <= 1; i++)
                        for (int j = 0; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item1 == 0)
                    for (int i = 0; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item2 == 0)
                    for (int i = -1; i <= 1; i++)
                        for (int j = 0; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item1 == GameBlock.ColumnDefinitions.Count - 1 && pos.Item2 == GameBlock.RowDefinitions.Count - 1)
                    for (int i = -1; i <= 0; i++)
                        for (int j = -1; j <= 0; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item1 == GameBlock.ColumnDefinitions.Count - 1)
                    for (int i = -1; i <= 0; i++)
                        for (int j = -1; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item2 == GameBlock.RowDefinitions.Count - 1)
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 0; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item2 == 0 && pos.Item1 == GameBlock.ColumnDefinitions.Count - 1)
                    for (int i = -1; i <= 0; i++)
                        for (int j = -1; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else if (pos.Item1 == 0 && pos.Item2 == GameBlock.RowDefinitions.Count - 1)
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 0; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                else
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                            OpenBtn((pos.Item1 + i, pos.Item2 + j), sender, e);
                
            }
           
        }
        private void OpenAllMine()
        {
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < GameBlock.RowDefinitions.Count; j++)
                {
                    if (matrixMine.cells[i,j].HasMine == true)
                    {
                        (int, int) pos = (i, j);
                        Label text = new Label();
                        text.Background = new ImageBrush(new BitmapImage(new Uri("https://clipartart.com/images/mine-sweeper-clipart-4.png")));
                        GameBlock.Children.Add(text);
                        Grid.SetColumn(text, pos.Item1);
                        Grid.SetRow(text, pos.Item2);
                    }
                }
            }

            MessageBox.Show("Вы проиграли");
            this.Close();
        }
        private void OpenAll()
        {
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < GameBlock.RowDefinitions.Count; j++)
                {
                    if (matrixMine.cells[i,j].HasMine == true)
                    {
                        (int, int) pos = (i, j);
                        Label text = new Label();
                        text.Background = new ImageBrush(new BitmapImage(new Uri("https://clipartart.com/images/mine-sweeper-clipart-4.png")));
                        GameBlock.Children.Add(text);
                        Grid.SetColumn(text, pos.Item1);
                        Grid.SetRow(text, pos.Item2);
                    }
                    else if(matrixMine.cells[i,j].MinesAround != 0)
                    {
                        (int, int) pos = (i, j);
                        Label text = new Label();
                        text.Content = "" + matrixMine.cells[i, j].MinesAround;
                        GameBlock.Children.Add(text);
                        Grid.SetColumn(text, pos.Item1);
                        Grid.SetRow(text, pos.Item2);
                    }
                    else if(matrixMine.cells[i,j].MinesAround == 0)
                    {
                        (int, int) pos = (i, j);
                        Label text = new Label();
                        text.Content = "" + matrixMine.cells[i, j].MinesAround;
                        GameBlock.Children.Add(text);
                        Grid.SetColumn(text, pos.Item1);
                        Grid.SetRow(text, pos.Item2);
                        
                    }
                }
            }

            MessageBox.Show("Вы проиграли");
            this.Close();
        }
        private int CheckClickCount((int, int) pos)
        {
            int count = 0;
            for (int i = 0; i < GameBlock.ColumnDefinitions.Count; ++i)
            {
                for (int k = 0; k < GameBlock.RowDefinitions.Count; ++k)
                {
                    if (matrixMine.cells[i, k].IsOpen == true)
                    {
                        count++;
                    }
                    if (i == pos.Item1 && k == pos.Item2)
                    {
                        return count;
                    }
                }
            }
            return count;
        }
    }
}
