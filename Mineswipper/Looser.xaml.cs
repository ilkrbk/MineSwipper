using System.Windows;

namespace Mineswipper
{
    public partial class Looser : Window
    {
        public bool win { get; set; }
        public Looser(bool win)
        {
            InitializeComponent();
            this.win = win;
            if (win)
            {
                GameOver.Text += "\nYOU ARE WINNER";
            }
            else
            {
                GameOver.Text += "\nYOU ARE LOOSER";

            }
        }
        private void startbtn_Click(object sender, RoutedEventArgs e)
        {
           MainWindow main = new MainWindow();
           main.Show();
           this.Close();
        }
        private void Clos(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }
    }
}