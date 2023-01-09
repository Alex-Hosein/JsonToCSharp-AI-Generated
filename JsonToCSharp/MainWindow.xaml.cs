using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace JsonToCSharpClasses
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

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            var jsonText = jsonInputTextBox.Text;
            var jo = JObject.Parse(jsonText);
            var sb = new StringBuilder();
            sb.Append("public class Rootobject\n{\n");
            ProcessJsonObject(jo, sb);
            sb.Append("}\n");
            cSharpOutputTextBox.Text = sb.ToString();
        }
        
        private void ProcessJsonObject(JObject jo, StringBuilder sb)
        {
            foreach (var prop in jo.Properties())
            {
                var val = prop.Value;
                sb.Append("\tpublic ");
                if (val.Type == JTokenType.Object)
                {
                    sb.Append(prop.Name + " \n\t{\n");
                    ProcessJsonObject(val.Value<JObject>(), sb);
                    sb.Append("\t}\n");
                }
                else if (val.Type == JTokenType.Array)
                {
                    var arr = val.Value<JArray>();
                    if (arr.Count > 0)
                    {
                        var firstElement = arr[0];
                        if (firstElement.Type == JTokenType.Object)
                        {
                            sb.Append("List<" + prop.Name + "> " + prop.Name + " { get; set; }\n");
                        }
                        else
                        {
                            sb.Append("List<" + GetCSharpType(firstElement.Type) + "> " + prop.Name + " { get; set; }\n");
                        }
                    }
                    else
                    {
                        sb.Append("List<dynamic> " + prop.Name + " { get; set; }\n");
                    }
                }
                else
                {
                    sb.Append(GetCSharpType(val.Type) + " " + prop.Name + " { get; set; }\n");
                }
            }
        }

        private string GetCSharpType(JTokenType type)
        {
            switch (type)
            {
                case JTokenType.Integer:
                    return "int";
                case JTokenType.Float:
                    return "float";
                case JTokenType.String:
                    return "string";
                case JTokenType.Boolean:
                    return "bool";
                case JTokenType.Null:
                    return "object";
                case JTokenType.Date:
                    return "DateTime";
                default:
                    return "object";
            }
        }

        private string FirstLetterCapital(string str)
        {
            return char.ToUpper(str[0]) + str.Remove(0, 1);
        }
    }
}