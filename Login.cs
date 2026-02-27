ta senior ou nao isso?

using System;
using System.Text.RegularExpressions;
using System.Linq;
public class ValidadorAcesso
{
    // Regex para validar o formato básico de um e-mail
    private static readonly string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public static bool ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        
        return Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase);
    }
    public static (bool Sucesso, string Mensagem) ValidarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha)) 
            return (false, "A senha não pode estar vazia.");
        if (senha.Length < 8) 
            return (false, "A senha deve ter pelo menos 8 caracteres.");
        if (!senha.Any(char.IsUpper)) 
            return (false, "A senha deve conter ao menos uma letra maiúscula.");
        if (!senha.Any(char.IsLower)) 
            return (false, "A senha deve conter ao menos uma letra minúscula.");
        if (!senha.Any(char.IsDigit)) 
            return (false, "A senha deve conter ao menos um número.");
        if (!senha.Any(ch => !char.IsLetterOrDigit(ch))) 
            return (false, "A senha deve conter ao menos um caractere especial (ex: @, #, $).");
        return (true, "Senha válida!");
    }
}
// Exemplo de Uso
public class Program
{
    public static void Main()
    {
        string meuEmail = "contato@exemplo.com";
        string minhaSenha = "Senha@Forte2026";
        if (ValidadorAcesso.ValidarEmail(meuEmail))
        {
            Console.WriteLine("E-mail válido.");
        }
        var resultadoSenha = ValidadorAcesso.ValidarSenha(minhaSenha);
        Console.WriteLine(resultadoSenha.Mensagem);
    }
}
