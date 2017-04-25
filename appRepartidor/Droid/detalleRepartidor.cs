
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.Geolocator;

namespace appRepartidor.Droid
{
	[Activity(Label = "detalleRepartidor")]
	public class detalleRepartidor : Activity
	{
		Button actualizarRepartidor;
		TextView distanciaRepartidor;
		public int id;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			SetContentView(Resource.Layout.detallePedido);
			base.OnCreate(savedInstanceState);
			actualizarRepartidor = FindViewById<Button>(Resource.Id.actualizarDetalle);
			distanciaRepartidor = FindViewById<TextView>(Resource.Id.txtDistanciaRepartidor);
			// Create your application here
			Bundle paquete = Intent.GetBundleExtra("bundle");
			id =paquete.GetInt("id");
			actualizarRepartidor.Click += actualzarRepartidorMetodo;
		}


		public async void actualzarRepartidorMetodo(object sender, EventArgs e)
		{
			string baseurl = "http://scmrocket.azurewebsites.net/api/pedidos/" + id.ToString();
			var Client = new HttpClient();
			Client.MaxResponseContentBufferSize = 256000;
			var uril = new Uri(baseurl);
			var response = Client.GetAsync(uril).Result;
			if (response.IsSuccessStatusCode)
			{
				var content = response.Content.ReadAsStringAsync().Result;
				var items = JsonConvert.DeserializeObject<PedidoModel>(content);
				Android.Locations.Location puntoa = new Android.Locations.Location("punto A");
				Android.Locations.Location puntob = new Android.Locations.Location("punto B");
				puntoa.Latitude = items.latitud;
				puntoa.Longitude = items.longitud;
				puntob.Latitude = await latitude();
				puntob.Longitude = await longitud();
				items.latitudRep = puntob.Latitude;
				items.longitudRep = puntob.Longitude;
				distanciaRepartidor.Text = puntoa.DistanceTo(puntob).ToString();
				if (Postear(items))
				{
					Toast.MakeText(ApplicationContext, "Actualizado", ToastLength.Long).Show();
				}
				else
				{
					Toast.MakeText(ApplicationContext, "Error al actualizar", ToastLength.Long).Show();
				}
			}
		}
		public async Task<Double> latitude()
		{
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;
			var position = await locator.GetPositionAsync(10000);
			return position.Latitude;
		}
		public async Task<Double> longitud()
		{
			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;
			var position = await locator.GetPositionAsync(10000);
			return position.Longitude;
		}
		public bool Postear(PedidoModel item)
		{
			string baseurl = "http://scmrocket.azurewebsites.net/api/pedidos/" + id.ToString();
			var Client = new HttpClient();
			Client.MaxResponseContentBufferSize = 256000;
			var uril = new Uri(baseurl);
			var json = JsonConvert.SerializeObject(item);
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = Client.PutAsync(uril, content).Result;
			if (response.IsSuccessStatusCode)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
