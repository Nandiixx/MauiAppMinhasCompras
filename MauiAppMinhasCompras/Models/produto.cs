using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;



namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        private string _categoria = string.Empty;
        public string Categoria
        {
            get => _categoria;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("Por favor, preencha a categoria");
                }

                _categoria = value;
            }
        }

        string _descricao = string.Empty;
        public string Descricao
        {
            get => _descricao;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("Por favor, preencha a descrição");
                }

                _descricao = value;
            }
        }

        double _quantidade = 0;
        public double Quantidade
        {
            get => _quantidade;
            set
            {
                if (value < 0)
                {
                    throw new Exception("Por favor, insira uma quantidade válida");
                }

                _quantidade = value;
            }
        }

        double _preco = 0;
        public double Preco {
            get => _preco;
            set
            {
                if (value < 0)
                {
                    throw new Exception("Por favor, insira um preço válido");
                }

                _preco = value;
            }
        }
        public double Total { get => Quantidade * Preco; }
    }
}
