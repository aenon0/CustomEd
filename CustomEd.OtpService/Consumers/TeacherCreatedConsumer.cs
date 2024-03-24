using CustomEd.OtpService.Repository;
using CustomEd.OtpService.Service;
using CustomEd.User.Events;
using CustomEd.User.Student.Events;
using CustomEd.User.Teacher.Events;
using MassTransit;
using MongoDB.Bson;

namespace CustomEd.OtpService;

public class TeacherCreatedConsumer: IConsumer<TeacherCreatedEvent>
{
    private readonly IOtpRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEmailService _emailService;
    public TeacherCreatedConsumer(IOtpRepository repository, IPublishEndpoint publishEndpoint, IEmailService emailService)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<TeacherCreatedEvent> context)
    {
        try
        {
            var message = context.Message;
            var item = await _repository.ExistsByEmailAddress(message.Email);
            string otpCode = new OtpGenerationService().GenerateOTP();

            if (item == true)
            {
                var otpItem = await _repository.GetByEmailAddress(message.Email);
                otpItem.OtpCode = otpCode;
                await _repository.Update(otpItem);
                return;
            }

            await _emailService.SendEmail(message.Email, otpCode);

            await _repository.Add(new Otp
            {
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
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}