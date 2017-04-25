using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;

namespace appRepartidor.Droid
{
	[Activity(Label = "appRepartidor", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		//int count = 1;
		ListView lv;
		EditText edtIdEleccion,edtNombreRepartidor;
		Button button;
		int id;
		string nombre;
		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			button = FindViewById<Button>(Resource.Id.myButton);
			edtIdEleccion = FindViewById<EditText>(Resource.Id.edtIdEleccion);
			edtNombreRepartidor = FindViewById<EditText>(Resource.Id.edtNombreRepartidor);
			lv = FindViewById<ListView>(Resource.Id.lvPedidos);
			//button.Click += delegate { button.Text = $"{count++} clicks!"; };
			List<string> nombres = new List<string>();
			List<PedidoModel> json = await LeerApi();
			for (int i = 0; i < json.Count; i++)
			{
				PedidoModel a = json.ElementAt(i);
				string nombreCompleto = a.id + " - " + a.telefono;
				nombres.Add(nombreCompleto);
			}
			ArrayAdapter ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, nombres);
			lv.SetAdapter(ListAdapter);
			button.Click += pedir;
		}

		public async Task<List<PedidoModel>> LeerApi()
		{
			string baseurl = "http://scmrocket.azurewebsites.net/api/pedidos";
			var Client = new HttpClient();
			Client.MaxResponseContentBufferSize = 256000;
			var uril = new Uri(baseurl);
			var response = await Client.GetAsync(uril);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var items = JsonConvert.DeserializeObject<List<PedidoModel>>(content);
				return items;
			}
			else
			{
				return null;
			}
		}

		public void pedir(object sender, EventArgs e)
		{
			Intent intento = new Intent(this, typeof(detalleRepartidor));
			Bundle contenedor = new Bundle();
			id = int.Parse(edtIdEleccion.Text);
			nombre = edtNombreRepartidor.Text;
			contenedor.PutInt("id", id);
			contenedor.PutString("telefono", nombre);
			intento.PutExtra("bundle", contenedor);
			StartActivity(intento);
		}
	}
}

