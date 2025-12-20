using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Somiod.AppA.Promocoes.Models;
using System.Text.Json;
using AplicationA.Models;

namespace AplicationA
{
    public partial class Form1 : Form
    {

        private static readonly HttpClient _httpClient = new HttpClient();

        // Altera para a porta onde o teu Middleware está a correr (ex: 44305, 9876, etc)
        private const string BaseUrl = "http://localhost:54249/api/somiod";

        public Form1()
        {
            InitializeComponent();
        }

        

        private async void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                // Agora a janela já existe e está visível
                await InitializeSomiodApplication();
                await PreencherListaProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar: {ex.Message}");
            }
        }

        private async Task InitializeSomiodApplication()
        {
            string appName = "LojaPromocoes";
            // O URL da aplicação específica: http://localhost:XXXX/api/somiod/LojaPromocoes
            string appUrl = $"{BaseUrl}/{appName}";

            try
            {
                // 1. Tentar fazer um GET para ver se a aplicação já existe
                var checkResponse = await _httpClient.GetAsync(appUrl);

                if (checkResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    // 2. Se o status for 404 (NotFound), então criamos pela primeira vez
                    var appData = new ApplicationDTO { ResourceName = appName };
                    string json = JsonSerializer.Serialize(appData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // O POST para criar é na raiz: /api/somiod
                    var createResponse = await _httpClient.PostAsync(BaseUrl, content);

                    if (createResponse.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Bem-vindo! A aplicação '{appName}' foi criada com sucesso.");
                    }
                    else
                    {
                        string error = await createResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro ao criar aplicação: {createResponse.StatusCode}\n{error}");
                    }
                }
                else if (checkResponse.IsSuccessStatusCode)
                {
                    // 3. Se o status for 200 (OK), a aplicação já existe, não fazemos nada
                    // Podes colocar um log ou mensagem discreta na status bar
                    Console.WriteLine("Aplicação já existente. Middleware pronto a usar.");
                }
                else
                {
                    // Caso aconteça outro erro qualquer (ex: 500 Erro de Servidor)
                    MessageBox.Show($"Erro inesperado ao verificar aplicação: {checkResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Se o Middleware nem estiver ligado, o código cai aqui
                MessageBox.Show($"Não foi possível ligar ao Middleware: {ex.Message}");
            }
        }

        private async Task PreencherListaProdutos()
        {
            string appName = "LojaPromocoes";
            string url = $"{BaseUrl}/{appName}";

            try
            {
                // 1. Criar o pedido GET
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                // 2. ADICIONAR O HEADER DE DISCOVERY (Obrigatório segundo a pág. 3)
                request.Headers.Add("somiod-discovery", "container");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    // O Middleware devolve uma lista de strings (paths)
                    var paths = JsonSerializer.Deserialize<List<string>>(json);

                    lbProdutos.Items.Clear();

                    if (paths != null)
                    {
                        foreach (var path in paths)
                        {
                            // O path vem como "/api/somiod/LojaPromocoes/NomeDoProduto"
                            // Vamos extrair apenas a última parte (o nome do produto)
                            string[] partes = path.Split('/');
                            string nomeProduto = partes[partes.Length - 1];

                            lbProdutos.Items.Add(nomeProduto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar produtos: " + ex.Message);
            }
        }

        private async void btnCreateContainer_Click(object sender, EventArgs e)
        {
            try
            {
                string appName = "LojaPromocoes"; // Nome da tua aplicação A
                string containerName = txtResnameContainer.Text.Trim(); // TextBox onde escreves o nome do produto

                if (string.IsNullOrEmpty(containerName))
                {
                    MessageBox.Show("Escreve um nome para o produto!");
                    return;
                }

                var dto = new ContainerDTO { ResourceName = containerName };
                string json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // URL para criar: http://localhost:XXXX/api/somiod/LojaPromocoes
                string url = $"{BaseUrl}/{appName}";

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Produto '{containerName}' criado com sucesso!");
                    txtResnameContainer.Text = "";
                    await PreencherListaProdutos();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private async void BtnUpdateContainer_Click(object sender, EventArgs e)
        {
            try
            {
                string appName = "LojaPromocoes";
                string oldContainerName = lbProdutos.SelectedItem.ToString();// Nome atual do produto
                if (oldContainerName.Length == 0)
                {
                    MessageBox.Show("Selecione um Produto primeiro");
                    return;
                }
                string newContainerName = txtResnameContainer.Text.Trim(); // Novo nome que queres dar

                var dto = new ContainerDTO { ResourceName = newContainerName };
                string json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // URL para atualizar: http://localhost:XXXX/api/somiod/LojaPromocoes/NomeAntigo
                string url = $"{BaseUrl}/{appName}/{oldContainerName}";

                // Usamos PutAsync para a operação de Update
                var response = await _httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Nome do produto atualizado!");
                    await PreencherListaProdutos();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao atualizar: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private async void btnCreateContent_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Obter dados da UI
                string appRn = "LojaPromocoes";
                string contRn = lbProdutos.SelectedItem.ToString(); // O produto selecionado

                if (string.IsNullOrEmpty(contRn))
                {
                    MessageBox.Show("Por favor, selecione um produto na lista primeiro!");
                    return;
                }

                // Dados da nova instância (Promoção)
                var dto = new ContentInstanceDTO
                { 
                    ContentType = "text/plain", // Podes obter de uma ComboBox
                    Content = txtcontent.Text.Trim() // O valor da promoção/preço
                };

                // 2. Serializar para JSON
                string json = JsonSerializer.Serialize(dto);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                // 3. Montar o URL conforme o teu Route: {appRn}/{contRn}
                // Exemplo: http://localhost:XXXX/api/somiod/LojaPromocoes/SapatoNike
                string url = $"{BaseUrl}/{appRn}/{contRn}";

                // 4. Enviar o POST
                var response = await _httpClient.PostAsync(url, httpContent);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Promoção criado com sucesso!\n{responseBody}");

                    // Limpar campo de valor após sucesso
                    txtcontent.Clear();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de ligação: " + ex.Message);
            }
        }



        private async void  lbProdutos_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Verificar se existe algum item selecionado
            if (lbProdutos.SelectedItem != null)
            {
                // 1. Obter o nome do produto selecionado
                string nomeSelecionado = lbProdutos.SelectedItem.ToString();

                // 2. Passar para a TextBox
                txtResnameContainer.Text = nomeSelecionado;

                await ListarContentInstances(nomeSelecionado);
            }
        }

        private async Task ListarContentInstances(string nomeProduto)
        {
            string appName = "LojaPromocoes";
            string urlDiscovery = $"{BaseUrl}/{appName}/{nomeProduto}";

            try
            {
                // 1. Discovery para obter os caminhos (paths)
                var request = new HttpRequestMessage(HttpMethod.Get, urlDiscovery);
                request.Headers.Add("somiod-discovery", "content-instance");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var listaCaminhos = JsonSerializer.Deserialize<List<string>>(json);

                    lbcontentInstance.Items.Clear();

                    if (listaCaminhos != null)
                    {
                        foreach (var caminho in listaCaminhos)
                        {
                            // 2. Para cada caminho, vamos buscar o objeto real (GET)
                            // Nota: O caminho já vem como "/api/somiod/..."
                            // Precisamos de juntar ao domínio/porta (ex: http://localhost:1234)
                            string urlBaseSomiod = BaseUrl.Replace("/api/somiod", "");
                            string urlCompleta = urlBaseSomiod + caminho;

                            var resItem = await _httpClient.GetAsync(urlCompleta);
                            if (resItem.IsSuccessStatusCode)
                            {
                                var ci = JsonSerializer.Deserialize<ContentInstanceDTO>(await resItem.Content.ReadAsStringAsync());

                                // 3. ADICIONAR OS DOIS À LISTA
                                // Formato: "Nome [Valor]"
                                lbcontentInstance.Items.Add($"{ci.ResourceName} | {ci.Content}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar lista detalhada: " + ex.Message);
            }
        }


        private async void lbContentInstances_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbcontentInstance.SelectedItem == null) return;

            string instanciaSelecionada = lbcontentInstance.SelectedItem.ToString();
            string produtoSelecionado = txtResnameContainer.Text;
            string appName = "LojaPromocoes";

            // URL Direto: api/somiod/Loja/Produto/Instancia
            string url = $"{BaseUrl}/{appName}/{produtoSelecionado}/{instanciaSelecionada}";

            try
            {
                // GET sem headers de discovery (devolve o objeto completo)
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    // Deserializa para o teu DTO que tem o campo "content"
                    var ci = JsonSerializer.Deserialize<ContentInstanceDTO>(json);

                    // Mostra o valor (ex: "19.99€") numa Label ou TextBox
                    txtcontent.Text = ci.Content.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao obter detalhes: " + ex.Message);
            }
        }

        private async void btndeleteContainer_Click(object sender, EventArgs e)
        {
            // 1. Obter o nome do produto que está na TextBox
            string containerName = txtResnameContainer.Text.Trim();
            string appName = "LojaPromocoes";

            // Validação básica
            if (string.IsNullOrEmpty(containerName))
            {
                MessageBox.Show("Por favor, selecione ou escreva o nome do produto que deseja apagar.");
                return;
            }

            // 2. Pedir confirmação ao utilizador (boa prática para DELETE)
            var confirmResult = MessageBox.Show(
                $"Tem a certeza que deseja apagar o produto '{containerName}' e todas as suas promoções?",
                "Confirmar Eliminação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes) return;

            try
            {
                // 3. Montar o URL: http://localhost:XXXX/api/somiod/LojaPromocoes/NomeDoProduto
                string url = $"{BaseUrl}/{appName}/{containerName}";

                // 4. Enviar o pedido DELETE
                var response = await _httpClient.DeleteAsync(url);

                // O teu Middleware devolve HttpStatusCode.NoContent (204) em caso de sucesso
                if (response.StatusCode == HttpStatusCode.NoContent || response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Produto '{containerName}' apagado com sucesso.");

                    // 5. LIMPEZA E ATUALIZAÇÃO DA UI
                    txtResnameContainer.Clear();
                    txtcontent.Clear();
                    lbcontentInstance.Items.Clear();

                    // Atualiza a lista principal de produtos para refletir a remoção
                    await PreencherListaProdutos();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao apagar ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de ligação ao Middleware: " + ex.Message);
            }
        }
    }
}
