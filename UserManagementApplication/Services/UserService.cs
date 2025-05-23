using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementApplication.Mappers;
using UserManagementCore.Common;
using UserManagementCore.Contracts;
using UserManagementCore.Exceptions;
using UserManagementCore.Models;

namespace UserManagementApplication.Services;

public class UserService : IUserService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly IUserResponseMapper _userResponseMapper;

    public UserService(
        IUsersRepository usersRepository,
        IPasswordEncoder passwordEncoder,
        IUserResponseMapper userResponseMapper)
    {
        _usersRepository = usersRepository;
        _passwordEncoder = passwordEncoder;
        _userResponseMapper = userResponseMapper;
    }

    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserDto createUserDto, string createdBy)
    {
        try
        {
            var user = await _usersRepository.FindByLoginAsync(createUserDto.Login);

            if (user is not null)
                return Result<UserResponse>.Failure($"User with login {createUserDto.Login} already exists");

            var hashedPassword = _passwordEncoder.HashPassword(createUserDto.Password);
            var now = DateTime.UtcNow;

            var userModelResult = UserModel.Create(
                id: Guid.NewGuid(),
                login: createUserDto.Login,
                password: hashedPassword,
                name: createUserDto.Name,
                gender: createUserDto.Gender,
                birthday: createUserDto.Birthday,
                admin: createUserDto.Admin,
                createdOn: now,
                createdBy: createdBy,
                modifiedOn: now,
                modifiedBy: createdBy,
                revokedOn: null,
                revokedBy: null);

            var userModel = userModelResult.TryGetValue(out var error);
            if (userModel is null)
                return new Result<UserResponse>.FailureType(error);

            await _usersRepository.CreateAsync(userModel);
            return Result<UserResponse>.SuccessCreate(_userResponseMapper.ToResponse(userModel));
        }
        catch (UniqueConstraintException ex)
        {
            return Result<UserResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Failed to create user: {ex.Message}");
        }
    }

    public async Task<Result<UserResponse>> GetUserByLoginAsync(string login)
    {
        try
        {
            var user = await _usersRepository.FindByLoginAsync(login);

            if (user is null)
                return Result<UserResponse>.NotFound("User not found");

            return Result<UserResponse>.Success(_userResponseMapper.ToResponse(user));
        }
        catch (InvalidOperationException ex)
        {
            return Result<UserResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Error retrieving user: {ex.Message}");
        }
    }

    public async IAsyncEnumerable<UserResponse> GetActiveUsersAsync()
    {
        await foreach (var user in _usersRepository.GetActiveUsersAsync())
        {
            yield return _userResponseMapper.ToResponse(user);
        }
        
    }

    public async IAsyncEnumerable<UserResponse> GetUsersOlderThanStreamAsync(int age)
    {
        await foreach (var user in _usersRepository.GetUsersOlderThanAsync(age))
        {
            yield return _userResponseMapper.ToResponse(user);
        }
    }

    public async Task<Result<UserResponse>> UpdateProfileAsync(string login, UpdateProfileDto dto, string modifiedBy)
    {
        try
        {
            var targetUser = await _usersRepository.FindByLoginAsync(login);
            if (targetUser is null)
                return Result<UserResponse>.NotFound("User not found");

            if (targetUser.RevokedOn is not null)
                return Result<UserResponse>.Failure("User is revoked");

            var updatedUser = await _usersRepository.UpdateProfileAsync(
                login,
                dto.Name,
                dto.Gender,
                dto.Birthday,
                modifiedBy);

            return Result<UserResponse>.Success(_userResponseMapper.ToResponse(updatedUser));
        }
        catch (UserNotFoundException ex)
        {
            return Result<UserResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Update failed: {ex.Message}");
        }
    }

    public async Task<Result<UserResponse>> ChangePasswordAsync(string login, ChangePasswordDto dto, string modifiedBy)
    {
        try
        {
            var targetUser = await _usersRepository.FindByLoginAsync(login);
            if (targetUser is null)
                return Result<UserResponse>.NotFound("User not found");

            if (targetUser.RevokedOn is not null)
                return Result<UserResponse>.Failure("User is revoked");
            
            var isAdmin = (await _usersRepository.FindByLoginAsync(modifiedBy))?.Admin ?? false;
            
            if (isAdmin is false &&
                dto.OldPassword is not null &&
                _passwordEncoder.VerifyPassword(dto.OldPassword, targetUser.Password) is false)
            {
                return Result<UserResponse>.Failure("Invalid old password");
            }

            var newPasswordHash = _passwordEncoder.HashPassword(dto.NewPassword);
            var updatedUser = await _usersRepository.ChangePasswordAsync(
                login,
                newPasswordHash,
                modifiedBy);

            return Result<UserResponse>.Success(_userResponseMapper.ToResponse(updatedUser));
        }
        catch (UserNotFoundException ex)
        {
            return Result<UserResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Password change failed: {ex.Message}");
        }
    }

    public async Task<Result<UserResponse>> ChangeLoginAsync(string currentLogin, ChangeLoginDto dto, string modifiedBy)
    {
        try
        {
            var targetUser = await _usersRepository.FindByLoginAsync(currentLogin);
            if (targetUser is null)
                return Result<UserResponse>.NotFound("User not found");

            if (targetUser.RevokedOn is not null)
                return Result<UserResponse>.Failure("User is revoked");

            if (await _usersRepository.FindByLoginAsync(dto.NewLogin) is not null)
                return Result<UserResponse>.Failure("Login already exists");

            var updatedUser = await _usersRepository.ChangeLoginAsync(
                currentLogin,
                dto.NewLogin,
                modifiedBy);

            return Result<UserResponse>.Success(_userResponseMapper.ToResponse(updatedUser));
        }
        catch (UniqueConstraintException ex)
        {
            return Result<UserResponse>.Failure(ex.Message);
        }
        catch (UserNotFoundException ex)
        {
            return Result<UserResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Login change failed: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteUserAsync(string login, bool softDelete, string revokedBy)
    {
        try
        {
            var targetUser = await _usersRepository.FindByLoginAsync(login);
            if (targetUser is null)
                return Result<bool>.NotFound("User not found");

            bool result;
            if (softDelete)
                result = await _usersRepository.SoftDeleteAsync(login, revokedBy);
            else
                result = await _usersRepository.HardDeleteAsync(login);

            return Result<bool>.Success(result);
        }
        catch (UserNotFoundException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Delete failed: {ex.Message}");
        }
    }

    public async Task<Result<UserResponse>> RestoreUserAsync(string login)
    {
        try
        {
            var targetUser = await _usersRepository.FindByLoginAsync(login);
            if (targetUser is null)
                return Result<UserResponse>.NotFound("User not found");

            var user = await _usersRepository.RestoreAsync(login);
            return Result<UserResponse>.Success(_userResponseMapper.ToResponse(user));
        }
        catch (UserNotFoundException ex)
        {
            return Result<UserResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Restore failed: {ex.Message}");
        }
    }
}