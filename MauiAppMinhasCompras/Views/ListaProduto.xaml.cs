using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;



public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

	public ListaProduto()
	{
		InitializeComponent();

        lst_produtos.ItemsSource = lista;
	}
    protected async override void OnAppearing()
    {
        List<Produto> tmp = await App.Db.GetAll();

        tmp.ForEach( i => lista.Add(i));
    }
    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", ex.Message, "OK");
        }


    }
    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"Total: {soma:C}";

        DisplayAlert("Total dos produtos", msg, "OK");
    }

    private CancellationTokenSource _debounceTimer;
    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = e.NewTextValue;

        // Cancela buscas anteriores e reinicia o timer
        _debounceTimer?.Cancel();
        _debounceTimer = new CancellationTokenSource();

        try
        {
            // Aguarda 300ms antes de executar a busca (evita chamadas a cada tecla pressionada)
            await Task.Delay(300, _debounceTimer.Token);

            // Busca os produtos no banco de dados
            List<Produto> resultado = await App.Db.Search(query);

            // Atualiza a lista sem limpar antes (evita UI vazia)
            lista.Clear();
            resultado.ForEach(i => lista.Add(i));
        }
        catch (TaskCanceledException)
        {
            // Se a busca foi cancelada, apenas ignore
        }
    }

    private void MenuItem_Clicked(object sender, EventArgs e)
    {

    }
}