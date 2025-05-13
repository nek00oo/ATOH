using System.Text.RegularExpressions;
using UserManagementCore.Common;
using UserManagementCore.Types;

namespace UserManagementCore.Models;

public class UserModel
{
    private UserModel(
        Guid id,
        string login,
        string password,
        string name,
        Gender gender,
        DateTime? birthday,
        bool admin,
        DateTime createdOn,
        string createdBy,
        DateTime modifiedOn,
        string modifiedBy,
        DateTime? revokedOn,
        string? revokedBy)
    {
        Id = id;
        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        Birthday = birthday;
        Admin = admin;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        ModifiedOn = modifiedOn;
        ModifiedBy = modifiedBy;
        RevokedOn = revokedOn;
        RevokedBy = revokedBy;
    }

    public Guid Id { get; }

    public string Login { get; }

    public string Password { get; }

    public string Name { get; }

    public Gender Gender { get; } = Gender.Unknown;

    public DateTime? Birthday { get; }

    public bool Admin { get; }

    public DateTime CreatedOn { get; } = DateTime.UtcNow;

    public string CreatedBy { get; }

    public DateTime ModifiedOn { get; }

    public string ModifiedBy { get; }

    public DateTime? RevokedOn { get; }

    public string? RevokedBy { get; }

    public static Result<UserModel> Create(
        Guid id,
        string login,
        string password,
        string name,
        Gender gender,
        DateTime? birthday,
        bool admin,
        DateTime createdOn,
        string createdBy,
        DateTime modifiedOn,
        string modifiedBy,
        DateTime? revokedOn,
        string? revokedBy)
    {
        if (string.IsNullOrWhiteSpace(login) || !Regex.IsMatch(login, "^[a-zA-Z0-9]+$"))
            return new Result<UserModel>.FailureType("Login must contain only Latin letters and digits.");

        if (string.IsNullOrWhiteSpace(password))
            return new Result<UserModel>.FailureType("There must be a password.");

        if (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(name, "^[a-zA-Zа-яА-ЯёЁ]+$"))
            return new Result<UserModel>.FailureType("Name must contain only Latin and Cyrillic letters.");

        var user = new UserModel(
            id,
            login,
            password,
            name,
            gender,
            birthday,
            admin,
            createdOn,
            createdBy,
            modifiedOn,
            modifiedBy,
            revokedOn,
            revokedBy);
        
        return new Result<UserModel>.SuccessType(user);
    }
}