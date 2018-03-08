namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public enum LoginStatus
    {
        Success = 10,
        IncorrectEmailAndPass = 15,
        NotConfirmed = 20,
        NotAdmin = 25,
        NotActived = 30
    }
}
