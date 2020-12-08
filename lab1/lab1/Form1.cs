﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Labaratorna1_ANTLRFree
{
    public partial class Form1 : Form
    {
        int curRow, curCol;
        public Dictionary<string, Cell> dict = new Dictionary<string, Cell>();
        public Dictionary<string, string> dict2 = new Dictionary<string, string>();
        public Dictionary<string, string> dict3 = new Dictionary<string, string>();
        Parser pars = new Parser();
        void DGVInitializer()
        {
            for (int i = 0; i < 10; i++)
            {
                Cell cell = new Cell();
                DataGridViewColumn column = new DataGridViewColumn(cell);
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = (i + 1).ToString();
                column.HeaderText = Converter26.To26(i);
                column.Name = Converter26.To26(i);
                dataGridView1.Columns.Add(column);
                dataGridView1.Rows.Add(row);

            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    string cell_name = Converter26.To26(j) + (i + 1).ToString();
                    Cell cell = new Cell();
                    cell.Value = "0";
                    cell.Exp = "0";
                    dict.Add(cell_name, cell);
                    dict2.Add(cell_name, "");
                    dict3.Add(cell_name, "");
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            DGVInitializer();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                curRow = dataGridView1.CurrentCell.RowIndex;
                curCol = dataGridView1.CurrentCell.ColumnIndex;
                string cell_name = Converter26.To26(curCol) + (curRow + 1).ToString();
                dataGridView1[curCol, curRow].Value = dict[cell_name].Exp;
            }
            catch { }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                curRow = dataGridView1.CurrentCell.RowIndex;
                curCol = dataGridView1.CurrentCell.ColumnIndex;
                string cell_name = Converter26.To26(curCol) + (curRow + 1).ToString();
                string str = dataGridView1[curCol, curRow].Value.ToString();
                dict2[cell_name] = str;
                string tmp = dict[cell_name].Exp;
                dict[cell_name].Exp = str;
                string st = AdressAnalizator(str, cell_name);
                if (st == "null")
                {
                    MessageBox.Show("Wrong reference");
                    dict[cell_name].Value = pars.ExpressionStart(tmp).ToString();
                    dataGridView1[curCol, curRow].Value = dict[cell_name].Value;
                    dict[cell_name].Exp = tmp;
                    dict[cell_name].Depends.Clear();
                    dict[cell_name].Depends.Capacity = 0;
                    dict2[cell_name] = "";
                    dict3[cell_name] = "";
                }
                else
                {
                    dict3[cell_name] = st;

                    if (IsRecur(cell_name))
                    {
                        MessageBox.Show("Expression is recur");
                        dict[cell_name].Value = pars.ExpressionStart(tmp).ToString();
                        dataGridView1[curCol, curRow].Value = dict[cell_name].Value;
                        dict[cell_name].Exp = tmp;
                        dict[cell_name].Depends.Clear();
                        dict[cell_name].Depends.Capacity = 0;
                        dict2[cell_name] = "";
                        dict3[cell_name] = "";
                    }
                    else
                    {

                        double res = pars.ExpressionStart(st);
                        string result = res.ToString();
                        string errors = pars.err;
                        if (errors != "")
                        {

                            dict[cell_name].Value = pars.ExpressionStart(tmp).ToString();
                            dataGridView1[curCol, curRow].Value = dict[cell_name].Value;
                            dict[cell_name].Exp = tmp;
                            dict[cell_name].Depends.Clear();
                            dict[cell_name].Depends.Capacity = 0;
                            AdressAnalizator(tmp, cell_name);
                            dict2[cell_name] = "";
                            dict3[cell_name] = "";

                            pars.err = "";
                        }
                        else
                        {
                            dict[cell_name].Value = result;
                            dataGridView1[curCol, curRow].Value = dict[cell_name].Value;
                        }
                    }
                }
                RefreshCells();
            }

            catch { }
        }

        public string AdressAnalizator(string str, string cell_name)
        {
            try
            {
                string st = "";
                char[] delim = { ' ' };
                List<string> lex = new List<string>(str.Split(delim));
                dict[cell_name].Depends.Clear();
                dict[cell_name].Depends.Capacity = 0;
                for (int i = 0; i < lex.Count; i++)
                {
                    string lexem = lex[i];
                    if (dict.ContainsKey(lexem))
                    {
                        dict[cell_name].Depends.Add(lexem);
                        lexem = dict[lexem].Value;

                        lex[i] = lexem;
                    }
                    else if (!"+-*/^<>()=m,]1234567890".Contains(lexem[0]))
                    {
                        MessageBox.Show("Ссилка на неіснуючу комірку");
                        lexem = int.MaxValue.ToString();
                        lex[i] = lexem;
                    }

                }
                for (int i = 0; i < lex.Count; i++)
                {
                    st += lex[i];
                }
                str = st;
                return str;
            }
            catch
            {
                str = "null";

                return str;
            }
        }

        List<string> dependencesFound = new List<string>();
        bool dop_rec(string cell_name1, string cell_name2)
        {

            foreach (string dependence in dict[cell_name1].Depends)
            {
                if (dependencesFound.Contains(dependence))
                {
                    return true;
                }
                else
                {
                    dependencesFound.Add(dependence);
                    if (dop_rec(dependence, dependence))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsRecur(string cell_name)
        {
            dependencesFound.Clear();
            dependencesFound.Capacity = 0;
            return dop_rec(cell_name, cell_name);
        }

        public void RefreshCells()
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    string cell_name = Converter26.To26(i) + (j + 1).ToString();
                    if (dict.ContainsKey(cell_name) == false)
                    {
                        Cell cell = new Cell();
                        cell.Value = "0";
                        cell.Exp = "0";
                        dict.Add(cell_name, cell);
                        dict2.Add(cell_name, "");
                        dict3.Add(cell_name, "");
                    }
                    string res;
                    string st = AdressAnalizator(dict[cell_name].Exp, cell_name);
                    if (st == "null")
                    {
                        res = "0";
                        dict[cell_name].Value = res;
                        dataGridView1[i, j].Value = dict[cell_name].Value;
                    }
                    else
                    {
                        res = pars.ExpressionStart(st).ToString();
                        string errors = pars.err;
                        if (errors != "")
                        {

                            dict[cell_name].Value = pars.ExpressionStart("0").ToString();
                            dataGridView1[curCol, curRow].Value = dict[cell_name].Value;
                            dict[cell_name].Exp = "0";
                            dict[cell_name].Depends.Clear();
                            dict[cell_name].Depends.Capacity = 0;

                            dict2[cell_name] = "";
                            dict3[cell_name] = "";
                            dataGridView1[i, j].Value = dict[cell_name].Value;
                            pars.err = "";
                        }
                        else
                        {
                            dict[cell_name].Value = res;
                            dataGridView1[i, j].Value = dict[cell_name].Value;
                        }
                    }

                }
            }
        }

        void AddRow()
        {
            try
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = (dataGridView1.RowCount + 1).ToString();
                dataGridView1.Rows.Add(row);
                RefreshCells();
            }
            catch { MessageBox.Show("Додайте стовпчик"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        void AddColumn()
        {
            DataGridViewColumn column = new DataGridViewColumn();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            column.CellTemplate = cell;
            column.HeaderText = Converter26.To26(dataGridView1.ColumnCount);
            column.Name = Converter26.To26(dataGridView1.ColumnCount);
            dataGridView1.Columns.Add(column);
            RefreshCells();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddColumn();
        }
        bool flag = false;

        void DeletedCellUpd(int row, int col)
        {
            string cell_name = Converter26.To26(col) + (row + 1).ToString();
            flag = false;
            foreach (var i in dict)
            {
                if (i.Value.Depends.Contains(cell_name))
                {
                    flag = true;
                    DialogResult result = MessageBox.Show("Ви намагаєтесь видалити клітинку на яку щось ссилається", "Помилка", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        flag = false;
                    }
                    break;
                }
            }
            if (flag == false)
            {
                dict2[cell_name] = "";
                dict[cell_name].Value = "0";
                dict[cell_name].Exp = "0";
                dict3[cell_name] = "";
                dict.Remove(cell_name);
                dict2.Remove(cell_name);
                dict3.Remove(cell_name);
            }
        }

        void DelRow()
        {
            flag = false;
            try
            {
                int row = dataGridView1.RowCount - 1;
                if (row == 0)
                {
                    DialogResult result = MessageBox.Show("Це останній рядок", "Видалити неможливо", MessageBoxButtons.OK);

                }
                else
                {
                    for (int col = 0; col < dataGridView1.ColumnCount; col++)
                    {
                        DeletedCellUpd(row, col);
                        if (flag == true)
                        {
                            flag = false;
                            return;
                        }
                    }
                    dataGridView1.Rows.RemoveAt(row);

                    RefreshCells();

                }
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DelRow();
        }

        void DelColumn()
        {
            flag = false;
            try
            {
                int col = dataGridView1.ColumnCount - 1;
                if (col == 0)
                {
                    DialogResult result = MessageBox.Show("Це остання колонка", "Видалити неможливо?", MessageBoxButtons.OK);

                }
                else
                {
                    for (int row = 0; row < dataGridView1.RowCount; row++)
                    {
                        DeletedCellUpd(row, col);
                        if (flag == true)
                        {
                            flag = false;
                            return;
                        }
                    }
                    RefreshCells();
                    dataGridView1.Columns.RemoveAt(col);
                }

            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DelColumn();
        }

        void SaveFile()
        {
            Stream mystream;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((mystream = saveFileDialog1.OpenFile()) != null)
                {
                    StreamWriter sw = new StreamWriter(mystream);
                    sw.WriteLine(dataGridView1.RowCount);
                    sw.WriteLine(dataGridView1.ColumnCount);
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        for (int j = 0; j < dataGridView1.ColumnCount; j++)
                        {
                            sw.WriteLine(dataGridView1.Rows[i].Cells[j].Value.ToString());
                        }

                    }
                    try
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            for (int j = 0; j < dataGridView1.ColumnCount; j++)
                            {
                                string cell_name = Converter26.To26(j) + (i + 1).ToString();
                                sw.WriteLine(dict[cell_name].Exp);
                            }

                        }
                    }
                    catch { }
                    sw.Close();
                    mystream.Close();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Хочете зберегти зміни", "Зберегти файл", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                SaveFile();
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Автор: Коваленко Ілля\nГрупа: К-25\n\nРекомендації щодо редактування форми: рекомендується відділяти лексеми пробілами, назви клітинок у формулах виділяються пробілом ОБОВ'ЯЗКОВО\n\nСписок операцій: + - * \\ > < = >= <= <> max[a, b], min[a,b]\nЯкщо ссилка на комірку створює помилку ділення на нуль - формула стирається\nЯкщо формула ссилається на видалену клітинку - вона очищається", "Допомога");
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string cell_name = Converter26.To26(dataGridView1.SelectedCells[0].ColumnIndex) + (dataGridView1.SelectedCells[0].RowIndex + 1).ToString();
            textBox1.Text = dict[cell_name].Exp;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        void OpenFile()
        {
            if (MessageBox.Show("Всі незбережені зміни зникнуть", "Попередження", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Stream mystr = null;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if ((mystr = openFileDialog1.OpenFile()) != null)
                    {
                        using (mystr)
                        {
                            try
                            {
                                StreamReader sr = new StreamReader(mystr);
                                string scr = sr.ReadLine();
                                string scc = sr.ReadLine();
                                int cr = Convert.ToInt32(scr);
                                int cc = Convert.ToInt32(scc);
                                while (dataGridView1.RowCount < cr)
                                {
                                    AddRow();
                                }
                                while (dataGridView1.ColumnCount < cc)
                                {
                                    AddColumn();
                                }
                                for (int i = 0; i < cr; i++)
                                {
                                    for (int j = 0; j < cc; j++)
                                    {
                                        string cell_name = Converter26.To26(j) + (i + 1).ToString();
                                        dataGridView1.Rows[i].Cells[j].Value = sr.ReadLine();

                                    }
                                }
                                for (int i = 0; i < cr; i++)
                                {
                                    for (int j = 0; j < cc; j++)
                                    {
                                        string cell_name = Converter26.To26(j) + (i + 1).ToString();
                                        dict[cell_name].Exp = sr.ReadLine();
                                        RefreshCells();
                                    }
                                }
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Неправильний формат файлу", "Помилка");
                            }
                            catch { }
                        }
                    }
                }
            }
        }


    }
}