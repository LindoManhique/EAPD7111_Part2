using System.Collections.Generic;
using System.Linq;
using TechMoves.API.Data;
using TechMoves.API.Models;

namespace TechMoves.API.Repositories
{
    public class ContractRepository
    {
        private readonly AppDbContext _context;

        public ContractRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Contract> GetAll()
        {
            return _context.Contracts.ToList();
        }

        public Contract GetById(int id)
        {
            return _context.Contracts.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Contract contract)
        {
            _context.Contracts.Add(contract);
            _context.SaveChanges();
        }

        public void Update(Contract contract)
        {
            _context.Contracts.Update(contract);
            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}