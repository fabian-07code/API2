using API2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace API2
{
    public partial class Form1 : Form
    {
        private readonly string apiUrl = "https://localhost:7225/api/Tabla2";
        private readonly HttpClient _httpClient;

        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await listaRegistros();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await CrearRegistro();
            await listaRegistros();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await EliminarRegistro();
            await listaRegistros();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await ActualizarRegistro();
            await listaRegistros();
        }

        private async Task listaRegistros()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
               string json = await response.Content.ReadAsStringAsync();
                var apiRespuesta= JsonConvert.DeserializeObject<dynamic>(json);

                var registroJson= apiRespuesta.resultado.ToString();
                var registros = JsonConvert.DeserializeObject<dynamic>(registroJson);

                var registroJsonResult= registros.result.ToString();

                var registroList = JsonConvert.DeserializeObject<List<Tabla2>>(registroJsonResult);
                dataGridView1.DataSource = registroList;

            }
            else
            {
                MessageBox.Show($"Error: {response.ReasonPhrase}");
            }

        }

        private async Task CrearRegistro()
        {
            Tabla2 registro = new Tabla2
            {
                Id = int.Parse(IDTXT.Text),
                Color = colorTXTS.Text,
                Descripcion = descripTXT.Text
            };

            string jsonData = JsonConvert.SerializeObject(registro);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Creado");
            else
                MessageBox.Show("Error");
        }

        private async Task ActualizarRegistro()
        {
            int id = int.Parse(IDTXT.Text);

            Tabla2 registroActualizado = new Tabla2
            {
                Id = id,
                Color = colorTXTS.Text,
                Descripcion = descripTXT.Text
            };

            string jsonData = JsonConvert.SerializeObject(registroActualizado);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{apiUrl}/{id}", content);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Actualizado");
            else
                MessageBox.Show("Error");
        }

        private async Task EliminarRegistro()
        {
            int id = int.Parse(IDTXT.Text);

            HttpResponseMessage response = await _httpClient.DeleteAsync($"{apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Registro eliminado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Error al eliminar registro: {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ActualizacionParcial()
        {
           var diccionario = new Dictionary<string, object>
            {
                { "Color", colorTXTS.Text},
                { "Descripcion", descripTXT.Text }
            };
            string jsonData = JsonConvert.SerializeObject(diccionario);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            int id = int.Parse(IDTXT.Text);
            HttpRequestMessage request= new HttpRequestMessage(new HttpMethod("PATCH"), $"{apiUrl}/{id}")
            {
                Content = content
            };

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Actualización parcial exitosa");
            else
                MessageBox.Show($"Error en actualización parcial: {response.ReasonPhrase}");

            dataGridView1.DataSource = null;


        }

        private void button5_Click(object sender, EventArgs e)
        {
            ActualizacionParcial().Wait();
        }
    }
}
