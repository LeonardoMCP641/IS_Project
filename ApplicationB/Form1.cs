using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json; // Resolve erro image_9c2570.png
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ApplicationB
{
    public partial class Form1 : Form
    {
        MqttClient client;
        private const string BaseUrl = "http://localhost:54249/api/somiod";

        public Form1()
        {
            InitializeComponent();
            ConfigurarMqtt();
            PreencherListaProdutos();
        }

        private void ConfigurarMqtt()
        {
            client = new MqttClient("127.0.0.1");
            client.MqttMsgPublishReceived += Evento_MensagemRecebida;
            client.Connect(Guid.NewGuid().ToString());
        }

        private async void btnSubscrever_Click(object sender, EventArgs e)
        {
            if (cbProdutos.SelectedItem == null)
            {
                MessageBox.Show("Matilde, escolhe um produto da lista primeiro!");
                return;
            }

            string produto = cbProdutos.SelectedItem.ToString();

            using (var httpClient = new HttpClient())
            {
                var subData = new
                {
                    res_type = "subscription",
                    resource_name = "Sub_Cliente_" + Guid.NewGuid().ToString().Substring(0, 5),
                    event_type = 1, // Creation
                    endpoint = "mqtt://127.0.0.1"
                };

                string json = JsonConvert.SerializeObject(subData)
                    .Replace("res_type", "res-type")
                    .Replace("resource_name", "resource-name")
                    .Replace("event_type", "event");

                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Resolve erro image_9c996f.png

                // Fazemos o POST para o Middleware para criar a subscrição
                var response = await httpClient.PostAsync($"{BaseUrl}/LojaPromocoes/{produto}/subs", content);

                if (response.IsSuccessStatusCode)
                {
                    // Subscrevemos também no Broker MQTT para este tópico específico
                    client.Subscribe(new string[] { "LojaPromocoes/" + produto }, new byte[] { 0 });
                    lblStatus.Text = "Status: Subscrito em " + produto;
                    lblStatus.ForeColor = Color.Green;
                }
            }
        }

        void Evento_MensagemRecebida(object sender, MqttMsgPublishEventArgs e)
        {
            string xmlRecebido = Encoding.UTF8.GetString(e.Message);
            string nomeFicheiro = "promo_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";

            if (ValidarXML(xmlRecebido, "Promocao.xsd"))
            {
                File.WriteAllText(nomeFicheiro, xmlRecebido); // Guarda no disco

                this.Invoke((MethodInvoker)delegate {
                    rtbPromocoes.SelectionColor = Color.Blue;
                    rtbPromocoes.AppendText($"[{DateTime.Now:HH:mm}] NOVA PROMOÇÃO: " + Environment.NewLine);
                    rtbPromocoes.SelectionColor = Color.Black;
                    rtbPromocoes.AppendText(xmlRecebido + Environment.NewLine + "----------------" + Environment.NewLine);
                });
            }
        }

        private async void PreencherListaProdutos()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // 1. Criar o pedido de descoberta (Discovery)
                    var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/LojaPromocoes");
                    request.Headers.Add("somiod-discovery", "container");

                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var paths = JsonConvert.DeserializeObject<List<string>>(json);

                        cbProdutos.Items.Clear(); // Limpa a lista antes de preencher

                        if (paths != null)
                        {
                            foreach (var path in paths)
                            {
                                // Extrai apenas o nome do produto do caminho
                                string[] partes = path.Split('/');
                                cbProdutos.Items.Add(partes[partes.Length - 1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao carregar lista: " + ex.Message);
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


        private void lblStatus_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
