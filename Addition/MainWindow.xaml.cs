using System.IO;
using System.Windows;
using System.Xml;

/* Создайте.xml файл, который соответствовал бы следующим требованиям: 
-  имя файла: TelephoneBook.xml
-  корневой элемент: “MyContacts” 
-  тег “Contact”, и в нем должно быть записано имя контакта и атрибут “TelephoneNumber” со значением номера телефона. */

namespace Addition
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

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var xmlWriter = new XmlTextWriter("TelephoneBook.xml", null);

            // Включить форматирование документа (с отступом).
            xmlWriter.Formatting = Formatting.Indented;

            // Для выделения уровня элемента использовать табуляцию.
            xmlWriter.IndentChar = '\t';

            // использовать один символ табуляции.
            xmlWriter.Indentation = 1;

            // Аналогично можно указать выравнивание с помощью четырех пробелов.
            //xmlWriter.IndentChar = ' ';
            //xmlWriter.Indentation = 4;

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("MyContacts");
            xmlWriter.WriteStartElement("Contact");
            xmlWriter.WriteStartAttribute("TelephoneNumber");
            xmlWriter.WriteString("(050)9440091");
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteString("Valerij Schurov");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            xmlWriter.Close();

            using (StreamReader reader = new StreamReader("TelephoneBook.xml"))
            {
                lbl.Content = reader.ReadToEnd();
            }




            


        }
    }
}
