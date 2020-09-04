using System.Windows;

namespace Puzzle
{
    public partial class InputWindow : Window
    {
        MainWindow mainWindow;

        public InputWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();

            for(int i = 2; i < 20; i++)
            {
                Cols.Items.Add(i);
                Rows.Items.Add(i);
            }
            Cols.SelectedIndex = 3;
            Rows.SelectedIndex = 3;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.SetupBlocks((int)Rows.SelectedValue, (int)Cols.SelectedValue);
            Close();
        }
    }
}