namespace RoboConsulta.Services;

public class HemoscUnavailableException(string message, Exception inner) : Exception(message, inner);
