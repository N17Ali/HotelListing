﻿using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using X.PagedList;

namespace HotelListing.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{ 
    private readonly DatabaseContext _context;
    private readonly DbSet<T> _db;

    public GenericRepository(DatabaseContext context)
    {
        _context = context;
        _db = _context.Set<T>();
    }
    
    public async Task Delete(int id)
    {
        var entity = await _db.FindAsync(id);
        _db.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _db.RemoveRange(entities);
    }

    public async Task<T> Get(Expression<Func<T, bool>> expressio, List<string> includes = null)
    {
        IQueryable<T> query = _db;
        if (includes != null && includes.Count > 0)
        {
            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }
        }
        return await query.AsNoTracking().FirstOrDefaultAsync(expressio);
    }

    public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> ordeyBy = null, List<string> includes = null)
    {
        IQueryable<T> query = _db;
        if (expression != null)
        {
            query = query.Where(expression);
        }
        if (includes != null && includes.Count > 0)
        {
            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }
        }
        if (ordeyBy != null)
        {
            query = ordeyBy(query);
        }
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IPagedList<T>> GetPagedList(RequestParams requestParams, List<string> includes = null)
    {
        IQueryable<T> query = _db;

        if (includes != null && includes.Count > 0)
        {
            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.AsNoTracking().ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);
    }

    public async Task Insert(T entity)
    {
        await _db.AddAsync(entity);
    }

    public async Task InsertRange(IEnumerable<T> entities)
    {
        await _db.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        _db.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
}
