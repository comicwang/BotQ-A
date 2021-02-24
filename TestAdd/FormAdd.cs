using Infoearth.BotEnvironment.Sealions;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Test;

namespace TestAdd
{
    public partial class FormAdd : Form
    {
        ElasticSearchContext esHelper = ElasticSearchContext.Intance;
        private string _index = string.Empty;
        private string _indexType = string.Empty;
        monitor _monitor = null;

        public FormAdd()
        {
            InitializeComponent();
            string[] strs = ConfigurationManager.AppSettings["ElasticSearchConnection"].Split(' ');
            if (strs.Length < 4)
                throw new InvalidOperationException("ES配置错误");
            _index = strs[2];
            _indexType = strs[3];
            _monitor = new monitor();
        }

        public FormAdd(monitor monitor) : this()
        {
            bindingSource1.DataSource = monitor;
            _monitor = monitor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_monitor != null)
            {
                IndexResult indexResult = esHelper.Index(_index, _indexType, _monitor._id, _monitor);
            }
            DialogResult = DialogResult.OK;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _monitor.keyword = textBox1.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            _monitor.model = textBox3.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            _monitor.answer = richTextBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件|*.*|图片jpg|*.jpg|图片png|*.png|图片jpeg|*.jpeg|图片gif|*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                {
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, (int)fileStream.Length);
                    string base64 = Convert.ToBase64String(buffer);
                    richTextBox1.Text = richTextBox1.Text + $"<img src='data:image/{Path.GetExtension(fileName).Remove(0,1)};base64,{base64}'/>";
                   // richTextBox1.ScrollToCaret();
                }
            }
        }
    }
}
