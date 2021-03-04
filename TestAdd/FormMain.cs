using Infoearth.BotEnvironment.Sealions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestAdd
{
    public partial class FormMain : Form
    {
        QA_Bot_Html<monitorQA> _context = new QA_Bot_Html<monitorQA>();
        public FormMain()
        {
            InitializeComponent();         
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //编辑
            if(e.ColumnIndex==1)
            {
                FormAdd formAdd = new FormAdd(new monitorQA()
                {
                    _id= dataGridView1[0, e.RowIndex].Value?.ToString(),
                    keyword = dataGridView1[5, e.RowIndex].Value?.ToString(),
                    model = dataGridView1[6, e.RowIndex].Value?.ToString(),
                    answer = dataGridView1[7, e.RowIndex].Value?.ToString()
                });
                if (formAdd.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("保存成功!");
                    Search(string.Empty);
                }
            }
            //删除
            else if(e.ColumnIndex==2)
            {
               string _id = dataGridView1[0, e.RowIndex].Value?.ToString();
                _context.ESEngine.Delete(_id);
                MessageBox.Show("删除成功!");
                Search(string.Empty);
            }
        }

        private void Search(string keyword)
        {
            ElasticModel<monitorQA> result = _context.ESEngine.Search<monitorQA>(keyword);

            dataGridView1.Rows.Clear();
            if (result != null && result.list != null && result.list.Count > 0)
            {
                int rowNum = 1;
                foreach (var item in result.list)
                {
                    dataGridView1.Rows.Add(new object[]
                   {
                       item._id,"编辑","删除",rowNum,item._score,item.keyword,item.model,item.answer
                   });
                    rowNum++;
                }

            }
            toolStripStatusLabel1.Text = $"查询到{result.list.Count}条数据，耗时{result.took}毫秒";
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Search(string.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Search(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormAdd formAdd = new FormAdd();
            if (formAdd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("新增成功!");
                Search(string.Empty);
            }
        }
    }
}
