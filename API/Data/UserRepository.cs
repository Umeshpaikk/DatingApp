using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DatatContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DatatContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedList<MemberDTO>> GetAllMembersAsync(UserParams userparams)
        {
            var query1 = _context.Users.AsQueryable();
            query1 = query1.Where(user => user.UserName != userparams.CurrentUsername);
            query1 = query1.Where(user => user.Gender == userparams.Gender);

            var minDate = DateTime.Today.AddYears(-userparams.MaxAge-1);
            var maxDate = DateTime.Today.AddYears(-userparams.MinAge);

            query1 = query1.Where(user => user.DateOfBirth >=  minDate && user.DateOfBirth <= maxDate);

            query1 = userparams.OrderBy switch
            {
                "created" => query1.OrderByDescending(user => user.Created),
                _ => query1.OrderByDescending(user => user.LastActive)
            };

            var query2 = query1.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).AsNoTracking();

            return await PagedList<MemberDTO>.CreateAsync(query2, userparams.PageNumber, userparams.PageSize);
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _context.Users
                                 .Include(p => p.Photos)
                                 .ToListAsync();
        }

        public async Task<MemberDTO> GetMemberByNameAsync(string Name)
        {
            return await _context.Users
                        .Where( x => x.UserName == Name)
                        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIDAsync(int Id)
        {
            return await _context.Users
                                 .Include(p => p.Photos)
                                 .SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<AppUser> GetUserByNameAsync(string Name)
        {
            return await _context.Users
                                 .Include(p => p.Photos)
                                 .SingleOrDefaultAsync(x => x.UserName == Name);
        }

        // public async Task<bool> SaveAllAsync()
        // {
        //     return await _context.SaveChangesAsync() > 0;
        // }

        public void update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}