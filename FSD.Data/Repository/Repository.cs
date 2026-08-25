using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FSD.Core.Entities;
using FSD.Data.Context;

namespace FSD.Data.Repository
{
    public class Repository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        // Tüm kayıtları getir
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // ID'ye göre tek kayıt getir
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // Yeni kayıt ekle
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        // Kayıt güncelle
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        // Kayıt sil
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        // Değişiklikleri kaydet (veritabanına yaz)
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}