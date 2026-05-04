namespace SistemaTurnos.Responses;

public class ServiceResponse<T>
{
    public bool Status { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }


    // Para mensaje de error
    public static ServiceResponse<T> Error(string msg)
    {
        return new ServiceResponse<T>()
        {
            Status = false,
            Message = msg
        };
    }

    // Para el mensaje de Exito + data
    public static ServiceResponse<T> Success(T data, string? msg)
    {
        return new ServiceResponse<T>()
        {
            Status = true,
            Data = data,
            Message = msg
        };
    }

    // Para mensaje de exito
    public static ServiceResponse<T> Success(string msg)
    {
        return new ServiceResponse<T>
        {
            Status = true,
            Message = msg
        };
    }

    // Para data
    public static ServiceResponse<T> Success(T data)
    {
        return new ServiceResponse<T>
        {
            Status = true,
            Data = data
        };
    }
}