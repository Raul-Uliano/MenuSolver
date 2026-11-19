# Menu Solver

Trabalho de Desenvolvimento Web - ASP.NET Core MVC (.NET 9)

Integrantes: Christian Andrei Pellizzaro Calvi e Raul Zanon Uliano
Tema: Restaurante

## Sobre o sistema

O Menu Solver e um sistema para organizar um cardapio de restaurante. Nele da para cadastrar categorias e pratos, ver os detalhes, editar e excluir os cadastros.

Cada prato pertence a uma categoria, mas uma categoria pode ter varios pratos. Os dados ficam em listas na memoria do programa, entao quando ele e fechado e aberto de novo os cadastros voltam para os exemplos iniciais.

Tambem colocamos pesquisa, ordenacao, paginacao, um resumo na pagina inicial e a opcao de adicionar foto aos pratos.

## Como rodar

Foi feito com ASP.NET Core MVC e .NET 9. Para abrir precisa ter o Visual Studio Community com desenvolvimento ASP.NET e Web instalado e o .NET 9 SDK/ASP.NET Core Runtime.

1. Baixar e extrair a pasta do projeto.
2. Abrir o arquivo `MenuSolver.csproj` no Visual Studio Community.
3. Compilar o projeto com `Ctrl + Shift + B`.
4. Abrir o terminal do Visual Studio dentro da pasta do projeto.
5. Executar o comando:

```powershell
dotnet run --no-launch-profile --urls http://localhost:5050
```

6. Abrir `http://localhost:5050` no navegador.

Para parar o sistema e so apertar `Ctrl + C` no terminal. Nao precisa configurar banco de dados.

Se quiser conferir a versao do .NET instalada, use:

```powershell
dotnet --list-sdks
dotnet --list-runtimes
```

## Arquivos

- `Models` tem as classes Categoria e Prato.
- `Controllers` tem o codigo das operacoes de cadastro, edicao, exclusao e consulta.
- `Views` tem as paginas que aparecem no navegador.
- `Data/RepositorioMemoria.cs` guarda os dados nas listas.
- `ViewModels/Telas.cs` tem os dados usados nos formularios e listagens.
- `wwwroot/css/site.css` tem os estilos da pagina.
- `wwwroot/uploads` e a pasta usada para as imagens dos pratos.

## Testes

Para testar e so criar uma categoria e depois cadastrar um prato escolhendo essa categoria. Tambem da para editar, ver os detalhes e excluir. Se tentar deixar algum campo obrigatorio vazio ou colocar um preco invalido, aparece uma mensagem de validacao.

As listagens tem pesquisa, ordenacao e paginacao. Na pagina inicial aparecem os totais de categorias e pratos. Nao e possivel excluir uma categoria enquanto ela tiver pratos cadastrados.
