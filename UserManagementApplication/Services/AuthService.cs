using Microsoft.AspNetCore.Http;
using UserManagementApplication.Abstractions;
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
    
    public async Task<string> Login(string login, string password)
    {
        var user = await _usersRepository.GetByLoginAsync(login);
        var result = _passwordEncoder.VerifyPassword(password, user.Password);

        if (result is false)
        {
            //TODO: сделеть отдельный возвращаемый тип ResultAuth с Sucess(token) и Fail(error)
        }
        
        return _jwtProvider.GenerateToken(user);
    }
}