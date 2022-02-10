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

/* Создайте приложение WPF Application, в главном окне которого поместите любой текст. 
Также, должно быть окно настроек (можно реализовать с помощью TabControl). 
Пользователь может изменять настройки. При повторном запуске приложения настройки должны оставаться прежними.  
Реализуйте два варианта (в одном приложении или двух разных):  
1) сохранение настроек в конфигурационном файле; 
2) сохранение настроек в реестре. 
В окне настроек реализуйте следующие опции: цвет фона, цвет текста, размер шрифта, стиль шрифта, а также кнопку «Сохранить».  
Для выбора цвета воспользуйтесь ColorPicker-ом по примеру задания из Урока №3. */

namespace Task_4
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
                    info.Items.Add("В файле конфигурации нет информации...");
                else
                    info.Items.Add("Информация загружена из файла конфигурации...");
            }
            catch (Exception e)
            {
                info.Items.Add("Проблема при загрузке данных из файла конфигурации:");
                info.Items.Add(e.Message);
            }
        }

        bool ReadSettings()
        {
            // Загрузка настроек по парам [ключ]-[значение].
            NameValueCollection allAppSettings = ConfigurationManager.AppSettings;
            if (allAppSettings.Count < 1) { return false; }

            // Восстановление текста
            lblOut.Content = allAppSettings["Label.Text"];

            // Восстановление: Цвет фона
            byte alfa = Convert.ToByte(allAppSettings["BackGround.A"]);
            byte red = Convert.ToByte(allAppSettings["BackGround.R"]);
            byte green = Convert.ToByte(allAppSettings["BackGround.G"]);
            byte blue = Convert.ToByte(allAppSettings["BackGround.B"]);
            lblOut.Background = new SolidColorBrush(Color.FromArgb(alfa, red, green, blue));
            info.Items.Add("Цвет фона: " + lblOut.Background.ToString());

            // Восстановление: Шрифт текста
            lblOut.FontFamily = new FontFamily(allAppSettings["FontFamily"]);
            lblOut.FontSize = Convert.ToInt32(allAppSettings["FontSize"]);
            lblOut.FontWeight = allAppSettings["FontSize"] == "Bold" ? FontWeights.Bold : FontWeights.Normal;
            lblOut.FontStyle = allAppSettings["FontStyle"] == "Italic" ? FontStyles.Italic : FontStyles.Normal;
            info.Items.Add("Шрифт восстановлен");

            //Восстановление: Расположение на экране.
            int wTop = Convert.ToInt32(allAppSettings["Window.Top"]);
            int wLeft = Convert.ToInt32(allAppSettings["Window.Left"]);
            Top = wTop;
            Left = wLeft;
            info.Items.Add("Расположение: " + wTop + "x" + wLeft);

            //Восстановление: Размеры окна.
            Height = Convert.ToInt32(allAppSettings["Window.Height"]);
            Width = Convert.ToInt32(allAppSettings["Window.Width"]);
            info.Items.Add("Размер: " + new Size(Width, Height).ToString());

            //Восстановление: Состояние окна.
            string winState = allAppSettings["Window.State"];
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
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(Assembly.GetExecutingAssembly().Location + ".config");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw new Exception("No configuration file found.", ex);
            }

            // Сохранение происходит при помощи работы с XML.
            // Открываем узел appSettings, в котором содержится перечень настроек.
            XmlNode node = doc.SelectSingleNode("//appSettings");

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

            // Цикл модификации файла конфигурации.
            foreach (var item in appSets)
            {
                // Обращаемся к конкретной строке по ключу.
                XmlElement element = node.SelectSingleNode(string.Format("//add[@key='{0}']", item.Key)) as XmlElement;

                // Если строка с таким ключем существует - записываем значение.
                if (element != null) { element.SetAttribute("value", item.Value); }
                else
                {
                    // Иначе: создаем строку и формируем в ней пару [ключ]-[значение].
                    element = doc.CreateElement("add");
                    element.SetAttribute("key", item.Key);
                    element.SetAttribute("value", item.Value);
                    node.AppendChild(element);
                }
            }

            // Сохраняем результат модификации.
            doc.Save(Assembly.GetExecutingAssembly().Location + ".config");
            info.Items.Clear();
            info.Items.Add("Сохранен текст!");
            info.Items.Add("Сохранен цвет фона и шрифт текста");
            info.Items.Add("Сохранено расположение и размер окна.");
            info.Items.Add("Сохранено сосотояние окна приложения.");
            info.Items.Add("Все изменение сохранены в .config файл!");
        }
    }
}
