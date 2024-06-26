using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CustomEd.User.Service.PasswordService.Interfaces;
using CusotmEd.User.Servce.DTOs;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using MassTransit;
using CustomEd.User.Service.Model;
using CustomEd.User.Student.Events;
using CustomEd.User.Teacher.Events;
using CustomEd.User.Service.Services;

namespace CustomEd.User.Service.Controllers
{
    [ApiController]
    public abstract class UserController<T> : ControllerBase where T : Model.User 
    {
        protected IGenericRepository<ForgotPasswordOtp> _forgotPasswordOtpRepository;
        protected readonly IGenericRepository<T> _userRepository; 
        protected readonly IGenericRepository<Otp> _otpRepository;
        protected readonly IMapper _mapper;
        protected readonly IPasswordHasher _passwordHasher;
        protected readonly IJwtService _jwtService;
        protected readonly IPublishEndpoint _publishEndpoint;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly CloudinaryService _cloudinaryService;
        protected readonly EmailService _emailServices;

        public UserController(EmailService emailServices, CloudinaryService cloudinaryService, IGenericRepository<ForgotPasswordOtp> forgotPasswordOtpRepository, IGenericRepository<Otp> otpRepository, IGenericRepository<T> userRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor)
        {
            _emailServices = emailServices;
            _cloudinaryService = cloudinaryService;
            _forgotPasswordOtpRepository = forgotPasswordOtpRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
            _otpRepository = otpRepository;
        }
    
        [HttpGet]
        public  async Task<ActionResult<SharedResponse<IEnumerable<T>>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(SharedResponse<IEnumerable<T>>.Success(users, "Users retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedResponse<T>>> GetUserById(Guid id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return NotFound(SharedResponse<T>.Fail("User not found", null));
            }
            return Ok(SharedResponse<T>.Success(user, "User retrieved successfully"));
        }

        [HttpPost("user/login")]
        public virtual async Task<ActionResult<SharedResponse<CustomEd.User.Service.DTOs.Common.LoginResponseDto>>> SignIn([FromBody] LoginRequestDto request)
        {

            var user = await _userRepository.GetAsync(x => (x.Email == request.Email && x.IsVerified == true) );
            if(user == null)
            {
                return BadRequest(SharedResponse<UserDto>.Fail("User not found", null));
            }
            if(!_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                return BadRequest(SharedResponse<bool>.Fail("Incorrect Password", null));
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = (IdentityRole) user.Role
            };

            var token = _jwtService.GenerateToken(userDto);
            userDto.Token = token;
            var loginResponse = new CustomEd.User.Service.DTOs.Common.LoginResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = userDto.Role,
                Token = userDto.Token,
                ImageUrl = user.ImageUrl
            };
            return Ok(SharedResponse<CustomEd.User.Service.DTOs.Common.LoginResponseDto>.Success(loginResponse, null));
        }

        [HttpPost("verify")]
        public virtual async Task<ActionResult<SharedResponse<UserDto>>> VerifyUser([FromBody] VerifyUserDto request)
        {
            var user = await _userRepository.GetAsync(x => x.Email == request.Email);
            if(user == null)
            {
                return BadRequest(SharedResponse<bool>.Fail("User not found", null));
            }
            if(user.IsVerified)
            {
                return Ok(SharedResponse<bool>.Success(true, "User has already been verified."));
            }
            var otp = await _otpRepository.GetAsync(x => x.Email == request.Email);
            if(otp == null)
            {
                return BadRequest(SharedResponse<bool>.Fail("Email not registered", null));
            }
            if(otp.OtpCode != request.OtpCode && otp.UpdatedAt.AddMinutes(30) > DateTime.UtcNow)
            {
                return BadRequest(SharedResponse<bool>.Fail("Invalid Otp", null));
            }
            user.IsVerified = true;
            if(user.Role == Role.Student)
            {
                var studentEvent = _mapper.Map<StudentCreatedEvent>(user);
                // await _publishEndpoint.Publish(studentEvent);
                var studentCreatedEvent = _mapper.Map<StudentCreatedEvent>(user);
                await _publishEndpoint.Publish(studentCreatedEvent);
            }
            else if(user.Role == Role.Teacher)
            {
                var teacherEvent = _mapper.Map<TeacherCreatedEvent>(user);
                // await _publishEndpoint.Publish(teacherEvent);
                var teacherCreatedEvent = _mapper.Map<TeacherCreatedEvent>(user);
                await _publishEndpoint.Publish(teacherCreatedEvent);
            }
            
            await _userRepository.UpdateAsync(user);
            return Ok(SharedResponse<bool>.Success(true, "User verified successfully"));
        }
        
    }
}
