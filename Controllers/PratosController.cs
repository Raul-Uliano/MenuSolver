using MenuSolver.Data;
using MenuSolver.Models;
using MenuSolver.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MenuSolver.Controllers;

public class PratosController : Controller
{
    private readonly IWebHostEnvironment ambiente;
    private const int TamanhoPagina = 5;
    private const long TamanhoMaximoFoto = 2 * 1024 * 1024;

    // O ambiente e fornecido pelo ASP.NET Core para encontrarmos a pasta wwwroot.
    public PratosController(IWebHostEnvironment ambiente)
    {
        this.ambiente = ambiente;
    }

    private void CarregarCategorias(int? selecionada = null)
    {
        ViewBag.Categorias = new SelectList(RepositorioMemoria.Categorias(), "Id", "Nome", selecionada);
    }

    public IActionResult Index(string? busca, int? categoriaId, string ordenar = "nome", string direcao = "asc", int pagina = 1)
    {
        List<Prato> filtrados = new List<Prato>();
        foreach (Prato prato in RepositorioMemoria.Pratos())
        {
            bool nomeCorresponde = string.IsNullOrWhiteSpace(busca) ||
                prato.Nome.Contains(busca.Trim(), StringComparison.OrdinalIgnoreCase);
            bool categoriaCorresponde = !categoriaId.HasValue || categoriaId == 0 || prato.CategoriaId == categoriaId;
            if (nomeCorresponde && categoriaCorresponde) filtrados.Add(prato);
        }

        if (ordenar != "nome" && ordenar != "preco" && ordenar != "categoria") ordenar = "nome";
        if (direcao != "desc") direcao = "asc";

        Dictionary<int, string> nomes = new Dictionary<int, string>();
        foreach (Categoria categoria in RepositorioMemoria.Categorias())
            nomes[categoria.Id] = categoria.Nome;

        // Ordenar os pratos do jeito que foi escolhido na tela.
        for (int i = 0; i < filtrados.Count; i++)
        {
            for (int j = i + 1; j < filtrados.Count; j++)
            {
                int comparacao;
                if (ordenar == "preco")
                    comparacao = filtrados[i].Preco.CompareTo(filtrados[j].Preco);
                else if (ordenar == "categoria")
                    comparacao = string.Compare(nomes[filtrados[i].CategoriaId], nomes[filtrados[j].CategoriaId], StringComparison.CurrentCultureIgnoreCase);
                else
                    comparacao = string.Compare(filtrados[i].Nome, filtrados[j].Nome, StringComparison.CurrentCultureIgnoreCase);

                if (direcao == "desc") comparacao = -comparacao;
                if (comparacao > 0)
                {
                    Prato auxiliar = filtrados[i];
                    filtrados[i] = filtrados[j];
                    filtrados[j] = auxiliar;
                }
            }
        }

        int totalPaginas = (filtrados.Count + TamanhoPagina - 1) / TamanhoPagina;
        if (totalPaginas == 0) totalPaginas = 1;
        if (pagina < 1) pagina = 1;
        if (pagina > totalPaginas) pagina = totalPaginas;

        List<Prato> itensPagina = new List<Prato>();
        int primeiroIndice = (pagina - 1) * TamanhoPagina;
        for (int i = primeiroIndice; i < primeiroIndice + TamanhoPagina && i < filtrados.Count; i++)
            itensPagina.Add(filtrados[i]);

        ViewBag.NomesCategorias = nomes;
        CarregarCategorias(categoriaId);
        return View(new ListaPaginada<Prato>
        {
            Itens = itensPagina, Pagina = pagina, TotalPaginas = totalPaginas,
            TotalItens = filtrados.Count, Busca = busca, CategoriaId = categoriaId,
            Ordenar = ordenar, Direcao = direcao
        });
    }

    public IActionResult Detalhes(int id)
    {
        Prato? prato = RepositorioMemoria.PratoPorId(id);
        if (prato == null) return NotFound();
        ViewBag.NomeCategoria = RepositorioMemoria.CategoriaPorId(prato.CategoriaId)?.Nome ?? "Categoria nao encontrada";
        return View(prato);
    }

    public IActionResult Criar()
    {
        CarregarCategorias();
        return View(new PratoFormulario());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(PratoFormulario formulario)
    {
        Validar(formulario);
        if (!ModelState.IsValid)
        {
            CarregarCategorias(formulario.CategoriaId);
            return View(formulario);
        }
        string? foto = await SalvarImagem(formulario.Imagem);
        Prato prato = new Prato
        {
            Nome = formulario.Nome.Trim(), Descricao = formulario.Descricao.Trim(),
            Preco = formulario.Preco, CategoriaId = formulario.CategoriaId, ImagemPath = foto
        };
        RepositorioMemoria.AdicionarPrato(prato);
        TempData["Sucesso"] = "Prato cadastrado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Editar(int id)
    {
        Prato? prato = RepositorioMemoria.PratoPorId(id);
        if (prato == null) return NotFound();
        CarregarCategorias(prato.CategoriaId);
        return View(new PratoFormulario
        {
            Id = prato.Id, Nome = prato.Nome, Descricao = prato.Descricao,
            Preco = prato.Preco, CategoriaId = prato.CategoriaId, ImagemAtual = prato.ImagemPath
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, PratoFormulario formulario)
    {
        if (id != formulario.Id) return BadRequest();
        Prato? antigo = RepositorioMemoria.PratoPorId(id);
        if (antigo == null) return NotFound();
        Validar(formulario);
        if (!ModelState.IsValid)
        {
            formulario.ImagemAtual = antigo.ImagemPath;
            CarregarCategorias(formulario.CategoriaId);
            return View(formulario);
        }

        string? fotoAnterior = antigo.ImagemPath;
        string? foto = fotoAnterior;
        if (formulario.Imagem != null) foto = await SalvarImagem(formulario.Imagem);
        bool sucesso = RepositorioMemoria.EditarPrato(new Prato
        {
            Id = id, Nome = formulario.Nome.Trim(), Descricao = formulario.Descricao.Trim(),
            Preco = formulario.Preco, CategoriaId = formulario.CategoriaId, ImagemPath = foto
        });
        if (!sucesso)
        {
            if (foto != fotoAnterior) ApagarImagem(foto);
            return NotFound();
        }
        if (foto != fotoAnterior) ApagarImagem(fotoAnterior);
        TempData["Sucesso"] = "Prato atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Excluir(int id)
    {
        Prato? prato = RepositorioMemoria.PratoPorId(id);
        if (prato == null) return NotFound();
        ViewBag.NomeCategoria = RepositorioMemoria.CategoriaPorId(prato.CategoriaId)?.Nome;
        return View(prato);
    }

    [HttpPost, ActionName("Excluir"), ValidateAntiForgeryToken]
    public IActionResult ConfirmarExclusao(int id)
    {
        Prato? excluido = RepositorioMemoria.ExcluirPrato(id);
        if (excluido == null) return NotFound();
        ApagarImagem(excluido.ImagemPath);
        TempData["Sucesso"] = "Prato excluido com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private void Validar(PratoFormulario formulario)
    {
        if (!RepositorioMemoria.ExisteCategoria(formulario.CategoriaId))
            ModelState.AddModelError(nameof(PratoFormulario.CategoriaId), "Escolha uma categoria cadastrada.");

        IFormFile? arquivo = formulario.Imagem;
        if (arquivo == null) return;
        if (arquivo.Length == 0 || arquivo.Length > TamanhoMaximoFoto)
            ModelState.AddModelError(nameof(PratoFormulario.Imagem), "A imagem deve ter ate 2 MB.");

        string extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
        if (extensao != ".jpg" && extensao != ".jpeg" && extensao != ".png" && extensao != ".webp")
            ModelState.AddModelError(nameof(PratoFormulario.Imagem), "Envie JPG, PNG ou WEBP.");
        // Extensao, tamanho e Content-Type sao verificacoes basicas de upload.
        if (arquivo.ContentType != "image/jpeg" && arquivo.ContentType != "image/png" && arquivo.ContentType != "image/webp")
            ModelState.AddModelError(nameof(PratoFormulario.Imagem), "Selecione um arquivo de imagem valido.");
    }

    private async Task<string?> SalvarImagem(IFormFile? arquivo)
    {
        if (arquivo == null) return null;
        string nome = Guid.NewGuid().ToString("N") + Path.GetExtension(arquivo.FileName).ToLowerInvariant();
        string pasta = Path.Combine(ambiente.WebRootPath, "uploads");
        Directory.CreateDirectory(pasta);
        using (FileStream destino = System.IO.File.Create(Path.Combine(pasta, nome)))
        {
            await arquivo.CopyToAsync(destino);
        }
        return "/uploads/" + nome;
    }

    private void ApagarImagem(string? caminho)
    {
        if (string.IsNullOrEmpty(caminho) || !caminho.StartsWith("/uploads/")) return;
        string nome = Path.GetFileName(caminho);
        string completo = Path.Combine(ambiente.WebRootPath, "uploads", nome);
        if (System.IO.File.Exists(completo)) System.IO.File.Delete(completo);
    }
}
