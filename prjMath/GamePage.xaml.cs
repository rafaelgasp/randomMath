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
using System.Windows.Threading;

namespace prjMath
{
    public partial class GamePage : PhoneApplicationPage
    {
        Random r = new Random((int)DateTime.Now.Ticks);
        DispatcherTimer timer;
        DateTime tempo = new DateTime(1, 1, 1, 0, 0, 1);
        string[] operacoes = {"adicao", "subtracao", "multiplicacao", "divisao"};
        List<string> op = new List<string>();
        int result, componentes = 2;
        int[] numeros;
        int[] alternativas = new int[4];
        int indiceOperacao = 0;
        int questao = 0;
        int acertos = 0;
        bool isPaused = false;


        public GamePage()
        {
            InitializeComponent();

            if (!PhoneApplicationService.Current.State.ContainsKey("min"))
            {
                ConfigPage.InicializaPadrao();
            }

            questao = 0;
            acertos = 0;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            txtTempo.Text = "Tempo: 00:01";

            ProximaQuestao();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            tempo = tempo.AddSeconds(1);
            txtTempo.Text = "Tempo: "+GetTempo();
        }

        string GetTempo()
        {
            if (tempo.Minute < 10)
            {
                if (tempo.Second < 10)
                {
                    return "0" + tempo.Minute.ToString() + ":0" + tempo.Second.ToString();
                }
                else
                {
                    return "0" + tempo.Minute.ToString() + ":" + tempo.Second.ToString();
                }

            }
            else
            {
                if (tempo.Second < 10)
                {
                    return tempo.Minute.ToString() + ":0" + tempo.Second.ToString();
                }
                else
                {
                    return tempo.Minute.ToString() + ":" + tempo.Second.ToString();
                }
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            AtualizarTitulo();
        }

        void GerarEquacao()
        {
            numeros = new int[componentes];

            for(int i = 0; i < numeros.Length; i++)
            {
                numeros[i] = r.Next((int)PhoneApplicationService.Current.State["min"], (int)PhoneApplicationService.Current.State["max"]);
            }
            
            SortearOperacao();           
        }

        void SortearOperacao()
        {
            for (int i = 0; i < operacoes.Length; i++)
            {
                if((bool)PhoneApplicationService.Current.State[operacoes[i]])
                {
                    op.Add(operacoes[i]);
                }
            }

            indiceOperacao = r.Next(0, op.Count);

            if (op[indiceOperacao] == "adicao")
            {
                result = numeros[0] + numeros[1];
            }

            if (op[indiceOperacao] == "subtracao")
            {     
                result = numeros[0] - numeros[1];
            }

            if (op[indiceOperacao] == "multiplicacao")
            {
                result = numeros[0] * numeros[1];
            }
            
            if (op[indiceOperacao] == "divisao")
            {
                
                if (numeros[0] % numeros[1] == 0)
                {
                    result = numeros[0] / numeros[1];
                }
                else
                {
                    SortearOperacao();
                }
            }
        }

        void SortearAlternativas()
        {
            for(int i = 0; i < alternativas.Length;i++){
                alternativas[i] = r.Next(result-10, result+10);
                if(alternativas[i] == result)
                {
                    i--;
                    continue;
                }
                if (i > 0)
                {
                    for (int j = i; j < 0; j--)
                    {
                        if (alternativas[j] == alternativas[j--])
                        {
                            i--;
                            continue;
                        }
                    }                   
                }
            }

            alternativas[r.Next(0, 3)] = result;

            txtA.Text = alternativas[0].ToString();
            txtB.Text = alternativas[1].ToString();
            txtC.Text = alternativas[2].ToString();
            txtD.Text = alternativas[3].ToString();
        }

        void MontarEquacao()
        {
            if (op[indiceOperacao] == "adicao")
            {
                txtEquacao.Text = numeros[0] + " + " + numeros[1] + " ?";
            }

            if (op[indiceOperacao] == "subtracao")
            {
                txtEquacao.Text = numeros[0] + " - " + numeros[1] + " ?";
            }

            if (op[indiceOperacao] == "multiplicacao")
            {
                txtEquacao.Text = numeros[0] + " * " + numeros[1] + " ?";
            }

            if (op[indiceOperacao] == "divisao")
            {
                txtEquacao.Text = numeros[0] + " / " + numeros[1] + " ?";
            } 
        }

        void ProximaQuestao()
        {
            GerarEquacao();
            SortearOperacao();
            SortearAlternativas();
            MontarEquacao();
        }

        void AtualizarTitulo()
        {
            questao++;
            PageTitle.Text = "questão " + questao + "/" + PhoneApplicationService.Current.State["questoes"];           
        }

        void VerificarAlternativa(TextBlock txt)
        {
            if (!String.IsNullOrEmpty(txt.Text))
            {
                if (questao != Int32.Parse(PhoneApplicationService.Current.State["questoes"].ToString()))
                {
                    if (Int32.Parse(txt.Text.Trim()) == result)
                    {
                        ProximaQuestao();
                        AtualizarTitulo();
                        acertos++;
                    }
                    else
                    {
                        ProximaQuestao();
                        AtualizarTitulo();
                    }
                }
                else
                {
                    GravarPontuacao();
                    timer.Stop();
                    MessageBox.Show("Você Acertou " + acertos + " de " + Int32.Parse(PhoneApplicationService.Current.State["questoes"].ToString()) + " em " + GetTempo(), "Fim de Jogo", MessageBoxButton.OK);
                    if (MessageBox.Show("Deseja Jogar Novamente", "Atenção", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        questao = 0;
                        acertos = 0;
                        LimparTela();
                        timer.Start();
                        ProximaQuestao();
                    }
                    else
                    {
                        NavigationService.GoBack();
                    }
                }
            }            
        }

        void GravarPontuacao()
        {
            Database.Current.Insert<Pontuacao>(new Pontuacao { tempo = GetTempo(), acertos = this.acertos.ToString() + " acertos de " + PhoneApplicationService.Current.State["questoes"].ToString()+" questões"});
        }

        void LimparTela()
        {
            AtualizarTitulo();
            txtEquacao.Text = "";
            txtA.Text = "";
            txtB.Text = "";
            txtC.Text = "";
            txtD.Text = "";
            txtTempo.Text = "Tempo: 00:00";
            tempo = new DateTime(1, 1, 1, 0, 0, 0);
        }

        private void txtA_Tap(object sender, GestureEventArgs e)
        {
            VerificarAlternativa((TextBlock) sender);
        }

        private void txtB_Tap(object sender, GestureEventArgs e)
        {
            VerificarAlternativa((TextBlock) sender);
        }

        private void txtC_Tap(object sender, GestureEventArgs e)
        {
            VerificarAlternativa((TextBlock) sender);
        }

        private void txtD_Tap(object sender, GestureEventArgs e)
        {
            VerificarAlternativa((TextBlock) sender);
        }

        void BloquearBotoes()
        {
            txtA.Visibility = System.Windows.Visibility.Collapsed;
            txtB.Visibility = System.Windows.Visibility.Collapsed;
            txtC.Visibility = System.Windows.Visibility.Collapsed;
            txtD.Visibility = System.Windows.Visibility.Collapsed;
            txtEquacao.Visibility = System.Windows.Visibility.Collapsed;
        }

        void DesbloquearBotoes()
        {
            txtA.Visibility = System.Windows.Visibility.Visible;
            txtB.Visibility = System.Windows.Visibility.Visible;
            txtC.Visibility = System.Windows.Visibility.Visible;
            txtD.Visibility = System.Windows.Visibility.Visible;
            txtEquacao.Visibility = System.Windows.Visibility.Visible;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void image1_Tap(object sender, GestureEventArgs e)
        {
            if (isPaused)
            {
                imgPause.Visibility = System.Windows.Visibility.Visible;
                imgPlay.Visibility = System.Windows.Visibility.Collapsed;
                timer.Start();
                DesbloquearBotoes();
                isPaused = false;
            }
            else
            {
                imgPause.Visibility = System.Windows.Visibility.Collapsed;
                imgPlay.Visibility = System.Windows.Visibility.Visible;
                timer.Stop();
                BloquearBotoes();
                isPaused = true;
            }
        }

        private void imgPlay_Tap(object sender, GestureEventArgs e)
        {
            image1_Tap(sender, e);
        }
    }
}