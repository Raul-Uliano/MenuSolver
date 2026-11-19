using System.ComponentModel.DataAnnotations;

namespace MenuSolver.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da categoria.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 60 caracteres.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = "";

    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    // Uma categoria pode ter muitos pratos (1:N).
    public List<Prato> Pratos { get; set; } = new();
}
