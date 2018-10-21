using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace FileManipulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Text (*.txt)|*.txt"
            };

            Nullable<bool> result = dlg.ShowDialog();
            this.licznik.Content = "";

            this.bUsun.IsEnabled = false;

            ulong counter = 0;
            string line, prevline = string.Empty;

            DateTime dt = DateTime.Now;
            TimeSpan ts = new TimeSpan();

            if (result == true)
            {
                using (StreamWriter sw = new StreamWriter(dlg.FileName + ".rdl"))
                {
                    using (StreamReader file = new StreamReader(dlg.FileName))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            if (!line.Equals(prevline)) sw.WriteLine( line );

                            prevline = line;
                            counter++;
                            this.licznik.Content = "Line no.: " + counter;
                            DoEvents();
                        }

                    }

                }

                ts = DateTime.Now - dt;
            }

            this.bUsun.IsEnabled = true;

            FileInfo forg = new FileInfo(dlg.FileName);
            FileInfo fopt = new FileInfo(dlg.FileName + ".rdl");

            MessageBox.Show(string.Format("Source file size: {0}[b]\nOptimize size: {1}[b]\nRecuded file size by: {2}[b]\nTime passed {3}:{4}:{5}",
                forg.Length, fopt.Length, forg.Length - fopt.Length, ts.TotalHours, ts.TotalMinutes, ts.TotalSeconds));

        }

        private void BPrzytnij_Click(object sender, RoutedEventArgs e)
        {
            long s1, s2;
            if(this.startOffset.Text!="") s1 = Convert.ToInt64(this.startOffset.Text);
            else s1 = 0;
            
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();

            this.bPrzytnij.IsEnabled = false;
            this.lPostep.Content = "";

            if (result == true)
            {
                 
                FileInfo forg = new FileInfo(dlg.FileName);

                if(this.stopoffset.Text!="") s2 = Convert.ToInt64(this.stopoffset.Text);
                else s2 = forg.Length;

                if (s1 < s2)
                {
                    char[] buffer = new char[8192];
                    using (StreamWriter sw = new StreamWriter(dlg.FileName + ".truncate"))
                    {
                        using (StreamReader file = new StreamReader(dlg.FileName))
                        {
                            file.BaseStream.Seek(s1, SeekOrigin.Begin);

                            for (; ; )
                            {
                                this.lPostep.Content = "Processed " + (file.BaseStream.Position - s1) + "/" + (s2-s1)+ " [b]";

                                if (file.BaseStream.Position + 8192 > s2)
                                {
                                    int count = file.Read(buffer, 0, (int)(s2 - file.BaseStream.Position));
                                    if (count == 0) break;
                                    sw.Write(buffer, 0, count);
                                    break;
                                }
                                else
                                {
                                    int count = file.Read(buffer, 0, buffer.Length);
                                    if (count == 0) break;
                                    sw.Write(buffer, 0, count);
                                }
                            }
                        }
                    }
                }
            }

            this.bPrzytnij.IsEnabled = true;

            MessageBox.Show("Trim complete.");
        }
    }
}
