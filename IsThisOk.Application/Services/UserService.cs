using IsThisOk.Application.Interfaces;
using IsThisOk.Domain.Entities;
using MongoDB.Driver;
using BCrypt.Net;

namespace IsThisOk.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserMst> _usersCollection;
        private readonly IMongoCollection<RoleMst> _rolesCollection;
        private readonly ITokenService _tokenService;

        public UserService(IMongoDatabase database, ITokenService tokenService)
        {
            _database = database;
            _usersCollection = _database.GetCollection<UserMst>("Users");
            _rolesCollection = _database.GetCollection<RoleMst>("Roles");
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> SignUpAsync(SignUpRequest request)
        {
            try
            {
                // Step 1: Check if email already exists
                var existingUser = await _usersCollection
                    .Find(u => u.sEmail == request.Email)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email already registered!"
                    };
                }

                // Step 2: Generate anonymous display name
                var anonymousName = GenerateAnonymousDisplayName();

                // Step 3: Get default "User" role
                var userRole = await _rolesCollection
                    .Find(r => r.sRoleName == "User" && r.bIsActive)
                    .FirstOrDefaultAsync();

                if (userRole == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "System error: User role not found"
                    };
                }

                // Step 4: Hash the password (NEVER store plain passwords!)
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Step 5: Create new user
                var newUser = new UserMst
                {
                    sUsername = anonymousName,  // We use anonymous name as username too
                    sEmail = request.Email,
                    sPasswordHash = hashedPassword,
                    iRoleId = userRole.Id,
                    sAnonymousDisplayName = anonymousName,
                    sGender = request.Gender,
                    bIsActive = true
                };

                // Step 6: Save to database
                await _usersCollection.InsertOneAsync(newUser);

                // Step 7: Generate JWT token
                var token = _tokenService.GenerateToken(newUser.Id, newUser.sEmail, userRole.sRoleName);

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    AnonymousDisplayName = anonymousName,
                    Message = "Account created successfully!"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Registration failed. Please try again."
                };
            }
        }

        public async Task<AuthResponse> SignInAsync(SignInRequest request)
        {
            try
            {
                // Step 1: Find user by email
                var user = await _usersCollection
                    .Find(u => u.sEmail == request.Email && u.bIsActive)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Step 2: Check password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.sPasswordHash);

                if (!isPasswordValid)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Step 3: Get user's role
                var userRole = await _rolesCollection
                    .Find(r => r.Id == user.iRoleId)
                    .FirstOrDefaultAsync();

                // Step 4: Generate new JWT token
                var token = _tokenService.GenerateToken(user.Id, user.sEmail, userRole?.sRoleName ?? "User");

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    AnonymousDisplayName = user.sAnonymousDisplayName,
                    Message = "Login successful!"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Login failed. Please try again."
                };
            }
        }

        public async Task<UserMst?> GetUserByIdAsync(string userId)
        {
            return await _usersCollection
                .Find(u => u.Id == userId && u.bIsActive)
                .FirstOrDefaultAsync();
        }

        // Helper method to create fun anonymous names
        private string GenerateAnonymousDisplayName()
        {
            var adjectives = new[] { "Cool", "Swift", "Brave", "Smart", "Wild", "Calm", "Bold", "Wise" };
            var animals = new[] { "Fox", "Wolf", "Eagle", "Tiger", "Lion", "Bear", "Hawk", "Owl" };

            var random = new Random();
            var adjective = adjectives[random.Next(adjectives.Length)];
            var animal = animals[random.Next(animals.Length)];
            var number = random.Next(100, 999);

            return $"Anonymous_{adjective}_{animal}_{number}";
        }
    }
}