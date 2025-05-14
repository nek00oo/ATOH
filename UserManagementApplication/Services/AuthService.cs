using UserManagementApplication.Abstractions;
using UserManagementCore.Common;
using UserManagementCore.Contracts;

namespace UserManagementApplication.Services;

public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(
        IUsersRepository usersRepository,
        IPasswordEncoder passwordEncoder,
        IJwtProvider jwtProvider)
    {
        _usersRepository = usersRepository;
        _passwordEncoder = passwordEncoder;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string>> Login(string login, string password)
    {
        try
        {
            var user = await _usersRepository.FindByLoginAsync(login);
            if (user == null)
                return Result<string>.NotFound("User not found");

            if (_passwordEncoder.VerifyPassword(password, user.Password) is false)
                return Result<string>.Failure("Invalid credentials");

            if (user.RevokedOn != null)
                return Result<string>.Failure("User is revoked");

            return Result<string>.Success(_jwtProvider.GenerateToken(user));
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(ex.Message);
        }
    }
}