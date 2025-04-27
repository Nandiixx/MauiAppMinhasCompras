using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MauiAppMinhasCompras.Models;


namespace MauiAppMinhasCompras.Helpers
{
   
    public class SQLiteDatabaseHelper
    {
        // Propriedade que mantém a conexão assíncrona com o banco de dados SQLite.
        readonly SQLiteAsyncConnection _conn;

        // O construtor recebe o caminho para o banco de dados e cria a conexão.
        // Também cria a tabela "Produto" se ela ainda não existir.
        public SQLiteDatabaseHelper(string path)
        {
            // Estabelece a conexão com o banco de dados usando o caminho fornecido.
            _conn = new SQLiteAsyncConnection(path);

            // Cria a tabela "Produto" no banco de dados de forma assíncrona.
            // A função CreateTableAsync() espera a tabela ser criada (usando .Wait()).
            _conn.CreateTableAsync<Produto>().Wait();
        }

        // Método para inserir um novo registro na tabela "Produto".
        // Recebe um objeto do tipo Produto e insere-o no banco de dados.
        // Retorna uma Task com um valor inteiro (número de registros inseridos).
        public Task<int> Insert(Produto p)
        {
            // Insere o objeto Produto de forma assíncrona e retorna o resultado.
            return _conn.InsertAsync(p);
        }

        // Método para atualizar os dados de um registro existente na tabela "Produto".
        // Recebe um objeto Produto com os dados atualizados.
        // A string SQL usa a cláusula UPDATE para modificar os valores de Descricao, Quantidade e Preco com base no Id.
        // Retorna uma Task que resulta em uma lista de objetos Produto após a execução da consulta.
        public Task<List<Produto>> Update(Produto p)
        {
            // Define a instrução SQL para atualização.
            string sql = "UPDATE Produto SET Categoria=?, Descricao=?, Quantidade=?, Preco=? WHERE Id=?";

            // Executa a consulta assíncrona com os parâmetros fornecidos.
            return _conn.QueryAsync<Produto>(
                sql, p.Categoria, p.Descricao, p.Quantidade, p.Preco, p.Id
            );
        }

        // Método para excluir um produto com base no Id.
        // Recebe o Id do produto e exclui o registro correspondente da tabela "Produto".
      
        public Task<int> Delete(int id)
        {
            // Deleta o produto da tabela onde o Id corresponde ao fornecido.
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> ObterTodosProdutos()
        {
            return _conn.Table<Produto>().ToListAsync();
        }


        // Método para obter todos os registros da tabela "Produto".
        // Retorna uma lista de objetos Produto.
        public Task<List<Produto>> GetAll()
        {
            // Retorna todos os registros da tabela "Produto" como uma lista.
            return _conn.Table<Produto>().ToListAsync();
        }

        // Método para buscar produtos com base em uma palavra-chave na descrição.
        // Recebe uma string de pesquisa e retorna produtos cuja descrição contenha a palavra-chave.
        public Task<List<Produto>> Search(string q)
        {
            // Define a instrução SQL para realizar a busca na coluna "descricao".
            string sql = "SELECT * FROM Produto WHERE descricao LIKE '%" + q + "%'";

            // Executa a consulta assíncrona e retorna uma lista de produtos.
            return _conn.QueryAsync<Produto>(sql);
        }
        public async Task<List<Produto>> GetByCategory(string categoria)
        {
            return await _conn.Table<Produto>()
                .Where(p => string.IsNullOrWhiteSpace(p.Categoria) && categoria == "Sem Categoria")
                .ToListAsync();
        }
    }
}
