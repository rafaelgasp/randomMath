using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace prjMath
{
    public partial class ConfigPage : PhoneApplicationPage
    {
        public ConfigPage()
        {
            InitializeComponent();
            if (!PhoneApplicationService.Current.State.ContainsKey("min"))
            {
                InicializaPadrao();
            }

            CarregarConfiguracoes();
        }

        private void txtMinimo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //Botão Voltar
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


        private void txtQuestoes_TextInputUpdate(object sender, TextCompositionEventArgs e)
        {
            
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int complexity = 1;
            var slider = sender as Slider;
            if (slider != null)
            {
                complexity = (int)slider.Value;
            }
            slider.Value = complexity;
        }

        public static void InicializaPadrao()
        {
            PhoneApplicationService.Current.State["min"] = 1;
            PhoneApplicationService.Current.State["max"] = 25;
            PhoneApplicationService.Current.State["adicao"] = true;
            PhoneApplicationService.Current.State["subtracao"] = true;
            PhoneApplicationService.Current.State["multiplicacao"] = false;
            PhoneApplicationService.Current.State["divisao"] = false;
            PhoneApplicationService.Current.State["questoes"] = 15;
        }

        public void CarregarConfiguracoes()
        {
            txtMinimo.Text = PhoneApplicationService.Current.State["min"].ToString();
            txtMaximo.Text = PhoneApplicationService.Current.State["max"].ToString();
            cbA.IsChecked = (bool) PhoneApplicationService.Current.State["adicao"];
            cbS.IsChecked = (bool)PhoneApplicationService.Current.State["subtracao"];
            cbM.IsChecked = (bool)PhoneApplicationService.Current.State["multiplicacao"];
            cbD.IsChecked = (bool)PhoneApplicationService.Current.State["divisao"];
            slider1.Value = Int32.Parse(PhoneApplicationService.Current.State["questoes"].ToString());
        }

        public bool contemNumeros(string texto)
        {
            if (texto.Where(c => char.IsNumber(c)).Count() > 0)
                return true;
            else
                return false;
        }

        //Salvar Configurações
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtMinimo.Text) || String.IsNullOrEmpty(txtMaximo.Text))
            {
                MessageBox.Show("Preencha todos os Campos!", "Atenção", MessageBoxButton.OK);        
                txtMinimo.Focus();
                
            }
            else
            {
                if (contemNumeros(txtMinimo.Text) || contemNumeros(txtMaximo.Text))
                {
                    MessageBox.Show("Utilize apenas Números!", "Atenção", MessageBoxButton.OK);
                    txtMinimo.Focus();
                }
                else
                {
                    if (!((bool)cbA.IsChecked || (bool)cbS.IsChecked || (bool)cbM.IsChecked || (bool)cbD.IsChecked))
                    {
                        MessageBox.Show("Selecione alguma operação!", "Atenção", MessageBoxButton.OK);
                    }
                    else
                    {
                        if (Int32.Parse(txtMinimo.Text) > Int32.Parse(txtMaximo.Text))
                        {
                            MessageBox.Show("Número Minímo Maior que Máximo!", "Atenção", MessageBoxButton.OK);
                            txtMinimo.Focus();
                        }
                        else
                        {
                            PhoneApplicationService.Current.State["min"] = Int32.Parse(txtMinimo.Text);
                            PhoneApplicationService.Current.State["max"] = Int32.Parse(txtMaximo.Text);
                            PhoneApplicationService.Current.State["adicao"] = cbA.IsChecked;
                            PhoneApplicationService.Current.State["subtracao"] = cbS.IsChecked;
                            PhoneApplicationService.Current.State["multiplicacao"] = cbM.IsChecked;
                            PhoneApplicationService.Current.State["divisao"] = cbD.IsChecked;
                            PhoneApplicationService.Current.State["questoes"] = slider1.Value.ToString();
                            NavigationService.GoBack();
                        }
                    }
                }                                
            }            
        }
    }
}