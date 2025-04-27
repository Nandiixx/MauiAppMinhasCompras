using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MauiAppMinhasCompras.Views.ListaProduto;

namespace MauiAppMinhasCompras.Views;



public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
    private CheckBox chk_categoria; // Declaração do campo chk_categoria

    public ListaProduto()
    {
        InitializeComponent();

        // Atribui a CheckBox existente no XAML ao campo chk_categoria
        chk_categoria = this.FindByName<CheckBox>("chk_categoria");

        lst_produtos.ItemsSource = lista;
    }

    protected async override void OnAppearing()
    {
        var categorias = new List<Categoria>
        {
            new Categoria { Nome = "Alimentos", IsSelected = false },
            new Categoria { Nome = "Bebidas", IsSelected = false },
            new Categoria { Nome = "Limpeza", IsSelected = false },
            new Categoria { Nome = "Higiene", IsSelected = false }
        };

        cv_categorias.ItemsSource = categorias;

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

    private async void chk_categoria_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            lista.Clear();

            if (e.Value) // Se o CheckBox estiver marcado
            {
                // Filtrar produtos pela categoria "Sem Categoria"
                List<Produto> produtosFiltrados = await App.Db.GetByCategory("Sem Categoria");
                produtosFiltrados.ForEach(p => lista.Add(p));
            }
            else
            {
                // Carregar todos os produtos
                List<Produto> todosProdutos = await App.Db.GetAll();
                todosProdutos.ForEach(p => lista.Add(p));
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Erro ao alternar o filtro", "OK");
        }
    }
    public class Categoria
    {
        public string Nome { get; set; } = string.Empty; // Define um valor padrão para evitar nulos
        public bool IsSelected { get; set; }
    }
    private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            // Obtenha as categorias selecionadas
            var categoriasSelecionadas = ((List<Categoria>)cv_categorias.ItemsSource)
                .Where(c => c.IsSelected)
                .Select(c => c.Nome)
                .ToList();

            // Se nenhuma categoria estiver selecionada, exiba todos os produtos
            if (categoriasSelecionadas.Count == 0)
            {
                lista.Clear();
                var todosProdutos = await App.Db.GetAll();
                todosProdutos.ForEach(p => lista.Add(p));
                return;
            }

            // Filtre os produtos com base nas categorias selecionadas
            var todosProdutosFiltrados = await App.Db.GetAll();
            var produtosFiltrados = todosProdutosFiltrados
                .Where(p => categoriasSelecionadas.Contains(p.Categoria))
                .ToList();

            // Atualize a lista exibida
            lista.Clear();
            produtosFiltrados.ForEach(p => lista.Add(p));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao atualizar a lista: {ex.Message}", "OK");
        }
    }
}

