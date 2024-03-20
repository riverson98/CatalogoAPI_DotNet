using CatalogoAPI.Controllers;
using CatalogoAPI.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoApixUnitTests.UnitTests.controllers;

public class ProdutosControllerTest : IClassFixture<ProdutosUnitTestControllerConfig>
{
    private readonly ProdutosController _controller;

    public ProdutosControllerTest(ProdutosUnitTestControllerConfig controller)
    {
        _controller = new ProdutosController(controller.repository, controller.mapper);
    }

    #region . GET .
    [Fact]
    public async Task GetProductById_ReturnOK_WhenItExist()
    {
        //Arrange
        var productId = 2;
        
        //act
        
        var data = await _controller.BuscaProdutosPorId(productId);
        
        //assert (xunit)
        //var result = Assert.IsType<OkObjectResult>(data.Result);
        //Assert.Equal(200, result.StatusCode);

        //assert (fluentAssertions)
        data.Result.Should().BeOfType<OkObjectResult>()
                   .Which.StatusCode.Should().Be(200);

    }

    [Fact]
    public async Task GetProductById_ReturnNotFound_WhenItDoesntExist()
    {
        var productId = 99;

        var data = await _controller.BuscaProdutosPorId(productId);

        data.Result.Should().BeOfType<NotFoundObjectResult>()
                   .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetProductById_ReturnBadRequest_WhenTheIdIsntValid()
    {
        var invalidId = -1;

        var data = await _controller.BuscaProdutosPorId(invalidId);

        data.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetAllProducts_ReturnOkAndAListOfProductsDto_WhenTheProductsExist()
    {
        var data = await _controller.BuscaTodosOsProdutos();

        data.Result.Should().BeOfType<OkObjectResult>()
                   .Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDTO>>()
                   .And.NotBeNull();
    }
    #endregion

    #region . POST .
    [Fact]
    public async Task PostProduct_ReturnCreatedStatusCode_WhenThePropertiesOfProductIsOk()
    {
        var productDto = new ProdutoDTO
        {
            Nome = "new product",
            Descricao = "Description",
            Preco = 10.99m,
            ImagemUrl = "imageFake.jpg",
            CategoriaId = 1
        };

        var data = await _controller.CriaProduto(productDto);

        data.Result.Should().BeOfType<CreatedAtRouteResult>()
            .Subject.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task PostProduct_ReturnBadRequest_WhenThisObjectIsNull()
    {
        ProdutoDTO? productDto = null;

        var data = await _controller.CriaProduto(productDto);

        data.Result?.Should().BeOfType<BadRequestResult>()
                    .Subject.StatusCode.Should().Be(400);
    }
    #endregion

    #region . PUT .
    [Fact]
    public async Task PutProduct_Update_ReturnOk_WhenThePropertiesOfTheProductAreCorrect()
    {
        var productDto = new ProdutoDTO
        {
            ProdutoId = 6,
            Nome = "This is an update on the new product",
            Descricao = "Description",
            Preco = 10.99m,
            ImagemUrl = "imageFake.jpg",
            CategoriaId = 1
        };

        var data = await _controller.AtualizaProduto(6, productDto) as ActionResult<ProdutoDTO>;

        data.Result.Should().NotBeNull();
        data.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PutProduct_Update_ReturnBadRequest_WhenThePropertiesOfTheProductArentCorrect()
    {
        var productDto = new ProdutoDTO
        {
            ProdutoId = 6,
            Nome = "This is an update on the new product",
            Descricao = "Description",
            Preco = 10.99m,
            ImagemUrl = "imageFake.jpg",
            CategoriaId = 1
        };

        var data = await _controller.AtualizaProduto(1000, productDto) as ActionResult<ProdutoDTO>;

        data.Result.Should().BeOfType<BadRequestResult>().Which
                                                         .StatusCode
                                                         .Should()
                                                         .Be(400);
    }
    #endregion

    #region . DELETE .
    [Fact]
    public async Task DeleteProductById_ReturnOk_WhenThisIdOfTheProductExists()
    {
        var productId = 6;

        var data = _controller.DeletaProdutoPorId(productId);

        data.Should().NotBeNull();
        data.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteProductById_ReturnNotFound_WhenThisIdOfProductDoesntExists()
    {
        var productId = 404;

        var data = _controller.DeletaProdutoPorId(productId);

        data.Should().NotBeNull();
        data.Result.Should().BeOfType<NotFoundObjectResult>();
    }
    #endregion

}
