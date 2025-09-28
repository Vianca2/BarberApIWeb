using AutoMapper;
using BarberappAPI.DTOs;
using BarberappAPI.Models;

namespace BarberappAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Usuario mappings
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<RegistroUsuarioDto, Usuario>();

            // Cita mappings
            CreateMap<Cita, CitaDto>()
                .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src => src.Cliente.Nombre))
                .ForMember(dest => dest.EmailCliente, opt => opt.MapFrom(src => src.Cliente.Email))
                .ForMember(dest => dest.TelefonoCliente, opt => opt.MapFrom(src => src.Cliente.Telefono));

            CreateMap<CrearCitaDto, Cita>();
        }
    }
}