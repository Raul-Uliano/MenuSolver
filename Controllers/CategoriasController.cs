using MenuSolver.Data;
using MenuSolver.Models;
using MenuSolver.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MenuSolver.Controllers;

public class CategoriasController : Controller
{
    private const int TamanhoPagina = 5;

    public IActionResult Index(string? busca, string ordenar = "nome", string direcao = "asc", int pagina = 1)
    {
        List<Categoria> filtradas = new List<Categoria>();
        foreach (Categoria categoria in RepositorioMemoria.Categorias())
        {
            if (string.IsNullOrWhiteSpace(busca) || categoria.Nome.Contains(busca.Trim(), StringComparison.OrdinalIgnoreCase))
                filtradas.Add(categoria);
        }

        if (ordenar != "quantidade") ordenar = "nome";
        if (direcao != "desc") direcao = "asc";

        // Aqui eu ordeno a lista sem usar LINQ.
        for (int i = 0; i < filtradas.Count; i++)
        {
            for (int j = i + 1; j < filtradas.Count; j++)
            {
                int comparacao;
                if (ordenar == "quantidade")
                    comparacao = filtradas[i].Pratos.Count.CompareTo(filtradas[j].Pratos.Count);
                else
                    comparacao = string.Compare(filtradas[i].Nome, filtradas[j].Nome, StringComparison.CurrentCultureIgnoreCase);
                if (direcao == "desc") comparacao = -comparacao;
                if (comparacao > 0)
                {
                    Categoria auxiliar = filtradas[i];
                    filtradas[i] = filtradas[j];
                    filtradas[j] = auxiliar;
                }
            }
        }

        int totalPaginas = (filtradas.Count + TamanhoPagina - 1) / TamanhoPagina;
        if (totalPaginas == 0) totalPaginas = 1;
        if (pagina < 1) pagina = 1;
        if (pagina > totalPaginas) pagina = totalPaginas;

        List<Categoria> itensPagina = new List<Categoria>();
        int primeiroIndice = (pagina - 1) * TamanhoPagina;
        for (int i = primeiroIndice; i < primeiroIndice + TamanhoPagina && i < filtradas.Count; i++)
            itensPagina.Add(filtradas[i]);

        ListaPaginada<Categoria> dados = new ListaPaginada<Categoria>
        {
            Itens = itensPagina, Pagina = pagina, TotalPaginas = totalPaginas,
            TotalItens = filtradas.Count, Busca = busca, Ordenar = ordenar, Direcao = direcao
        };
        return View(dados);
    }

    public IActionResult Detalhes(int id)
    {
        Categoria? categoria = RepositorioMemoria.CategoriaPorId(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    public IActionResult Criar() => View(new Categoria());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Criar(Categoria categoria)
    {
        categoria.Nome = (categoria.Nome ?? "").Trim();
        categoria.Descricao = categoria.Descricao?.Trim();
        if (RepositorioMemoria.NomeCategoriaEmUso(categoria.Nome))
            ModelState.AddModelError(nameof(Categoria.Nome), "Ja existe uma categoria com esse nome.");
        if (!ModelState.IsValid) return View(categoria);
        RepositorioMemoria.AdicionarCategoria(categoria);
        TempData["Sucesso"] = "Categoria cadastrada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Editar(int id)
    {
        Categoria? categoria = RepositorioMemoria.CategoriaPorId(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Editar(int id, Categoria categoria)
    {
        if (id != categoria.Id) return BadRequest();
        if (RepositorioMemoria.CategoriaPorId(id) == null) return NotFound();
        categoria.Nome = (categoria.Nome ?? "").Trim();
        categoria.Descricao = categoria.Descricao?.Trim();
        if (RepositorioMemoria.NomeCategoriaEmUso(categoria.Nome, id))
            ModelState.AddModelError(nameof(Categoria.Nome), "Ja existe uma categoria com esse nome.");
        if (!ModelState.IsValid) return View(categoria);
        RepositorioMemoria.EditarCategoria(categoria);
        TempData["Sucesso"] = "Categoria atualizada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Excluir(int id)
    {
        Categoria? categoria = RepositorioMemoria.CategoriaPorId(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    [HttpPost, ActionName("Excluir"), ValidateAntiForgeryToken]
    public IActionResult ConfirmarExclusao(int id)
    {
        if (RepositorioMemoria.CategoriaPorId(id) == null) return NotFound();
        if (!RepositorioMemoria.ExcluirCategoria(id))
        {
            TempData["Erro"] = "Esta categoria possui pratos. Exclua ou transfira os pratos antes de remove-la.";
            return RedirectToAction(nameof(Index));
        }
        TempData["Sucesso"] = "Categoria excluida com sucesso.";
        return RedirectToAction(nameof(Index));
    }
}
