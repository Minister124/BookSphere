using AutoMapper;
using BookSphere.Data;
using BookSphere.DTOs;
using BookSphere.IServices;
using BookSphere.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSphere.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly BookSphereDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;


        public AnnouncementService(BookSphereDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<AnnouncementDto> CreateAnnouncementAsync(Guid? userId, CreateAnnouncementDto createAnnouncementDto)
        {
            // If userId is provided, check if the user is Admin or Staff
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }
                if (user.Role != "Admin" && user.Role != "Staff")
                {
                    // Allow system-generated (userId is null) or Admin/Staff to create
                    // This logic is slightly off if OrderService calls with Guid.Empty.
                    // Let's refine: if userId is Guid.Empty, treat as system.
                    if (userId.Value != Guid.Empty)
                    {
                         throw new UnauthorizedAccessException("User is not authorized to create announcements.");
                    }
                }
            }
            // If userId is null OR Guid.Empty, it's a system announcement (e.g. from OrderService)

            var announcement = _mapper.Map<Announcement>(createAnnouncementDto);
            announcement.Id = Guid.NewGuid();
            announcement.CreatedDate = DateTime.UtcNow;
            announcement.IsActive = true; // Default to active, can be changed by ToggleAnnouncementStatusAsync
            announcement.UserId = userId == Guid.Empty ? null : userId; // Store null if it's a system (Guid.Empty) or truly null call

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            return _mapper.Map<AnnouncementDto>(announcement);
        }

        public async Task<bool> DeleteAnnouncementAsync(Guid userId, Guid announcementId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (user.Role != "Admin" && user.Role != "Staff")
            {
                throw new UnauthorizedAccessException("User is not authorized to delete announcements.");
            }

            var announcement = await _context.Announcements.FindAsync(announcementId);
            if (announcement == null)
            {
                throw new KeyNotFoundException("Announcement not found.");
            }

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AnnouncementDto>> GetActiveAnnouncementsAsync()
        {
            var now = DateTime.UtcNow;
            var announcements = await _context.Announcements
                .Where(a => a.IsActive && a.StartDate <= now && a.EndDate >= now)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
            return _mapper.Map<List<AnnouncementDto>>(announcements);
        }

        // Helper for Admin/Staff to get all announcements, potentially for management.
        public async Task<PaginatedResponse<AnnouncementDto>> GetAllAnnouncementsAsync(int pageNumber, int pageSize, string? typeFilter)
        {
            var query = _context.Announcements.AsQueryable();

            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                query = query.Where(a => a.Type.ToLower() == typeFilter.ToLower());
            }

            query = query.OrderByDescending(a => a.CreatedDate);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var announcements = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            var announcementDtos = _mapper.Map<List<AnnouncementDto>>(announcements);

            return new PaginatedResponse<AnnouncementDto>
            {
                Items = announcementDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }


        public async Task<AnnouncementDto> GetAnnouncementByIdAsync(Guid announcementId) // Renamed to match interface
        {
            var announcement = await _context.Announcements.FindAsync(announcementId);
            if (announcement == null)
            {
                throw new KeyNotFoundException("Announcement not found.");
            }
            return _mapper.Map<AnnouncementDto>(announcement);
        }

        public async Task<List<AnnouncementDto>> GetAnnouncementsByTypeAsync(string type)
        {
            var now = DateTime.UtcNow;
            var announcements = await _context.Announcements
                .Where(a => a.Type.ToLower() == type.ToLower() && a.IsActive && a.StartDate <= now && a.EndDate >= now)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
            return _mapper.Map<List<AnnouncementDto>>(announcements);
        }

        public async Task<AnnouncementDto> ToggleAnnouncementStatusAsync(Guid userId, Guid announcementId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (user.Role != "Admin" && user.Role != "Staff")
            {
                throw new UnauthorizedAccessException("User is not authorized to toggle announcement status.");
            }

            var announcement = await _context.Announcements.FindAsync(announcementId);
            if (announcement == null)
            {
                throw new KeyNotFoundException("Announcement not found.");
            }

            announcement.IsActive = isActive;
            await _context.SaveChangesAsync();
            return _mapper.Map<AnnouncementDto>(announcement);
        }

        public async Task<AnnouncementDto> UpdateAnnouncementAsync(Guid userId, Guid announcementId, CreateAnnouncementDto updateAnnouncementDto)
        {
            // Check if the user is Admin or Staff
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (user.Role != "Admin" && user.Role != "Staff")
            {
                throw new UnauthorizedAccessException("User is not authorized to update announcements.");
            }

            var announcement = await _context.Announcements.FindAsync(announcementId);
            if (announcement == null)
            {
                throw new KeyNotFoundException("Announcement not found.");
            }

            // Use AutoMapper to update fields from DTO to existing entity
            // Ensure MappingProfile is configured if specific mappings are needed for update
            _mapper.Map(updateAnnouncementDto, announcement);
            //announcement.UserId = userId; // Record who last updated it, if desired. Model needs UpdatedByUserId field.

            await _context.SaveChangesAsync();
            return _mapper.Map<AnnouncementDto>(announcement);
        }
    }
}
