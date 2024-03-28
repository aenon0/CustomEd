using CustomEd.OtpService.Repository;
using CustomEd.OtpService.Service;
using CustomEd.User.Events;
using CustomEd.User.Student.Events;
using MassTransit;
using MongoDB.Bson;

namespace CustomEd.OtpService;

public class UserCreatedConsumer : IConsumer<StudentCreatedEvent>
{
    private readonly IOtpRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEmailService _emailService;
    public UserCreatedConsumer(IOtpRepository repository, IPublishEndpoint publishEndpoint, IEmailService emailService)
    {
        _emailService = emailService;
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        Console.WriteLine("CONSUMERRRRRR");
    }

    public async Task Consume(ConsumeContext<StudentCreatedEvent> context)
    {
        
        var message = context.Message;
        var item = await _repository.ExistsByEmailAddress(message.Email);
        string otpCode = new OtpGenerationService().GenerateOTP();
        Console.WriteLine($"bool: {item}");
        if (item == true){
            var otpItem = await _repository.GetByEmailAddress(message.Email);
            otpItem.OtpCode = otpCode;
            await _repository.Update(otpItem);
            await _emailService.SendEmail(message.Email, otpCode);
            Console.WriteLine("sent the message");
            return;
        }
        Console.WriteLine("sent the message");
        await _emailService.SendEmail(message.Email, otpCode);
        await _repository.Add(new Otp{
            Id = ObjectId.GenerateNewId(),
           EmailAddress = message.Email,
           OtpCode = otpCode
        });
        await _publishEndpoint.Publish(new OtpGeneratedEvent
        {
            Email = message.Email,
            OtpCode = otpCode
        });
        
    }
}
