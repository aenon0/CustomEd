using System.Threading.Tasks;
using MassTransit;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Classroom.Service.Model;
using AutoMapper;
using CustomEd.User.Teacher.Events;

namespace CustomEd.Classroom.Service.Consumers
{
    ///I think you forgot to replace teacher with student
    public class TeacherUpdatedEventConsumer : IConsumer<TeacherUpdatedEvent>
    {
        private readonly IGenericRepository<Teacher> _teacherRepository;
        private readonly IMapper _mapper;

        public TeacherUpdatedEventConsumer(IGenericRepository<Teacher> teacherRepository, IMapper mapper)
        {
    
            _teacherRepository = teacherRepository;
            _mapper = mapper;   
        }
        public async Task Consume(ConsumeContext<TeacherUpdatedEvent> context)
        {
            var teacherUpdatedEvent = context.Message;
            var teacher = _mapper.Map<Teacher>(teacherUpdatedEvent);
            teacher.Id = teacherUpdatedEvent.Id; 
            await _teacherRepository.UpdateAsync(teacher);
            return;
        }
    }
}