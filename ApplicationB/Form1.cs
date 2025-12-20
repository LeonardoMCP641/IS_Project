using Newtonsoft.Json; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt; 
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ApplicationB
{
    public partial class Form1 : Form
    {
        
        private const string BaseApiUrl = "http://localhost:54249/api/somiod";
        private const string AppName = "LojaPromocoes"; 
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

            // 1. Obter o índice e o nome limpo (caso já tenha o sufixo)
            int indexSelecionado = lstProdutos.SelectedIndex;
            string itemTexto = lstProdutos.Items[indexSelecionado].ToString();
            string produto = itemTexto.Replace(" (Subscrito)", "").Trim();

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

                    // 2. Se correu bem OU se já existia (Conflict)
                    if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        if (response.IsSuccessStatusCode)
                            MessageBox.Show($"Sucesso! Agora está subscrito a: {produto}");
                        else
                            MessageBox.Show($"Já estava subscrito em: {produto}");

                        // --- ATUALIZAÇÃO DA LISTAGEM NA UI ---
                        // Mudamos o texto do item na lista para dar feedback visual
                        lstProdutos.Items[indexSelecionado] = produto + " (Subscrito)";

                        // Ligar ao MQTT
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

        private async void btndessubscrever_Click(object sender, EventArgs e)
        {
            // 1. Verificar se há um produto selecionado
            if (lstProdutos.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um produto da lista.");
                return;
            }

            // 2. Limpar o nome do produto (remover o sufixo " (Subscrito)" se existir)
            string itemTexto = lstProdutos.SelectedItem.ToString();
            string produto = itemTexto.Replace(" (Subscrito)", "").Trim();

            // 3. Definir o nome da subscrição (deve ser igual ao que usaste no btnSubscrever)
            string subName = "Sub" + produto;

            // URL seguindo o padrão do enunciado: /api/somiod/App/Container/subs/SubName
            string urlFinal = $"{BaseApiUrl}/{AppName}/{produto}/subs/{subName}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 4. Enviar o pedido DELETE para o Middleware
                    var response = await client.DeleteAsync(urlFinal);

                    if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        MessageBox.Show($"Sucesso! Deixou de seguir o produto: {produto}");

                        // 5. Parar de receber mensagens MQTT para este tópico
                        string topicoCompleto = $"api/somiod/{AppName}/{produto}";
                        if (mqttClient != null && mqttClient.IsConnected)
                        {
                            mqttClient.Unsubscribe(new string[] { topicoCompleto });
                        }

                        // 6. Atualizar a UI: Remover o texto "(Subscrito)" da lista
                        int index = lstProdutos.SelectedIndex;
                        lstProdutos.Items[index] = produto;

                        // Opcional: Limpar o histórico se quiseres
                        lstHistorico.Items.Insert(0, $"--- Parou de seguir {produto} ---");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MessageBox.Show("A subscrição já não existe no servidor.");
                        // Limpa a UI mesmo assim
                        lstProdutos.Items[lstProdutos.SelectedIndex] = produto;
                    }
                    else
                    {
                        string erroDescricao = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro ao remover subscrição {response.StatusCode}: {erroDescricao}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao dessubscrever: " + ex.Message);
            }
        }
    }
}