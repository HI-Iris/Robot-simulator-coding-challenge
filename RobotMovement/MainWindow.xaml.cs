using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotMovement
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static private int xPosition = -1;
        static private int yPosition = -1;
        static private string direction = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        public static bool isPlaced(string command)
        {
            bool isPlaced = Regex.IsMatch(command, @"(?i)^PLACE [0-4],[0-4],(NORTH|SOUTH|EAST|WEST)$");
            return isPlaced;
        }

        public static bool isFell()
        {
            if ((xPosition < 0) || (yPosition < 0))
                return false;
            else if ((xPosition > 4) || (yPosition > 4))
                return false;
            else
                return true;
        }

        public void paint()
        {
            var cellName = "c" + xPosition + yPosition;
            var cell = RobotTable.FindName(cellName) as TableCell;
            switch (direction)
            {
                case "NORTH":
                    cell.Background = Brushes.Yellow; ; break;
                case "WEST":
                    cell.Background = Brushes.Red; ; break;
                case "SOUTH":
                    cell.Background = Brushes.Green; ; break;
                case "EAST":
                    cell.Background = Brushes.Blue; ; break;
            }
        }

        public void removeColor()
        {
            for (int i = 0; i <= 4; i++)
            {
                for (int j = 4; j >= 0; j--)
                {
                    var cellName = "c" + i + j;
                    var cell = RobotTable.FindName(cellName) as TableCell;
                    cell.Background = Brushes.Transparent;
                }
            }

        }

        private string move()
        {
            removeColor();
            string result = string.Empty;
            int lastX = xPosition;
            int lastY = yPosition;
            switch (direction)
            {
                case "NORTH":
                    yPosition++; break;
                case "WEST":
                    xPosition--; break;
                case "SOUTH":
                    yPosition--; break;
                case "EAST":
                    xPosition++; break;
            }
            if (!isFell())
            {
                xPosition = lastX;
                yPosition = lastY;
                MessageBox.Show("Robot falls off the table.");
                removeColor();
                result = "";
                Command.Text = "";
            }
            else
            {
                result = xPosition + "," + yPosition + "," + direction;
                paint();
            }

            return result;
        }

        private string left()
        {
            removeColor();
            string result = string.Empty;
            switch (direction)
            {
                case "NORTH":
                    direction = "WEST"; break;
                case "WEST":
                    direction = "SOUTH"; break;
                case "SOUTH":
                    direction = "EAST"; break;
                case "EAST":
                    direction = "NORTH"; break;
            }
            result = xPosition + "," + yPosition + "," + direction;
            paint();
            return result;
        }

        private string right()
        {
            removeColor();
            string result = string.Empty;
            switch (direction)
            {
                case "NORTH":
                    direction = "EAST"; break;
                case "EAST":
                    direction = "SOUTH"; break;
                case "SOUTH":
                    direction = "WEST"; break;
                case "WEST":
                    direction = "NORTH"; break;
            }
            result = xPosition + "," + yPosition + "," + direction;
            paint();
            return result;
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            Output.Content = "";
            string[] commandLines = Command.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (commandLines.Length > 0)
            {
                var placed = isPlaced(commandLines[0]);
                if (placed)
                {
                    char[] delimiterChars = { ',', ' ' };
                    string[] placeCommand = commandLines[0].Split(delimiterChars);
                    xPosition = Int32.Parse(placeCommand[1]);
                    yPosition = Int32.Parse(placeCommand[2]);
                    direction = placeCommand[3].ToUpper();

                    if (commandLines.Length > 1)
                    {
                        for (int i = 1; i < commandLines.Length; i++)
                        {
                            switch (commandLines[i].ToUpper())
                            {
                                case "MOVE":
                                    Output.Content = move();
                                    break;
                                case "LEFT":
                                    Output.Content = left();
                                    break;
                                case "RIGHT":
                                    Output.Content = right();
                                    break;
                                default:
                                    MessageBox.Show("Please check your command.");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Output.Content = xPosition + "," + yPosition + "," + direction;
                        paint();
                    }
                }
                else
                {
                    MessageBox.Show("Please check your command.");
                    removeColor();
                }
            }
            else
            {
                MessageBox.Show("Please enter your command.");

            }
        }

        private void ReadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "txt|*.txt";
            if (fd.ShowDialog() == true)
            {
                if (fd.FileName != "")
                {
                    FilePath.Text = fd.FileName;
                    FileStream fs = new FileStream(@fd.FileName, FileMode.Open, FileAccess.Read);
                    StreamReader rd = new StreamReader(fs);
                    rd.BaseStream.Seek(0, SeekOrigin.Begin);
                    Command.Text = "";
                    string s;
                    var lines = new List<string>();
                    while ((s = rd.ReadLine()) != null)
                    {
                        lines.Add(s);
                    }
                    rd.Close();
                    fs.Close();
                    if (lines.Count >= 1)
                    {
                        for (int i = 0; i < lines.Count; i++)
                        {
                            Command.Text += lines[i] + "\n";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter your command.");
                    }
                }
            }
        }

        private void InputMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Command.Text = "";
            FilePath.Text = "";
            Output.Content = "";
            removeColor();
        }
    }
}


