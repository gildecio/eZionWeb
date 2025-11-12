using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public interface IGrupoRepository
{
    IEnumerable<Grupo> GetAll();
    Grupo? GetById(int id);
    IEnumerable<Grupo> GetChildren(int? parentId);
    Grupo Add(Grupo grupo);
    void Update(Grupo grupo);
    void Delete(int id);
    bool IsLeaf(int id);
    bool IsDescendantOf(int id, int ancestorId);
}