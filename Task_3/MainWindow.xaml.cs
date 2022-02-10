using System.Windows;
using System.Xml.XPath;

// Из файла  TelephoneBook.xml  (файл  должен  был  быть  создан  в  процессе  выполнения 
// дополнительного задания) выведите на экран только номера телефонов. 

namespace Task_3
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

        private void findNumb_Click(object sender, RoutedEventArgs e)
        {
            // Создание XPath документа.
            var document = new XPathDocument("TelephoneBook.xml");
            XPathNavigator navigator = document.CreateNavigator();

            // Прямой запрос XPath.
            XPathNodeIterator iterator1 = navigator.Select("MyContacts/Contact");

            foreach (var item in iterator1)
                outPut.Items.Add(item);

            // Скомпилированный запрос XPath
            XPathExpression expr = navigator.Compile("MyContacts/Contact/@TelephoneNumber");
            XPathNodeIterator iterator2 = navigator.Select(expr);

            foreach (var item in iterator2)
                outPut.Items.Add(item);
        }
    }
}
