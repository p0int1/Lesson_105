using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using MessageBox = System.Windows.MessageBox;

namespace Task_5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ColorDialog colorDlg = new ColorDialog();
        private FontDialog fontDlg = new FontDialog();
        private Dictionary<string, string> appSets = new Dictionary<string, string>();
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                if (ReadSettings() == false)
                    info.Items.Add("В реестре нет информации...");
                else
                    info.Items.Add("Информация загружена из реестра...");
            }
            catch (Exception e)
            {
                info.Items.Add("Проблема при загрузке данных из реестра:");
                info.Items.Add(e.Message);
            }
        }

        bool ReadSettings()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\appTest\");
                string[] allAppSettings = regKey.GetValueNames();

                if (allAppSettings.Length < 1) { return false; }

                appSets.Clear();
                foreach (var item in allAppSettings)
                {
                    appSets.Add(item, regKey.GetValue(item).ToString());
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Загрузка настроек по парам [ключ]-[значение].
            // Восстановление текста
            lblOut.Content = appSets["Label.Text"];

            // Восстановление: Цвет фона
            byte alfa = Convert.ToByte(appSets["BackGround.A"]);
            byte red = Convert.ToByte(appSets["BackGround.R"]);
            byte green = Convert.ToByte(appSets["BackGround.G"]);
            byte blue = Convert.ToByte(appSets["BackGround.B"]);
            lblOut.Background = new SolidColorBrush(Color.FromArgb(alfa, red, green, blue));
            info.Items.Add("Цвет фона: " + lblOut.Background.ToString());

            // Восстановление: Шрифт текста
            lblOut.FontFamily = new FontFamily(appSets["FontFamily"]);
            lblOut.FontSize = Convert.ToInt32(appSets["FontSize"]);
            lblOut.FontWeight = appSets["FontSize"] == "Bold" ? FontWeights.Bold : FontWeights.Normal;
            lblOut.FontStyle = appSets["FontStyle"] == "Italic" ? FontStyles.Italic : FontStyles.Normal;
            info.Items.Add("Шрифт восстановлен");

            //Восстановление: Расположение на экране.
            int wTop = Convert.ToInt32(appSets["Window.Top"]);
            int wLeft = Convert.ToInt32(appSets["Window.Left"]);
            Top = wTop;
            Left = wLeft;
            info.Items.Add("Расположение: " + wTop + "x" + wLeft);

            //Восстановление: Размеры окна.
            Height = Convert.ToInt32(appSets["Window.Height"]);
            Width = Convert.ToInt32(appSets["Window.Width"]);
            info.Items.Add("Размер: " + new Size(Width, Height).ToString());

            //Восстановление: Состояние окна.
            string winState = appSets["Window.State"];
            WindowState = (WindowState)WindowState.Parse(WindowState.GetType(), winState);
            info.Items.Add("Состояние окна: " + winState);

            return true;
        }

        private void changeText(object sender, RoutedEventArgs e)
        {
            lblOut.Content = txtIn.Text;
        }

        private void setColor_Click(object sender, RoutedEventArgs e)
        {
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lblOut.Background = new SolidColorBrush(Color.FromArgb(colorDlg.Color.A, colorDlg.Color.R, colorDlg.Color.G, colorDlg.Color.B));
            }
        }

        private void lblOut_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            setColor_Click(sender, e);
        }

        private void setFont_Click(object sender, RoutedEventArgs e)
        {
            if (fontDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lblOut.FontFamily = new FontFamily(fontDlg.Font.Name);
                lblOut.FontSize = fontDlg.Font.Size * 96.0 / 72.0;
                lblOut.FontWeight = fontDlg.Font.Bold ? FontWeights.Bold : FontWeights.Normal;
                lblOut.FontStyle = fontDlg.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        private void saveUI_Click(object sender, RoutedEventArgs e)
        {
            var color = (SolidColorBrush)lblOut.Background;
            appSets.Clear();
            appSets.Add("Label.Text", lblOut.Content.ToString());
            appSets.Add("BackGround.A", color.Color.A.ToString());
            appSets.Add("BackGround.R", color.Color.R.ToString());
            appSets.Add("BackGround.G", color.Color.G.ToString());
            appSets.Add("BackGround.B", color.Color.B.ToString());
            appSets.Add("FontFamily", lblOut.FontFamily.ToString());
            appSets.Add("FontSize", Math.Floor(lblOut.FontSize).ToString());
            appSets.Add("FontWeight", lblOut.FontWeight.ToString());
            appSets.Add("FontStyle", lblOut.FontStyle.ToString());
            appSets.Add("Window.Top", Top.ToString());
            appSets.Add("Window.Left", Left.ToString());
            appSets.Add("Window.Height", Height.ToString());
            appSets.Add("Window.Width", Width.ToString());
            appSets.Add("Window.State", WindowState.ToString());

            // Цикл модификации реестра.
            try
            {
                RegistryKey regKey = Registry.CurrentUser;
                regKey = regKey.CreateSubKey(@"Software\appTest");
                foreach (var item in appSets)
                {
                    regKey.SetValue(item.Key, item.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            info.Items.Clear();
            info.Items.Add("Сохранен текст!");
            info.Items.Add("Сохранен цвет фона и шрифт текста");
            info.Items.Add("Сохранено расположение и размер окна.");
            info.Items.Add("Сохранено сосотояние окна приложения.");
            info.Items.Add("Все изменение сохранены в реестре!");
        }
    }
}
