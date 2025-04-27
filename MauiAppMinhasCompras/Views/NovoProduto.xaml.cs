using MauiAppMinhasCompras.Models;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
    public NovoProduto()
    {
        InitializeComponent();
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto p = new Produto
            {
                Categoria = pickerCategoria.SelectedItem?.ToString(),
                Descricao = txtDescricao.Text,
                Preco = Convert.ToDouble(txtPreco.Text),
                Quantidade = Convert.ToDouble(txtQuantidade.Text)
            };

            await App.Db.Insert(p);

            await DisplayAlert("Sucesso", "Produto inserido com sucesso", "OK");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
}
