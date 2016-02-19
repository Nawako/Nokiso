using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Json;
using System.Collections.ObjectModel;

namespace Nokiso
{
	public partial class ProductPage : ContentPage
	{
		string mCategoryUid;
		public ProductPage (string categoryuid)
		{
			InitializeComponent ();
			mCategoryUid = categoryuid;
			GetProducts ();
		}

		private void UpdateUI(JsonValue data, List<Product> products)
		{
			int responseCode = data ["code"];

			if (data != null) {
				if (responseCode != 0) {
					if (responseCode != 401) {
						Console.WriteLine ("Something went wrong with the request");
					} else {
						Console.WriteLine ("Invalid token or refresh token");
					}
				} else {
					Console.WriteLine ("Result : {0}", data);

					// Ca marche !

					ObservableCollection<Product> observableCategories = new ObservableCollection<Product> ();
					ListView lstView = new ListView ();
					lstView.ItemsSource = observableCategories;

					#region textCell
					lstView.ItemTemplate = new DataTemplate (typeof(TextCell));
					lstView.ItemTemplate.SetBinding (TextCell.TextProperty, "name");
					lstView.ItemTemplate.SetBinding (TextCell.DetailProperty, "uid");
					lstView.ItemTapped += OnTap;
					#endregion

					//Observable Collection ! La ListView se met à jour en temps réel dès qu'on ajoute un objet.
					Content = lstView;

					foreach (Product product in products) {
						observableCategories.Add (new Product () { name = product.name, uid = product.uid, available = product.available, price = product.price });			
					}
				}
			}
		}

		// Gestion du tap sur la ListView
		void OnTap (object sender, ItemTappedEventArgs e)
		{
			Product product = (Product) e.Item;
			//		GetProducts (category.Uid);
		}

		private async void GetProducts()
		{
			Dictionary<string, string> body = new Dictionary<string, string> ();
			body.Add ("category_uid", mCategoryUid);
			Service s = new Service ("/product/list", body, "user");
			Task<JsonValue> result = s.CallAsync();

			JsonValue data = await result;

			if (!data.ContainsKey("erreur")) {
				Console.WriteLine ("Result product : {0}", data.ToString());

			} else {
				Console.WriteLine ("Something went wrong with the request");
			}

			UpdateUI (data, DeserializeProduct (data));
		}

		private async void OnLogoutButtonClicked (object sender, EventArgs e)
		{
			App.IsUserLoggedIn = false;
			Navigation.InsertPageBefore (new SignInPage (), this);
			await Navigation.PopAsync ();
		}

		public List<Product> DeserializeProduct(JsonValue datas)
		{

			ProductJson json = new ProductJson ();
			List<Product> products = new List<Product> ();

			// Seriously, obligé de passer en string pour que ça deserialize. 
			// WTF.
			string output = datas.ToString ();

			json = JsonConvert.DeserializeObject<ProductJson>(output);

			// Boucle qui parse le JSON pour en extraire les categories
			for (int i = 0; i < json.result.Count; i++) {
				try {
					Product product = new Product ();
					product.name = json.result[i].name;
					product.available = json.result[i].available;
					product.price = json.result[i].price;
					product.uid = json.result[i].uid;

					products.Add(product);

				} catch (FormatException ex) {
					Console.WriteLine(ex.ToString ());
				} catch (JsonException ex) {
					Console.WriteLine(ex.ToString ());
				}			
			}
			return products;
		}
	}
}

