using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Autenticacao.Validacao
{
    /// <summary>
    /// Resultado de uma operação de validação.
    /// Carrega todos os erros encontrados, não apenas o primeiro.
    /// </summary>
    public sealed class ResultadoValidacao
    {
        public bool Sucesso => Erros.Count == 0;
        public IReadOnlyList<string> Erros { get; }

        private ResultadoValidacao(List<string> erros) => Erros = erros.AsReadOnly();

        public static ResultadoValidacao Ok() => new(new List<string>());
        public static ResultadoValidacao Falha(List<string> erros) => new(erros);

        /// <summary>Lança InvalidOperationException se a validação falhou.</summary>
        public void ThrowIfInvalid(string paramName = "valor")
        {
            if (!Sucesso)
                throw new InvalidOperationException(
                    $"Validação de '{paramName}' falhou: {string.Join("; ", Erros)}"
                );
        }
    }

    public static class ValidadorAcesso
    {
        // MailAddress do próprio .NET faz o parsing correto de RFC 5321,
        // cobrindo edge cases que regex simples não trata (ex: quoted strings,
        // domínios internacionalizados). O try/catch é intencional aqui.
        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                // MailAddress aceita "Nome <email@dominio.com>" — garantir que
                // o input seja somente o endereço, sem display name.
                return addr.Address == email.Trim();
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Valida a senha contra todas as regras e retorna TODOS os erros,
        /// não apenas o primeiro. Isso permite que o frontend mostre um
        /// checklist completo em vez de forçar o usuário a submeter várias vezes.
        /// </summary>
        public static ResultadoValidacao ValidarSenha(string senha)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(senha))
            {
                // Falha crítica — não faz sentido continuar checando.
                return ResultadoValidacao.Falha(new List<string> { "A senha não pode estar vazia." });
            }

            if (senha.Length < 8)
                erros.Add("A senha deve ter pelo menos 8 caracteres.");

            if (!ContémTipo(senha, char.IsUpper))
                erros.Add("A senha deve conter ao menos uma letra maiúscula.");

            if (!ContémTipo(senha, char.IsLower))
                erros.Add("A senha deve conter ao menos uma letra minúscula.");

            if (!ContémTipo(senha, char.IsDigit))
                erros.Add("A senha deve conter ao menos um número.");

            if (!ContémTipo(senha, ch => !char.IsLetterOrDigit(ch)))
                erros.Add("A senha deve conter ao menos um caractere especial (ex: @, #, $).");

            return erros.Count == 0
                ? ResultadoValidacao.Ok()
                : ResultadoValidacao.Falha(erros);
        }

        // Helper local — evita repetir a lógica de iteração em cada regra.
        private static bool ContémTipo(string valor, Func<char, bool> predicate)
        {
            foreach (var ch in valor)
                if (predicate(ch)) return true;
            return false;
        }
    }
}
