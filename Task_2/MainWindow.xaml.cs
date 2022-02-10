using System.IO;
using System.Windows;
using System.Xml;

// Создайте приложение, которое выводит на экран всю информацию об указанном .xml файле. 

namespace Task_2
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

        private void readXml_Click(object sender, RoutedEventArgs e)
        {
            FileStream stream = new FileStream("TelephoneBook.xml", FileMode.Open);

            XmlTextReader xmlReader = new XmlTextReader(stream);

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        outPut.AppendText("<" + xmlReader.Name);

                        while (xmlReader.MoveToNextAttribute()) // Read the attributes.
                            outPut.AppendText(" " + xmlReader.Name + "='" + xmlReader.Value + "'");
                        outPut.AppendText(">\n");
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        outPut.AppendText(xmlReader.Value + "\n");
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        outPut.AppendText("</" + xmlReader.Name);
                        outPut.AppendText(">\n");
                        break;
                }
            }
            xmlReader.Close();
        }
    }
}
