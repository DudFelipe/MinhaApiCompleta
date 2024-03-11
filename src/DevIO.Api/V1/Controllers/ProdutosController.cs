﻿using AutoMapper;
using DevIO.Api.Controllers;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.Api.V1.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/produtos")]
public class ProdutosController : MainController
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IProdutoService _produtoService;
    private readonly IMapper _mapper;

    public ProdutosController(INotificador notificador,
                              IProdutoRepository produtoReposioRepository,
                              IProdutoService produtoService,
                              IMapper mapper,
                              IUser user) : base(notificador, user)
    {
        _produtoRepository = produtoReposioRepository;
        _produtoService = produtoService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
    {
        return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
    {
        var produtoViewModel = await ObterProduto(id);

        if (produtoViewModel == null) return NotFound();

        return produtoViewModel;
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
        if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            return CustomResponse(produtoViewModel);

        produtoViewModel.Imagem = imagemNome;

        await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

        return CustomResponse(produtoViewModel);
    }

    [RequestSizeLimit(20000000)] //Definindo o tamanho do request para 20MB
    [HttpPost("AdicionarAlternativo")]
    public async Task<ActionResult<ProdutoViewModel>> AdicionarAlternativo(ProdutoImagemViewModel produtoViewModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var imgPrefixo = Guid.NewGuid() + "_";
        if (!await UploadAlternativo(produtoViewModel.ImagemUpload, imgPrefixo))
            return CustomResponse(produtoViewModel);

        produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;

        await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

        return CustomResponse(produtoViewModel);
    }

    [RequestSizeLimit(20000000)] //Definindo o tamanho do request para 20MB
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, ProdutoImagemViewModel produtoViewModel)
    {
        if (id != produtoViewModel.Id) return NotFound();

        var produtoAtualizacao = await ObterProduto(id);
        produtoViewModel.Imagem = produtoAtualizacao.Imagem;

        if (!ModelState.IsValid) return CustomResponse(ModelState);

        if (produtoViewModel.ImagemUpload != null)
        {
            var imgPrefixo = Guid.NewGuid() + "_";

            if (!await UploadAlternativo(produtoViewModel.ImagemUpload, imgPrefixo)) return CustomResponse(ModelState);

            produtoAtualizacao.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
        }

        produtoAtualizacao.Nome = produtoViewModel.Nome;
        produtoAtualizacao.Descricao = produtoViewModel.Descricao;
        produtoAtualizacao.Valor = produtoViewModel.Valor;
        produtoAtualizacao.Ativo = produtoViewModel.Ativo;

        await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

        return CustomResponse(produtoAtualizacao);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
    {
        var produto = await ObterProduto(id);

        if (produto == null) return NotFound();

        await _produtoService.Remover(id);

        return CustomResponse(produto);
    }

    private bool UploadArquivo(string arquivo, string imgNome)
    {
        if (string.IsNullOrEmpty(arquivo))
        {
            NotificarErro("Forneça uma imagem para este produto!");
            return false;
        }

        var imageDataByteArray = Convert.FromBase64String(arquivo);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

        if (System.IO.File.Exists(filePath))
        {
            NotificarErro("Já existe um arquivo com este nome!");
            return false;
        }

        System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

        return true;
    }

    private async Task<bool> UploadAlternativo(IFormFile arquivo, string imgPrefixo)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            NotificarErro("Forneça uma imagem para este produto!");
            return false;
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

        if (System.IO.File.Exists(path))
        {
            NotificarErro("Já existe um arquivo com este nome!");
            return false;
        }

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        return true;
    }

    private async Task<ProdutoViewModel> ObterProduto(Guid id)
    {
        var produto = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
        return produto;
    }
}