using AutoMapper;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class FornecedoresController : MainController
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IFornecedorService _fornecedorService;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IMapper _mapper;

    public FornecedoresController(IFornecedorRepository fornecedorRepository, 
                                  IMapper mapper,
                                  IFornecedorService fornecedorService,
                                  INotificador notificador,
                                  IEnderecoRepository enderecoRepository,
                                  IUser user) : base(notificador, user)
    {
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
        _fornecedorService = fornecedorService;
        _enderecoRepository = enderecoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
    {
        var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
        return CustomResponse(fornecedores);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
    {
        var fornecedor = await ObterFornecedorProdutosEndereco(id);

        if (fornecedor == null)
        {
            return NotFound();
        }

        return CustomResponse(fornecedor);
    }

    [CustomAuthorizization.ClaimsAuthorize("Fornecedor", "Adicionar")]
    [HttpPost]
    public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
        await _fornecedorService.Adicionar(fornecedor);

        return CustomResponse(fornecedorViewModel);
    }

    [CustomAuthorizization.ClaimsAuthorize("Fornecedor", "Atualizar")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
    {
        if(id != fornecedorViewModel.Id) return BadRequest();

        if (!ModelState.IsValid) return CustomResponse(ModelState); ;

        var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
        await _fornecedorService.Atualizar(fornecedor);

        return CustomResponse(fornecedorViewModel);
    }

    [CustomAuthorizization.ClaimsAuthorize("Fornecedor", "Excluir")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
    {
        var fornecedorViewModel = await ObterFornecedorEndereco(id);

        if (fornecedorViewModel == null) return NotFound();

        await _fornecedorService.Remover(id);

        return CustomResponse(fornecedorViewModel);
    }

    [HttpGet("obter-endereco/{id:guid}")]
    public async Task<ActionResult<EnderecoViewModel>> ObterEnderecoPorId(Guid id)
    {
        var enderecoViewModel = _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
        return CustomResponse(enderecoViewModel);
    }

    [CustomAuthorizization.ClaimsAuthorize("Fornecedor", "Atualizar")]
    [HttpPut("atualizar-endereco/{id:guid}")]
    public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
    {
        if (id != enderecoViewModel.Id) return BadRequest();

        if(!ModelState.IsValid) return CustomResponse(ModelState);

        var endereco = _mapper.Map<Endereco>(enderecoViewModel);
        await _fornecedorService.AtualizarEndereco(endereco);

        return CustomResponse(enderecoViewModel);
    }

    private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
    }

    private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
    }
}