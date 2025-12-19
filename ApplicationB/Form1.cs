using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Xml.Schema;  
using Newtonsoft.Json; 
using System.Net.Http;

namespace ApplicationB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ConfigurarCliente();
        }

        MqttClient client;
        void ConfigurarCliente()
        {
            client = new MqttClient("127.0.0.1");

            client.MqttMsgPublishReceived += Evento_MensagemRecebida;

            client.Connect(Guid.NewGuid().ToString());

            client.Subscribe(new string[] { "LojaModa/Sapatos" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        void Evento_MensagemRecebida(object sender, MqttMsgPublishEventArgs e)
        {
            string xmlRecebido = Encoding.UTF8.GetString(e.Message);
            string nomeFicheiro = "promocao_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";

            if (ValidarXML(xmlRecebido, "Promocao.xsd"))
            {
                File.WriteAllText(nomeFicheiro, xmlRecebido);

                this.Invoke((MethodInvoker)delegate {
                    // Agora mostramos na caixa de texto que criaste!
                    rtbNotificacoes.SelectionColor = Color.Green;
                    rtbNotificacoes.AppendText($"[{DateTime.Now:HH:mm:ss}] VÁLIDA: {nomeFicheiro} guardada! " + Environment.NewLine);
                    MessageBox.Show("Oba! Chegou uma promoção válida!");
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate {
                    rtbNotificacoes.SelectionColor = Color.Red;
                    rtbNotificacoes.AppendText($"[{DateTime.Now:HH:mm:ss}] INVÁLIDA: Tentativa de fraude detetada! " + Environment.NewLine);
                });
            }
        }

        public bool ValidarXML(string xmlConteudo, string caminhoXsd)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.Schemas.Add(null, caminhoXsd);

                settings.ValidationType = ValidationType.Schema;

                settings.ValidationEventHandler += (sender, args) =>
                {
                    if (args.Severity == XmlSeverityType.Error)
                        throw new Exception("Erro de Formato: " + args.Message);
                };

                using (XmlReader reader = XmlReader.Create(new StringReader(xmlConteudo), settings))
                {
                    while (reader.Read())
                    {
                    }

                    return true;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Promoção Inválida! Causa: " + ex.Message);
                return false;
            }
        }

        private async void btnSubscrever_Click(object sender, EventArgs e)
        {
            using (var clientHttp = new System.Net.Http.HttpClient())
            {
                var sub = new
                {
                    res_type = "subscription",
                    resource_name = "Sub_Matilde_Cliente",
                    event_type = 1, 
                    endpoint = "mqtt://127.0.0.1"
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(sub)
                    .Replace("res_type", "res-type")
                    .Replace("resource_name", "resource-name")
                    .Replace("event_type", "event");

                var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await clientHttp.PostAsync("http://localhost:54249/api/somiod/LojaModa/Sapatos/subs", content);

                    if (response.IsSuccessStatusCode)
                    {
                        lblStatus.Text = "Status: Subscrito com Sucesso! ";
                        lblStatus.ForeColor = Color.Green;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao ligar ao Middleware: " + ex.Message);
                }
            }
        }
    }
}
