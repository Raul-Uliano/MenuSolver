using Microsoft.AspNetCore.Mvc;
using MenuSolver.Data;
using MenuSolver.Models;
using MenuSolver.ViewModels;

namespace MenuSolver.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        List<Categoria> categorias = RepositorioMemoria.Categorias();
        List<Prato> pratos = RepositorioMemoria.Pratos();
        InicioResumo resumo = new InicioResumo();
        resumo.TotalCategorias = categorias.Count;
        resumo.TotalPratos = pratos.Count;

        foreach (Categoria categoria in categorias)
            resumo.NomesCategorias[categoria.Id] = categoria.Nome;

        // IDs crescem com os cadastros; os ultimos estao no fim da lista.
        for (int i = pratos.Count - 1; i >= 0 && resumo.UltimosPratos.Count < 5; i--)
            resumo.UltimosPratos.Add(pratos[i]);

        return View(resumo);
    }

    public IActionResult Erro() => View();
}
