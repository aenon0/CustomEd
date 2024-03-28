using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Classroom.Service.Model;
using AutoMapper;
using CustomEd.User.Student.Events;

namespace CustomEd.Classroom.Service.Consumers
{
    ///I think you forgot to replace student with teacher
    public class StudentUpdatedEventConsumer : IConsumer<StudentUpdatedEvent>
    {
        private readonly IGenericRepository<Student> _StudentRepository;
        private readonly IMapper _mapper;

        public StudentUpdatedEventConsumer(IGenericRepository<Student> StudentRepository, IMapper mapper)
        {
    
            _StudentRepository = StudentRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<StudentUpdatedEvent> context)
        {
            var StudentUpdatedEvent = context.Message;
            var Student = _mapper.Map<Student>(StudentUpdatedEvent);
            Student.Id = StudentUpdatedEvent.Id; 
            await _StudentRepository.UpdateAsync(Student);
            return;
        }
    }
}