using Application.Services;
using AutoMapper;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService : BaseService<User>
{
    private readonly IMapper _mapper;

    public UserService(
        IGenericRepository<User> repo,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<BaseService<User>> logger)
        : base(repo, uow, logger)
    {
        _mapper = mapper;
    }

    // READ ALL (DTO)
    public async Task<List<UserReadDto>> GetAllReadAsync()
    {
        var users = await _repo.Query()
            .ToListAsync();

        return _mapper.Map<List<UserReadDto>>(users);
    }

    // READ BY ID (DTO)
    public async Task<UserReadDto?> GetReadByIdAsync(int id)
    {
        var user = await _repo.Query()
            .FirstOrDefaultAsync(u => u.Id == id);

        return _mapper.Map<UserReadDto?>(user);
    }

    // CREATE
    public async Task<int> CreateAsync(UserCreateDto dto)
    {
        var entity = _mapper.Map<User>(dto);

        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        return entity.Id;
    }
}