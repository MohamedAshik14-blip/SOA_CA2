using AutoMapper;
using CollegeEventsApi.Dtos;
using CollegeEventsApi.Models;

namespace CollegeEventsApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentReadDto>();

            CreateMap<Event, EventReadDto>();
            CreateMap<EventCreateDto, Event>();
            CreateMap<EventUpdateDto, Event>();

            CreateMap<Registration, RegistrationReadDto>()
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title));
        }
    }
}
