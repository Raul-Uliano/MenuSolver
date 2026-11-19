using MenuSolver.Models;

namespace MenuSolver.Data;

// Listas estaticas: os dados permanecem na memoria enquanto o programa estiver aberto.
// Projeto didatico para demonstracao local, sem acesso simultaneo de varios usuarios.
public static class RepositorioMemoria
{
    private static readonly List<Categoria> categorias = new List<Categoria>();
    private static readonly List<Prato> pratos = new List<Prato>();
    private static int proximaCategoria = 1;
    private static int proximoPrato = 1;

    static RepositorioMemoria()
    {
        AdicionarCategoria(new Categoria { Nome = "Entradas", Descricao = "Para comecar a refeicao" });
        AdicionarCategoria(new Categoria { Nome = "Pratos principais", Descricao = "Opcoes para a refeicao" });
        AdicionarCategoria(new Categoria { Nome = "Sobremesas", Descricao = "Para finalizar" });
        AdicionarPrato(new Prato { Nome = "Salada da casa", Descricao = "Folhas frescas com tomate e molho especial.", Preco = 19.90m, CategoriaId = 1 });
        AdicionarPrato(new Prato { Nome = "Bruschetta", Descricao = "Pao torrado, tomate, manjericao e azeite.", Preco = 22.00m, CategoriaId = 1 });
        AdicionarPrato(new Prato { Nome = "Massa ao sugo", Descricao = "Massa artesanal com molho de tomates.", Preco = 39.90m, CategoriaId = 2 });
        AdicionarPrato(new Prato { Nome = "Frango grelhado", Descricao = "Frango acompanhado de arroz e legumes.", Preco = 36.50m, CategoriaId = 2 });
        AdicionarPrato(new Prato { Nome = "Pudim", Descricao = "Pudim tradicional com calda de caramelo.", Preco = 15.00m, CategoriaId = 3 });
        AdicionarPrato(new Prato { Nome = "Brownie", Descricao = "Brownie de chocolate servido com sorvete.", Preco = 21.00m, CategoriaId = 3 });
    }

    public static List<Categoria> Categorias()
    {
        List<Categoria> resultado = new List<Categoria>();
        foreach (Categoria categoria in categorias)
        {
            Categoria copia = new Categoria { Id = categoria.Id, Nome = categoria.Nome, Descricao = categoria.Descricao };
            foreach (Prato prato in pratos)
            {
                if (prato.CategoriaId == categoria.Id) copia.Pratos.Add(prato);
            }
            resultado.Add(copia);
        }
        return resultado;
    }

    public static List<Prato> Pratos()
    {
        // A lista devolvida e independente: ordenar/filtrar nao altera os registros salvos.
        return new List<Prato>(pratos);
    }

    public static Categoria? CategoriaPorId(int id)
    {
        foreach (Categoria categoria in Categorias())
            if (categoria.Id == id) return categoria;
        return null;
    }

    public static Prato? PratoPorId(int id)
    {
        foreach (Prato prato in pratos)
            if (prato.Id == id) return prato;
        return null;
    }

    public static bool ExisteCategoria(int id) => CategoriaPorId(id) != null;

    public static bool NomeCategoriaEmUso(string nome, int excetoId = 0)
    {
        foreach (Categoria categoria in categorias)
        {
            if (categoria.Id != excetoId &&
                categoria.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    public static int AdicionarCategoria(Categoria categoria)
    {
        categoria.Id = proximaCategoria;
        proximaCategoria++;
        categorias.Add(categoria);
        return categoria.Id;
    }

    public static bool EditarCategoria(Categoria categoria)
    {
        foreach (Categoria atual in categorias)
        {
            if (atual.Id == categoria.Id)
            {
                atual.Nome = categoria.Nome;
                atual.Descricao = categoria.Descricao;
                return true;
            }
        }
        return false;
    }

    public static bool ExcluirCategoria(int id)
    {
        foreach (Prato prato in pratos)
            if (prato.CategoriaId == id) return false;

        foreach (Categoria categoria in categorias)
        {
            if (categoria.Id == id)
            {
                categorias.Remove(categoria);
                return true;
            }
        }
        return false;
    }

    public static int AdicionarPrato(Prato prato)
    {
        if (!ExisteCategoria(prato.CategoriaId)) throw new InvalidOperationException("Categoria inexistente.");
        prato.Id = proximoPrato;
        proximoPrato++;
        prato.CriadoEm = DateTime.Now;
        pratos.Add(prato);
        return prato.Id;
    }

    public static bool EditarPrato(Prato prato)
    {
        if (!ExisteCategoria(prato.CategoriaId)) return false;
        foreach (Prato atual in pratos)
        {
            if (atual.Id == prato.Id)
            {
                atual.Nome = prato.Nome;
                atual.Descricao = prato.Descricao;
                atual.Preco = prato.Preco;
                atual.CategoriaId = prato.CategoriaId;
                atual.ImagemPath = prato.ImagemPath;
                return true;
            }
        }
        return false;
    }

    public static Prato? ExcluirPrato(int id)
    {
        foreach (Prato prato in pratos)
        {
            if (prato.Id == id)
            {
                pratos.Remove(prato);
                return prato;
            }
        }
        return null;
    }
}
