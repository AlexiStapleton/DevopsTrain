namespace API.Exceptions;

public class EmailAlreadyUsedException : Exception
{
    public EmailAlreadyUsedException() : base("Cet email est deja utilise.") { }
}