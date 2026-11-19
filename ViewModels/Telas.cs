using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MenuSolver.Models;

namespace MenuSolver.ViewModels;

public class InicioResumo
{
    public int TotalCategorias { get; set; }
    public int TotalPratos { get; set; }
    public List<Prato> UltimosPratos { get; set; } = new();
    public Dictionary<int, string> NomesCategorias { get; set; } = new();
}

public class ListaPaginada<T>
{
    public List<T> Itens { get; set; } = new();
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalItens { get; set; }
    public string? Busca { get; set; }
    public string Ordenar { get; set; } = "nome";
    public string Direcao { get; set; } = "asc";
    public int? CategoriaId { get; set; }
}

public class PratoFormulario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do prato.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 80 caracteres.")]
    public string Nome { get; set; } = "";

    [Required(ErrorMessage = "Informe a descrição do prato.")]
    [StringLength(300, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 300 caracteres.")]
    public string Descricao { get; set; } = "";

    [Range(typeof(decimal), "0.01", "9999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "Informe um preço entre R$ 0,01 e R$ 9.999,99.")]
    [Display(Name = "Preço (R$)")]
    public decimal Preco { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    [Display(Name = "Categoria")]
    public int CategoriaId { get; set; }

    [Display(Name = "Foto do prato (opcional)")]
    public IFormFile? Imagem { get; set; }
    public string? ImagemAtual { get; set; }
}

