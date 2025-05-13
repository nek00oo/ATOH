using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementApplication.Mappers;
using UserManagementCore.Common;
using UserManagementCore.Contracts;
using UserManagementCore.Models;

namespace UserManagementApplication.Services;

public class UserService : IUserService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly UserMapper _userMapper;

    public UserService(IUsersRepository usersRepository, IPasswordEncoder passwordEncoder, UserMapper userMapper)
    {
        _usersRepository = usersRepository;
        _passwordEncoder = passwordEncoder;
        _userMapper = userMapper;
    }
    
    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserDto createUserDto)
    {
        if (!await _usersRepository.IsLoginUniqueAsync(createUserDto.Login)) 
            return new Result<UserResponse>.FailureType("Login already exists");

        var hashedPassword = _passwordEncoder.HashPassword(createUserDto.Password);
    
        var now = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var currentUser = "system"; // TODO: взять актуального пользователя из контекста

        var userModelResult = UserModel.Create(
            userId,
            createUserDto.Login,
            hashedPassword,
            createUserDto.Name,
            createUserDto.Gender,
            createUserDto.Birthday,
            createUserDto.Admin,
            now,
            currentUser,
            now,
            currentUser,
            null,
            null);
        

        return await userModelResult.MatchAsync<UserModel, Result<UserResponse>>(
            async user => {
                await _usersRepository.CreateAsync(user);
                return Result<UserResponse>.Success(_userMapper.ToResponse(user));
            },
            error => Task.FromResult(Result<UserResponse>.Failure(error))
        );
    }
}