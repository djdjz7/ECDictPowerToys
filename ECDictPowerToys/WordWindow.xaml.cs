using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

namespace Community.PowerToys.Run.Plugin.ECDict
{
    /// <summary>
    /// WordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WordWindow : MicaWPF.Lite.Controls.MicaWindow
    {
        public WordWindow(Dictionary<string, object> info)
        {
            InitializeComponent();

            var word = info["word"] as string;
            var phonetic = info["phonetic"] as string;
            var translation = info["translation"] as string;
            translation = translation?.Replace("\\n", Environment.NewLine);

            Title = word;
            TitleWord.Text = word;
            Phonetic.Text = $"/{phonetic}/";
            Translation.Text = translation;

            if(string.IsNullOrEmpty(phonetic))
                Phonetic.Visibility = Visibility.Collapsed;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter || e.Key == Key.Space)
            {
                try
                {
                    Close();
                }
                catch { }
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch { }
        }
    }
}
