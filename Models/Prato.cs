using System.ComponentModel.DataAnnotations;

namespace MenuSolver.Models;

public class Prato
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do prato.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 80 caracteres.")]
    public string Nome { get; set; } = "";

    [Required(ErrorMessage = "Informe a descrição do prato.")]
    [StringLength(300, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 300 caracteres.")]
    public string Descricao { get; set; } = "";

    [Range(typeof(decimal), "0.01", "9999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "O preço deve estar entre R$ 0,01 e R$ 9.999,99.")]
    [Display(Name = "Preço")]
    public decimal Preco { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    [Display(Name = "Categoria")]
    public int CategoriaId { get; set; } // Muitos pratos pertencem a uma categoria.

    public string? ImagemPath { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.Now;
}
