using Newtonsoft.Json; // Se der erro, instala o NuGet Newtonsoft.Json
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt; // Se der erro, instala o NuGet M2Mqtt
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ApplicationB
{
    public partial class Form1 : Form
    {
        // 1. CONFIGURAÇÕES (Muda aqui a porta se a tua for diferente!)
        private const string BaseApiUrl = "http://localhost:54249/api/somiod";
        private const string AppName = "LojaPromocoes"; // Nome da tua Application na BD
        private MqttClient mqttClient;

        public Form1()
        {
            InitializeComponent();
        }

        // --- AÇÃO: BOTÃO "VER PRODUTOS" ---
        private async void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Pedimos à API os containers (produtos) da nossa loja
                    client.DefaultRequestHeaders.Add("somiod-discovery", "container");

                    var response = await client.GetAsync($"{BaseApiUrl}/{AppName}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var listaCaminhos = JsonConvert.DeserializeObject<List<string>>(json);

                        lstProdutos.Items.Clear();
                        foreach (var path in listaCaminhos)
                        {
                            // Extrai o nome do produto: /api/somiod/LojaMatilde/Produto -> Produto
                            lstProdutos.Items.Add(path.Split('/').Last());
                        }
                        await CarregarSubscricoesExistentes();
                    }
                    else
                    {
                        MessageBox.Show("Erro ao buscar produtos. Verifica se a API e a BD estão ligadas!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        // --- AÇÃO: BOTÃO "SUBSCREVER" ---
        // --- AÇÃO: BOTÃO "SUBSCREVER" CORRIGIDO ---
        private async void btnSubscrever_Click(object sender, EventArgs e)
        {
            if (lstProdutos.SelectedItem == null) return;
            string produto = lstProdutos.SelectedItem.ToString();
            string urlFinal = $"{BaseApiUrl}/{AppName}/{produto}/subs";

            var subData = new Dictionary<string, object>
            {
                { "res-type", "subscription" },
                { "resource-name", "Sub" + produto },
                { "evt", 1 },
                { "endpoint", $"{AppName}/{produto}" }
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(subData), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(urlFinal, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Sucesso! Agora está subscrito a: {produto} ");
                        LigarAoMQTT($"{AppName}/{produto}");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        MessageBox.Show($"Já está subscrito em {produto}!");
                        LigarAoMQTT($"{AppName}/{produto}");
                    }
                    else
                    {
                        string erroDescricao = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro {response.StatusCode}: {erroDescricao}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
        // --- LÓGICA DO MQTT ---
        private void LigarAoMQTT(string topicoRelativo)
        {
            try
            {
                string topicoCompleto = $"api/somiod/{topicoRelativo}";

                if (mqttClient == null)
                {
                    mqttClient = new MqttClient("127.0.0.1");
                    mqttClient.MqttMsgPublishReceived += AoReceberMensagem;
                }

                if (!mqttClient.IsConnected)
                {
                    mqttClient.Connect(Guid.NewGuid().ToString());
                }

                
                mqttClient.Subscribe(new string[] { topicoCompleto }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro no MQTT: " + ex.Message);
            }
        }

        private void AoReceberMensagem(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                string rawPayload = Encoding.UTF8.GetString(e.Message);
                string hora = DateTime.Now.ToString("HH:mm");
                string produto = e.Topic.Split('/').Last();

                var dados = JsonConvert.DeserializeObject<dynamic>(rawPayload);

                string mensagemLimpa = dados.resource.content;

                this.Invoke((MethodInvoker)delegate {
                    string novaLinha = $"[{hora}] {produto.ToUpper()} ➔ {mensagemLimpa}";
                    lstHistorico.Items.Insert(0, novaLinha);

                    if (mensagemLimpa.ToLower().Contains("desconto") || mensagemLimpa.Contains("%"))
                    {
                        Console.Beep();
                    }
                });
            }
            catch
            {
                this.Invoke((MethodInvoker)delegate {
                    lstHistorico.Items.Insert(0, "Raw: " + Encoding.UTF8.GetString(e.Message));
                });
            }
        }

        private async Task CarregarSubscricoesExistentes()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("somiod-discovery", "subscription");

                    var response = await client.GetAsync($"{BaseApiUrl}/{AppName}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var caminhosSub = JsonConvert.DeserializeObject<List<string>>(json);

                        foreach (var caminho in caminhosSub)
                        {
                            var partes = caminho.Split('/');
                            if (partes.Length >= 5)
                            {
                                string loja = partes[3];
                                string produto = partes[4];
                                string topicoParaLigar = $"{loja}/{produto}";

                                LigarAoMQTT(topicoParaLigar);

                                for (int i = 0; i < lstProdutos.Items.Count; i++)
                                {
                                    if (lstProdutos.Items[i].ToString() == produto)
                                    {
                                        lstProdutos.Items[i] = produto + " (Subscrito)"; 
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Erro ao recuperar subscrições: " + ex.Message);
            }
        }
    }
}