using Infoearth.BotEnvironment.Sealions;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        monitor _monitor = null;

        public FormAdd()
        {
            InitializeComponent();
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
                IndexResult indexResult = esHelper.Index("test", "monitor", _monitor._id, _monitor);
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
            _monitor.answer = textBox2.Text;
        }
    }
}
