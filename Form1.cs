using Microsoft.Win32;
using System.Diagnostics;

namespace Steam_Login_Manager
{
    public partial class SteamUserManager : Form
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/UserData.txt";
        struct user
        {
            public user(string user, string pass)
            {
                username = user;
                password = pass;
            }

            public string username { get; set; }
            public string password { get; set; }
        }

        List<user> userList = new List<user>();

        public SteamUserManager()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            if (!File.Exists(path))
            {
                StreamWriter sw = File.CreateText(path);
                sw.Flush();
                sw.Dispose();
            }

            List<string> lines = File.ReadAllLines(path).ToList();
            foreach(var line in lines)
            {
                // username,password
                string[] entries = line.Split(',');
                user newUser = new user(entries[0], entries[1]);

                comboBox1.Items.Add(entries[0]);
                userList.Add(newUser);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Fill the fields first!");
                return;
            }

            AddNewUser();
            ResetTextBoxes();
            UpdateComboBox();
            MessageBox.Show("New steam user has been added!");
        }

        private void AddNewUser()
        {
            List<string> lines = File.ReadAllLines(path).ToList();
            var User = new user(textBox1.Text, textBox2.Text);
            userList.Add(User);
            lines.Add(User.username + "," + User.password);
            File.WriteAllLines(path, lines);
        }

        private void UpdateComboBox()
        {
            comboBox1.Items.Clear();
            foreach (var user in userList)
            {
                comboBox1.Items.Add(user.username);
            }
        }

        private void ResetTextBoxes()
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex== -1)
            {
                MessageBox.Show("Chose the account first! If you already entered one.");
                return;
            }
            foreach(var process in Process.GetProcessesByName("steam"))
            {
                process.Kill();
            }

            ProcessStartInfo startInfo= new ProcessStartInfo();

            startInfo.FileName = (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamExe", "null");
            startInfo.Arguments = " -login "+ userList[comboBox1.SelectedIndex].username + " " + userList[comboBox1.SelectedIndex].password;
            Process.Start(startInfo);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            File.Delete(path);
            StreamWriter sw = File.CreateText(path);
            sw.Flush();
            sw.Dispose();

            System.Windows.Forms.Application.Exit();
            UpdateComboBox();
        }
    }
}