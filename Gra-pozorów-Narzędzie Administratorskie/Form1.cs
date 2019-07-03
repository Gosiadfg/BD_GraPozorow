using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Data.Entity.Validation;
//using System.Object;

namespace Gra_pozorów_Narzędzie_Administratorskie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int wybor1 = 0; //1 dodawanie, 2 zmiana, 3 usuwanie
        private DataClasses1DataContext dc = new DataClasses1DataContext();
        private void odswiez(){
            dc = new DataClasses1DataContext();
            var uzytkowniczy = (from n in dc.GetTable<Gracz>()
                               select n).ToList();
            dataGridView1.DataSource = uzytkowniczy;

            /* var nazwy = (from n in dc.GetTable<Gracz>()
                          group n by n.Haslo
                          into m
                         select new {
                             Haslo = m.Key,
                             Liczba = m.Select(x=>x.Haslo).Count()
                         }).ToList();
                         */
            var nazwy = from n in dc.V_HasloGraczas
                        select n;
            dataGridView2.DataSource = nazwy;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            odswiez();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                switch (wybor1)
                {
                    case 1: //dodawanie

                        Gracz gracz = new Gracz
                        {
                            Nazwa = textBox2.Text,
                            Haslo = textBox3.Text,
                            Email = textBox4.Text
                        };

                        dc.Graczs.InsertOnSubmit(gracz);
                        dc.SubmitChanges();
                        break;

                    case 2: //zmiana
                        int id = Int32.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                        var gracz2 = from n in dc.GetTable<Gracz>()
                                     where n.GraczID == id
                                     select n;
                        gracz2.First().Haslo = textBox3.Text;
                        gracz2.First().Nazwa = textBox2.Text;
                        gracz2.First().Email = textBox4.Text;
                        gracz2.First().GraczID = Int32.Parse(textBox1.Text.ToString());
                        dc.SubmitChanges();

                        break;

                    case 3: //usuwanie
                        int GraczID = Int32.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                        var gamer = from n in dc.GetTable<Gracz>()
                                    where n.GraczID == GraczID
                                    select n;
                        dc.Graczs.DeleteOnSubmit(gamer.First());
                        dc.SubmitChanges();

                        break;
                    default:
                        MessageBox.Show("Nie wybrano operacji");
                        break;
                }
                odswiez();
            }
            catch (Exception f)
            {
                if (wybor1 == 1)
                    foreach (var n in dc.GetChangeSet().Inserts)
                    {
                        dc.GetTable(n.GetType()).DeleteOnSubmit(n);
                    }
                if (wybor1 == 3)
                    foreach (var n in dc.GetChangeSet().Deletes)
                    {
                        dc.GetTable(n.GetType()).InsertOnSubmit(n);
                    }
                MessageBox.Show(f.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            wybor1 = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
            wybor1 = 2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            wybor1 = 3;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox3.Text = row.Cells[2].Value.ToString();
                textBox4.Text = row.Cells[3].Value.ToString();
                break;
            }  
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int kod;
            try
            {
                
                switch (wybor1)
                {
                    case 1:
                        kod = dc.InsertGracz(textBox2.Text, textBox3.Text, textBox4.Text);
                        //MessageBox.Show(kod.ToString());
                        break;
                    case 2:
                        string nazwa = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                        kod = dc.UpdateGracz(Int32.Parse(textBox1.Text),
                            textBox2.Text, nazwa,
                            textBox3.Text, textBox4.Text);
                        //MessageBox.Show(kod.ToString());
                        break;
                    case 3:
                        kod = dc.DeleteGracz(Int32.Parse(textBox1.Text), textBox2.Text);
                        //MessageBox.Show(kod.ToString());
                        break;
                }
                odswiez();
        
            }
            catch (SqlException odbcEx)
            {
                 MessageBox.Show(odbcEx.Message);
            }
            catch (Exception f)
            {
                  MessageBox.Show(f.Message);
            }
        }
    }
}
