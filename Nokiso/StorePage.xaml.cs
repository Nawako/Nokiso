using System;
using System.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using Newtonsoft.Json;

using Xamarin.Forms;
using System.Collections.ObjectModel;


namespace Nokiso
{
	public partial class StorePage : ContentPage
	{
//		static int PRODUCT = 1;
//		static int CATEGORY = 0;
		
		public StorePage ()
		{
			InitializeComponent ();
			this.Title = "Store";

			GetStore ();
		}
			
		private void UpdateUI(JsonValue data, List<Category> categories)
		{
			int responseCode = data ["code"];

			if (data != null) {
				if (responseCode != 0) {
					if (responseCode != 401) {
						//Console.WriteLine ("Something went wrong with the request");
					} else {
						//Console.WriteLine ("Invalid token or refresh token");
					}
				} else {
					//Console.WriteLine ("Result : {0}", data);

					// Ca marche!
					ObservableCollection<Category> observableCategories = new ObservableCollection<Category> ();
					ListView lstView = new ListView ();
					lstView.ItemsSource = observableCategories;

					#region textCell
					lstView.ItemTemplate = new DataTemplate (typeof(TextCell));
					lstView.ItemTemplate.SetBinding (TextCell.TextProperty, "Name");
					lstView.ItemTemplate.SetBinding (TextCell.DetailProperty, "Uid");
					lstView.ItemTapped += OnTap;
					#endregion

					//Observable Collection ! La ListView se met à jour en temps réel dès qu'on ajoute un objet.
					Content = lstView;

					foreach (Category category in categories) {
						observableCategories.Add (new Category () { Name = category.Name, Uid = category.Uid });			
					}
				}
			}
		}

		// Gestion du tap sur la ListView
		void OnTap (object sender, ItemTappedEventArgs e)
		{
			Category category = (Category) e.Item;
			Navigation.PushAsync (new CategoryPage(category.Uid));
	//		GetProducts (category.Uid);
		}

		private async void GetStore()
		{
			Service s = new Service ("/store/list", "app");
			Task<JsonValue> result = s.CallAsync();
	
			JsonValue data = await result;

			if (!data.ContainsKey("erreur")) {
				string uid = data["result"][0]["uid"];

				s.Operation = "/category/list";
				s.Body.Add ("store_uid", uid);
				Task<JsonValue> result_category = s.CallAsync ();
				JsonValue data_category = await result_category;

				//Console.WriteLine ("Result category : {0}", data_category);

			} else {
				//Console.WriteLine ("Something went wrong with the request");
			}

			UpdateUI (data, DeserializeStore (data));
		}

		private async void OnLogoutButtonClicked (object sender, EventArgs e)
		{
			App.IsUserLoggedIn = false;
			Navigation.InsertPageBefore (new SignInPage (), this);
			await Navigation.PopAsync ();
		}
			
		public List<Category> DeserializeStore(JsonValue datas)
		{
			
			CategoryJson json = new CategoryJson ();
			List<Category> stores = new List<Category> ();

			// Seriously, obligé de passer en string pour que ça deserialize. 
			// WTF.
			string output = datas.ToString ();

			json = JsonConvert.DeserializeObject<CategoryJson>(output);

			// Alert si aucun produit
			if (json.result.Count == 0) {
				DisplayAlert ("No items", "Oops, it seems that no items are available in this page.", "Ok");
			}

			// Boucle qui parse le JSON pour en extraire les categories
			for (int i = 0; i < json.result.Count; i++) {
				try {
					Category category = new Category ();
					category.Name = json.result[i].name;
					category.Uid = json.result[i].uid;

					stores.Add(category);

				} catch (FormatException ex) {
					//Console.WriteLine(ex.ToString ());
				} catch (JsonException ex) {
					//Console.WriteLine(ex.ToString ());
				}			
			}
			return stores;
		}
	}
}

