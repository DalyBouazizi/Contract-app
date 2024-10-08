﻿using Projet_Data.Abstract;
using Projet_Data.Repo.Interfaces;
using Projet_Data.ModelsEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Projet_Data.Features;

namespace Projet_Data.Repo.Classes
{
    public class ContractRepository : IContractRepository
    {

        private readonly DataContext _context;
        private readonly IRepository<Contract> _repository;

        //constructor
        public ContractRepository(DataContext context , IRepository<Contract> repository) 
        {
            this._context = context;
            this._repository = repository;

        }

        public async Task<bool> AddContractAsync(Contract Contract)
        {
            try
            {
                return await _repository.AddEntity(Contract);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async  Task<bool> DeleteContractAsync( int IdContract)
        {
            try
            {
                var ContractToDelete = await GetContractByIdAsync(IdContract);
                if(ContractToDelete == null)
                {
                    return false;
                }
                else
                {
                    return await _repository.DeleteEntity(ContractToDelete);
                }
                
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ICollection<Contract>> GetAllContractsAsync()
        {
            try
            {
                var ListContracts =  await _repository.GetAllEntity();
                return ListContracts;

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }   
        }

        public async Task<Contract> GetContractByIdAsync(int IdContract)
        {
            Contract Contract;
            Contract = await _context.Contracts.Where(c => c.Idcontrat.Equals(IdContract)).FirstOrDefaultAsync();
            if (Contract == null)
            {
                Console.WriteLine("Contract not found.");
                return null;
            }
            else

            {
                Console.WriteLine("Contract not found.");
                Console.WriteLine(Contract);
                return Contract;
            }

           
        }

        public async Task<List<Contract>> GetContractByTypeAsync(string Type)
        {
            return await _context.Contracts.Where(e => e.Type == Type).ToListAsync();
        }

        public async Task<bool> UpdateContractAsync(Contract Contract)
        {
            try
            {
                await _repository.UpdateEntity(Contract);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ICollection<Contract>> GetContractsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Contracts
                    .Where(c => c.Datedeb >= startDate && c.DateFin <= endDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving contracts: {ex.Message}");
            }
        }

        public async Task<List<Contract>> GetContractByEmployeeIdAsync(int EmployeeId)
        {
            try
            {
                return await _context.Contracts.Where(e => e.EmployeeId == EmployeeId).ToListAsync();

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            {

            }
        }

        public async Task<Contract> GetLatestContractByEmployeeIdAsync(int employeeId)
        {
            
                return await _context.Contracts
                    .Where(c => c.EmployeeId == employeeId)
                    .OrderByDescending(c => c.DateFin)  // Assuming dateFin is the end date
                    .FirstOrDefaultAsync();
            
        }

        public async Task<ICollection<Contract>> GetLatestContractsAsync()
        {
            try
            {
                var latestContracts = await _context.Contracts
                    .GroupBy(c => c.EmployeeId)
                    .Select(g => g.OrderByDescending(c => c.DateFin).FirstOrDefault())
                    .ToListAsync();

                return latestContracts;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Contract>> GetContractsEndingInOneMonthAsync()
        {
            var nowDate = DateTime.Now;
            var endDate = nowDate.AddMonths(1);

            return await _context.Contracts
                .Where(c => c.DateFin >= nowDate && c.DateFin <= endDate)
                .ToListAsync();
        }

        public async Task<List<Contract>> GetContractsByFilterAsync(ContractFilterCriteria criteria)
        {
            var query = _context.Contracts.AsQueryable();

            try
            {
                // Apply type filter
                if (criteria.Types != null && criteria.Types.Any())
                {
                    var typesList = criteria.Types.SelectMany(t => t.Split(',')).Select(t => t.Trim()).ToList();
                    query = query.Where(c => typesList.Contains(c.Type));
                }

                // Apply active filter
                if (criteria.IsActive.HasValue && criteria.IsActive.Value)
                {
                    query = query.Where(c => c.DateFin > DateTime.Now);
                }

                // Apply ending soon filter
                if (criteria.IsEndingSoon.HasValue && criteria.IsEndingSoon.Value)
                {
                    var oneMonthFromNow = DateTime.Now.AddMonths(1);
                    query = query.Where(c => c.DateFin <= oneMonthFromNow && c.DateFin > DateTime.Now);
                }

                // Apply recently ended filter
                if (criteria.IsEndedRecently.HasValue && criteria.IsEndedRecently.Value)
                {
                    var oneMonthAgo = DateTime.Now.AddMonths(-1);
                    query = query.Where(c => c.DateFin <= DateTime.Now && c.DateFin >= oneMonthAgo);
                }

                // Apply ended over one month ago filter
                if (criteria.EndedOverOneMonth.HasValue && criteria.EndedOverOneMonth.Value)
                {
                    var moreThanOneMonthAgo = DateTime.Now.AddMonths(-1);
                    query = query.Where(c => c.DateFin < moreThanOneMonthAgo);
                }

                // Apply date range filters
                if (criteria.StartDate.HasValue)
                {
                    query = query.Where(c => c.Datedeb >= criteria.StartDate.Value);
                }

                if (criteria.EndDate.HasValue)
                {
                    query = query.Where(c => c.DateFin <= criteria.EndDate.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error filtering contracts: " + ex.Message);
            }

            return await query.ToListAsync();
        }

        //public async Task<bool> SignContractAsync(Contract contract)
        //{
        //    try
        //    {
        //        var res = await UpdateContractAsync(contract);
        //        if (res == null)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
    }
    }

