using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Json;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Nokiso
{
	public partial class CategoryPage : ContentPage
	{
		string mStoreUid;

		public CategoryPage (string storeuid)
		{
			InitializeComponent ();
			this.Title = "Category";
			mStoreUid = storeuid;
			GetCategories ();
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

					// Ca marche !

					ObservableCollection<Category> observableCategories = new ObservableCollection<Category> ();
					InitializeComponent ();
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
			Navigation.PushAsync (new ProductPage(category.Uid));
	//		GetProducts (category.Uid);
		}

		private async void GetCategories()
		{
			Dictionary<string, string> body = new Dictionary<string, string> ();
			body.Add ("store_uid", mStoreUid);
			Service s = new Service ("/category/list", body, "app");
			Task<JsonValue> result = s.CallAsync();

			JsonValue data = await result;

			if (!data.ContainsKey("erreur")) {
				//Console.WriteLine ("Result category : {0}", data.ToString());

			} else {
				//Console.WriteLine ("Something went wrong with the request");
			}

			UpdateUI (data, DeserializeCategory (data));
		}

		private async void OnLogoutButtonClicked (object sender, EventArgs e)
		{
			App.IsUserLoggedIn = false;
			Navigation.InsertPageBefore (new SignInPage (), this);
			await Navigation.PopAsync ();
		}

		public List<Category> DeserializeCategory(JsonValue datas)
		{

			CategoryJson json = new CategoryJson ();
			List<Category> categories = new List<Category> ();

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

					categories.Add(category);

				} catch (FormatException ex) {
					//Console.WriteLine(ex.ToString ());
				} catch (JsonException ex) {
					//Console.WriteLine(ex.ToString ());
				}			
			}
			return categories;
		}
	}
}