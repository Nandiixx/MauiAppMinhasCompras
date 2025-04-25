using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;



public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

	public ListaProduto()
	{
		InitializeComponent();
        CarregarProdutos();
        lst_produtos.ItemsSource = lista;
        collectionCategorias.ItemsSource = categorias;
    }
    private async void CarregarProdutos(string? categoria = null)
    {
        if (categoria == null)
        {
            lst_produtos.ItemsSource = await App.Db.GetAll();
        }
        else
        {
            var todosProdutos = await App.Db.GetAll();
            lst_produtos.ItemsSource = todosProdutos.Where(p => p.Categoria == categoria).ToList();
        }
    }
    protected async override void OnAppearing()
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        } 
        catch (Exception) 
        {
            await DisplayAlert("Erro", "Erro ao carregar os produtos", "OK");
        }
       
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
    private async void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string query = e.NewTextValue;

            lst_produtos.IsRefreshing = true;
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
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }
    private void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is MenuItem selecionado)
            {
                if (selecionado.BindingContext is Produto p)
                {
                    Navigation.PushAsync(new Views.EditarProduto
                    {
                        BindingContext = p,
                    });
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void MenuItem_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            if (sender is MenuItem selecionado)
            {
                if (selecionado.BindingContext is Produto p)
                {
                    bool confirm = await DisplayAlert("Por favor Confirme", $"Deseja excluir o {p.Descricao}", "Sim", "Não");

                    if (confirm)
                    {
                        await App.Db.Delete(p.Id);
                        lista.Remove(p);
                    }
                }
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Erro ao excluir o produto", "OK");
        }
    }
    private async void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            if (e.SelectedItem is Produto p)
            {
                await Navigation.PushAsync(new Views.EditarProduto
                {
                    BindingContext = p,
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Erro ao carregar os produtos", "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var todosProdutos = await App.Db.GetAll();
        var totalPorCategoria = todosProdutos
            .GroupBy(p => string.IsNullOrWhiteSpace(p.Categoria) ? "Sem Categoria" : p.Categoria)
            .Select(g => new { Categoria = g.Key, Total = g.Sum(p => p.Preco * p.Quantidade) })
            .ToList();

        string relatorio = string.Join("\n", totalPorCategoria.Select(c => $"{c.Categoria}: {c.Total:C}"));
        await DisplayAlert("Relatório de Gastos", relatorio, "OK");
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        var categoriasSelecionadas = categorias
            .Where(c => c.IsSelected)
            .Select(c => c.Nome)
            .ToList();

        if (categoriasSelecionadas.Count == 0)
        {
            // Se nenhuma categoria for selecionada, exibe todos os produtos
            CarregarProdutos();
        }
        else
        {
            var todosProdutos = await App.Db.GetAll();
            lst_produtos.ItemsSource = todosProdutos
                .Where(p => categoriasSelecionadas.Contains(p.Categoria))
                .ToList();
        }
    }

    ObservableCollection<CategoriaFiltro> categorias = new ObservableCollection<CategoriaFiltro>
    {
        new CategoriaFiltro { Nome = "Alimentos" },
        new CategoriaFiltro { Nome = "Bebidas" },
        new CategoriaFiltro { Nome = "Limpeza" },
        new CategoriaFiltro { Nome = "Higiene" }
    };

    public class CategoriaFiltro
    {
        public string Nome { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}

