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

        private async void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("A tentar ligar ao Middleware...");
            await InitializeSomiodApplication();
        }

        private async Task InitializeSomiodApplication()
        {

            try
            {
                var appData = new ApplicationDTO { ResourceName = "LojaPromocoes" };
                string json = JsonSerializer.Serialize(appData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(BaseUrl, content);

                // ISTO VAI AJUDAR A VER O ERRO:
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Sucesso! O Middleware criou: {responseBody}");
                }
                else
                {
                    MessageBox.Show($"O Middleware recusou! Erro: {response.StatusCode}\nResposta: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro de ligação: {ex.Message}");
            }
            //try
            //{
            //    var appData = new ApplicationDTO
            //    {
            //        ResourceName = "LojaPromocoes" // O nome da tua App A
            //    };

            //    string json = JsonSerializer.Serialize(appData);
            //    var content = new StringContent(json, Encoding.UTF8, "application/json");

            //    // Segundo o enunciado: "The Web service URL must always start by: http://<domain:port>/api/somiod/..."
            //    // Para criar uma aplicação, o POST é feito na raiz /api/somiod
            //    var response = await _httpClient.PostAsync(BaseUrl, content);

            //    if (response.StatusCode == HttpStatusCode.Created)
            //    {
            //        MessageBox.Show("Aplicação registada com sucesso no SOMIOD!");
            //    }
            //    else if (response.StatusCode == HttpStatusCode.Conflict)
            //    {
            //        // O teu código da API devolve Conflict() se já existir
            //        Console.WriteLine("A aplicação já existe no middleware. Prosseguindo...");
            //    }
            //    else
            //    {
            //        string error = await response.Content.ReadAsStringAsync();
            //        MessageBox.Show($"Erro ao inicializar: {response.StatusCode}\n{error}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Erro de ligação ao Middleware: {ex.Message}");
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
