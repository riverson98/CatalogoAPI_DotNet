using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO catalogodb.produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataDeCadastro, CategoriaId) " +
                "VALUES('Coca-Cola Diet', 'Refrigerante de Cola 350ml', 5.45, 'cocacola.jpg', 50, now(), 1);");

            migrationBuilder.Sql("INSERT INTO catalogodb.produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataDeCadastro, CategoriaId) " +
                "VALUES('Lanche de Atum', 'Lanche de Atum com maionese', 8.50, 'atum.jpg', 10, now(), 2);");

            migrationBuilder.Sql("INSERT INTO catalogodb.produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataDeCadastro, CategoriaId) " +
                "VALUES('Pudim 100g', 'Pudim de leite condensado de 100g', 6.75, 'pudim.jpg', 20, now(), 3);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from Produtos");
        }
    }
}
