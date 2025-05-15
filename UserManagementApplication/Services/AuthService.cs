using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementApplication.Mappers;
using UserManagementCore.Common;
using UserManagementCore.Contracts;

namespace UserManagementApplication.Services;

public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly IJwtProvider _jwtProvider;
    private readonly UserResponseMapper _userResponseMapper;

    public AuthService(
        IUsersRepository usersRepository,
        IPasswordEncoder passwordEncoder,
        IJwtProvider jwtProvider,
        UserResponseMapper userResponseMapper)
    {
        _usersRepository = usersRepository;
        _passwordEncoder = passwordEncoder;
        _jwtProvider = jwtProvider;
        _userResponseMapper = userResponseMapper;
    }

    public async Task<Result<LoginUserResponse>> Login(string login, string password)
    {
        try
        {
            var user = await _usersRepository.FindByLoginAsync(login);
            if (user == null)
                return Result<LoginUserResponse>.NotFound("User not found");

            if (_passwordEncoder.VerifyPassword(password, user.Password) is false)
                return Result<LoginUserResponse>.Failure("Invalid credentials");

            if (user.RevokedOn != null)
                return Result<LoginUserResponse>.Failure("User is revoked");

            var userLoginResponse = new LoginUserResponse(_userResponseMapper.ToResponse(user),
                _jwtProvider.GenerateToken(user));
            
            return Result<LoginUserResponse>.Success(userLoginResponse);
        }
        catch (Exception ex)
        {
            return Result<LoginUserResponse>.Failure(ex.Message);
        }
    }
}