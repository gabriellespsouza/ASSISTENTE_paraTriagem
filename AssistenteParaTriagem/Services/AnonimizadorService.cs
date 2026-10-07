using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AssistenteParaTriagem.Services
{
    /// <summary>
    /// Gera um código anônimo e estável (ex.: P-7F3A2C) para cada participante.
    /// O mesmo usuário sempre recebe o mesmo código, mas o código não revela
    /// o e-mail nem o nome. Assim o sistema consegue impedir respostas
    /// repetidas sem guardar a identidade de quem respondeu.
    /// </summary>
    public class AnonimizadorService
    {
        // Chave usada no cálculo do código. Pode ser trocada em
        // appsettings.json ("Anonimizacao": { "Chave": "..." }), mas NÃO
        // mude depois de começar a coleta: os códigos mudariam.
        private const string ChavePadrao =
            "tc-triagem-unifenas-chave-padrao-2026";

        private readonly string _chave;

        public AnonimizadorService(IConfiguration configuration)
        {
            var configurada = configuration["Anonimizacao:Chave"];

            _chave = string.IsNullOrWhiteSpace(configurada)
                ? ChavePadrao
                : configurada;
        }

        /// <summary>Código anônimo do usuário logado.</summary>
        public string CodigoDoUsuario(ClaimsPrincipal usuario)
        {
            var id =
                usuario.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? usuario.Identity?.Name;

            return CodigoDe(id);
        }

        /// <summary>Código anônimo a partir de um texto (id ou nome).</summary>
        public string CodigoDe(string? valor)
        {
            valor = string.IsNullOrWhiteSpace(valor)
                ? "desconhecido"
                : valor.Trim().ToLowerInvariant();

            using var hmac =
                new HMACSHA256(Encoding.UTF8.GetBytes(_chave));

            var hash =
                hmac.ComputeHash(Encoding.UTF8.GetBytes(valor));

            return "P-" + Convert.ToHexString(hash, 0, 3);
        }
    }
}
