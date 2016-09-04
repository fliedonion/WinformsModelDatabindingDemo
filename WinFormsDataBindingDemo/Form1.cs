using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsDataBindingDemo {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        public class Model : INotifyPropertyChanged{
            // Add Allow Dictionary if InValid.

            public Model() {
            }

            private void Model_PropertyChanged([CallerMemberName]string name = null) {
                PropertyChanged(null, new PropertyChangedEventArgs(name));
            }

            private string value = "";
            public string Value
            {
                get
                {
                    return value;
                }
                set
                {
                    var validated = sortedValidatorNames.Select((key) => !validators[key](value) ? key : "").Where(x => x != "");
                    if (validated.Any() && validated.First() != "") {
                        var critical = validated.First();
                        this.Error = ErrorMessages[critical].Value;
                        this.value = value;
                        // Model_PropertyChanged();
                    }
                    else { 
                        this.value = value;
                        Error = "";
                    }
                }
            }

            private void Validate() {
                var validated = sortedValidatorNames.Select((key) => !validators[key](value) ? key : "").Where(x => x != "");
                if (validated.Any()) {
                    var critical = validated.First();
                    this.Error = ErrorMessages[critical].Value;
                    // Model_PropertyChanged();
                }else {
                    Error = "";
                }
            }


            public void SetValue(string value) {
                // force set value, also value is NOT valid.
                this.value = value;
                Validate();
            }


            private string error;
            public string Error {
                get {
                    return error;
                }
                private set {
                    error = value;
                    Model_PropertyChanged();
                }
            }

            List<string> sortedValidatorNames = null;

            Dictionary<string, Func<string, bool>> validators = new Dictionary<string,Func<string, bool>>();
            Dictionary<string, KeyValuePair<int, string>> ErrorMessages = new Dictionary<string, KeyValuePair<int, string>>();

            public event PropertyChangedEventHandler PropertyChanged;

            public void AddValidator(string name, Func<string, bool> expectInspection, string errorMessage, int errorRank) {
                if (validators.ContainsKey(name)) {
                    validators[name] = expectInspection;
                    ErrorMessages[name] = new KeyValuePair<int, string>(errorRank, errorMessage);
                }
                else {
                    validators.Add(name, expectInspection);
                    ErrorMessages.Add(name, new KeyValuePair<int, string>(errorRank, errorMessage));
                }
                sortedValidatorNames = ErrorMessages.OrderByDescending(x => x.Value.Key).Select(x => x.Key).ToList();
            }

            // public Dictionary<string, Func<string, bool>> Validators { private set; get; }

        }


        public class ComboModel {
            public string Value { get; set; }
            public int Index { get; set; }

            public override string ToString() {
                return Value;
            }


        }



        private Model m = new Model();

        private void Form1_Load(object sender, EventArgs e) {

            m.AddValidator("Decimal", x => { decimal d; return decimal.TryParse(x, out d); }, "Is not numeric", 5);
            m.AddValidator("Max", x => decimal.Parse(x) < 100.0M, "Value is too large", 1);
            m.AddValidator("Max", x => decimal.Parse(x) > 10.0M, "Value is too small", 1);

            textBox1.DataBindings.Add("Text", m, "Value", false, DataSourceUpdateMode.OnPropertyChanged);
            textBox2.DataBindings.Add("Text", m, "Value", false, DataSourceUpdateMode.OnPropertyChanged);

            label1.DataBindings.Add("Text", m, "Error", false, DataSourceUpdateMode.Never);

            textBox2.Validating += TextBox2_Validating;


            List<ComboModel> cm = new List<ComboModel>();
            cm.Add(new ComboModel { Value = "不明", Index = 100 });
            cm.Add(new ComboModel { Value = "1 X", Index = 101 });
            cm.Add(new ComboModel { Value = "2 Y", Index = 102 });
            cm.Add(new ComboModel { Value = "3 Z", Index = 103 });
            cm.Add(new ComboModel { Value = "4", Index = 104 });

            comboBox1.DataSource = cm;
            comboBox1.DataBindings.Add("Text", m, "Value", false, DataSourceUpdateMode.OnPropertyChanged);
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

            var ac = new AutoCompleteStringCollection();
            cm.ForEach(x => ac.Add(x.Value));

            comboBox1.AutoCompleteCustomSource = ac;


        }

        private void TextBox2_Validating(object sender, CancelEventArgs e) {
            if(textBox2.Equals(sender)) {
                var m = (Model)((TextBox)sender).DataBindings[0].DataSource;
                if(m.Value != textBox2.Text) {
                    textBox2.Text = m.Value;
                    e.Cancel = true;
                }
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e) {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e) {

        }

        private void Form1_Shown(object sender, EventArgs e) {

            textBox2.Text = "aaa";
        }

        private void button1_Click(object sender, EventArgs e) {
            var o = comboBox1.SelectedIndex;
            if (o == -1) {
                MessageBox.Show(comboBox1.Text);
            }
            else {
                var sm = ((List<ComboModel>)comboBox1.DataSource)[o];
                MessageBox.Show(sm.Index.ToString() + "-" + comboBox1.Text);
            }
        }


    }
}
