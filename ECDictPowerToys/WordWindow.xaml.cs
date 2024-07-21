using Common.UI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static SystemBackdropTypes.PInvoke.ParameterTypes;
using static SystemBackdropTypes.PInvoke.Methods;

namespace Community.PowerToys.Run.Plugin.ECDict
{
    /// <summary>
    /// WordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WordWindow : Window
    {
        private bool _isDarkMode;
        public WordWindow(Dictionary<string, object> info, bool isDarkMode)
        {
            _isDarkMode = isDarkMode;
            Loaded += OnLoaded;
            InitializeComponent();

            if(_isDarkMode)
            {
                Resources["TextElementBrush"] = new SolidColorBrush(Colors.White);
            }

            var word = info["word"] as string;
            var phonetic = info["phonetic"] as string;
            var translation = info["translation"] as string;
            translation = translation?.Replace("\\n", Environment.NewLine);

            Title = word;
            TitleWord.Text = word;
            Phonetic.Text = $"/{phonetic}/";
            Translation.Text = translation;

            if (string.IsNullOrEmpty(phonetic))
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

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
            HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            MARGINS margins = new MARGINS();
            margins.cxLeftWidth = -1;
            margins.cxRightWidth = -1;
            margins.cyTopHeight = -1;
            margins.cyBottomHeight = -1;

            ExtendFrame(mainWindowSrc.Handle, margins);
            int flag = _isDarkMode ? 1 : 0;
            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                flag);
            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                2);
        }
    }
}
