using AutoMapper;
using CatalogoAPI.Context;
using CatalogoAPI.DTOs.Mappings;
using CatalogoAPI.Repositories;
using CatalogoAPI.Repositories.Impl;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApixUnitTests.UnitTests.controllers;

public class ProdutosUnitTestControllerConfig
{
    public IUnitOfWork repository;
    public IMapper mapper;
    public static DbContextOptions<AppDbContext> DbContextOptions { get; }

    public static string connectionString = "Server=localhost;DataBase=CatalogoDB;Uid=root;Pwd=1234";

    static ProdutosUnitTestControllerConfig()
    {
        DbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }

    public ProdutosUnitTestControllerConfig()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DTOMappingProfile());
        });

        mapper = config.CreateMapper();
        var context = new AppDbContext(DbContextOptions);
        repository = new UnitOfWorkImpl(context);
    }
}
