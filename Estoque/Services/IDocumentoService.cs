using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public interface IDocumentoService
{
    AjusteEstoque AddAjuste(AjusteEstoque doc);
    AjusteEstoque UpdateAjuste(AjusteEstoque doc);
    RequisicaoEstoque AddRequisicao(RequisicaoEstoque doc);
    RequisicaoEstoque UpdateRequisicao(RequisicaoEstoque doc);
    DevolucaoEstoque AddDevolucao(DevolucaoEstoque doc);
    DevolucaoEstoque UpdateDevolucao(DevolucaoEstoque doc);
    TransferenciaEstoque AddTransferencia(TransferenciaEstoque doc);
    TransferenciaEstoque UpdateTransferencia(TransferenciaEstoque doc);
    IEnumerable<DocumentoBase> GetAll();
    IEnumerable<AjusteEstoque> GetAjustes();
    IEnumerable<RequisicaoEstoque> GetRequisicoes();
    IEnumerable<DevolucaoEstoque> GetDevolucoes();
    IEnumerable<TransferenciaEstoque> GetTransferencias();
    void DeleteDocumento(int id);
    void EstornarDocumento(int id);
    void EfetivarDocumento(int id);
}