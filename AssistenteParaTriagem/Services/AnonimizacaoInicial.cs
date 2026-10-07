using AssistenteParaTriagem.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AssistenteParaTriagem.Services
{
    /// <summary>
    /// Executa uma vez ao iniciar o sistema: troca e-mails/nomes que já
    /// estejam gravados no banco por códigos anônimos. Pode rodar quantas
    /// vezes for preciso (registros já anônimos são ignorados).
    /// </summary>
    public static class AnonimizacaoInicial
    {
        private static readonly Regex Padrao =
            new("^P-[0-9A-F]{6}$", RegexOptions.Compiled);

        public static async Task ExecutarAsync(
            ApplicationDbContext context,
            AnonimizadorService anonimizador,
            ILogger logger)
        {
            try
            {
                var usuarios = await context.Users
                    .AsNoTracking()
                    .Select(u => new { u.Id, u.UserName, u.Email })
                    .ToListAsync();

                var porId =
                    usuarios.ToDictionary(
                        u => u.Id,
                        u => anonimizador.CodigoDe(u.Id));

                var porNome =
                    new Dictionary<string, string>(
                        StringComparer.OrdinalIgnoreCase);

                foreach (var u in usuarios)
                {
                    var codigo = porId[u.Id];

                    if (!string.IsNullOrWhiteSpace(u.UserName))
                        porNome[u.UserName.Trim()] = codigo;

                    if (!string.IsNullOrWhiteSpace(u.Email))
                        porNome[u.Email.Trim()] = codigo;
                }

                string CodigoPorNome(string? nome)
                {
                    if (string.IsNullOrWhiteSpace(nome))
                        return anonimizador.CodigoDe(null);

                    nome = nome.Trim();

                    if (Padrao.IsMatch(nome))
                        return nome;

                    return porNome.TryGetValue(nome, out var c)
                        ? c
                        : anonimizador.CodigoDe(nome);
                }

                int alterados = 0;

                // Registros de auditoria (guardam UserId e nome)
                foreach (var a in await context.AuditLogs.ToListAsync())
                {
                    string codigo;

                    if (!string.IsNullOrWhiteSpace(a.UserId)
                        && porId.TryGetValue(a.UserId, out var doId))
                    {
                        codigo = doId;
                    }
                    else if (!string.IsNullOrWhiteSpace(a.UserId)
                        && Padrao.IsMatch(a.UserId))
                    {
                        codigo = a.UserId;
                    }
                    else
                    {
                        codigo = CodigoPorNome(a.NomeProfissional);
                    }

                    if (a.UserId != codigo || a.NomeProfissional != codigo)
                    {
                        a.UserId = codigo;
                        a.NomeProfissional = codigo;
                        alterados++;
                    }
                }

                foreach (var a in await context.AvaliacoesCenarios.ToListAsync())
                {
                    var codigo = CodigoPorNome(a.NomeProfissional);

                    if (a.NomeProfissional != codigo)
                    {
                        a.NomeProfissional = codigo;
                        alterados++;
                    }
                }

             

               

                if (alterados > 0)
                {
                    await context.SaveChangesAsync();

                    logger.LogInformation(
                        "Anonimização: {Qtde} registro(s) convertido(s) para código.",
                        alterados);
                }
            }
            catch (Exception ex)
            {
                // Não impede o sistema de abrir.
                logger.LogWarning(
                    ex,
                    "Não foi possível anonimizar os registros antigos.");
            }
        }
    }
}
