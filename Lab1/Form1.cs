using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using AngouriMath;
using AngouriMath.Core.Exceptions;
using AngouriMath.Extensions;

namespace WindowsFormsApp1
{
   
  public partial class Form1 : Form
  {

    public Form1()
    {
      InitializeComponent();
      StartPosition = FormStartPosition.CenterScreen;
    }

    void textBoxFormulaKeyPress(object sender, KeyPressEventArgs e)
    {
      var number = e.KeyChar;
      //   Число                   Запятая         Backspace      Минус
      if (!Char.IsDigit(number) && number != 44 && number != 8 && number != 45)
      {
        e.Handled = true;
      }
    }
    //Для точности
    void TextBoxAccuracyKeyPress(object sender, KeyPressEventArgs e)
    {
      var number = e.KeyChar;
      if (!Char.IsDigit(number) && number != 44 && number != 8)
      {
        e.Handled = true;
      }
    }
    private void buttonStart_Click(object sender, EventArgs e)
    {
      try
      {
        double firstLimit = Convert.ToDouble(textBox2.Text);
        double secondLimit = Convert.ToDouble(textBox3.Text);
        if (firstLimit < secondLimit)
        {
          {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            //Парсим формулу из textbox
            Entity formula = Convert.ToString(textBoxFormula.Text);
            formula = formula.Simplify();

            double y = 0;
            //Строим график
            for (double i = firstLimit; i < secondLimit + 1; i += 1)
            {
              //основной график
              y = (double)formula.Substitute("x", i).EvalNumerical();
              chart1.Series[0].Points.AddXY(i, y);
            }

            double Fx = 0;
            double x = 0;

            //Точность
            double precision = Convert.ToDouble(textBoxAccuracy.Text);
            //Сигма для формул
            double sigma = precision / 2 - precision / 4;
            do
            {
              chart1.Series[1].Points.Clear();
              x = (firstLimit + secondLimit) / 2;
              double l = x - sigma;
              double r = x + sigma;
              double Fl = (double)formula.Substitute("x", l).EvalNumerical();
              double Fr = (double)formula.Substitute("x", r).EvalNumerical();

              Fx = (double)formula.Substitute("x", x).EvalNumerical();

              if (Fl <= Fr)
              {
                secondLimit = r;
              }
              else if (Fl > Fr)
              {
                firstLimit = l;
              }
              chart1.Series[1].Points.AddXY(x, Fx);
              textBox4.Text = Convert.ToString(Math.Round(x, 6));
            } while (Math.Abs(firstLimit - secondLimit) >= precision);
          }
        }
        else
        {
          chart1.Series[0].Points.Clear();
          chart1.Series[1].Points.Clear();
          MessageBox.Show("Неверные границы X", "Ошибка");
        }
      }
      catch (AngouriMathBaseException)
      {
        MessageBox.Show("Ошибка в формуле или введенных границах X", "Ошибка");
      }
      catch (FormatException)
      {
        MessageBox.Show("Ошибка формата введенных данных", "Ошибка");
      }
      catch (OverflowException)
      {
        MessageBox.Show("Слишком сложно", "Ошибка");
      }
      catch (Exception)
      {
        MessageBox.Show("Ошибка: {ex.Source}", "Ошибка");
      }

    }

  }
}