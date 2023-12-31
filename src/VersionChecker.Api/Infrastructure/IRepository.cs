﻿using VersionChecker.Api.Model;

namespace VersionChecker.Infrastructure
{
    public interface IRepository<T> where T : IVersionDetail
    {
        Task<IEnumerable<T>> GetAsync();
        Task<T> GetByAdditionalPropertyAsync(KeyValuePair<string, string> property);
    }
}